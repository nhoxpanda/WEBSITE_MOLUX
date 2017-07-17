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
    
    public class MenuAndFormsController : Controller
    {
        private CRM_TTVEntities db = new CRM_TTVEntities();

        // GET: MenuAndForms
        public async Task<ActionResult> Index(string order, Int32? size = 10, Int32? page = 1)
        {
            //var tbMenuAndForms = db.tbMenuAndForms.Include(t => t.tbMenuAndForm2);
            //return View(await tbMenuAndForms.ToListAsync());


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
                var tbMenuAndForms = db.tbMenuAndForms.Where(x => !x.del).OrderBy(orderBy[0] + " " + orderBy[1]).Skip(skip).Take(take).Include(t => t.tbMenuAndForm2);
                ViewBag.Paging = Paging.Pagination(tbMenuAndForms.Count(), page, size);
                return View(await tbMenuAndForms.ToListAsync());
            }
            else
            {
                //trả về danh sách mặc định từ csdl và xắp xếp theo ID giảm dần
                var tbMenuAndForms = db.tbMenuAndForms.Where(x => !x.del).OrderByDescending(x => x.menuID).Skip(skip).Take(take).Include(t => t.tbMenuAndForm2);
                ViewBag.Paging = Paging.Pagination(tbMenuAndForms.Count(), page, size);
                return View(await tbMenuAndForms.ToListAsync());
            }
        }
        public ActionResult FreeViews()
        {
            return View();
        }

        // GET: MenuAndForms/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbMenuAndForm tbMenuAndForm = await db.tbMenuAndForms.FindAsync(id);
            if (tbMenuAndForm == null)
            {
                return HttpNotFound();
            }
            return PartialView(tbMenuAndForm);
        }

        // GET: MenuAndForms/Create
        public ActionResult Create()
        {
            ViewBag.parentID = new SelectList(db.tbMenuAndForms, "menuID", "name");
            return PartialView();
        }

        // POST: MenuAndForms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "menuID,parentID,name,icon,redirect,sort,isForm,jsonData,del")] tbMenuAndForm tbMenuAndForm)
        {
            if (ModelState.IsValid)
            {
                db.tbMenuAndForms.Add(tbMenuAndForm);
                await db.SaveChangesAsync();
                TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Thành công...!!!" };

                if (Request.IsAjaxRequest())
                    return Json("insert successfully!");

                return RedirectToAction("Index");
            }

            ViewBag.parentID = new SelectList(db.tbMenuAndForms, "menuID", "name", tbMenuAndForm.parentID);
            TempData["speaker"] = new speaker { type = 2, title = "Error!", content = "Đã có lỗi xẩy ra...!!!" };
            return new HttpStatusCodeResult(500);
        }

        // GET: MenuAndForms/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbMenuAndForm tbMenuAndForm = await db.tbMenuAndForms.FindAsync(id);
            if (tbMenuAndForm == null)
            {
                TempData["speaker"] = new speaker { type = 3, title = "Warning!", content = "Không tìm thấy dữ liệu...!!!" };
                return HttpNotFound();
            }
            ViewBag.parentID = new SelectList(db.tbMenuAndForms, "menuID", "name", tbMenuAndForm.parentID);
            return PartialView(tbMenuAndForm);
        }

        // POST: MenuAndForms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "menuID,parentID,name,icon,redirect,sort,isForm,jsonData,del")] tbMenuAndForm tbMenuAndForm)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbMenuAndForm).State = EntityState.Modified;
                await db.SaveChangesAsync();
                TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Cập nhật thành công...!!!" };
                if (Request.IsAjaxRequest())
                    return Json("update successfully!");

                return RedirectToAction("Index");
            }
            ViewBag.parentID = new SelectList(db.tbMenuAndForms, "menuID", "name", tbMenuAndForm.parentID);
            TempData["speaker"] = new speaker { type = 3, title = "Warning!", content = "Token or form data is note valid...!!!" };
            return PartialView(tbMenuAndForm);
        }

        // GET: MenuAndForms/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbMenuAndForm tbMenuAndForm = await db.tbMenuAndForms.FindAsync(id);
            if (tbMenuAndForm == null)
            {
                return HttpNotFound();
            }
            return View(tbMenuAndForm);
        }

        // POST: MenuAndForms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int[] ids)
        {
            foreach (var id in ids)
            {
                tbMenuAndForm tbMenuAndForm = await db.tbMenuAndForms.FindAsync(id);
                db.tbMenuAndForms.Remove(tbMenuAndForm);
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
