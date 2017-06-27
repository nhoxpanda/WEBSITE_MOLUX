using CRM.Core;
using CRM.Infrastructure;
using TOURDEMO.Models;
using TOURDEMO.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace TOURDEMO.Controllers
{
    [Authorize]
    public class DocumentManageController : BaseController
    {
        //
        // GET: /DocumentManage/

        #region Init

        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_Form> _formRepository;
        private IGenericRepository<tbl_Module> _moduleRepository;
        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;

        private DataContext _db;

        public DocumentManageController(IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Form> formRepository,
            IGenericRepository<tbl_Module> moduleRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._documentFileRepository = documentFileRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._tagsRepository = tagsRepository;
            this._updateHistoryRepository = updateHistoryRepository;
            this._staffRepository = staffRepository;
            this._formRepository = formRepository;
            this._moduleRepository = moduleRepository;
            _db = new DataContext();
        }

        #endregion

        #region List & Permission
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
        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 33);

            if (SDBID == 6)
                return View(new List<tbl_DocumentFile>());

            var model = _documentFileRepository.GetAllAsQueryable().Where(p => (p.StaffId == maNV | maNV == 0)
                    & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                    & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                    & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false))
                    .OrderByDescending(p => p.CreatedDate).ToList();
            return View(model);
        }
        #endregion

        #region Create
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(tbl_DocumentFile model, FormCollection form, List<HttpPostedFileBase> FileName)
        {
            Permission(clsPermission.GetUser().PermissionID, 33);

            try
            {
                model.Code = GenerateCode.DocumentCode();
                model.CreatedDate = DateTime.Now;
                model.IsRead = false;
                model.ModifiedDate = DateTime.Now;
                if (form["TagsId"] != null && form["TagsId"] != "")
                {
                    model.TagsId = form["TagsId"].ToString();
                }
                model.PermissionStaff = form["PermissionStaff"] == null ? null : form["PermissionStaff"].ToString();
                model.StaffId = clsPermission.GetUser().StaffID;

                if (FileName != null)
                {
                    foreach (var file in FileName)
                    {
                        String path = Server.MapPath("~/Upload/file/" + file.FileName);
                        file.SaveAs(path);
                        model.FileName = file.FileName;
                        model.FileSize = Common.ConvertFileSize(file.ContentLength);

                        if (await _documentFileRepository.Create(model))
                        {
                            UpdateHistory.SaveHistory(33, "Thêm mới tài liệu, code: " + model.Code + " - " + model.FileName,
                                null, //appointment
                                null, //contract
                                null, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                model.Id, //document
                                null, //history
                                null // ticket
                                );
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Dữ liệu đầu vào không đúng định dạng!");
                }
            }
            catch { }
            return RedirectToAction("Index");
        }
        #endregion

        #region Update
        [HttpPost]
        public async Task<ActionResult> EditInfoDocument(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 33);

            var model = await _documentFileRepository.GetById(id);
            return PartialView("_Partial_EditDocument", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tbl_DocumentFile model, FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 33);

            try
            {
                model.IsRead = true;
                model.ModifiedDate = DateTime.Now;
                if (form["TagsId"] != null && form["TagsId"] != "")
                {
                    model.TagsId = form["TagsId"].ToString();
                }
                model.PermissionStaff = form["PermissionStaff"] == null ? null : form["PermissionStaff"].ToString();
                HttpPostedFileBase file = Request.Files["FileName"];
                if (file != null && file.ContentLength > 0)
                {
                    String path = Server.MapPath("~/Upload/file/" + file.FileName);
                    file.SaveAs(path);
                    model.FileName = file.FileName;
                    model.FileSize = Common.ConvertFileSize(file.ContentLength);
                }
                else
                {
                    var doc = _db.tbl_DocumentFile.Find(model.Id);
                    model.FileName = doc.FileName;
                    model.FileSize = doc.FileSize;
                }

                if (await _documentFileRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(33, "Cập nhật tài liệu: " + model.Code,
                        null, //appointment
                                null, //contract
                                null, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                model.Id, //document
                                null, //history
                                null // ticket
                                );
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Dữ liệu đầu vào không đúng định dạng!");
                }
            }
            catch
            {
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(FormCollection fc)
        {
            Permission(clsPermission.GetUser().PermissionID, 33);

            try
            {
                if (fc["listItemId"] != null && fc["listItemId"] != "")
                {
                    var listIds = fc["listItemId"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    foreach (var i in listIds)
                    {
                        var document = _documentFileRepository.FindId(Convert.ToInt32(i));
                        UpdateHistory.SaveHistory(33, "Xóa tài liệu: " + document.Code + " - " + document.FileName,
                            null, //appointment
                                null, //contract
                                null, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                document.Id, //document
                                null, //history
                                null // ticket
                                );
                    }
                    if (listIds.Count() > 0)
                    {
                        if (await _documentFileRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "DocumentManage") }, JsonRequestBehavior.AllowGet);
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

        #region Tab
        [ChildActionOnly]
        public ActionResult _Partial_TabInfoDocument()
        {
            var model = new List<tbl_Staff>();
            var item = _documentFileRepository.GetAllAsQueryable().FirstOrDefault();
            if (item != null && item.PermissionStaff != null)
            {
                foreach (var i in item.PermissionStaff.Split(','))
                {
                    model.Add(_db.tbl_Staff.Find(Convert.ToInt32(i)));
                }
            }

            return PartialView("_Partial_TabInfoDocument", model);
        }

        public ActionResult TabInfoDocument(int id)
        {
            var model = new List<tbl_Staff>();
            var item = _documentFileRepository.GetAllAsQueryable().FirstOrDefault(p => p.Id == id);
            if (item != null && item.PermissionStaff != null)
            {
                foreach (var i in item.PermissionStaff.Split(','))
                {
                    model.Add(_db.tbl_Staff.Find(Convert.ToInt32(i)));
                }
            }

            return PartialView("_Partial_TabInfoDocument", model);
        }
        #endregion

        #region Add Staff
        [ChildActionOnly]
        public ActionResult _Partial_AddStaffDocument()
        {
            return PartialView("_Partial_AddStaffDocument", new List<tbl_Staff>());
        }
        #endregion

        #region Cập nhật thay đổi
        [ChildActionOnly]
        public ActionResult _CapNhatThayDoi()
        {
            return PartialView("_CapNhatThayDoi");
        }

        [HttpPost]
        public ActionResult InfoCapNhatThayDoi(int id)
        {
            var model = _updateHistoryRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.IsDelete == false && p.DocumentFileId == id)
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new HistoryLogViewModel()
                {
                    Id = p.Id,
                    Staff = _staffRepository.FindId(p.StaffId).FullName,
                    Date = string.Format("{0:dd-MM-yyyy hh:mm tt}", p.CreatedDate),
                    Note = p.Note,
                    StaffId = p.StaffId,
                    Form = p.FormId != null ? _formRepository.FindId(p.FormId).Name : "",
                    Module = p.ModuleId != null ? _moduleRepository.FindId(p.ModuleId).Name : ""
                }).ToList();
            return PartialView("_CapNhatThayDoi", model);
        }
        #endregion
    }
}
