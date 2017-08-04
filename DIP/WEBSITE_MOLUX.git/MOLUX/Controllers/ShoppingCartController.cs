using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MOLUX.Models;
using MOLUX.Helper;

namespace MOLUX.Controllers
{
    public class ShoppingCartController : Controller
    {
        // GET: ShoppingCart
        private BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();

        public ActionResult Index()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = _db.web_Cart.AsEnumerable().Where(p=>p.CartId == cart.GetCartId(this.HttpContext)).ToList(),
                CartTotal = cart.GetTotal(),
            };
            if (Session["UserLogin"] != null)
            {
                string username = Session["UserLogin"].ToString();
                var customer = _db.Customer.FirstOrDefault(p => p.User_Login == username);
                if (customer != null)
                {
                    viewModel.SingleCustomer = customer;
                }
            }
            // Return the view
            ViewBag.PaymentMethodWeb = _db.Payment_Method.Where(p => p.IsWeb == true).ToList();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddToCart(int id,int? number,decimal price)
        {
            var product = _db.Item.Find(id);
            var cart = ShoppingCart.GetCart(this.HttpContext);
            int data = cart.AddToCart(product, number??1, price);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            // Remove the item from the cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Remove from cart
            int itemCount = cart.RemoveFromCart(id);

            var results = new ShoppingCartRemoveViewModel
            {
                CartTotal = cart.GetTotal(),
                CartCount = cart.CartCount(),
                ItemCount = itemCount,
                StrCartTotal = string.Format("{0:0,0₫}", cart.GetTotal()).Replace(",",".")
            };

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int id, int qty)
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            var item = _db.web_Cart.Find(id);
            item.Count = qty;
            _db.SaveChanges();

            var results = new ShoppingCartRemoveViewModel
            {
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                StrCartTotal= string.Format("{0:0,0₫}", cart.GetTotal()).Replace(",", ".")
            };

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CalculatorPriceQuantity(string price, int qty)
        {
            decimal? fmPrice = Convert.ToDecimal(price.Replace(".", "").Replace("₫", ""));
            string data = string.Format("{0:0,0₫}", fmPrice * qty).Replace(",", ".");
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        public ActionResult _Partial_CartCount()

        {
           
            var cart = ShoppingCart.GetCart(this.HttpContext);
            int? count = _db.web_Cart.AsEnumerable().Where(p => p.CartId == cart.GetCartId(this.HttpContext)).Count();
            // Return 0 if all entries are null
            var model = new ShoppingCartRemoveViewModel
            {
                CartCount = count ?? 0
            };
            return PartialView("_Partial_CartCount", model);
        }
    }
}