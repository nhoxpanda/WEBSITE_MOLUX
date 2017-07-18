using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MOLUX.Models;

namespace MOLUX.Controllers
{
    public class PartialViewController : Controller
    {
        // GET: PartialView

        private BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();

        [ChildActionOnly]
        public ActionResult _Partial_HeaderMenu()
        {
            var model = new HeaderModelView()
            {
                Config = _db.web_ConfigWebsite.Find(1),
                Menu = _db.web_NewsCategory.Where(p => p.ParentId == 0 && p.IsShow == true && p.IsDelete == false).OrderBy(p => p.Orders).ToList(),
                Social = _db.web_SocialNetwork.ToList()
            };
            return PartialView("_Partial_HeaderMenu");
        }

        [ChildActionOnly]
        public ActionResult _Partial_Footer()
        {
            var model = new FooterModelView
            {
                Config = _db.web_ConfigWebsite.Find(1),
                Manufacturer =  _db.Manufacturer.ToList(),
                PaymentMethod = _db.web_PaymentMethod.OrderBy(p => p.Orders).ToList()
            };
            return PartialView("_Partial_Footer", model);
        }

        [ChildActionOnly]
        public ActionResult _Partial_PaymentMethod()
        {
            var model = _db.web_getAllPaymentMethod().ToList();
            return PartialView("_Partial_PaymentMethod", model);
        }

        [ChildActionOnly]
        public ActionResult _Partial_Partner()
        {
            var model = _db.web_getAllPartner().ToList();
            return PartialView("_Partial_Partner", model);
        }

        [ChildActionOnly]
        public ActionResult _Partial_Advertisement()
        {
            var model = new AdvertisementViewModel
            {
                left = _db.web_Slider.Find(14),
                right = _db.web_Slider.Find(15),
                center = _db.web_Slider.Find(13)
            };
            return PartialView("_Partial_Advertisement", model);
        }
    }
}