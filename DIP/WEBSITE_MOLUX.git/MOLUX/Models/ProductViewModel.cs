using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MOLUX.Models
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public List<web_getChildCategory_Result> Level2 { get; set; }
        public List<web_get15ProductSameCategory_Result> TheSameCategory { get; set; }
        public List<web_get15ProductSameManufacturer_Result> TheSameManufacturer { get; set; }
        public List<web_get15ProductOther_Result> TheOther { get; set; }
        public List<web_ItemSizeColor> Color { get; set; }
        public List<web_ItemSizeColor> Size { get; set; }
        public List<web_ItemImage> Images { get; set; }
    }

    public class CateViewModel
    {
        public int Id { get; set; }
        public int? Page { get; set; }
        public string Code { get; set; }
        public string TitleName { get; set; }
        public string SortCode { get; set; }
        public string ManuCode { get; set; }
        public string CateName { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public List<web_getChildCategory_Result> Category { get; set; }
        public List<web_getAllManufacturerList_Result> Manufacturer { get; set; }
        public List<web_getCategoryLevel3ByLevel2_Result> CategoryLeft2 { get; set; }
        public List<web_getSameLevelCategory_Result> CategoryLevel3 { get; set; }
    }

    public class SearchViewModel
    {
        public int? page { get; set; }
        public string keyword { get; set; }
        public string sort { get; set; }
        public string manuCode { get; set; }
        public string cateCode { get; set; }
    }
}