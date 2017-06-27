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
using System.Data.Entity.Core.Objects;

namespace TOURDEMO.Controllers.Staff
{
    [Authorize]
    public class StaffTabInfoController : BaseController
    {
        // GET: StaffTabInfo

        #region Init

        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_StaffSalary> _staffSalaryRepository;
        private IGenericRepository<tbl_StaffDayOff> _staffDayOffRepository;
        private IGenericRepository<tbl_StaffBonusDiscipline> _staffBonusDisciplineRepository;
        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_CustomerVisa> _customerVisaRepository;
        private IGenericRepository<tbl_Partner> _partnerRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;
        private IGenericRepository<tbl_CustomerContact> _customerContactRepository;
        private IGenericRepository<tbl_StaffVisa> _staffVisaRepository;
        private IGenericRepository<tbl_Task> _taskRepository;
        private IGenericRepository<tbl_Ticket> _ticketRepository;
        private IGenericRepository<tbl_Contract> _contractRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
        private IGenericRepository<tbl_ContactHistory> _contactHistoryRepository;
        private IGenericRepository<tbl_AppointmentHistory> _appointmentHistoryRepository;
        private IGenericRepository<tbl_TourSchedule> _tourScheduleRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;
        private IGenericRepository<tbl_LiabilityCustomer> _liabilityCustomerRepository;
        private IGenericRepository<tbl_LiabilityPartner> _liabilityPartnerRepository;
        private IGenericRepository<tbl_Program> _programRepository;
        private IGenericRepository<tbl_Form> _formRepository;
        private IGenericRepository<tbl_Module> _moduleRepository;
        private IGenericRepository<tbl_StaffSalaryDetail> _staffSalaryDetailRepository;
        private DataContext _db;

        public StaffTabInfoController(
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_StaffSalary> staffSalaryRepository,
            GenericRepository<tbl_StaffDayOff> staffDayOffRepository,
            IGenericRepository<tbl_StaffBonusDiscipline> staffBonusDisciplineRepository,
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IGenericRepository<tbl_CustomerContact> customerContactRepository,
            IGenericRepository<tbl_StaffVisa> staffVisaRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Task> taskRepository,
            IGenericRepository<tbl_Ticket> ticketRepository,
            IGenericRepository<tbl_Contract> contractRepository,
            IGenericRepository<tbl_CustomerVisa> customerVisaRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IGenericRepository<tbl_ContactHistory> contactHistoryRepository,
            IGenericRepository<tbl_AppointmentHistory> appointmentHistoryRepository,
            IGenericRepository<tbl_TourSchedule> tourScheduleRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IGenericRepository<tbl_LiabilityCustomer> liabilityCustomerRepository,
            IGenericRepository<tbl_LiabilityPartner> liabilityPartnerRepository,
            IGenericRepository<tbl_Program> programRepository,
            IGenericRepository<tbl_Form> formRepository,
            IGenericRepository<tbl_Module> moduleRepository,
            IGenericRepository<tbl_StaffSalaryDetail> staffSalaryDetailRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._staffRepository = staffRepository;
            this._staffSalaryRepository = staffSalaryRepository;
            this._staffBonusDisciplineRepository = staffBonusDisciplineRepository;
            this._partnerRepository = partnerRepository;
            this._customerRepository = customerRepository;
            this._customerContactRepository = customerContactRepository;
            this._tagsRepository = tagsRepository;
            this._staffVisaRepository = staffVisaRepository;
            this._taskRepository = taskRepository;
            this._ticketRepository = ticketRepository;
            this._contractRepository = contractRepository;
            this._customerVisaRepository = customerVisaRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._documentFileRepository = documentFileRepository;
            this._contactHistoryRepository = contactHistoryRepository;
            this._appointmentHistoryRepository = appointmentHistoryRepository;
            this._updateHistoryRepository = updateHistoryRepository;
            this._tourScheduleRepository = tourScheduleRepository;
            this._tourRepository = tourRepository;
            this._liabilityCustomerRepository = liabilityCustomerRepository;
            this._liabilityPartnerRepository = liabilityPartnerRepository;
            this._programRepository = programRepository;
            this._formRepository = formRepository;
            this._moduleRepository = moduleRepository;
            this._staffDayOffRepository = staffDayOffRepository;
            this._staffSalaryDetailRepository = staffSalaryDetailRepository;
            _db = new DataContext();
        }
        #endregion

