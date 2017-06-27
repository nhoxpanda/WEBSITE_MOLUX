using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TOURDEMO.Models;
using CRM.Core;
using CRM.Infrastructure;
using System.Threading.Tasks;
using System.Data.Entity;
using TOURDEMO.Utilities;

namespace TOURDEMO.Controllers.Customer
{
    [Authorize]
    public class CustomerTabInfoController : BaseController
    {
        // GET: CustomerTabInfo
        #region Init

        private IGenericRepository<tbl_ReviewTour> _reviewTourRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;
        private IGenericRepository<tbl_TourGuide> _tourGuideRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_Partner> _partnerRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;
        private IGenericRepository<tbl_CustomerContact> _customerContactRepository;
        private IGenericRepository<tbl_CustomerVisa> _customerVisaRepository;
        private IGenericRepository<tbl_CustomerContactVisa> _customerContactVisaRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
        private IGenericRepository<tbl_ContactHistory> _contactHistoryRepository;
        private IGenericRepository<tbl_AppointmentHistory> _appointmentHistoryRepository;
        private IGenericRepository<tbl_TourCustomer> _tourCustomerRepository;
        private IGenericRepository<tbl_LiabilityCustomer> _liabilityCustomerRepository;
        private IGenericRepository<tbl_LiabilityPartner> _liabilityPartnerRepository;
        private IGenericRepository<tbl_Contract> _contractRepository;
        private IGenericRepository<tbl_Program> _programRepository;
        private IGenericRepository<tbl_Quotation> _quotationRepository;
        private IGenericRepository<tbl_Ticket> _ticketRepository;
        private IGenericRepository<tbl_Form> _formRepository;
        private IGenericRepository<tbl_Module> _moduleRepository;
        private IGenericRepository<tbl_MemberCard> _memberCardRepository;
        private IGenericRepository<tbl_MemberCardHistory> _memberCardHistoryRepository;
        private DataContext _db;

        public CustomerTabInfoController(
            IGenericRepository<tbl_ReviewTour> reviewTourRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IGenericRepository<tbl_TourGuide> tourGuideRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IGenericRepository<tbl_CustomerContact> customerContactRepository,
            IGenericRepository<tbl_CustomerVisa> customerVisaRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Form> formRepository,
            IGenericRepository<tbl_Module> moduleRepository,
            IGenericRepository<tbl_CustomerContactVisa> customerContactVisaRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IGenericRepository<tbl_ContactHistory> contactHistoryRepository,
            IGenericRepository<tbl_AppointmentHistory> appointmentHistoryRepository,
            IGenericRepository<tbl_TourCustomer> tourCustomerRepository,
            IGenericRepository<tbl_LiabilityCustomer> liabilityCustomerRepository,
            IGenericRepository<tbl_LiabilityPartner> liabilityPartnerRepository,
            IGenericRepository<tbl_Contract> contractRepository,
            IGenericRepository<tbl_Program> programRepository,
            IGenericRepository<tbl_Quotation> quotationRepository,
            IGenericRepository<tbl_Ticket> ticketRepository,
            IGenericRepository<tbl_MemberCard> memberCardRepository,
            IGenericRepository<tbl_MemberCardHistory> memberCardHistoryRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._staffRepository = staffRepository;
            this._reviewTourRepository = reviewTourRepository;
            this._partnerRepository = partnerRepository;
            this._customerRepository = customerRepository;
            this._customerContactRepository = customerContactRepository;
            this._tagsRepository = tagsRepository;
            this._customerVisaRepository = customerVisaRepository;
            this._customerContactVisaRepository = customerContactVisaRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._formRepository = formRepository;
            this._moduleRepository = moduleRepository;
            this._documentFileRepository = documentFileRepository;
            this._contactHistoryRepository = contactHistoryRepository;
            this._appointmentHistoryRepository = appointmentHistoryRepository;
            this._updateHistoryRepository = updateHistoryRepository;
            this._tourCustomerRepository = tourCustomerRepository;
            this._liabilityCustomerRepository = liabilityCustomerRepository;
            this._liabilityPartnerRepository = liabilityPartnerRepository;
            this._contractRepository = contractRepository;
            this._programRepository = programRepository;
            this._ticketRepository = ticketRepository;
            this._quotationRepository = quotationRepository;
            this._tourRepository = tourRepository;
            this._tourGuideRepository = tourGuideRepository;
            this._memberCardHistoryRepository = memberCardHistoryRepository;
            this._memberCardRepository = memberCardRepository;
            _db = new DataContext();
        }
        #endregion

        #region Phân quyền
        void Permission(int PermissionsId, int formId)
        {
            var list = _db.tbl_ActionData.Where(p => p.FormId == formId && p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);
            ViewBag.IsImport = list.Contains(4);
            ViewBag.IsExport = list.Contains(5);
        }
        #endregion

        #region Phản hồi khách hàng
        [ChildActionOnly]
        public ActionResult _PhanHoiKhachHang()
        {
            return PartialView("_PhanHoiKhachHang");
        }

