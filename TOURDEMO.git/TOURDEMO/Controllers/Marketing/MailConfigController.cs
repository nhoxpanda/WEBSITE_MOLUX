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
    public class MailConfigController : BaseController
    {
        // GET: MailConfig
        #region Init

        private IGenericRepository<tbl_MailConfig> _mailConfigRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private DataContext _db;

        public MailConfigController(
            IGenericRepository<tbl_MailConfig> mailConfigRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
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
            Permission(clsPermission.GetUser().PermissionID, 1112);

            var model = _mailConfigRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false)
                .OrderByDescending(p => p.DateModify).ToList();
            return View(model);
        }
        #endregion

        #region Create
        [HttpPost]
        public async Task<ActionResult> Create(tbl_MailConfig model)
        {
            Permission(clsPermission.GetUser().PermissionID, 1112);
            model.Password = Common.MaHoa(model.Password);
            model.DateModify = DateTime.Now;
            model.IsDelete = false;
            model.IsForgotPassword = false;
            model.StaffId = clsPermission.GetUser().StaffID;
            try
            {
                await _mailConfigRepository.Create(model);
            }
            catch { }
            return RedirectToAction("Index");
        }
        #endregion

        #region Update
        public ActionResult Edit(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1112);

            var model = _mailConfigRepository.FindId(id);
            return PartialView("_Partial_EditMailConfig", model);
        }

        [HttpPost]
        public ActionResult Update(tbl_MailConfig model)
        {
            Permission(clsPermission.GetUser().PermissionID, 1112);
            if (model.Password == null)
            {
                model.Password = _mailConfigRepository.FindId(model.Id).Password;
            }
            else
            {
                model.Password = Common.MaHoa(model.Password);
            }
            model.DateModify = DateTime.Now;
            model.IsDelete = false;
            model.StaffId = clsPermission.GetUser().StaffID;
            try
            {
                _db.Entry<tbl_MailConfig>(model).State = EntityState.Modified;
                _db.SaveChanges();
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
                Permission(clsPermission.GetUser().PermissionID, 1112);

                if (fc["listItemId"] != null && fc["listItemId"] != "")
                {
                    var listIds = fc["listItemId"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        //
                        foreach (var i in listIds)
                        {
                            var mail = _mailConfigRepository.FindId(Convert.ToInt32(i));
                            UpdateHistory.SaveHistory(1112, "Xóa mail cấu hình để gửi: " + mail.Email,
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
                        if (await _mailConfigRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "MailConfig") }, JsonRequestBehavior.AllowGet);
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