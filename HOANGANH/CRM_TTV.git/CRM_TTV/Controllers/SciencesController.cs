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
    
    public class SciencesController : Controller
    {
        private CRM_TTVEntities db = new CRM_TTVEntities();

        #region DS Khóa học
        // GET: Sciences
        public async Task<ActionResult> Index(string order, Int32? size = 10, Int32? page = 1)
        {
            //var tbSciences = db.tbSciences.Include(t => t.tbEducation).Include(t => t.tbServicePack).Include(t => t.tbSpecialized);
            //return View(await tbSciences.ToListAsync());

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
            //var ghghgh = db.tbSciences.OrderBy("name asc");

            string[] orderBy = null;
            if (!string.IsNullOrEmpty(order))
            {
                // trả về kết quả xắp xếp theo yêu cầu của client
                orderBy = order.Split('-');
                var tbSciences = db.tbSciences.OrderBy(orderBy[0] + " " + orderBy[1]).Skip(skip).Take(take).Include(t => t.tbEducation).Include(t => t.tbServicePack).Include(t => t.tbSpecialized);
                ViewBag.Paging = Paging.Pagination(db.tbSciences.Count(), page, size);
                return View(await tbSciences.ToListAsync());
            }
            else
            {
                //trả về danh sách mặc định từ csdl và xắp xếp theo ID giảm dần
                var tbSciences = db.tbSciences.OrderByDescending(x => x.idScience).Skip(skip).Take(take).Include(t => t.tbEducation).Include(t => t.tbServicePack).Include(t => t.tbSpecialized);
                ViewBag.Paging = Paging.Pagination(db.tbSciences.Count(), page, size);
                return View(await tbSciences.ToListAsync());
            }
        }
        #endregion

        #region Lớp học theo Khóa
        [HttpGet]
        // GET: Sciences/ClassWithSeciences/4
        [Route("Sciences/ClassWithSeciences/{idScience}")]
        public async Task<ActionResult> ClassWithSeciences(int? idScience)
        {
            var tbSciences = db.tbClasses.Where(z => z.idScience == idScience).OrderByDescending(x => x.idClass);
            return PartialView(await tbSciences.ToListAsync());
        }
        #endregion

        #region Lịch sử báo giá
        [HttpGet]
        // GET: Sciences/ClassWithSeciences/4
        [Route("Sciences/QuotationHistory/{idScience}")]
        public async Task<ActionResult> QuotationHistory(int? idScience)
        {
            var tbSciences = db.tbClasses.Where(z => z.idScience == idScience).OrderByDescending(x => x.idClass);
            return PartialView(await tbSciences.ToListAsync());
        }
        #endregion

        #region Chi tiết
        // GET: Sciences/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbScience tbScience = await db.tbSciences.FindAsync(id);
            if (tbScience == null)
            {
                return HttpNotFound();
            }
            return PartialView(tbScience);
        }
        #endregion

        #region Thêm mới
        // GET: Sciences/Create
        public ActionResult Create()
        {
            ViewBag.idEducation = new SelectList(db.tbEducations, "idEducation", "name");
            var tbSpecializeds = db.tbSpecializeds.Where(x => x.idEducation == db.tbEducations.FirstOrDefault().idEducation).ToList();
            ViewBag.idSpecialized = new SelectList(tbSpecializeds, "idSpecialized", "name");
            int id = tbSpecializeds.FirstOrDefault().idSpecialized;
            var tbServicePacks = db.tbServicePacks.Where(x => x.idSpecialized == id).ToList();
            ViewBag.idServicePack = new SelectList(tbServicePacks, "idServicePack", "name");

            return PartialView();
        }

        // POST: Sciences/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idScience,idEducation,idSpecialized,idServicePack,name,startDate,endDate,studyTime,unitPrice,minimumStudents,totalCoaches,note")] tbScience tbScience)
        {
            if (ModelState.IsValid)
            {
                db.tbSciences.Add(tbScience);
                await db.SaveChangesAsync();
                TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Thành công...!!!" };
                return RedirectToAction("Index");
            }

            ViewBag.idEducation = new SelectList(db.tbEducations, "idEducation", "name", tbScience.idEducation);
            var tbSpecializeds = db.tbSpecializeds.Where(x => x.idEducation == tbScience.idEducation).ToList();
            ViewBag.idSpecialized = new SelectList(tbSpecializeds, "idSpecialized", "name", tbScience.idSpecialized);
            var tbServicePacks = db.tbServicePacks.Where(x => x.idSpecialized == tbScience.idSpecialized).ToList();
            ViewBag.idServicePack = new SelectList(tbServicePacks, "idServicePack", "name", tbScience.idServicePack);

            TempData["speaker"] = new speaker { type = 2, title = "Error!", content = "Đã có lỗi xẩy ra...!!!" };
            return RedirectToAction("Index");
        }

        #endregion

        #region Cập nhật

        // GET: Sciences/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbScience tbScience = await db.tbSciences.FindAsync(id);
            if (tbScience == null)
            {
                return HttpNotFound();
            }
            //ViewBag.idEducation = new SelectList(db.tbEducations, "idEducation", "name", tbScience.idEducation);
            //ViewBag.idServicePack = new SelectList(db.tbServicePacks, "idServicePack", "name", tbScience.idServicePack);
            //ViewBag.idSpecialized = new SelectList(db.tbSpecializeds, "idSpecialized", "name", tbScience.idSpecialized);

            ViewBag.idEducation = new SelectList(db.tbEducations, "idEducation", "name", tbScience.idEducation);
            var tbSpecializeds = db.tbSpecializeds.Where(x => x.idEducation == tbScience.idEducation).ToList();
            ViewBag.idSpecialized = new SelectList(tbSpecializeds, "idSpecialized", "name", tbScience.idSpecialized);
            var tbServicePacks = db.tbServicePacks.Where(x => x.idSpecialized == tbScience.idSpecialized).ToList();
            ViewBag.idServicePack = new SelectList(tbServicePacks, "idServicePack", "name", tbScience.idServicePack);

            TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Cập nhật thành công...!!!" };

            return PartialView(tbScience);
        }

        // POST: Sciences/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idScience,idEducation,idSpecialized,idServicePack,name,startDate,endDate,studyTime,unitPrice,minimumStudents,totalCoaches,note")] tbScience tbScience)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbScience).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.idEducation = new SelectList(db.tbEducations, "idEducation", "name", tbScience.idEducation);
            var tbSpecializeds = db.tbSpecializeds.Where(x => x.idEducation == tbScience.idEducation).ToList();
            ViewBag.idSpecialized = new SelectList(tbSpecializeds, "idSpecialized", "name", tbScience.idSpecialized);
            var tbServicePacks = db.tbServicePacks.Where(x => x.idSpecialized == tbScience.idSpecialized).ToList();
            ViewBag.idServicePack = new SelectList(tbServicePacks, "idServicePack", "name", tbScience.idServicePack);

            //ViewBag.idEducation = new SelectList(db.tbEducations, "idEducation", "name", tbScience.idEducation);
            //ViewBag.idServicePack = new SelectList(db.tbServicePacks, "idServicePack", "name", tbScience.idServicePack);
            //ViewBag.idSpecialized = new SelectList(db.tbSpecializeds, "idSpecialized", "name", tbScience.idSpecialized);
            return PartialView(tbScience);
        }

        #endregion

        #region Xóa

        // GET: Sciences/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbScience tbScience = await db.tbSciences.FindAsync(id);
            if (tbScience == null)
            {
                return HttpNotFound();
            }
            return View(tbScience);
        }

        // POST: Sciences/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int[] ids)
        {
            foreach (var id in ids)
            {
                tbScience tbScience = await db.tbSciences.FindAsync(id);
                db.tbSciences.Remove(tbScience);
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
        #endregion
    }
}
