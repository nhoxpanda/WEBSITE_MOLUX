using CRM.Core;
using CRM.Infrastructure;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TOURDEMO.Models;
using TOURDEMO.Utilities;

namespace TOURDEMO.Controllers.Marketing
{
    public class MailImportManageController : BaseController
    {
        // GET: MailImportManage

        #region Init
        private IGenericRepository<tbl_MailImport> _mailImportRepository;
        private DataContext _db;

        public MailImportManageController(
            IGenericRepository<tbl_MailImport> mailImportRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._mailImportRepository = mailImportRepository;
            _db = new DataContext();
        }
        #endregion

        #region Index

        int SDBID = 6;
        int maPB = 0, maNKD = 0, maNV = 0, maCN = 0;
        void Permission(int PermissionsId, int formId)
        {
            var list = _db.tbl_ActionData.Where(p => p.FormId == formId && p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsImport = list.Contains(4);

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
            Permission(clsPermission.GetUser().PermissionID, 1127);

            var model = _mailImportRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false)
                            .OrderByDescending(p => p.CreatedDate).ToList();
            return View(model);
        }

        #endregion

        #region Import
        [HttpPost]
        public ActionResult ImportFile(HttpPostedFileBase FileName)
        {
            try
            {
                using (var excelPackage = new ExcelPackage(FileName.InputStream))
                {
                    List<tbl_MailImport> list = new List<tbl_MailImport>();
                    var worksheet = excelPackage.Workbook.Worksheets[1];
                    var lastRow = worksheet.Dimension.End.Row;
                    for (int row = 6; row <= lastRow; row++)
                    {
                        if (worksheet.Cells["B" + row].Value == null || worksheet.Cells["B" + row].Text == "")
                            continue;
                        var cus = new tbl_MailImport
                        {
                            Name = worksheet.Cells["B" + row].Value != null ? worksheet.Cells["B" + row].Text : null,
                            Email = worksheet.Cells["C" + row].Value != null ? worksheet.Cells["C" + row].Text : null,
                            Phone = worksheet.Cells["D" + row].Value != null ? worksheet.Cells["D" + row].Text : null,
                            CreatedDate = DateTime.Now
                        };
                        list.Add(cus);
                    }
                    Session["listMailImport"] = list;
                    return PartialView("_Partial_ImportDataList", list);
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
            try
            {
                List<tbl_MailImport> list = Session["listMailImport"] as List<tbl_MailImport>;
                int i = 0;
                foreach (var item in list)
                {
                    item.CreatedDate = DateTime.Now;
                    item.StaffId = clsPermission.GetUser().StaffID;
                    await _mailImportRepository.Create(item);
                    i++;
                }
                Session["listMailImport"] = null;
                if (i != 0)
                {
                    UpdateHistory.SaveHistory(1127, "Import danh sách email",
                        null, //appointment
                                null, //contract
                                null, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                null // ticket
                                );
                    return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Đã import thành công " + i + " dòng dữ liệu !", IsPartialView = false, RedirectTo = Url.Action("Index", "MailImportManage") }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Chưa có dữ liệu nào được import !" }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                Session["listMailImport"] = null;
                return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Import dữ liệu lỗi !" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Excel Sample
        public void ExportExcelTemplateCustomer(MemoryStream stream, string templateFile, IDictionary<string, string> header = null)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream, new MemoryStream(System.IO.File.ReadAllBytes(templateFile))))
            {
                var ws = xlPackage.Workbook.Worksheets[1]; //first worksheet
                var valWs = xlPackage.Workbook.Worksheets.Add("Validation");
                valWs.Hidden = eWorkSheetHidden.VeryHidden;

                xlPackage.Save();
            }
        }

        public ActionResult ExcelSample()
        {
            try
            {
                IDictionary<string, string> header = new Dictionary<string, string>();
                header.Add("TITLE", "DANH SÁCH EMAIL");

                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    string templateFile = System.Web.HttpContext.Current.Server.MapPath("~\\Upload\\ImportExport\\Import_EmailTOURDEMO.xlsx");
                    ExportExcelTemplateCustomer(stream, templateFile, header); 
                    bytes = stream.ToArray();
                }

                string fileName = "Mau-import-email-TOURDEMO.xlsx";
                return File(bytes, "text/xls", fileName);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(FormCollection fc)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1127);

                if (fc["listItemId"] != null && fc["listItemId"] != "")
                {
                    var listIds = fc["listItemId"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        //
                        foreach (var i in listIds)
                        {
                            var mail = _mailImportRepository.FindId(Convert.ToInt32(i));
                            UpdateHistory.SaveHistory(1127, "Xóa mail đã import: " + mail.Email,
                                null, //appointment
                                null, //contract
                                null, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                null // ticket
                                );
                        }
                        //
                        if (await _mailImportRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "MailImportManage") }, JsonRequestBehavior.AllowGet);
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
    }
}