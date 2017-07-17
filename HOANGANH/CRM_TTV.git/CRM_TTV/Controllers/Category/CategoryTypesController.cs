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
    
    public class CategoryTypesController : Controller
    {
        private CRM_TTVEntities db = new CRM_TTVEntities();

        // GET: CategoryTypes
        public async Task<ActionResult> Index(string order, Int32? size = 10, Int32? page = 1)
        {
            //return View(await db.tbCategoryTypes.ToListAsync());
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
                var tbCategoryTypes = db.tbCategoryTypes.OrderBy(orderBy[0] + " " + orderBy[1]).Skip(skip).Take(take);
                ViewBag.Paging = Paging.Pagination(db.tbCategoryTypes.Count(), page, size);
                return View(await tbCategoryTypes.ToListAsync());
            }
            else
            {
                //trả về danh sách mặc định từ csdl và xắp xếp theo ID giảm dần
                var tbCategoryTypes = db.tbCategoryTypes.OrderByDescending(x => x.idCategoryType).Skip(skip).Take(take);
                ViewBag.Paging = Paging.Pagination(db.tbCategoryTypes.Count(), page, size);
                return View(await tbCategoryTypes.ToListAsync());
            }
        }

        // GET: CategoryTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbCategoryType tbCategoryType = await db.tbCategoryTypes.FindAsync(id);
            if (tbCategoryType == null)
            {
                return HttpNotFound();
            }
            return PartialView(tbCategoryType);
        }

        // GET: CategoryTypes/Create
        public ActionResult Create()
        {
            return PartialView();
        }

        // POST: CategoryTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idCategoryType,name,sort,note")] tbCategoryType tbCategoryType)
        {
            if (ModelState.IsValid)
            {
                db.tbCategoryTypes.Add(tbCategoryType);
                await db.SaveChangesAsync();
                TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Thành công...!!!" };
                return RedirectToAction("Index");
            }

            TempData["speaker"] = new speaker { type = 2, title = "Error!", content = "Đã có lỗi xẩy ra...!!!" };
            return RedirectToAction("Index");
        }

        // GET: CategoryTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbCategoryType tbCategoryType = await db.tbCategoryTypes.FindAsync(id);
            if (tbCategoryType == null)
            {
                TempData["speaker"] = new speaker { type = 3, title = "Warning!", content = "Không tìm thấy dữ liệu...!!!" };
                return HttpNotFound();
            }
            TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Cập nhật thành công...!!!" };

            return PartialView(tbCategoryType);
        }

        // POST: CategoryTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idCategoryType,name,sort,note")] tbCategoryType tbCategoryType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbCategoryType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            TempData["speaker"] = new speaker { type = 3, title = "Warning!", content = "Token form is note valid...!!!" };
            return PartialView(tbCategoryType);
        }

        // GET: CategoryTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbCategoryType tbCategoryType = await db.tbCategoryTypes.FindAsync(id);
            if (tbCategoryType == null)
            {
                TempData["speaker"] = new speaker { type = 2, title = "Error!", content = "Dữ liệu có liên kết hoặc không tìm thấy...!!!" };
                return HttpNotFound();
            }
            return View(tbCategoryType);
        }

        // POST: CategoryTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int[] ids)
        {
            foreach (var id in ids)
            {
                tbCategoryType tbCategoryType = await db.tbCategoryTypes.FindAsync(id);
                db.tbCategoryTypes.Remove(tbCategoryType);
            }
            await db.SaveChangesAsync();
            TempData["speaker"] = new speaker { type = 3, title = "Success!", content = "Xóa thành công " + ids.Count() + " dòng dữ liệu...!!!" };
            return Json("Deleted successfully!");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        public async Task<ActionResult> Search(CategorySearchModel searchModel, string order, Int32? size = 10, Int32? page = 0)
        {
            CategorySearch search = new CategorySearch();
            var tbCategories = search.Filter(searchModel, order, size, page);

            TempData["search"] = tbCategories;
            TempData["searchModel"] = searchModel;
            return View("Index", await tbCategories.ToListAsync());
        }
    }
}
