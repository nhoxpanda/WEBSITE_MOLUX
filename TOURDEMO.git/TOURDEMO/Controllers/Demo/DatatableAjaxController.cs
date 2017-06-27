using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRM.Core;
using CRM.Infrastructure;

namespace TOURDEMO.Controllers.Demo
{
    public class DatatableAjaxController : Controller
    {
        // GET: DatatableAjax

        private DataContext _db = new DataContext();

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult GetData()
        {
            var model = _db.tbl_Customer.ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}