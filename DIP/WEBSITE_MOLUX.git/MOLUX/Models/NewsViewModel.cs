using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MOLUX.Models
{
    public class NewsViewModel
    {
        public List<web_getProductSameCategory_Result> SameProduct { get; set; }
        public List<web_getProductDifferenceCategory_Result> DifferenceProduct { get; set; }
        public List<web_NewsCategory> ListCategory { get; set; }
        public List<web_News> ListViews { get; set; }
    }

    public class AdvertisementViewModel
    {
        public web_Slider left { get; set; }
        public web_Slider right { get; set; }
        public web_Slider center { get; set; }
    }
}