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
    
    public class ClassesxxxxController : Controller
    {
        private CRM_TTVEntities db = new CRM_TTVEntities();

        // GET: Classes
        public async Task<ActionResult> Index(string order, Int32? size = 10, Int32? page = 1)
        {
            //var tbClasses = db.tbClasses.Include(t => t.tbCategory).Include(t => t.tbCompany).Include(t => t.tbEducation).Include(t => t.tbLever).Include(t => t.tbScience).Include(t => t.tbServicePack).Include(t => t.tbSpecialized);
            //return View(await tbClasses.ToListAsync());
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

            string[] orderBy = null;
            if (!string.IsNullOrEmpty(order))
            {
                // trả về kết quả xắp xếp theo yêu cầu của client
                orderBy = order.Split('-');
                var tbClasses = db.tbClasses.OrderBy(orderBy[0] + " " + orderBy[1]).Skip(skip).Take(take).Include(t => t.tbCategory).Include(t => t.tbCompany).Include(t => t.tbEducation).Include(t => t.tbLever).Include(t => t.tbScience).Include(t => t.tbServicePack).Include(t => t.tbSpecialized);
                ViewBag.Paging = Paging.Pagination(db.tbClasses.Count(), page, size);
                return View(await tbClasses.ToListAsync());
            }
            else
            {
                //trả về danh sách mặc định từ csdl và xắp xếp theo ID giảm dần
                var tbClasses = db.tbClasses.OrderByDescending(x => x.idClass).Skip(skip).Take(take).Include(t => t.tbCategory).Include(t => t.tbCompany).Include(t => t.tbEducation).Include(t => t.tbLever).Include(t => t.tbScience).Include(t => t.tbServicePack).Include(t => t.tbSpecialized);
                ViewBag.Paging = Paging.Pagination(db.tbClasses.Count(), page, size);
                return View(await tbClasses.ToListAsync());
            }
        }

        // GET: Classes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbClass tbClass = await db.tbClasses.FindAsync(id);
            if (tbClass == null)
            {
                return HttpNotFound();
            }
            return PartialView(tbClass);
        }

        // GET: Classes/Create
        public ActionResult Create()
        {
            ViewBag.idCategory = new SelectList(db.tbCategories.Where(x => x.idCategoryType == 6), "idCategory", "name");
            ViewBag.idCompany = new SelectList(db.tbCompanies, "idCompany", "name");
            ViewBag.idEducation = new SelectList(db.tbEducations, "idEducation", "name");
            ViewBag.idLever = new SelectList(db.tbLevers, "idLever", "name");
            ViewBag.idScience = new SelectList(db.tbSciences, "idScience", "name");
            ViewBag.idServicePack = new SelectList(db.tbServicePacks, "idServicePack", "name");
            ViewBag.idSpecialized = new SelectList(db.tbSpecializeds, "idSpecialized", "name");
            return PartialView();
        }

        // POST: Classes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idClass,idCompany,idEducation,idSpecialized,idServicePack,idScience,idCategory,idLever,name,saveOff,note")] tbClass tbClass, List<category> v2)
        {
            if (ModelState.IsValid)
            {
                tbClass.timeStudy = "";
                foreach (var item in v2)
                {
                    tbClass.timeStudy += item.value + ',';
                }

                db.tbClasses.Add(tbClass);
                await db.SaveChangesAsync();
                TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Thành công...!!!" };

                if (Request.IsAjaxRequest())
                    return Json("insert successfully!");

                return RedirectToAction("Index");
            }

            ViewBag.idCategory = new SelectList(db.tbCategories.Where(x => x.idCategoryType == 6), "idCategory", "name", tbClass.@object);
            ViewBag.idCompany = new SelectList(db.tbCompanies, "idCompany", "name", tbClass.idCompany);
            ViewBag.idEducation = new SelectList(db.tbEducations, "idEducation", "name", tbClass.idEducation);
            ViewBag.idLever = new SelectList(db.tbLevers, "idLever", "name", tbClass.idLever);
            ViewBag.idScience = new SelectList(db.tbSciences, "idScience", "name", tbClass.idScience);
            ViewBag.idServicePack = new SelectList(db.tbServicePacks, "idServicePack", "listIdSubject", tbClass.idServicePack);
            ViewBag.idSpecialized = new SelectList(db.tbSpecializeds, "idSpecialized", "name", tbClass.idSpecialized);

            TempData["speaker"] = new speaker { type = 2, title = "Error!", content = "Đã có lỗi xẩy ra...!!!" };
            return new HttpStatusCodeResult(500);
        }

        // GET: Classes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbClass tbClass = await db.tbClasses.FindAsync(id);
            if (tbClass == null)
            {
                TempData["speaker"] = new speaker { type = 3, title = "Warning!", content = "Không tìm thấy dữ liệu...!!!" };
                return HttpNotFound();
            }
            ViewBag.idCategory = new SelectList(db.tbCategories.Where(x => x.idCategoryType == 6), "idCategory", "name", tbClass.@object);
            ViewBag.idCompany = new SelectList(db.tbCompanies, "idCompany", "name", tbClass.idCompany);
            ViewBag.idEducation = new SelectList(db.tbEducations, "idEducation", "name", tbClass.idEducation);
            ViewBag.idLever = new SelectList(db.tbLevers, "idLever", "name", tbClass.idLever);
            ViewBag.idScience = new SelectList(db.tbSciences, "idScience", "name", tbClass.idScience);
            ViewBag.idServicePack = new SelectList(db.tbServicePacks, "idServicePack", "name", tbClass.idServicePack);
            ViewBag.idSpecialized = new SelectList(db.tbSpecializeds, "idSpecialized", "name", tbClass.idSpecialized);

            TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Cập nhật thành công...!!!" };
            return PartialView(tbClass);
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idClass,idCompany,idEducation,idSpecialized,idServicePack,idScience,idCategory,idLever,name,saveOff,note")] tbClass tbClass, List<category> v2)
        {
            if (ModelState.IsValid)
            {

                tbClass.timeStudy = "";
                foreach (var item in v2)
                {
                    tbClass.timeStudy += item.value + ',';
                }

                db.Entry(tbClass).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.idCategory = new SelectList(db.tbCategories.Where(x => x.idCategoryType == 6), "idCategory", "name", tbClass.@object);
            ViewBag.idCompany = new SelectList(db.tbCompanies, "idCompany", "name", tbClass.idCompany);
            ViewBag.idEducation = new SelectList(db.tbEducations, "idEducation", "name", tbClass.idEducation);
            ViewBag.idLever = new SelectList(db.tbLevers, "idLever", "name", tbClass.idLever);
            ViewBag.idScience = new SelectList(db.tbSciences, "idScience", "name", tbClass.idScience);
            ViewBag.idServicePack = new SelectList(db.tbServicePacks, "idServicePack", "listIdSubject", tbClass.idServicePack);
            ViewBag.idSpecialized = new SelectList(db.tbSpecializeds, "idSpecialized", "name", tbClass.idSpecialized);

            TempData["speaker"] = new speaker { type = 3, title = "Warning!", content = "Token or form data is note valid...!!!" };
            return PartialView(tbClass);
        }

        // GET: Classes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbClass tbClass = await db.tbClasses.FindAsync(id);
            if (tbClass == null)
            {
                TempData["speaker"] = new speaker { type = 2, title = "Error!", content = "Dữ liệu có liên kết hoặc không tìm thấy...!!!" };
                return HttpNotFound();
            }
            return View(tbClass);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int[] ids)
        {
            foreach (var id in ids)
            {
                tbClass tbClass = await db.tbClasses.FindAsync(id);
                db.tbClasses.Remove(tbClass);
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
