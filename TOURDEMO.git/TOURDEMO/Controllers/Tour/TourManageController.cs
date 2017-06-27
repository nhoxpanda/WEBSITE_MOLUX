using CRM.Core;
using TOURDEMO.Utilities;
using CRM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TOURDEMO.Models;
using System.Threading.Tasks;
using System.Globalization;
using CRM.Enum;
using OfficeOpenXml;
using System.IO;

namespace TOURDEMO.Controllers
{
    [Authorize]
    public class TourManageController : BaseController
    {
        //
        // GET: /TourManage/

        #region Init

        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_ServicesPartner> _servicesPartnerRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;
        private IGenericRepository<tbl_ReviewTour> _reviewTourRepository;
        private IGenericRepository<tbl_ReviewTourDetail> _reviewTourDetailRepository;
        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_CustomerVisa> _customerVisaRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;
        private IGenericRepository<tbl_Task> _taskRepository;
        private IGenericRepository<tbl_TaskStaff> _taskStaffRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_AppointmentHistory> _appointmentHistoryRepository;
        private IGenericRepository<tbl_ContactHistory> _contactHistoryRepository;
        private IGenericRepository<tbl_Contract> _contractRepository;
        private IGenericRepository<tbl_Partner> _partnerRepository;
        private IGenericRepository<tbl_TourGuide> _tourGuideRepository;
        private IGenericRepository<tbl_TourSchedule> _tourScheduleRepository;
        private IGenericRepository<tbl_TourCustomer> _tourCustomerRepository;
        private IGenericRepository<tbl_TourCustomerVisa> _tourCustomerVisaRepository;
        private IGenericRepository<tbl_TourOption> _tourOptionRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_LiabilityCustomer> _liabilityCustomerRepository;
        private IGenericRepository<tbl_LiabilityPartner> _liabilityPartnerRepository;
        private IGenericRepository<tbl_CustomerContact> _customerContactRepository;
        private IGenericRepository<tbl_CustomerContactVisa> _customerContactVisaRepository;
        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
        private DataContext _db;

        public TourManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_TaskStaff> taskStaffRepository,
            IGenericRepository<tbl_ServicesPartner> servicesPartnerRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IGenericRepository<tbl_ReviewTour> reviewTourRepository,
            IGenericRepository<tbl_ReviewTourDetail> reviewTourDetailRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_CustomerVisa> customerVisaRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IGenericRepository<tbl_Task> taskRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_AppointmentHistory> appointmentHistoryRepository,
            IGenericRepository<tbl_ContactHistory> contactHistoryRepository,
            IGenericRepository<tbl_Contract> contractRepository,
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_TourGuide> tourGuideRepository,
            IGenericRepository<tbl_TourSchedule> tourScheduleRepository,
            IGenericRepository<tbl_TourCustomer> tourCustomerRepository,
            IGenericRepository<tbl_TourCustomerVisa> tourCustomerVisaRepository,
            IGenericRepository<tbl_TourOption> tourOptionRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_LiabilityCustomer> liabilityCustomerRepository,
            IGenericRepository<tbl_LiabilityPartner> liabilityPartnerRepository,
            IGenericRepository<tbl_CustomerContact> customerContactRepository,
            IGenericRepository<tbl_CustomerContactVisa> customerContactVisaRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._dictionaryRepository = dictionaryRepository;
            this._servicesPartnerRepository = servicesPartnerRepository;
            this._tourRepository = tourRepository;
            this._reviewTourRepository = reviewTourRepository;
            this._reviewTourDetailRepository = reviewTourDetailRepository;
            this._customerRepository = customerRepository;
            this._customerVisaRepository = customerVisaRepository;
            this._tagsRepository = tagsRepository;
            this._taskRepository = taskRepository;
            this._documentFileRepository = documentFileRepository;
            this._appointmentHistoryRepository = appointmentHistoryRepository;
            this._contactHistoryRepository = contactHistoryRepository;
            this._contractRepository = contractRepository;
            this._partnerRepository = partnerRepository;
            this._tourGuideRepository = tourGuideRepository;
            this._tourScheduleRepository = tourScheduleRepository;
            this._tourCustomerRepository = tourCustomerRepository;
            this._tourCustomerVisaRepository = tourCustomerVisaRepository;
            this._tourOptionRepository = tourOptionRepository;
            this._staffRepository = staffRepository;
            this._liabilityCustomerRepository = liabilityCustomerRepository;
            this._liabilityPartnerRepository = liabilityPartnerRepository;
            this._taskStaffRepository = taskStaffRepository;
            this._customerContactRepository = customerContactRepository;
            this._customerContactVisaRepository = customerContactVisaRepository;
            this._updateHistoryRepository = updateHistoryRepository;
            _db = new DataContext();
        }

        #endregion

        #region List

