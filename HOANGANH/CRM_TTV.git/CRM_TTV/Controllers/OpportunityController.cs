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
    
    public class OpportunityController : Controller
    {
        private CRM_TTVEntities db = new CRM_TTVEntities();

        // GET: Opportunity
        public async Task<ActionResult> Index(string order, Int32? size = 10, Int32? page = 1)
        {
            //var tbUsers = db.tbUsers.Include(t => t.tbCategory).Include(t => t.tbCategory1).Include(t => t.tbCategory2).Include(t => t.tbCompany).Include(t => t.tbRole).Include(t => t.tbUser2);
            //return View(await tbUsers.ToListAsync());

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
                var tbUsers = db.tbUsers.OrderBy(orderBy[0] + " " + orderBy[1]).Skip(skip).Take(take).Include(t => t.tbCategory).Include(t => t.tbCompany).Include(t => t.tbRole).Include(t => t.tbUser2);
                ViewBag.Paging = Paging.Pagination(db.tbUsers.Count(), page, size);
                return View(await tbUsers.ToListAsync());
            }
            else
            {
                //trả về danh sách mặc định từ csdl và xắp xếp theo ID giảm dần
                var tbUsers = db.tbUsers.OrderByDescending(x => x.userID).Skip(skip).Take(take).Include(t => t.tbCategory).Include(t => t.tbCompany).Include(t => t.tbRole).Include(t => t.tbUser2);
                ViewBag.Paging = Paging.Pagination(db.tbUsers.Count(), page, size);
                return View(await tbUsers.ToListAsync());
            }
        }

        // GET: Opportunity/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbUser tbUser = await db.tbUsers.FindAsync(id);
            if (tbUser == null)
            {
                return HttpNotFound();
            }
            return PartialView(tbUser);
        }

        // GET: Opportunity/Create
        public ActionResult Create()
        {
            ViewBag.idState = new SelectList(db.tbCategories.Where(x => x.idCategoryType == 14), "idCategory", "name");
            ViewBag.idJob = new SelectList(db.tbCategories.Where(x => x.idCategoryType == 4), "idCategory", "name");
            ViewBag.healthStatus = new SelectList(db.tbCategories.Where(x=>x.idCategoryType == 13), "idCategory", "name");
            return PartialView();
        }

        // POST: Opportunity/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "userID,fullName,birthDay,idState,idJob,healthStatus,phone,address,passport,aspiration,sex,yogaLearned,email,skype,facebook,note")] tbUser tbUser)
        {
            if (ModelState.IsValid)
            {
                db.tbUsers.Add(tbUser);
                await db.SaveChangesAsync();
                TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Thành công...!!!" };

                if (Request.IsAjaxRequest())
                    return Json("insert successfully!");

                return RedirectToAction("Index");
            }

            ViewBag.idState = new SelectList(db.tbCategories, "idCategory", "name", tbUser.idState);
            ViewBag.idJob = new SelectList(db.tbCategories, "idCategory", "name", tbUser.idJob);
            ViewBag.healthStatus = new SelectList(db.tbCategories, "idCategory", "name", tbUser.healthStatus);
            TempData["speaker"] = new speaker { type = 2, title = "Error!", content = "Đã có lỗi xẩy ra...!!!" };
            return new HttpStatusCodeResult(500);
        }

        // GET: Opportunity/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbUser tbUser = await db.tbUsers.FindAsync(id);
            if (tbUser == null)
            {
                TempData["speaker"] = new speaker { type = 3, title = "Warning!", content = "Không tìm thấy dữ liệu...!!!" };
                return HttpNotFound();
            }
            ViewBag.idState = new SelectList(db.tbCategories.Where(x=>x.idCategoryType == 14), "idCategory", "name", tbUser.idState);
            ViewBag.idJob = new SelectList(db.tbCategories.Where(x => x.idCategoryType == 4), "idCategory", "name", tbUser.idJob);
            ViewBag.healthStatus = new SelectList(db.tbCategories.Where(x => x.idCategoryType == 13), "idCategory", "name", tbUser.healthStatus);

           
            return PartialView(tbUser);
        }

        // POST: Opportunity/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "userID,fullName,birthDay,idState,idJob,healthStatus,phone,address,passport,aspiration,sex,yogaLearned,email,skype,facebook,note")] tbUser tbUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbUser).State = EntityState.Modified;
                await db.SaveChangesAsync();

                TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Cập nhật thành công...!!!" };
                if (Request.IsAjaxRequest())
                    return Json("update successfully!");

                return RedirectToAction("Index");
            }
            ViewBag.idState = new SelectList(db.tbCategories.Where(x=>x.idCategoryType == 14), "idCategory", "name", tbUser.idState);
            ViewBag.idJob = new SelectList(db.tbCategories.Where(x => x.idCategoryType == 4), "idCategory", "name", tbUser.idJob);
            ViewBag.healthStatus = new SelectList(db.tbCategories.Where(x => x.idCategoryType == 13), "idCategory", "name", tbUser.healthStatus);

            TempData["speaker"] = new speaker { type = 3, title = "Warning!", content = "Token or form data is note valid...!!!" };
            return PartialView(tbUser);
        }

        // GET: Opportunity/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbUser tbUser = await db.tbUsers.FindAsync(id);
            if (tbUser == null)
            {
                return HttpNotFound();
            }
            return View(tbUser);
        }

        // POST: Opportunity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int[] ids)
        {
            foreach (var id in ids)
            {
                tbUser tbUser = await db.tbUsers.FindAsync(id);
                db.tbUsers.Remove(tbUser);
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
