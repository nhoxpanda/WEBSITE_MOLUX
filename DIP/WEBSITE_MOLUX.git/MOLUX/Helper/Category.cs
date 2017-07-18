using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MOLUX.Models;

namespace MOLUX.Helper
{
    public static class Category
    {
        private static BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();

        public static List<web_getParentCategory_Result> CategoryParent()
        {
            return _db.web_getParentCategory().ToList();
        }

        public static List<web_getChildCategory_Result> CategoryChild(int id)
        {
            return _db.web_getChildCategory(id).ToList();
        }

        public static List<web_getHeaderMenu_Result> HeaderMenu()
        {
            return _db.web_getHeaderMenu().ToList();
        }

        public static List<web_getLevel3Category_Result> Level3Category(int id)
        {
            return _db.web_getLevel3Category(id).ToList();
        }

        public static web_getItemCategoryById_Result GetItemCategoryById(int id)
        {
            return _db.web_getItemCategoryById(id).FirstOrDefault();
        }

        public static Item_Category GetCategoryByCode(string code)
        {
            return _db.Item_Category.FirstOrDefault(p => p.Code == code);
        }
    }
}