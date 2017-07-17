using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM_TTV.Controllers
{
    
    public class CoachController : Controller
    {
        // GET: Coach
        //dsadas

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Lever()
        {
            return View();
        }

        // GET: Coach/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Coach/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Coach/Create
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

        // GET: Coach/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Coach/Edit/5
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

        // GET: Coach/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Coach/Delete/5
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
