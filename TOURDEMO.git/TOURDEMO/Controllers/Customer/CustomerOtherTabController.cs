using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TOURDEMO.Models;
using CRM.Core;
using CRM.Infrastructure;
using System.Threading.Tasks;
using CRM.Enum;
using TOURDEMO.Utilities;

namespace TOURDEMO.Controllers.Customer
{
    [Authorize]
    public class CustomerOtherTabController : BaseController
    {
        // GET: CustomerOtherTab

        #region Init

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
        private IGenericRepository<tbl_TourGuide> _tourGuideRepository;
        private IGenericRepository<tbl_TourCustomerVisa> _tourCustomerVisaRepository;
        private IGenericRepository<tbl_TourCustomer> _tourCustomerRepository;
        private IGenericRepository<tbl_TourSchedule> _tourScheduleRepository;
        private IGenericRepository<tbl_Contract> _contractRepository;
        private IGenericRepository<tbl_Program> _programRepository;
        private IGenericRepository<tbl_Quotation> _quotationRepository;
        private IGenericRepository<tbl_Ticket> _ticketRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;
        private IGenericRepository<tbl_LiabilityCustomer> _liabilityCustomerRepository;
        private IGenericRepository<tbl_LiabilityPartner> _liabilityPartnerRepository;
        private DataContext _db;

        public CustomerOtherTabController(
            IGenericRepository<tbl_TourSchedule> tourScheduleRepository,
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IGenericRepository<tbl_CustomerContact> customerContactRepository,
            IGenericRepository<tbl_CustomerVisa> customerVisaRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_CustomerContactVisa> customerContactVisaRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IGenericRepository<tbl_ContactHistory> contactHistoryRepository,
            IGenericRepository<tbl_AppointmentHistory> appointmentHistoryRepository,
            IGenericRepository<tbl_TourCustomerVisa> tourCustomerVisaRepository,
            IGenericRepository<tbl_TourCustomer> tourCustomerRepository,
            IGenericRepository<tbl_Contract> contractRepository,
            IGenericRepository<tbl_Program> programRepository,
            IGenericRepository<tbl_Quotation> quotationRepository,
            IGenericRepository<tbl_Ticket> ticketRepository,
            IGenericRepository<tbl_LiabilityCustomer> liabilityCustomerRepository,
            IGenericRepository<tbl_LiabilityPartner> liabilityPartnerRepository,
            IGenericRepository<tbl_TourGuide> tourGuideRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._tourScheduleRepository = tourScheduleRepository;
            this._tourGuideRepository = tourGuideRepository;
            this._partnerRepository = partnerRepository;
            this._customerRepository = customerRepository;
            this._customerContactRepository = customerContactRepository;
            this._tagsRepository = tagsRepository;
            this._customerVisaRepository = customerVisaRepository;
            this._customerContactVisaRepository = customerContactVisaRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._documentFileRepository = documentFileRepository;
            this._contactHistoryRepository = contactHistoryRepository;
            this._appointmentHistoryRepository = appointmentHistoryRepository;
            this._updateHistoryRepository = updateHistoryRepository;
            this._staffRepository = staffRepository;
            this._tourCustomerVisaRepository = tourCustomerVisaRepository;
            this._tourCustomerRepository = tourCustomerRepository;
            this._contractRepository = contractRepository;
            this._programRepository = programRepository;
            this._ticketRepository = ticketRepository;
            this._quotationRepository = quotationRepository;
            this._tourRepository = tourRepository;
            this._liabilityCustomerRepository = liabilityCustomerRepository;
            this._liabilityPartnerRepository = liabilityPartnerRepository;
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
            ViewBag.IsImport = list.Contains(4);
            ViewBag.IsExport = list.Contains(5);
        }
        #endregion

        #region Lịch hẹn
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateAppointment(tbl_AppointmentHistory model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 53);
                model.CustomerId = Convert.ToInt32(Session["idCustomer"].ToString());
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.StaffId = clsPermission.GetUser().StaffID;
                model.OtherStaff = form["OtherStaff"];
                model.PartnerId = model.PartnerId == 0 ? null : model.PartnerId;

                if (await _appointmentHistoryRepository.Create(model))
                {
                    UpdateHistory.SaveHistory(69, "Thêm mới lịch hẹn: " + model.Title + ", khách hàng: " + _customerRepository.FindId(model.CustomerId).Code,
                            model.Id, //appointment
                            model.ContractId, //contract
                            model.CustomerId, //customer
                            model.PartnerId, //partner
                            model.ProgramId, //program
                            model.TaskId, //task
                            model.TourId, //tour
                            null, //quotation
                            null, //document
                            null, //history
                            null // ticket
                            );

                    var list = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable()
                            .Where(p => p.CustomerId == model.CustomerId).Where(p => p.IsDelete == false)
                            .OrderByDescending(p => p.CreatedDate)
                            .Select(p => new tbl_AppointmentHistory
                            {
                                Id = p.Id,
                                Title = p.Title,
                                Time = p.Time,
                                tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                                tbl_Staff = _staffRepository.FindId(p.StaffId),
                                Note = p.Note,
                                StatusId = p.StatusId,
                                OtherStaff = p.OtherStaff
                            }).ToList();
                    return PartialView("~/Views/CustomerTabInfo/_LichHen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_LichHen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_LichHen.cshtml");
            }
        }

