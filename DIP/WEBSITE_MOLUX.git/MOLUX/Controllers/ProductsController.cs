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
            IteamProduct item = new IteamProduct();
            var model = _db.Item.Find(id);
            //var model = _db.Item.Where(n => n.RowID == id).Single();

            if (model!=null)
            {
                
                item.RowID = model.RowID;
                item.Name = model.Name;
                item.Code = model.Code;
                item.Picture = model.Picture;
                item.Description = model.Description;
                item.Images = _db.web_ItemImage.Where(n => n.ItemId == id).ToList();
                item.MadeIn = model.MadeIn;
                item.Manufacturer_Code = model.Manufacturer_Code;
                item.ShortDesc = model.ShortDesc;
                item.Sale_Price = model.Sale_Price;
                item.MetaTitle = model.MetaTitle;
                item.MetaDescription = model.MetaDescription;
                item.Status = "còn hàng";
                item.guarantee ="12 tháng";
                var pld = _db.Price_Level_Detail.Where(n => n.Item_Code == model.Code).FirstOrDefault();
                if (pld!=null)
                {
                    item.Sale = pld.Price;
                    item.Item_Code = pld.Item_Code;
                    item.Item_Code_2 = pld.Item_Code_2;
                    var pl = _db.Price_Level.Where(n => n.RowID == pld.RowID).Single();
                    item.From_Date = (DateTime)pl.From_Date;
                    item.To_Date = (DateTime)pl.To_Date;
                }
            
                item.Sizes = _db.web_ItemSizeColor.Where(n => n.ItemId == id && n.IsDelete == false && n.Type == 2).ToList();
                item.Colors = _db.web_ItemSizeColor.Where(n => n.ItemId == id && n.IsDelete == false && n.Type == 1).ToList();

            }
            //ViewBag.ProductViewModel = new ProductViewModel()
            //{
            //    //sp cùng loaij
            //    TheSameCategory = _db.web_get15ProductSameCategory(id, model.Item_Category_Code).ToList(),
            //    //sp cung nha sx
            //    TheSameManufacturer = _db.web_get15ProductSameManufacturer(id, model.Manufacturer_Code).ToList(),
            //    TheOther = _db.web_get15ProductOther(id).ToList(),
            //    Color = _db.web_ItemSizeColor.Where(p=> p.ItemId == id && p.IsDelete == false && p.Type == 1).ToList(),
            //    Size = _db.web_ItemSizeColor.Where(p => p.ItemId == id && p.IsDelete == false && p.Type == 2).ToList()
            //};
            var spcungloai = _db.Get15_SPCungLoai(id, model.Item_Category_Code).ToList();
            ViewBag.spcungloai = spcungloai;
            ViewBag.spCungHang = _db.Get15_SPCungHang(id,model.Manufacturer_Code).ToList();
            ViewBag.spKhac = _db.Get15_SPKhac(id).ToList();
            return View(item);
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

        public ActionResult PromotionProduct(string sort ,int?page)
        {
            int pageN = page ?? 1;
            int pageS = 50;
            ViewBag.sort = sort;
            ViewBag.page = pageN;
            var model = new List<GetSPKhuyenMai_Result>();
            if (string.IsNullOrEmpty(sort))
            {
                model = _db.GetSPKhuyenMai().ToList();
            }
            else if (sort=="asc")
            {
                model = _db.GetSPKhuyenMai().OrderBy(n => n.Sale_Price).ToList();
            }
            else
            {
                model = _db.GetSPKhuyenMai().OrderByDescending(n => n.Sale_Price).ToList();
            }

            return View(model.ToPagedList(pageN, pageS));
        }

        #endregion
    }
}