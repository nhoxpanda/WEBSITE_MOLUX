using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MOLUX.Models;
using System.Web.Mvc;

namespace MOLUX.Helper
{
    public class ShoppingCart
    {
        private static BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();
        string ShoppingCartId { get; set; }
        public const string web_CartessionKey = "CartId";
        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }
        // Helper method to simplify shopping cart calls
        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }
        public int AddToCart(Item item, int color, int size,int number,decimal price)
        {
            // Get the matching cart and Item instances
            var cartItem = _db.web_Cart.SingleOrDefault(
                c => c.CartId == ShoppingCartId
                && c.ItemId == item.RowID);

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new web_Cart
                {
                    ItemId = item.RowID,
                    CartId = ShoppingCartId,
                    Count = number,
                    DateCreated = DateTime.Now,
                    SizeId = size == 0 ? (int?)null : size,
                    ColorId = color == 0 ? (int?)null : color,
                    UnitPrice =price
                };
                _db.web_Cart.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart, 
                // then add one to the quantity
                cartItem.Count++;
            }
            // Save changes
            _db.SaveChanges();

            int countTotal = _db.web_Cart.Where(c => c.CartId == ShoppingCartId).Count();
            return countTotal;
        }

        public int RemoveFromCart(int id)
        {
            // Get the cart
            var listItem = _db.web_Cart.Where(cart => cart.CartId == ShoppingCartId).ToList();
            var cartItem = listItem.FirstOrDefault(cart => cart.Id == id);
            int itemCount = 0;
            if (cartItem != null)
            {
                _db.web_Cart.Remove(cartItem);
                _db.SaveChanges();
                itemCount = listItem.Count() - 1;
            }
            return itemCount;
        }

        public void EmptyCart()
        {
            var cartItems = _db.web_Cart.Where(
                cart => cart.CartId == ShoppingCartId);

            foreach (var cartItem in cartItems)
            {
                _db.web_Cart.Remove(cartItem);
            }
            // Save changes
            _db.SaveChanges();
        }

        public int CartCount()
        {
            int? count = (from cartItems in _db.web_Cart
                          where cartItems.CartId == ShoppingCartId
                          select cartItems).Count();
            // Return 0 if all entries are null
            return count ?? 0;
        }

        public int GetCount()
        {
            // Get the count of each item in the cart and sum them up
            int? count = (from cartItems in _db.web_Cart
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.Count).Sum();
            // Return 0 if all entries are null
            return count ?? 0;
        }
        public decimal GetTotal()
        {
            // Multiply Item Sale_Price by count of that Item to get 
            // the current Sale_Price for each of those Items in the cart
            // sum all Item Sale_Price totals to get the cart total
            decimal? total = (from cartItems in _db.web_Cart
                              where cartItems.CartId == ShoppingCartId
                              select (int?)cartItems.Count * cartItems.UnitPrice).Sum();

            return total ?? decimal.Zero;
        }

        public int CreateOrder(Sales_Order_Master order)
        {
            decimal? orderTotal = 0;

            var cartItems = _db.web_Cart.Where(p => p.CartId == ShoppingCartId).ToList();
            // Iterate over the items in the cart, 
            // adding the order details for each
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
                    Amount__LCY_ = item.Item.Sale_Price * item.Count
                };
                // Set the order total of the shopping cart
                orderTotal += (item.Count * item.Item.Sale_Price);
                _db.Sales_Order_Detail.Add(orderDetail);
            }
            // Save the order
            _db.SaveChanges();
            // Empty the shopping cart
            EmptyCart();
            // Return the OrderId as the confirmation number
            return order.RowID;
        }

        // We're using HttpContextBase to allow access to cookies.
        public string GetCartId(HttpContextBase context)
        {
            if (context.Session[web_CartessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[web_CartessionKey] =
                        context.User.Identity.Name;
                }
                else
                {
                    // Generate a new random GUID using System.Guid class
                    Guid tempCartId = Guid.NewGuid();
                    // Send tempCartId back to client as a cookie
                    context.Session[web_CartessionKey] = tempCartId.ToString();
                }
            }
            return context.Session[web_CartessionKey].ToString();
        }
        // When a user has logged in, migrate their shopping cart to
        // be associated with their username
        public void MigrateCart(string userName)
        {
            var shoppingCart = _db.web_Cart.Where(
                c => c.CartId == ShoppingCartId);

            foreach (web_Cart item in shoppingCart)
            {
                item.CartId = userName;
            }
            _db.SaveChanges();
        }
    }
}