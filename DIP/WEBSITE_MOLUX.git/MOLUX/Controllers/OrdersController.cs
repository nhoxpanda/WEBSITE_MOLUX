using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MOLUX.Models;
using MOLUX.Helper;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MOLUX.Controllers
{
    public class OrdersController : Controller
    {
        // GET: Orders

        private BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();
        private ApplicationDbContext context = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Buy(FormCollection fc)
        {
            // tạo khách hàng mới
            var customer = new Customer();
            if (Session["UserLogin"] != null)
            {
                var cus = _db.Customer.AsEnumerable().FirstOrDefault(p => p.User_Login == Session["UserLogin"].ToString());
                customer = cus;
            }
            else
            {
                var checkCustomer = _db.Customer.AsEnumerable().FirstOrDefault(p => p.Name == fc["FullName"] && p.Phone == fc["Phone"]);
                if (checkCustomer == null) // chưa có khách hàng này
                {
                    customer.Name = fc["FullName"];
                    customer.Search_Name = fc["FullName"];
                    customer.Phone = fc["Phone"];
                    customer.Address = fc["Address"];
                    customer.Ship_Address = fc["Address"];
                    customer.Bill_Address = fc["Address"];
                    customer.Payment_Method = fc["PaymentMethod"];
                    customer.Description = fc["Note"];
                    customer.Created_Date = DateTime.Now;
                    customer.Modified_Date = DateTime.Now;
                    customer.Code = Customers.GenerateCode();
                    customer.Country_Code = fc["Country"];
                    customer.City_Code = fc["City"];
                    customer.Email = fc["Email"];
                    customer.Clocked = 0;
                    customer.User_Login = fc["Email"];

                    _db.Customer.Add(customer);
                    _db.SaveChanges();

                    try
                    {
                        // tạo tài khoản đăng nhập cho khách hàng
                        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                        var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                        var user = new ApplicationUser { UserName = customer.User_Login, Email = customer.Email };
                        var chkUser = UserManager.Create(user, customer.Phone);
                        if (chkUser.Succeeded)
                        {
                            // phân quyền
                            var result = UserManager.AddToRole(user.Id, "Registered");
                        }

                    }
                    catch (Exception ex)
                    {
                        string str = ex.Message;
                    }
                }
            }
            
            // tạo đơn hàng
            var order = new Sales_Order_Master()
            {
                Bill_Address = customer.Bill_Address,
                Bill_Customer_Code = customer.Code,
                Bill_Email = customer.Email,
                Code = Customers.GenerateOrderCode(),
                Bill_Phone = customer.Phone,
                Created_Date = DateTime.Now,
                Customer_Code = customer.Code,
                Payment_Method_Code = fc["PaymentMethod"],
                Sale_Note = fc["Note"],
                Currency_Code = "VND",
                Ship_Address = customer.Address,
                Ship_Date = !string.IsNullOrEmpty(fc["ShipDate"]) ? Convert.ToDateTime(fc["ShipDate"]) : DateTime.Now.AddDays(7),
                Ship_Phone = customer.Phone,
                User_Login  = "ADMIN",
                Saler_Code = "ADMIN"
            };
            _db.Sales_Order_Master.Add(order);
            _db.SaveChanges();
            var cart = ShoppingCart.GetCart(this.HttpContext);
            // tạo chi tiết đơn hàng
            decimal? orderTotal = 0;
            var cartItems = _db.web_Cart.AsEnumerable().Where(p => p.CartId == cart.GetCartId(this.HttpContext)).ToList();
            foreach (var item in cartItems)
            {
                var orderDetail = new Sales_Order_Detail
                {
                    Item_Code = item.Item.Code,
                    Qty = item.Count,
                    Rate = item.Item.Sale_Price,
                    Sales_Order_Code = order.Code,
                    Amount = item.Item.Sale_Price * item.Count,
                    Rate__LCY_ = item.Item.Sale_Price,
                    Amount__LCY_ = item.Item.Sale_Price * item.Count,
                };
                // Set the order total of the shopping cart
                orderTotal += (item.Count * item.Item.Sale_Price);
                _db.Sales_Order_Detail.Add(orderDetail);
            }
            _db.SaveChanges();
            // xóa giỏ hàng
            cart.EmptyCart();
            // gửi mail thông báo cho khách hàng
            try
            {
                SendEmail.Email_With_CCandBCC("smtp.gmail.com", "panda.sendmail.demo@gmailcom", "zxcvbnm@123", fc["Email"], "[MOLUX.VN] THÔNG BÁO ĐẶT HÀNG THÀNH CÔNG.", Customers.BodyEmail(order.RowID));
            }
            catch { }
            return Json(JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// load danh sách thành phố của quốc gia
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public JsonResult GetCityList(string code)
        {
            var model = _db.City.Where(p => p.Country_Code == code).ToList();
            return Json(new SelectList(model, "Code", "Name"), JsonRequestBehavior.AllowGet);
        }
    }

}