        [HttpPost]
        public ActionResult GetIdTour(int id)
        {
            Session["idTour"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            // phân quyền
            Permission(clsPermission.GetUser().PermissionID, 24);
            return View();
        }

        int SDBID = 6;
        void Permission(int PermissionsId, int formId)
        {
            var list = _db.tbl_ActionData.Where(p => p.FormId == formId & p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsEdit = list.Contains(3);
            ViewBag.IsDelete = list.Contains(2);

            //cập nhật trạng thái
            var listUS = _db.tbl_ActionData.Where(p => p.FormId == 24 & p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsUpdateStatus = list.Contains(1);

            //phân công nhiệm vụ
            var listNV = _db.tbl_ActionData.Where(p => p.FormId == 78 & p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAddNV = list.Contains(1);

            //tạo lịch đi tour
            var listLDT = _db.tbl_ActionData.Where(p => p.FormId == 91 & p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAddLDT = listLDT.Contains(1);

            var ltAccess = _db.tbl_AccessData.Where(p => p.PermissionId == PermissionsId & p.FormId == formId).Select(p => p.ShowDataById).FirstOrDefault();
            if (ltAccess != 0)
                this.SDBID = ltAccess;
        }

        public ActionResult _Partial_ListTours()
        {
            Permission(clsPermission.GetUser().PermissionID, 24);
            if (SDBID == 6)
                return PartialView("_Partial_ListTours", new List<TourListViewModel>());

            try
            {
                int maPB = 0, maNKD = 0, maNV = 0, maCN = 0;
                switch (SDBID)
                {
                    case 2: maPB = clsPermission.GetUser().DepartmentID;
                        maCN = clsPermission.GetUser().BranchID;
                        break;
                    case 3: maNKD = clsPermission.GetUser().GroupID;
                        maCN = clsPermission.GetUser().BranchID; break;
                    case 4: maNV = clsPermission.GetUser().StaffID; break;
                    case 5: maCN = clsPermission.GetUser().BranchID; break;
                }

                var model = _tourRepository.GetAllAsQueryable().Where(p =>
                    ((p.tbl_TourGuide.FirstOrDefault(g => g.StaffId == maNV) != null)
                    || (p.Permission.Contains(maNV.ToString()))
                    || (p.StaffId == maNV | maNV == 0))
                    || (p.CreateStaffId == maNV | maNV == 0)
                    & (p.tbl_StaffCreate.DepartmentId == maPB | maPB == 0)
                    & (p.tbl_StaffCreate.StaffGroupId == maNKD | maNKD == 0)
                    & (p.tbl_StaffCreate.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false))
                    .OrderByDescending(p => p.CreatedDate)
                    .Select(p => new TourListViewModel
                    {
                        Id = p.Id,
                        Code = p.Code,
                        Name = p.Name,
                        CustomerName = p.tbl_Customer.FullName,
                        NumberCustomer = p.NumberCustomer ?? 0,
                        DestinationPlace = p.tbl_TagsDestinationPlace.Tag,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        NumberDay = p.NumberDay ?? 0,
                        TourGuide = p.tbl_TourGuide.FirstOrDefault() == null ? "" : p.tbl_TourGuide.FirstOrDefault().tbl_Staff.FullName,
                        TourType = p.tbl_DictionaryTypeTour.Name,
                        Staff = p.tbl_StaffCreate.FullName,
                        Manager = p.tbl_Staff.FullName,
                        Permission = p.Permission,
                        Status = p.StatusId != null ? p.tbl_DictionaryStatus.Name : "",
                        Color = p.StatusId != null ? p.tbl_DictionaryStatus.Note : ""
                    }).ToList();

                foreach (var item in model)
                {
                    item.CongNoDoiTac = _liabilityPartnerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id && c.IsDelete == false).Sum(c => c.TotalRemaining) ?? 0;
                    item.CongNoKhachHang = _liabilityCustomerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id && c.IsDelete == false).Sum(c => c.TotalRemaining) ?? 0;
                }
                return PartialView("_Partial_ListTours", model);
            }
            catch
            { }

            return PartialView("_Partial_ListTours");
        }

        #endregion

        #region Filter Type
        public ActionResult FilterTour(int? id, int? sltu, int? slden, DateTime? tungay, DateTime? denngay, int codinh)
        {
            Permission(clsPermission.GetUser().PermissionID, 24);
            if (SDBID == 6)
                return PartialView("_Partial_ListTours", new List<TourListViewModel>());

            int maPB = 0, maNKD = 0, maNV = 0, maCN = 0;
            switch (SDBID)
            {
                case 2: maPB = clsPermission.GetUser().DepartmentID;
                    maCN = clsPermission.GetUser().BranchID;
                    break;
                case 3: maNKD = clsPermission.GetUser().GroupID;
                    maCN = clsPermission.GetUser().BranchID; break;
                case 4: maNV = clsPermission.GetUser().StaffID; break;
                case 5: maCN = clsPermission.GetUser().BranchID; break;
            }

            var model = _tourRepository.GetAllAsQueryable().Where(p =>
                    (((p.tbl_TourGuide.FirstOrDefault(g => g.StaffId == maNV) != null)
                    || (p.Permission.Contains(maNV.ToString()))
                    || (p.StaffId == maNV | maNV == 0))
                    || (p.CreateStaffId == maNV | maNV == 0))
                    && (p.TypeTourId == (id == 0 ? p.TypeTourId : id))
                    && ((sltu != null ? sltu <= p.NumberCustomer || p.NumberDay == null : p.Id != 0)
                    && (slden != null ? slden >= p.NumberCustomer || p.NumberDay == null : p.Id != 0))
                    && (tungay <= p.StartDate && p.StartDate <= denngay || p.StartDate == null)
                    && (codinh == 1 ? p.IsFixed == true :  p.IsFixed == false)
                    & (p.tbl_StaffCreate.DepartmentId == maPB | maPB == 0)
                    & (p.tbl_StaffCreate.StaffGroupId == maNKD | maNKD == 0)
                    & (p.tbl_StaffCreate.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false))
                    .OrderByDescending(p => p.CreatedDate)
                    .Select(p => new TourListViewModel
                    {
                        Id = p.Id,
                        Code = p.Code,
                        Name = p.Name,
                        CustomerName = p.tbl_Customer.FullName,
                        NumberCustomer = p.NumberCustomer ?? 0,
                        DestinationPlace = p.tbl_TagsDestinationPlace.Tag,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        NumberDay = p.NumberDay ?? 0,
                        TourGuide = p.tbl_TourGuide.FirstOrDefault() == null ? "" : p.tbl_TourGuide.FirstOrDefault().tbl_Staff.FullName,
                        TourType = p.tbl_DictionaryTypeTour.Name,
                        Staff = p.tbl_StaffCreate.FullName,
                        Manager = p.tbl_Staff.FullName,
                        Permission = p.Permission,
                        Status = p.StatusId != null ? p.tbl_DictionaryStatus.Name : "",
                        Color = p.StatusId != null ? p.tbl_DictionaryStatus.Note : ""
                    }).ToList();

            foreach (var item in model)
            {
                item.CongNoDoiTac = _liabilityPartnerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id && c.IsDelete == false).Sum(c => c.TotalRemaining) ?? 0;
                item.CongNoKhachHang = _liabilityCustomerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id && c.IsDelete == false).Sum(c => c.TotalRemaining) ?? 0;
            }
            return PartialView("_Partial_ListTours", model);

        }
        #endregion

        #region Create
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(TourViewModel model, FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 24);
            try
            {
                var _price = form["SingleTour.Price"].ToString();
                model.SingleTour.CreatedDate = DateTime.Now;
                model.SingleTour.ModifiedDate = DateTime.Now;
                model.SingleTour.Price = decimal.Parse(_price.Replace(",", ""));
                model.SingleTour.Permission = form["SingleTour.Permission"] != null && form["SingleTour.Permission"] != "" ? form["SingleTour.Permission"].ToString() : null;
                if (model.StartDateTour != null && model.StartDateTour.Value.Year >= 1980)
                {
                    model.SingleTour.StartDate = model.StartDateTour;
                }
                if (model.EndDateTour != null && model.EndDateTour.Value.Year >= 1980)
                {
                    model.SingleTour.EndDate = model.EndDateTour;
                }
                model.SingleTour.CreateStaffId = clsPermission.GetUser().StaffID;
                if (await _tourRepository.Create(model.SingleTour))
                {
                    UpdateHistory.SaveHistory(24, "Thêm mới tour, code: " + model.SingleTour.Code + " - " + model.SingleTour.Name,
                        null, //appointment
                        null, //contract
                        model.SingleTour.CustomerId, //customer
                        null, //partner
                        null, //program
                        null, //task
                        model.SingleTour.Id, //tour
                        null, //quotation
                        null, //document
                        null, //history
                        null // ticket
                        );
                    // lưu TourCustomer
                    var tc = new tbl_TourCustomer
                    {
                        IsDelete = false,
                        TourId = model.SingleTour.Id
                    };
                    if (model.SingleTour.CustomerId != null)
                    {
                        tc.CustomerId = model.SingleTour.CustomerId ?? 0;
                    }
                    await _tourCustomerRepository.Create(tc);
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region Update

        public async Task<ActionResult> TourInfomation(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 24);
            var singleTour = await _tourRepository.GetById(id);
           
            var singleTourGuide = _tourGuideRepository.GetAllAsQueryable().FirstOrDefault(p => p.TourId == id);
            var stringPrice = decimal.ToInt32(singleTour.Price??0);
            var model = new TourViewModel
            {
                PriceString = stringPrice.ToString(),
                EndDateTour = singleTour.EndDate,
                SingleTour = singleTour,
                StartDateTour = singleTour.StartDate,
                SingleTourGuide = singleTourGuide,
                StartDateTourGuide = singleTourGuide != null ? singleTourGuide.StartDate : null,
                EndDateTourGuide = singleTourGuide != null ? singleTourGuide.EndDate : null
            };
            ViewBag.ListTourGuide = _tourGuideRepository.GetAllAsQueryable().Where(p => p.TourId == id).ToList();
            if (clsPermission.GetUser().StaffID == 9 || model.SingleTour.StatusId <= 3296)
            {
                return PartialView("_Partial_EditTour", model);
            }
            else
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(TourViewModel model, FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 24);
            try
            {
                int oldCustomer = _db.tbl_Tour.Find(model.SingleTour.Id).CustomerId ?? 0;
                var _price = form["PriceString"].ToString().Replace(",", "");
                model.SingleTour.Price = decimal.Parse(_price);
                model.SingleTour.ModifiedDate = DateTime.Now;
                model.SingleTour.Permission = form["SingleTour.Permission"] != null && form["SingleTour.Permission"] != "" ? form["SingleTour.Permission"].ToString() : null;
                if (model.StartDateTour != null && model.StartDateTour.Value.Year >= 1980)
                {
                    model.SingleTour.StartDate = model.StartDateTour;
                }
                if (model.EndDateTour != null && model.EndDateTour.Value.Year >= 1980)
                {
                    model.SingleTour.EndDate = model.EndDateTour;
                }
                model.SingleTour.CreateStaffId = clsPermission.GetUser().StaffID;

                // xóa tất cả lịch tour
                var lich = _tourScheduleRepository.GetAllAsQueryable().Where(p => p.TourId == model.SingleTour.Id).ToList();
                if (lich.Count() > 0)
                {
                    foreach (var c in lich)
                    {
                        var listId = c.Id.ToString().Split(',').ToArray();
                        await _tourScheduleRepository.DeleteMany(listId, true);
                    }
                }
                // xóa tất cả hướng dẫn viên đã có
                var items = _tourGuideRepository.GetAllAsQueryable().Where(p => p.TourId == model.SingleTour.Id).ToList();
                if (items.Count() > 0)
                {
                    foreach (var i in items)
                    {
                        await _tourGuideRepository.DeleteMany(i.Id.ToString().Split(',').ToArray(), true);
                    }
                }
                if (await _tourRepository.Update(model.SingleTour))
                {
                    if (oldCustomer == model.SingleTour.CustomerId)
                    {
                        UpdateHistory.UpdateCustomerTour(model.SingleTour.CustomerId ?? 0, model.SingleTour.Id);
                    }

                    UpdateHistory.SaveHistory(24, "Cập nhật tour: " + model.SingleTour.Name,
                        null, //appointment
                        null, //contract
                        model.SingleTour.CustomerId, //customer
                        null, //partner
                        null, //program
                        null, //task
                        model.SingleTour.Id, //tour
                        null, //quotation
                        null, //document
                        null, //history
                        null // ticket
                        );
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        #endregion

        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(FormCollection fc)
        {
            Permission(clsPermission.GetUser().PermissionID, 24);
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
                            var item = _tourRepository.FindId(Convert.ToInt32(i));
                            UpdateHistory.SaveHistory(24, "Xóa tour, code: " + item.Code + " - " + item.Name,
                                null, //appointment
                                null, //contract
                                item.CustomerId, //customer
                                null, //partner
                                null, //program
                                null, //task
                                item.Id, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                null // ticket
                                );
                        }
                        //
                        if (await _tourRepository.DeleteMany(listIds, false))
                        {
                            for (int i = 0; i < listIds.Count(); i++)
                            {
                                // delete TourCustomer
                                var tc = _tourCustomerRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId.ToString() == listIds[i]).ToList();
                                if (tc.Count() > 0)
                                {
                                    foreach (var t in tc)
                                    {
                                        await _tourCustomerRepository.DeleteMany(t.Id.ToString().Split(',').ToArray(), false);
                                    }
                                }

                                // delete TourGuide
                                var tg = _tourGuideRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId.ToString() == listIds[i]).ToList();
                                if (tg.Count() > 0)
                                {
                                    foreach (var t in tg)
                                    {
                                        await _tourGuideRepository.DeleteMany(t.Id.ToString().Split(',').ToArray(), false);
                                    }
                                }

                                // delete TourSchedule
                                var ts = _tourScheduleRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId.ToString() == listIds[i]).ToList();
                                if (ts.Count() > 0)
                                {
                                    foreach (var t in ts)
                                    {
                                        await _tourScheduleRepository.DeleteMany(t.Id.ToString().Split(',').ToArray(), false);
                                    }
                                }
                            }

                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "TourManage") }, JsonRequestBehavior.AllowGet);
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

