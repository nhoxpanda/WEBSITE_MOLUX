using MOLUX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MOLUX.Areas.Admin.Models
{
    public class MenuViewModel
    {
        public MenuViewModel()
        {
            listMenu = new List<web_NewsCategory>();
            listMenuHomePage = new List<web_NewsCategory>();
            listCate = new List<web_NewsCategory>();
            menu = new web_NewsCategory();
            listMenuItem = new List<SelectListItem>();
           
            
        }
        public web_NewsCategory menu { get; set; }
        public IList<web_NewsCategory> listMenu { get; set; }
        public IList<web_NewsCategory> listMenuHomePage { get; set; }
        public IList<web_NewsCategory> listCate { get; set; }
        public IList<SelectListItem> listMenuItem { get; set; }
        
    }
}