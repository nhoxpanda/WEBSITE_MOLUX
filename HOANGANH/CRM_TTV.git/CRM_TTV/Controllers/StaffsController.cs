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
using CRM_TTV.Authorize;

namespace CRM_TTV.Controllers
{
    [Authorize]
    [DinamicAuthorize]
    public class StaffsController : Controller
    {
        private CRM_TTVEntities db = new CRM_TTVEntities();

        // GET: Staffs
        #region DS nhân viên

        public async Task<ActionResult> Index(string order, Int32? size = 10, Int32? page = 1)
        {
            //var tbUsers = db.tbUsers.Include(t => t.tbCategory).Include(t => t.tbCategory1).Include(t => t.tbCategory2).Include(t => t.tbCategory3).Include(t => t.tbCompany).Include(t => t.tbRegion).Include(t => t.tbRegion1).Include(t => t.tbRole).Include(t => t.tbRole1).Include(t => t.tbRole2).Include(t => t.tbRole3).Include(t => t.tbUser2);
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
            //var ghghgh = db.tbUsers.OrderBy("name asc");

            string[] orderBy = null;
            if (!string.IsNullOrEmpty(order))
            {
                // trả về kết quả xắp xếp theo yêu cầu của client
                orderBy = order.Split('-');
                var tbUsers = db.tbUsers.OrderBy(orderBy[0] + " " + orderBy[1]).Skip(skip).Take(take).Include(t => t.tbCategory).Include(t => t.tbCategory1).Include(t => t.tbCategory2).Include(t => t.tbCategory3).Include(t => t.tbCompany).Include(t => t.tbRegion).Include(t => t.tbRegion1).Include(t => t.tbRole).Include(t => t.tbRole1).Include(t => t.tbRole2).Include(t => t.tbRole3).Include(t => t.tbUser2);
                ViewBag.Paging = Paging.Pagination(db.tbUsers.Count(), page, size);
                return View(await tbUsers.ToListAsync());
            }
            else
            {
                //trả về danh sách mặc định từ csdl và xắp xếp theo ID giảm dần
                var tbUsers = db.tbUsers.OrderByDescending(x => x.userID).Skip(skip).Take(take).Include(t => t.tbCategory).Include(t => t.tbCategory1).Include(t => t.tbCategory2).Include(t => t.tbCategory3).Include(t => t.tbCompany).Include(t => t.tbRegion).Include(t => t.tbRegion1).Include(t => t.tbRole).Include(t => t.tbRole1).Include(t => t.tbRole2).Include(t => t.tbRole3).Include(t => t.tbUser2);
                ViewBag.Paging = Paging.Pagination(db.tbUsers.Count(), page, size);
                return View(await tbUsers.ToListAsync());
            }
        }
        #endregion

        #region Chi tiết nhân viên
        // GET: Staffs/Details/5
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

        #endregion

        #region Hồ sơ
        [HttpGet]
        // GET: Staffs/Document/4
        [Route("Staffs/Document/{idUser}")]
        public async Task<ActionResult> Documents(int? id)
        {
            var tbUserFileAttaches = db.tbUserFileAttaches.Where(z => z.idUser == id).OrderByDescending(x => x.postDate);
            ViewBag.idUser = id;
            return PartialView(await tbUserFileAttaches.ToListAsync());
        }

        public ActionResult GetUserId(int id)
        {
            ViewBag.idUser = id;
            ViewBag.fileType = new SelectList(db.tbCategories.Where(p => p.idCategoryType == 21), "idCategory", "name");
            return PartialView("_Partial_CreateFile");
        }

