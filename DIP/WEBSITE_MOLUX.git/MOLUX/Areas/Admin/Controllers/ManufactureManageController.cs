using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MOLUX.Models;

namespace MOLUX.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManufactureManageController : Controller
    {
        // GET: Admin/ManufactureManage

        private BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();

        public ActionResult Index()
        {
            var model =  _db.Manufacturer.OrderByDescending(p => p.RowID).ToList();
            return View(model);
        }

        public ActionResult Create(Manufacturer model, HttpPostedFileBase Picture)
        {
            if (Picture != null)
            {
                String newName = Picture.FileName.Insert(Picture.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                String path = Server.MapPath("~/Images/Manufacturer/" + newName);
                Picture.SaveAs(path);
                model.Picture = newName;
            }
             _db.Manufacturer.Add(model);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var model =  _db.Manufacturer.FirstOrDefault(p=>p.RowID == id);
            return PartialView("_Partial_Update", model);
        }

        public ActionResult Update(Manufacturer model, HttpPostedFileBase Picture)
        {
            if (Picture != null)
            {
                String newName = Picture.FileName.Insert(Picture.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                String path = Server.MapPath("~/Images/Manufacturer/" + newName);
                Picture.SaveAs(path);
                model.Picture = newName;
            }
            _db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var model = _db.Manufacturer.FirstOrDefault(p => p.RowID == id);
                _db.Manufacturer.Remove(model);
                _db.SaveChanges();
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
    }
}