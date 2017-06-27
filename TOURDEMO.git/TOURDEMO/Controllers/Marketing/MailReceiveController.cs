using CRM.Core;
using CRM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TOURDEMO.Models;
using TOURDEMO.Utilities;

namespace TOURDEMO.Controllers.Marketing
{
    [Authorize]
    public class MailReceiveController : BaseController
    {
        // GET: MailReceive
        #region Init

        private IGenericRepository<tbl_MailReceiveList> _mailReceiveListRepository;
        private IGenericRepository<tbl_MailReceives> _mailReceivesRepository;
        private IGenericRepository<tbl_MailTemplates> _mailTemplatesRepository;
        private IGenericRepository<tbl_MailConfig> _mailConfigRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private DataContext _db;

        public MailReceiveController(
            IGenericRepository<tbl_MailReceiveList> mailReceiveListRepository,
            IGenericRepository<tbl_MailReceives> mailReceivesRepository,
            IGenericRepository<tbl_MailTemplates> mailTemplatesRepository,
            IGenericRepository<tbl_MailConfig> mailConfigRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._mailReceiveListRepository = mailReceiveListRepository;
            this._mailReceivesRepository = mailReceivesRepository;
            this._mailTemplatesRepository = mailTemplatesRepository;
            this._mailConfigRepository = mailConfigRepository;
            this._staffRepository = staffRepository;
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
            Permission(clsPermission.GetUser().PermissionID, 1109);

            var model = _mailReceivesRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false)
                .OrderByDescending(p => p.CreateDate).ToList();
            return View(model);
        }
        #endregion

        #region Create
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(tbl_MailReceives model)
        {
            Permission(clsPermission.GetUser().PermissionID, 1109);
            model.CreateDate = DateTime.Now;
            model.ModifyDate = DateTime.Now;
            model.IsDelete = false;
            model.StaffId = clsPermission.GetUser().StaffID;
            try
            {
                await _mailReceivesRepository.Create(model);
            }
            catch { }
            return RedirectToAction("Index");
        }
        #endregion

        #region Update
        public ActionResult Edit(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1109);

            var model = _mailReceivesRepository.FindId(id);
            return PartialView("_Partial_EditMailReceive", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tbl_MailReceives model)
        {
            Permission(clsPermission.GetUser().PermissionID, 1109);
            model.ModifyDate = DateTime.Now;
            model.IsDelete = false;
            model.StaffId = clsPermission.GetUser().StaffID;
            try
            {
                if (await _mailReceivesRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(1109, "Cập nhật danh sách nhận mail: " + model.Name,
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
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1102);

                if (fc["listItemId"] != null && fc["listItemId"] != "")
                {
                    var listIds = fc["listItemId"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        //
                        foreach (var i in listIds)
                        {
                            var appointment = _mailReceivesRepository.FindId(Convert.ToInt32(i));
                            UpdateHistory.SaveHistory(1102, "Xóa danh sách mail nhận: " + appointment.Name,
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
                        if (await _mailReceivesRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "MailReceive") }, JsonRequestBehavior.AllowGet);
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

        #region Tab Info

        [HttpPost]
        public ActionResult GetIdReceive(int id)
        {
            Session["idReceive"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        public ActionResult _Partial_ListMailReceive()
        {
            Permission(clsPermission.GetUser().PermissionID, 1104);
            return PartialView("_Partial_ListMailReceive");
        }

        public ActionResult TabInfoMailReceive(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1104);
            var model = _mailReceiveListRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.MailReceiveId == id && p.IsDelete == false).ToList();
            return PartialView("_Partial_ListMailReceive", model);
        }

        [HttpPost]
        public ActionResult RemoveFromList(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1104);
            var model = _db.tbl_MailReceiveList.Find(id);
            _db.tbl_MailReceiveList.Remove(model);
            _db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}