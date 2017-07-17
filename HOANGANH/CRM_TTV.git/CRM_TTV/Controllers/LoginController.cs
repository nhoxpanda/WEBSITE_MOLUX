using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CRM_TTV.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        private CRM_TTVEntities db = new CRM_TTVEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Authentication(string username, string password)
        {
            try
            {
                var userLogin = UserManager(username, password);

                if (userLogin != null)
                {
                    return Json("Successfully!");
                }
                else
                {
                    return HttpNotFound();
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public bool LogOut()
        {
            Session.Clear();
            return true;
        }
        public tbUser UserManager(string username, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    return null;
                }
                using (var db = new CRM_TTVEntities())
                {
                    //mã hóa pass
                    string passEncy = password;
                    var item = db.tbUsers.FirstOrDefault(u => u.email == username
                        && u.password == passEncy);
                    if (item != null)
                    {
                        RolesController rcl = new RolesController();
                        if (string.IsNullOrEmpty(item.userRole))
                        {
                            Session["Role"] = db.tbRoles.FirstOrDefault(x => x.roleID == item.roleID).role; //quyền theo nhóm
                            Session["Menu"] = rcl.UserMenuRole(item.roleID.ToString());
                        }
                        else
                        {
                            Session["Role"] = item.userRole; //quyền đặc biệt
                            Session["Menu"] = rcl.UserMenuRole("user_" + item.userID);
                        }
                        Session["user"] = item;
                        return item;
                    }
                    return null;
                    
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult Viewss()
        {
            return View();
        }
    }
}