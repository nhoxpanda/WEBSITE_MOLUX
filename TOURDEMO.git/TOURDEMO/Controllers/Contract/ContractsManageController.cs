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
    public class ContractsManageController : BaseController
    {
        //
        // GET: /ContractsManage/

        #region Init

        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
        private IGenericRepository<tbl_Contract> _contractRepository;
        private IGenericRepository<tbl_Tags> _tagRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;
        private DataContext _db;

        public ContractsManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IGenericRepository<tbl_Contract> contractRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Tags> tagRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._updateHistoryRepository = updateHistoryRepository;
            this._customerRepository = customerRepository;
            this._staffRepository = staffRepository;
            this._contractRepository = contractRepository;
            this._documentFileRepository = documentFileRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._tagRepository = tagRepository;
            this._tourRepository = tourRepository;
            _db = new DataContext();
        }

        #endregion

        #region List
        int SDBID = 6;
        int maPB = 0, maNKD = 0, maNV = 0, maCN = 0;
        void Permission(int PermissionsId, int formId)
        {
            var list = _db.tbl_ActionData.Where(p => p.FormId == formId && p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);
            ViewBag.IsExport = list.Contains(5);

            //cập nhật trạng thái
            var listUS = _db.tbl_ActionData.Where(p => p.FormId == 20 & p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsUpdateStatus = list.Contains(1);

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

        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 20);

            if (SDBID == 6)
                return View(new List<tbl_Contract>());

            var model = _contractRepository.GetAllAsQueryable().AsEnumerable()
                    .Where(p => ((p.StaffId == maNV | maNV == 0)
                        || ((p.TourId != null) && (p.tbl_Tour.Permission != null && p.tbl_Tour.Permission.Contains(maNV.ToString())
                        || p.tbl_Tour.StaffId == maNV || p.tbl_Tour.CreateStaffId == maNV)))
                        & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                        & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                        & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false))
                    .OrderByDescending(p => p.CreatedDate)
                    .ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult GetIdContract(int id)
        {
            Session["idContract"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Create
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(tbl_Contract model, FormCollection form, HttpPostedFileBase fileUpload)
        {
            Permission(clsPermission.GetUser().PermissionID, 20);
            try
            {
                var checkCode = _contractRepository.GetAllAsQueryable().FirstOrDefault(p => p.Code == model.Code && p.IsDelete == false);
                if (checkCode == null)
                {
                    model.CreatedDate = DateTime.Now;
                    model.ModifiedDate = DateTime.Now;
                    model.Permission = clsPermission.GetUser().StaffID.ToString();
                    model.DictionaryId = 28;
                    model.StaffId = clsPermission.GetUser().StaffID;
                    if (model.TongDuKien != null)
                    {
                        model.LoiNhuanDuKien = model.TotalPrice - model.TongDuKien;
                        model.CurrencyLNDK = model.CurrencyTDK;
                    }
                    if (form["TagsId"] != null && form["TagsId"] != "")
                    {
                        model.TagsId = form["TagsId"].ToString();
                    }
                    if (fileUpload != null)
                    {

                        string FileSize = Common.ConvertFileSize(fileUpload.ContentLength);
                        String newName = fileUpload.FileName.Insert(fileUpload.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                        String path = Server.MapPath("~/Upload/file/" + newName);
                        fileUpload.SaveAs(path);
                        model.FileName = newName;

                    }
                    if (await _contractRepository.Create(model))
                    {
                        UpdateHistory.SaveHistory(20, "Thêm mới hợp đồng: " + model.Code + " - " + model.Name,
                            null, //appointment
                            model.Id, //contract
                            model.CustomerId, //customer
                            null, //partner
                            null, //program
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
            }
            catch { }

            return RedirectToAction("Index");
        }
        #endregion

        #region Update
        [HttpPost]
        public ActionResult ContractInfomation(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 20);
            var model = _db.tbl_Contract.Find(id);
            if (clsPermission.GetUser().StaffID == 9 || model.StatusContractId <= 1155)
            {
                return PartialView("_Partial_EditContract", model);
            }
            else
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tbl_Contract model, FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 20);
            try
            {
                model.ModifiedDate = DateTime.Now;
                model.StaffId = clsPermission.GetUser().StaffID;
                model.TagsId = "";
                if (model.TongDuKien != null)
                {
                    model.LoiNhuanDuKien = model.TotalPrice - model.TongDuKien;
                    model.CurrencyLNDK = model.CurrencyTDK;
                }
                if (await _contractRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(20, "Cập nhật hợp đồng: " + model.Code,
                            null, //appointment
                            model.Id, //contract
                            model.CustomerId, //customer
                            null, //partner
                            null, //program
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
            Permission(clsPermission.GetUser().PermissionID, 20);
            try
            {
                if (fc["listItemId"] != null && fc["listItemId"] != "")
                {
                    var listIds = fc["listItemId"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        foreach (string id in listIds)
                        {
                            var update = _db.tbl_UpdateHistory.AsEnumerable().FirstOrDefault(p => p.ContractId.ToString() == id);
                            if (update != null)
                            {
                                _db.tbl_UpdateHistory.Remove(update);
                            }
                            ////
                            var contract = _contractRepository.FindId(Convert.ToInt32(id));
                            UpdateHistory.SaveHistory(20, "Xóa hợp đồng: " + contract.Code + " - " + contract.Name,
                                null, //appointment
                                contract.Id, //contract
                                contract.CustomerId, //customer
                                null, //partner
                                null, //program
                                null, //task
                                contract.TourId, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                null // ticket
                                );
                        }

                        if (await _contractRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "ContractsManage") }, JsonRequestBehavior.AllowGet);
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
                Session["ContractFile"] = FileName;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateDocument(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 69);
                string id = Session["idContract"].ToString();
                if (ModelState.IsValid)
                {
                    model.Code = GenerateCode.DocumentCode();
                    model.ContractId = Convert.ToInt32(id);
                    model.CreatedDate = DateTime.Now;
                    model.IsRead = false;
                    model.ModifiedDate = DateTime.Now;
                    if (form["TagsId"] != null)
                    {
                        model.TagsId = form["TagsId"].ToString();
                    }
                    model.StaffId = clsPermission.GetUser().StaffID;
                    //file
                    HttpPostedFileBase FileName = Session["ContractFile"] as HttpPostedFileBase;
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
                        UpdateHistory.SaveHistory(69, "Thêm mới tài liệu: " + model.Code + " - " + model.FileName,
                            null, //appointment
                            model.ContractId, //contract
                            model.CustomerId, //customer
                            model.PartnerId, //partner
                            model.ProgramId, //program
                            model.TaskId, //task
                            model.TourId, //tour
                            null, //quotation
                            model.Id, //document
                            null, //history
                            null // ticket
                            );

                        Session["ContractFile"] = null;
                        //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.CustomerId.ToString() == id).ToList();
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ContractId.ToString() == id).Where(p => p.IsDelete == false)
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
                        return PartialView("~/Views/ContractTabInfo/_TaiLieuMau.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/ContractTabInfo/_TaiLieuMau.cshtml");
                    }
                }
            }
            catch { }
            return PartialView("~/Views/ContractTabInfo/_TaiLieuMau.cshtml");
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
                Permission(clsPermission.GetUser().PermissionID, 69);
                if (ModelState.IsValid)
                {
                    model.IsRead = true;
                    model.ModifiedDate = DateTime.Now;
                    if (form["TagsId"] != null && form["TagsId"] != "")
                    {
                        model.TagsId = form["TagsId"].ToString();
                    }
                    if (Session["ContractFile"] != null && form["ContractFile"] != "")
                    {
                        //file
                        HttpPostedFileBase FileName = Session["ContractFile"] as HttpPostedFileBase;
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
                        UpdateHistory.SaveHistory(69, "Cập nhật tài liệu hợp đồng: " + model.Code,
                            null, //appointment
                            model.ContractId, //contract
                            model.CustomerId, //customer
                            model.PartnerId, //partner
                            model.ProgramId, //program
                            model.TaskId, //task
                            model.TourId, //tour
                            null, //quotation
                            model.Id, //document
                            null, //history
                            null // ticket
                            );

                        Session["ContractFile"] = null;
                        //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ContractId == model.ContractId).ToList();
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ContractId == model.ContractId).Where(p => p.IsDelete == false)
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
                        return PartialView("~/Views/ContractTabInfo/_TaiLieuMau.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/ContractTabInfo/_TaiLieuMau.cshtml");
                    }
                }
            }
            catch
            {
            }

            return PartialView("~/Views/ContractTabInfo/_TaiLieuMau.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteDocument(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 69);
                int conId = _documentFileRepository.FindId(id).ContractId ?? 0;

                //file
                tbl_DocumentFile documentFile = _documentFileRepository.FindId(id) ?? new tbl_DocumentFile();
                String path = Server.MapPath("~/Upload/file/" + documentFile.FileName);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                //end file
                var listId = id.ToString().Split(',').ToArray();
                if (await _documentFileRepository.DeleteMany(listId, false))
                {
                    // var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.CustomerId == cusId).ToList();
                    var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ContractId == conId).Where(p => p.IsDelete == false)
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
                    return PartialView("~/Views/ContractTabInfo/_TaiLieuMau.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/ContractTabInfo/_TaiLieuMau.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/ContractTabInfo/_TaiLieuMau.cshtml");
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
            var contracts = _contractRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false)
                .Select(c => new tbl_Contract
                {
                    Code = c.Code,
                    ContractDate = c.ContractDate,
                    Name = c.Name,
                    tbl_Customer = c.tbl_Customer,
                    tbl_Staff = c.tbl_Staff,
                    StartDate = c.StartDate,
                    NumberDay = c.NumberDay,
                    tbl_DictionaryStatus = c.tbl_DictionaryStatus,
                    tbl_Dictionary = c.tbl_Dictionary,
                    Note = c.Note,
                    TotalPrice = c.TotalPrice,
                    CreatedDate = c.CreatedDate,
                    ModifiedDate = c.ModifiedDate,
                    TourId = c.TourId,
                    tbl_Tour = c.tbl_Tour,
                    TongDuKien = c.TongDuKien,
                    LoiNhuanDuKien = c.LoiNhuanDuKien
                }).ToList();
            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    ExportCustomersToXlsx(stream, contracts);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "Contracts.xlsx");
            }
            catch (Exception ex)
            {
                string ms = ex.Message;
            }
            return RedirectToAction("Index");
        }

        public virtual void ExportCustomersToXlsx(Stream stream, IList<tbl_Contract> contracts)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {

                var worksheet = xlPackage.Workbook.Worksheets.Add("Contracts");

                var properties = new[]
                    {
                        "Mã hợp đồng",
                        "Ngày ký",
                        "Khách hàng",
                        "Điện thoại",
                        "Code tour",
                        "Tên tour",
                        "Nhân viên quản lý",
                        "Ngày hiệu lực",
                        "Thời hạn",
                        "Tình trạng",
                        "Phân loại",
                        "Diễn giải",
                        "Tổng giá trị",
                        "Tổng chi phí dự kiến",
                        "Lợi nhuận dự kiến",
                        "Người tạo",
                        "Ngày tạo",
                    };

                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i];
                }


                int row = 1;
                foreach (var contract in contracts)
                {
                    row++;
                    int col = 1;

                    worksheet.Cells[row, col].Value = contract.Code;
                    col++;

                    worksheet.Cells[row, col].Value = contract.ContractDate != null ? contract.ContractDate.Value.ToString("d/M/yyyy") : "";
                    col++;

                    worksheet.Cells[row, col].Value = contract.tbl_Customer != null ? contract.tbl_Customer.FullName : "";
                    col++;

                    worksheet.Cells[row, col].Value = contract.tbl_Customer != null ? contract.tbl_Customer.Phone : "";
                    col++;

                    worksheet.Cells[row, col].Value = contract.tbl_Tour != null ?  contract.tbl_Tour.Code : "";
                    col++;

                    worksheet.Cells[row, col].Value = contract.tbl_Tour != null ? contract.tbl_Tour.Name : "";
                    col++;
                    
                    worksheet.Cells[row, col].Value = contract.tbl_Staff.FullName;
                    col++;

                    worksheet.Cells[row, col].Value = contract.StartDate != null ? contract.StartDate.Value.ToString("d/M/yyyy") : "";
                    col++;

                    worksheet.Cells[row, col].Value = contract.NumberDay;
                    col++;

                    worksheet.Cells[row, col].Value = contract.tbl_DictionaryStatus.Name;
                    col++;

                    worksheet.Cells[row, col].Value = contract.tbl_Dictionary.Name;
                    col++;

                    worksheet.Cells[row, col].Value = contract.Note == null ? "" : HttpUtility.HtmlDecode(contract.Note.Replace("<p>","").Replace("</p>",""));
                    col++;

                    worksheet.Cells[row, col].Value = string.Format("{0:0,0}", contract.TotalPrice).Replace(',','.');
                    col++;

                    worksheet.Cells[row, col].Value = string.Format("{0:0,0}", contract.TongDuKien).Replace(',', '.');
                    col++;

                    worksheet.Cells[row, col].Value = string.Format("{0:0,0}", contract.LoiNhuanDuKien).Replace(',', '.');
                    col++;

                    worksheet.Cells[row, col].Value = contract.tbl_Staff.FullName;
                    col++;

                    worksheet.Cells[row, col].Value = contract.CreatedDate.ToString("d/M/yyyy");
                    col++;

                }
                worksheet.Cells["a1:q" + row].Style.Font.SetFromFont(new Font("Tahoma", 8));

                worksheet.Cells["a1:q1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["a1:q1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["a1:q1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));

                worksheet.Cells["a1:q" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a1:q" + row].Style.Border.Top.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["a1:q" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a1:q" + row].Style.Border.Left.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["a1:q" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a1:q" + row].Style.Border.Bottom.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["a1:q" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a1:q" + row].Style.Border.Right.Color.SetColor(Color.FromArgb(169, 169, 169));

                row++;

                worksheet.Cells["a" + row + ":q" + row].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["a" + row + ":q" + row].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));

                worksheet.Cells["a1:q" + row].AutoFitColumns();

                xlPackage.Save();
            }
        }
        #endregion

        #region Update Status

        public ActionResult GetInfoStatus(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 20);
            var model = _contractRepository.FindId(id);
            return PartialView("_Partial_UpdateStatus", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateStatusContract(tbl_Contract model)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 20);
                model.ModifiedDate = DateTime.Now;
                if (await _contractRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(20, "Cập nhật tình trạng hợp đồng: " + model.Code,
                            null, //appointment
                            model.Id, //contract
                            model.CustomerId, //customer
                            null, //partner
                            null, //program
                            null, //task
                            model.TourId, //tour
                            null, //quotation
                            null, //document
                            null, //history
                            null // ticket
                            );
                }
            }
            catch { }

            return Json(JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