        #region Tạo lịch đi tour

        [ValidateInput(false)]
        public async Task<ActionResult> CreateScheduleTour(tbl_TourSchedule model, FormCollection form)
        {
            try
            {
                model.CreatedDate = DateTime.Now;
                model.TourId = Convert.ToInt32(Session["idTour"].ToString());
                model.StaffId = clsPermission.GetUser().StaffID;
                if (await _tourScheduleRepository.Create(model))
                {
                    UpdateHistory.SaveHistory(25, "Tạo lịch đi tour " + _tourRepository.FindId(model.TourId).Name,
                        null, //appointment
                        null, //contract
                        null, //customer
                        null, //partner
                        null, //program
                        null, //task
                        model.TourId, //tour
                        null, //quotation
                        null, //document
                        null, //history
                        null // ticket
                        );
                }
                //Response.Write("<script>alert('Đã lưu');</script>");
            }
            catch { }

            return Json(JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Nhiệm vụ

        public JsonResult LoadPermission(int id)
        {
            var model = new SelectList(_staffRepository.GetAllAsQueryable().Where(p => p.DepartmentId == id && p.IsVietlike == true && p.IsDelete == false).ToList(), "Id", "FullName");
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public async Task<ActionResult> CreateTaskTour(tbl_Task model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 78);
                model.IsNotify = model.NotifyDate != null ? true : false;
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.TaskStatusId = 1193;
                model.TourId = Convert.ToInt32(Session["idTour"].ToString());
                model.CodeTour = _tourRepository.FindId(model.TourId).Code;
                model.IsNotify = false;
                model.StaffId = clsPermission.GetUser().StaffID;
                await _taskRepository.Create(model);
                UpdateHistory.SaveHistory(78, "Thêm mới nhiệm vụ, code: " + model.Code + " - " + model.Name,
                    null, //appointment
                    null, //contract
                    null, //customer
                    null, //partner
                    null, //program
                    model.Id, //task
                    model.TourId, //tour
                    null, //quotation
                    null, //document
                    null, //history
                    null // ticket
                    );

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

                Response.Write("<script>alert('Đã lưu');</script>");
            }
            catch { }

            var list = _taskRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false)
                            .Select(p => new tbl_Task
                            {
                                Id = p.Id,
                                tbl_DictionaryTaskType = _dictionaryRepository.FindId(p.TaskTypeId),
                                Name = p.Name,
                                Permission = p.Permission,
                                StartDate = p.StartDate,
                                EndDate = p.EndDate,
                                Time = p.Time,
                                TimeType = p.TimeType,
                                FinishDate = p.FinishDate,
                                PercentFinish = p.PercentFinish,
                                tbl_Staff = _staffRepository.FindId(p.StaffId),
                                Note = p.Note,
                                tbl_DictionaryTaskPriority = _dictionaryRepository.FindId(p.TaskPriorityId),
                                TaskPriorityId = p.TaskPriorityId
                            }).ToList();
            return PartialView("~/Views/TourTabInfo/_NhiemVu.cshtml", list);
        }
        #endregion

        #region cập nhật loại tour