        [HttpPost]
        public async Task<ActionResult> InfoPhanHoiKhachHang(int id)
        {
            var model = await _reviewTourRepository.GetAllAsQueryable()
                .Where(p => p.CustomerId == id && p.IsDelete == false).OrderByDescending(p => p.CreatedDate).ToListAsync();
            return PartialView("_PhanHoiKhachHang", model);
        }
        #endregion

        #region Người liên hệ
        [ChildActionOnly]
        public ActionResult _NguoiLienHe()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsDelete = false;
            ViewBag.IsEdit = false;
            return PartialView("_NguoiLienHe");
        }

        [HttpPost]
        public async Task<ActionResult> InfoNguoiLienHe(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1097);
            var model = await _customerContactRepository.GetAllAsQueryable()
                .Where(p => p.CustomerId == id && p.IsDelete == false).OrderByDescending(p => p.CreatedDate).ToListAsync();
            return PartialView("_NguoiLienHe", model);
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
            var cus = _customerRepository.FindId(id);
            var model = _updateHistoryRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.IsDelete == false && (p.Note.Contains(cus.Code) || p.CustomerId == id))
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

        #region Email
        [ChildActionOnly]
        public ActionResult _Email()
        {
            return PartialView("_Email");
        }

        [HttpPost]
        public ActionResult InfoEmail()
        {
            return PartialView("_Email");
        }
        #endregion

        #region Hồ sơ liên quan
        [ChildActionOnly]
        public ActionResult _HoSoLienQuan()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsDelete = false;
            ViewBag.IsEdit = false;
            return PartialView("_HoSoLienQuan");
        }

        [HttpPost]
        public ActionResult InfoHoSoLienQuan(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 55);
            var model = _db.tbl_DocumentFile.AsEnumerable()
                     .Where(p => p.CustomerId == id && p.IsDelete == false)
                     .OrderByDescending(p => p.CreatedDate)
                     .Select(p => new tbl_DocumentFile
                     {
                         Id = p.Id,
                         FileName = p.FileName,
                         FileSize = p.FileSize,
                         Note = p.Note,
                         ModifiedDate = p.ModifiedDate,
                         CreatedDate = p.CreatedDate,
                         TagsId = p.TagsId,
                         tbl_Staff = _staffRepository.FindId(p.StaffId)
                     }).ToList();
            return PartialView("_HoSoLienQuan", model);
        }
        #endregion

        #region Lịch hẹn
        [ChildActionOnly]
        public ActionResult _LichHen()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsDelete = false;
            ViewBag.IsEdit = false;
            return PartialView("_LichHen");
        }

        [HttpPost]
        public ActionResult InfoLichHen(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 53);
            var model = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.CustomerId == id && p.IsDelete == false).OrderByDescending(p => p.CreatedDate)
                .Select(p => new tbl_AppointmentHistory
                {
                    Id = p.Id,
                    Title = p.Title,
                    Time = p.Time,
                    tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                    tbl_Staff = _staffRepository.FindId(p.StaffId),
                    Note = p.Note,
                    StatusId = p.StatusId,
                    OtherStaff = p.OtherStaff,
                    StaffId = p.StaffId
                }).ToList();
            return PartialView("_LichHen", model);
        }
        #endregion

        #region Lịch sử liên hệ
        [ChildActionOnly]
        public ActionResult _LichSuLienHe()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsDelete = false;
            ViewBag.IsEdit = false;
            return PartialView("_LichSuLienHe");
        }

        [HttpPost]
        public async Task<ActionResult> InfoLichSuLienHe(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 56);
            var model = _contactHistoryRepository.GetAllAsQueryable().AsEnumerable()
                        .Where(p => p.CustomerId == id && p.IsDelete == false).OrderByDescending(p => p.CreatedDate)
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

        #region SMS
        [ChildActionOnly]
        public ActionResult _SMS()
        {
            return PartialView("_SMS");
        }

        [HttpPost]
        public ActionResult InfoSMS()
        {
            return PartialView("_SMS");
        }
        #endregion

        #region Thông tin chi tiết
        [ChildActionOnly]
        public ActionResult _ThongTinChiTiet()
        {
            return PartialView("_ThongTinChiTiet");
        }

        [HttpPost]
        public async Task<ActionResult> InfoThongTinChiTiet(int id)
        {
            var model = await _customerRepository.GetById(id);
            return PartialView("_ThongTinChiTiet", model);
        }
        #endregion

        #region Visa
        [ChildActionOnly]
        public ActionResult _Visa()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsDelete = false;
            ViewBag.IsEdit = false;
            return PartialView("_Visa");
        }

        [HttpPost]
        public async Task<ActionResult> InfoVisa(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 54);
            var model = await _customerVisaRepository.GetAllAsQueryable()
                .Where(p => p.CustomerId == id && p.IsDelete == false).OrderByDescending(p => p.CreatedDate).ToListAsync();
            return PartialView("_Visa", model);
        }
        #endregion

        #region Tour tuyến
        [ChildActionOnly]
        public ActionResult _TourTuyen()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsDelete = false;
            ViewBag.IsEdit = false;
            return PartialView("_TourTuyen");
        }

        [HttpPost]
        public ActionResult InfoTourTuyen(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1103);
            var model = _tourCustomerRepository.GetAllAsQueryable().AsEnumerable()
                        .Where(c => c.CustomerId == id && c.IsDelete == false)
                        .OrderByDescending(p => p.tbl_Tour.CreatedDate)
                        .Select(p => new TourListViewModel
                        {
                            SingleTour = p.TourId != null ? _tourRepository.FindId(p.TourId) : null,
                            TourType = p.TourId != null ? p.tbl_Tour.tbl_DictionaryTypeTour.Name : "",
                            Status = p.TourId != null ? p.tbl_Tour.tbl_DictionaryStatus.Name : ""
                        }).ToList();
            foreach (var item in model)
            {
                item.CongNoDoiTac = _liabilityPartnerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id && c.IsDelete == false).Sum(c => c.ServicePrice) ?? 0;
                item.Currency = item.CongNoDoiTac != 0 ? _liabilityPartnerRepository.GetAllAsQueryable().FirstOrDefault(c => c.TourId == item.Id && c.IsDelete == false).tbl_DictionaryCurrencyType1.Name : "";
                item.CongNoKhachHang = _liabilityCustomerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id && c.IsDelete == false).Sum(c => c.TotalContract) ?? 0;
                item.Currency = item.CongNoKhachHang != 0 ? _liabilityCustomerRepository.GetAllAsQueryable().FirstOrDefault(c => c.TourId == item.Id && c.IsDelete == false).tbl_DictionaryCurrency.Name : "";
            }

            return PartialView("_TourTuyen", model);
        }
        #endregion

        #region Hợp đồng
        [ChildActionOnly]
        public ActionResult _HopDong()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsDelete = false;
            ViewBag.IsEdit = false;
            return PartialView("_HopDong");
        }

        [HttpPost]
        public ActionResult InfoHopDong(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1104);
            var docs = _documentFileRepository.GetAllAsQueryable();
            var model = _contractRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.CustomerId == id && p.IsDelete == false)
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new ContractTourViewModel()
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

        #region Chương trình
        [ChildActionOnly]
        public ActionResult _ChuongTrinh()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsDelete = false;
            ViewBag.IsEdit = false;
            return PartialView("_ChuongTrinh");
        }

        [HttpPost]
        public ActionResult InfoChuongTrinh(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1106);
            var model = _programRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.CustomerId == id && p.IsDelete == false)
                .OrderByDescending(p => p.CreatedDate).ToList();
            return PartialView("_ChuongTrinh", model);
        }
        #endregion

        #region Báo giá
        [ChildActionOnly]
        public ActionResult _BaoGia()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsDelete = false;
            ViewBag.IsEdit = false;
            return PartialView("_BaoGia");
        }

        [HttpPost]
        public async Task<ActionResult> InfoBaoGia(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1107);
            var model = await _quotationRepository.GetAllAsQueryable()
                .Where(p => p.CustomerId == id && p.IsDelete == false)
                .OrderByDescending(p => p.CreatedDate).ToListAsync();
            return PartialView("_BaoGia", model);
        }
        #endregion

        #region Vé máy bay
        [ChildActionOnly]
        public ActionResult _CodeVe()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsDelete = false;
            ViewBag.IsEdit = false;
            return PartialView("_CodeVe");
        }

        [HttpPost]
        public async Task<ActionResult> InfoCodeVe(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1105);
            var model = await _ticketRepository.GetAllAsQueryable()
                .Where(p => p.CustomerId == id && p.IsDelete == false)
                .OrderByDescending(p => p.CreateDate).ToListAsync();
            return PartialView("_CodeVe", model);
        }
        #endregion

        #region Hướng dẫn viên
        [ChildActionOnly]
        public ActionResult _DanhSachHDV()
        {
            return PartialView("_DanhSachHDV");
        }

        [HttpPost]
        public ActionResult ListTourGuide(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1103);
            var model = _db.tbl_TourGuide.AsEnumerable()
                .Where(p => p.TourId == id && p.IsDelete == false)
                .OrderByDescending(p => p.CreateDate)
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
                 }).OrderByDescending(p => p.CreateDate).ToList();
            return PartialView("_DanhSachHDV", model);
        }
        #endregion

        #region Lịch sử thẻ thành viên
        [ChildActionOnly]
        public ActionResult _TheThanhVien()
        {
            return PartialView("_TheThanhVien");
        }

        [HttpPost]
        public ActionResult InfoTheThanhVien(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1103);
            var model = _memberCardHistoryRepository.GetAllAsQueryable().AsEnumerable()
                        .Where(p => p.CustomerId == id).OrderByDescending(p => p.CreatedDate).ToList();
            return PartialView("_TheThanhVien", model);
        }
        #endregion
    }
}