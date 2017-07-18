using MOLUX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MOLUX.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SocialNetworkManageController : Controller
    {
        // GET: Admin/SocialNetworkManage
        private BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();

        public ActionResult Index()
        {
            var model = _db.web_SocialNetwork.ToList();
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _db.web_SocialNetwork.Find(id);
            return PartialView("_Partial_Update", model);
        }

        public ActionResult Update(web_SocialNetwork model)
        {
            _db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}