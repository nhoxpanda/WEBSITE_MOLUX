using CRM.Core;
using CRM.Infrastructure;
using TOURDEMO.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TOURDEMO.Controllers.Tour
{
    [Authorize]
    public class TourGuideScheduleController : BaseController
    {
        // GET: TourGuideSchedule
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
        private DataContext _db;

        public TourGuideScheduleController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
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
            _db = new DataContext();
        }
        #endregion

        #region Permission
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

            var ltAccess = _db.tbl_AccessData.Where(p => p.PermissionId == PermissionsId && p.FormId == formId).Select(p => p.ShowDataById).FirstOrDefault();
            if (ltAccess != 0)
                this.SDBID = ltAccess;

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
        }

        #endregion

        #region List
        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 1094);
            return View();
        }

        public ActionResult TourGuideFilter(int id, DateTime? start, DateTime? end, int guide)
        {
            Permission(clsPermission.GetUser().PermissionID, 1094);

            if (SDBID == 6)
                return PartialView("_Partial_TourGuideList", new List<tbl_TourSchedule>());

            if (id == -1)
            {
                var _model = _tourScheduleRepository.GetAllAsQueryable().AsEnumerable()
                    .Where(p => (start != null ? p.StartDate >= start : p.StartDate >= DateTime.Now.AddMonths(-1))
                    && (end != null ? p.EndDate <= end : p.EndDate <= DateTime.Now.AddMonths(1))
                    && p.StaffId == (guide == 0 ? p.StaffId : guide)
                    & (p.StaffId == maNV | maNV == 0)
                    & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                    & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                    & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false))
                    .OrderByDescending(p => p.CreatedDate).ToList();

                return PartialView("_Partial_TourGuideList", _model);
            }
            else
            {
                var model = _tourScheduleRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.tbl_Tour.TypeTourId == id
                    && (start != null ? p.StartDate >= start : p.StartDate >= DateTime.Now.AddMonths(-1))
                    && (end != null ? p.EndDate <= end : p.EndDate <= DateTime.Now.AddMonths(1))
                    && p.StaffId == (guide == 0 ? p.StaffId : guide)
                    && p.StaffId == maNV | maNV == 0
                    & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                    & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                    & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false))
                    .OrderByDescending(p => p.CreatedDate).ToList();

                return PartialView("_Partial_TourGuideList", model);
            }
        }

        [ChildActionOnly]
        public ActionResult _Partial_TourGuideList()
        {
            Permission(clsPermission.GetUser().PermissionID, 1094);

            if (SDBID == 6)
                return PartialView("_Partial_TourGuideList", new List<tbl_TourSchedule>());
            var model = _tourScheduleRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => (p.StaffId == maNV | maNV == 0)
                    & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                    & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                    & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false))
                    .OrderByDescending(p => p.CreatedDate).ToList();

            return PartialView("_Partial_TourGuideList", model);
        }

        public JsonResult FilterDepartment(int id)
        {
            var model = _staffRepository.GetAllAsQueryable().Where(p => p.DepartmentId == id && p.IsVietlike == true).ToList();
            return Json(new SelectList(model, "Id", "FullName"), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region JsonCalendar
        [HttpPost]
        public JsonResult JsonCalendar(int id, DateTime? start, DateTime? end, int guide)
        {
            if (id == -1)
            {
                var _model = _tourScheduleRepository.GetAllAsQueryable().AsEnumerable()
                    .Where(p => (p.tbl_Tour.TypeTourId == (id == -1 ? p.tbl_Tour.TypeTourId : id)
                            && start != null ? p.StartDate >= start : p.StartDate >= DateTime.Now.AddMonths(-1))
                            && (end != null ? p.EndDate <= end : p.EndDate <= DateTime.Now.AddMonths(1))
                            && p.StaffId == (guide == 0 ? p.StaffId : guide)
                            & (p.StaffId == maNV | maNV == 0)
                            & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                            & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                            & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false))
                       .Select(p => new tbl_TourSchedule
                       {
                           Id = p.Id,
                           StartDate = p.StartDate,
                           EndDate = p.EndDate,
                           TourId = p.TourId,
                           tbl_Tour = _tourRepository.FindId(p.TourId),
                           StaffId = p.StaffId,
                           tbl_Staff = _staffRepository.FindId(p.StaffId)
                       }).ToList();

                var _eventList = from e in _model
                                 select new
                                 {
                                     id = e.Id,
                                     title = e.tbl_Tour.Name + " (" + e.tbl_Staff.FullName + ")",
                                     start = e.StartDate.Value.ToString("yyyy-MM-dd"),
                                     end = e.EndDate.Value.AddDays(1).ToString("yyyy-MM-dd"),
                                     constraint = e.Id,
                                 };
                var _rows = _eventList.ToArray();
                return Json(_rows, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Session["idTour"] = id;
                var model = _tourScheduleRepository.GetAllAsQueryable().AsEnumerable()
                    .Where(p => p.tbl_Tour.TypeTourId == id
                    && (start != null ? p.StartDate >= start : p.StartDate >= DateTime.Now.AddMonths(-1))
                    && (end != null ? p.EndDate <= end : p.EndDate <= DateTime.Now.AddMonths(1))
                    && p.StaffId == (guide == 0 ? p.StaffId : guide)
                    & (p.StaffId == maNV | maNV == 0)
                    & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                    & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                    & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false))
                   .Select(p => new tbl_TourSchedule
                   {
                       Id = p.Id,
                       StartDate = p.StartDate,
                       EndDate = p.EndDate,
                       TourId = p.TourId,
                       tbl_Tour = _tourRepository.FindId(p.TourId),
                       StaffId = p.StaffId,
                       tbl_Staff = _staffRepository.FindId(p.StaffId)
                   }).ToList();

                var eventList = from e in model
                                select new
                                {
                                    id = e.Id,
                                    title = e.tbl_Tour.Name + " (" + e.tbl_Staff.FullName + ")",
                                    start = e.StartDate.Value.ToString("yyyy-MM-dd"),
                                    end = e.EndDate.Value.AddDays(1).ToString("yyyy-MM-dd"),
                                    constraint = e.Id,
                                };
                var rows = eventList.ToArray();
                return Json(rows, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult JsonCalendarDefault()
        {
            var model = _tourScheduleRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => (p.StaffId == maNV | maNV == 0)
                    & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                    & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                    & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false))
               .Select(p => new tbl_TourSchedule
               {
                   Id = p.Id,
                   StartDate = p.StartDate,
                   EndDate = p.EndDate,
                   TourId = p.TourId,
                   tbl_Tour = _tourRepository.FindId(p.TourId),
                   StaffId = p.StaffId,
                   tbl_Staff = _staffRepository.FindId(p.StaffId)
               }).ToList();

            var eventList = from e in model
                            select new
                            {
                                id = e.Id,
                                title = e.tbl_Tour.Name + " (" + e.tbl_Staff.FullName + ")",
                                start = e.StartDate.Value.ToString("yyyy-MM-dd"),
                                end = e.EndDate.Value.AddDays(1).ToString("yyyy-MM-dd"),
                                constraint = e.Id,
                            };
            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Sửa lịch hướng dẫn viên

        public async Task<ActionResult> EditTourGuide(int id)
        {
            var model = await _tourScheduleRepository.GetById(id);
            return PartialView("_Partial_EditTourGuide", model);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTourGuide(tbl_TourSchedule model)
        {
            try
            {
                UpdateHistory.SaveHistory(1094, "Cập nhật hướng dẫn viên của tour: " + _tourRepository.FindId(model.TourId).Name,
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
                await _tourScheduleRepository.Update(model);
            }
            catch
            {
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}