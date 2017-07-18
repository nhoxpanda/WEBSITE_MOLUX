using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MOLUX.Models;
using MOLUX.Helper;
using PagedList;
using PagedList.Mvc;

namespace MOLUX.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products

        private BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();

        #region Danh mục cấp 1
        /// <summary>
        /// load danh mục sp cấp 1
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int id, string code)
        {
            var model = new ProductViewModel()
            {
                Level2 = _db.web_getChildCategory(id).ToList()
            };
            ViewBag.Item_Category = _db.Item_Category.Find(id);
            return View(model);
        }
        #endregion

        #region Danh mục cấp 2

        /// <summary>
        /// load danh mục sp cấp 2
        /// </summary>
        /// <returns></returns>
        public ActionResult Cate(int id, string code, string sort, string manufacturer, int? page = 1)
        {
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var cate = _db.web_getItemCategoryById(id).FirstOrDefault();
            ViewBag.CateViewModel = new CateViewModel()
            {
                Category = _db.web_getChildCategory(id).ToList(),
                Id = id,
                Code = code,
                Page = page,
                SortCode = !string.IsNullOrEmpty(sort) ? sort : "0",
                ManuCode = !string.IsNullOrEmpty(sort) ? manufacturer : "0",
                CateName = cate != null ? Common.MakeUrlSlug(cate.Name) : "",
                TitleName = cate.Name,
                CategoryLeft2 = _db.web_getCategoryLevel3ByLevel2(id).ToList(),
                Manufacturer = _db.web_getAllManufacturerList().ToList(),
                MetaDescription = cate.MetaDescription,
                MetaTitle = cate.MetaTitle
            };
            var model = _db.web_getAllItemLevel2Filter(id, manufacturer, sort).ToList();
            return View(model.ToPagedList(pageNumber, pageSize));
        }

        #endregion

        #region Danh mục cấp 3

        /// <summary>
        /// load sản phẩm theo danh mục cấp 3
        /// </summary>
        /// <returns></returns>
        public ActionResult List(int id, string code, string sort, string manufacturer, int? page = 1)
        {
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var cate = _db.web_getItemCategoryById(id).FirstOrDefault();
            ViewBag.CateViewModel = new CateViewModel()
            {
                CategoryLevel3 = _db.web_getSameLevelCategory(id).ToList(),
                Id = id,
                Code = code,
                Page = page,
                SortCode = !string.IsNullOrEmpty(sort) ? sort : "0",
                ManuCode = !string.IsNullOrEmpty(sort) ? manufacturer : "0",
                CateName = cate != null ? Common.MakeUrlSlug(cate.Name) : "",
                TitleName = cate.Name,
                Manufacturer = _db.web_getAllManufacturerList().ToList(),
                MetaTitle = cate.MetaTitle,
                MetaDescription = cate.MetaDescription
            };
            var model = _db.web_getProductByCategoryFilter(code, manufacturer, sort).ToList();
            return View(model.ToPagedList(pageNumber, pageSize));
        }

        #endregion

        #region Chi tiết sản phẩm
        public ActionResult Detail(int id)
        {
            var model = _db.Item.Find(id);
            ViewBag.ProductViewModel = new ProductViewModel()
            {
                TheSameCategory = _db.web_get15ProductSameCategory(id, model.Item_Category_Code).ToList(),
                TheSameManufacturer = _db.web_get15ProductSameManufacturer(id, model.Manufacturer_Code).ToList(),
                TheOther = _db.web_get15ProductOther(id).ToList(),
                Color = _db.web_ItemSizeColor.Where(p=> p.ItemId == id && p.IsDelete == false && p.Type == 1).ToList(),
                Size = _db.web_ItemSizeColor.Where(p => p.ItemId == id && p.IsDelete == false && p.Type == 2).ToList()
            };
            return View(model);
        }
        #endregion

        #region Tìm kiếm sản phẩm

        public ActionResult Search(string keyword, string sort, int? page = 1)
        {
            var model = _db.web_getMultiSearchItem(keyword, 5, page - 1, sort).ToList();
            ViewBag.SearchViewModel = new SearchViewModel
            {
                page = page,
                keyword = keyword,
                sort = sort
            };
            return View(model);
        }

        #endregion

        #region SP khuyến mãi

        public ActionResult PromotionProduct()
        {
            return View();
        }

        #endregion
    }
}