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
    
    public class StudentsController : Controller
    {
        private CRM_TTVEntities db = new CRM_TTVEntities();

        #region DS Học viên
        // GET: Students
        public async Task<ActionResult> Index(string order, Int32? size = 10, Int32? page = 1)
        {
            //var tbStudents = db.tbStudents.Include(t => t.tbCategory).Include(t => t.tbClass).Include(t => t.tbUser);
            //return View(await tbStudents.ToListAsync());

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
                var tbStudents = db.tbStudents.OrderBy(orderBy[0] + " " + orderBy[1]).Skip(skip).Take(take).Include(t => t.tbCategory).Include(t => t.tbClass).Include(t => t.tbUser);
                ViewBag.Paging = Paging.Pagination(db.tbStudents.Count(), page, size);
                return View(await tbStudents.ToListAsync());
            }
            else
            {
                //trả về danh sách mặc định từ csdl và xắp xếp theo ID giảm dần
                var tbStudents = db.tbStudents.OrderByDescending(x => x.idStudent).Skip(skip).Take(take).Include(t => t.tbCategory).Include(t => t.tbClass).Include(t => t.tbUser);
                ViewBag.Paging = Paging.Pagination(db.tbStudents.Count(), page, size);
                return View(await tbStudents.ToListAsync());
            }
        }

        #endregion

        #region Chi tiết

        // GET: Students/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbStudent tbStudent = await db.tbStudents.FindAsync(id);
            if (tbStudent == null)
            {
                return HttpNotFound();
            }
            return PartialView(tbStudent);
        }
        #endregion

        #region Hồ sơ
        [HttpGet]
        // GET: Students/Document/4
        [Route("Students/Document/{idStudent}")]
        public async Task<ActionResult> Documents(int? id)
        {
            var tbStudentFileAttaches = db.tbStudentFileAttaches.Where(z => z.idStudent == id).OrderByDescending(x => x.postDate);
            ViewBag.idStudent = id;
            return PartialView(await tbStudentFileAttaches.ToListAsync());
        }

        public ActionResult GetStudentId(int id)
        {
            ViewBag.idStudent = id;
            ViewBag.fileType = new SelectList(db.tbCategories.Where(p=>p.idCategoryType == 21), "idCategory", "name");
            return PartialView("_Partial_CreateFile");
        }

        public ActionResult UploadFile(HttpPostedFileBase fileName)
        {
            Session["fileNameStudent"] = fileName;
            return Json(JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> CreateFile(tbStudentFileAttach model)
        {
            try
            {
                model.byUser = 4;
                model.postDate = DateTime.Now;
                if (Session["fileNameStudent"] != null)
                {
                    var Source = Session["fileNameStudent"] as HttpPostedFileBase;
                    String newName = Source.FileName.Insert(Source.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/Student/" + newName);
                    Source.SaveAs(path);
                    model.fileName = newName;
                }
                db.tbStudentFileAttaches.Add(model);
                await db.SaveChangesAsync();
                Session["fileNameStudent"] = null;
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
            ViewBag.idStudent = model.idStudent;
            var tbStudentFileAttaches = db.tbStudentFileAttaches.Where(z => z.idStudent == model.idStudent).OrderByDescending(x => x.postDate);
            return PartialView("Documents", await tbStudentFileAttaches.ToListAsync());

        }

        public async Task<ActionResult> EditFile(int id)
        {
            var model = await db.tbStudentFileAttaches.FindAsync(id);
            ViewBag.fileType = new SelectList(db.tbCategories.Where(p=>p.idCategoryType == 21), "idCategory", "name", model.fileType);
            return PartialView("_Partial_EditFile", model);
        }

        public async Task<ActionResult> UpdateFile(tbStudentFileAttach model)
        {
            model.byUser = 4;
            model.postDate = DateTime.Now;
            if (Session["fileNameStudent"] != null)
            {
                var Source = Session["fileNameStudent"] as HttpPostedFileBase;
                String newName = Source.FileName.Insert(Source.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                String path = Server.MapPath("~/Upload/Student/" + newName);
                Source.SaveAs(path);
                model.fileName = newName;
            }
            db.Entry(model).State = EntityState.Modified;
            await db.SaveChangesAsync();
            Session["fileNameStudent"] = null;
            //
            ViewBag.idStudent = model.idStudent;
            var tbStudentFileAttaches =  db.tbStudentFileAttaches.Where(z => z.idStudent == model.idStudent).OrderByDescending(x => x.postDate);
            return PartialView("Documents", await tbStudentFileAttaches.ToListAsync());
        }

        public async Task<ActionResult> DeleteFile(int id)
        {
            var item = await db.tbStudentFileAttaches.FindAsync(id);
            // xóa file
            string fullPath = Request.MapPath("~/Upload/Student/" + item.fileName);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
            // xóa data
            db.tbStudentFileAttaches.Remove(item);
            await db.SaveChangesAsync();
            return Json(JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Thêm mới
        // GET: Students/Create
        public ActionResult Create()
        {
            ViewBag.achievements = new SelectList(db.tbCategories.Where(p => p.idCategoryType == 12), "idCategory", "name");
            ViewBag.idClass = new SelectList(db.tbClasses, "idClass", "name");
            ViewBag.idUser = new SelectList(db.tbUsers, "userID", "fullName");
            return PartialView();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idStudent,idUser,idClass,startDate,endDate,achievements,code,timeLess,listDateabsent,status")] tbStudent tbStudent)
        {
            if (ModelState.IsValid)
            {
                db.tbStudents.Add(tbStudent);
                await db.SaveChangesAsync();
                TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Thành công...!!!" };

                if (Request.IsAjaxRequest())
                    return Json("insert successfully!");

                return RedirectToAction("Index");
            }

            ViewBag.achievements = new SelectList(db.tbCategories.Where(x => x.idCategoryType == 12), "idCategory", "name", tbStudent.achievements);
            ViewBag.idClass = new SelectList(db.tbClasses, "idClass", "name", tbStudent.idClass);
            ViewBag.idUser = new SelectList(db.tbUsers, "userID", "fullName", tbStudent.idUser);
            TempData["speaker"] = new speaker { type = 2, title = "Error!", content = "Đã có lỗi xẩy ra...!!!" };
            return new HttpStatusCodeResult(500);
        }

        #endregion

        #region Cập nhật

        // GET: Students/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbStudent tbStudent = await db.tbStudents.FindAsync(id);
            if (tbStudent == null)
            {
                TempData["speaker"] = new speaker { type = 3, title = "Warning!", content = "Không tìm thấy dữ liệu...!!!" };
                return HttpNotFound();
            }
            ViewBag.achievements = new SelectList(db.tbCategories.Where(x => x.idCategoryType == 12), "idCategory", "name", tbStudent.achievements);
            ViewBag.idClass = new SelectList(db.tbClasses, "idClass", "name", tbStudent.idClass);
            ViewBag.idUser = new SelectList(db.tbUsers, "userID", "fullName", tbStudent.idUser);
            return PartialView(tbStudent);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idStudent,idUser,idClass,startDate,endDate,achievements,code,timeLess,listDateabsent,status")] tbStudent tbStudent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbStudent).State = EntityState.Modified;
                await db.SaveChangesAsync();
                TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Cập nhật thành công...!!!" };
                if (Request.IsAjaxRequest())
                    return Json("update successfully!");

                return RedirectToAction("Index");
            }
            ViewBag.achievements = new SelectList(db.tbCategories.Where(x => x.idCategoryType == 12), "idCategory", "name", tbStudent.achievements);
            ViewBag.idClass = new SelectList(db.tbClasses, "idClass", "name", tbStudent.idClass);
            ViewBag.idUser = new SelectList(db.tbStudents, "userID", "fullName", tbStudent.idUser);
            TempData["speaker"] = new speaker { type = 3, title = "Warning!", content = "Token or form data is note valid...!!!" };
            return PartialView(tbStudent);
        }

        #endregion

        #region Xóa
        // GET: Students/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbStudent tbStudent = await db.tbStudents.FindAsync(id);
            if (tbStudent == null)
            {
                return HttpNotFound();
            }
            return View(tbStudent);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int[] ids)
        {
            foreach (var id in ids)
            {
                tbStudent tbStudent = await db.tbStudents.FindAsync(id);
                db.tbStudents.Remove(tbStudent);
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
