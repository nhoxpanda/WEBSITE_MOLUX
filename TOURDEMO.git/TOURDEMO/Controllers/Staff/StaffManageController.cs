using CRM.Core;
using CRM.Infrastructure;
using TOURDEMO.Models;
using TOURDEMO.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CRM.Enum;

namespace TOURDEMO.Controllers
{
    [Authorize]
    public class StaffManageController : BaseController
    {
        //
        // GET: /StaffManage/

        #region Init

        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_StaffSalary> _staffSalaryRepository;
        private IGenericRepository<tbl_StaffVisa> _staffVisaRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;
        private IGenericRepository<tbl_Task> _taskRepository;
        private IGenericRepository<tbl_TaskStaff> _taskStaffRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;
        private IGenericRepository<tbl_Headquater> _headquaterRepository;
        private DataContext _db;

        public StaffManageController(IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_StaffSalary> staffSalaryRepository,
            IGenericRepository<tbl_TaskStaff> taskStaffRepository,
            IGenericRepository<tbl_StaffVisa> staffVisaRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IGenericRepository<tbl_Task> taskRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IGenericRepository<tbl_Headquater> headquaterRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._staffVisaRepository = staffVisaRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._staffRepository = staffRepository;
            this._documentFileRepository = documentFileRepository;
            this._tagsRepository = tagsRepository;
            this._taskRepository = taskRepository;
            this._tourRepository = tourRepository;
            this._taskStaffRepository = taskStaffRepository;
            this._headquaterRepository = headquaterRepository;
            this._staffSalaryRepository = staffSalaryRepository;
            _db = new DataContext();
        }
        #endregion

        #region Index

        int SDBID = 6;
        int maPB = 0, maNKD = 0, maNV = 0, maCN = 0;
        void Permission(int PermissionsId, int formId)
        {
            var list = _db.tbl_ActionData.Where(p => p.FormId == formId && p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);
            ViewBag.IsImport = list.Contains(4);
            ViewBag.IsExport = list.Contains(5);
            ViewBag.IsLock = list.Contains(6);
            ViewBag.IsUnLock = list.Contains(7);

            var listNV = _db.tbl_ActionData.Where(p => p.FormId == 6 && p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAddNV = listNV.Contains(1);

            var ltAccess = _db.tbl_AccessData.Where(p => p.PermissionId == PermissionsId && p.FormId == formId).Select(p => p.ShowDataById).FirstOrDefault();
            if (ltAccess != 0)
                this.SDBID = ltAccess;

            switch (SDBID)
            {
                case 2:
                    maPB = clsPermission.GetUser().DepartmentID;
                    maCN = clsPermission.GetUser().BranchID;
                    break;
                case 3:
                    maNKD = clsPermission.GetUser().GroupID;
                    maCN = clsPermission.GetUser().BranchID; break;
                case 4: maNV = clsPermission.GetUser().StaffID; break;
                case 5: maCN = clsPermission.GetUser().BranchID; break;
            }
        }

        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 6);

            if (SDBID == 6)
                return View(new List<StaffListViewModel>());