        public ActionResult UploadFile(HttpPostedFileBase fileName)
        {
            Session["fileNameStaff"] = fileName;
            return Json(JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> CreateFile(tbUserFileAttach model)
        {
            try
            {
                model.byUser = 4;
                model.postDate = DateTime.Now;
                if (Session["fileNameStaff"] != null)
                {
                    var Source = Session["fileNameStaff"] as HttpPostedFileBase;
                    String newName = Source.FileName.Insert(Source.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/Staff/" + newName);
                    Source.SaveAs(path);
                    model.fileName = newName;
                }
                db.tbUserFileAttaches.Add(model);
                await db.SaveChangesAsync();
                Session["fileNameStaff"] = null;
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
            ViewBag.idUser = model.idUser;
            var tbUserFileAttaches = db.tbUserFileAttaches.Where(z => z.idUser == model.idUser).OrderByDescending(x => x.postDate);
            return PartialView("Documents", await tbUserFileAttaches.ToListAsync());

        }

        public async Task<ActionResult> EditFile(int id)
        {
            var model = await db.tbUserFileAttaches.FindAsync(id);
            ViewBag.fileType = new SelectList(db.tbCategories.Where(p => p.idCategoryType == 21), "idCategory", "name", model.fileType);
            return PartialView("_Partial_EditFile", model);
        }

        public async Task<ActionResult> UpdateFile(tbUserFileAttach model)
        {
            model.byUser = 4;
            model.postDate = DateTime.Now;
            if (Session["fileNameStaff"] != null)
            {
                var Source = Session["fileNameStaff"] as HttpPostedFileBase;
                String newName = Source.FileName.Insert(Source.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                String path = Server.MapPath("~/Upload/Staff/" + newName);
                Source.SaveAs(path);
                model.fileName = newName;
            }
            db.Entry(model).State = EntityState.Modified;
            await db.SaveChangesAsync();
            Session["fileNameStaff"] = null;
            //
            ViewBag.idUser = model.idUser;
            var tbUserFileAttaches = db.tbUserFileAttaches.Where(z => z.idUser == model.idUser).OrderByDescending(x => x.postDate);
            return PartialView("Documents", await tbUserFileAttaches.ToListAsync());
        }

        public async Task<ActionResult> DeleteFile(int id)
        {
            var item = await db.tbUserFileAttaches.FindAsync(id);
            // xóa file
            string fullPath = Request.MapPath("~/Upload/Staff/" + item.fileName);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
            // xóa data
            db.tbUserFileAttaches.Remove(item);
            await db.SaveChangesAsync();
            return Json(JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Thêm nhân viên mới

        // GET: Staffs/Create
        public ActionResult Create()
        {
            ViewBag.idState = new SelectList(db.tbCategories, "idCategory", "name");
            ViewBag.idJob = new SelectList(db.tbCategories.Where(x => x.idCategoryType == 4), "idCategory", "name");
            ViewBag.healthStatus = new SelectList(db.tbCategories.Where(x => x.idCategoryType == 13), "idCategory", "name");
            ViewBag.lever = new SelectList(db.tbCategories.Where(x => x.idCategoryType == 6), "idCategory", "name");
            ViewBag.idCompany = new SelectList(db.tbCompanies, "idCompany", "name");
            ViewBag.city = new SelectList(db.tbRegions.Where(x => x.type == 2096), "idRegion", "name");
            ViewBag.district = new SelectList(db.tbRegions.Where(x => x.type == 2098), "idRegion", "name");
            ViewBag.roleID = new SelectList(db.tbRoles.Where(x => x.idRoleType == 1), "roleID", "name");
            ViewBag.position = new SelectList(db.tbRoles.Where(x => x.idRoleType == 1), "roleID", "name");
            ViewBag.department = new SelectList(db.tbRoles.Where(x => x.idRoleType == 2), "roleID", "name");
            ViewBag.groupMemb = new SelectList(db.tbRoles.Where(x => x.idRoleType == 3), "roleID", "name");
            ViewBag.idParent = new SelectList(db.tbUsers, "userID", "fullName");
            return PartialView();
        }

        // POST: Staffs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "userID,idParent,idCompany,fullName,birthDay,idState,idJob,healthStatus,phone,phone2,experience,trainingPlaces,InternalNumber,citizenship,city,district,passport,dateIssued,locationIssued,homeTown,resident,address,aspiration,sex,married,yogaLearned,email,skype,facebook,state,code,roleID,position,department,groupMemb,userRole,dateUnlock,dateLock,lever,bonusScore,accumulativePoint,salary,partTime,working,dateToWork,dateTrialEnds,codeHDLD,IssuedDateHDLD,HDLDStartDate,HDLDEndDate,codeBHXH,taxCode,DateRegisterTaxCode,locationRegisterTaxCode,banking,username,password,locked,reason,note")] tbUser tbUser)
        {
            if (ModelState.IsValid)
            {
                db.tbUsers.Add(tbUser);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.idState = new SelectList(db.tbCategories, "idCategory", "name", tbUser.idState);
            ViewBag.idJob = new SelectList(db.tbCategories, "idCategory", "name", tbUser.idJob);
            ViewBag.healthStatus = new SelectList(db.tbCategories, "idCategory", "name", tbUser.healthStatus);
            ViewBag.lever = new SelectList(db.tbCategories, "idCategory", "name", tbUser.lever);
            ViewBag.idCompany = new SelectList(db.tbCompanies, "idCompany", "name", tbUser.idCompany);
            ViewBag.city = new SelectList(db.tbRegions, "idRegion", "name", tbUser.city);
            ViewBag.district = new SelectList(db.tbRegions, "idRegion", "name", tbUser.district);
            ViewBag.roleID = new SelectList(db.tbRoles, "roleID", "name", tbUser.roleID);
            ViewBag.position = new SelectList(db.tbRoles, "roleID", "name", tbUser.position);
            ViewBag.department = new SelectList(db.tbRoles, "roleID", "name", tbUser.department);
            ViewBag.groupMemb = new SelectList(db.tbRoles, "roleID", "name", tbUser.groupMemb);
            ViewBag.idParent = new SelectList(db.tbUsers, "userID", "fullName", tbUser.idParent);
            TempData["speaker"] = new speaker { type = 2, title = "Error!", content = "Đã có lỗi xẩy ra...!!!" };
            return RedirectToAction("Index");
        }

        #endregion

        #region Cập nhật nhân viên

        // GET: Staffs/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
            ViewBag.idState = new SelectList(db.tbCategories, "idCategory", "name", tbUser.idState);
            ViewBag.idJob = new SelectList(db.tbCategories, "idCategory", "name", tbUser.idJob);
            ViewBag.healthStatus = new SelectList(db.tbCategories, "idCategory", "name", tbUser.healthStatus);
            ViewBag.lever = new SelectList(db.tbCategories, "idCategory", "name", tbUser.lever);
            ViewBag.idCompany = new SelectList(db.tbCompanies, "idCompany", "name", tbUser.idCompany);
            ViewBag.city = new SelectList(db.tbRegions, "idRegion", "name", tbUser.city);
            ViewBag.district = new SelectList(db.tbRegions, "idRegion", "name", tbUser.district);
            ViewBag.roleID = new SelectList(db.tbRoles, "roleID", "name", tbUser.roleID);
            ViewBag.position = new SelectList(db.tbRoles, "roleID", "name", tbUser.position);
            ViewBag.department = new SelectList(db.tbRoles, "roleID", "name", tbUser.department);
            ViewBag.groupMemb = new SelectList(db.tbRoles, "roleID", "name", tbUser.groupMemb);
            ViewBag.idParent = new SelectList(db.tbUsers, "userID", "fullName", tbUser.idParent);
            TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Cập nhật thành công...!!!" };

            return PartialView(tbUser);
        }

        // POST: Staffs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "userID,idParent,idCompany,fullName,birthDay,idState,idJob,healthStatus,phone,phone2,experience,trainingPlaces,InternalNumber,citizenship,city,district,passport,dateIssued,locationIssued,homeTown,resident,address,aspiration,sex,married,yogaLearned,email,skype,facebook,state,code,roleID,position,department,groupMemb,userRole,dateUnlock,dateLock,lever,bonusScore,accumulativePoint,salary,partTime,working,dateToWork,dateTrialEnds,codeHDLD,IssuedDateHDLD,HDLDStartDate,HDLDEndDate,codeBHXH,taxCode,DateRegisterTaxCode,locationRegisterTaxCode,banking,username,password,locked,reason,note")] tbUser tbUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbUser).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.idState = new SelectList(db.tbCategories, "idCategory", "name", tbUser.idState);
            ViewBag.idJob = new SelectList(db.tbCategories, "idCategory", "name", tbUser.idJob);
            ViewBag.healthStatus = new SelectList(db.tbCategories, "idCategory", "name", tbUser.healthStatus);
            ViewBag.lever = new SelectList(db.tbCategories, "idCategory", "name", tbUser.lever);
            ViewBag.idCompany = new SelectList(db.tbCompanies, "idCompany", "name", tbUser.idCompany);
            ViewBag.city = new SelectList(db.tbRegions, "idRegion", "name", tbUser.city);
            ViewBag.district = new SelectList(db.tbRegions, "idRegion", "name", tbUser.district);
            ViewBag.roleID = new SelectList(db.tbRoles, "roleID", "name", tbUser.roleID);
            ViewBag.position = new SelectList(db.tbRoles, "roleID", "name", tbUser.position);
            ViewBag.department = new SelectList(db.tbRoles, "roleID", "name", tbUser.department);
            ViewBag.groupMemb = new SelectList(db.tbRoles, "roleID", "name", tbUser.groupMemb);
            ViewBag.idParent = new SelectList(db.tbUsers, "userID", "fullName", tbUser.idParent);
            return PartialView(tbUser);
        }

        #endregion

        #region Xóa nhân viên
        // GET: Staffs/Delete/5
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

        // POST: Staffs/Delete/5
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
        #endregion
    }
}
