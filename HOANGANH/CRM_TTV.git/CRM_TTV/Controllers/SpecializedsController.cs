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

namespace CRM_TTV.Controllers
{
    
    public class SpecializedsController : Controller
    {
        private CRM_TTVEntities db = new CRM_TTVEntities();

        // GET: Specializeds
        public async Task<ActionResult> Index(string order, Int32? size = 10, Int32? page = 1)
        {
            //var tbSpecializeds = db.tbSpecializeds.Include(t => t.tbEducation);
            //return View(await tbSpecializeds.ToListAsync());

            if ((Int32)size < 0)
                size = 10;
            if ((Int32)page < 0)
                page = 1;

            if (TempData["search"] != null) //trả về kết quả tìm kiếm
            {
                if (TempData["searchModel"] != null) //diss mẹ phải chơi kiểu này nó mới lưu dc vcl chưa biết fix
                    TempData["sModel"] = TempData["searchModel"];

                IEnumerable<object> search = TempData.ContainsKey("search") ? TempData["search"] as IEnumerable<tbCompany> : null;
                ViewBag.Paging = Paging.Pagination(search.Count(), page, size);
                TempData["speaker"] = new speaker { type = 2, title = "Thành công...!!!", content = "Đã tìm thấy " + search.Count() + " dòng dữ liệu...!!!" };
                return View(search);
            }

            int take = (Int32)size;
            int skip = 0;
            if (page > 0)
                skip = ((Int32)page - 1) * take;
            //var ghghgh = db.tbSpecializeds.OrderBy("name asc");

            string[] orderBy = null;
            if (!string.IsNullOrEmpty(order))
            {
                // trả về kết quả xắp xếp theo yêu cầu của client
                orderBy = order.Split('-');
                var tbSpecializeds = db.tbSpecializeds.OrderBy(orderBy[0] + " " + orderBy[1]).Skip(skip).Take(take).Include(t => t.tbEducation);
                ViewBag.Paging = Paging.Pagination(db.tbSpecializeds.Count(), page, size);
                return View(await tbSpecializeds.ToListAsync());
            }
            else
            {
                //trả về danh sách mặc định từ csdl và xắp xếp theo ID giảm dần
                var tbSpecializeds = db.tbSpecializeds.OrderByDescending(x => x.idSpecialized).Skip(skip).Take(take).Include(t => t.tbEducation);
                ViewBag.Paging = Paging.Pagination(db.tbSpecializeds.Count(), page, size);
                return View(await tbSpecializeds.ToListAsync());
            }
        }

        // GET: Specializeds/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbSpecialized tbSpecialized = await db.tbSpecializeds.FindAsync(id);
            if (tbSpecialized == null)
            {
                return HttpNotFound();
            }
            return PartialView(tbSpecialized);
        }

        // GET: Specializeds/Create
        public ActionResult Create()
        {
            ViewBag.idEducation = new SelectList(db.tbEducations, "idEducation", "name");
            return PartialView();
        }

        // POST: Specializeds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idSpecialized,idEducation,name,startDate,endDate,note,sort")] tbSpecialized tbSpecialized)
        {
            if (ModelState.IsValid)
            {
                db.tbSpecializeds.Add(tbSpecialized);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.idEducation = new SelectList(db.tbEducations, "idEducation", "name", tbSpecialized.idEducation);
            TempData["speaker"] = new speaker { type = 2, title = "Error!", content = "Đã có lỗi xẩy ra...!!!" };
            return RedirectToAction("Index");
            //return PartialView(tbSpecialized);
        }

        // GET: Specializeds/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbSpecialized tbSpecialized = await db.tbSpecializeds.FindAsync(id);
            if (tbSpecialized == null)
            {
                TempData["speaker"] = new speaker { type = 2, title = "Error!", content = "Cập nhật không thành công...!!!" };
                return HttpNotFound();
            }
            ViewBag.idEducation = new SelectList(db.tbEducations, "idEducation", "name", tbSpecialized.idEducation);
            TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Cập nhật thành công...!!!" };

            return PartialView(tbSpecialized);
        }

        // POST: Specializeds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idSpecialized,idEducation,name,startDate,endDate,note,sort")] tbSpecialized tbSpecialized)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbSpecialized).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.idEducation = new SelectList(db.tbEducations, "idEducation", "name", tbSpecialized.idEducation);
            return PartialView(tbSpecialized);
        }

        // GET: Specializeds/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbSpecialized tbSpecialized = await db.tbSpecializeds.FindAsync(id);
            if (tbSpecialized == null)
            {
                return HttpNotFound();
            }
            return View(tbSpecialized);
        }

        // POST: Specializeds/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int[] ids)
        {
            foreach (var id in ids)
            {
                tbSpecialized tbSpecialized = await db.tbSpecializeds.FindAsync(id);
                db.tbSpecializeds.Remove(tbSpecialized);
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
    }
}
