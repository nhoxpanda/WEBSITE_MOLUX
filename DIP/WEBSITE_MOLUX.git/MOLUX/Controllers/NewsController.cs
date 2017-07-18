using MOLUX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MOLUX.Helper;
using PagedList;
using PagedList.Mvc;

namespace MOLUX.Controllers
{
    public class NewsController : Controller
    {
        // GET: News

        private BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();

        /// <summary>
        /// load danh mục tin cấp 1
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int id)
        {
            var child = _db.web_getChildCateNews(id).ToList();
            if (child.Count() > 0)
            {
                return View(child);
            }
            else
            {
                var page = _db.web_getAllNewsByCate(id).ToList();
                if (page.Count() == 1)
                {
                    return Redirect("/tin-tuc/d" + page.FirstOrDefault().Id + "/" + Common.MakeUrlSlug(page.FirstOrDefault().Title));
                }
                else if (page.Count() > 0)
                {
                    var cate = _db.web_NewsCategory.Find(page.FirstOrDefault().NewsCategoryId);
                    return Redirect("/tin-tuc/c" + cate.Id + "/" + Common.MakeUrlSlug(cate.Title));
                }
            }
            return View();
        }

        /// <summary>
        /// load danh mục tin cấp 2
        /// </summary>
        /// <returns></returns>
        public ActionResult Cate(int id, int? page = 1)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var model = _db.web_getAllNewsByCate(id).ToList();
            var demo = _db.web_NewsCategory.Find(id);
            ViewBag.NewsCategory = demo;
            ViewBag.ListCategory = _db.web_NewsCategory.Where(p=>p.ParentId == demo.ParentId && p.IsShow== true && p.IsDelete == false).ToList();
            ViewBag.Page = page;
            return View(model.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// chi tiết tin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(int id)
        {
            var model = _db.web_News.Find(id);
            var cate = _db.web_NewsCategory.Find(model.NewsCategoryId);
            ViewBag.NewsViewModel = new NewsViewModel()
            {
                SameProduct = _db.web_getProductSameCategory(id, model.NewsCategoryId).ToList(),
                DifferenceProduct = _db.web_getProductDifferenceCategory(id, model.NewsCategoryId).ToList(),
                ListCategory = _db.web_NewsCategory.Where(p => p.ParentId == cate.ParentId && p.IsShow == true && p.IsDelete == false).ToList(),
                ListViews = _db.web_News.Where(p => p.Id != id).OrderByDescending(p => p.Views).Take(7).ToList()
            };
            return View(model);
        }
    }
}