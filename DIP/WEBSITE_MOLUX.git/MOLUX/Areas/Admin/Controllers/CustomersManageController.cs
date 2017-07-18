using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MOLUX.Models;
using MOLUX.Helper;

namespace MOLUX.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CustomersManageController : Controller
    {
        // GET: Admin/CustomersManage
        private BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();

        public ActionResult Index()
        {
            return View(_db.Customer.OrderByDescending(p => p.Created_Date).ToList());
        }
    }
}