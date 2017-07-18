using MOLUX.Areas.Admin.Models;
using MOLUX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MOLUX.Areas.Admin.Repository
{
    public class PostRepository
    {
        BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();
        public List<web_News> getAllPost()
        {
            return _db.web_News.Where(p => p.IsDelete == false).OrderByDescending(p => p.CreatedDate).ToList();
        }

        public void AddPost(web_News _post)
        {
            _db.web_News.Add(_post);
            _db.SaveChanges();
        }

        public void UpdatePost(web_News _post)
        {
            _db.Entry(_post).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
        }

        public List<PostCateViewModel> LoadPostByMenuID(int idMenu, int idCate=0)
        {
            
            var listPostTemp = _db.sp_LoadPostByMenuID(idMenu, idCate).Select(i => new PostCateViewModel
            {
                post=new web_News()
                {
                    Content = i.Content,
                    CreatedDate = i.CreatedDate,
                    Description = i.Description,
                    Id = i.Id,
                    Image = i.Image,
                    IsDelete = i.IsDelete,
                    IsShow = i.IsShow??true,
                    MetaDesc = i.MetaDesc,
                    MetaTitle = i.MetaTitle,
                    NewsCategoryId = i.NewsCategoryId,
                    Orders = i.Orders,
                    ParentId = i.ParentId,
                    Title = i.Title,
                    UrlCustom = i.UrlCustom
                },
                ChuyenMuc =i.ChuyenMuc
            }).ToList();

            return listPostTemp;
        }

        public web_News getPostByID(int id)
        {
            return _db.web_News.Find(id);
        }

        public bool DeletePost(int id)
        {
            try
            {
                var temp = getPostByID(id);
                _db.web_News.Remove(temp);
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