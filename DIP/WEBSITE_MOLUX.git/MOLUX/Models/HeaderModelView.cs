using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MOLUX.Models
{
    public class HeaderModelView
    {
        public web_ConfigWebsite Config { get; set; }
        public List<web_NewsCategory> Menu { get; set; }
        public List<web_SocialNetwork> Social { get; set; }
    }

    public class FooterModelView
    {
        public web_ConfigWebsite Config { get; set; }
        public List<web_PaymentMethod> PaymentMethod { get; set; }
        public List<Manufacturer> Manufacturer { get; set; }
    }
}