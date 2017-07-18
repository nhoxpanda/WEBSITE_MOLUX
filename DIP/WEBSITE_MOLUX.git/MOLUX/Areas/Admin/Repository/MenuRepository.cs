using MOLUX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MOLUX.Areas.Admin.Repository
{
    public class MenuRepository
    {
        BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();

        public List<web_NewsCategory> getAllMenuHomePage()
        {
            return _db.web_NewsCategory.Where(p => p.IsDelete != true && p.Type == 0 && p.IsShow == true).OrderBy(p => p.Orders).ToList();
        }

        public List<web_NewsCategory> getAllMenu()
        {
            return _db.web_NewsCategory.Where(p => p.IsDelete != true && p.Type == 0).OrderBy(p => p.Orders).ToList();
        }

        public List<web_NewsCategory> getAllMenuShow()
        {
            var menuList = (from menu in _db.web_NewsCategory
                            where menu.Type == 0 && menu.IsDelete == false && menu.IsShow == true
                            select menu).ToList();
            return menuList;
        }
        public void AddMenu(web_NewsCategory _menu)
        {
            _db.web_NewsCategory.Add(_menu);
            _db.SaveChanges();
        }
        public web_NewsCategory getMenuByID(int id)
        {
            return _db.web_NewsCategory.FirstOrDefault(x => x.Id == id);
           
        }
        public List<web_NewsCategory> getAllCatgory()
        {
            var cateList = (from cate in _db.web_NewsCategory
                            where cate.Type == 1 && cate.IsDelete == false
                            select cate).ToList();
            return cateList;
        }
        public List<web_NewsCategory> getAllCatgoryByMenuID(int id)
        {
            var cateList = (from cate in _db.web_NewsCategory
                            where cate.ParentId == id && cate.Type == 1 && cate.IsDelete == false
                            select cate).ToList();
            return cateList;
        }
        public bool ChangeShow(int id)
        {
            try
            {
                var temp = getMenuByID(id);
                temp.IsShow = !temp.IsShow;
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return false;
            }
        }
        public bool DeleteMenu(int id)
        {
            try
            {
                var temp = getMenuByID(id);
                temp.IsDelete = true;
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return false;
            }

        }
    }
}