        #region Permission
        void Permission(int PermissionsId, int formId)
        {
            var list = _db.tbl_ActionData.Where(p => p.FormId == formId && p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);
        }
        #endregion

        #region ThongTinChiTiet
        [ChildActionOnly]
        public ActionResult _ThongTinChiTiet()
        {
            return PartialView("_ThongTinChiTiet");
        }

        [HttpPost]
        public async Task<ActionResult> InfoThongTinChiTiet(int id)
        {
            var firstDate = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0, DateTimeKind.Local);
            var lastDate=new DateTime(DateTime.Now.Year, 12, 31, 23, 59, 59, DateTimeKind.Local);
            var sumNumDayOff = _staffDayOffRepository.GetAllAsQueryable().Where(x => x.StaffId == id && x.StartDay >= firstDate && x.StartDay <= lastDate && x.EndDate >= firstDate && x.StartDay <= lastDate).Sum(x=>x.NumberDay);
            ViewBag.sumNumDayOff = sumNumDayOff;
            var model = await _staffRepository.GetById(id);

            return PartialView("_ThongTinChiTiet", model);
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
        public async Task<ActionResult> InfoLichHen(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 58);
            //var model = await _appointmentHistoryRepository.GetAllAsQueryable().Where(p => p.StaffId == id).ToListAsync();
            var model = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable()
                            .Where(p => p.StaffId == id || (p.OtherStaff != null &&
                                p.OtherStaff.Contains(id.ToString()) && p.IsDelete == false))
                            .OrderByDescending(p => p.CreatedDate)
                            .Select(p => new tbl_AppointmentHistory
                            {
                                Id = p.Id,
                                Title = p.Title,
                                Time = p.Time,
                                StatusId = p.StatusId,
                                tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                                tbl_Staff = _staffRepository.FindId(p.StaffId),
                                Note = p.Note,
                                OtherStaff = p.OtherStaff,
                                StaffId = p.StaffId
                            }).ToList();
            return PartialView("_LichHen", model);
        }
        #endregion

