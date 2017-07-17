using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CRM_TTV;
using CRM_TTV.Models;
using CRM_TTV.Search;

namespace CRM_TTV.Controllers
{
    
    public class CategoryRankCourseController : Controller
    {
        private CRM_TTVEntities db = new CRM_TTVEntities();
        int idCategoryType = 12;
        // GET: CategoryPhone
        public async Task<ActionResult> Index(string order, Int32? size = 10, Int32? page = 1)
        {
            //var tbCategories = db.tbCategories.Include(t => t.tbCategoryType);
            //return View(await tbCategories.ToListAsync());

            if ((Int32)size < 0)
                size = 10;
            if ((Int32)page < 0)
                page = 1;

            if (TempData["search"] != null) //trả về kết quả tìm kiếm
            {
                if (TempData["searchModel"] != null) //diss mẹ phải chơi kiểu này nó mới lưu dc vcl chưa biết fix
                    TempData["sModel"] = TempData["searchModel"];

                IEnumerable<object> search = TempData.ContainsKey("search") ? TempData["search"] as IEnumerable<tbCategory> : null;
                ViewBag.Paging = Paging.Pagination(search.Count(), page, size);
                TempData["speaker"] = new speaker { type = 2, title = "Thành công...!!!", content = "Đã tìm thấy " + search.Count() + " dòng dữ liệu...!!!" };
                return View(search);
            }

            int take = (Int32)size;
            int skip = 0;
            if (page > 0)
                skip = ((Int32)page - 1) * take;

            string[] orderBy = null;
            if (!string.IsNullOrEmpty(order))
            {
                // trả về kết quả xắp xếp theo yêu cầu của client
                orderBy = order.Split('-');
                var tbCategories = db.tbCategories.Where(x=>x.idCategoryType == idCategoryType).OrderBy(orderBy[0] + " " + orderBy[1]).Skip(skip).Take(take);
                ViewBag.Paging = Paging.Pagination(tbCategories.Count(), page, size);
                return View(await tbCategories.ToListAsync());
            }
            else
            {
                //trả về danh sách mặc định từ csdl và xắp xếp theo ID giảm dần
                var tbCategories = db.tbCategories.Where(x => x.idCategoryType == idCategoryType).OrderByDescending(x => x.idCategory).Skip(skip).Take(take);
                ViewBag.Paging = Paging.Pagination(tbCategories.Count(), page, size);
                return View(await tbCategories.ToListAsync());
            }
        }

        // GET: CategoryPhone/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbCategory tbCategory = await db.tbCategories.FindAsync(id);
            if (tbCategory == null)
            {
                return HttpNotFound();
            }
            return PartialView(tbCategory);
        }

        // GET: CategoryPhone/Create
        public ActionResult Create()
        {
            return PartialView();
        }

        // POST: CategoryPhone/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idCategory,name,note,sort")] tbCategory tbCategory)
        {
            if (ModelState.IsValid)
            {
                tbCategory.idCategoryType = idCategoryType;
                db.tbCategories.Add(tbCategory);
                await db.SaveChangesAsync();
                TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Thành công...!!!" };

                if (Request.IsAjaxRequest())
                    return Json("insert successfully!");

                return RedirectToAction("Index");
            }

            TempData["speaker"] = new speaker { type = 2, title = "Error!", content = "Đã có lỗi xẩy ra...!!!" };
            return new HttpStatusCodeResult(500);
        }

        // GET: CategoryPhone/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbCategory tbCategory = await db.tbCategories.FindAsync(id);
            if (tbCategory == null)
            {
                TempData["speaker"] = new speaker { type = 3, title = "Warning!", content = "Không tìm thấy dữ liệu...!!!" };
                return HttpNotFound();
            }
            return PartialView(tbCategory);
        }

        // POST: CategoryPhone/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idCategory,name,note,sort")] tbCategory tbCategory)
        {
            if (ModelState.IsValid)
            {
                tbCategory.idCategoryType = idCategoryType;
                db.Entry(tbCategory).State = EntityState.Modified;
                await db.SaveChangesAsync();
                TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Cập nhật thành công...!!!" };
                if (Request.IsAjaxRequest())
                    return Json("update successfully!");

                return RedirectToAction("Index");
            }
            TempData["speaker"] = new speaker { type = 3, title = "Warning!", content = "Token or form data is note valid...!!!" };
            return PartialView(tbCategory);
        }

        // GET: CategoryPhone/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbCategory tbCategory = await db.tbCategories.FindAsync(id);
            if (tbCategory == null)
            {
                return HttpNotFound();
            }
            return View(tbCategory);
        }

        // POST: CategoryPhone/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int[] ids)
        {
            foreach (var id in ids)
            {
                tbCategory tbCategory = await db.tbCategories.FindAsync(id);
                db.tbCategories.Remove(tbCategory);
            }
            await db.SaveChangesAsync();
            TempData["speaker"] = new speaker { type = 3, title = "Success!", content = "Xóa thành công " + ids.Count() + " dòng dữ liệu...!!!" };
            return Json("Deleted successfully!");
        }

        [HttpPost]
        public async Task<ActionResult> Search(CategorySearchModel searchModel, string order, Int32? size = 10, Int32? page = 0)
        {
            searchModel.idCategoryType = idCategoryType;
            CategorySearch search = new CategorySearch();
            var tbCategories = search.Filter(searchModel, order, size, page);

            TempData["search"] = tbCategories;
            TempData["searchModel"] = searchModel;
            return View("Index", await tbCategories.ToListAsync());
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
