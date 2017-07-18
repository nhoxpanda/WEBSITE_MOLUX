using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MOLUX.Models;

namespace MOLUX.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ConfigManageController : Controller
    {
        // GET: Admin/ConfigManage

        private BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();

        #region website
        [ValidateInput(false)]
        public ActionResult Index(web_ConfigWebsite model, HttpPostedFileBase Logo, HttpPostedFileBase LogoBCT, HttpPostedFileBase IconSale, HttpPostedFileBase IconNew)
        {
            if (Request["btnSave"] != null)
            {
                // logo
                if (Logo != null)
                {
                    String newName = Logo.FileName.Insert(Logo.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Images/Logo/" + newName);
                    Logo.SaveAs(path);
                    model.Logo = newName;
                }
                // logobct
                if (LogoBCT != null)
                {
                    String newName = LogoBCT.FileName.Insert(LogoBCT.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Images/Logo/" + newName);
                    LogoBCT.SaveAs(path);
                    model.LogoBCT = newName;
                }
                // iconSale
                if (IconSale != null)
                {
                    String newName = IconSale.FileName.Insert(IconSale.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Images/Logo/" + newName);
                    IconSale.SaveAs(path);
                    model.IconSale = newName;
                }
                // iconNew
                if (IconNew != null)
                {
                    String newName = IconNew.FileName.Insert(IconNew.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Images/Logo/" + newName);
                    IconNew.SaveAs(path);
                    model.IconNew = newName;
                }
                _db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
            }
            return View(_db.web_ConfigWebsite.Find(1));
        }
        #endregion

        #region config footer
        public ActionResult Footer()
        {
            var model = _db.web_Footer.ToList();
            return View(model);
        }

        public ActionResult Create(web_Footer model)
        {
            _db.web_Footer.Add(model);
            _db.SaveChanges();

            return RedirectToAction("Footer");
        }

        public ActionResult Edit(int id)
        {
            var model = _db.web_Footer.Find(id);
            return PartialView("_Partial_Update", model);
        }

        public ActionResult Update(web_Footer model)
        {
            _db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();

            return RedirectToAction("Footer");
        }

        public ActionResult Delete(int id)
        {
            _db.web_Footer.Remove(_db.web_Footer.FirstOrDefault(p=>p.Id == id));
            _db.SaveChanges();

            return RedirectToAction("Footer");
        }

        public ActionResult SaveTitleFooter(int id, string title)
        {
            var model = _db.web_ConfigWebsite.Find(1);
            if (id == 1)
            {
                model.TitleFooter1 = title;
            }
            else
            {
                model.TitleFooter2 = title;
            }
            _db.SaveChanges();

            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region company list
        public ActionResult BusinessDepartment()
        {
            return View(_db.web_Company.ToList());
        }

        [HttpPost]
        public ActionResult EditCompany(int id)
        {
            return PartialView("_Partial_UpdateCompany", _db.web_Company.Find(id));
        }

        [HttpPost]
        public ActionResult UpdateCompany(web_Company model)
        {
            _db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("BusinessDepartment");
        }
        #endregion
    }
}