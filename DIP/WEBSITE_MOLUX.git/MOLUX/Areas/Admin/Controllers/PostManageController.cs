using MOLUX.Areas.Admin.Models;
using MOLUX.Areas.Admin.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MOLUX.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PostManageController : Controller
    {
        MenuRepository menuRepo = new MenuRepository();
        PostRepository postRepo = new PostRepository();

        // GET: Admin/PostManage
        public ActionResult Index()
        {
            PostViewModel postModel = new PostViewModel();
            postModel.listPost = postRepo.getAllPost();
            postModel.listCategory = menuRepo.getAllCatgory();
            postModel.listCateItem = menuRepo.getAllCatgory().Select(
                  x => new SelectListItem { Value = x.Id.ToString(), Text = x.Title }).ToList();
            postModel.listMenuItem = menuRepo.getAllMenuShow().Select(
                  x => new SelectListItem { Value = x.Id.ToString(), Text = x.Title }).ToList();
            return View(postModel);
        }
        public ActionResult AddPost()
        {
            PostViewModel postModel = new PostViewModel();
            postModel.listCateItem = menuRepo.getAllCatgory().Select(
                 x => new SelectListItem { Value = x.Id.ToString(), Text = x.Title }).ToList();
            postModel.listMenuItem = menuRepo.getAllMenuShow().Select(
                  x => new SelectListItem { Value = x.Id.ToString(), Text = x.Title }).ToList();
            return View(postModel);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AddPost(PostViewModel postModel, HttpPostedFileBase Image)
        {
            // image
            if (Image != null)
            {
                String newName = Image.FileName.Insert(Image.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                String path = Server.MapPath("~/Images/News/" + newName);
                Image.SaveAs(path);
                postModel.post.Image = newName;
            }
            //
            postModel.post.IsDelete = false;
            postModel.post.IsShow = true;
            postModel.post.Views = 1;
            postModel.post.ParentId = 0;
            postModel.post.Orders = 1;
            postModel.post.CreatedDate = DateTime.Now;
            if (postModel.post.NewsCategoryId == null || postModel.post.NewsCategoryId == 0)
            {
                postModel.post.NewsCategoryId = Convert.ToInt32(postModel.Temp);
            }
            postRepo.AddPost(postModel.post);
            return RedirectToAction("Index","PostManage");
        }

        public JsonResult LoadCate(int ID)
        {
            var listCateItem= menuRepo.getAllCatgoryByMenuID(ID).Select(
                 x => new SelectListItem { Value = x.Id.ToString(), Text = x.Title }).ToList();
            return Json(new { listCateItem = listCateItem }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadPostByMenuID(int IDMenu, int IDCate=0)
        {
            var listPost = postRepo.LoadPostByMenuID(IDMenu, IDCate);
            return Json(new { listPost = listPost }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeletePost(int id)
        {
            postRepo.DeletePost(id);
            return Json(JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public ActionResult EditPost(int id, PostViewModel postModel, HttpPostedFileBase Image)
        {
            if (Request["btnLuu"] != null)
            {
                // image
                if (Image != null)
                {
                    String newName = Image.FileName.Insert(Image.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Images/News/" + newName);
                    Image.SaveAs(path);
                    postModel.post.Image = newName;
                }
                //
                if (postModel.post.NewsCategoryId == null || postModel.post.NewsCategoryId == 0)
                {
                    postModel.post.NewsCategoryId = Convert.ToInt32(postModel.Temp);
                }
                postRepo.UpdatePost(postModel.post);
                return RedirectToAction("Index", "PostManage");
            }
            var item = new PostViewModel
            {
                post = postRepo.getPostByID(id)
            };
            return View(item);
        }
    }
}