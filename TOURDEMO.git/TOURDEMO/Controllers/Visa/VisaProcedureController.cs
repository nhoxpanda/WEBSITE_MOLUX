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

namespace TOURDEMO.Controllers.Visa
{
    [Authorize]
    public class VisaProcedureController : BaseController
    {
        #region Init

        private IGenericRepository<tbl_VisaProcedure> _visaProcedureRepository;
        private IGenericRepository<tbl_ActionData> _actionDataRepository;
        private DataContext _db;

        public VisaProcedureController(IGenericRepository<tbl_VisaProcedure> visaProcedureRepository,
            IGenericRepository<tbl_ActionData> actionDataRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._visaProcedureRepository = visaProcedureRepository;
            this._actionDataRepository = actionDataRepository;
            _db = new DataContext();
        }

        #endregion
        void Permission(int PermissionsId, int formId)
        {
            var list = _actionDataRepository.GetAllAsQueryable().Where(p => p.FormId == formId & p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsEdit = list.Contains(3);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsImport = list.Contains(4);
        }
        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 1141);
            var dictionary = _visaProcedureRepository.GetAllAsQueryable().Where(p => p.IsDelete == false).Select(p => new DictionaryViewModel
            {
                Id = p.Id,
                Name = p.Name
            });

            return View(dictionary.ToList());
        }

        [HttpPost]
        public ActionResult SaveData(int id, string name)
        {
            try
            {
                if (id == 0) // insert
                {
                    tbl_VisaProcedure dictionary = new tbl_VisaProcedure()
                    {
                        Name = name,
                        IsDelete = false
                    };

                    _db.tbl_VisaProcedure.Add(dictionary);
                }
                else // update
                {
                    var item = _db.tbl_VisaProcedure.Find(id);
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
            if (fc["listItemId"] != null && fc["listItemId"] != "")
            {
                var listIds = fc["listItemId"].Split(',');
                listIds = listIds.Take(listIds.Count() - 1).ToArray();
                if (listIds.Count() > 0)
                {
                    if (await _visaProcedureRepository.DeleteMany(listIds, false))
                    {
                        return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "VisaProcedure") }, JsonRequestBehavior.AllowGet);
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