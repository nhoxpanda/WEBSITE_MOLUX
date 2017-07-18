using MOLUX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MOLUX.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ContactManageController : Controller
    {
        private BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();

        // GET: Admin/ContactManage
        public ActionResult Index()
        {
            return View(_db.web_Contact.OrderByDescending(p => p.CreatedDate).ToList());
        }
    }
}