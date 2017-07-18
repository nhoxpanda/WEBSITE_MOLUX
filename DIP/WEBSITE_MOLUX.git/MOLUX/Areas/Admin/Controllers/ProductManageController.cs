using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MOLUX.Helper;
using MOLUX.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace MOLUX.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductManageController : Controller
    {
        // GET: Admin/ProductManage

        private BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();

        #region Danh sách sản phẩn

        public ActionResult Index(string keyword, string cateCode, string manuCode, int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var model = _db.web_getProductAdminFilter(keyword, cateCode, manuCode).ToList();
            ViewBag.SearchViewModel = new SearchViewModel()
            {
                page = page,
                manuCode = manuCode,
                cateCode = cateCode,
                keyword = keyword
            };
            ViewBag.Manufacturer =  _db.Manufacturer.ToList();
            return View(model.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult UploadImage(HttpPostedFileBase Image, int Id)
        {
            if (Image != null && Image.ContentLength > 0)
            {
                String newName = Image.FileName.Insert(Image.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                String path = Server.MapPath("~/Images/Products/" + newName);
                Image.SaveAs(path);
                //
                var model = _db.Item.Find(Id);
                model.Picture = newName;
                _db.SaveChanges();
                string data = "<img src='/Images/Products/" + model.Picture + "' class='img-responsive img-thumbnail' style='width: 70px' />";
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetSizeColor(int id)
        {
            var model = new ProductViewModel()
            {
                ProductId = id,
                Color = _db.web_ItemSizeColor.Where(p => p.ItemId == id && p.IsDelete == false && p.Type == 1).ToList(),
                Size = _db.web_ItemSizeColor.Where(p => p.ItemId == id && p.IsDelete == false && p.Type == 2).ToList(),
            };
            ViewBag.AllColor = _db.web_SizeColor.Where(p => p.Type == 1 && p.IsDelete == false).ToList();
            ViewBag.AllSize = _db.web_SizeColor.Where(p => p.Type == 2 && p.IsDelete == false).ToList();
            return PartialView("_Partial_GetSizeColor", model);
        }

        [HttpPost]
        public ActionResult UpdateSizeColor(int pId, int csId, int type, int check)
        {
            var item = _db.web_ItemSizeColor.FirstOrDefault(p => p.ItemId == pId && p.SizeColorId == csId);
            if (item != null)
            {
                if (check == 0)
                {
                    item.IsDelete = true;
                }
                else
                {
                    item.IsDelete = false;
                }
            }
            else
            {
                // chưa có -> thêm mới
                var model = new web_ItemSizeColor()
                {
                    IsDelete = false,
                    ItemId = pId,
                    SizeColorId = csId,
                    Type = type
                };
                _db.web_ItemSizeColor.Add(model);
            }
            _db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Danh mục sản phẩm

        public ActionResult Cate()
        {
            return View(_db.Item_Category.ToList());
        }

        public ActionResult CreateCate(Item_Category model)
        {
            _db.Item_Category.Add(model);
            _db.SaveChanges();
            return RedirectToAction("Cate");
        }

        public ActionResult EditCate(int id)
        {
            return PartialView("_Partial_Update", _db.Item_Category.Find(id));
        }

        public ActionResult UpdateCate(Item_Category model)
        {
            _db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Cate");
        }

        public ActionResult DeleteCate(int id)
        {
            try
            {
                var model = _db.Item_Category.FirstOrDefault(p => p.RowID == id);
                _db.Item_Category.Remove(model);
                _db.SaveChanges();
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Danh sách hình ảnh

        public ActionResult UpdateImage(int id)
        {
            ViewBag.ID = id;
            ViewBag.Name = _db.Item.Find(id).Name;
            return View();
        }

        public ActionResult SaveUploadedFile(FormCollection fc)
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    if (file != null && file.ContentLength > 0)
                    {
                        String newName = file.FileName.Insert(file.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                        String path = Server.MapPath("~/Images/Products/" + newName);
                        file.SaveAs(path);

                        // insert hình sản phẩm
                        var image = new web_ItemImage
                        {
                            Image = newName,
                            ItemId = Convert.ToInt32(fc["ID"])
                        };
                        _db.web_ItemImage.Add(image);
                        _db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }
            if (isSavedSuccessfully)
            {
                return Json(new { Message = fName });
            }
            else
            {
                return Json(new { Message = "Lỗi!" });
            }
        }

        [HttpPost]
        public ActionResult GetAttachments(int id)
        {
            //Get the images list from repository
            var attachmentsList = new List<web_ItemImage>();
            var images = _db.web_ItemImage.Where(p => p.ItemId == id).ToList();
            foreach (var i in images)
            {
                attachmentsList.Add(new web_ItemImage { Id = i.Id, Image = "/Images/Products/" + i.Image, ItemId = i.ItemId });
            }

            return Json(new { Data = attachmentsList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemoveImage(int id)
        {
            var model = _db.web_ItemImage.Find(id);
            // xóa file khỏi hệ thống
            string fullPath = Request.MapPath("~/Images/Products/" + model.Image);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
            // xóa file khỏi database
            _db.web_ItemImage.Remove(model);
            _db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Cập nhật SEO

        public ActionResult EditSEO(int id)
        {
            var model = _db.Item.Find(id);
            return PartialView("_Partial_UpdateSEO", model);
        }

        [HttpPost]
        public ActionResult UpdateSEO(Item model)
        {
            var item = _db.Item.Find(model.RowID);
            item.MetaDescription = model.MetaDescription;
            item.MetaTitle = model.MetaTitle;
            _db.SaveChanges();

            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Check hiển thị + xếp thứ tự
        public ActionResult CheckShow(int id)
        {
            var item = _db.Item.Find(id);
            if (item.IsSale == true)
            {
                item.IsSale = false;
            }
            else
            {
                item.IsSale = true;
            }
            _db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpadateOrder(int id, int orders)
        {
            var item = _db.Item.Find(id);
            item.Orders = orders;
            _db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}