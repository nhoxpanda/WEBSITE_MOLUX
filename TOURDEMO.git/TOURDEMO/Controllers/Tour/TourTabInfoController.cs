using CRM.Core;
using CRM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TOURDEMO.Models;
using TOURDEMO.Utilities;
using System.Data.Entity;

namespace TOURDEMO.Controllers.Tour
{
    [Authorize]
    public class TourTabInfoController : BaseController
    {
        // GET: TourTabInfo

        #region init
        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_ServicesPartner> _servicesPartnerRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;
        private IGenericRepository<tbl_TourGuide> _tourGuideRepository;
        private IGenericRepository<tbl_TourOption> _tourOptionRepository;
        private IGenericRepository<tbl_ReviewTour> _reviewTourRepository;
        private IGenericRepository<tbl_ReviewTourDetail> _reviewTourDetailRepository;
        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_CustomerVisa> _customerVisaRepository;
        private IGenericRepository<tbl_TourCustomerVisa> _tourCustomerVisaRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;
        private IGenericRepository<tbl_Task> _taskRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_AppointmentHistory> _appointmentHistoryRepository;
        private IGenericRepository<tbl_ContactHistory> _contactHistoryRepository;
        private IGenericRepository<tbl_Contract> _contractRepository;
        private IGenericRepository<tbl_Partner> _partnerRepository;
        private IGenericRepository<tbl_Program> _programRepository;
        private IGenericRepository<tbl_Quotation> _quotationRepository;
        private IGenericRepository<tbl_LiabilityCustomer> _liabilityCustomerRepository;
        private IGenericRepository<tbl_LiabilityPartner> _liabilityPartnerRepository;
        private IGenericRepository<tbl_TourCustomer> _tourCustomerRepository;
        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
        private IGenericRepository<tbl_Form> _formRepository;
        private IGenericRepository<tbl_Module> _moduleRepository;
        private DataContext _db;

        public TourTabInfoController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_ServicesPartner> servicesPartnerRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IGenericRepository<tbl_TourGuide> tourGuideRepository,
            IGenericRepository<tbl_TourOption> tourOptionRepository,
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
            IGenericRepository<tbl_Program> programRepository,
            IGenericRepository<tbl_Quotation> quotationRepository,
            IGenericRepository<tbl_LiabilityCustomer> liabilityCustomerRepository,
            IGenericRepository<tbl_LiabilityPartner> liabilityPartnerRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Form> formRepository,
            IGenericRepository<tbl_Module> moduleRepository,
            IGenericRepository<tbl_TourCustomer> tourCustomerRepository,
            IGenericRepository<tbl_TourCustomerVisa> tourCustomerVisaRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._tourOptionRepository = tourOptionRepository;
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
            this._programRepository = programRepository;
            this._quotationRepository = quotationRepository;
            this._liabilityCustomerRepository = liabilityCustomerRepository;
            this._liabilityPartnerRepository = liabilityPartnerRepository;
            this._staffRepository = staffRepository;
            this._tourCustomerRepository = tourCustomerRepository;
            this._tourCustomerVisaRepository = tourCustomerVisaRepository;
            this._tourGuideRepository = tourGuideRepository;
            this._formRepository = formRepository;
            this._moduleRepository = moduleRepository;
            this._updateHistoryRepository = updateHistoryRepository;
            _db = new DataContext();
        }
        #endregion

        void Permission(int PermissionsId, int formId)
        {
            var list = _db.tbl_ActionData.Where(p => p.FormId == formId & p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsEdit = list.Contains(3);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsImport = list.Contains(4);
        }

        #region Chi tiết tour
        [ChildActionOnly]
        public ActionResult _ChiTietTour()
        {
            return PartialView("_ChiTietTour");
        }

        [HttpPost]
        public async Task<ActionResult> InfoChiTietTour(int id)
        {
            var model = await _tourRepository.GetById(id);
            return PartialView("_ChiTietTour", model);
        }
        #endregion

        #region Ds dịch vụ
        [ChildActionOnly]
        public ActionResult _DichVu()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsEdit = false;
            ViewBag.IsDelete = false;
            return PartialView("_DichVu");
        }

