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
    
    public class ServicePacksController : Controller
    {
        private CRM_TTVEntities db = new CRM_TTVEntities();

        // GET: ServicePacks
        public async Task<ActionResult> Index(string order, Int32? size = 10, Int32? page = 1)
        {
            //var tbServicePacks = db.tbServicePacks.Include(t => t.tbSpecialized);
            //return View(await tbServicePacks.ToListAsync());

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
            //var ghghgh = db.tbServicePacks.OrderBy("name asc");

            string[] orderBy = null;
            if (!string.IsNullOrEmpty(order))
            {
                // trả về kết quả xắp xếp theo yêu cầu của client
                orderBy = order.Split('-');
                var tbServicePacks = db.tbServicePacks.OrderBy(orderBy[0] + " " + orderBy[1]).Skip(skip).Take(take).Include(t => t.tbSpecialized).Include(t => t.tbEducation);
                ViewBag.Paging = Paging.Pagination(db.tbServicePacks.Count(), page, size);
                return View(await tbServicePacks.ToListAsync());
            }
            else
            {
                //trả về danh sách mặc định từ csdl và xắp xếp theo ID giảm dần
                var tbServicePacks = db.tbServicePacks.OrderByDescending(x => x.idServicePack).Skip(skip).Take(take).Include(t => t.tbSpecialized).Include(t => t.tbEducation);
                ViewBag.Paging = Paging.Pagination(db.tbServicePacks.Count(), page, size);
                return View(await tbServicePacks.ToListAsync());
            }
        }

        // GET: ServicePacks/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbServicePack tbServicePack = await db.tbServicePacks.FindAsync(id);
            if (tbServicePack == null)
            {
                return HttpNotFound();
            }
            return PartialView(tbServicePack);
        }

        // GET: ServicePacks/Create
        public ActionResult Create()
        {
            ViewBag.idEducation = new SelectList(db.tbEducations, "idEducation", "name");
            var tbSpecializeds = db.tbSpecializeds.Where(x => x.idEducation == db.tbEducations.FirstOrDefault().idEducation).ToList();
            ViewBag.idSpecialized = new SelectList(tbSpecializeds, "idSpecialized", "name");

            return PartialView();
        }

        // POST: ServicePacks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idEducation,idServicePack,idSpecialized,name,note")] tbServicePack tbServicePack)
        {
            if (ModelState.IsValid)
            {
                db.tbServicePacks.Add(tbServicePack);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.idEducation = new SelectList(db.tbEducations, "idEducation", "name", tbServicePack.idEducation);
            var tbSpecializeds = db.tbSpecializeds.Where(x => x.idEducation == tbServicePack.idEducation).ToList();
            ViewBag.idSpecialized = new SelectList(tbSpecializeds, "idSpecialized", "name", tbServicePack.idSpecialized);

            TempData["speaker"] = new speaker { type = 2, title = "Error!", content = "Đã có lỗi xẩy ra...!!!" };
            return RedirectToAction("Index");
        }

        // GET: ServicePacks/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbServicePack tbServicePack = await db.tbServicePacks.FindAsync(id);
            if (tbServicePack == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.idEducation = new SelectList(db.tbEducations, "idEducation", "name", tbServicePack.idEducation);
            var tbSpecializeds = db.tbSpecializeds.Where(x => x.idEducation == tbServicePack.idEducation).ToList();
            ViewBag.idSpecialized = new SelectList(tbSpecializeds, "idSpecialized", "name", tbServicePack.idSpecialized);

            TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Cập nhật thành công...!!!" };
            return PartialView(tbServicePack);
        }

        // POST: ServicePacks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idEducation,idServicePack,idSpecialized,name,note")] tbServicePack tbServicePack)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbServicePack).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.idEducation = new SelectList(db.tbEducations, "idEducation", "name", tbServicePack.idEducation);
            var tbSpecializeds = db.tbSpecializeds.Where(x => x.idEducation == tbServicePack.idEducation).ToList();
            ViewBag.idSpecialized = new SelectList(tbSpecializeds, "idSpecialized", "name", tbServicePack.idSpecialized);


            return PartialView(tbServicePack);
        }

        // GET: ServicePacks/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbServicePack tbServicePack = await db.tbServicePacks.FindAsync(id);
            if (tbServicePack == null)
            {
                return HttpNotFound();
            }
            return View(tbServicePack);
        }

        // POST: ServicePacks/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int[] ids)
        {
            foreach (var id in ids)
            {
                tbServicePack tbServicePack = await db.tbServicePacks.FindAsync(id);
                db.tbServicePacks.Remove(tbServicePack);
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