        public JsonResult LoadPartner(int id)
        {
            var model = new SelectList(_partnerRepository.GetAllAsQueryable().Where(p => p.DictionaryId == id && p.IsDelete == false).ToList(), "Id", "Name");
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EditAppointment(int id)
        {
            var model = await _appointmentHistoryRepository.GetById(id);
            return PartialView("_Partial_EditAppointmentHistory", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateAppointment(tbl_AppointmentHistory model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 53);
                model.ModifiedDate = DateTime.Now;
                model.OtherStaff = form["OtherStaff"];
                model.PartnerId = model.PartnerId == 0 ? null : model.PartnerId;

                if (await _appointmentHistoryRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(53, "Cập nhật lịch hẹn: " + model.Title + ", khách hàng: " + _customerRepository.FindId(model.CustomerId).Code,
                            model.Id, //appointment
                            model.ContractId, //contract
                            model.CustomerId, //customer
                            model.PartnerId, //partner
                            model.ProgramId, //program
                            model.TaskId, //task
                            model.TourId, //tour
                            null, //quotation
                            null, //document
                            null, //history
                        null // ticket
                            );
                    var list = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable()
                            .Where(p => p.CustomerId == model.CustomerId).Where(p => p.IsDelete == false)
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
                                OtherStaff = p.OtherStaff
                            }).ToList();
                    return PartialView("~/Views/CustomerTabInfo/_LichHen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_LichHen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_LichHen.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAppointment(int id)
        {
            int cusId = _appointmentHistoryRepository.FindId(id).CustomerId ?? 0;
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 53);
                var listId = id.ToString().Split(',').ToArray();
                //
                var item = _appointmentHistoryRepository.FindId(id);
                UpdateHistory.SaveHistory(53, "Xóa lịch hẹn: " + item.Title + ", khách hàng: " + _staffRepository.FindId(item.CustomerId).Code,
                            item.Id, //appointment
                            item.ContractId, //contract
                            item.CustomerId, //customer
                            item.PartnerId, //partner
                            item.ProgramId, //program
                            item.TaskId, //task
                            item.TourId, //tour
                            null, //quotation
                            null, //document
                            null, //history
                        null // ticket
                            );
                //
                if (await _appointmentHistoryRepository.DeleteMany(listId, false))
                {
                    var list = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable()
                            .Where(p => p.CustomerId == cusId).Where(p => p.IsDelete == false)
                            .OrderByDescending(p => p.CreatedDate)
                            .Select(p => new tbl_AppointmentHistory
                            {
                                Id = p.Id,
                                Title = p.Title,
                                StatusId = p.StatusId,
                                Time = p.Time,
                                tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                                tbl_Staff = _staffRepository.FindId(p.StaffId),
                                Note = p.Note,
                                OtherStaff = p.OtherStaff
                            }).ToList();
                    return PartialView("~/Views/CustomerTabInfo/_LichHen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_LichHen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_LichHen.cshtml");
            }
        }
        #endregion

        #region Lịch sử liên hệ
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateContactHistory(tbl_ContactHistory model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 56);
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.StaffId = clsPermission.GetUser().StaffID;
                model.OtherStaffId = model.StaffId;
                model.CustomerId = Int32.Parse(Session["idCustomer"].ToString());
                if (await _contactHistoryRepository.Create(model))
                {
                    UpdateHistory.SaveHistory(56, "Thêm mới lịch sử liên hệ: " + model.Request + ", khách hàng: " + _customerRepository.FindId(model.CustomerId).Code,
                            null, //appointment
                            model.ContractId, //contract
                            model.CustomerId, //customer
                            model.PartnerId, //partner
                            model.ProgramId, //program
                            null, //task
                            model.TourId, //tour
                            null, //quotation
                            null, //document
                            model.Id, //history
                        null // ticket
                            );

                    var list = _db.tbl_ContactHistory.AsEnumerable()
                        .Where(p => p.CustomerId == model.CustomerId).Where(p => p.IsDelete == false)
                        .OrderByDescending(p => p.CreatedDate)
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
                    return PartialView("~/Views/CustomerTabInfo/_LichSuLienHe.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_LichSuLienHe.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_LichSuLienHe.cshtml");
            }
        }

        //[ChildActionOnly]
        //public ActionResult _Partial_EditContactHistory()
        //{
        //    return PartialView("_Partial_EditContactHistory", new tbl_ContactHistory());
        //}

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EditContactHistory(int id)
        {
            var m = await _contactHistoryRepository.GetById(id);
            return PartialView("_Partial_EditContactHistory", await _contactHistoryRepository.GetById(id));
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateContactHistory(tbl_ContactHistory model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 56);
                model.ModifiedDate = DateTime.Now;
                model.OtherStaffId = model.StaffId;
                if (await _contactHistoryRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(56, "Cập nhật lịch sử liên hệ: " + model.Request + ", khách hàng: " + _customerRepository.FindId(model.CustomerId).Code,
                            null, //appointment
                            model.ContractId, //contract
                            model.CustomerId, //customer
                            model.PartnerId, //partner
                            model.ProgramId, //program
                            null, //task
                            model.TourId, //tour
                            null, //quotation
                            null, //document
                            model.Id, //history
                        null // ticket
                            );

                    var list = _db.tbl_ContactHistory.AsEnumerable()
                        .Where(p => p.CustomerId == model.CustomerId).Where(p => p.IsDelete == false)
                        .OrderByDescending(p => p.CreatedDate)
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
                    return PartialView("~/Views/CustomerTabInfo/_LichSuLienHe.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_LichSuLienHe.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_LichSuLienHe.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteContactHistory(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 56);
                int cusId = _contactHistoryRepository.FindId(id).CustomerId ?? 0;
                var listId = id.ToString().Split(',').ToArray();
                //
                var item = _contactHistoryRepository.FindId(id);
                UpdateHistory.SaveHistory(56, "Xóa lịch sử liên hệ: " + item.Request + ", khách hàng: " + _staffRepository.FindId(item.CustomerId).Code,
                            null, //appointment
                            item.ContractId, //contract
                            item.CustomerId, //customer
                            item.PartnerId, //partner
                            item.ProgramId, //program
                            null, //task
                            item.TourId, //tour
                            null, //quotation
                            null, //document
                            item.Id, //history
                        null // ticket
                            );
                //
                if (await _contactHistoryRepository.DeleteMany(listId, false))
                {
                    var list = _db.tbl_ContactHistory.AsEnumerable()
                        .Where(p => p.CustomerId == cusId).Where(p => p.IsDelete == false)
                        .OrderByDescending(p => p.CreatedDate)
                        .Select(p => new tbl_ContactHistory
                        {
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
                    return PartialView("~/Views/CustomerTabInfo/_LichSuLienHe.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_LichSuLienHe.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_LichSuLienHe.cshtml");
            }
        }
        #endregion

        #region Visa

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> InsertTourVisa(tbl_TourCustomerVisa model)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 54);
                var cus = _customerVisaRepository.FindId(model.CustomerId);
                int idCus = cus.CustomerId;
                var cusTour = _tourCustomerRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.TourId == model.TourId && c.CustomerId == idCus).SingleOrDefault();
                var tourVisa = _tourCustomerVisaRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.CustomerId == model.CustomerId && c.TourId == model.TourId).SingleOrDefault();
                if (cusTour != null && tourVisa == null)
                {
                    await _tourCustomerVisaRepository.Create(model);

                    UpdateHistory.SaveHistory(54, "Thêm mới visa cho khách hàng: " + cus.tbl_Customer.Code
                        + " (VISA: " + tourVisa.tbl_CustomerVisa.VisaNumber + ") vào tour " + tourVisa.tbl_Tour.Name,
                            null, //appointment
                            null, //contract
                            model.CustomerId, //customer
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

                var list = _customerVisaRepository.GetAllAsQueryable().Where(p => p.IsDelete == false && p.CustomerId == idCus).ToList();
                return PartialView("~/Views/CustomerTabInfo/_Visa.cshtml", list);
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_Visa.cshtml");
            }
        }
        [HttpPost]
        public async Task<ActionResult> TourVisa(int id)
        {
            var model = new tbl_TourCustomerVisa
            {
                CustomerId = id
            };
            return PartialView("_Partial_InsertTourVisa", model);
        }
        #endregion

        #region Người liên hệ
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateContact(tbl_CustomerContact model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1097);
                Random rd = new Random();
                model.Code = rd.Next(1111, 9999).ToString();
                model.CustomerId = Convert.ToInt32(Session["idCustomer"].ToString());
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.IsDelete = false;
                model.TagsId = form["TagsId"];

                if (await _customerContactRepository.Create(model))
                {
                    UpdateHistory.SaveHistory(1097, "Thêm mới người liên hệ: " + model.FullName
                        + " của khách hàng " + _customerRepository.FindId(model.CustomerId).Code,
                            null, //appointment
                            null, //contract
                            model.CustomerId, //customer
                            null, //partner
                            null, //program
                            null, //task
                            null, //tour
                            null, //quotation
                            null, //document
                            null, //history
                        null // ticket
                            );

                    var list = _customerContactRepository.GetAllAsQueryable().AsEnumerable()
                                .Where(p => p.CustomerId == model.CustomerId).Where(p => p.IsDelete == false)
                                .OrderByDescending(p => p.CreatedDate)
                                .Select(p => new tbl_CustomerContact
                                {
                                    Id = p.Id,
                                    FullName = p.FullName,
                                    Address = p.Address,
                                    Mobile = p.Mobile,
                                    IdentityCard = p.IdentityCard,
                                    PassportCard = p.PassportCard,
                                    CompanyPhone = p.CompanyPhone,
                                    TagsId = p.TagsId,
                                    Email = p.Email,
                                    Birthday = p.Birthday,
                                    Position = p.Position
                                }).ToList();
                    return PartialView("~/Views/CustomerTabInfo/_NguoiLienHe.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_NguoiLienHe.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_NguoiLienHe.cshtml");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EditContact(int id)
        {
            var model = await _customerContactRepository.GetById(id);
            return PartialView("_Partial_EditContact", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateContact(tbl_CustomerContact model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1097);
                model.ModifiedDate = DateTime.Now;
                model.TagsId = form["TagsId"];

                if (await _customerContactRepository.Update(model))
                {
                    var list = _customerContactRepository.GetAllAsQueryable().AsEnumerable()
                                .Where(p => p.CustomerId == model.CustomerId).Where(p => p.IsDelete == false)
                                .OrderByDescending(p => p.CreatedDate)
                                .Select(p => new tbl_CustomerContact
                                {
                                    Id = p.Id,
                                    FullName = p.FullName,
                                    Address = p.Address,
                                    Mobile = p.Mobile,
                                    IdentityCard = p.IdentityCard,
                                    PassportCard = p.PassportCard,
                                    CompanyPhone = p.CompanyPhone,
                                    TagsId = p.TagsId,
                                    Email = p.Email,
                                    Birthday = p.Birthday,
                                    Position = p.Position
                                }).ToList();
                    return PartialView("~/Views/CustomerTabInfo/_NguoiLienHe.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_NguoiLienHe.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_NguoiLienHe.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteContact(int id)
        {
            int cusId = _customerContactRepository.FindId(id).CustomerId;
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1097);
                var listId = id.ToString().Split(',').ToArray();
                //
                var item = _customerContactRepository.FindId(id);
                UpdateHistory.SaveHistory(1097, "Xóa người liên liên hệ: " + item.FullName + ", khách hàng: " + _staffRepository.FindId(item.CustomerId).Code,
                            null, //appointment
                            null, //contract
                            item.CustomerId, //customer
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
                if (await _customerContactRepository.DeleteMany(listId, false))
                {
                    var list = _customerContactRepository.GetAllAsQueryable().AsEnumerable()
                                .Where(p => p.CustomerId == cusId).Where(p => p.IsDelete == false)
                                .OrderByDescending(p => p.CreatedDate)
                                .Select(p => new tbl_CustomerContact
                                {
                                    Id = p.Id,
                                    FullName = p.FullName,
                                    Address = p.Address,
                                    Mobile = p.Mobile,
                                    IdentityCard = p.IdentityCard,
                                    PassportCard = p.PassportCard,
                                    CompanyPhone = p.CompanyPhone,
                                    TagsId = p.TagsId,
                                    Email = p.Email,
                                    Birthday = p.Birthday,
                                    Position = p.Position
                                }).ToList();
                    return PartialView("~/Views/CustomerTabInfo/_NguoiLienHe.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_NguoiLienHe.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_NguoiLienHe.cshtml");
            }
        }
        #endregion

        #region Hợp đồng

        [HttpPost]
        public ActionResult CheckCodeContract(string code)
        {
            var check = _contractRepository.GetAllAsQueryable().FirstOrDefault(p => p.Code == code);
            if (check != null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadFileContract(HttpPostedFileBase FileNameContract)
        {
            if (FileNameContract != null && FileNameContract.ContentLength > 0)
            {
                Session["ContractCustomerFile"] = FileNameContract;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateContract(tbl_Contract model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1104);

                var checkcode = _contractRepository.GetAllAsQueryable().FirstOrDefault(p => p.Code == model.Code && p.IsDelete == false);
                if (checkcode == null)
                {
                    model.CustomerId = Convert.ToInt32(Session["idCustomer"].ToString());
                    model.CreatedDate = DateTime.Now;
                    model.ModifiedDate = DateTime.Now;
                    model.IsDelete = false;
                    model.DictionaryId = 28;
                    model.StaffId = clsPermission.GetUser().StaffID;
                    model.LoiNhuanDuKien = model.TotalPrice - model.TongDuKien;
                    model.CurrencyLNDK = model.CurrencyTDK;
                    if (await _contractRepository.Create(model))
                    {
                        UpdateHistory.SaveHistory(1104, "Thêm mới hợp đồng: " + model.Code + " - " + model.Name + ", khách hàng: " + _customerRepository.FindId(model.CustomerId).Code,
                            null, //appointment
                            model.Id, //contract
                            model.CustomerId, //customer
                            null, //partner
                            null, //program
                            null, //task
                            model.TourId, //tour
                            null, //quotation
                            null, //document
                            null, //history
                        null // ticket
                            );

                        // upload file
                        if (Session["ContractCustomerFile"] != null && Session["ContractCustomerFile"] != "")
                        {
                            HttpPostedFileBase FileName = Session["ContractCustomerFile"] as HttpPostedFileBase;
                            string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                            String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                            String path = Server.MapPath("~/Upload/file/" + newName);
                            FileName.SaveAs(path);
                            if (newName != null && FileSize != null)
                            {
                                var file = new tbl_DocumentFile
                                {
                                    Code = GenerateCode.DocumentCode(),
                                    ContractId = model.Id,
                                    CreatedDate = DateTime.Now,
                                    CustomerId = model.CustomerId,
                                    DictionaryId = 28,
                                    FileName = newName,
                                    FileSize = FileSize,
                                    IsCustomer = false,
                                    IsDelete = false,
                                    IsRead = false,
                                    ModifiedDate = DateTime.Now,
                                    PermissionStaff = model.StaffId.ToString(),
                                    StaffId = model.StaffId,
                                    TagsId = form["TagsId"] == null ? null : form["TagsId"].ToString(),
                                    TourId = model.TourId
                                };
                                await _documentFileRepository.Create(file);
                                Session["ContractCustomerFile"] = null;
                            }
                        }
                    }
                }

                //
                var docs = _documentFileRepository.GetAllAsQueryable();
                var list = _contractRepository.GetAllAsQueryable().AsEnumerable()
                    .Where(p => p.CustomerId == model.CustomerId && p.IsDelete == false)
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
                        tbl_DictionaryCurrency = _dictionaryRepository.FindId(p.CurrencyId),
                        tbl_DictionaryCurrencyLNDK = _dictionaryRepository.FindId(p.CurrencyLNDK),
                        tbl_DictionaryCurrencyTDK = _dictionaryRepository.FindId(p.CurrencyTDK),
                        tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusContractId),
                        tbl_Staff = _staffRepository.FindId(p.StaffId),
                        TongDuKien = p.TongDuKien,
                        TotalPrice = p.TotalPrice,
                        DocumentId = docs.FirstOrDefault(x => x.ContractId == p.Id) != null ? docs.FirstOrDefault(x => x.ContractId == p.Id).Id : 0,
                        FileName = docs.FirstOrDefault(x => x.ContractId == p.Id) != null ? docs.FirstOrDefault(x => x.ContractId == p.Id).FileName : ""
                    }).ToList();
                return PartialView("~/Views/CustomerTabInfo/_HopDong.cshtml", list);

            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_HopDong.cshtml");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EditContract(int id, int docId)
        {

            var model = await _contractRepository.GetById(id);
            ViewBag.DocId = docId;
            if (clsPermission.GetUser().StaffID == 9 || model.StatusContractId <= 1155)
            {
                return PartialView("_Partial_EditContract", model);
            }
            else
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateContract(tbl_Contract model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1104);
                model.ModifiedDate = DateTime.Now;
                model.LoiNhuanDuKien = model.TotalPrice - model.TongDuKien;
                model.CurrencyLNDK = model.CurrencyTDK;
                if (await _contractRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(1104, "Cập nhật hợp đồng: " + model.Code + ", khách hàng: " + _customerRepository.FindId(model.CustomerId).Code,
                            null, //appointment
                            model.Id, //contract
                            model.CustomerId, //customer
                            null, //partner
                            null, //program
                            null, //task
                            model.TourId, //tour
                            null, //quotation
                            null, //document
                            null, //history
                        null // ticket
                            );
                    // file name
                    // upload filele
                    if (Session["ContractCustomerFile"] != null && Session["ContractCustomerFile"] != "")
                    {
                        if (form["docId"] != "0")
                        {
                            HttpPostedFileBase FileName = Session["ContractCustomerFile"] as HttpPostedFileBase;
                            if (FileName != null)
                            {
                                string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                                String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                                String path = Server.MapPath("~/Upload/file/" + newName);
                                FileName.SaveAs(path);

                                var doc = _db.tbl_DocumentFile.Find(Convert.ToInt32(form["docId"]));
                                doc.FileName = newName;
                                doc.FileSize = FileSize;
                                doc.ModifiedDate = DateTime.Now;
                                doc.TagsId = form["TagsId"] == null ? null : form["TagsId"].ToString();
                                _db.SaveChanges();
                            }
                        }
                        else
                        {
                            HttpPostedFileBase FileName = Session["ContractCustomerFile"] as HttpPostedFileBase;
                            if (FileName != null)
                            {
                                string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                                String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                                String path = Server.MapPath("~/Upload/file/" + newName);
                                FileName.SaveAs(path);

                                var file = new tbl_DocumentFile
                                {
                                    Code = GenerateCode.DocumentCode(),
                                    ContractId = model.Id,
                                    CreatedDate = DateTime.Now,
                                    CustomerId = _tourRepository.FindId(model.TourId).CustomerId,
                                    DictionaryId = 28,
                                    FileName = newName,
                                    FileSize = FileSize,
                                    IsCustomer = false,
                                    IsDelete = false,
                                    IsRead = false,
                                    ModifiedDate = DateTime.Now,
                                    PermissionStaff = model.StaffId.ToString(),
                                    StaffId = model.StaffId,
                                    TagsId = form["TagsId"] == null ? null : form["TagsId"].ToString(),
                                    TourId = model.TourId
                                };
                                await _documentFileRepository.Create(file);
                            }
                            Session["ContractCustomerFile"] = null;
                        }
                    }
                    //

                    var docs = _documentFileRepository.GetAllAsQueryable();
                    var list = _contractRepository.GetAllAsQueryable().AsEnumerable()
                        .Where(p => p.CustomerId == model.CustomerId && p.IsDelete == false)
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
                            tbl_DictionaryCurrency = _dictionaryRepository.FindId(p.CurrencyId),
                            tbl_DictionaryCurrencyLNDK = _dictionaryRepository.FindId(p.CurrencyLNDK),
                            tbl_DictionaryCurrencyTDK = _dictionaryRepository.FindId(p.CurrencyTDK),
                            tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusContractId),
                            tbl_Staff = _staffRepository.FindId(p.StaffId),
                            TongDuKien = p.TongDuKien,
                            TotalPrice = p.TotalPrice,
                            DocumentId = docs.FirstOrDefault(x => x.ContractId == p.Id) != null ? docs.FirstOrDefault(x => x.ContractId == p.Id).Id : 0,
                            FileName = docs.FirstOrDefault(x => x.ContractId == p.Id) != null ? docs.FirstOrDefault(x => x.ContractId == p.Id).FileName : ""
                        }).ToList();
                    return PartialView("~/Views/CustomerTabInfo/_HopDong.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_HopDong.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_HopDong.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteContract(int id)
        {
            int cusId = _contractRepository.FindId(id).CustomerId ?? 0;
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1104);
                var history = _updateHistoryRepository.GetAllAsQueryable().Where(p => p.ContractId == id).Where(p => p.IsDelete == false).ToList();
                if (history.Count() > 0)
                {
                    foreach (var item in history)
                    {
                        var list = item.Id.ToString().Split(',').ToArray();
                        await _updateHistoryRepository.DeleteMany(list, false);
                    }
                }
                //
                var items = _contractRepository.FindId(id);
                UpdateHistory.SaveHistory(1104, "Xóa hợp đồng: " + items.Code + " - " + items.Name + ", khách hàng: " + _staffRepository.FindId(items.CustomerId).Code,
                            null, //appointment
                            items.Id, //contract
                            items.CustomerId, //customer
                            null, //partner
                            null, //program
                            null, //task
                            items.TourId, //tour
                            null, //quotation
                            null, //document
                            null, //history
                        null // ticket
                            );
                //
                var listId = id.ToString().Split(',').ToArray();
                if (await _contractRepository.DeleteMany(listId, false))
                {
                    var docs = _documentFileRepository.GetAllAsQueryable();
                    var list = _contractRepository.GetAllAsQueryable().AsEnumerable()
                        .Where(p => p.CustomerId == cusId && p.IsDelete == false)
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
                            tbl_DictionaryCurrency = _dictionaryRepository.FindId(p.CurrencyId),
                            tbl_DictionaryCurrencyLNDK = _dictionaryRepository.FindId(p.CurrencyLNDK),
                            tbl_DictionaryCurrencyTDK = _dictionaryRepository.FindId(p.CurrencyTDK),
                            tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusContractId),
                            tbl_Staff = _staffRepository.FindId(p.StaffId),
                            TongDuKien = p.TongDuKien,
                            TotalPrice = p.TotalPrice,
                            DocumentId = docs.FirstOrDefault(x => x.ContractId == p.Id) != null ? docs.FirstOrDefault(x => x.ContractId == p.Id).Id : 0,
                            FileName = docs.FirstOrDefault(x => x.ContractId == p.Id) != null ? docs.FirstOrDefault(x => x.ContractId == p.Id).FileName : ""
                        }).ToList();
                    return PartialView("~/Views/CustomerTabInfo/_HopDong.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_HopDong.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_HopDong.cshtml");
            }
        }
        #endregion

        #region Chương trình

        [HttpPost]
        public ActionResult UploadFileProgram(HttpPostedFileBase FileNameProgram)
        {
            if (FileNameProgram != null && FileNameProgram.ContentLength > 0)
            {
                Session["ProgramCustomerFile"] = FileNameProgram;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateProgram(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1106);
                var ct = new tbl_Program();
                string FileSize = "", newName = "";
                var program = _programRepository.GetAllAsQueryable().AsEnumerable()
                    .FirstOrDefault(p => p.Code == form["Code"] && p.Id.ToString() == form["Name"]);
                if (Session["ProgramCustomerFile"] != null)
                {
                    HttpPostedFileBase FileName = Session["ProgramCustomerFile"] as HttpPostedFileBase;
                    FileSize = Common.ConvertFileSize(FileName.ContentLength);
                    newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    FileName.SaveAs(path);
                }

                if (program != null)
                {
                    // update chương trình
                    ct = _db.tbl_Program.Find(program.Id);
                    ct.CustomerId = Convert.ToInt32(Session["idCustomer"].ToString());
                    ct.ModifiedDate = DateTime.Now;
                    ct.StatusId = Convert.ToInt32(form["StatusId"]);
                    ct.StaffId = clsPermission.GetUser().StaffID;
                    ct.TourId = model.TourId;
                    ct.TagsId = model.TagsId;
                    ct.Note = model.Note;
                    if (newName != "")
                    {
                        ct.FileName = newName;
                        ct.FileSize = FileSize;
                    }
                    _db.SaveChanges();
                }
                else
                {
                    // insert chương trình
                    ct = new tbl_Program
                    {
                        Code = GenerateCode.ProgramCode(),
                        TourId = model.TourId,
                        CreatedDate = DateTime.Now,
                        CurrencyId = 1209,
                        CustomerId = Convert.ToInt32(Session["idCustomer"].ToString()),
                        DictionaryId = 30,
                        IsDelete = false,
                        ModifiedDate = DateTime.Now,
                        Name = form["Name"],
                        Note = model.Note,
                        StaffId = clsPermission.GetUser().StaffID,
                        StatusId = Convert.ToInt32(form["StatusId"])
                    };
                    if (newName != "")
                    {
                        ct.FileName = newName;
                        ct.FileSize = FileSize;
                    }
                    if (model.TourId != null)
                    {
                        var tour = _tourRepository.FindId(model.TourId);
                        ct.StartDate = tour != null ? tour.StartDate : null;
                        ct.EndDate = tour != null ? tour.EndDate : null;
                    }
                    await _programRepository.Create(ct);
                    UpdateHistory.SaveHistory(1106, "Thêm mới chương trình: " + ct.Code + " - " + ct.Name + ", khách hàng: " + (_customerRepository.FindId(model.CustomerId) != null ? _customerRepository.FindId(model.CustomerId).Code : ""),
                            null, //appointment
                            null, //contract
                            ct.CustomerId, //customer
                            null, //partner
                            ct.Id, //program
                            null, //task
                            ct.TourId, //tour
                            null, //quotation
                            null, //document
                            null, //history
                        null // ticket
                            );
                }

                // insert tài liệu
                if (Session["ProgramCustomerFile"] != null)
                {
                    model.CustomerId = Convert.ToInt32(Session["idCustomer"].ToString());
                    model.CreatedDate = DateTime.Now;
                    model.ModifiedDate = DateTime.Now;
                    model.Code = GenerateCode.DocumentCode();
                    model.ProgramId = ct.Id;
                    model.TourId = model.TourId;
                    model.DictionaryId = 30;
                    model.IsRead = false;
                    model.IsDelete = false;
                    model.StaffId = clsPermission.GetUser().StaffID;
                    model.PermissionStaff = model.StaffId.ToString();
                    if (form["TagsId"] != null && form["TagsId"] != "")
                    {
                        model.TagsId = form["TagsId"].ToString();
                    }
                    //end file
                    if (newName != "" && FileSize != "")
                    {
                        model.FileName = newName;
                        model.FileSize = FileSize;
                    }
                    await _documentFileRepository.Create(model);
                    Session["ProgramCustomerFile"] = null;
                }
                var list = _db.tbl_Program.AsEnumerable()
                                .Where(p => p.CustomerId == ct.CustomerId && p.IsDelete == false)
                                .OrderByDescending(p => p.CreatedDate)
                                .Select(p => new tbl_Program()
                                {
                                    Id = p.Id,
                                    Name = p.Name,
                                    CreatedDate = p.CreatedDate,
                                    ModifiedDate = p.ModifiedDate,
                                    FileSize = p.FileSize,
                                    FileName = p.FileName,
                                    TagsId = p.TagsId,
                                    StaffId = p.StaffId,
                                    tbl_Staff = _staffRepository.FindId(p.StaffId),
                                    Code = p.Code,
                                    Note = p.Note
                                }).ToList();
                return PartialView("~/Views/CustomerTabInfo/_ChuongTrinh.cshtml", list);
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_ChuongTrinh.cshtml");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EditProgram(int id)
        {
            var model = await _programRepository.GetById(id);
            if (clsPermission.GetUser().StaffID == 9 || model.DictionaryId <= 1147)
            {
                return PartialView("_Partial_EditProgram", model);
            }
            else
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateProgram(tbl_Program model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1106);
                string FileSize = "", newName = "";
                if (Session["ProgramCustomerFile"] != null)
                {
                    //file
                    HttpPostedFileBase FileName = Session["ProgramCustomerFile"] as HttpPostedFileBase;
                    FileSize = Common.ConvertFileSize(FileName.ContentLength);
                    newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    FileName.SaveAs(path);
                }

                var program = _db.tbl_Program.Find(model.Id);
                // chương trình
                program.Name = form["Name"];
                program.TourId = model.TourId;
                program.StatusId = Convert.ToInt32(form["StatusId"]);
                program.ModifiedDate = DateTime.Now;
                program.Note = model.Note;
                if (newName != "")
                {
                    program.FileSize = FileSize;
                    program.FileName = newName;
                }
                _db.SaveChanges();
                if (Session["ProgramCustomerFile"] != null)
                {
                    //file
                    HttpPostedFileBase FileName = Session["ProgramCustomerFile"] as HttpPostedFileBase;
                    FileSize = Common.ConvertFileSize(FileName.ContentLength);
                    newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    FileName.SaveAs(path);
                    //end file
                    if (FileName != null && FileSize != null)
                    {
                        var doc = new tbl_DocumentFile
                        {
                            FileName = newName,
                            FileSize = FileSize,
                            Code = GenerateCode.DocumentCode(),
                            CreatedDate = DateTime.Now,
                            CustomerId = model.CustomerId,
                            DictionaryId = Convert.ToInt32(form["StatusId"]),
                            IsDelete = false,
                            IsRead = false,
                            ModifiedDate = DateTime.Now,
                            Note = form["Note"],
                            PermissionStaff = clsPermission.GetUser().StaffID.ToString(),
                            ProgramId = model.Id,
                            StaffId = clsPermission.GetUser().StaffID,
                            TagsId = form["TagsId"],
                            TourId = model.TourId
                        };
                        if (await _documentFileRepository.Create(doc))
                        {
                            UpdateHistory.SaveHistory(1106, "Thêm mới tài liệu chương trình: " + model.Code + ", khách hàng: " + _customerRepository.FindId(model.CustomerId).Code,
                                null, //appointment
                                null, //contract
                                model.CustomerId, //customer
                                null, //partner
                                model.Id, //program
                                null, //task
                                model.TourId, //tour
                                null, //quotation
                                doc.Id, //document
                                null, //history
                        null // ticket
                                );
                        }
                    }
                }
                var list = _db.tbl_Program.AsEnumerable()
                                        .Where(p => p.CustomerId == model.CustomerId && p.IsDelete == false)
                                        .OrderByDescending(p => p.CreatedDate)
                                        .Select(p => new tbl_Program()
                                        {
                                            Id = p.Id,
                                            Name = p.Name,
                                            CreatedDate = p.CreatedDate,
                                            ModifiedDate = p.ModifiedDate,
                                            FileSize = p.FileSize,
                                            FileName = p.FileName,
                                            TagsId = p.TagsId,
                                            StaffId = p.StaffId,
                                            tbl_Staff = _staffRepository.FindId(p.StaffId),
                                            Code = p.Code,
                                            Note = p.Note
                                        }).ToList();
                return PartialView("~/Views/CustomerTabInfo/_ChuongTrinh.cshtml", list);
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_ChuongTrinh.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteProgram(int id)
        {
            var program = _programRepository.FindId(id);
            int cusId = program.CustomerId ?? 0;
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1106);
                //file
                tbl_DocumentFile documentFile = _documentFileRepository.GetAllAsQueryable().FirstOrDefault(p => p.FileName == program.FileName) ?? new tbl_DocumentFile();
                if (documentFile != null)
                {
                    await _documentFileRepository.DeleteMany(documentFile.Id.ToString().Split(',').ToArray(), false);
                    String path = Server.MapPath("~/Upload/file/" + documentFile.FileName);
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }
                //end file
                //
                UpdateHistory.SaveHistory(1106, "Xóa chương trình: " + program.Code + " - " + program.Name + ", khách hàng: " + _staffRepository.FindId(program.StaffId).Code,
                            null, //appointment
                            null, //contract
                            program.CustomerId, //customer
                            null, //partner
                            program.Id, //program
                            null, //task
                            program.TourId, //tour
                            null, //quotation
                            null, //document
                            null, //history
                        null // ticket
                            );
                //
                if (await _programRepository.DeleteMany(id.ToString().Split(',').ToArray(), false))
                {
                    var list = _db.tbl_Program.AsEnumerable()
                                .Where(p => p.CustomerId == cusId && p.IsDelete == false)
                                .OrderByDescending(p => p.CreatedDate)
                                .Select(p => new tbl_Program()
                                {
                                    Id = p.Id,
                                    Name = p.Name,
                                    CreatedDate = p.CreatedDate,
                                    ModifiedDate = p.ModifiedDate,
                                    FileSize = p.FileSize,
                                    FileName = p.FileName,
                                    TagsId = p.TagsId,
                                    StaffId = p.StaffId,
                                    tbl_Staff = _staffRepository.FindId(p.StaffId),
                                    Code = p.Code,
                                    Note = p.Note
                                }).ToList();
                    return PartialView("~/Views/CustomerTabInfo/_ChuongTrinh.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_ChuongTrinh.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_ChuongTrinh.cshtml");
            }
        }

        public JsonResult ProgramListInTour(int id)
        {
            return Json(new SelectList(_programRepository.GetAllAsQueryable()
                .Where(p => p.TourId == id && p.IsDelete == false && p.CustomerId == null || p.CustomerId == Convert.ToInt32(Session["idCustomer"].ToString())).ToList(), "Id", "Name")
                , JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LoadProgram(int? id)
        {
            if (id == null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var model = _programRepository.FindId(id);
                return Json(new tbl_Program
                {
                    Code = model.Code,
                    Id = model.Id,
                    StatusId = model.StatusId,
                    TourId = model.TourId
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Báo giá

        [HttpPost]
        public ActionResult UploadMultipleFileQuotation(IEnumerable<HttpPostedFileBase> FileNameQuotation)
        {
            if (FileNameQuotation != null)
            {
                Session["QuotationCustomerFile"] = FileNameQuotation;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadFileQuotation(HttpPostedFileBase FileNameQuotation)
        {
            if (FileNameQuotation != null && FileNameQuotation.ContentLength > 0)
            {
                Session["QuotationCustomerFile"] = FileNameQuotation;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateQuotation(tbl_Quotation model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1107);
                model.CustomerId = Convert.ToInt32(Session["idCustomer"].ToString());
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.IsDelete = false;
                model.StaffId = clsPermission.GetUser().StaffID;
                model.StartDate = DateTime.Now;
                model.EndDate = DateTime.Now;
                model.DictionaryId = 29;
                model.Code = GenerateCode.QuotationCode();
                model.QuotationDate = form["QuotationDate"] != "" ? Convert.ToDateTime(form["QuotationDate"].ToString()) : DateTime.Now;

                if (Session["QuotationCustomerFile"] != null)
                {
                    //file
                    IEnumerable<HttpPostedFileBase> FileName = Session["QuotationCustomerFile"] as IEnumerable<HttpPostedFileBase>;
                    foreach (var file in FileName)
                    {
                        string FileSize = Common.ConvertFileSize(file.ContentLength);
                        String newName = file.FileName.Insert(file.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                        String path = Server.MapPath("~/Upload/file/" + newName);
                        file.SaveAs(path);
                        //end file

                        if (file != null && FileSize != null)
                        {
                            String pathOld = Server.MapPath("~/Upload/file/" + model.FileName);
                            if (System.IO.File.Exists(pathOld))
                                System.IO.File.Delete(pathOld);
                            model.FileName = newName;
                        }
                        await _quotationRepository.Create(model);
                        UpdateHistory.SaveHistory(1107, "Thêm mới báo giá, code: " + model.Code + ", khách hàng: " + _customerRepository.FindId(model.CustomerId).Code,
                                null, //appointment
                                null, //contract
                                model.CustomerId, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                model.Id, //quotation
                                null, //document
                                null, //history
                        null // ticket
                                );
                    }
                    Session["QuotationCustomerFile"] = null;
                }

                var list = _quotationRepository.GetAllAsQueryable().AsEnumerable()
                       .Where(p => p.CustomerId == model.CustomerId && p.IsDelete == false)
                       .OrderByDescending(p => p.CreatedDate)
                       .Select(p => new tbl_Quotation
                       {
                           Id = p.Id,
                           Code = p.Code,
                           QuotationDate = p.QuotationDate,
                           tbl_StaffQuotation = _staffRepository.FindId(p.StaffQuotationId),
                           tbl_Staff = _staffRepository.FindId(p.StaffId),
                           PriceTour = p.PriceTour,
                           tbl_DictionaryCurrency = _dictionaryRepository.FindId(p.CurrencyId),
                           FileName = p.FileName,
                           Note = p.Note,
                           CreatedDate = p.CreatedDate,
                           ModifiedDate = p.ModifiedDate,
                           StaffId = p.StaffId,
                           StaffQuotationId = p.StaffQuotationId,
                           CountryId = p.CountryId,
                           tbl_TagsCountry = _tagsRepository.FindId(p.CountryId)
                       }).ToList();
                return PartialView("~/Views/CustomerTabInfo/_BaoGia.cshtml", list);
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_BaoGia.cshtml");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EditQuotation(int id)
        {
            var model = await _quotationRepository.GetById(id);
            return PartialView("_Partial_EditQuotation", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateQuotation(tbl_Quotation model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1107);
                model.ModifiedDate = DateTime.Now;

                if (Session["QuotationCustomerFile"] != null)
                {
                    //file
                    HttpPostedFileBase FileName = Session["QuotationCustomerFile"] as HttpPostedFileBase;
                    string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                    String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    FileName.SaveAs(path);
                    //end file

                    if (FileName != null && FileSize != null)
                    {
                        String pathOld = Server.MapPath("~/Upload/file/" + model.FileName);
                        if (System.IO.File.Exists(pathOld))
                            System.IO.File.Delete(pathOld);
                        model.FileName = newName;
                    }
                }

                if (await _quotationRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(1107, "Cập nhật báo giá: " + model.Code + ", khách hàng: " + _customerRepository.FindId(model.CustomerId).Code,
                                null, //appointment
                                null, //contract
                                model.CustomerId, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                model.Id, //quotation
                                null, //document
                                null, //history
                        null // ticket
                                );
                    var list = _quotationRepository.GetAllAsQueryable().AsEnumerable()
                        .Where(p => p.CustomerId == model.CustomerId && p.IsDelete == false)
                        .OrderByDescending(p => p.CreatedDate)
                        .Select(p => new tbl_Quotation
                        {
                            Id = p.Id,
                            Code = p.Code,
                            QuotationDate = p.QuotationDate,
                            tbl_StaffQuotation = _staffRepository.FindId(p.StaffQuotationId),
                            tbl_Staff = _staffRepository.FindId(p.StaffId),
                            PriceTour = p.PriceTour,
                            tbl_DictionaryCurrency = _dictionaryRepository.FindId(p.CurrencyId),
                            FileName = p.FileName,
                            Note = p.Note,
                            CreatedDate = p.CreatedDate,
                            ModifiedDate = p.ModifiedDate,
                            StaffId = p.StaffId,
                            StaffQuotationId = p.StaffQuotationId,
                            CountryId = p.CountryId,
                            tbl_TagsCountry = _tagsRepository.FindId(p.CountryId)
                        }).ToList();
                    return PartialView("~/Views/CustomerTabInfo/_BaoGia.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_BaoGia.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_BaoGia.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteQuotation(int id)
        {
            int cusId = _quotationRepository.FindId(id).CustomerId ?? 0;
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1107);
                //file
                tbl_Quotation documentFile = _quotationRepository.FindId(id) ?? new tbl_Quotation();
                String path = Server.MapPath("~/Upload/file/" + documentFile.FileName);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                //end file
                var listId = id.ToString().Split(',').ToArray();
                //
                UpdateHistory.SaveHistory(1107, "Xóa báo giá: " + documentFile.Code + ", khách hàng: " + _staffRepository.FindId(documentFile.CustomerId).Code,
                                null, //appointment
                                null, //contract
                                documentFile.CustomerId, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                documentFile.Id, //quotation
                                null, //document
                                null, //history
                        null // ticket
                                );
                //
                if (await _quotationRepository.DeleteMany(listId, false))
                {
                    var list = _quotationRepository.GetAllAsQueryable().AsEnumerable()
                        .Where(p => p.CustomerId == cusId && p.IsDelete == false)
                        .OrderByDescending(p => p.CreatedDate)
                        .Select(p => new tbl_Quotation
                        {
                            Id = p.Id,
                            Code = p.Code,
                            QuotationDate = p.QuotationDate,
                            tbl_StaffQuotation = _staffRepository.FindId(p.StaffQuotationId),
                            tbl_Staff = _staffRepository.FindId(p.StaffId),
                            PriceTour = p.PriceTour,
                            tbl_DictionaryCurrency = _dictionaryRepository.FindId(p.CurrencyId),
                            FileName = p.FileName,
                            Note = p.Note,
                            CreatedDate = p.CreatedDate,
                            ModifiedDate = p.ModifiedDate,
                            StaffId = p.StaffId,
                            StaffQuotationId = p.StaffQuotationId,
                            CountryId = p.CountryId,
                            tbl_TagsCountry = _tagsRepository.FindId(p.CountryId)
                        }).ToList();
                    return PartialView("~/Views/CustomerTabInfo/_BaoGia.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_BaoGia.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_BaoGia.cshtml");
            }
        }
        #endregion

        #region Code vé

        [HttpPost]
        public ActionResult CheckCodeTicket(string code)
        {
            var check = _ticketRepository.GetAllAsQueryable().FirstOrDefault(p => p.Code == code);
            if (check != null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateTicket(tbl_Ticket model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1105);
                var checkcode = _ticketRepository.GetAllAsQueryable().FirstOrDefault(p => p.Code == model.Code && p.IsDelete == false);
                if (checkcode == null)
                {
                    model.CustomerId = Convert.ToInt32(Session["idCustomer"].ToString());
                    model.CreateDate = DateTime.Now;
                    model.ModifyDate = DateTime.Now;
                    model.IsDelete = false;
                    model.Staff = clsPermission.GetUser().StaffID;
                    //
                    var cus = _customerRepository.FindId(model.CustomerId);
                    model.Name = cus.FullName;
                    model.Phone = cus.Phone;
                    await _ticketRepository.Create(model);

                    UpdateHistory.SaveHistory(1105, "Thêm mới vé, code: " + model.Code + ", khách hàng: " + _customerRepository.FindId(model.CustomerId).Code,
                                null, //appointment
                                null, //contract
                                model.CustomerId, //customer
                                null, //partner
                                null, //program
                                null, //task
                                model.TourId, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                model.Id // ticket
                                );
                    //
                }
                var list = _ticketRepository.GetAllAsQueryable().AsEnumerable()
                            .Where(p => p.CustomerId == model.CustomerId && p.IsDelete == false)
                            .OrderByDescending(p => p.CreateDate)
                            .Select(p => new tbl_Ticket()
                            {
                                Id = p.Id,
                                Code = p.Code,
                                CurrencyId = p.CurrencyId,
                                EndDate = p.EndDate,
                                EndPlace = p.EndPlace,
                                Price = p.Price,
                                StartDate = p.StartDate,
                                StartPlace = p.StartPlace,
                                StatusId = p.StatusId,
                                TypeId = p.TypeId,
                                TourId = p.TourId,
                                tbl_TagsStart = _tagsRepository.FindId(p.StartPlace),
                                tbl_TagsEnd = _tagsRepository.FindId(p.EndPlace),
                                tbl_DictionaryCurrency = _dictionaryRepository.FindId(p.CurrencyId),
                                tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                                tbl_DictionaryType = _dictionaryRepository.FindId(p.TypeId),
                                tbl_Tour = _tourRepository.FindId(p.TourId),
                                CreateDate = p.CreateDate,
                                Staff = p.Staff,
                                tbl_Staff = _staffRepository.FindId(p.Staff)
                            }).ToList();
                return PartialView("~/Views/CustomerTabInfo/_CodeVe.cshtml", list);
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_CodeVe.cshtml");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EditTicket(int id)
        {
            var model = await _ticketRepository.GetById(id);
            return PartialView("_Partial_EditTicket", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateTicket(tbl_Ticket model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1105);
                model.ModifyDate = DateTime.Now;
                var cus = _customerRepository.FindId(model.CustomerId);
                model.Name = cus.FullName;
                model.Phone = cus.Phone;
                if (await _ticketRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(1105, "Cập nhật vé: " + model.Code + ", khách hàng: " + _customerRepository.FindId(model.CustomerId).Code,
                                null, //appointment
                                null, //contract
                                model.CustomerId, //customer
                                null, //partner
                                null, //program
                                null, //task
                                model.TourId, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                model.Id // ticket
                                );

                    var list = _ticketRepository.GetAllAsQueryable().AsEnumerable()
                        .Where(p => p.CustomerId == model.CustomerId && p.IsDelete == false)
                        .OrderByDescending(p => p.CreateDate)
                        .Select(p => new tbl_Ticket()
                        {
                            Id = p.Id,
                            Code = p.Code,
                            CurrencyId = p.CurrencyId,
                            EndDate = p.EndDate,
                            EndPlace = p.EndPlace,
                            Price = p.Price,
                            StartDate = p.StartDate,
                            StartPlace = p.StartPlace,
                            StatusId = p.StatusId,
                            TypeId = p.TypeId,
                            TourId = p.TourId,
                            tbl_TagsStart = _tagsRepository.FindId(p.StartPlace),
                            tbl_TagsEnd = _tagsRepository.FindId(p.EndPlace),
                            tbl_DictionaryCurrency = _dictionaryRepository.FindId(p.CurrencyId),
                            tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                            tbl_DictionaryType = _dictionaryRepository.FindId(p.TypeId),
                            tbl_Tour = _tourRepository.FindId(p.TourId),
                            CreateDate = p.CreateDate,
                            Staff = p.Staff,
                            tbl_Staff = _staffRepository.FindId(p.Staff)
                        }).ToList();
                    return PartialView("~/Views/CustomerTabInfo/_CodeVe.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_CodeVe.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_CodeVe.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTicket(int id)
        {
            int cusId = _ticketRepository.FindId(id).CustomerId ?? 0;
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1105);
                var listId = id.ToString().Split(',').ToArray();
                //
                var item = _ticketRepository.FindId(id);
                UpdateHistory.SaveHistory(1105, "Xóa code vé: " + item.Code + ", khách hàng: " + _staffRepository.FindId(item.CustomerId).Code,
                                null, //appointment
                                null, //contract
                                item.CustomerId, //customer
                                null, //partner
                                null, //program
                                null, //task
                                item.TourId, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                item.Id // ticket
                                );
                //
                if (await _ticketRepository.DeleteMany(listId, false))
                {
                    var list = _ticketRepository.GetAllAsQueryable().AsEnumerable()
                        .Where(p => p.CustomerId == cusId && p.IsDelete == false)
                        .OrderByDescending(p => p.CreateDate)
                        .Select(p => new tbl_Ticket()
                        {
                            Id = p.Id,
                            Code = p.Code,
                            CurrencyId = p.CurrencyId,
                            EndDate = p.EndDate,
                            EndPlace = p.EndPlace,
                            Price = p.Price,
                            StartDate = p.StartDate,
                            StartPlace = p.StartPlace,
                            StatusId = p.StatusId,
                            TypeId = p.TypeId,
                            TourId = p.TourId,
                            tbl_TagsStart = _tagsRepository.FindId(p.StartPlace),
                            tbl_TagsEnd = _tagsRepository.FindId(p.EndPlace),
                            tbl_DictionaryCurrency = _dictionaryRepository.FindId(p.CurrencyId),
                            tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                            tbl_DictionaryType = _dictionaryRepository.FindId(p.TypeId),
                            tbl_Tour = _tourRepository.FindId(p.TourId),
                            CreateDate = p.CreateDate,
                            Staff = p.Staff,
                            tbl_Staff = _staffRepository.FindId(p.Staff)
                        }).ToList();
                    return PartialView("~/Views/CustomerTabInfo/_CodeVe.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_CodeVe.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_CodeVe.cshtml");
            }
        }
        #endregion

        #region Tour tuyến

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateTour(TourViewModel model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1103);

                var checkcode = _tourRepository.GetAllAsQueryable().FirstOrDefault(p => p.Code == model.SingleTour.Code && p.IsDelete == false);
                if (checkcode == null)
                {
                    model.SingleTour.CustomerId = Convert.ToInt32(Session["idCustomer"].ToString());
                    model.SingleTour.CreatedDate = DateTime.Now;
                    model.SingleTour.ModifiedDate = DateTime.Now;
                    model.SingleTour.IsDelete = false;
                    model.SingleTour.CreateStaffId = clsPermission.GetUser().StaffID;
                    model.SingleTour.Permission = form["SingleTour.Permission"] != null && form["SingleTour.Permission"] != "" ? form["SingleTour.Permission"].ToString() : null;
                    if (model.StartDateTour != null && model.StartDateTour.Value.Year >= 1980)
                    {
                        model.SingleTour.StartDate = model.StartDateTour;
                    }
                    if (model.EndDateTour != null && model.EndDateTour.Value.Year >= 1980)
                    {
                        model.SingleTour.EndDate = model.EndDateTour;
                    }
                    if (await _tourRepository.Create(model.SingleTour))
                    {
                        UpdateHistory.SaveHistory(1103, "Thêm mới tour: " + model.SingleTour.Code + " - " + model.SingleTour.Name + ", khách hàng: " + _customerRepository.FindId(model.SingleTour.CustomerId).Code,
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
                            TourId = model.SingleTour.Id,
                            CustomerId = model.SingleTour.CustomerId ?? 0
                        };
                        await _tourCustomerRepository.Create(tc);
                    }
                }
                ///////
                var list = _tourCustomerRepository.GetAllAsQueryable().AsEnumerable()
                               .Where(c => c.CustomerId == model.SingleTour.CustomerId && c.IsDelete == false)
                               .OrderByDescending(p => p.Id)
                               .Select(p => new TourListViewModel
                               {
                                   SingleTour = p.TourId != null ? _tourRepository.FindId(p.TourId) : null,
                                   TourType = _dictionaryRepository.FindId(_tourRepository.FindId(p.TourId).TypeTourId).Name,
                                   Status = _dictionaryRepository.FindId(_tourRepository.FindId(p.TourId).StatusId).Name,
                                   Manager = LoadHDV(p.TourId),
                               }).ToList();
                foreach (var item in list)
                {
                    item.CongNoDoiTac = _liabilityPartnerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id && c.IsDelete == false).Sum(c => c.ServicePrice) ?? 0;
                    item.Currency = item.CongNoDoiTac != 0 ? _liabilityPartnerRepository.GetAllAsQueryable().FirstOrDefault(c => c.TourId == item.Id && c.IsDelete == false).tbl_DictionaryCurrencyType1.Name : "";
                    item.Currency = item.CongNoKhachHang != 0 ? _liabilityCustomerRepository.GetAllAsQueryable().FirstOrDefault(c => c.TourId == item.Id && c.IsDelete == false).tbl_DictionaryCurrency.Name : "";
                    item.CongNoKhachHang = _liabilityCustomerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id && c.IsDelete == false).Sum(c => c.TotalContract) ?? 0;
                }
                return PartialView("~/Views/CustomerTabInfo/_TourTuyen.cshtml", list);
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_TourTuyen.cshtml");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EditTour(int id)
        {
            var tour = await _tourRepository.GetById(id);
            var model = new TourViewModel()
            {
                SingleTour = tour,
                StartDateTour = tour.StartDate,
                EndDateTour = tour.EndDate
            };
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
        public async Task<ActionResult> UpdateTour(TourViewModel model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1103);
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
                if (await _tourRepository.Update(model.SingleTour))
                {
                    UpdateHistory.SaveHistory(1103, "Cập nhật tour: " + model.SingleTour.Name + ", khách hàng: " + _staffRepository.FindId(model.SingleTour.CustomerId).Code,
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

                    // xóa TourCustomer của customer
                    var check = _tourCustomerRepository.GetAllAsQueryable().AsEnumerable()
                        .FirstOrDefault(p => p.TourId == model.SingleTour.Id && model.SingleTour.CustomerId == p.CustomerId);
                    if (check == null)
                    {
                        // lưu TourCustomer
                        var tc = new tbl_TourCustomer
                        {
                            IsDelete = false,
                            TourId = model.SingleTour.Id,
                            CustomerId = model.SingleTour.CustomerId ?? 0
                        };
                        await _tourCustomerRepository.Create(tc);
                    }

                    UpdateHistory.SaveHistory(1103, "Cập nhật tour, code: " + model.SingleTour.Code + ", khách hàng: " + _staffRepository.FindId(model.SingleTour.CustomerId).Code,
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

                    var list = _tourCustomerRepository.GetAllAsQueryable().AsEnumerable()
                                .Where(c => c.CustomerId == model.SingleTour.CustomerId && c.IsDelete == false)
                                .OrderByDescending(p => p.Id)
                                .Select(p => new TourListViewModel
                                {
                                    SingleTour = p.TourId != null ? _tourRepository.FindId(p.TourId) : null,
                                    TourType = _dictionaryRepository.FindId(_tourRepository.FindId(p.TourId).TypeTourId).Name,
                                    Status = _dictionaryRepository.FindId(_tourRepository.FindId(p.TourId).StatusId).Name,
                                    Manager = LoadHDV(p.TourId),
                                }).ToList();
                    foreach (var item in list)
                    {
                        item.CongNoDoiTac = _liabilityPartnerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id && c.IsDelete == false).Sum(c => c.ServicePrice) ?? 0;
                        item.Currency = item.CongNoDoiTac != 0 ? _liabilityPartnerRepository.GetAllAsQueryable().FirstOrDefault(c => c.TourId == item.Id && c.IsDelete == false).tbl_DictionaryCurrencyType1.Name : "";
                        item.Currency = item.CongNoKhachHang != 0 ? _liabilityCustomerRepository.GetAllAsQueryable().FirstOrDefault(c => c.TourId == item.Id && c.IsDelete == false).tbl_DictionaryCurrency.Name : "";
                        item.CongNoKhachHang = _liabilityCustomerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id && c.IsDelete == false).Sum(c => c.TotalContract) ?? 0;
                    }
                    return PartialView("~/Views/CustomerTabInfo/_TourTuyen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_TourTuyen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_TourTuyen.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTour(int id)
        {
            int cusId = _tourRepository.FindId(id).CustomerId ?? 0;
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1103);
                var history = _updateHistoryRepository.GetAllAsQueryable()
                    .Where(p => p.TourId == id).Where(p => p.IsDelete == false).ToList();
                if (history.Count() > 0)
                {
                    foreach (var item in history)
                    {
                        var list = item.Id.ToString().Split(',').ToArray();
                        await _updateHistoryRepository.DeleteMany(list, false);
                    }
                }
                //
                var items = _tourRepository.FindId(id);
                UpdateHistory.SaveHistory(1103, "Xóa tour: " + items.Code + " - " + items.Name + ", khách hàng: " + _staffRepository.FindId(items.CustomerId).Code,
                    null, //appointment
                                null, //contract
                                items.CustomerId, //customer
                                null, //partner
                                null, //program
                                null, //task
                                items.Id, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                null // ticket
                                );
                //
                int listId = _tourCustomerRepository.GetAllAsQueryable().FirstOrDefault(p => p.TourId == id && p.CustomerId == cusId).Id;
                if (await _tourCustomerRepository.DeleteMany(listId.ToString().Split(',').ToArray(), true))
                {
                    // delete tour
                    await _tourRepository.DeleteMany(id.ToString().Split(',').ToArray(), false);
                    var list = _tourCustomerRepository.GetAllAsQueryable().AsEnumerable()
                                .Where(c => c.CustomerId == cusId && c.IsDelete == false)
                                .OrderByDescending(p => p.Id)
                                .Select(p => new TourListViewModel
                                {
                                    SingleTour = p.TourId != null ? _tourRepository.FindId(p.TourId) : null,
                                    TourType = p.TourId != null ? p.tbl_Tour.tbl_DictionaryTypeTour.Name : "",
                                    Status = p.TourId != null ? p.tbl_Tour.tbl_DictionaryStatus.Name : "",
                                    Manager = LoadHDV(p.TourId),
                                }).ToList();
                    foreach (var item in list)
                    {
                        item.CongNoDoiTac = _liabilityPartnerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id && c.IsDelete == false).Sum(c => c.ServicePrice) ?? 0;
                        item.Currency = item.CongNoDoiTac != 0 ? _liabilityPartnerRepository.GetAllAsQueryable().FirstOrDefault(c => c.TourId == item.Id && c.IsDelete == false).tbl_DictionaryCurrencyType1.Name : "";
                        item.Currency = item.CongNoKhachHang != 0 ? _liabilityCustomerRepository.GetAllAsQueryable().FirstOrDefault(c => c.TourId == item.Id && c.IsDelete == false).tbl_DictionaryCurrency.Name : "";
                        item.CongNoKhachHang = _liabilityCustomerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id && c.IsDelete == false).Sum(c => c.TotalContract) ?? 0;
                    }
                    return PartialView("~/Views/CustomerTabInfo/_TourTuyen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_TourTuyen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_TourTuyen.cshtml");
            }
        }
        #endregion

        #region Hướng dẫn viên

        public string LoadHDV(int idtour)
        {
            string rs = "";
            var hdv = _tourGuideRepository.GetAllAsQueryable().Where(p => p.TourId == idtour && p.IsDelete == false).ToList();
            foreach (var item in hdv)
            {
                rs += item.tbl_Staff.FullName + "<br/>";
            }
            return rs;
        }

        public ActionResult GetIdTour(int id)
        {
            Session["idTourCustomer"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadStaffInfo(int id)
        {
            var staff = _staffRepository.FindId(id);
            var data = new
            {
                FullName = staff.FullName,
                CodeGuide = staff.CodeGuide,
                Birthday = staff.Birthday == null ? "" : staff.Birthday.Value.ToString("yyyy-MM-dd")
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadMultipleFileStaff(IEnumerable<HttpPostedFileBase> FileStaff)
        {
            if (FileStaff != null)
            {
                Session["StaffFile"] = FileStaff;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase Image)
        {
            if (Image != null && Image.ContentLength > 0)
            {
                Session["StaffImage"] = Image;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> CreateGuide(TourGuideViewModel model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1103);
                string idtour = Session["idTourCustomer"].ToString();
                var tour = _tourRepository.FindId(Convert.ToInt32(idtour));

                if (form["StaffId"] == "0" || form["StaffId"] == null)
                {
                    // lưu nhân viên mới nếu ko chọn nhân viên công ty
                    var staff = new tbl_Staff
                    {
                        Code = model.SingleStaff.CodeGuide,
                        Birthday = model.SingleStaff.Birthday,
                        CodeGuide = model.SingleStaff.CodeGuide,
                        CreatedDate = DateTime.Now,
                        FullName = model.SingleStaff.FullName,
                        IsDelete = false,
                        IsGuide = true,
                        IsLock = false,
                        IsVietlike = false,
                        ModifiedDate = DateTime.Now,
                        StaffId = clsPermission.GetUser().StaffID
                    };
                    if (Session["StaffImage"] != null)
                    {
                        //file
                        var Image = Session["StaffImage"] as HttpPostedFileBase;
                        string FileSize = Common.ConvertFileSize(Image.ContentLength);
                        String newName = Image.FileName.Insert(Image.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                        String path = Server.MapPath("~/Upload/file/" + newName);
                        Image.SaveAs(path);
                        staff.Image = newName;
                        //end file
                    }
                    await _staffRepository.Create(staff);

                    // lưu tourguide
                    var tourguide = new tbl_TourGuide
                    {
                        CreateDate = DateTime.Now,
                        IsDelete = false,
                        StaffId = staff.Id,
                        TourId = Convert.ToInt32(idtour),
                        TagId = tour.DestinationPlace ?? 0,
                        StartDate = model.SingleGuide.StartDate,
                        EndDate = model.SingleGuide.EndDate,
                    };

                    if (await _tourGuideRepository.Create(tourguide))
                    {
                        UpdateHistory.SaveHistory(1103, "Thêm mới hướng dẫn viên: " + staff.Code + " - " + staff.FullName,
                                null, //appointment
                                null, //contract
                                null, //customer
                                null, //partner
                                null, //program
                                null, //task
                                tourguide.TourId, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                null // ticket
                                );
                        // lịch tour của hướng dẫn viên
                        var lichditour = new tbl_TourSchedule()
                        {
                            TourId = Convert.ToInt32(idtour),
                            StaffId = staff.Id,
                            CreatedDate = DateTime.Now,
                            StartDate = model.SingleGuide.StartDate,
                            EndDate = model.SingleGuide.EndDate,
                            IsDelete = false
                        };
                        await _tourScheduleRepository.Create(lichditour);
                    }
                    // lưu tài liệu của hướng dẫn viên
                    if (Session["StaffFile"] != null)
                    {
                        var File = Session["StaffFile"] as IEnumerable<HttpPostedFileBase>;
                        foreach (var file in File)
                        {
                            if (file != null && file.ContentLength > 0)
                            {
                                //file
                                string FileSize = Common.ConvertFileSize(file.ContentLength);
                                String newName = file.FileName.Insert(file.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                                String path = Server.MapPath("~/Upload/file/" + newName);
                                file.SaveAs(path);
                                //end file
                                var f = new tbl_DocumentFile
                                {
                                    Code = GenerateCode.DocumentCode(),
                                    CreatedDate = DateTime.Now,
                                    DictionaryId = 1063,
                                    FileName = newName,
                                    FileSize = FileSize,
                                    IsDelete = false,
                                    IsRead = false,
                                    ModifiedDate = DateTime.Now,
                                    PermissionStaff = tourguide.StaffId.ToString(),
                                    StaffId = clsPermission.GetUser().StaffID,
                                    TourId = Convert.ToInt32(idtour)
                                };
                                await _documentFileRepository.Create(f);
                            }
                        }
                    }
                }
                else // chon nhan vien cong ty
                {
                    // update thong tin nhan vien
                    var staff = _db.tbl_Staff.Find(Convert.ToInt32(form["StaffId"]));
                    staff.Birthday = model.SingleStaff.Birthday;
                    staff.CodeGuide = model.SingleStaff.CodeGuide;
                    staff.FullName = model.SingleStaff.FullName;
                    if (Session["StaffImage"] != null)
                    {
                        //file
                        var Image = Session["StaffImage"] as HttpPostedFileBase;
                        string FileSize = Common.ConvertFileSize(Image.ContentLength);
                        String newName = Image.FileName.Insert(Image.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                        String path = Server.MapPath("~/Upload/file/" + newName);
                        Image.SaveAs(path);
                        staff.Image = newName;
                        //end file
                    }
                    _db.SaveChanges();

                    // lưu tourguide
                    var tourguide = new tbl_TourGuide
                    {
                        CreateDate = DateTime.Now,
                        IsDelete = false,
                        StaffId = Convert.ToInt32(form["StaffId"]),
                        TourId = Convert.ToInt32(idtour),
                        TagId = tour.DestinationPlace ?? 0,
                        StartDate = model.SingleGuide.StartDate,
                        EndDate = model.SingleGuide.EndDate,
                    };

                    if (await _tourGuideRepository.Create(tourguide))
                    {
                        // lịch tour của hướng dẫn viên
                        var lichditour = new tbl_TourSchedule()
                        {
                            TourId = Convert.ToInt32(idtour),
                            StaffId = Convert.ToInt32(form["StaffId"]),
                            CreatedDate = DateTime.Now,
                            StartDate = model.SingleGuide.StartDate,
                            EndDate = model.SingleGuide.EndDate,
                            IsDelete = false
                        };
                        await _tourScheduleRepository.Create(lichditour);
                    }
                    // lưu tài liệu của hướng dẫn viên
                    if (Session["StaffFile"] != null)
                    {
                        var File = Session["StaffFile"] as IEnumerable<HttpPostedFileBase>;
                        foreach (var file in File)
                        {
                            if (file != null && file.ContentLength > 0)
                            {
                                //file
                                string FileSize = Common.ConvertFileSize(file.ContentLength);
                                String newName = file.FileName.Insert(file.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                                String path = Server.MapPath("~/Upload/file/" + newName);
                                file.SaveAs(path);
                                //end file
                                var f = new tbl_DocumentFile
                                {
                                    Code = GenerateCode.DocumentCode(),
                                    CreatedDate = DateTime.Now,
                                    DictionaryId = 1063,
                                    FileName = newName,
                                    FileSize = FileSize,
                                    IsDelete = false,
                                    IsRead = false,
                                    ModifiedDate = DateTime.Now,
                                    PermissionStaff = tourguide.StaffId.ToString(),
                                    StaffId = clsPermission.GetUser().StaffID,
                                    TourId = Convert.ToInt32(idtour)
                                };
                                await _documentFileRepository.Create(f);
                            }
                        }
                    }
                }

                var list = _tourCustomerRepository.GetAllAsQueryable().AsEnumerable()
                                .Where(c => c.CustomerId == tour.CustomerId && c.IsDelete == false)
                                .OrderByDescending(p => p.Id)
                                .Select(p => new TourListViewModel
                                {
                                    SingleTour = p.TourId != null ? _tourRepository.FindId(p.TourId) : null,
                                    TourType = _dictionaryRepository.FindId(_tourRepository.FindId(p.TourId).TypeTourId).Name,
                                    Status = _dictionaryRepository.FindId(_tourRepository.FindId(p.TourId).StatusId).Name,
                                    Manager = LoadHDV(p.TourId),
                                }).ToList();
                foreach (var item in list)
                {
                    item.CongNoDoiTac = _liabilityPartnerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id && c.IsDelete == false).Sum(c => c.ServicePrice) ?? 0;
                    item.Currency = item.CongNoDoiTac != 0 ? _liabilityPartnerRepository.GetAllAsQueryable().FirstOrDefault(c => c.TourId == item.Id && c.IsDelete == false).tbl_DictionaryCurrencyType1.Name : "";
                    item.Currency = item.CongNoKhachHang != 0 ? _liabilityCustomerRepository.GetAllAsQueryable().FirstOrDefault(c => c.TourId == item.Id && c.IsDelete == false).tbl_DictionaryCurrency.Name : "";
                    item.CongNoKhachHang = _liabilityCustomerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id && c.IsDelete == false).Sum(c => c.TotalContract) ?? 0;
                }
                return PartialView("~/Views/CustomerTabInfo/_TourTuyen.cshtml", list);
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_TourTuyen.cshtml");
            }
        }
        
        #endregion

    }
}