using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MOLUX
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Intro",
                url: "gioi-thieu",
                defaults: new { controller = "Home", action = "About" }
            );
            routes.MapRoute(
                 name: "Contact",
                 url: "lien-he",
                 defaults: new { controller = "Home", action = "Contact" }
             );

            routes.MapRoute(
                name: "Login",
                url: "dang-nhap",
                defaults: new { controller = "Account", action = "Login" }
            );

            routes.MapRoute(
                name: "Register",
                url: "dang-ky",
                defaults: new { controller = "Account", action = "Register" }
            );

            routes.MapRoute(
                name: "Orders",
                url: "quan-ly-don-hang/{username}",
                defaults: new { controller = "Customer", action = "OrdersManage", username = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "InfoAccount",
                url: "quan-ly-tai-khoan",
                defaults: new { controller = "Customer", action = "Index" }
            );

            routes.MapRoute(
                 name: "Promotion",
                 url: "san-pham-khuyen-mai",
                 defaults: new { controller = "Products", action = "PromotionProduct" }
             );

            routes.MapRoute(
                name: "Search",
                url: "ket-qua-tim-kiem",
                defaults: new { controller = "Products", action = "Search" }
            );

            routes.MapRoute(
                name: "ShoppingCart",
                url: "gio-hang",
                defaults: new { controller = "ShoppingCart", action = "Index" }
            );

            routes.MapRoute(
                name: "IndexNews",
                url: "tin-tuc/p{id}/{name}",
                defaults: new { controller = "News", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "CateNews",
                url: "tin-tuc/c{id}/{name}",
                defaults: new { controller = "News", action = "Cate", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "DetailNews",
                url: "tin-tuc/d{id}/{name}",
                defaults: new { controller = "News", action = "Detail", id = UrlParameter.Optional }
            );
            
            routes.MapRoute(
                name: "IndexProducts",
                url: "san-pham/pr{id}-{code}/{name}",
                defaults: new { controller = "Products", action = "Index", id = UrlParameter.Optional, code = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "CateProducts",
                url: "san-pham/ch{id}-{code}/{name}",
                defaults: new { controller = "Products", action = "Cate", id = UrlParameter.Optional, code = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ListProducts",
                url: "san-pham/li{id}-{code}/{name}",
                defaults: new { controller = "Products", action = "List", id = UrlParameter.Optional, code = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "DetailProducts",
                url: "san-pham/ct{id}/{name}",
                defaults: new { controller = "Products", action = "Detail", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
