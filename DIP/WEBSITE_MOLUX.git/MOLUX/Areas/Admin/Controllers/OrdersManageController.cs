using MOLUX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MOLUX.Helper;

namespace MOLUX.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrdersManageController : Controller
    {
        // GET: Admin/OrdersManage
        private BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();

        public ActionResult Index()
        {
            return View(_db.Sales_Order_Master.OrderByDescending(p => p.Created_Date).ToList());
        }

        [ChildActionOnly]
        public ActionResult _Partial_Detail()
        {
            return PartialView("~/Areas/Admin/Views/OrdersManage/_Partial_Detail.cshtml");
        }

        [HttpPost]
        public ActionResult Detail(string code)
        {
            var model = _db.Sales_Order_Detail.Where(p => p.Sales_Order_Code == code).ToList();
            return PartialView("~/Areas/Admin/Views/OrdersManage/_Partial_Detail.cshtml", model);
        }
    }
}