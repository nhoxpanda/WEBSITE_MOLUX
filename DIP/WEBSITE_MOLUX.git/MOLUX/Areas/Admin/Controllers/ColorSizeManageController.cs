using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MOLUX.Helper;
using MOLUX.Models;
using PagedList;
using PagedList.Mvc;

namespace MOLUX.Areas.Admin.Controllers
{
    public class ColorSizeManageController : Controller
    {
        BMSMoluxHongKongEntities db = new BMSMoluxHongKongEntities();

        public ActionResult Index()
        {
            var model = db.web_SizeColor.ToList();
            return View(model);
        }
        public ActionResult Create(web_SizeColor model)
        {
            if (model!= null)
            {
                db.web_SizeColor.Add(model);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public PartialViewResult Edit(int id)
        {
            var model = db.web_SizeColor.Find(id);
            return PartialView("_PartialEdit", model);
        }
        public ActionResult Edits(web_SizeColor model)
        {
            db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id)
        {
            try
            {
                var model = db.web_SizeColor.FirstOrDefault(p => p.Id == id);
                db.web_SizeColor.Remove(model);
                db.SaveChanges();
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
    }
}