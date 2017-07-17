using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CRM_TTV.Authorize
{
    public class DinamicAuthorize : AuthorizeAttribute
    {
        public string UserRole { get; set; }
        protected override bool AuthorizeCore(HttpContextBase context)
        {
            var rd = context.Request.RequestContext.RouteData;
            string currentAction = rd.GetRequiredString("action");//Index
            string currentController = rd.GetRequiredString("controller");//staffs
            var url = context.Request.Url;
            
            
            bool Authorized = base.AuthorizeCore(context);
            if (!Authorized)
            {
                return false;
            }
            else
            {
                var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    if (authTicket != null && !authTicket.Expired)
                    {
                        string curentAction = "F-5-A-1";//get từ csdl
                        if (authTicket.UserData.IndexOf(curentAction) != -1)
                        {
                            return true;
                        }
                    }
                }


                //string Urole = "M-33,M-34,M-35,M-32,M-1,F-4-A-1,F-4-A-2,F-5-A-1,F-5-A-2,F-5-A-3,F-15-A-2,F-15-A-10,F-4,F-5,F-15,M-2,F-9-A-1,F-9-A-2,F-9-A-3,F-9-A-5,F-10-A-1,F-10-A-2,F-9,F-10,M-17,M-18,M-19,M-20,M-26,M-27,M-28,M-21,M-22,M-23,M-24,M-25,M-29,M-30,M-31,M-36,M-37,M-38,M-39,M-40,M-41,M-42,M-43,M-44,M-45,M-46,M-47,M-48";
                
            }
            return false;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectResult("~/Error/AcessDenied");
            }
        }
    }
}