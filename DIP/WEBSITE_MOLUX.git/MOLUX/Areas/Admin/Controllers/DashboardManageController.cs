using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MOLUX.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardManageController : Controller
    {
        // GET: Admin/DashboardManage
        public ActionResult Index()
        {
            return View();
        }
    }
}