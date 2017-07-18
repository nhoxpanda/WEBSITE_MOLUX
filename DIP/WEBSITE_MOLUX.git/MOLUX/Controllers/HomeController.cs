using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MOLUX.Models;

namespace MOLUX.Controllers
{
    public class HomeController : Controller
    {
        private BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();

        public ActionResult Index()
        {
            var model = new HomeViewModel()
            {
                ParentCode = _db.web_getParentCategory().ToList()
            };
            return View(model);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact(FormCollection fc)
        {
            if (Request["btnSend"] != null)
            {
                string body = "<p>KHÁCH HÀNG GỬI THÔNG TIN LIÊN HỆ</p>" +
                    "<p>Họ tên: " + fc["txtName"] + "</p>" +
                    "<p>Điện thoại: " + fc["txtPhone"] + "</p>" +
                    "<p>Email: " + fc["txtEmail"] + "</p>" +
                    "<p>Thông điệp: " + fc["txtNote"] + "</p>" +
                    "<p>Ngày liên hệ: " + DateTime.Now.ToString("dd-MM-yyyy") + "</p>";
                SendEmail.Email_With_CCandBCC("smtp.gmail.com", "panda.sendmail.demo@gmail.com", "zxcvbnm@123", "molux@molux.vn", "[MOLUX.VN] Khách hàng liên hệ", body);
                return Json(JsonRequestBehavior.AllowGet);
            }
            var model = new ContactViewModel()
            {
                Showroom = _db.web_GetCompanyById(1).FirstOrDefault(),
                Online = _db.web_GetCompanyById(2).FirstOrDefault(),
                Project = _db.web_GetCompanyById(3).FirstOrDefault(),
                Building = _db.web_GetCompanyById(4).FirstOrDefault(),
                HongKong = _db.web_GetCompanyById(5).FirstOrDefault(),
                VietNam = _db.web_GetCompanyById(6).FirstOrDefault()
            };
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult _Partial_Slider1()
        {
            var model = _db.web_getSliderByType(1).ToList();
            return PartialView("_Partial_Slider1", model);
        }

        [ChildActionOnly]
        public ActionResult _Partial_Slider2()
        {
            var model = _db.web_getSliderByType(2).ToList();
            return PartialView("_Partial_Slider2", model);
        }

        public ActionResult SubscribeEmail(string email)
        {
            var contact = new web_Contact
            {
                CreatedDate = DateTime.Now,
                Email = email,
                Message = "Đăng ký email nhận bản tin khuyến mãi",
            };
            _db.web_Contact.Add(contact);
            _db.SaveChanges();

            return Json(JsonRequestBehavior.AllowGet);
        }
    }
}