using CRM.Core;
using CRM.Infrastructure;
using TOURDEMO.Models;
using TOURDEMO.Utilities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TOURDEMO.Controllers
{
    [Authorize]
    public class ProgramsManageController : BaseController
    {
        //
        // GET: /ProgramsManage/

        #region Init

        private IGenericRepository<tbl_Program> _programRepository;
        private IGenericRepository<tbl_Tags> _tagRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;
        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
        private DataContext _db;

        public ProgramsManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Program> programRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_Tags> tagRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._tourRepository = tourRepository;
            this._programRepository = programRepository;
            this._documentFileRepository = documentFileRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._tagRepository = tagRepository;
            this._staffRepository = staffRepository;
            this._updateHistoryRepository = updateHistoryRepository;
            _db = new DataContext();
        }

        #endregion

        #region Permisison
        int SDBID = 6;
        int maPB = 0, maNKD = 0, maNV = 0, maCN = 0;
        void Permission(int PermissionsId, int formId)
        {
            var list = _db.tbl_ActionData.Where(p => p.FormId == formId && p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);
            ViewBag.IsImport = list.Contains(4);
            ViewBag.IsExport = list.Contains(5);
            ViewBag.IsLock = list.Contains(6);
            ViewBag.IsUnLock = list.Contains(7);

            var ltAccess = _db.tbl_AccessData.Where(p => p.PermissionId == PermissionsId && p.FormId == formId).Select(p => p.ShowDataById).FirstOrDefault();
            if (ltAccess != 0)
                this.SDBID = ltAccess;

            switch (SDBID)
            {
                case 2: maPB = clsPermission.GetUser().DepartmentID;
                    maCN = clsPermission.GetUser().BranchID;
                    break;
                case 3: maNKD = clsPermission.GetUser().GroupID;
                    maCN = clsPermission.GetUser().BranchID; break;
                case 4: maNV = clsPermission.GetUser().StaffID; break;
                case 5: maCN = clsPermission.GetUser().BranchID; break;
            }
        }
        #endregion

        #region GetIdProgram
        [HttpPost]
        public ActionResult GetIdProgram(int id)
        {
            Session["idProgram"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region List
        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 18);

            if (SDBID == 6)
                return View(new List<tbl_Program>());

            var model = _programRepository.GetAllAsQueryable()
                .Where(p => ((p.StaffId == maNV | maNV == 0)
                    || ((p.TourId != null) && (p.tbl_Tour.Permission != null && p.tbl_Tour.Permission.Contains(maNV.ToString())
                    || p.tbl_Tour.StaffId == maNV || p.tbl_Tour.CreateStaffId == maNV)))
                    & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                    & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                    & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0)
                    & (p.IsDelete == false)).OrderByDescending(p => p.CreatedDate).ToList();
            return View(model);
        }
        #endregion

        #region Create
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(tbl_Program model, FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 18);
            try
            {
                model.Code = GenerateCode.ProgramCode();
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.Permission = "";
                model.DictionaryId = 30;
                model.StaffId = clsPermission.GetUser().StaffID;
                if (model.TourId != null)
                {
                    var tour = _tourRepository.FindId(model.TourId);
                    model.CustomerId = tour.CustomerId;
                    model.TagsId = tour.DestinationPlace.ToString();
                    model.StartDate = tour.StartDate;
                    model.EndDate = tour.EndDate;
                    model.NumberDay = tour.NumberDay;
                }
                model.TotalPrice = 0;
                model.CurrencyId = 1209;

                if (await _programRepository.Create(model))
                {
                    UpdateHistory.SaveHistory(18, "Thêm mới chương trình, code: " + model.Code + " - " + model.Name,
                        null, //appointment
                        null, //contract
                        model.CustomerId, //customer
                        null, //partner
                        model.Id, //program
                        null, //task
                        model.TourId, //tour
                        null, //quotation
                        null, //document
                        null, //history
                        null // ticket
                        );
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch { }

            return RedirectToAction("Index");
        }
        #endregion

        #region Update

        [HttpPost]
        public ActionResult ProgramInfomation(int id)
        {
            var model = _db.tbl_Program.Find(id);
            if (clsPermission.GetUser().StaffID == 9 || model.DictionaryId <= 1147)
            {
                return PartialView("_Partial_EditProgram", model);
            }
            else
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tbl_Program model, FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 18);
            try
            {
                model.ModifiedDate = DateTime.Now;
                model.StaffId = clsPermission.GetUser().StaffID;
                if (model.TourId != null)
                {
                    var tour = _tourRepository.FindId(model.TourId);
                    model.CustomerId = tour.CustomerId;
                    model.TagsId = tour.DestinationPlace.ToString();
                    model.StartDate = tour.StartDate;
                    model.EndDate = tour.EndDate;
                    model.NumberDay = tour.NumberDay;
                }
                model.TotalPrice = 0;
                model.CurrencyId = 1209;
                if (await _programRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(18, "Cập nhật chương trình: " + model.Code,
                        null, //appointment
                        null, //contract
                        model.CustomerId, //customer
                        null, //partner
                        model.Id, //program
                        null, //task
                        model.TourId, //tour
                        null, //quotation
                        null, //document
                        null, //history
                        null // ticket
                        );
                    return RedirectToAction("Index");
                }
            }
            catch { }

            return RedirectToAction("Index");
        }
        #endregion

        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(FormCollection fc)
        {
            Permission(clsPermission.GetUser().PermissionID, 18);
            try
            {
                if (fc["listItemId"] != null && fc["listItemId"] != "")
                {
                    var listIds = fc["listItemId"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        //
                        foreach (var i in listIds)
                        {
                            var item = _programRepository.FindId(Convert.ToInt32(i));
                            UpdateHistory.SaveHistory(18, "Xóa chương trình: " + item.Code + " - " + item.Name,
                                null, //appointment
                                null, //contract
                                item.CustomerId, //customer
                                null, //partner
                                item.Id, //program
                                null, //task
                                item.TourId, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                null // ticket
                            );
                        }
                        //
                        if (await _programRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "ProgramsManage") }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Xóa dữ liệu thất bại !" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Vui lòng chọn những mục cần xóa !" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Document
        /********** Quản lý tài liệu ************/

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase FileName)
        {
            if (FileName != null && FileName.ContentLength > 0)
            {
                Session["ProgramFile"] = FileName;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateDocument(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 66);
                string id = Session["idProgram"].ToString();
                if (ModelState.IsValid)
                {
                    model.Code = GenerateCode.DocumentCode();
                    model.ProgramId = Convert.ToInt32(id);
                    model.CreatedDate = DateTime.Now;
                    model.IsRead = false;
                    model.ModifiedDate = DateTime.Now;
                    model.DictionaryId = 30;
                    if (form["TagsId"] != null && form["TagsId"] != "")
                    {
                        model.TagsId = form["TagsId"].ToString();
                    }
                    model.StaffId = clsPermission.GetUser().StaffID;
                    //file
                    HttpPostedFileBase FileName = Session["ProgramFile"] as HttpPostedFileBase;
                    string CustomerFileSize = Common.ConvertFileSize(FileName.ContentLength);
                    String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    FileName.SaveAs(path);
                    //end file
                    if (newName != null && CustomerFileSize != null)
                    {
                        model.FileName = newName;
                        model.FileSize = CustomerFileSize;
                    }

                    if (await _documentFileRepository.Create(model))
                    {
                        UpdateHistory.SaveHistory(66, "Thêm mới tài liệu, code: " + model.Code + " - " + model.FileName,
                            null, //appointment
                            model.ContractId, //contract
                            model.CustomerId, //customer
                            model.PartnerId, //partner
                            model.Id, //program
                            model.TaskId, //task
                            model.TourId, //tour
                            null, //quotation
                            model.Id, //document
                            null, //history
                            null // ticket
                        );

                        Session["ProgramFile"] = null;
                        //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ProgramId.ToString() == id).ToList();
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ProgramId.ToString() == id).Where(p => p.IsDelete == false)
                     .Select(p => new tbl_DocumentFile
                     {
                         Id = p.Id,
                         FileName = p.FileName,
                         FileSize = p.FileSize,
                         Note = p.Note,
                         CreatedDate = p.CreatedDate,
                         TagsId = p.TagsId,
                         tbl_Staff = _staffRepository.FindId(p.StaffId)
                     }).ToList();
                        return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml");
                    }
                }
            }
            catch { }
            return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml");
        }

        //[ChildActionOnly]
        //public ActionResult _Partial_EditDocument()
        //{
        //    List<SelectListItem> lstTag = new List<SelectListItem>();
        //    List<SelectListItem> lstDictionary = new List<SelectListItem>();
        //    ViewData["TagsId"] = lstTag;
        //    ViewBag.DictionaryId = lstDictionary;
        //    return PartialView("_Partial_EditDocument", new tbl_DocumentFile());
        //}

        [HttpPost]
        public async Task<ActionResult> EditInfoDocument(int id)
        {
            var model = await _documentFileRepository.GetById(id);
            ViewBag.DictionaryId = new SelectList(_dictionaryRepository.GetAllAsQueryable().Where(p => p.DictionaryCategoryId == 1 && p.IsDelete == false), "Id", "Name", model.DictionaryId);
            return PartialView("_Partial_EditDocument", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateDocument(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 66);
                if (ModelState.IsValid)
                {
                    model.IsRead = false;
                    model.DictionaryId = 30;
                    model.ModifiedDate = DateTime.Now;
                    if (form["TagsId"] != null && form["TagsId"] != "")
                    {
                        model.TagsId = form["TagsId"].ToString();
                    }
                    if (Session["ProgramFile"] != null)
                    {
                        //file
                        HttpPostedFileBase FileName = Session["ProgramFile"] as HttpPostedFileBase;
                        string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                        String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                        String path = Server.MapPath("~/Upload/file/" + newName);
                        FileName.SaveAs(path);
                        //end file

                        if (FileName != null && FileSize != null)
                        {
                            String pathOld = Server.MapPath("~/Upload/file/" + model.FileName);
                            if (System.IO.File.Exists(pathOld))
                                System.IO.File.Delete(pathOld);
                            model.FileName = newName;
                            model.FileSize = FileSize;
                        }
                    }
                    if (await _documentFileRepository.Update(model))
                    {
                        UpdateHistory.SaveHistory(66, "Cập nhật tài liệu của chương trình: " + model.Code,
                            null, //appointment
                            model.ContractId, //contract
                            model.CustomerId, //customer
                            model.PartnerId, //partner
                            model.Id, //program
                            model.TaskId, //task
                            model.TourId, //tour
                            null, //quotation
                            model.Id, //document
                            null, //history
                            null // ticket
                            );
                        Session["ProgramFile"] = null;
                        //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ProgramId == model.ProgramId).ToList();
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ProgramId == model.ProgramId).Where(p => p.IsDelete == false)
                     .Select(p => new tbl_DocumentFile
                     {
                         Id = p.Id,
                         FileName = p.FileName,
                         FileSize = p.FileSize,
                         Note = p.Note,
                         CreatedDate = p.CreatedDate,
                         TagsId = p.TagsId,
                         tbl_Staff = _staffRepository.FindId(p.StaffId)
                     }).ToList();
                        return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml");
                    }
                }
            }
            catch
            {
            }

            return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteDocument(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 66);
                int proId = _documentFileRepository.FindId(id).ProgramId ?? 0;
                //file
                tbl_DocumentFile documentFile = _documentFileRepository.FindId(id) ?? new tbl_DocumentFile();
                String path = Server.MapPath("~/Upload/file/" + documentFile.FileName);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                //end file

                var listId = id.ToString().Split(',').ToArray();
                //
                var item = _documentFileRepository.FindId(id);
                UpdateHistory.SaveHistory(66, "Xóa tài liệu: " + item.Code,
                    null, //appointment
                    item.ContractId, //contract
                    item.CustomerId, //customer
                    item.PartnerId, //partner
                    item.Id, //program
                    item.TaskId, //task
                    item.TourId, //tour
                    null, //quotation
                    item.Id, //document
                    null, //history
                    null // ticket
                    );
                //
                if (await _documentFileRepository.DeleteMany(listId, false))
                {
                    //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ProgramId == proId).ToList();
                    var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ProgramId == proId).Where(p => p.IsDelete == false)
                     .Select(p => new tbl_DocumentFile
                     {
                         Id = p.Id,
                         FileName = p.FileName,
                         FileSize = p.FileSize,
                         Note = p.Note,
                         CreatedDate = p.CreatedDate,
                         TagsId = p.TagsId,
                         tbl_Staff = _staffRepository.FindId(p.StaffId)
                     }).ToList();
                    return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml");
            }
        }

        #endregion

        #region Export
        /// <summary>
        /// Export file excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportFile()
        {
            var programs = _programRepository.GetAllAsQueryable().Where(p => p.IsDelete == false).ToList();

            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    ExportCustomersToXlsx(stream, programs);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "Programs.xlsx");
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Index");
        }


        public virtual void ExportCustomersToXlsx(Stream stream, IList<tbl_Program> programs)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {

                var worksheet = xlPackage.Workbook.Worksheets.Add("Programs");

                var properties = new[]
                    {
                        "Mã chương trình",
                        "Tên chương trình",
                        "Tour",
                        "Khách hàng",
                        "Địa điểm",
                        "Ngày bắt đầu",
                        "Ngày kết thúc",
                        "Số ngày",
                        "Tổng giá trị",
                        "Ghi chú",
                        "Người nhập",
                        "Ngày nhập"
                    };

                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i];
                }

                int row = 1;
                foreach (var program in programs)
                {
                    row++;
                    int col = 1;

                    worksheet.Cells[row, col].Value = program.Code;
                    col++;

                    worksheet.Cells[row, col].Value = program.Name == null ? "" : program.Name;
                    col++;

                    worksheet.Cells[row, col].Value = program.TourId != null ? program.tbl_Tour.Name : "";
                    col++;

                    worksheet.Cells[row, col].Value = program.CustomerId != null ? program.tbl_Customer.FullName : "";
                    col++;

                    worksheet.Cells[row, col].Value = program.TagsId != null ? LoadData.LocationTags(program.TagsId) : "";
                    col++;

                    worksheet.Cells[row, col].Value = program.StartDate != null ? program.StartDate.Value.ToString("dd/MM/yyyy") : "";
                    col++;

                    worksheet.Cells[row, col].Value = program.EndDate != null ? program.EndDate.Value.ToString("dd/MM/yyyy") : "";
                    col++;

                    worksheet.Cells[row, col].Value = program.NumberDay;
                    col++;

                    worksheet.Cells[row, col].Value = program.TotalPrice;
                    col++;

                    worksheet.Cells[row, col].Value = program.Note == null ? "" : HttpUtility.HtmlDecode(Regex.Replace(program.Note, "<.*?>", String.Empty));
                    col++;

                    worksheet.Cells[row, col].Value = program.StaffId != null ? program.tbl_Staff.FullName : "";
                    col++;

                    worksheet.Cells[row, col].Value = program.CreatedDate != null ?  program.CreatedDate.ToString("dd/MM/yyyy") : "";
                    col++;

                }
                worksheet.Cells["A1:L" + row].Style.Font.SetFromFont(new Font("Arial", 12));

                worksheet.Cells["A1:L1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["A1:L1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:L1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));

                row++;

                worksheet.Cells["A1:L" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:L" + row].Style.Border.Top.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["A1:L" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:L" + row].Style.Border.Left.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["A1:L" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:L" + row].Style.Border.Bottom.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["A1:L" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:L" + row].Style.Border.Right.Color.SetColor(Color.FromArgb(169, 169, 169));


                worksheet.Cells["A" + row + ":L" + row].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A" + row + ":L" + row].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
                worksheet.Cells["C" + row + ":H" + row].Merge = true;
                worksheet.Cells["B" + row].Value = row - 2;
                worksheet.Cells["I" + row].Formula = String.Format("=SUM(I2:I{0})", row - 1);
                worksheet.Cells["I2:I" + row].Style.Numberformat.Format = "#,#";

                worksheet.Cells["A1:L" + row].AutoFitColumns();

                xlPackage.Save();
            }
        }
        #endregion

        #region Status
        public async Task<ActionResult> UpdateStatus(tbl_UpdateHistory model)
        {
            try
            {
                int id = Convert.ToInt32(Session["idProgram"]);
                model.CreatedDate = DateTime.Now;
                model.ProgramId = id;
                model.StaffId = clsPermission.GetUser().StaffID;
                model.IsDelete = false;
                model.Note = "Cập nhật trạng thái: " + _dictionaryRepository.FindId(model.DictionaryId).Name;
                if (await _updateHistoryRepository.Create(model))
                {
                    UpdateHistory.SaveHistory(18, "Cập nhật trạng thái của chương trình: " + _programRepository.FindId(id).Name,
                            null, //appointment
                            null, //contract
                            null, //customer
                            null, //partner
                            model.ProgramId, //program
                            null, //task
                            null, //tour
                            null, //quotation
                            null, //document
                            null, //history
                            null // ticket
                            );
                }

                // cập nhật tình trạng
                var item = _db.tbl_Program.Find(id);
                item.StatusId = model.DictionaryId;
                _db.SaveChanges();
            }
            catch
            {
            }
            return RedirectToAction("Index");
        }
        #endregion

    }
}
