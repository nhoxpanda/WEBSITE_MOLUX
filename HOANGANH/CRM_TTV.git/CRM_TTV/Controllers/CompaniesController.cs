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
    
    public class CompaniesController : Controller
    {
        private CRM_TTVEntities db = new CRM_TTVEntities();

        // GET: Companies
        #region DS chi nhánh

        public async Task<ActionResult> Index(string order, Int32? size = 10, Int32? page = 1)
        {
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
            //var ghghgh = db.tbCompanies.OrderBy("name asc");

            string[] orderBy = null;
            if (!string.IsNullOrEmpty(order))
            {
                // trả về kết quả xắp xếp theo yêu cầu của client
                orderBy = order.Split('-');
                var tbCompanies = db.tbCompanies.OrderBy(orderBy[0] + " " + orderBy[1]).Skip(skip).Take(take).Include(t => t.tbCompany2);
                ViewBag.Paging = Paging.Pagination(db.tbCompanies.Count(), page, size);
                return View(await tbCompanies.ToListAsync());
            }
            else
            {
                //trả về danh sách mặc định từ csdl và xắp xếp theo ID giảm dần
                var tbCompanies = db.tbCompanies.OrderByDescending(x => x.idCompany).Skip(skip).Take(take).Include(t => t.tbCompany2);
                ViewBag.Paging = Paging.Pagination(db.tbCompanies.Count(), page, size);
                return View(await tbCompanies.ToListAsync());
            }


        }

        #endregion

        #region chi tiết chi nhánh
        // GET: Companies/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbCompany tbCompany = await db.tbCompanies.FindAsync(id);
            if (tbCompany == null)
            {
                return HttpNotFound();
            }
            return PartialView(tbCompany);
        }

        #endregion

        #region liên hệ
        [HttpGet]
        // GET: Companies/Document/4
        [Route("Companies/Contacts/{idCompany}")]
        public async Task<ActionResult> Contacts(int? id)
        {
            var tbCompanyContact = db.tbCompanyContacts.Where(z => z.idCompany == id);
            ViewBag.idCompany = id;
            return PartialView(await tbCompanyContact.ToListAsync());
        }

        public async Task<ActionResult> CreateContact(tbCompanyContact model)
        {
            try
            {
                db.tbCompanyContacts.Add(model);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
            ViewBag.idCompany = model.idCompany;
            var tbCompanyContacts = db.tbCompanyContacts.Where(z => z.idCompany == model.idCompany);
            return PartialView("Contacts", await tbCompanyContacts.ToListAsync());
        }

        public async Task<ActionResult> EditContact(int id)
        {
            var model = await db.tbCompanyContacts.FindAsync(id);
            return PartialView("_Partial_EditContact", model);
        }

        public async Task<ActionResult> UpdateContact(tbCompanyContact model)
        {
            db.Entry(model).State = EntityState.Modified;
            await db.SaveChangesAsync();
            ViewBag.idCompany = model.idCompany;
            var tbCompanyContacts = db.tbCompanyContacts.Where(z => z.idCompany == model.idCompany);
            return PartialView("Contacts", await tbCompanyContacts.ToListAsync());
        }

        public async Task<ActionResult> DeleteContact(int id)
        {
            var item = await db.tbCompanyContacts.FindAsync(id);
            db.tbCompanyContacts.Remove(item);
            await db.SaveChangesAsync();
            return Json(JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Hồ sơ
        [HttpGet]
        // GET: Companies/Document/4
        [Route("Companies/Document/{idCompany}")]
        public async Task<ActionResult> Documents(int? id)
        {
            var tbCompanyFileAttacheses = db.tbCompanyFileAttaches.Where(z => z.idCompany == id).OrderByDescending(x => x.postDate);
            ViewBag.idCompany = id;
            return PartialView(await tbCompanyFileAttacheses.ToListAsync());
        }

        public ActionResult GetCompanyId(int id, string tab)
        {
            ViewBag.idCompany = id;
            if (tab == "Documents")
            {
                return PartialView("_Partial_CreateFile");
            }
            else
            {
                return PartialView("_Partial_CreateContact");
            }
        }

        public ActionResult UploadFile(HttpPostedFileBase fileName)
        {
            Session["fileNameCompany"] = fileName;
            return Json(JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> CreateFile(tbCompanyFileAttach model)
        {
            try
            {
                model.uploadByUser = 4;
                model.postDate = DateTime.Now;
                if (Session["fileNameCompany"] != null)
                {
                    var Source = Session["fileNameCompany"] as HttpPostedFileBase;
                    String newName = Source.FileName.Insert(Source.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/Student/" + newName);
                    Source.SaveAs(path);
                    model.fileName = newName;
                }
                db.tbCompanyFileAttaches.Add(model);
                await db.SaveChangesAsync();
                Session["fileNameCompany"] = null;
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
            ViewBag.idCompany = model.idCompany;
            var tbCompanyFileAttacheses = db.tbCompanyFileAttaches.Where(z => z.idCompany == model.idCompany).OrderByDescending(x => x.postDate);
            return PartialView("Documents", await tbCompanyFileAttacheses.ToListAsync());

        }

        public async Task<ActionResult> EditFile(int id)
        {
            var model = await db.tbCompanyFileAttaches.FindAsync(id);
            return PartialView("_Partial_EditFile", model);
        }

        public async Task<ActionResult> UpdateFile(tbCompanyFileAttach model)
        {
            model.uploadByUser = 4;
            model.postDate = DateTime.Now;
            if (Session["fileNameCompany"] != null)
            {
                var Source = Session["fileNameCompany"] as HttpPostedFileBase;
                String newName = Source.FileName.Insert(Source.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                String path = Server.MapPath("~/Upload/Company/" + newName);
                Source.SaveAs(path);
                model.fileName = newName;
            }
            db.Entry(model).State = EntityState.Modified;
            await db.SaveChangesAsync();
            Session["fileNameCompany"] = null;
            //
            ViewBag.idCompany = model.idCompany;
            var tbCompanyFileAttacheses = db.tbCompanyFileAttaches.Where(z => z.idCompany == model.idCompany).OrderByDescending(x => x.postDate);
            return PartialView("Documents", await tbCompanyFileAttacheses.ToListAsync());
        }

        public async Task<ActionResult> DeleteFile(int id)
        {
            var item = await db.tbCompanyFileAttaches.FindAsync(id);
            // xóa file
            string fullPath = Request.MapPath("~/Upload/Company/" + item.fileName);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
            // xóa data
            db.tbCompanyFileAttaches.Remove(item);
            await db.SaveChangesAsync();
            return Json(JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region thêm chi nhánh mới
        // GET: Companies/Create
        public ActionResult Create()
        {
            ViewBag.parentID = new SelectList(db.tbCompanies, "idCompany", "name");
            return PartialView();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idCompany,parentID,name,taxCode,address,phone,email,website,banking,sort")] tbCompany tbCompany)
        {
            if (ModelState.IsValid)
            {
                db.tbCompanies.Add(tbCompany);
                await db.SaveChangesAsync();
                TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Thành công...!!!" };
                return RedirectToAction("Index");
            }

            ViewBag.parentID = new SelectList(db.tbCompanies, "idCompany", "name", tbCompany.parentID);
            TempData["speaker"] = new speaker { type = 2, title = "Error!", content = "Đã có lỗi xẩy ra...!!!" };
            return RedirectToAction("Index");
            //return View(tbCompany);
        }

        #endregion

        #region cập nhật chi nhánh
        // GET: Companies/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbCompany tbCompany = await db.tbCompanies.FindAsync(id);
            if (tbCompany == null)
            {
                return HttpNotFound();
            }
            ViewBag.parentID = new SelectList(db.tbCompanies, "idCompany", "name", tbCompany.parentID);
            TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Cập nhật thành công...!!!" };

            return PartialView(tbCompany);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idCompany,parentID,name,taxCode,address,phone,email,website,banking,sort")] tbCompany tbCompany)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbCompany).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.parentID = new SelectList(db.tbCompanies, "idCompany", "name", tbCompany.parentID);
            return PartialView(tbCompany);
        }

        #endregion

        #region xóa chi nhánh
        // GET: Companies/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbCompany tbCompany = await db.tbCompanies.FindAsync(id);
            if (tbCompany == null)
            {
                return HttpNotFound();
            }
            return View(tbCompany);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int[] ids)
        {
            foreach (var id in ids)
            {
                tbCompany tbCompany = await db.tbCompanies.FindAsync(id);
                db.tbCompanies.Remove(tbCompany);
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
