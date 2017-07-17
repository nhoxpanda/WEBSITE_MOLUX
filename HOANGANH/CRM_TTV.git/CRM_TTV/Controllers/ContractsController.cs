using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM_TTV.Controllers
{
    
    public class ContractsController : Controller
    {
        // GET: Contracts
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ContractStatus()
        {
            return View();
        }
        public ActionResult ContractType()
        {
            return View();
        }
        public ActionResult PaymentType()
        {
            return View();
        }
        
        // GET: Contracts/Details/5
        //dsadasdas
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Contracts/Create
        //dsadas
        public ActionResult Create()
        {
            return View();
        }

        // POST: Contracts/Create
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

        // GET: Contracts/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Contracts/Edit/5
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

        // GET: Contracts/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Contracts/Delete/5
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