        public ActionResult UpdateTypeTour(tbl_Tour model)
        {
            try
            {
                int tourId = Convert.ToInt32(Session["idTour"].ToString());
                var item = _tourRepository.FindId(tourId);
                _db.SaveChanges();
            }
            catch { }

            return Json(JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region cập nhật trạng thái tour

        public ActionResult GetInfoStatus(int id)
        {
            var model = _tourRepository.FindId(id);
            return PartialView("_Partial_UpdateStatus", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateStatusTour(tbl_Tour model)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 24);
                model.ModifiedDate = DateTime.Now;
                if (await _tourRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(24, "Cập nhật tình trạng tour: " + model.Name,
                        null, //appointment
                        null, //contract
                        model.CustomerId, //customer
                        null, //partner
                        null, //program
                        null, //task
                        model.Id, //tour
                        null, //quotation
                        null, //document
                        null, //history
                        null // ticket
                        );
                }
            }
            catch { }

            int maPB = 0, maNKD = 0, maNV = 0, maCN = 0;
            switch (SDBID)
            {
                case 2: maPB = clsPermission.GetUser().DepartmentID;
                    maCN = clsPermission.GetUser().BranchID;
                    break;
                case 3: maNKD = clsPermission.GetUser().GroupID;
                    maCN = clsPermission.GetUser().BranchID; break;
                case 4: maNV = clsPermission.GetUser().StaffID; break;
                case 5: maCN = clsPermission.GetUser().BranchID; break;
            }

            DateTime tungay = DateTime.Now.AddDays(-15);
            DateTime denngay = DateTime.Now.AddDays(15);

            var items = _tourRepository.GetAllAsQueryable().Where(p =>
                    ((p.tbl_TourGuide.FirstOrDefault(g => g.StaffId == maNV) != null)
                    || (p.Permission.Contains(maNV.ToString()))
                    || (p.StaffId == maNV | maNV == 0))
                    || (p.CreateStaffId == maNV | maNV == 0) 
                    & (tungay <= p.StartDate && p.StartDate <= denngay || tungay <= p.EndDate && p.EndDate <= denngay)
                    & (p.tbl_StaffCreate.DepartmentId == maPB | maPB == 0)
                    & (p.tbl_StaffCreate.StaffGroupId == maNKD | maNKD == 0)
                    & (p.tbl_StaffCreate.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false))
                    .OrderByDescending(p => p.CreatedDate)
                    .Select(p => new TourListViewModel
                    {
                        Id = p.Id,
                        Code = p.Code,
                        Name = p.Name,
                        CustomerName = p.tbl_Customer.FullName,
                        NumberCustomer = p.NumberCustomer ?? 0,
                        DestinationPlace = p.tbl_TagsDestinationPlace.Tag,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        NumberDay = p.NumberDay ?? 0,
                        TourGuide = p.tbl_TourGuide.FirstOrDefault() == null ? "" : p.tbl_TourGuide.FirstOrDefault().tbl_Staff.FullName,
                        TourType = p.tbl_DictionaryTypeTour.Name,
                        Staff = p.tbl_StaffCreate.FullName,
                        Manager = p.tbl_Staff.FullName,
                        Permission = p.Permission,
                        Status = p.StatusId != null ? p.tbl_DictionaryStatus.Name : "",
                        Color = p.StatusId != null ? p.tbl_DictionaryStatus.Note : ""
                    }).ToList();

            foreach (var item in items)
            {
                item.CongNoDoiTac = _liabilityPartnerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id && c.IsDelete == false).Sum(c => c.TotalRemaining) ?? 0;
                item.CongNoKhachHang = _liabilityCustomerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id && c.IsDelete == false).Sum(c => c.TotalRemaining) ?? 0;
            }
            return PartialView("_Partial_ListTours", items);
        }

        #endregion

        #region Import Customer
        [HttpPost]
        public ActionResult ImportFile(HttpPostedFileBase FileName)
        {
            try
            {
                using (var excelPackage = new ExcelPackage(FileName.InputStream))
                {
                    int idtour = Int32.Parse(Session["idTour"].ToString());
                    int daidien = _tourRepository.FindId(idtour).CustomerId ?? 0;
                    List<tbl_Customer> list = new List<tbl_Customer>();
                    List<tbl_CustomerVisa> listVisa = new List<tbl_CustomerVisa>();
                    int i = 0;
                    var worksheet = excelPackage.Workbook.Worksheets[1];
                    var lastRow = worksheet.Dimension.End.Row;
                    for (int row = 7; row <= lastRow; row++)
                    {
                        if (worksheet.Cells["d" + row].Value != null)
                        {
                            var cus = new tbl_Customer
                            {
                                CustomerType = worksheet.Cells["b" + row].Value != null ? (worksheet.Cells["b" + row].Text.Contains("Cá nhân") ? CustomerType.Personal : CustomerType.Organization) : CustomerType.Personal,
                                FullName = worksheet.Cells["d" + row].Value != null ? worksheet.Cells["d" + row].Text : null,
                                PersonalEmail = worksheet.Cells["f" + row].Value != null ? worksheet.Cells["f" + row].Text : null,
                                Phone = worksheet.Cells["g" + row].Value != null ? worksheet.Cells["g" + row].Text : null,
                                Address = worksheet.Cells["h" + row].Value != null ? worksheet.Cells["h" + row].Text : null,
                                AccountNumber = worksheet.Cells["o" + row].Value != null ? worksheet.Cells["o" + row].Text : null,
                                Bank = worksheet.Cells["p" + row].Value != null ? worksheet.Cells["p" + row].Text : null,
                                IdentityCard = worksheet.Cells["q" + row].Value != null ? worksheet.Cells["q" + row].Text : null,
                                PassportCard = worksheet.Cells["t" + row].Value != null ? worksheet.Cells["t" + row].Text : null,
                                Position = worksheet.Cells["l" + row].Value != null ? worksheet.Cells["l" + row].Text : null,
                                Department = worksheet.Cells["m" + row].Value != null ? worksheet.Cells["m" + row].Text : null,
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now,
                                IsTemp = true,
                                ParentId = daidien,
                                StaffManager = clsPermission.GetUser().StaffID,
                            };
                            // staff
                            if (Request.IsAuthenticated)
                            {
                                string user = User.Identity.Name;
                                if (Request.Cookies["CookieUser" + user] != null)
                                {
                                    cus.StaffId = Convert.ToInt32(Request.Cookies["CookieUser" + user]["MaNV"]);
                                }
                            }

                            string cel = "";
                            try//ngay sinh
                            {
                                cel = "e";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    cus.Birthday = DateTime.ParseExact(worksheet.Cells[cel + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                                }
                            }
                            catch { }
                            try//ngay cấp cmnd
                            {
                                cel = "r";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    cus.CreatedDateIdentity = DateTime.ParseExact(worksheet.Cells[cel + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                                }
                            }
                            catch { }
                            try//ngay hiệu lực passport
                            {
                                cel = "u";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    cus.CreatedDatePassport = DateTime.ParseExact(worksheet.Cells[cel + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                                }
                            }
                            catch { }
                            try//ngay hết hạn passport
                            {
                                cel = "v";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    cus.ExpiredDatePassport = DateTime.ParseExact(worksheet.Cells[cel + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                                }
                            }
                            catch { }
                            try//danh xưng
                            {
                                cel = "c";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                    {
                                        string danhsung = worksheet.Cells[cel + row].Text;
                                        cus.NameTypeId = _dictionaryRepository.GetAllAsQueryable().AsEnumerable().FirstOrDefault(c => c.Name == danhsung && c.DictionaryCategoryId == 7 && c.IsDelete == false).Id;
                                    }
                                }
                            }
                            catch { }
                            try//tagid dia chi
                            {
                                cel = "i";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string tinhtp = worksheet.Cells[cel + row].Text;
                                    cus.TagsId = _tagsRepository.GetAllAsQueryable().AsEnumerable().SingleOrDefault(c => c.Tag == tinhtp && c.TypeTag == 5 && c.IsDelete == false).Id.ToString();
                                }
                                cel = "j";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string quanhuyen = worksheet.Cells[cel + row].Text;
                                    var tagid = _tagsRepository.GetAllAsQueryable().AsEnumerable().FirstOrDefault(c => c.Tag == quanhuyen && c.TypeTag == 6 && c.IsDelete == false);
                                    if (tagid != null)
                                        if (cus.TagsId != null)
                                            cus.TagsId += "," + tagid.Id;
                                        else
                                            cus.TagsId = tagid.Id.ToString();
                                }
                                cel = "k";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string phuongxa = worksheet.Cells[cel + row].Text;
                                    var tagid = _tagsRepository.GetAllAsQueryable().AsEnumerable().FirstOrDefault(c => c.Tag == phuongxa && c.TypeTag == 7 && c.IsDelete == false);
                                    if (tagid != null)
                                        if (cus.TagsId != null)
                                            cus.TagsId += "," + tagid.Id;
                                        else
                                            cus.TagsId = tagid.Id.ToString();
                                }
                            }
                            catch { }
                            try//ngành nghề
                            {
                                cel = "n";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string nganhnghe = worksheet.Cells[cel + row].Text;
                                    cus.CareerId = _dictionaryRepository.GetAllAsQueryable().AsEnumerable().SingleOrDefault(c => c.Name == nganhnghe && c.DictionaryCategoryId == 2 && c.IsDelete == false).Id;
                                }
                            }
                            catch { }
                            try//noi cap cmnd
                            {
                                cel = "s";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string noicap = worksheet.Cells[cel + row].Text;
                                    cus.IdentityTagId = _tagsRepository.GetAllAsQueryable().AsEnumerable().SingleOrDefault(c => c.Tag == noicap && c.TypeTag == 5 && c.IsDelete == false).Id;
                                }
                            }
                            catch { }
                            try//noi cap passport
                            {
                                cel = "w";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string noicap = worksheet.Cells[cel + row].Text;
                                    cus.PassportTagId = _tagsRepository.GetAllAsQueryable().AsEnumerable().SingleOrDefault(c => c.Tag == noicap && c.TypeTag == 5 && c.IsDelete == false).Id;
                                }
                            }
                            catch { }
                            try
                            {
                                var visa = new tbl_CustomerVisa
                                {
                                    CustomerId = i,
                                    VisaNumber = worksheet.Cells["x" + row].Value != null ? worksheet.Cells["x" + row].Text : "",
                                    Deadline = Int16.Parse(worksheet.Cells["ab" + row].Value != null ? worksheet.Cells["ab" + row].Text : "0"),
                                    CreatedDate = DateTime.Now,
                                    ModifiedDate = DateTime.Now,
                                    CreatedDateVisa = worksheet.Cells["z" + row].Value != null && worksheet.Cells["z" + row].Text != "" ? DateTime.ParseExact(worksheet.Cells["z" + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture) : new DateTime(),
                                    ExpiredDateVisa = worksheet.Cells["aa" + row].Value != null && worksheet.Cells["aa" + row].Text != "" ? DateTime.ParseExact(worksheet.Cells["aa" + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture) : new DateTime(),
                                };
                                try//trạng thái visa
                                {
                                    cel = "ac";
                                    if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                    {
                                        string trangthai = worksheet.Cells[cel + row].Text;
                                        visa.DictionaryId = _dictionaryRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Name == trangthai && c.DictionaryCategoryId == 14 && c.IsDelete == false).Select(c => c.Id).SingleOrDefault();
                                    }
                                }
                                catch { }
                                try//loại visa
                                {
                                    cel = "ad";
                                    if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                    {
                                        string loai = worksheet.Cells[cel + row].Text;
                                        visa.VisaTypeId = _dictionaryRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Name == loai && c.DictionaryCategoryId == 15 && c.IsDelete == false).Select(c => c.Id).SingleOrDefault();
                                    }
                                }
                                catch { }
                                try//tag id
                                {
                                    cel = "y";
                                    if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                    {
                                        string tag = worksheet.Cells[cel + row].Text;
                                        visa.TagsId = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == tag && c.TypeTag == 3 && c.IsDelete == false).Select(c => c.Id).SingleOrDefault();
                                    }
                                }
                                catch { }
                                listVisa.Add(visa);
                            }
                            catch { }
                            list.Add(cus);
                            i++;
                        }
                    }

                    Session["listCustomerImportTour"] = list;
                    Session["listCustomerVisaImportTour"] = listVisa;
                    return PartialView("_Partial_ImportDataList", list);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveImport()
        {
            Permission(clsPermission.GetUser().PermissionID, 79);

            int idtour = Int32.Parse(Session["idTour"].ToString());
            var tour = _db.tbl_Tour.Where(x => x.Id == idtour && x.IsDelete == false).FirstOrDefault();
            var pointOfTour = Convert.ToInt32(tour.Price / 50000);
            try
            {
                List<tbl_Customer> list = Session["listCustomerImportTour"] as List<tbl_Customer>;
                List<tbl_CustomerVisa> listVisa = Session["listCustomerVisaImportTour"] as List<tbl_CustomerVisa>;
                int i = 0, ii = 0;

                foreach (var item in list)
                {
                    var listTemp = _db.tbl_Customer.FirstOrDefault(x => x.FullName == item.FullName && x.IdentityCard == item.IdentityCard && x.Phone == item.Phone);
                    if (listTemp== null)
                    {
                        if (item.CustomerType == CustomerType.Personal)
                        {
                            //item.Code = GenerateCode.CustomerCode();
                            item.Point += pointOfTour;
                            await _customerRepository.Create(item);
                            var tcus = new tbl_TourCustomer()
                            {
                                TourId = idtour,
                                CustomerId = item.Id
                            };
                            await _tourCustomerRepository.Create(tcus);
                            i++;
                            if (listVisa[ii].VisaNumber != null)
                            {
                                var visa = listVisa[ii];
                                visa.CustomerId = item.Id;
                                await _customerVisaRepository.Create(visa);
                                var tourvisa = new tbl_TourCustomerVisa
                                {
                                    CustomerId = visa.Id,
                                    TourId = idtour
                                };
                                await _tourCustomerVisaRepository.Create(tourvisa);
                            }
                        }
                        ii++;
                    }
                    else
                    {
                        item.Point += pointOfTour;
                        await _customerRepository.Create(item);
                        i++;
                    }
                    
                }

                UpdateHistory.SaveHistory(79, "Import khách hàng vào tour " + _tourRepository.FindId(idtour).Name,
                    null, //appointment
                    null, //contract
                    null, //customer
                    null, //partner
                    null, //program
                    null, //task
                    idtour, //tour
                    null, //quotation
                    null, //document
                    null, //history
                    null // ticket
                    );
                Session["listCustomerImportTour"] = null;
                Session["listCustomerVisaImportTour"] = null;

                //var items = _tourCustomerRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.TourId == idtour && c.IsDelete == false).Select(c => c.tbl_Customer).ToList();
                //if (i != 0)
                //    return Json(new ActionModel() { Succeed = true, Html = RenderViewToString("~/Views/TourTabInfo/_KhachHang.cshtml", items), Code = "200", Message = "Đã import thành công!", IsPartialView = false }, JsonRequestBehavior.AllowGet);
                //else
                //    return Json(new ActionModel() { Succeed = false, Html = RenderViewToString("~/Views/TourTabInfo/_KhachHang.cshtml", items), Code = "200", Message = "Chưa có dữ liệu nào được import!" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                Session["listCustomerImportTour"] = null;
                Session["listCustomerVisaImportTour"] = null;
            }

            var items = _db.tbl_TourCustomer.AsEnumerable().Where(c => c.TourId == idtour && c.IsDelete == false).Select(c => c.tbl_Customer).ToList();
            return PartialView("~/Views/TourTabInfo/_KhachHang.cshtml", items);
        }

        [HttpPost]
        public ActionResult DeleteImport(String listItemId)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 79);
                List<tbl_Customer> list = Session["listCustomerImportTour"] as List<tbl_Customer>;
                List<tbl_CustomerVisa> listVisa = Session["listCustomerVisaImportTour"] as List<tbl_CustomerVisa>;
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
                            listVisa.RemoveAt(item);
                            list.RemoveAt(item);
                        }
                    }
                }
                Session["listCustomerImportTour"] = list;
                return PartialView("_Partial_ImportDataList", list);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Customer in tour
        [HttpPost]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 79);
                int idtour = Int16.Parse(Session["idTour"].ToString());
                var ctu = _tourCustomerRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.TourId == idtour && c.CustomerId == id).Single();
                var history = _db.tbl_UpdateHistory.AsEnumerable().Where(c => c.CustomerId == id).Where(p => p.IsDelete == false).ToList();
                foreach (var item in history)
                {
                    var listId = item.Id.ToString().Split(',').ToArray();
                    await _updateHistoryRepository.DeleteMany(listId, false);
                }
                var ctulistId = ctu.Id.ToString().Split(',').ToArray();
                await _tourCustomerRepository.DeleteMany(ctulistId, false);

                var listIds = ctu.Id.ToString().Split(',').ToArray();
                await _customerRepository.DeleteMany(listIds, false);
                return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Xóa dữ liệu thành công!" }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        #region Update
        /// <summary>
        /// partial view edit thông tin khách hàng
        /// </summary>
        /// <returns></returns>
        /// 
        //[ChildActionOnly]
        //public ActionResult _Partial_EditCustomer()
        //{
        //    return PartialView("_Partial_EditCustomer", new CustomerViewModel());
        //}

        /// <summary>
        /// load thông tin khách hàng
        /// </summary>
        /// <param name="model"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CustomerInfomation(int id)
        {
            var model = new CustomerViewModel();
            var customer = _customerRepository.GetAllAsQueryable().FirstOrDefault(p => p.Id == id);
            var customerVisa = _customerVisaRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.CustomerId == id).Where(p => p.IsDelete == false).ToList();
            if (customer.CustomerType == 0) // doanh nghiep
            {
                model.SingleCompany = customer;
                model.CreatedDateIdentity = customer.CreatedDateIdentity ?? DateTime.Now;
                model.CreatedDatePassport = customer.CreatedDatePassport ?? DateTime.Now;
                model.ExpiredDatePassport = customer.ExpiredDatePassport ?? DateTime.Now;
                model.IdentityCard = customer.IdentityCard;
                model.IdentityTagId = customer.IdentityTagId ?? 0;
                model.PassportCard = customer.PassportCard;
                model.PassportTagId = customer.PassportTagId ?? 0;
            }
            else // ca nhan
            {
                model.SinglePersonal = customer;
                model.CreatedDateIdentity = customer.CreatedDateIdentity ?? DateTime.Now;
                model.CreatedDatePassport = customer.CreatedDatePassport ?? DateTime.Now;
                model.ExpiredDatePassport = customer.ExpiredDatePassport ?? DateTime.Now;
                model.IdentityCard = customer.IdentityCard;
                model.IdentityTagId = customer.IdentityTagId ?? 0;
                model.PassportCard = customer.PassportCard;
                model.PassportTagId = customer.PassportTagId ?? 0;
                model.IsTemp = customer.IsTemp;
            }
            var contact = _customerContactRepository.GetAllAsQueryable().FirstOrDefault(p => p.CustomerId == id);
            if (contact != null)
            {
                model.SingleContact = contact;
            }
            if (customerVisa.Count() > 0)
            {
                model.ListCustomerVisa = customerVisa;
            }
            return PartialView("_Partial_EditCustomer", model);
        }

        /// <summary>
        /// cập nhật khách hàng
        /// </summary>
        /// <param name="model"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateCustomer(CustomerViewModel model, FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 79);
            try
            {
                if (model.SingleCompany.Id != 0)
                {
                    model.SingleCompany.CustomerType = CustomerType.Personal;
                    model.SingleCompany.TagsId = form["SingleCompany.TagsId"];
                    model.SingleCompany.ModifiedDate = DateTime.Now;
                    model.SingleCompany.IdentityCard = model.IdentityCard;
                    model.SingleCompany.IdentityTagId = model.IdentityTagId;
                    model.SingleCompany.ParentId = 0;
                    model.SingleCompany.PassportCard = model.PassportCard;
                    model.SingleCompany.PassportTagId = model.PassportTagId;
                    model.SingleCompany.NameTypeId = 47;
                    if (model.CreatedDateIdentity != null && model.CreatedDateIdentity.Year >= 1980)
                    {
                        model.SingleCompany.CreatedDateIdentity = model.CreatedDateIdentity;
                    }
                    if (model.CreatedDatePassport != null && model.CreatedDatePassport.Year >= 1980)
                    {
                        model.SingleCompany.CreatedDatePassport = model.CreatedDatePassport;
                    }
                    if (model.ExpiredDatePassport != null && model.ExpiredDatePassport.Year >= 1980)
                    {
                        model.SingleCompany.ExpiredDatePassport = model.ExpiredDatePassport;
                    }

                    if (await _customerRepository.Update(model.SingleCompany))
                    {
                        UpdateHistory.SaveHistory(79, "Cập nhật khách hàng, code: " + model.SingleCompany.Code + " - " + model.SingleCompany.FullName,
                            null, //appointment
                            null, //contract
                            model.SingleCompany.Id, //customer
                            null, //partner
                            null, //program
                            null, //task
                            null, //tour
                            null, //quotation
                            null, //document
                            null, //history
                            null // ticket
                            );
                        // xóa tất cả visa của customer
                        var visaList = _customerVisaRepository.GetAllAsQueryable().Where(p => p.CustomerId == model.SingleCompany.Id).Where(p => p.IsDelete == false).ToList();
                        if (visaList.Count() > 0)
                        {
                            foreach (var v in visaList)
                            {
                                var listIds = v.Id.ToString().Split(',').ToArray();
                                await _customerVisaRepository.DeleteMany(listIds, false);
                            }
                        }

                        // add các visa mới
                        for (int i = 1; i < 6; i++)
                        {
                            if (form["VisaNumber" + i] != null && form["VisaNumber" + i] != "")
                            {
                                var visa = new tbl_CustomerVisa
                                {
                                    VisaNumber = form["VisaNumber" + i].ToString(),
                                    TagsId = Convert.ToInt32(form["TagsId" + i].ToString()),
                                    CreatedDate = DateTime.Now,
                                    ModifiedDate = DateTime.Now,
                                    CustomerId = model.SingleCompany.Id,
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
                                await _customerVisaRepository.Create(visa);
                            }
                        }

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                if (model.SinglePersonal.Id != 0)
                {
                    model.SinglePersonal.CustomerType = CustomerType.Personal;
                    model.SinglePersonal.TagsId = form["SinglePersonal.TagsId"];
                    model.SinglePersonal.ModifiedDate = DateTime.Now;
                    model.SinglePersonal.IdentityCard = model.IdentityCard;
                    model.SinglePersonal.IdentityTagId = model.IdentityTagId;
                    model.SinglePersonal.ParentId = 0;
                    model.SinglePersonal.PassportCard = model.PassportCard;
                    model.SinglePersonal.PassportTagId = model.PassportTagId;
                    model.SinglePersonal.IsTemp = model.IsTemp;
                    if (model.CreatedDateIdentity != null && model.CreatedDateIdentity.Year >= 1980)
                    {
                        model.SinglePersonal.CreatedDateIdentity = model.CreatedDateIdentity;
                    }
                    if (model.CreatedDatePassport != null && model.CreatedDatePassport.Year >= 1980)
                    {
                        model.SinglePersonal.CreatedDatePassport = model.CreatedDatePassport;
                    }
                    if (model.ExpiredDatePassport != null && model.ExpiredDatePassport.Year >= 1980)
                    {
                        model.SinglePersonal.ExpiredDatePassport = model.ExpiredDatePassport;
                    }

                    if (await _customerRepository.Update(model.SinglePersonal))
                    {
                        UpdateHistory.SaveHistory(79, "Cập nhật khách hàng, code: " + model.SinglePersonal.Code + " - " + model.SinglePersonal.FullName,
                            null, //appointment
                            null, //contract
                            model.SinglePersonal.Id, //customer
                            null, //partner
                            null, //program
                            null, //task
                            null, //tour
                            null, //quotation
                            null, //document
                            null, //history
                            null // ticket
                            );
                        
                        // xóa tất cả visa của customer
                        var visaList = _customerVisaRepository.GetAllAsQueryable().Where(p => p.CustomerId == model.SinglePersonal.Id).Where(p => p.IsDelete == false).ToList();
                        if (visaList.Count() > 0)
                        {
                            foreach (var v in visaList)
                            {
                                var listIds = v.Id.ToString().Split(',').ToArray();
                                await _customerVisaRepository.DeleteMany(listIds, false);
                            }
                        }

                        // add các visa mới
                        for (int i = 1; i < 6; i++)
                        {
                            if (form["VisaNumber" + i] != null && form["VisaNumber" + i] != "")
                            {
                                var visa = new tbl_CustomerVisa
                                {
                                    VisaNumber = form["VisaNumber" + i].ToString(),
                                    TagsId = Convert.ToInt32(form["TagsId" + i].ToString()),
                                    CreatedDate = DateTime.Now,
                                    ModifiedDate = DateTime.Now,
                                    CustomerId = model.SinglePersonal.Id,
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
                                if (Convert.ToDateTime(form["CreatedDateVisa" + i]).Year >= 1980 && Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year >= 1980)
                                {
                                    int age = Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year - Convert.ToDateTime(form["CreatedDateVisa" + i]).Year;
                                    if (Convert.ToDateTime(form["CreatedDateVisa" + i]) > Convert.ToDateTime(form["ExpiredDateVisa" + i]).AddYears(-age)) age--;
                                    visa.Deadline = age;
                                }
                                await _customerVisaRepository.Create(visa);
                            }
                        }

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                if (model.SingleContact.Id != 0)
                {
                    model.SingleContact.TagsId = form["SinglePersonal.TagsId"];
                    model.SingleContact.ModifiedDate = DateTime.Now;
                    model.SingleContact.CreatedDateIdentity = model.CreatedDateIdentity;
                    model.SingleContact.CreatedDatePassport = model.CreatedDatePassport;
                    model.SingleContact.ExpiredDatePassport = model.ExpiredDatePassport;
                    model.SingleContact.IdentityCard = model.IdentityCard;
                    model.SingleContact.IdentityTagId = model.IdentityTagId;
                    model.SingleContact.PassportCard = model.PassportCard;
                    model.SingleContact.PassportTagId = model.PassportTagId;
                    if (model.CreatedDateIdentity != null && model.CreatedDateIdentity.Year >= 1980)
                    {
                        model.SingleContact.CreatedDateIdentity = model.CreatedDateIdentity;
                    }
                    if (model.CreatedDatePassport != null && model.CreatedDatePassport.Year >= 1980)
                    {
                        model.SingleContact.CreatedDatePassport = model.CreatedDatePassport;
                    }
                    if (model.ExpiredDatePassport != null && model.ExpiredDatePassport.Year >= 1980)
                    {
                        model.SingleContact.ExpiredDatePassport = model.ExpiredDatePassport;
                    }

                    if (await _customerContactRepository.Update(model.SingleContact))
                    {
                        UpdateHistory.SaveHistory(79, "Cập nhật người liên hệ: " + model.SingleContact.FullName,
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
                        // xóa tất cả visa của customer
                        var visaList = _customerContactVisaRepository.GetAllAsQueryable().Where(p => p.CustomerContactId == model.SingleContact.Id).Where(p => p.IsDelete == false).ToList();
                        if (visaList.Count() > 0)
                        {
                            foreach (var v in visaList)
                            {
                                var listIds = v.Id.ToString().Split(',').ToArray();
                                await _customerContactVisaRepository.DeleteMany(listIds, false);
                            }
                        }

                        // add các visa mới
                        for (int i = 1; i < 6; i++)
                        {
                            if (form["VisaNumber" + i] != null && form["VisaNumber" + i] != "")
                            {
                                var visa = new tbl_CustomerContactVisa
                                {
                                    VisaNumber = form["VisaNumber" + i].ToString(),
                                    TagsId = Convert.ToInt32(form["TagsId" + i].ToString()),
                                    CreatedDate = DateTime.Now,
                                    ModifiedDate = DateTime.Now,
                                    CustomerContactId = model.SingleContact.Id,
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
                                if (Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year >= 1980 && Convert.ToDateTime(form["CreatedDateVisa" + i]).Year >= 1980)
                                {
                                    int age = Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year - Convert.ToDateTime(form["CreatedDateVisa" + i]).Year;
                                    if (Convert.ToDateTime(form["CreatedDateVisa" + i]) > Convert.ToDateTime(form["ExpiredDateVisa" + i]).AddYears(-age)) age--;
                                    visa.Deadline = age;
                                }

                                await _customerContactVisaRepository.Create(visa);
                            }
                        }

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            catch
            {

            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CustomerNote(int id)
        {
            var model = _customerRepository.FindId(id);
            return PartialView("_Partial_EditCustomerNote", model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateCustomerNote(tbl_Customer mdl)
        {
            var model = _customerRepository.FindId(mdl.Id);
            model.NoteTour = mdl.NoteTour;
            await _customerRepository.Update(model);
            int idtour = Int16.Parse(Session["idTour"].ToString());
            var list = _tourCustomerRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.TourId == idtour && c.IsDelete == false).Select(c => c.tbl_Customer).ToList();
            return PartialView("~/Views/TourTabInfo/_KhachHang.cshtml", list);
        }
        #endregion

        [HttpPost]
        public async Task<ActionResult> CapNhatKH(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 79);
                var tour = _tourRepository.FindId(id);
                var custour = _tourCustomerRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.TourId == id).Where(p => p.IsDelete == false).Select(c => c.tbl_Customer).ToList();
                foreach (var item in custour)
                {
                    _db = new DataContext();
                    bool temp = false;
                    tbl_Customer cus = new tbl_Customer();

                    var namebirthday = _db.tbl_Customer.AsEnumerable().Where(c => c.FullName == item.FullName && c.Birthday == item.Birthday && c.IsTemp == false).FirstOrDefault();
                    if (namebirthday != null)
                    {
                        cus = namebirthday;
                        temp = true;
                    }

                    var cmnd = _db.tbl_Customer.AsEnumerable().Where(c => c.IdentityCard == item.IdentityCard && c.IsTemp == false).FirstOrDefault();
                    if (cmnd != null)
                    {
                        cus = cmnd;
                        temp = true;
                    }

                    var passport = _db.tbl_Customer.AsEnumerable().Where(c => c.PassportCard == item.PassportCard && c.IsTemp == false).FirstOrDefault();
                    if (passport != null)
                    {
                        cus = passport;
                        temp = true;
                    }

                    if (!temp)
                    {
                        try
                        {
                            var abs = _db.tbl_Customer.Find(item.Id);
                            abs.Code = LoadData.NewCodeCustomerPersonal();
                            abs.IsTemp = false;
                            _db.SaveChanges();
                        }
                        catch { }
                    }
                    else
                    {
                        cus = _db.tbl_Customer.AsEnumerable().Where(c => c.Id == cus.Id).Single();
                        cus.FullName = item.FullName;
                        cus.Birthday = item.Birthday;
                        cus.PersonalEmail = item.PersonalEmail;
                        cus.Phone = item.Phone;
                        cus.Address = item.Address;
                        cus.TagsId = item.TagsId;
                        cus.Position = item.Position;
                        cus.Department = item.Department;
                        cus.CareerId = item.CareerId;
                        cus.AccountNumber = item.AccountNumber;
                        cus.Bank = item.Bank;
                        cus.IdentityCard = item.IdentityCard;
                        cus.CreatedDateIdentity = item.CreatedDateIdentity;
                        cus.IdentityTagId = item.IdentityTagId;
                        cus.PassportCard = item.PassportCard;
                        cus.CreatedDatePassport = item.CreatedDatePassport;
                        cus.ExpiredDatePassport = item.ExpiredDatePassport;
                        cus.PassportTagId = item.PassportTagId;
                        cus.ParentId = item.ParentId;
                        cus.StaffId = item.StaffId;
                        cus.StaffManager = item.StaffManager;
                        cus.Note = item.Note;
                        var ctu = _tourCustomerRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.TourId == id && c.CustomerId == item.Id).SingleOrDefault();
                        ctu.CustomerId = cus.Id;
                        await _tourCustomerRepository.Update(ctu);
                        _db.SaveChanges();
                        var visas = _customerVisaRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.CustomerId == item.Id).Where(p => p.IsDelete == false).ToList();
                        foreach (var vs in visas)
                        {
                            vs.CustomerId = cus.Id;
                            await _customerVisaRepository.Update(vs);
                        }
                        var history = _db.tbl_UpdateHistory.AsEnumerable().Where(c => c.CustomerId == item.Id).Where(p => p.IsDelete == false).ToList();
                        foreach (var it in history)
                        {
                            var listIds = it.Id.ToString().Split(',').ToArray();
                            await _updateHistoryRepository.DeleteMany(listIds, false);
                        }
                        var listId = item.Id.ToString().Split(',').ToArray();
                        await _customerRepository.DeleteMany(listId, false);
                    }
                }
                var st = tour.StartDate ?? DateTime.Now;
                var en = tour.EndDate ?? DateTime.Now;
                TimeSpan totalDay = en - st;
                tour.NumberDay = Int32.Parse(totalDay.TotalDays.ToString());
                tour.NumberCustomer = _db.tbl_TourCustomer.AsEnumerable().Where(c => c.TourId == tour.Id).Count();
                tour.IsUpdate = true;
                if (await _tourRepository.Update(tour))
                {
                    UpdateHistory.SaveHistory(79, "Cập nhật tour: " + tour.Name,
                        null, //appointment
                        null, //contract
                        tour.CustomerId, //customer
                        null, //partner
                        null, //program
                        null, //task
                        tour.Id, //tour
                        null, //quotation
                        null, //document
                        null, //history
                        null // ticket
                        );
                }
                return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Cập nhật dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "TourManage") }, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("Index");
            }
            catch
            {
                return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Cập nhật dữ liệu thất bại !" }, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("Index");
            }
        }
        #endregion

        #region Excel Sample to Import

        public void ExportExcelTemplateCustomer(MemoryStream stream, string templateFile, IDictionary<string, string> header = null)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream, new MemoryStream(System.IO.File.ReadAllBytes(templateFile))))
            {
                var ws = xlPackage.Workbook.Worksheets[1]; //first worksheet
                var valWs = xlPackage.Workbook.Worksheets.Add("Validation");
                valWs.Hidden = eWorkSheetHidden.VeryHidden;

                // Nhóm KH
                var staffGroup = new List<ExportItem>();
                staffGroup.Add(new ExportItem { Text = "Cá nhân", Value = 1 });
                staffGroup.Add(new ExportItem { Text = "Doanh nghiệp", Value = 0 });

                var columnIndex = ws.GetColumnIndex(CustomerColumn.NHOMKHACH.ToString());
                ws.AddListValidation(valWs, staffGroup, columnIndex, "Lỗi", "Lỗi", "NHOMKHACH", "NHOMKHACHName");

                // Danh xưng
                var nametype = LoadData.NameTypeList().Select(p => new ExportItem
                {
                    Text = p.Name,
                    Value = p.Id
                });
                columnIndex = ws.GetColumnIndex(CustomerColumn.DANHXUNG.ToString());
                ws.AddListValidation(valWs, nametype, columnIndex, "Lỗi", "Lỗi", "DANHXUNG", "DANHXUNGName");

                // Tỉnh TP
                var tinhtp = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TypeTag == 5 && p.IsDelete == false)
                    .Select(p => new ExportItem
                    {
                        Text = p.Tag,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(CustomerColumn.TINHTP.ToString());
                ws.AddListValidation(valWs, tinhtp, columnIndex, "Lỗi", "Lỗi", "TINHTP", "TINHTPName");

                // Quận huyện
                var quanhuyen = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TypeTag == 6 && p.IsDelete == false)
                    .Select(p => new ExportItem
                    {
                        Text = p.Tag,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(CustomerColumn.QUANHUYEN.ToString());
                ws.AddListValidation(valWs, quanhuyen, columnIndex, "Lỗi", "Lỗi", "QUANHUYEN", "QUANHUYENName");

                // Phường xã
                var phuongxa = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TypeTag == 7 && p.IsDelete == false)
                    .Select(p => new ExportItem
                    {
                        Text = p.Tag,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(CustomerColumn.PHUONGXA.ToString());
                ws.AddListValidation(valWs, phuongxa, columnIndex, "Lỗi", "Lỗi", "PHUONGXA", "PHUONGXAName");

                // Ngành nghề
                var nganhnghe = LoadData.CareerList().Select(p => new ExportItem
                {
                    Text = p.Name,
                    Value = p.Id
                });
                columnIndex = ws.GetColumnIndex(CustomerColumn.NGANHNGHE.ToString());
                ws.AddListValidation(valWs, nganhnghe, columnIndex, "Lỗi", "Lỗi", "NGANHNGHE", "NGANHNGHEName");

                // Nơi cấp CMND
                var noicapcmnd = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TypeTag == 5 && p.IsDelete == false)
                    .Select(p => new ExportItem
                    {
                        Text = p.Tag,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(CustomerColumn.NOICAP.ToString());
                ws.AddListValidation(valWs, noicapcmnd, columnIndex, "Lỗi", "Lỗi", "NOICAP", "NOICAPName");

                // Nơi cấp Passport
                var noicappassprt = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TypeTag == 5 && p.IsDelete == false)
                    .Select(p => new ExportItem
                    {
                        Text = p.Tag,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(CustomerColumn.NOICAPPASSPORT.ToString());
                ws.AddListValidation(valWs, noicappassprt, columnIndex, "Lỗi", "Lỗi", "NOICAPPASSPORT", "NOICAPPASSPORTName");

                // Quốc gia Visa
                var quocgiavisa = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TypeTag == 3 && p.IsDelete == false)
                    .Select(p => new ExportItem
                    {
                        Text = p.Tag,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(CustomerColumn.QUOCGIAVISA.ToString());
                ws.AddListValidation(valWs, quocgiavisa, columnIndex, "Lỗi", "Lỗi", "QUOCGIAVISA", "QUOCGIAVISAName");

                // Trạng thái Visa
                var trangthai = LoadData.VisaStatusList().Select(p => new ExportItem
                {
                    Text = p.Name,
                    Value = p.Id
                });
                columnIndex = ws.GetColumnIndex(CustomerColumn.TRANGTHAIVISA.ToString());
                ws.AddListValidation(valWs, trangthai, columnIndex, "Lỗi", "Lỗi", "TRANGTHAIVISA", "TRANGTHAIVISAName");

                // Loại Visa
                var loaivisa = LoadData.VisaTypeList().Select(p => new ExportItem
                {
                    Text = p.Name,
                    Value = p.Id
                });
                columnIndex = ws.GetColumnIndex(CustomerColumn.LOAIVISA.ToString());
                ws.AddListValidation(valWs, loaivisa, columnIndex, "Lỗi", "Lỗi", "LOAIVISA", "LOAIVISAName");

                xlPackage.Save();
            }
        }

        public ActionResult ExcelSample()
        {
            try
            {
                IDictionary<string, string> header = new Dictionary<string, string>();
                header.Add("TourCode", "Tour Code:");
                header.Add("FlightDetails", "Flight details:");
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {

                    string templateFile = System.Web.HttpContext.Current.Server.MapPath("~\\Upload\\file\\CustomerTour_Import.xlsx");
                    ExportExcelTemplateCustomer(stream, templateFile, header);
                    bytes = stream.ToArray();
                }

                string fileName = "Mau-import-khach-hang-di-tour.xlsx";
                return File(bytes, "text/xls", fileName);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region CheckCodeTour
        public ActionResult CheckCodeTour(string code)
        {
            var check = _tourRepository.GetAllAsQueryable().FirstOrDefault(p => p.Code == code);
            if (check != null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
