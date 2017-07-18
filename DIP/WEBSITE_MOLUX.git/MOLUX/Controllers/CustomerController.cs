using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MOLUX.Models;
using MOLUX.Helper;

namespace MOLUX.Controllers
{
    [Authorize(Roles = "Registered")]
    public class CustomerController : Controller
    {
        // GET: Customer

        private BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();

        public ActionResult Index(Customer model)
        {
            if (Session["UserLogin"] != null)
            {
                string username = Session["UserLogin"].ToString();
                var customer = _db.Customer.AsEnumerable().FirstOrDefault(p => p.User_Login == username);
                if (customer != null)
                {
                    if(Request["btnSave"] != null)
                    {
                        customer.Name = model.Name;
                        customer.Search_Name = model.Name;
                        customer.Phone = model.Phone;
                        customer.Email = model.Email;
                        customer.Address = model.Address;
                        customer.Country_Code = model.Country_Code;
                        customer.City_Code = model.City_Code;
                        _db.SaveChanges();

                        return Json(JsonRequestBehavior.AllowGet);
                    }
                    return View(customer);
                }
            }
            return Redirect("/dang-nhap");
        }

        public ActionResult OrdersManage()
        {
            if (Session["UserLogin"] != null)
            {
                string username = Session["UserLogin"].ToString();
                var customer = _db.Customer.AsEnumerable().FirstOrDefault(p => p.User_Login == username);
                if (customer != null)
                {
                    var model = _db.Sales_Order_Master.Where(p => p.Customer_Code == customer.Code).ToList();
                    return View(model);
                }
            }
            return Redirect("/dang-nhap");
        }

        public ActionResult OrdersDetail(string code)
        {
            var model = _db.Sales_Order_Detail.Where(p => p.Sales_Order_Code == code).ToList();
            return PartialView("_Partial_Detail", model);
        }
    }
}