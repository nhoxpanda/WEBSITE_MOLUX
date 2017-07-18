using MOLUX.Models;
using MOLUX.Areas.Admin.Models;
using MOLUX.Areas.Admin.Repository;
using MOLUX.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MOLUX.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MenuManageController : Controller
    {
        MenuRepository menuRepo = new MenuRepository();
        private BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();
        // GET: Admin/MenuManage

        #region Menu
        public ActionResult Index()
        {
            MenuViewModel menuModel = new MenuViewModel();
            menuModel.listMenu = menuRepo.getAllMenu();
            menuModel.listMenuHomePage = menuRepo.getAllMenuHomePage();
            return View(menuModel);
        }

        public ActionResult DemoMenu()
        {
            return View();
        }

        public ActionResult AddMenu()
        {
            MenuViewModel menuModel = new MenuViewModel();
            menuModel.listMenuItem = menuRepo.getAllMenu().Select(
                  x => new SelectListItem { Value = x.Id.ToString(), Text = x.Title }).ToList();
            return View(menuModel);
        }

        [HttpPost]
        public ActionResult AddMenu(MenuViewModel model)
        {
            if (model.menu.ParentId == null)
            {
                model.menu.ParentId = 0;
            }
            model.menu.IsDelete = false;
            model.menu.CreatedDate = DateTime.Now;
            model.menu.Type = Constant.Menu;
            menuRepo.AddMenu(model.menu);
            return RedirectToAction("Index", "MenuManage");
        }

        public ActionResult EditMenu(int id, MenuViewModel model)
        {
            if (Request["btnLuu"] != null)
            {
                {
                    _db.Entry(model.menu).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    return RedirectToAction("Index", "MenuManage");
                }
            }
            var item = new MenuViewModel
            {
                menu = _db.web_NewsCategory.Find(id)
            };
            return View(item);
        }

        public JsonResult ChangeIsShow(int menuID)
        {
            var check = menuRepo.ChangeShow(menuID);
            return Json(new { result = check }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteMenu(int menuID)
        {
            var check = menuRepo.DeleteMenu(menuID);
            return Json(new { result = check }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Category

        public ActionResult Category()
        {
            MenuViewModel cateModel = new MenuViewModel();
            cateModel.listMenuItem = menuRepo.getAllMenu().Select(
                  x => new SelectListItem { Value = x.Id.ToString(), Text = x.Title }).ToList();
            cateModel.listCate = menuRepo.getAllCatgory();
            cateModel.listMenu = menuRepo.getAllMenu();
            return View(cateModel);
        }

        public ActionResult AddCategory()
        {
            MenuViewModel cateModel = new MenuViewModel();
            cateModel.listMenuItem = menuRepo.getAllMenuShow().Select(
                  x => new SelectListItem { Value = x.Id.ToString(), Text = x.Title }).ToList();
            //cateModel.listCate = menuRepo.getAllCatgory();
            //cateModel.listMenu = menuRepo.getAllMenu();
            return View(cateModel);
        }

        [HttpPost]
        public ActionResult AddCategory(MenuViewModel model)
        {
            model.menu.IsDelete = false;
            model.menu.CreatedDate = DateTime.Now;
            model.menu.Type = Constant.Category;
            model.menu.IsShow = true;
            menuRepo.AddMenu(model.menu);
            return RedirectToAction("Category", "MenuManage");
        }

        public ActionResult EditCategory(int id, MenuViewModel model)
        {
            if (Request["btnLuu"] != null)
            {
                {
                    _db.Entry(model.menu).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    return RedirectToAction("Category", "MenuManage");
                }
            }
            var item = new MenuViewModel
            {
                menu = _db.web_NewsCategory.Find(id)
            };
            
            return View(item);
        }

        #endregion

    }
}