            var model = _staffRepository.GetAllAsQueryable().AsEnumerable()
                    .Where(p => p.IsVietlike == true && (p.StaffId == maNV | maNV == 0)
                    & (p.DepartmentId == maPB | maPB == 0)
                    & (p.StaffGroupId == maNKD | maNKD == 0)
                    & (p.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false))
                    .OrderByDescending(p => p.CreatedDate)
                    .Select(p => new StaffListViewModel
                    {
                        Birthday = p.Birthday != null ? p.Birthday.Value.ToString("dd-MM-yyyy") : "",
                        Code = p.Code,
                        CreateDatePassport = p.CreatedDatePassport != null ? p.CreatedDatePassport.Value.ToString("dd-MM-yyyy") : "",
                        Department = p.DepartmentId != null ? p.tbl_DictionaryDepartment.Name : "",
                        Email = p.Email,
                        ExpiredDatePassport = p.ExpiredDatePassport != null ? p.ExpiredDatePassport.Value.ToString("dd-MM-yyyy") : "",
                        Fullname = p.FullName,
                        Id = p.Id,
                        InternalNumber = p.InternalNumber ?? 0,
                        IsLock = p.IsLock,
                        Passport = p.PassportCard != null ? p.PassportCard : "",
                        Phone = p.Phone,
                        Position = p.PositionId != null ? p.tbl_DictionaryPosition.Name : "",
                        Skype = p.Skype != null ? p.Skype : "",
                        IdentityCard = p.IdentityCard,
                        PlaceIdentity = p.tbl_TagsIdentity != null ? p.tbl_TagsIdentity.Tag : "",
                        CreateDateIdentity = p.CreatedDateIdentity != null ? string.Format("{0:dd-MM-yyyy}", p.CreatedDateIdentity) : "",
                        StartSalary = p.tbl_StaffSalary.FirstOrDefault() != null ? string.Format("{0:0,0}", p.tbl_StaffSalary.FirstOrDefault().StartSalary).Replace(",", ".") : "",
                        BasicSalary = p.tbl_StaffSalary.FirstOrDefault() != null ? string.Format("{0:0,0}", p.tbl_StaffSalary.FirstOrDefault().BasicSalary).Replace(",", ".") : "",
                        //StartDateSalary = p.tbl_StaffSalary.FirstOrDefault() != null ? string.Format("{0:dd-MM-yyyy}", p.tbl_StaffSalary.FirstOrDefault().StartDate) : "",
                        SubsidySalary = p.tbl_StaffSalary.FirstOrDefault() != null ? string.Format("{0:0,0}", p.tbl_StaffSalary.FirstOrDefault().SubsidySalary).Replace(",", ".") : "",
                        StartWork = p.StartWork != null ? p.StartWork.Value.ToString("dd-MM-yyyy") : "",
                        EndWork = p.EndWork != null ? p.EndWork.Value.ToString("dd-MM-yyyy") : ""
                    }).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult GetIdStaff(int id)
        {
            Session["idStaff"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Create
        [HttpPost]
        public async Task<ActionResult> Create(StaffViewModel model, FormCollection form, HttpPostedFileBase Image)
        {
            Permission(clsPermission.GetUser().PermissionID, 6);
            try
            {
                if (form["SingleStaff.TagsId"] != null && form["SingleStaff.TagsId"] != "")
                {
                    model.SingleStaff.TagsId = form["SingleStaff.TagsId"].ToString();
                }
                //model.SingleStaff.Code = GenerateCode.StaffCode();
                model.SingleStaff.CreatedDate = DateTime.Now;
                model.SingleStaff.ModifiedDate = DateTime.Now;
                model.SingleStaff.IdentityTagId = Convert.ToInt32(form["IdentityTagId"].ToString());
                model.SingleStaff.PassportTagId = Convert.ToInt32(form["PassportTagId"].ToString());
                model.SingleStaff.StaffId = clsPermission.GetUser().StaffID;
                model.SingleStaff.IsLock = false;
                model.SingleStaff.IsDelete = false;
                model.SingleStaff.IsVietlike = true;

                if (Image != null)
                {
                    //file
                    string FileSize = Common.ConvertFileSize(Image.ContentLength);
                    String newName = Image.FileName.Insert(Image.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    Image.SaveAs(path);
                    model.SingleStaff.Image = newName;
                    //end file
                }

                if (model.CreatedDateIdentity != null && model.CreatedDateIdentity.Value.Year >= 1980)
                {
                    model.SingleStaff.CreatedDateIdentity = model.CreatedDateIdentity;
                }
                if (model.CreatedDatePassport != null && model.CreatedDatePassport.Value.Year >= 1980)
                {
                    model.SingleStaff.CreatedDatePassport = model.CreatedDatePassport;
                }
                if (model.ExpiredDatePassport != null && model.ExpiredDatePassport.Value.Year >= 1980)
                {
                    model.SingleStaff.ExpiredDatePassport = model.ExpiredDatePassport;
                }

                if (await _staffRepository.Create(model.SingleStaff))
                {
                    UpdateHistory.SaveHistory(6, "Thêm mới nhân viên, code: " + model.SingleStaff.Code + " - " + model.SingleStaff.FullName,
                        null, //appointment
                        null, //contract
                        null, //customer
                        null, //partner
                        null, //program
                        null, //task
                        null, //tour
                        null, //quotation
                        null, //document
                        null, //history
                        null // ticket
                        );
                    // thêm vào bảng lương
                    var _luongThuViec = decimal.Parse(model.LuongThuViecNV.Replace(",", ""));
                    var salary = new tbl_StaffSalary()
                    {
                        //CreatedDate = DateTime.Now,
                        //IsDelete = false,
                        StaffId = model.SingleStaff.Id,
                        StartSalary = model.LuongThuViecNV != null ? _luongThuViec : 0,
                        Note = model.SingleStaffSalary.Note
                        //BasicSalary = model.SingleStaffSalary.BasicSalary != null ? model.SingleStaffSalary.BasicSalary : 0,
                        //StartSalary = model.SingleStaffSalary.StartSalary != null ? model.SingleStaffSalary.StartSalary : 0,
                        //SubsidySalary = model.SingleStaffSalary.SubsidySalary != null ? model.SingleStaffSalary.SubsidySalary : 0
                    };
                    await _staffSalaryRepository.Create(salary);

                    for (int i = 1; i < 6; i++)
                    {
                        if (form["VisaNumber" + i] != null && form["VisaNumber" + i] != "")
                        {
                            var visa = new tbl_StaffVisa
                            {
                                VisaNumber = form["VisaNumber" + i].ToString(),
                                TagsId = Convert.ToInt32(form["TagsId" + i].ToString()),
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now,
                                StaffId = model.SingleStaff.Id,
                                DictionaryId = 1069
                            };
                            if (Convert.ToDateTime(form["CreatedDateVisa" + i]).Year >= 1980)
                            {
                                visa.CreatedDateVisa = Convert.ToDateTime(form["CreatedDateVisa" + i]);
                            }
                            if (Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year >= 1980)
                            {
                                visa.ExpiredDateVisa = Convert.ToDateTime(form["ExpiredDateVisa" + i]);
                            }
                            if (Convert.ToDateTime(form["CreatedDateVisa" + i]).Year >= 1980 && (Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year >= 1980))
                            {
                                int age = Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year - Convert.ToDateTime(form["CreatedDateVisa" + i]).Year;
                                if (Convert.ToDateTime(form["CreatedDateVisa" + i]) > Convert.ToDateTime(form["ExpiredDateVisa" + i]).AddYears(-age)) age--;
                                visa.Deadline = age;
                            }
                            await _staffVisaRepository.Create(visa);
                        }
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch { }
            return RedirectToAction("Index");
        }
        #endregion

        #region Update

        /// <summary>
        /// load thông tin khách hàng
        /// </summary>
        /// <param name="model"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult StaffInformation(int id)
        {
            var model = new StaffViewModel();
            var staff = _staffRepository.GetAllAsQueryable().FirstOrDefault(p => p.Id == id);
            var staffVisa = _staffVisaRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false).Where(p => p.StaffId == id).ToList();
            model.SingleStaff = staff;
            model.SingleStaffSalary = staff.tbl_StaffSalary.FirstOrDefault();
            model.CreatedDateIdentity = staff.CreatedDateIdentity;
            model.CreatedDatePassport = staff.CreatedDatePassport;
            model.ExpiredDatePassport = staff.ExpiredDatePassport;
            model.IdentityCard = staff.IdentityCard;
            model.IdentityTagId = staff.IdentityTagId ?? 0;
            model.PassportCard = staff.PassportCard;
            model.PassportTagId = staff.PassportTagId ?? 0;
            if (staffVisa.Count() > 0)
            {
                model.ListStaffVisa = staffVisa;
            }
            return PartialView("_Partial_EditStaff", model);
        }

        /// <summary>
        /// cập nhật nhân viên
        /// </summary>
        /// <param name="model"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(StaffViewModel model, FormCollection form, HttpPostedFileBase Image)
        {
            Permission(clsPermission.GetUser().PermissionID, 6);
            try
            {
                if (form["SingleStaff.TagsId"] != null && form["SingleStaff.TagsId"] != "")
                {
                    model.SingleStaff.TagsId = form["SingleStaff.TagsId"].ToString();
                }
                model.SingleStaff.ModifiedDate = DateTime.Now;
                model.SingleStaff.IsVietlike = true;
                model.SingleStaff.IdentityTagId = Convert.ToInt32(form["IdentityTagId"].ToString());
                model.SingleStaff.PassportTagId = Convert.ToInt32(form["PassportTagId"].ToString());

                if (Image != null)
                {
                    //file
                    string FileSize = Common.ConvertFileSize(Image.ContentLength);
                    String newName = Image.FileName.Insert(Image.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    Image.SaveAs(path);
                    model.SingleStaff.Image = newName;
                    //end file
                }

                if (model.CreatedDateIdentity != null && model.CreatedDateIdentity.Value.Year >= 1980)
                {
                    model.SingleStaff.CreatedDateIdentity = model.CreatedDateIdentity;
                }
                if (model.CreatedDatePassport != null && model.CreatedDatePassport.Value.Year >= 1980)
                {
                    model.SingleStaff.CreatedDatePassport = model.CreatedDatePassport;
                }
                if (model.ExpiredDatePassport != null && model.ExpiredDatePassport.Value.Year >= 1980)
                {
                    model.SingleStaff.ExpiredDatePassport = model.ExpiredDatePassport;
                }

                if (await _staffRepository.Update(model.SingleStaff))
                {
                    UpdateHistory.SaveHistory(6, "Cập nhật nhân viên: " + model.SingleStaff.FullName,
                        null, //appointment
                        null, //contract
                        null, //customer
                        null, //partner
                        null, //program
                        null, //task
                        null, //tour
                        null, //quotation
                        null, //document
                        null, //history
                        null // ticket
                        );

                    // cập nhật lương khởi điểm
                    var salary = await _staffSalaryRepository.GetById(model.SingleStaffSalary.Id);
                    salary.BasicSalary = model.SingleStaffSalary.BasicSalary != null ? model.SingleStaffSalary.BasicSalary : 0;
                    salary.StartSalary = model.SingleStaffSalary.StartSalary != null ? model.SingleStaffSalary.StartSalary : 0;
                    salary.SubsidySalary = model.SingleStaffSalary.SubsidySalary != null ? model.SingleStaffSalary.SubsidySalary : 0;
                    await _staffSalaryRepository.Update(salary);

                    // xóa tất cả visa của staff
                    var visaList = _staffVisaRepository.GetAllAsQueryable().Where(p => p.IsDelete == false).Where(p => p.StaffId == model.SingleStaff.Id).ToList();
                    if (visaList.Count() > 0)
                    {
                        foreach (var v in visaList)
                        {
                            var listId = v.Id.ToString().Split(',').ToArray();
                            await _staffVisaRepository.DeleteMany(listId, false);
                        }
                    }

                    for (int i = 1; i < 6; i++)
                    {
                        if (form["VisaNumber" + i] != null && form["VisaNumber" + i] != "")
                        {
                            var visa = new tbl_StaffVisa
                            {
                                VisaNumber = form["VisaNumber" + i].ToString(),
                                TagsId = Convert.ToInt32(form["TagsId" + i].ToString()),
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now,
                                StaffId = model.SingleStaff.Id,
                                DictionaryId = 1069
                            };
                            if (Convert.ToDateTime(form["CreatedDateVisa" + i]).Year >= 1980)
                            {
                                visa.CreatedDateVisa = Convert.ToDateTime(form["CreatedDateVisa" + i]);
                            }
                            if (Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year >= 1980)
                            {
                                visa.ExpiredDateVisa = Convert.ToDateTime(form["ExpiredDateVisa" + i]);
                            }
                            if (Convert.ToDateTime(form["CreatedDateVisa" + i]).Year >= 1980 && (Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year >= 1980))
                            {
                                int age = Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year - Convert.ToDateTime(form["CreatedDateVisa" + i]).Year;
                                if (Convert.ToDateTime(form["CreatedDateVisa" + i]) > Convert.ToDateTime(form["ExpiredDateVisa" + i]).AddYears(-age)) age--;
                                visa.Deadline = age;
                            }
                            await _staffVisaRepository.Create(visa);
                        }
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch { }

            return RedirectToAction("Index");
        }
        #endregion

        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(FormCollection fc)
        {
            Permission(clsPermission.GetUser().PermissionID, 6);
            try
            {
                if (fc["listItemId"] != null && fc["listItemId"] != "")
                {
                    var listIds = fc["listItemId"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        //
                        foreach (var i in listIds)
                        {
                            var item = _staffRepository.FindId(Convert.ToInt32(i));
                            UpdateHistory.SaveHistory(6, "Xóa nhân viên: " + item.Code + " - " + item.FullName,
                                null, //appointment
                                null, //contract
                                null, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                null // ticket
                                );
                        }
                        //
                        if (await _staffRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "StaffManage") }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Xóa dữ liệu thất bại !" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Vui lòng chọn những mục cần xóa !" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Lock & Unlock
        [HttpPost]
        public ActionResult LockStaff(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 6);
            var item = _db.tbl_Staff.Find(id);
            item.IsLock = true;
            _db.SaveChanges();

            //UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
            //userManager.SetLockoutEnabled(userManager.FindByName(item.Code).Id, false);

            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UnlockStaff(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 6);
            var item = _db.tbl_Staff.Find(id);
            item.IsLock = false;
            _db.SaveChanges();

            //UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
            //userManager.SetLockoutEnabled(userManager.FindByName(item.Code).Id, true);

            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Visa
        /********** Quản lý visa ************/
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateVisa(tbl_StaffVisa model, FormCollection form)
        {
            string id = Session["idStaff"].ToString();

            try
            {
                Permission(clsPermission.GetUser().PermissionID, 61);
                var checkCode = _staffVisaRepository.GetAllAsQueryable().FirstOrDefault(p => p.VisaNumber == model.VisaNumber && p.IsDelete == false);
                if (checkCode == null)
                {
                    model.StaffId = Convert.ToInt32(id);
                    model.CreatedDate = DateTime.Now;
                    model.ModifiedDate = DateTime.Now;
                    if (model.CreatedDateVisa != null)
                    {
                        model.CreatedDateVisa = Convert.ToDateTime(form["CreatedDateVisa"]);
                    }
                    if (model.ExpiredDateVisa != null)
                    {
                        model.ExpiredDateVisa = Convert.ToDateTime(form["ExpiredDateVisa"]);
                    }
                    if (model.ExpiredDateVisa != null && model.CreatedDateVisa != null)
                    {
                        int age = model.ExpiredDateVisa.Value.Year - model.CreatedDateVisa.Value.Year;
                        if (model.CreatedDateVisa > model.ExpiredDateVisa.Value.AddYears(-age)) age--;
                        model.Deadline = age;
                    }
                    if (await _staffVisaRepository.Create(model))
                    {
                        UpdateHistory.SaveHistory(61, "Thêm mới thẻ visa " + model.VisaNumber + " cho nhân viên: " + _staffRepository.FindId(model.StaffId),
                            null, //appointment
                            null, //contract
                            null, //customer
                            null, //partner
                            null, //program
                            null, //task
                            null, //tour
                            null, //quotation
                            null, //document
                            null, //history
                            null // ticket
                            );
                    }
                }
            }
            catch { }

            var list = _db.tbl_StaffVisa.AsEnumerable()
                        .Where(p => p.IsDelete == false).Where(p => p.StaffId.ToString() == id)
                        .OrderByDescending(p => p.CreatedDate).ToList();
            return PartialView("~/Views/StaffTabInfo/_Visa.cshtml", list);
        }

        //[ChildActionOnly]
        //public ActionResult _Partial_EditVisa()
        //{
        //    List<SelectListItem> lstTag = new List<SelectListItem>();
        //    List<SelectListItem> lstDictionary = new List<SelectListItem>();
        //    ViewData["TagsId"] = lstTag;
        //    ViewBag.DictionaryId = lstDictionary;
        //    return PartialView("_Partial_EditVisa", new tbl_StaffVisa());
        //}

        [HttpPost]
        public async Task<ActionResult> EditInfoVisa(int id)
        {
            var model = await _staffVisaRepository.GetById(id);
            List<SelectListItem> lstTag = new List<SelectListItem>();
            foreach (var t in LoadData.DropdownlistCountry().ToList())
            {
                lstTag.Add(new SelectListItem()
                {
                    Text = t.Tags,
                    Value = t.Id.ToString(),
                    Selected = model.TagsId == t.Id ? true : false
                });
            }
            ViewBag.TagsId = lstTag;
            ViewBag.DictionaryId = new SelectList(_dictionaryRepository.GetAllAsQueryable().Where(p => p.DictionaryCategoryId == 14 && p.IsDelete == false), "Id", "Name", model.DictionaryId);
            return PartialView("_Partial_EditVisa", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateVisa(tbl_StaffVisa model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 61);
                model.StaffId = Convert.ToInt32(model.StaffId);
                model.ModifiedDate = DateTime.Now;
                if (model.CreatedDateVisa != null)
                {
                    model.CreatedDateVisa = Convert.ToDateTime(form["CreatedDateVisa"]);
                }
                if (model.ExpiredDateVisa != null)
                {
                    model.ExpiredDateVisa = Convert.ToDateTime(form["ExpiredDateVisa"]);
                }
                if (model.ExpiredDateVisa != null && model.CreatedDateVisa != null)
                {
                    int age = model.ExpiredDateVisa.Value.Year - model.CreatedDateVisa.Value.Year;
                    if (model.CreatedDateVisa > model.ExpiredDateVisa.Value.AddYears(-age)) age--;
                    model.Deadline = age;
                }

                if (await _staffVisaRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(61, "Cập nhật visa cho nhân viên " + _staffRepository.FindId(model.StaffId).FullName,
                        null, //appointment
                        null, //contract
                        null, //customer
                        null, //partner
                        null, //program
                        null, //task
                        null, //tour
                        null, //quotation
                        null, //document
                        null, //history
                        null // ticket
                        );
                    var list = _db.tbl_StaffVisa.AsEnumerable()
                        .Where(p => p.IsDelete == false).Where(p => p.StaffId == model.StaffId)
                        .OrderByDescending(p => p.CreatedDate).ToList();
                    return PartialView("~/Views/StaffTabInfo/_Visa.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/StaffTabInfo/_Visa.cshtml");
                }
            }
            catch { }
            return PartialView("~/Views/StaffTabInfo/_Visa.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteVisa(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 61);
                var sId = _staffVisaRepository.FindId(id).StaffId;
                var listId = id.ToString().Split(',').ToArray();
                //
                var item = _staffVisaRepository.FindId(id);
                UpdateHistory.SaveHistory(61, "Xóa visa của nhân viên, code visa: " + item.VisaNumber,
                    null, //appointment
                        null, //contract
                        null, //customer
                        null, //partner
                        null, //program
                        null, //task
                        null, //tour
                        null, //quotation
                        null, //document
                        null, //history
                        null // ticket
                        );
                //
                if (await _staffVisaRepository.DeleteMany(listId, false))
                {
                    var list = _db.tbl_StaffVisa.AsEnumerable()
                        .Where(p => p.IsDelete == false).Where(p => p.StaffId == sId)
                        .OrderByDescending(p => p.CreatedDate).ToList();
                    return PartialView("~/Views/StaffTabInfo/_Visa.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/StaffTabInfo/_Visa.cshtml");
                }
            }
            catch { }
            return PartialView("~/Views/StaffTabInfo/_Visa.cshtml");
        }
        #endregion

        #region Document
        /********** Quản lý tài liệu ************/

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase FileName)
        {
            if (FileName != null && FileName.ContentLength > 0)
            {
                Session["StaffFile"] = FileName;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateDocument(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 60);
                string id = Session["idStaff"].ToString();
                if (ModelState.IsValid)
                {
                    model.Code = GenerateCode.DocumentCode();
                    model.StaffId = Convert.ToInt32(id);
                    model.CreatedDate = DateTime.Now;
                    model.IsRead = false;
                    model.ModifiedDate = DateTime.Now;
                    model.PermissionStaff = id;
                    if (form["TagsId"] != null && form["TagsId"] != "")
                    {
                        model.TagsId = form["TagsId"].ToString();
                    }
                    model.StaffId = clsPermission.GetUser().StaffID;
                    //file
                    HttpPostedFileBase FileName = Session["StaffFile"] as HttpPostedFileBase;
                    string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                    String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    FileName.SaveAs(path);
                    //end file
                    if (newName != null && FileSize != null)
                    {
                        model.FileName = newName;
                        model.FileSize = FileSize;
                    }

                    if (await _documentFileRepository.Create(model))
                    {
                        Session["StaffFile"] = null;
                        UpdateHistory.SaveHistory(60, "Thêm mới tài liệu, code: " + model.Code,
                            null, //appointment
                            null, //contract
                            null, //customer
                            null, //partner
                            null, //program
                            null, //task
                            null, //tour
                            null, //quotation
                            model.Id, //document
                            null, //history
                            null // ticket
                            );
                        //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.StaffId.ToString() == id).ToList();
                        var list = _db.tbl_DocumentFile.AsEnumerable()
                            .Where(p => p.PermissionStaff != null && p.PermissionStaff.Contains(id.ToString())).Where(p => p.IsDelete == false)
                            .OrderByDescending(p => p.CreatedDate)
                            .Select(p => new tbl_DocumentFile
                            {
                                Id = p.Id,
                                FileName = p.FileName,
                                FileSize = p.FileSize,
                                Note = p.Note,
                                CreatedDate = p.CreatedDate,
                                TagsId = p.TagsId,
                                tbl_Staff = _staffRepository.FindId(p.StaffId)
                            }).ToList();
                        return PartialView("~/Views/StaffTabInfo/_HoSoLienQuan.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/StaffTabInfo/_HoSoLienQuan.cshtml");
                    }
                }
            }
            catch { }
            return PartialView("~/Views/StaffTabInfo/_HoSoLienQuan.cshtml");
        }

        //[ChildActionOnly]
        //public ActionResult _Partial_EditDocument()
        //{
        //    List<SelectListItem> lstTag = new List<SelectListItem>();
        //    List<SelectListItem> lstDictionary = new List<SelectListItem>();
        //    ViewData["TagsId"] = lstTag;
        //    ViewBag.DictionaryId = lstDictionary;
        //    return PartialView("_Partial_EditDocument", new tbl_DocumentFile());
        //}

        [HttpPost]
        public async Task<ActionResult> EditInfoDocument(int id)
        {
            var model = await _documentFileRepository.GetById(id);
            ViewBag.DictionaryId = new SelectList(_dictionaryRepository.GetAllAsQueryable().Where(p => p.DictionaryCategoryId == 1 && p.IsDelete == false), "Id", "Name", model.DictionaryId);
            return PartialView("_Partial_EditDocument", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateDocument(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 60);
                if (ModelState.IsValid)
                {
                    model.IsRead = true;
                    model.ModifiedDate = DateTime.Now;
                    if (form["TagsId"] != null && form["TagsId"] != "")
                    {
                        model.TagsId = form["TagsId"].ToString();
                    }

                    //file
                    if (Session["StaffFile"] != null)
                    {
                        HttpPostedFileBase FileName = Session["StaffFile"] as HttpPostedFileBase;
                        string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                        String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                        String path = Server.MapPath("~/Upload/file/" + newName);
                        FileName.SaveAs(path);

                        //end file

                        if (FileName != null && FileSize != null)
                        {
                            String pathOld = Server.MapPath("~/Upload/file/" + model.FileName);
                            if (System.IO.File.Exists(pathOld))
                                System.IO.File.Delete(pathOld);
                            model.FileName = newName;
                            model.FileSize = FileSize;
                        }
                    }

                    if (await _documentFileRepository.Update(model))
                    {
                        UpdateHistory.SaveHistory(60, "Cập nhật tài liệu của nhân viên: " + model.Code,
                                null, //appointment
                                null, //contract
                                null, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                model.Id, //document
                                null, //history
                                null // ticket
                                );
                        Session["StaffFile"] = null;

                        //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.StaffId == model.StaffId).ToList();
                        var list = _db.tbl_DocumentFile.AsEnumerable()
                            .Where(p => p.PermissionStaff != null && p.PermissionStaff == model.PermissionStaff).Where(p => p.IsDelete == false)
                            .OrderByDescending(p => p.CreatedDate)
                            .Select(p => new tbl_DocumentFile
                            {
                                Id = p.Id,
                                FileName = p.FileName,
                                FileSize = p.FileSize,
                                Note = p.Note,
                                CreatedDate = p.CreatedDate,
                                TagsId = p.TagsId,
                                tbl_Staff = _staffRepository.FindId(p.StaffId)
                            }).ToList();
                        return PartialView("~/Views/StaffTabInfo/_HoSoLienQuan.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/StaffTabInfo/_HoSoLienQuan.cshtml");
                    }
                }
            }
            catch
            {
            }

            return PartialView("~/Views/StaffTabInfo/_HoSoLienQuan.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteDocument(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 60);
                string sId = _documentFileRepository.FindId(id).PermissionStaff;
                //file
                tbl_DocumentFile documentFile = _documentFileRepository.FindId(id) ?? new tbl_DocumentFile();
                String path = Server.MapPath("~/Upload/file/" + documentFile.FileName);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                //end file
                var listId = id.ToString().Split(',').ToArray();
                //
                var item = _documentFileRepository.FindId(id);
                UpdateHistory.SaveHistory(60, "Xóa tài liệu: " + item.Code,
                                null, //appointment
                                null, //contract
                                null, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                id, //document
                                null, //history
                                null // ticket
                                );
                //
                if (await _documentFileRepository.DeleteMany(listId, false))
                {
                    UpdateHistory.SaveHistory(60, "Xóa danh sách tài liệu của nhân viên " + _staffRepository.FindId(item.StaffId).FullName,
                                null, //appointment
                                null, //contract
                                null, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                null // ticket
                                );
                    //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.StaffId == sId).ToList();
                    var list = _db.tbl_DocumentFile.AsEnumerable()
                        .Where(p => p.PermissionStaff == sId && p.IsDelete == false)
                        .OrderByDescending(p => p.CreatedDate)
                        .Select(p => new tbl_DocumentFile
                        {
                            Id = p.Id,
                            FileName = p.FileName,
                            FileSize = p.FileSize,
                            Note = p.Note,
                            CreatedDate = p.CreatedDate,
                            TagsId = p.TagsId,
                            tbl_Staff = _staffRepository.FindId(p.StaffId)
                        }).ToList();
                    return PartialView("~/Views/StaffTabInfo/_HoSoLienQuan.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/StaffTabInfo/_HoSoLienQuan.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/StaffTabInfo/_HoSoLienQuan.cshtml");
            }
        }

        #endregion

        #region Export

        public tbl_Tags Tags(string ids, int type)
        {
            return _tagsRepository.GetAllAsQueryable().AsEnumerable().FirstOrDefault(x => ids != null && ids.Contains(x.Id.ToString()) && x.TypeTag == type);
        }

        /// <summary>
        /// Export file excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportFile()
        {
            var staffs = _staffRepository.GetAllAsQueryable().AsEnumerable()
                    .Where(p => p.IsDelete == false && p.IsVietlike == true)
                    .Select(p => new StaffExportNew
                    {
                        CODE = p.Code,
                        DANHXUNG = p.NameTypeId != null ? p.tbl_DictionaryNameType.Name : "",
                        HOTEN = p.FullName,
                        MST = p.TaxCode,
                        GIOITINH = p.Gender == false ? "Nữ" : "Nam",
                        NGAYSINH = p.Birthday != null ? string.Format("{0:dd/MM/yyyy}", p.Birthday) : "",
                        NOISINH = Tags(p.Birthplace.ToString(), 5) != null ? Tags(p.Birthplace.ToString(), 5).Tag : "",
                        DIACHI = p.Address,
                        PHUONGXA = Tags(p.TagsId, 7) != null ? Tags(p.TagsId, 7).Tag : "",
                        QUANHUYEN = Tags(p.TagsId, 6) != null ? Tags(p.TagsId, 6).Tag : "",
                        TINHTP = Tags(p.TagsId, 5) != null ? Tags(p.TagsId, 5).Tag : "",
                        EMAIL = p.Email,
                        PHONE = p.Phone,
                        PHONGBAN = p.DepartmentId != null ? p.tbl_DictionaryDepartment.Name : "",
                        CHUCVU = p.PositionId != null ? p.tbl_DictionaryPosition.Name : "",
                        CHINHANH = "",
                        CMND = p.IdentityCard,
                        NGAYCAP = p.CreatedDateIdentity != null ? string.Format("{0:dd/MM/yyyy}", p.CreatedDateIdentity) : "",
                        NOICAP = Tags(p.IdentityTagId.ToString(), 3) != null ? Tags(p.IdentityTagId.ToString(), 3).Tag : "",
                        PASSPORT = p.PassportCard,
                        NGAYHIEULUC = p.CreatedDatePassport != null ? string.Format("{0:dd/MM/yyyy}", p.CreatedDatePassport) : "",
                        NGAYHETHAN = p.ExpiredDatePassport != null ? string.Format("{0:dd/MM/yyyy}", p.ExpiredDatePassport) : "",
                        NOICAPPASSPORT = Tags(p.PassportTagId.ToString(), 3) != null ? Tags(p.PassportTagId.ToString(), 3).Tag : "",
                    }).ToList();

            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    ExportCustomersToXlsx(stream, staffs);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "Danh-sach-nhan-vien.xlsx");
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Index");
        }

        public virtual void ExportCustomersToXlsx(Stream stream, IList<StaffExportNew> staffs)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {

                var worksheet = xlPackage.Workbook.Worksheets.Add("Staffs");

                var properties = new[]
                    {
                        "STT",
                        "CODE",
                        "DANH XƯNG",
                        "HỌ TÊN",
                        "MST",
                        "GIỚI TÍNH",
                        "NGÀY SINH",
                        "NƠI SINH",
                        "ĐỊA CHỈ",
                        "TỈNH TP",
                        "QUẬN HUYỆN",
                        "PHƯỜNG XÃ",
                        "EMAIL",
                        "PHONE",
                        "PHÒNG BAN",
                        "CHỨC VỤ",
                        "CHI NHÁNH",
                        "CMND",
                        "NGÀY CẤP",
                        "NƠI CẤP",
                        "PASSPORT",
                        "NGÀY HIỆU LỰC",
                        "NGÀY HẾT HẠN",
                        "NƠI CẤP PASSPORT"
                    };

                worksheet.Cells["A3:X4"].Value = "DANH SÁCH NHÂN VIÊN";
                worksheet.Cells["A3:X4"].Style.Font.SetFromFont(new Font("Tahoma", 16));
                worksheet.Cells["A3:X4"].Style.Font.Bold = true;
                worksheet.Cells["A3:X4"].Merge = true;
                worksheet.Cells["A3:X4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A3:X4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells["A3:X4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A3:X4"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));

                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[6, i + 1].Value = properties[i];
                    worksheet.Cells[6, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[6, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[6, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(191, 191, 191));
                }

                int row = 7, stt = 1;
                foreach (var staff in staffs)
                {
                    int col = 1;

                    worksheet.Cells[row, col].Value = stt;
                    col++;

                    worksheet.Cells[row, col].Value = staff.CODE;
                    col++;

                    worksheet.Cells[row, col].Value = staff.DANHXUNG;
                    col++;

                    worksheet.Cells[row, col].Value = staff.HOTEN;
                    col++;

                    worksheet.Cells[row, col].Value = staff.MST;
                    col++;

                    worksheet.Cells[row, col].Value = staff.GIOITINH;
                    col++;

                    worksheet.Cells[row, col].Value = staff.NGAYSINH;
                    col++;

                    worksheet.Cells[row, col].Value = staff.NOISINH;
                    col++;

                    worksheet.Cells[row, col].Value = staff.DIACHI;
                    col++;

                    worksheet.Cells[row, col].Value = staff.TINHTP;
                    col++;

                    worksheet.Cells[row, col].Value = staff.QUANHUYEN;
                    col++;

                    worksheet.Cells[row, col].Value = staff.PHUONGXA;
                    col++;

                    worksheet.Cells[row, col].Value = staff.EMAIL;
                    col++;

                    worksheet.Cells[row, col].Value = staff.PHONE;
                    col++;

                    worksheet.Cells[row, col].Value = staff.PHONGBAN;
                    col++;

                    worksheet.Cells[row, col].Value = staff.CHUCVU;
                    col++;

                    worksheet.Cells[row, col].Value = staff.CHINHANH;
                    col++;

                    worksheet.Cells[row, col].Value = staff.CMND;
                    col++;

                    worksheet.Cells[row, col].Value = staff.NGAYCAP;
                    col++;

                    worksheet.Cells[row, col].Value = staff.NOICAP;
                    col++;

                    worksheet.Cells[row, col].Value = staff.PASSPORT;
                    col++;

                    worksheet.Cells[row, col].Value = staff.NGAYHIEULUC;
                    col++;

                    worksheet.Cells[row, col].Value = staff.NGAYHETHAN;
                    col++;

                    worksheet.Cells[row, col].Value = staff.NOICAPPASSPORT;
                    col++;

                    stt++;
                    row++;
                }
                row--;
                worksheet.Cells["A6:X" + row].Style.Font.SetFromFont(new Font("Tahoma", 8));
                worksheet.Cells["A6:X" + row].AutoFitColumns();
                worksheet.Cells["A6:X6"].Style.Font.Bold = true;
                xlPackage.Save();
            }
        }
        #endregion

        #region Import

        [HttpPost]
        public ActionResult ImportFile(HttpPostedFileBase FileName)
        {
            Permission(clsPermission.GetUser().PermissionID, 6);
            try
            {

                using (var excelPackage = new ExcelPackage(FileName.InputStream))
                {
                    List<tbl_Staff> list = new List<tbl_Staff>();
                    var worksheet = excelPackage.Workbook.Worksheets[1];
                    var lastRow = worksheet.Dimension.End.Row;
                    for (int row = 7; row <= lastRow; row++)
                    {
                        if (worksheet.Cells["c" + row].Value == null || worksheet.Cells["c" + row].Text == "")
                            continue;
                        var stf = new tbl_Staff
                        {
                            FullName = worksheet.Cells["C" + row].Text,
                            TaxCode = worksheet.Cells["D" + row].Value != null ? worksheet.Cells["d" + row].Text : null,
                            Address = worksheet.Cells["H" + row].Value != null ? worksheet.Cells["h" + row].Text : null,
                            Email = worksheet.Cells["L" + row].Value != null ? worksheet.Cells["l" + row].Text : null,
                            Phone = worksheet.Cells["M" + row].Value != null ? worksheet.Cells["m" + row].Text : null,
                            IdentityCard = worksheet.Cells["Q" + row].Value != null ? worksheet.Cells["q" + row].Text : null,
                            PassportCard = worksheet.Cells["T" + row].Value != null ? worksheet.Cells["t" + row].Text : null,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            StaffId = clsPermission.GetUser().StaffID,
                            IsVietlike = true
                        };
                        String cel = "B";
                        // danh xưng
                        try
                        {
                            cel = "B";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string danhxung = worksheet.Cells[cel + row].Text;
                                stf.NameTypeId = _dictionaryRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Name == danhxung && c.DictionaryCategoryId == 7).Select(c => c.Id).SingleOrDefault();
                            }
                        }
                        catch { }
                        // giới tính
                        try
                        {
                            cel = "E";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string gioitinh = worksheet.Cells[cel + row].Text;
                                stf.Gender = gioitinh == "Nam" ? true : false;
                            }
                        }
                        catch { }
                        //ngày sinh
                        try
                        {
                            cel = "F";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                stf.Birthday = DateTime.ParseExact(worksheet.Cells[cel + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                            }
                        }
                        catch { }
                        // nơi sinh
                        try
                        {
                            cel = "G";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string noisinh = worksheet.Cells[cel + row].Text;
                                stf.Birthplace = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == noisinh && c.TypeTag == 5).Select(c => c.Id).SingleOrDefault();
                            }
                        }
                        catch { }
                        // tỉnh huyện xã
                        try
                        {
                            cel = "I";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string tinhtp = worksheet.Cells[cel + row].Text;
                                stf.TagsId = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == tinhtp && c.TypeTag == 5).Select(c => c.Id).SingleOrDefault().ToString();
                            }
                            cel = "J";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string quanhuyen = worksheet.Cells[cel + row].Text;
                                var tagid = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == quanhuyen && c.TypeTag == 6).SingleOrDefault();
                                if (tagid != null)
                                    if (stf.TagsId != null)
                                        stf.TagsId += "," + tagid.Id;
                                    else
                                        stf.TagsId = tagid.Id.ToString();
                            }
                            cel = "K";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string phuongxa = worksheet.Cells[cel + row].Text;
                                var tagid = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == phuongxa && c.TypeTag == 7).SingleOrDefault();
                                if (tagid != null)
                                    if (stf.TagsId != null)
                                        stf.TagsId += "," + tagid.Id;
                                    else
                                        stf.TagsId = tagid.Id.ToString();
                            }
                        }
                        catch { }
                        // phòng ban
                        try
                        {
                            cel = "N";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string phongban = worksheet.Cells[cel + row].Text;
                                stf.DepartmentId = _dictionaryRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Name == phongban && c.DictionaryCategoryId == 6).Select(c => c.Id).SingleOrDefault();
                            }
                        }
                        catch { }
                        // chức vụ
                        try
                        {
                            cel = "O";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string chucvu = worksheet.Cells[cel + row].Text;
                                stf.PositionId = _dictionaryRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Name == chucvu && c.DictionaryCategoryId == 5).Select(c => c.Id).SingleOrDefault();
                            }
                        }
                        catch { }
                        // chi nhánh
                        try
                        {
                            cel = "P";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string chinhanh = worksheet.Cells[cel + row].Text;
                                stf.HeadquarterId = _headquaterRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.HeadquarterName == chinhanh).Select(c => c.Id).SingleOrDefault();
                            }
                        }
                        catch { }
                        // ngày cấp CMND
                        try
                        {
                            cel = "R";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                stf.CreatedDateIdentity = DateTime.ParseExact(worksheet.Cells[cel + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                            }
                        }
                        catch { }
                        // nơi cấp CMND
                        try
                        {
                            cel = "S";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string noicap = worksheet.Cells[cel + row].Text;
                                stf.IdentityTagId = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == noicap && c.TypeTag == 3).Select(c => c.Id).SingleOrDefault();
                            }
                        }
                        catch { }
                        // ngày hiệu lực passport
                        try
                        {
                            cel = "U";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                stf.CreatedDatePassport = DateTime.ParseExact(worksheet.Cells[cel + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                            }
                        }
                        catch { }
                        // ngày hết hạn passport
                        try
                        {
                            cel = "V";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                stf.ExpiredDatePassport = DateTime.ParseExact(worksheet.Cells[cel + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                            }
                        }
                        catch { }
                        // nơi cấp passport
                        try
                        {
                            cel = "W";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string noicap = worksheet.Cells[cel + row].Text;
                                stf.PassportTagId = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == noicap && c.TypeTag == 3).Select(c => c.Id).SingleOrDefault();
                            }
                        }
                        catch { }
                        list.Add(stf);
                    }
                    Session["listStaffImport"] = list;
                    return PartialView("_Partial_ImportDataList", list);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveImport(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 6);
            try
            {
                List<tbl_Staff> list = Session["listStaffImport"] as List<tbl_Staff>;
                int i = 0;
                if (id == 1) //cap nhat
                {
                    foreach (var item in list)
                    {
                        var check = _db.tbl_Staff.FirstOrDefault(p => p.Code == item.Code || p.FullName == item.FullName);
                        if (check != null)
                        {
                            // đã có nhân viên này --> cập nhật
                            item.Id = check.Id;
                            item.IsDelete = false;
                            item.CreatedDate = check.CreatedDate;
                            item.ModifiedDate = DateTime.Now;
                            await _staffRepository.Update(item);
                        }
                    }
                }
                else //them moi
                {
                    foreach (var item in list)
                    {
                        item.Code = GenerateCode.StaffCode();
                        bool temp = false;

                        var namebirthday = _staffRepository.GetAllAsQueryable().AsEnumerable()
                            .FirstOrDefault(c => c.FullName == item.FullName && c.Birthday == item.Birthday);
                        if (namebirthday != null)
                            temp = true;

                        var cmnd = _staffRepository.GetAllAsQueryable().AsEnumerable()
                            .FirstOrDefault(c => c.IdentityCard == item.IdentityCard);
                        if (cmnd != null)
                            temp = true;

                        if (!temp)
                        {
                            try
                            {
                                await _staffRepository.Create(item);
                                i++;
                            }
                            catch { }
                        }
                    }
                }
                Session["listStaffImport"] = null;
                if (i != 0)
                    return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Đã import thành công " + i + " dòng dữ liệu !", IsPartialView = false, RedirectTo = Url.Action("Index", "StaffManage") }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Chưa có dữ liệu nào được import !" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                Session["listStaffImport"] = null;
                return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Import dữ liệu lỗi !" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteImport(String listItemId)
        {
            Permission(clsPermission.GetUser().PermissionID, 6);
            try
            {
                List<tbl_Staff> list = Session["listStaffImport"] as List<tbl_Staff>;
                if (listItemId != null && listItemId != "")
                {
                    var listIds = listItemId.Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        int[] listIdsint = new int[listIds.Length];
                        for (int i = 0; i < listIds.Length; i++)
                        {
                            listIdsint[i] = Int32.Parse(listIds[i]);
                        }
                        for (int i = 0; i < listIdsint.Length; i++)
                        {
                            for (int j = i; j < listIdsint.Length; j++)
                            {
                                if (listIdsint[i] < listIdsint[j])
                                {
                                    int temp = listIdsint[i];
                                    listIdsint[i] = listIdsint[j];
                                    listIdsint[j] = temp;
                                }
                            }
                        }
                        foreach (var item in listIdsint)
                        {
                            list.RemoveAt(item);
                        }
                    }
                }
                Session["listStaffImport"] = list;
                return PartialView("_Partial_ImportDataList", list);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Task
        [ValidateInput(false)]
        public async Task<ActionResult> CreateTaskStaff(tbl_Task model, FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 57);
            int idStaff = Convert.ToInt32(Session["idStaff"].ToString());
            try
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.TaskStatusId = 1193;
                model.IsNotify = model.NotifyDate != null ? true : false;
                model.Permission = idStaff.ToString();
                if (model.TourId != null)
                {
                    model.CodeTour = _tourRepository.FindId(model.TourId).Code;
                }
                model.IsNotify = false;
                model.StaffId = clsPermission.GetUser().StaffID;
                if (await _taskRepository.Create(model))
                {
                    UpdateHistory.SaveHistory(57, "Phân công nhiệm vụ " + model.Name,
                        null, //appointment
                        null, //contract
                        model.CustomerId, //customer
                        null, //partner
                        null, //program
                        model.Id, //task
                        model.TourId, //tour
                        null, //quotation
                        null, //document
                        null, //history
                        null // ticket
                        );
                }

                // tạo Task Staff
                var stt = new tbl_TaskStaff
                {
                    CreateDate = DateTime.Now,
                    CreateStaffId = model.StaffId,
                    IsDelete = false,
                    IsUse = true,
                    Note = model.Note,
                    Role = model.Name,
                    StaffId = Convert.ToInt32(model.Permission),
                    TaskId = model.Id
                };
                await _taskStaffRepository.Create(stt);
            }
            catch { }
            var list = _taskRepository.GetAllAsQueryable().AsEnumerable()
                        .Where(p => (p.StaffId == idStaff || p.Permission.Contains(idStaff.ToString())) && p.IsDelete == false)
                        .OrderByDescending(p => p.CreatedDate)
                        .Select(p => new tbl_Task
                        {
                            Id = p.Id,
                            tbl_DictionaryTaskType = _dictionaryRepository.FindId(p.TaskTypeId),
                            tbl_DictionaryTaskStatus = _dictionaryRepository.FindId(p.TaskStatusId),
                            Name = p.Name,
                            Permission = p.Permission,
                            StartDate = p.StartDate,
                            EndDate = p.EndDate,
                            Time = p.Time,
                            TimeType = p.TimeType,
                            FinishDate = p.FinishDate,
                            PercentFinish = p.PercentFinish,
                            Note = p.Note,
                            StaffId = p.StaffId,
                            tbl_Staff = _staffRepository.FindId(p.StaffId)
                        }).ToList();
            return PartialView("~/Views/StaffTabInfo/_NhiemVu.cshtml", list);
        }
        #endregion

        #region check code staff
        [HttpPost]
        public ActionResult CheckCode(string text)
        {
            var cus = _staffRepository.GetAllAsQueryable().AsEnumerable().FirstOrDefault(p => p.Code == text);
            if (cus != null)
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Check Visa
        // trùng Visa
        [HttpPost]
        public ActionResult CheckVisa(string text)
        {
            var cus = _staffVisaRepository.GetAllAsQueryable().AsEnumerable().FirstOrDefault(p => p.VisaNumber == text);
            if (cus != null)
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Excel Sample

        public void ExportExcelTemplateCustomer(MemoryStream stream, string templateFile, IDictionary<string, string> header = null)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream, new MemoryStream(System.IO.File.ReadAllBytes(templateFile))))
            {
                var ws = xlPackage.Workbook.Worksheets[1]; //first worksheet
                var valWs = xlPackage.Workbook.Worksheets.Add("Validation");
                valWs.Hidden = eWorkSheetHidden.VeryHidden;

                //ws.AddHeader(header);

                // danh xung
                var danhxung = LoadData.NameTypeList()
                    .Select(p => new ExportItem
                    {
                        Text = p.Name,
                        Value = p.Id
                    });
                var columnIndex = ws.GetColumnIndex(StaffColumn.DANHXUNG.ToString());
                ws.AddListValidation(valWs, danhxung, columnIndex, "Lỗi", "Lỗi", "DANHXUNG", "DANHXUNGName");

                // giới tính
                var gioitinh = new List<ExportItem>();
                gioitinh.Add(new ExportItem { Text = "Nam", Value = 1 });
                gioitinh.Add(new ExportItem { Text = "Nữ", Value = 0 });
                columnIndex = ws.GetColumnIndex(StaffColumn.GIOITINH.ToString());
                ws.AddListValidation(valWs, gioitinh, columnIndex, "Lỗi", "Lỗi", "GIOITINH", "GIOITINHName");

                // nơi sinh
                var noisinh = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false && p.TypeTag == 5)
                    .Select(p => new ExportItem
                    {
                        Text = p.Tag,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(StaffColumn.NOISINH.ToString());
                ws.AddListValidation(valWs, noisinh, columnIndex, "Lỗi", "Lỗi", "NOISINH", "NOISINHName");

                // tinh tp
                var tinhthanh = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false && p.TypeTag == 5)
                    .Select(p => new ExportItem
                    {
                        Text = p.Tag,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(StaffColumn.TINHTP.ToString());
                ws.AddListValidation(valWs, tinhthanh, columnIndex, "Lỗi", "Lỗi", "TINHTP", "TINHTPName");

                // quan huyen
                var quanhuyen = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false && p.TypeTag == 6)
                    .Select(p => new ExportItem
                    {
                        Text = p.Tag,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(StaffColumn.QUANHUYEN.ToString());
                ws.AddListValidation(valWs, quanhuyen, columnIndex, "Lỗi", "Lỗi", "QUANHUYEN", "QUANHUYENName");

                // phuong xa
                var phuongxa = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false && p.TypeTag == 7)
                    .Select(p => new ExportItem
                    {
                        Text = p.Tag,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(StaffColumn.PHUONGXA.ToString());
                ws.AddListValidation(valWs, phuongxa, columnIndex, "Lỗi", "Lỗi", "PHUONGXA", "PHUONGXAName");

                // phòng ban
                var phongban = LoadData.DepartmentList()
                    .Select(p => new ExportItem
                    {
                        Text = p.Name,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(StaffColumn.PHONGBAN.ToString());
                ws.AddListValidation(valWs, phongban, columnIndex, "Lỗi", "Lỗi", "PHONGBAN", "PHONGBANName");


                // chức vụ
                var chucvu = LoadData.PositionList()
                    .Select(p => new ExportItem
                    {
                        Text = p.Name,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(StaffColumn.CHUCVU.ToString());
                ws.AddListValidation(valWs, chucvu, columnIndex, "Lỗi", "Lỗi", "CHUCVU", "CHUCVUName");

                // chi nhánh
                var chinhanh = LoadData.HeadquarterList()
                    .Select(p => new ExportItem
                    {
                        Text = p.HeadquarterName,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(StaffColumn.CHINHANH.ToString());
                ws.AddListValidation(valWs, chinhanh, columnIndex, "Lỗi", "Lỗi", "CHINHANH", "CHINHANHName");

                // nơi cấp
                var noicap = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false && p.TypeTag == 5)
                    .Select(p => new ExportItem
                    {
                        Text = p.Tag,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(StaffColumn.NOICAP.ToString());
                ws.AddListValidation(valWs, noicap, columnIndex, "Lỗi", "Lỗi", "NOICAP", "NOICAPName");

                // nơi cấp passport
                var noicappassport = LoadData.DropdownlistCountry()
                    .Select(p => new ExportItem
                    {
                        Text = p.Tags,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(StaffColumn.NOICAPPASSPORT.ToString());
                ws.AddListValidation(valWs, noicappassport, columnIndex, "Lỗi", "Lỗi", "NOICAPPASSPORT", "NOICAPPASSPORTName");

                xlPackage.Save();
            }
        }

        public ActionResult ExcelSample()
        {
            try
            {
                IDictionary<string, string> header = new Dictionary<string, string>();
                header.Add("TITLE", "DANH SÁCH NHÂN VIÊN");

                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    string templateFile = System.Web.HttpContext.Current.Server.MapPath("~\\Upload\\ImportExport\\Import_NhanVienTOURDEMO.xlsx");
                    ExportExcelTemplateCustomer(stream, templateFile, header);
                    bytes = stream.ToArray();
                }

                string fileName = "Mau-import-nhan-vien-TOURDEMO.xlsx";
                return File(bytes, "text/xls", fileName);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        #endregion
    }

}
