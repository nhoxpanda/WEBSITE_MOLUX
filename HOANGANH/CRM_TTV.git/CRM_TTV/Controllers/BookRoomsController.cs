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
    
    public class BookRoomsController : Controller
    {
        private CRM_TTVEntities db = new CRM_TTVEntities();

        // GET: BookRooms
        public async Task<ActionResult> Index(string order, Int32? size = 10, Int32? page = 1)
        {
            //var tbBookRooms = db.tbBookRooms.Include(t => t.tbRoom).Include(t => t.tbTimeFrame).Include(t => t.tbUser).Include(t => t.tbClass);
            //return View(await tbBookRooms.ToListAsync());
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
                var tbBookRooms = db.tbBookRooms.OrderBy(orderBy[0] + " " + orderBy[1]).Skip(skip).Take(take).Include(t => t.tbRoom).Include(t => t.tbTimeFrame).Include(t => t.tbUser).Include(t => t.tbClass);
                ViewBag.Paging = Paging.Pagination(db.tbBookRooms.Count(), page, size);
                return View(await tbBookRooms.ToListAsync());
            }
            else
            {
                //trả về danh sách mặc định từ csdl và xắp xếp theo ID giảm dần
                var tbBookRooms = db.tbBookRooms.OrderByDescending(x => x.idbookRoom).Skip(skip).Take(take).Include(t => t.tbRoom).Include(t => t.tbTimeFrame).Include(t => t.tbUser).Include(t => t.tbClass);
                ViewBag.Paging = Paging.Pagination(db.tbBookRooms.Count(), page, size);
                return View(await tbBookRooms.ToListAsync());
            }
        }

        // GET: BookRooms/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbBookRoom tbBookRoom = await db.tbBookRooms.FindAsync(id);
            if (tbBookRoom == null)
            {
                return HttpNotFound();
            }
            return PartialView(tbBookRoom);
        }

        // GET: BookRooms/Create
        public ActionResult Create()
        {
            ViewBag.idRoom = new SelectList(db.tbRooms, "idRoom", "name");
            ViewBag.idTimeFrame = new SelectList(db.tbTimeFrames, "idTimeFrame", "note");
            ViewBag.idUser = new SelectList(db.tbUsers, "userID", "fullName");
            ViewBag.idClass = new SelectList(db.tbClasses, "idClass", "name");
            return PartialView();
        }

        // POST: BookRooms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idbookRoom,idRoom,idClass,idUser,idTimeFrame,dateBook,note")] tbBookRoom tbBookRoom)
        {
            if (ModelState.IsValid)
            {
                db.tbBookRooms.Add(tbBookRoom);
                await db.SaveChangesAsync();
                TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Thành công...!!!" };

                if (Request.IsAjaxRequest())
                    return Json("insert successfully!");

                return RedirectToAction("Index");
            }

            ViewBag.idRoom = new SelectList(db.tbRooms, "idRoom", "name", tbBookRoom.idRoom);
            ViewBag.idTimeFrame = new SelectList(db.tbTimeFrames, "idTimeFrame", "note", tbBookRoom.idTimeFrame);
            ViewBag.idUser = new SelectList(db.tbUsers, "userID", "fullName", tbBookRoom.idUser);
            ViewBag.idClass = new SelectList(db.tbClasses, "idClass", "name", tbBookRoom.idClass);
            TempData["speaker"] = new speaker { type = 2, title = "Error!", content = "Đã có lỗi xẩy ra...!!!" };
            return new HttpStatusCodeResult(500);
        }

        // GET: BookRooms/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbBookRoom tbBookRoom = await db.tbBookRooms.FindAsync(id);
            if (tbBookRoom == null)
            {
                TempData["speaker"] = new speaker { type = 3, title = "Warning!", content = "Không tìm thấy dữ liệu...!!!" };
                return HttpNotFound();
            }
            ViewBag.idRoom = new SelectList(db.tbRooms, "idRoom", "name", tbBookRoom.idRoom);
            ViewBag.idTimeFrame = new SelectList(db.tbTimeFrames, "idTimeFrame", "note", tbBookRoom.idTimeFrame);
            ViewBag.idUser = new SelectList(db.tbUsers, "userID", "fullName", tbBookRoom.idUser);
            ViewBag.idClass = new SelectList(db.tbClasses, "idClass", "name", tbBookRoom.idClass);
            return PartialView(tbBookRoom);
        }

        // POST: BookRooms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idbookRoom,idRoom,idClass,idUser,idTimeFrame,dateBook,note")] tbBookRoom tbBookRoom)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbBookRoom).State = EntityState.Modified;
                await db.SaveChangesAsync();
                TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Cập nhật thành công...!!!" };
                if (Request.IsAjaxRequest())
                    return Json("update successfully!");

                return RedirectToAction("Index");
            }
            ViewBag.idRoom = new SelectList(db.tbRooms, "idRoom", "name", tbBookRoom.idRoom);
            ViewBag.idTimeFrame = new SelectList(db.tbTimeFrames, "idTimeFrame", "note", tbBookRoom.idTimeFrame);
            ViewBag.idUser = new SelectList(db.tbUsers, "userID", "fullName", tbBookRoom.idUser);
            ViewBag.idClass = new SelectList(db.tbClasses, "idClass", "name", tbBookRoom.idClass);
            TempData["speaker"] = new speaker { type = 3, title = "Warning!", content = "Token or form data is note valid...!!!" };
            return PartialView(tbBookRoom);
        }

        // GET: BookRooms/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbBookRoom tbBookRoom = await db.tbBookRooms.FindAsync(id);
            if (tbBookRoom == null)
            {
                return HttpNotFound();
            }
            return View(tbBookRoom);
        }

        // POST: BookRooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int[] ids)
        {
            foreach (var id in ids)
            {
                tbBookRoom tbBookRoom = await db.tbBookRooms.FindAsync(id);
                db.tbBookRooms.Remove(tbBookRoom);
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
