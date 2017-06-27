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

namespace TOURDEMO.Controllers.Bill
{
    public class BillStatusManageController : BaseController
    {
        // GET: BillStatusManage
        #region Init

        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_ActionData> _actionDataRepository;
        private DataContext _db;

        public BillStatusManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_ActionData> actionDataRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._actionDataRepository = actionDataRepository;
            this._dictionaryRepository = dictionaryRepository;
            _db = new DataContext();
        }

        #endregion
        void Permission(int PermissionsId, int formId)
        {
            var list = _actionDataRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.FormId == formId && p.PermissionsId == PermissionsId)
                .Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);
            ViewBag.IsImport = list.Contains(4);
            ViewBag.IsExport = list.Contains(5);
        }

        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 1144);
            var dictionary = _dictionaryRepository.GetAllAsQueryable().Where(p => p.DictionaryCategoryId == 30 && p.IsDelete == false).Select(p => new DictionaryViewModel
            {
                Id = p.Id,
                Name = p.Name
            });

            return View(dictionary.ToList());
        }

        [HttpPost]
        public ActionResult SaveData(int id, string name)
        {
            Permission(clsPermission.GetUser().PermissionID, 1144);
            try
            {
                if (id == 0) // insert
                {
                    tbl_Dictionary dictionary = new tbl_Dictionary()
                    {
                        Name = name,
                        DictionaryCategoryId = 30
                    };

                    _db.tbl_Dictionary.Add(dictionary);
                }
                else // update
                {
                    var item = _db.tbl_Dictionary.Find(id);
                    item.Name = name;
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
            Permission(clsPermission.GetUser().PermissionID, 1144);
            if (fc["listItemId"] != null && fc["listItemId"] != "")
            {
                var listIds = fc["listItemId"].Split(',');
                listIds = listIds.Take(listIds.Count() - 1).ToArray();
                if (listIds.Count() > 0)
                {
                    if (await _dictionaryRepository.DeleteMany(listIds, false))
                    {
                        return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "BillStatusManage") }, JsonRequestBehavior.AllowGet);
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