        [HttpPost]
        public ActionResult InfoDichVu(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 77);
            var list = _tourOptionRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == id && p.IsDelete == false)
                            .Select(p => new TourServiceViewModel
                            {
                                Id = p.Id,
                                Code = p.tbl_Partner.Code,
                                ServiceId = _dictionaryRepository.FindId(p.tbl_Partner.DictionaryId).Id,
                                ServiceName = _dictionaryRepository.FindId(p.tbl_Partner.DictionaryId).Name,
                                Name = p.tbl_Partner.Name,
                                Address = p.tbl_Partner.Address,
                                StaffContact = p.tbl_Partner.StaffContact,
                                Phone = p.tbl_Partner.Phone,
                                Note = p.tbl_Partner.Note,
                                TourOptionId = p.Id,
                                TourId = p.TourId,
                                Price = p.tbl_ServicesPartner.Price,
                                PartnerId = p.PartnerId,
                                ServicePartnerId = p.ServicePartnerId ?? 0,
                                Currency = _dictionaryRepository.FindId(p.tbl_ServicesPartner.CurrencyId).Name
                            }).ToList();
            return PartialView("_DichVu", list);
        }
        #endregion

        #region Ds nhiệm vụ
        [ChildActionOnly]
        public ActionResult _NhiemVu()
        {
            Permission(clsPermission.GetUser().PermissionID, 78);
            ViewBag.IsAdd = false;
            ViewBag.IsEdit = false;
            ViewBag.IsDelete = false;
            return PartialView("_NhiemVu");
        }

        [HttpPost]
        public ActionResult InfoNhiemVu(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 78);

            var model = _taskRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == id && p.IsDelete == false)
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
                               tbl_DictionaryTaskPriority = _dictionaryRepository.FindId(p.TaskPriorityId),
                               TaskPriorityId = p.TaskPriorityId,
                               Note = p.Note
                           }).ToList();
            return PartialView("_NhiemVu", model);
        }
        #endregion

        #region Khách hàng
        [ChildActionOnly]
        public ActionResult _KhachHang()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsEdit = false;
            ViewBag.IsDelete = false;
            ViewBag.IsImport = false;
            return PartialView("_KhachHang");
        }

        [HttpPost]
        public async Task<ActionResult> InfoKhachHang(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 79);
            Session["idTour"] = id;
            var model = _tourCustomerRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.TourId == id && c.IsDelete == false).Select(c => c.tbl_Customer).ToList();
            return PartialView("_KhachHang", model);
        }
        #endregion

        #region Chương trình
        [ChildActionOnly]
        public ActionResult _ChuongTrinh()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsEdit = false;
            ViewBag.IsDelete = false;
            return PartialView("_ChuongTrinh");
        }

        [HttpPost]
        public async Task<ActionResult> InfoChuongTrinh(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 80);
            var model = await _programRepository.GetAllAsQueryable()
                            .Where(p => p.TourId == id && p.IsDelete == false).OrderByDescending(p => p.CreatedDate).ToListAsync();
            return PartialView("_ChuongTrinh", model);
        }
        #endregion

        #region Hợp đồng
        [ChildActionOnly]
        public ActionResult _HopDong()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsEdit = false;
            ViewBag.IsDelete = false;
            return PartialView("_HopDong");
        }

        [HttpPost]
        public ActionResult InfoHopDong(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1140);
            var docs = _documentFileRepository.GetAllAsQueryable();
            var model = _contractRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.TourId == id && p.IsDelete == false)
                .Select(p => new ContractTourViewModel
                {
                    Code = p.Code,
                    ContractDate = p.ContractDate,
                    CreatedDate = p.CreatedDate,
                    Id = p.Id,
                    LoiNhuanDuKien = p.LoiNhuanDuKien,
                    NumberDay = p.NumberDay ?? 0,
                    Permission = p.Permission,
                    StartDate = p.StartDate,
                    StatusContractId = p.StatusContractId,
                    tbl_DictionaryCurrency = p.tbl_DictionaryCurrency,
                    tbl_DictionaryCurrencyLNDK = p.tbl_DictionaryCurrencyLNDK,
                    tbl_DictionaryCurrencyTDK = p.tbl_DictionaryCurrencyTDK,
                    tbl_DictionaryStatus = p.tbl_DictionaryStatus,
                    tbl_Staff = p.tbl_Staff,
                    TongDuKien = p.TongDuKien,
                    TotalPrice = p.TotalPrice,
                    DocumentId = docs.FirstOrDefault(x => x.ContractId == p.Id) != null ? docs.FirstOrDefault(x => x.ContractId == p.Id).Id : 0,
                    FileName = docs.FirstOrDefault(x => x.ContractId == p.Id) != null ? docs.FirstOrDefault(x => x.ContractId == p.Id).FileName : ""
                }).ToList();
            return PartialView("_HopDong", model);
        }
        #endregion

        #region Visa
        [ChildActionOnly]
        public ActionResult _Visa()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsEdit = false;
            ViewBag.IsDelete = false;
            return PartialView("_Visa");
        }

        [HttpPost]
        public ActionResult InfoVisa(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 82);
            Session["idTour"] = id;
            var model = _tourCustomerVisaRepository.GetAllAsQueryable().AsEnumerable()
                .Where(c => c.TourId == id && c.IsDelete == false).Select(c => c.tbl_CustomerVisa).ToList();
            return PartialView("_Visa", model);
        }
        #endregion

        #region Báo giá
        [ChildActionOnly]
        public ActionResult _ViettourBaoGia()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsEdit = false;
            ViewBag.IsDelete = false;
            return PartialView("_ViettourBaoGia");
        }

        //[HttpPost]
        //public async Task<ActionResult> InfoViettourBaoGia(int id)
        //{
        //    Permission(clsPermission.GetUser().PermissionID, 83);
        //    var model = await _quotationRepository.GetAllAsQueryable().Where(p => p.TourId == id && p.IsDelete == false).ToListAsync();
        //    return PartialView("_ViettourBaoGia", model);
        //}
        #endregion

        #region Công nợ khách hàng
        [ChildActionOnly]
        public ActionResult _CongNoKH()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsEdit = false;
            ViewBag.IsDelete = false;
            return PartialView("_CongNoKH");
        }

        [HttpPost]
        public async Task<ActionResult> InfoCongNoKH(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 84);
            var model = await _liabilityCustomerRepository.GetAllAsQueryable().Where(p => p.TourId == id && p.IsDelete == false).ToListAsync();
            return PartialView("_CongNoKH", model);
        }
        #endregion

        #region Công nợ đối tác
        [ChildActionOnly]
        public ActionResult _CongNoDoiTac()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsEdit = false;
            ViewBag.IsDelete = false;
            return PartialView("_CongNoDoiTac");
        }

        [HttpPost]
        public ActionResult InfoCongNoDoiTac(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 85);
            var model = _liabilityPartnerRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == id).Where(p => p.IsDelete == false)
                            .Select(p => new tbl_LiabilityPartner
                            {
                                Id = p.Id,
                                tbl_Staff = _staffRepository.FindId(p.StaffId),
                                tbl_Partner = _partnerRepository.FindId(p.PartnerId),
                                PaymentMethod = p.PaymentMethod,
                                ServicePrice = p.ServicePrice,
                                FirstPayment = p.FirstPayment,
                                SecondPayment = p.SecondPayment,
                                TotalRemaining = p.TotalRemaining,
                                tbl_DictionaryCurrencyType1 = _dictionaryRepository.FindId(p.FirstCurrencyType),
                                Note = p.Note
                            }).ToList();
            return PartialView("_CongNoDoiTac", model);
        }
        #endregion

        #region Đánh giá
        [ChildActionOnly]
        public ActionResult _DanhGia()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsEdit = false;
            ViewBag.IsDelete = false;
            return PartialView("_DanhGia");
        }

        [HttpPost]
        public ActionResult InfoDanhGia(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 86);
            var model = _reviewTourDetailRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.tbl_ReviewTour.TourId == id).Where(p => p.IsDelete == false)
                .Select(p => new ReviewTourModel
                {
                    Id = p.Id,
                    Email = p.tbl_ReviewTour.tbl_Customer.PersonalEmail,
                    Name = p.tbl_ReviewTour.tbl_Customer.FullName,
                    Note = p.tbl_ReviewTour.Note,
                    Phone = p.tbl_ReviewTour.tbl_Customer.MobilePhone,
                    Service = p.tbl_Dictionary.Name,
                    Mark = p.Mark,
                    Staff = p.tbl_ReviewTour.tbl_Staff.FullName,
                    Date = p.tbl_ReviewTour.CreatedDate
                }).ToList();
            return PartialView("_DanhGia", model);
        }
        #endregion

        #region Lịch hẹn
        [ChildActionOnly]
        public ActionResult _LichHen()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsEdit = false;
            ViewBag.IsDelete = false;
            return PartialView("_LichHen");
        }

        [HttpPost]
        public ActionResult InfoLichHen(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 87);
            var model = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.TourId == id && p.IsDelete == false)
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new tbl_AppointmentHistory
                {
                    Id = p.Id,
                    Title = p.Title,
                    Time = p.Time,
                    tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                    tbl_Staff = _staffRepository.FindId(p.StaffId),
                    Note = p.Note,
                    OtherStaff = p.OtherStaff,
                    StatusId = p.StatusId,
                    StaffId = p.StaffId
                }).ToList();
            return PartialView("_LichHen", model);
        }
        #endregion

        #region Tài liệu mẫu
        [ChildActionOnly]
        public ActionResult _TaiLieuMau()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsEdit = false;
            ViewBag.IsDelete = false;
            return PartialView("_TaiLieuMau");
        }

        [HttpPost]
        public async Task<ActionResult> InfoTaiLieuMau(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 88);
            var model = await _documentFileRepository.GetAllAsQueryable().Where(p => p.TourId == id && p.IsDelete == false).ToListAsync();
            return PartialView("_TaiLieuMau", model);
        }
        #endregion

        #region Lịch sử liên hệ
        [ChildActionOnly]
        public ActionResult _LichSuLienHe()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsEdit = false;
            ViewBag.IsDelete = false;
            return PartialView("_LichSuLienHe");
        }

        [HttpPost]
        public ActionResult InfoLichSuLienHe(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 89);
            var model = _contactHistoryRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == id).Where(p => p.IsDelete == false)
                 .Select(p => new tbl_ContactHistory
                 {
                     Id = p.Id,
                     ContactDate = p.ContactDate,
                     Request = p.Request,
                     Note = p.Note,
                     tbl_Staff = _staffRepository.FindId(p.StaffId),
                     tbl_Dictionary = _dictionaryRepository.FindId(p.DictionaryId),
                     StaffId = p.StaffId,
                     DictionaryId = p.DictionaryId,
                     OtherStaffId = p.OtherStaffId,
                     tbl_StaffOther = _staffRepository.FindId(p.OtherStaffId)
                 }).ToList();
            return PartialView("_LichSuLienHe", model);
        }
        #endregion

        #region Hướng dẫn viên
        [ChildActionOnly]
        public ActionResult _HuongDanVien()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsEdit = false;
            ViewBag.IsDelete = false;
            return PartialView("_HuongDanVien");
        }

        [HttpPost]
        public ActionResult InfoHuongDanVien(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1115);
            var model = _tourGuideRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.TourId == id && p.IsDelete == false)
                 .Select(p => new GuideListViewModel
                 {
                     StaffId = p.StaffId ?? 0,
                     GuideId = p.Id,
                     Name = p.tbl_Staff.FullName,
                     Birthday = p.tbl_Staff.Birthday != null ? string.Format("{0:dd-MM-yyyy}", p.tbl_Staff.Birthday) : "",
                     CodeGuide = p.tbl_Staff.CodeGuide,
                     File = _documentFileRepository.GetAllAsQueryable().Where(x => x.PermissionStaff.Contains(p.StaffId.ToString()) && x.IsDelete == false).ToList(),
                     Image = p.tbl_Staff.Image,
                     StartDate = p.StartDate != null ? string.Format("{0:dd-MM-yyyy}", p.StartDate) : "",
                     EndDate = p.EndDate != null ? string.Format("{0:dd-MM-yyyy}", p.EndDate) : "",
                     CreateDate = _staffRepository.FindId(p.StaffId).CreatedDate
                 }).OrderByDescending(p=>p.CreateDate).ToList();
            return PartialView("_HuongDanVien", model);
        }
        #endregion

        #region Cập nhật thay đổi
        [ChildActionOnly]
        public ActionResult _CapNhatThayDoi()
        {
            return PartialView("_CapNhatThayDoi");
        }

        [HttpPost]
        public ActionResult InfoCapNhatThayDoi(int id)
        {
            var model = _updateHistoryRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.IsDelete == false && p.TourId == id)
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new HistoryLogViewModel()
                {
                    Id = p.Id,
                    Staff = _staffRepository.FindId(p.StaffId).FullName,
                    Date = string.Format("{0:dd-MM-yyyy hh:mm tt}", p.CreatedDate),
                    Note = p.Note,
                    StaffId = p.StaffId,
                    Form = p.FormId != null ? _formRepository.FindId(p.FormId).Name : "",
                    Module = p.ModuleId != null ? _moduleRepository.FindId(p.ModuleId).Name : ""
                }).ToList();
            return PartialView("_CapNhatThayDoi", model);
        }
        #endregion
    }
}