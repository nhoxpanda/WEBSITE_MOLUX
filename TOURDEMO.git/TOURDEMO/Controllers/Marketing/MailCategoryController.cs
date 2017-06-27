using CRM.Core;
using CRM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TOURDEMO.Models;
using TOURDEMO.Utilities;

namespace TOURDEMO.Controllers.Marketing
{
    [Authorize]
    public class MailCategoryController : BaseController
    {
        // GET: MailCategory
        #region Init
        private IGenericRepository<tbl_MailCategory> _mailCategoryRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private DataContext _db;

        public MailCategoryController(
            IGenericRepository<tbl_MailCategory> mailCategoryRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._mailCategoryRepository = mailCategoryRepository;
            this._staffRepository = staffRepository;
            _db = new DataContext();
        }
        #endregion

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
            Permission(clsPermission.GetUser().PermissionID, 1111);
            var dictionary = _mailCategoryRepository.GetAllAsQueryable().Where(p => p.IsDelete == false).Select(p => new DictionaryViewModel
            {
                Id = p.Id,
                Name = p.CateName
            });

            return View(dictionary.ToList());
        }

        [HttpPost]
        public ActionResult SaveData(int id, string name)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1111);
                if (id == 0) // insert
                {
                    tbl_MailCategory dictionary = new tbl_MailCategory()
                    {
                        CateName = name,
                        IsDelete = false
                    };

                    _db.tbl_MailCategory.Add(dictionary);
                }
                else // update
                {
                    var item = _db.tbl_MailCategory.Find(id);
                    item.CateName = name;
                }

                _db.SaveChanges();
            }
            catch
            {
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(FormCollection fc)
        {
            Permission(clsPermission.GetUser().PermissionID, 1111);
            if (fc["listItemId"] != null && fc["listItemId"] != "")
            {
                var listIds = fc["listItemId"].Split(',');
                listIds = listIds.Take(listIds.Count() - 1).ToArray();
                if (listIds.Count() > 0)
                {
                    if (await _mailCategoryRepository.DeleteMany(listIds, false))
                    {
                        return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "MailCategory") }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Xóa dữ liệu thất bại !" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Vui lòng chọn những mục cần xóa !" }, JsonRequestBehavior.AllowGet);
        }
    }
}