        #region Nhiệm vụ
        [ChildActionOnly]
        public ActionResult _NhiemVu()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsDelete = false;
            ViewBag.IsEdit = false;
            return PartialView("_NhiemVu");
        }

        [HttpPost]
        public async Task<ActionResult> InfoNhiemVu(int id)
        {
            ViewBag.IdStaff = id;
            Permission(clsPermission.GetUser().PermissionID, 57);
            var model = _taskRepository.GetAllAsQueryable().AsEnumerable()
                            .Where(p => (p.StaffId == id || p.Permission.Contains(id.ToString())) && p.IsDelete == false)
                            .OrderByDescending(p => p.CreatedDate)
                              .Select(p => new tbl_Task
                              {
                                  Id = p.Id,
                                  tbl_Staff = _staffRepository.FindId(p.StaffId),
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
                                  Note = p.Note
                              }).ToList();
            return PartialView("_NhiemVu", model);
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
        public ActionResult InfoLichSuLienHe(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 59);
            //var model = await _contactHistoryRepository.GetAllAsQueryable().Where(p => p.StaffId == id).ToListAsync();
            var model = _contactHistoryRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.OtherStaffId == id && p.IsDelete == false)
                .OrderByDescending(p => p.CreatedDate)
                       .Select(p => new tbl_ContactHistory
                       {
                           Id = p.Id,
                           ContactDate = p.CreatedDate,
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

        #region Thầu/tour
        [ChildActionOnly]
        public ActionResult _ThauTour()
        {
            return PartialView("_ThauTour");
        }

        [HttpPost]
        public async Task<ActionResult> InfoThauTour(int id)
        {
            var model = _tourRepository.GetAllAsQueryable().Where(c => c.StaffId == id).Where(p => p.IsDelete == false)
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new TourListViewModel
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    NumberCustomer = p.NumberCustomer ?? 0,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    NumberDay = p.NumberDay ?? 0,
                    TourType = p.tbl_DictionaryTypeTour.Name,
                    Status = p.tbl_DictionaryStatus.Name,
                }).ToList();
            foreach (var item in model)
            {
                item.CongNoDoiTac = _liabilityPartnerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id && c.IsDelete == false).Sum(c => c.ServicePrice) ?? 0;
                item.CongNoKhachHang = _liabilityCustomerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id && c.IsDelete == false).Sum(c => c.TotalContract) ?? 0;
            }
            return PartialView("_ThauTour", model);
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
            Permission(clsPermission.GetUser().PermissionID, 60);
            var model = _documentFileRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.PermissionStaff != null && p.PermissionStaff.Contains(id.ToString()) && p.IsDelete == false)
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
            return PartialView("_HoSoLienQuan", model);
        }
        #endregion

        #region Khách hàng
        [ChildActionOnly]
        public ActionResult _KhachHang()
        {
            return PartialView("_KhachHang");
        }

        [HttpPost]
        public async Task<ActionResult> InfoKhachHang(int id)
        {
            var model = _customerRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.StaffId == id).Where(p => p.IsDelete == false)
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new CustomerListViewModel
                {
                    Id = p.Id,
                    Address = p.Address,
                    Birthday = p.Birthday == null ? "" : p.Birthday.Value.ToString("dd-MM-yyyy"),
                    Career = p.CareerId != null ? p.tbl_DictionaryCareer.Name : "",
                    Code = p.Code == null ? "" : p.Code,
                    // Company = p.CompanyId == null ? "" : _db.tbl_Company.Find(p.CompanyId).Name,
                    Email = p.CompanyEmail == null ? p.PersonalEmail : p.CompanyEmail,
                    StartDate = p.CreatedDatePassport == null ? "" : p.CreatedDatePassport.Value.ToString("dd-MM-yyyy"),
                    EndDate = p.ExpiredDatePassport == null ? "" : p.ExpiredDatePassport.Value.ToString("dd-MM-yyyy"),
                    Fullname = p.FullName == null ? "" : p.FullName,
                    Phone = p.Phone == null ? "" : p.Phone,
                    OtherPhone = p.MobilePhone == null ? "" : p.MobilePhone,
                    Passport = p.PassportCard == null ? "" : p.PassportCard,
                    Skype = p.Skype == null ? "" : p.Skype,
                    TagsId = p.TagsId,
                    IdentityCard = p.IdentityCard ?? "",
                    Position = p.Position,
                    Department = p.Department,
                    CustomerType = p.CustomerType,
                    TaxCode = p.TaxCode

                }).ToList();
            return PartialView("_KhachHang", model);
        }
        #endregion

        #region Lịch sử đi tour
        [ChildActionOnly]
        public ActionResult _LichSuDiTour()
        {
            return PartialView("_LichSuDiTour");
        }

        [HttpPost]
        public ActionResult InfoLichSuDiTour(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 92);
            var model = _tourScheduleRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.IsDelete == false).Where(c => c.StaffId == id)
                .OrderByDescending(p => p.CreatedDate).ToList();
            return PartialView("_LichSuDiTour", model);
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
            Permission(clsPermission.GetUser().PermissionID, 61);
            var model = await _staffVisaRepository.GetAllAsQueryable()
                .Where(p => p.StaffId == id && p.IsDelete == false)
                .OrderByDescending(p => p.CreatedDate).ToListAsync();
            return PartialView("_Visa", model);
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
                .Where(p => p.IsDelete == false && p.StaffId == id)
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
            //Permission(clsPermission.GetUser().PermissionID, 1104);
            var docs = _documentFileRepository.GetAllAsQueryable();
            var model = _contractRepository.GetAllAsQueryable().AsEnumerable()
                        .Where(p => p.StaffId == id && p.IsDelete == false)
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
        public async Task<ActionResult> InfoChuongTrinh(int id)
        {
            //Permission(clsPermission.GetUser().PermissionID, 1106);
            var model = await _programRepository.GetAllAsQueryable()
                .Where(p => p.StaffId == id && p.IsDelete == false)
                .OrderByDescending(p => p.CreatedDate).ToListAsync();
            return PartialView("_ChuongTrinh", model);
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
            //Permission(clsPermission.GetUser().PermissionID, 1105);
            var model = await _ticketRepository.GetAllAsQueryable()
                .Where(p => p.Staff == id && p.IsDelete == false)
                .OrderByDescending(p => p.CreateDate).ToListAsync();
            return PartialView("_CodeVe", model);
        }
        #endregion

        #region Visa
        [ChildActionOnly]
        public ActionResult _VisaKH()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsDelete = false;
            ViewBag.IsEdit = false;
            return PartialView("_VisaKH");
        }

        [HttpPost]
        public async Task<ActionResult> InfoVisaKH(int id)
        {
            //Permission(clsPermission.GetUser().PermissionID, 54);
            var model = await _customerVisaRepository.GetAllAsQueryable()
                .Where(p => p.tbl_Customer.StaffId == id && p.IsDelete == false)
                .OrderByDescending(p => p.CreatedDate).ToListAsync();
            return PartialView("_VisaKH", model);
        }
        #endregion

        #region Lương
        [ChildActionOnly]
        public ActionResult _Luong()
        {
            return PartialView("_Luong");
        }

        [HttpPost]
        public ActionResult InfoLuong(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1128);
            List<tbl_StaffSalaryDetail> _listStaffSalaryDetail = new List<tbl_StaffSalaryDetail>();
            var _staffSalary = _staffSalaryRepository.GetAllAsQueryable().AsEnumerable()
                .Where(c => c.StaffId == id).LastOrDefault();
            if (_staffSalary != null)
            {
                _listStaffSalaryDetail = _staffSalaryDetailRepository.GetAllAsQueryable().AsEnumerable()
                .Where(c => c.StaffSalaryId == _staffSalary.Id).ToList();
            }


            var staffSalaryViewModel = new StaffSalaryViewModel()
            {
                staffSalary = _staffSalary,
                listStaffSalaryDetail = _listStaffSalaryDetail
            };
            return PartialView("_Luong", staffSalaryViewModel);
        }
        #endregion


        #region Ngày nghỉ phép
        [ChildActionOnly]
        public ActionResult _NgayNghiPhep()
        {
            return PartialView("_NgayNghiPhep");
        }

        [HttpPost]
        public ActionResult InfoNgayNghiPhep(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1130);
            var model = _staffDayOffRepository.GetAllAsQueryable().AsEnumerable()
                .Where(c => c.StaffId == id)
                .OrderByDescending(p => p.StartDay).ToList();

            return PartialView("_NgayNghiPhep", model);
        }
        #endregion

        #region Khen thưởng, kỷ luật
        [ChildActionOnly]
        public ActionResult _KhenThuongKyLuat()
        {
            return PartialView("_KhenThuongKyLuat");
        }

        [HttpPost]
        public ActionResult InfoKhenThuongKyLuat(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1129);
            var model = _staffBonusDisciplineRepository.GetAllAsQueryable().AsEnumerable()
                .Where(c => c.StaffId == id && c.IsDelete == false)
                .OrderByDescending(p => p.CreatedDate).ToList();

            return PartialView("_KhenThuongKyLuat", model);
        }
        #endregion
    }
}