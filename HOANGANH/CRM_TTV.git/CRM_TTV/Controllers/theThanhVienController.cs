using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM_TTV.Controllers
{
    
    public class theThanhVienController : Controller
    {
        // GET: theThanhVien
        public ActionResult Index()
        {
            return View();
        }

        // GET: theThanhVien/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: theThanhVien/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: theThanhVien/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: theThanhVien/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: theThanhVien/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: theThanhVien/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: theThanhVien/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
