using CRM.Core;
using CRM.Enum;
using CRM.Infrastructure;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TOURDEMO.Models;
using TOURDEMO.Utilities;

namespace TOURDEMO.Controllers.Staff
{
    public class CandidateManageController : BaseController
    {
        private DataContext _db;
        private IGenericRepository<tbl_Candidate> _candidateRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;
        public CandidateManageController(
            IGenericRepository<tbl_Candidate> candidateRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._candidateRepository = candidateRepository;
            this._tagsRepository = tagsRepository;
            _db = new DataContext();
        }


        #region Phân quyền
        void Permission(int PermissionsId, int formId)
        {
            var list = _db.tbl_ActionData.Where(p => p.FormId == formId && p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);
            ViewBag.IsImport = list.Contains(4);
            ViewBag.IsExport = list.Contains(5);
        }

        #endregion
        // GET: CandidateManage
        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 1132);
            var candidatesList = new CandidateViewModel();

            var candidateDetailList = _candidateRepository.GetAllAsQueryable().Where(k => k.IsDelete == false)
                .Select(p => new CandidateDetailViewModel()
                {
                    Id = p.Id,
                    Address = p.Address,
                    ApplyDate = p.ApplyDate ?? DateTime.Now,
                    Birthday = p.Birthday ?? DateTime.Now,
                    Birthplace = p.Birthplace ?? 0,
                    CreatedDateIdentity = p.CreatedDateIdentity ?? DateTime.Now,
                    Email = p.Email,
                    Fullname = p.FullName,
                    Gender = p.Gender,
                    IdentityCard = p.IdentityCard,
                    IdentityTagId = p.IdentityTagId ?? 0,
                    ImageLink = p.Image,
                    InformationTechnology = p.InformationTechnology,
                    Phone = p.Phone,
                    HeadQuarterName=p.HeadquarterId??0
                }).ToList();
            candidatesList.listCandidates = candidateDetailList;
            candidatesList.listTags = LoadData.TinhThanhQuocGia().ToList();
            return View(candidatesList);
        }
        [HttpPost]
        public async Task<ActionResult> Create(CandidateViewModel model, FormCollection form, HttpPostedFileBase Image)
        {
            Permission(clsPermission.GetUser().PermissionID, 1132);
            try
            {
                model.candidateDetail.Code = GenerateCode.CandidateCode();
                model.candidateDetail.IsDelete = false;
                model.candidateDetail.StaffId = clsPermission.GetUser().StaffID;
                if (Image != null)
                {
                    //file
                    string FileSize = Common.ConvertFileSize(Image.ContentLength);
                    String newName = Image.FileName.Insert(Image.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    Image.SaveAs(path);
                    model.candidateDetail.Image = newName;
                    //end file
                }

                if (model.candidateDetail.CreatedDateIdentity != null && model.candidateDetail.CreatedDateIdentity.Value.Year >= 1980)
                {
                    model.candidateDetail.CreatedDateIdentity = model.candidateDetail.CreatedDateIdentity;
                }


                if (await _candidateRepository.Create(model.candidateDetail))
                {

                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch { }
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(CandidateViewModel model, FormCollection form, HttpPostedFileBase Image)
        {
            Permission(clsPermission.GetUser().PermissionID, 6);
            try
            {
                model.candidateDetail.ModifiedDate = DateTime.Now;
                model.candidateDetail.StaffId = clsPermission.GetUser().StaffID;
                if (Image != null)
                {
                    string FileSize = Common.ConvertFileSize(Image.ContentLength);
                    String newName = Image.FileName.Insert(Image.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    Image.SaveAs(path);
                    model.candidateDetail.Image = newName;
                }

                if (model.candidateDetail.CreatedDateIdentity != null && model.candidateDetail.CreatedDateIdentity.Value.Year >= 1980)
                {
                    model.candidateDetail.CreatedDateIdentity = model.candidateDetail.CreatedDateIdentity;
                }
                if (await _candidateRepository.Update(model.candidateDetail))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch { }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult CandidateInformation(int id)
        {
            var model = new CandidateViewModel();
            var candidate = _candidateRepository.GetAllAsQueryable().FirstOrDefault(p => p.Id == id);
            model.candidateDetail = candidate;
            return PartialView("_Partial_EditCandidate", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(FormCollection fc)
        {
            Permission(clsPermission.GetUser().PermissionID, 1132);
            try
            {
                if (fc["listItemId"] != null && fc["listItemId"] != "")
                {
                    var listIds = fc["listItemId"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        if (await _candidateRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "CandidateManage") }, JsonRequestBehavior.AllowGet);
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

        #region Import

        [HttpPost]
        public ActionResult ImportFile(HttpPostedFileBase FileName)
        {
            Permission(clsPermission.GetUser().PermissionID, 1132);
            try
            {
                using (var excelPackage = new ExcelPackage(FileName.InputStream))
                {
                    List<tbl_Candidate> list = new List<tbl_Candidate>();
                    var worksheet = excelPackage.Workbook.Worksheets[1];
                    var lastRow = worksheet.Dimension.End.Row;
                    for (int row = 7; row <= lastRow; row++)
                    {
                        if (worksheet.Cells["B" + row].Value == null || worksheet.Cells["B" + row].Text == "")
                            continue;
                        var stf = new tbl_Candidate
                        {
                            FullName = worksheet.Cells["B" + row].Text,
                            Address = worksheet.Cells["F" + row].Value != null ? worksheet.Cells["F" + row].Text : null,
                            Email = worksheet.Cells["J" + row].Value != null ? worksheet.Cells["J" + row].Text : null,
                            Phone = worksheet.Cells["K" + row].Value != null ? worksheet.Cells["K" + row].Text : null,
                            IdentityCard = worksheet.Cells["G" + row].Value != null ? worksheet.Cells["G" + row].Text : null,
                            InformationTechnology = worksheet.Cells["M" + row].Value != null ? worksheet.Cells["M" + row].Text : null,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            StaffId = clsPermission.GetUser().StaffID,
                            Code = GenerateCode.CandidateCode()
                        };
                        String cel = "B";
                        // giới tính
                        try
                        {
                            cel = "C";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string gioitinh = worksheet.Cells[cel + row].Text;
                                stf.Gender = gioitinh == "Nam" ? true : false;
                            }
                        }
                        catch { }
                        //ngày sinh
                        try
                        {
                            cel = "D";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                stf.Birthday = DateTime.ParseExact(worksheet.Cells[cel + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                            }
                        }
                        catch { }
                        // nơi sinh
                        try
                        {
                            cel = "E";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string noisinh = worksheet.Cells[cel + row].Text;
                                stf.Birthplace = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == noisinh && c.TypeTag == 5).Select(c => c.Id).SingleOrDefault();
                            }
                        }
                        catch { }
                        // ngày cấp CMND
                        try
                        {
                            cel = "H";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                stf.CreatedDateIdentity = DateTime.ParseExact(worksheet.Cells[cel + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                            }
                        }
                        catch { }
                        // nơi cấp CMND
                        try
                        {
                            cel = "I";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string noicap = worksheet.Cells[cel + row].Text;
                                stf.IdentityTagId = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == noicap && c.TypeTag == 5).Select(c => c.Id).SingleOrDefault();
                            }
                        }
                        catch { }
                        // ngày ứng tuyển
                        try
                        {
                            cel = "L";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                stf.ApplyDate = DateTime.ParseExact(worksheet.Cells[cel + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                            }
                        }
                        catch { }
                        list.Add(stf);
                    }
                    Session["listCandidateImport"] = list;
                    return PartialView("_Partial_ImportDataListCandidate", list);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveImport()
        {
            Permission(clsPermission.GetUser().PermissionID, 1132);
            try
            {
                List<tbl_Candidate> list = Session["listCandidateImport"] as List<tbl_Candidate>;
                int i = 0;
                foreach (var item in list)
                {
                    await _candidateRepository.Create(item);
                    i++;
                }
                Session["listCandidateImport"] = null;
                if (i != 0)
                    return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Đã import thành công " + i + " dòng dữ liệu !", IsPartialView = false, RedirectTo = Url.Action("Index", "CandidateManage") }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Chưa có dữ liệu nào được import !" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                Session["listCandidateImport"] = null;
                return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Import dữ liệu lỗi !" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteImport(String listItemId)
        {
            Permission(clsPermission.GetUser().PermissionID, 1132);
            try
            {
                List<tbl_Candidate> list = Session["listCandidateImport"] as List<tbl_Candidate>;
                if (listItemId != null && listItemId != "")
                {
                    var listIds = listItemId.Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        int[] listIdsint = new int[listIds.Length];
                        for (int i = 0; i < listIds.Length; i++)
                        {
                            listIdsint[i] = Int32.Parse(listIds[i]);
                        }
                        for (int i = 0; i < listIdsint.Length; i++)
                        {
                            for (int j = i; j < listIdsint.Length; j++)
                            {
                                if (listIdsint[i] < listIdsint[j])
                                {
                                    int temp = listIdsint[i];
                                    listIdsint[i] = listIdsint[j];
                                    listIdsint[j] = temp;
                                }
                            }
                        }
                        foreach (var item in listIdsint)
                        {
                            list.RemoveAt(item);
                        }
                    }
                }
                Session["listCandidateImport"] = list;
                return PartialView("_Partial_ImportDataListCandidate", list);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Excel Template
        public void ExportExcelTemplateCandidate(MemoryStream stream, string templateFile, IDictionary<string, string> header = null)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream, new MemoryStream(System.IO.File.ReadAllBytes(templateFile))))
            {
                var ws = xlPackage.Workbook.Worksheets[1]; //first worksheet
                var valWs = xlPackage.Workbook.Worksheets.Add("Validation");
                valWs.Hidden = eWorkSheetHidden.VeryHidden;

                // nơi sinh
                var noisinh = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false && p.TypeTag == 5)
                    .Select(p => new ExportItem
                    {
                        Text = p.Tag,
                        Value = p.Id
                    });
                var columnIndex = ws.GetColumnIndex(CandidateColumn.NOISINH.ToString());
                ws.AddListValidation(valWs, noisinh, columnIndex, "Lỗi", "Lỗi", "NOISINH", "NOISINHName");

                // nơi cấp CMND
                var noicap = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false && p.TypeTag == 5)
                    .Select(p => new ExportItem
                    {
                        Text = p.Tag,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(CandidateColumn.NOICAP.ToString());
                ws.AddListValidation(valWs, noicap, columnIndex, "Lỗi", "Lỗi", "NOICAP", "NOICAPName");

                // giới tính
                var gioitinh = new List<ExportItem>();
                gioitinh.Add(new ExportItem { Text = "Nam", Value = 1 });
                gioitinh.Add(new ExportItem { Text = "Nữ", Value = 0 });
                columnIndex = ws.GetColumnIndex(CandidateColumn.GIOITINH.ToString());
                ws.AddListValidation(valWs, gioitinh, columnIndex, "Lỗi", "Lỗi", "GIOITINH", "GIOITINHName");

                xlPackage.Save();
            }
        }

        public ActionResult ExcelSample()
        {
            try
            {
                IDictionary<string, string> header = new Dictionary<string, string>();
                header.Add("TITLE", "DANH SÁCH ỨNG VIÊN");

                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    string templateFile = System.Web.HttpContext.Current.Server.MapPath("~\\Upload\\ImportExport\\Import_UngVienTOURDEMO.xlsx");
                    ExportExcelTemplateCandidate(stream, templateFile, header);
                    bytes = stream.ToArray();
                }

                string fileName = "Mau-import-ung-vien-TOURDEMO.xlsx";
                return File(bytes, "text/xls", fileName);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        #endregion
    }
}