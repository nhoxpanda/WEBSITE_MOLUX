using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MOLUX.Models;

namespace MOLUX.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PageIntroController : Controller
    {
        private BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();
        // GET: Admin/PageIntro

        #region Phần slide trang giới thiệu
        
       
        public ActionResult Index()
        {
            var model = _db.web_Slider.Where(q=>q.Type==4 || q.Type==5).OrderBy(p => p.Orders).ToList();
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
            return PartialView("_PartialUpdate", model);
        }
        public ActionResult Update(web_Slider model, HttpPostedFileBase Image)
        {
            // Picture
            var item = _db.web_Slider.Find(model.Id);
            if (Image != null)
            {
                String newName = Image.FileName.Insert(Image.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                String path = Server.MapPath("~/Images/Slider/" + newName);
                Image.SaveAs(path);
                item.Image = newName;
            }

                item.IsShow = model.IsShow;
                item.Link = model.Link;
                item.Type = model.Type;
           
            
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
        #region phần sản phẩm tiêu biểu
        
       
        public ActionResult MainProduct()
        {
            var model = _db.MainProduct.ToList();
            return View(model);
        }
        public ActionResult CreateMainProduct(MainProduct model, HttpPostedFileBase Image)
        {
            // Picture
            if (Image != null)
            {
                String newName = Image.FileName.Insert(Image.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                String path = Server.MapPath("~/Images/MainProduct/" + newName);
                Image.SaveAs(path);
                model.Image = newName;
            }
            _db.MainProduct.Add(model);
            _db.SaveChanges();

            return RedirectToAction("MainProduct");
        }
        public ActionResult EditMainProduct(int id)
        {
            var model = _db.MainProduct.Find(id);
            return PartialView("_PartialUpdateMainProduct", model);
        }
        public ActionResult UpdateMainProduct(MainProduct model, HttpPostedFileBase Image)
        {
            // Picture  
            var item = _db.MainProduct.Find(model.ID);
            if (Image != null)
            {
                String newName = Image.FileName.Insert(Image.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                String path = Server.MapPath("~/Images/MainProduct/" + newName);
                Image.SaveAs(path);
                item.Image = newName;
               
            }
            item.Title = model.Title;
            item.Link = model.Link;
            item.Orders = model.Orders;
            item.IsShow = model.IsShow;
            

          
            _db.SaveChanges();
            return RedirectToAction("MainProduct");
        }
        public ActionResult DeleteMainProduct(int id)
        {
            try
            {
                var model = _db.MainProduct.Find(id);
                _db.MainProduct.Remove(model);
                _db.SaveChanges();
                return Json(JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(JsonRequestBehavior.AllowGet);
            }
        } 
#endregion
    }
}