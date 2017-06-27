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

namespace TOURDEMO.Controllers.Tour
{
    public class TourStatusManageController : BaseController
    {
        // GET: TourStatus
        #region Init

        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private DataContext _db;

        public TourStatusManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository, IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._dictionaryRepository = dictionaryRepository;
            _db = new DataContext();
        }

        #endregion
        void Permission(int PermissionsId, int formId)
        {
            var list = _db.tbl_ActionData.Where(p => p.FormId == formId && p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);
            ViewBag.IsImport = list.Contains(4);
            ViewBag.IsExport = list.Contains(5);
        }
        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 1102);
            var dictionary = _dictionaryRepository.GetAllAsQueryable().Where(p => p.DictionaryCategoryId == 29 && p.IsDelete == false).Select(p => new DictionaryViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Note = p.Note
            });

            return View(dictionary.ToList());
        }

        [HttpPost]
        public ActionResult SaveData(int id, string name, string note)
        {
            try
            {
                if (id == 0) // insert
                {
                    tbl_Dictionary dictionary = new tbl_Dictionary()
                    {
                        Name = name,
                        DictionaryCategoryId = 29,
                        Note = note
                    };

                    _db.tbl_Dictionary.Add(dictionary);
                }
                else // update
                {
                    var item = _db.tbl_Dictionary.Find(id);
                    item.Name = name;
                    item.Note = note;
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
                    if (await _dictionaryRepository.DeleteMany(listIds, false))
                    {
                        return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "TourStatusManage") }, JsonRequestBehavior.AllowGet);
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