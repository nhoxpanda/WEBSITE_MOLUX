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
    
    public class RoleTypesController : Controller
    {
        private CRM_TTVEntities db = new CRM_TTVEntities();

        // GET: RoleTypes
        public async Task<ActionResult> Index(string order, Int32? size = 10, Int32? page = 1)
        {
            //return View(await db.tbRoleTypes.ToListAsync());

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
            //var ghghgh = db.tbRoleTypes.OrderBy("name asc");

            string[] orderBy = null;
            if (!string.IsNullOrEmpty(order))
            {
                // trả về kết quả xắp xếp theo yêu cầu của client
                orderBy = order.Split('-');
                var tbRoleTypes = db.tbRoleTypes.OrderBy(orderBy[0] + " " + orderBy[1]).Skip(skip).Take(take);
                ViewBag.Paging = Paging.Pagination(db.tbRoleTypes.Count(), page, size);
                return View(await tbRoleTypes.ToListAsync());
            }
            else
            {
                //trả về danh sách mặc định từ csdl và xắp xếp theo ID giảm dần
                var tbRoleTypes = db.tbRoleTypes.OrderByDescending(x => x.idRoleType).Skip(skip).Take(take);
                ViewBag.Paging = Paging.Pagination(db.tbRoleTypes.Count(), page, size);
                return View(await tbRoleTypes.ToListAsync());
            }
        }

        // GET: RoleTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbRoleType tbRoleType = await db.tbRoleTypes.FindAsync(id);
            if (tbRoleType == null)
            {
                return HttpNotFound();
            }
            return PartialView(tbRoleType);
        }

        // GET: RoleTypes/Create
        public ActionResult Create()
        {
            return PartialView();
        }

        // POST: RoleTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idRoleType,name,sort")] tbRoleType tbRoleType)
        {
            if (ModelState.IsValid)
            {
                db.tbRoleTypes.Add(tbRoleType);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            TempData["speaker"] = new speaker { type = 2, title = "Error!", content = "Đã có lỗi xẩy ra...!!!" };
            return RedirectToAction("Index");
        }

        // GET: RoleTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbRoleType tbRoleType = await db.tbRoleTypes.FindAsync(id);
            if (tbRoleType == null)
            {
                return HttpNotFound();
            }
            TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Cập nhật thành công...!!!" };

            return PartialView(tbRoleType);
        }

        // POST: RoleTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idRoleType,name,sort")] tbRoleType tbRoleType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbRoleType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return PartialView(tbRoleType);
        }

        // GET: RoleTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbRoleType tbRoleType = await db.tbRoleTypes.FindAsync(id);
            if (tbRoleType == null)
            {
                return HttpNotFound();
            }
            return View(tbRoleType);
        }

        // POST: RoleTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int[] ids)
        {
            foreach (var id in ids)
            {
                tbRoleType tbRoleType = await db.tbRoleTypes.FindAsync(id);
                db.tbRoleTypes.Remove(tbRoleType);
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
