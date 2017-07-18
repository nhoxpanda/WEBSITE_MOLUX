﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MOLUX.Models;

namespace MOLUX.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PartnerManageController : Controller
    {
        // GET: Admin/PartnerManageController

        private BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();

        public ActionResult Index()
        {
            var model = _db.web_Partner.OrderBy(p => p.Orders).ToList();
            return View(model);
        }

        public ActionResult Create(web_Partner model, HttpPostedFileBase Logo)
        {
            // Picture
            if (Logo != null)
            {
                String newName = Logo.FileName.Insert(Logo.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                String path = Server.MapPath("~/Images/Manufacturer/" + newName);
                Logo.SaveAs(path);
                model.Logo = newName;
            }
            _db.web_Partner.Add(model);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var model = _db.web_Partner.Find(id);
            return PartialView("_Partial_Update", model);
        }

        public ActionResult Update(web_Partner model, HttpPostedFileBase Logo)
        {
            // Picture
            if (Logo != null)
            {
                String newName = Logo.FileName.Insert(Logo.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                String path = Server.MapPath("~/Images/Manufacturer/" + newName);
                Logo.SaveAs(path);
                model.Logo = newName;
            }
            _db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var model = _db.web_Partner.Find(id);
                _db.web_Partner.Remove(model);
                _db.SaveChanges();
                return Json(JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(JsonRequestBehavior.AllowGet);
            }
        }
    }
}