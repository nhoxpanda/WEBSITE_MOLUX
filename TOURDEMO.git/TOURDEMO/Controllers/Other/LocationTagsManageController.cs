using CRM.Core;
using CRM.Infrastructure;
using TOURDEMO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TOURDEMO.Controllers.Other
{
    [Authorize]
    public class LocationTagsManageController : BaseController
    {
        // GET: TagsManage
        #region Init

        private IGenericRepository<tbl_Tags> _locationRepository;
        private DataContext _db;

        public LocationTagsManageController(IGenericRepository<tbl_Tags> locationRepository, IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._locationRepository = locationRepository;
            _db = new DataContext();
        }

        #endregion

        public ActionResult Index()
        {
            var model = _locationRepository.GetAllAsQueryable().Where(p => p.IsDelete == false).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(int id, int type, string name)
        {
            var tag = new tbl_Tags
            {
                IsDelete = false,
                ParentId = id,
                Tag = name,
                TypeTag = type
            };
            _db.tbl_Tags.Add(tag);
            _db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int id)
        {
            var item = _locationRepository.FindId(id);
            return PartialView("_Partial_UpdateTag", item);
        }

        [HttpPost]
        public ActionResult Update(tbl_Tags model)
        {
            model.IsDelete = false;
            _locationRepository.Update(model);
            return Redirect("~/LocationTagsManage");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var items = _locationRepository.GetAllAsQueryable().Where(p => p.ParentId == id).ToList();

            if (items.Count() > 0)
            {
                return Json(0, JsonRequestBehavior.AllowGet);                
            }
            else
            {
                await _locationRepository.Delete(id, false);
                return Json(1, JsonRequestBehavior.AllowGet);
            }
        }

    }
}