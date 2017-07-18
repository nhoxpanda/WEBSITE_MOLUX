using MOLUX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MOLUX.Areas.Admin.Models
{
    public class PostViewModel
    {
        public PostViewModel()
        {
            post = new web_News();
            listPost = new List<web_News>();
            listCategory = new List<web_NewsCategory>();
            listCateItem = new List<SelectListItem>();
            listMenuItem = new List<SelectListItem>();
        }
        public string Temp { get; set; }
        public web_News post { get; set; }
        public IList<web_News> listPost { get; set; }
        public IList<web_NewsCategory> listCategory { get; set; }
        public List<SelectListItem> listCateItem { get; set; }
        public List<SelectListItem> listMenuItem { get; set; }
        
    }
    public class PostCateViewModel
    {
        public PostCateViewModel()
        {
            post = new web_News();
        }
        public web_News post { get; set; }
        public string ChuyenMuc { get; set; }
    }

}