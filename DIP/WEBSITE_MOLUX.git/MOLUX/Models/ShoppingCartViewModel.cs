using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MOLUX.Models
{
    public class ShoppingCartViewModel
    {
        public List<web_Cart> CartItems { get; set; }
        public decimal CartTotal { get; set; }
        public Customer SingleCustomer { get; set; }
    }

    public class ShoppingCartRemoveViewModel
    {
        public string Message { get; set; }
        public decimal CartTotal { get; set; }
        public int CartCount { get; set; }
        public int ItemCount { get; set; }
        public int DeleteId { get; set; }
        public string StrCartTotal { get; set; }
    }
}