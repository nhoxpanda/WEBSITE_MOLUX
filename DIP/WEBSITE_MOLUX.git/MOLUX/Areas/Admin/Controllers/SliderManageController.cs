using MOLUX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MOLUX.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SliderManageController : Controller
    {
        private BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();

        #region Slider
        public ActionResult Index()
        {
            var model = _db.web_Slider.OrderBy(p => p.Orders).ToList();
            return View(model);
        }

        public ActionResult Create(web_Slider model, HttpPostedFileBase Image)
        {
            // Picture
            if (Image != null)
            {
                String newName = Image.FileName.Insert(Image.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                String path = Server.MapPath("~/Images/Slider/" + newName);
                Image.SaveAs(path);
                model.Image = newName;
            }
            _db.web_Slider.Add(model);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var model = _db.web_Slider.Find(id);
            return PartialView("_Partial_Update", model);
        }

        public ActionResult Update(web_Slider model, HttpPostedFileBase Image)
        {
            // Picture
            if (Image != null)
            {
                String newName = Image.FileName.Insert(Image.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                String path = Server.MapPath("~/Images/Slider/" + newName);
                Image.SaveAs(path);
                model.Image = newName;
            }
            _db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var model = _db.web_Slider.Find(id);
                _db.web_Slider.Remove(model);
                _db.SaveChanges();
                return Json(JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Advertisement
        public ActionResult Advertisement()
        {
            var model = _db.web_Slider.Where(p => p.Type == 3).OrderBy(p => p.Orders).ToList();
            return View(model);
        }

        public ActionResult EditAds(int id)
        {
            var model = _db.web_Slider.Find(id);
            return PartialView("_Partial_UpdateAds", model);
        }

        public ActionResult UpdateAds(web_Slider model, HttpPostedFileBase Image)
        {
            // Picture
            if (Image != null)
            {
                String newName = Image.FileName.Insert(Image.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                String path = Server.MapPath("~/Images/Slider/" + newName);
                Image.SaveAs(path);
                model.Image = newName;
            }
            _db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();

            return RedirectToAction("Advertisement");
        }

        public ActionResult ShowHideAds(int id)
        {
            var model = _db.web_Slider.Find(id);
            if (model.IsShow == false)
            {
                model.IsShow = true;
            }
            else
            {
                model.IsShow = false;
            }
            _db.SaveChanges();

            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}