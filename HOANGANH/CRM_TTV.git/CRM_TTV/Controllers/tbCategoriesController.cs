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
using CRM_TTV.Search;
using CRM_TTV.Models;

namespace CRM_TTV.Controllers
{
    
    public class tbCategoriesController : Controller
    {
        private CRM_TTVEntities db = new CRM_TTVEntities();

        public async Task<ActionResult> Index(string order, Int32? size = 10, Int32? page = 1)
        {
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
            //var ghghgh = db.tbCategories.OrderBy("name asc");

            string[] orderBy = null;
            if (!string.IsNullOrEmpty(order)) 
            {
                // trả về kết quả xắp xếp theo yêu cầu của client
                orderBy = order.Split('-');
                var tbCategories = db.tbCategories.OrderBy(orderBy[0] + " " + orderBy[1]).Skip(skip).Take(take).Include(t => t.tbCategoryType);
                ViewBag.Paging = Paging.Pagination(db.tbCategories.Count(), page, size);
                return View(await tbCategories.ToListAsync());
            }
            else
            {
                //trả về danh sách mặc định từ csdl và xắp xếp theo ID giảm dần
                var tbCategories = db.tbCategories.OrderByDescending(x => x.idCategory).Skip(skip).Take(take).Include(t => t.tbCategoryType);
                ViewBag.Paging = Paging.Pagination(db.tbCategories.Count(), page, size);
                return View(await tbCategories.ToListAsync());
            }
            
        }

        [HttpPost]
        public async Task<ActionResult> Search(tbCategorySearchModel searchModel, string order, Int32? size = 10, Int32? page = 0)
        {
            tbCategorySearch search = new tbCategorySearch();
            var tbCategories = search.Filter(searchModel, order, size, page);
            
            TempData["search"] = tbCategories;
            TempData["searchModel"] = searchModel;
            return View("Index", await tbCategories.ToListAsync());
        }

        // GET: tbCategories/Details/5
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
            return PartialView("View", tbCategory);
        }

        // GET: tbCategories/Create
        public ActionResult Create()
        {
            ViewBag.idCategoryType = new SelectList(db.tbCategoryTypes, "idCategoryType", "name");
            return PartialView();
        }
        
        // POST: tbCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "idCategory,idCategoryType,name,sort")] tbCategory tbCategory, string[] v2)
        public async Task<ActionResult> Create(tbCategory tbCategory, List<category> v2)
        {
            if (ModelState.IsValid)
            {
                db.tbCategories.Add(tbCategory);
                await db.SaveChangesAsync();
                TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Thành công...!!!" };

                if (Request.IsAjaxRequest())
                    return Json("insert successfully!");

                return RedirectToAction("Index");
            }
            ViewBag.idCategoryType = new SelectList(db.tbCategoryTypes, "idCategoryType", "name", tbCategory.idCategoryType);
            TempData["speaker"] = new speaker { type = 2, title = "Error!", content = "Đã có lỗi xẩy ra...!!!" };

            return new HttpStatusCodeResult(500);
        }

        // GET: tbCategories/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
            ViewBag.idCategoryType = new SelectList(db.tbCategoryTypes, "idCategoryType", "name", tbCategory.idCategoryType);
            TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Cập nhật thành công...!!!" };

            return PartialView(tbCategory);
        }

        // POST: tbCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idCategory,idCategoryType,name,sort")] tbCategory tbCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbCategory).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.idCategoryType = new SelectList(db.tbCategoryTypes, "idCategoryType", "name", tbCategory.idCategoryType);
            return View(tbCategory);
        }

        // GET: tbCategories/Delete/5
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

        // POST: tbCategories/Delete/5
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
            //return RedirectToAction("Index");
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
