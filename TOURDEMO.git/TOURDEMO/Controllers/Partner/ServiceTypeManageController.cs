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

namespace TOURDEMO.Controllers.Partner
{
    [Authorize]
    public class ServiceTypeManageController : BaseController
    {
        // GET: ServiceTypeManage
        #region Init

        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private DataContext _db;

        public ServiceTypeManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository, IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._dictionaryRepository = dictionaryRepository;
            _db = new DataContext();
        }

        #endregion

        #region List
        public ActionResult Index()
        {
            int perId = clsPermission.GetUser().PermissionID;
            var list = _db.tbl_ActionData.Where(p => p.FormId == 17 && p.PermissionsId == perId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);

            var dictionary = _dictionaryRepository.GetAllAsQueryable().Where(p => p.DictionaryCategoryId == 13 && p.IsDelete == false).Select(p => new DictionaryViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Note = p.Note,
                Icon = p.Icon
            });

            return View(dictionary.ToList());
        }
        #endregion

        #region Create

        [HttpPost]
        public async Task<ActionResult> Create(tbl_Dictionary model, HttpPostedFileBase Icon)
        {
            if (Icon != null)
            {
                String path = Server.MapPath("~/Images/Icon/" + Icon.FileName);
                Icon.SaveAs(path);
                model.Icon = Icon.FileName;
            }
            model.DictionaryCategoryId = 13;
            await _dictionaryRepository.Create(model);

            return RedirectToAction("Index");
        }
        #endregion

        #region Update

        [HttpPost]
        public async Task<ActionResult> Edit(int id)
        {
            return PartialView("_Partial_Edit", await _dictionaryRepository.GetById(id));
        }

        [HttpPost]
        public async Task<ActionResult> Update(tbl_Dictionary model, HttpPostedFileBase Icon)
        {
            if (Icon != null)
            {
                String path = Server.MapPath("~/Images/Icon/" + Icon.FileName);
                Icon.SaveAs(path);
                model.Icon = Icon.FileName;
            }
            model.DictionaryCategoryId = 13;
            await _dictionaryRepository.Update(model);

            return RedirectToAction("Index");
        }
        #endregion

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
                    if (await _dictionaryRepository.DeleteMany(listIds, false))
                    {
                        return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "ServiceTypeManage") }, JsonRequestBehavior.AllowGet);
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