using CRM.Core;
using CRM.Infrastructure;
using TOURDEMO.Models;
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
    public class TourServiceController : BaseController
    {
        // GET: TourService
        #region Init

        private IGenericRepository<tbl_DeadlineOption> _deadlineOptionRepository;

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

        public TourServiceController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_DeadlineOption> deadlineOptionRepository,

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
            this._deadlineOptionRepository = deadlineOptionRepository;
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
        void Permission(int PermissionsId, int formId)
        {
            var list = _db.tbl_ActionData.Where(p => p.FormId == formId & p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsEdit = list.Contains(3);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsImport = list.Contains(4);
        }
        #endregion

        #region Khách sạn

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateHotel(FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 77);
            int tourId = Convert.ToInt32(Session["idTour"].ToString());
            try
            {
                for (int i = 1; i <= Convert.ToInt32(form["NumberOptionHotel"]); i++)
                {
                    // insert dịch vụ khách hàng
                    var service = new tbl_ServicesPartner()
                    {
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        CurrencyId = Convert.ToInt32(form["currency-hotel-tour" + i]),
                        Note = form["note-hotel" + i].ToString(),
                        NumberNight = form["night-hotel" + i] != null && form["night-hotel" + i] != "" ? Convert.ToInt32(form["night-hotel" + i]) : 0,
                        NumberRoom = form["room-hotel" + i] != null && form["room-hotel" + i] != "" ? Convert.ToInt32(form["room-hotel" + i]) : 0,
                        Phone = form["phone-hotel" + i],
                        Position = form["position-hotel" + i],
                        Price = form["price-hotel" + i] != null && form["price-hotel" + i] != "" ? Convert.ToDouble(form["price-hotel" + i].ToString()) : 0,
                        StaffContact = form["nguoi-lien-he-hotel" + i],
                        Standard = form["star-hotel"] + i != null && form["star-hotel"] + i != "" ? Convert.ToInt32(form["star-hotel"] + i) : 0,
                        PartnerId = Convert.ToInt32(form["hotel-tour" + i])
                    };
                    if (form["tungay-hotel" + i] != null && form["tungay-hotel" + i] != "")
                    {
                        service.Time = Convert.ToDateTime(form["tungay-hotel" + i].ToString());
                    }
                    if (await _servicesPartnerRepository.Create(service))
                    {
                        // lưu option --> lịch sử liên hệ
                        var contact = new tbl_ContactHistory
                        {
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            DictionaryId = 1145,
                            Note = _partnerRepository.FindId(service.PartnerId).Name + "<br/>" + form["note-hotel" + i].ToString(),
                            PartnerId = service.PartnerId,
                            Request = service.NumberRoom + " phòng, " + service.NumberNight + " đêm, giá/đơn vị: " + string.Format("{0:0,0}", service.Price).Replace(",", ".") + " " + _dictionaryRepository.FindId(service.CurrencyId).Name,
                            StaffId = clsPermission.GetUser().StaffID,
                            TourId = tourId
                        };
                        if (form["tungay-hotel" + i] != null && form["tungay-hotel" + i] != "")
                        {
                            contact.ContactDate = Convert.ToDateTime(form["tungay-hotel" + i].ToString());
                        }
                        if (await _contactHistoryRepository.Create(contact))
                        {
                            // insert deadline dịch vụ
                            for (int j = 1; j <= Convert.ToInt32(form["NumberDeadlineHotel" + i]); j++)
                            {
                                if (form["deadline-name-hotel" + i + j] != null && form["deadline-name-hotel" + i + j] != "")
                                {
                                    // insert tbl_DeadlineOption
                                    var deadline = new tbl_DeadlineOption
                                    {
                                        CreatedDate = DateTime.Now,
                                        Deposit = form["deadline-total-hotel" + i + j] != null && form["deadline-total-hotel" + i + j] != "" ? Convert.ToDecimal(form["deadline-total-hotel" + i + j].ToString()) : 0,
                                        Name = form["deadline-name-hotel" + i + j],
                                        Note = form["deadline-note-hotel" + i + j],
                                        ServicesPartnerId = service.Id,
                                        StaffId = clsPermission.GetUser().StaffID,
                                        StatusId = Convert.ToInt32(form["deadline-status-hotel" + i + j].ToString()),
                                        Time = form["deadline-thoigian-hotel" + i + j] != null && form["deadline-thoigian-hotel" + i + j] != "" ? Convert.ToDateTime(form["deadline-thoigian-hotel" + i + j].ToString()) : DateTime.Now,
                                        CurrencyId = Convert.ToInt32(form["deadline-currency-hotel" + i + j].ToString())
                                    };
                                    if (await _deadlineOptionRepository.Create(deadline))
                                    {
                                        // insert tbl_AppointmentHistory
                                        var appointment = new tbl_AppointmentHistory
                                        {
                                            CreatedDate = DateTime.Now,
                                            ModifiedDate = DateTime.Now,
                                            DictionaryId = 1214,
                                            IsNotify = true,
                                            IsRepeat = true,
                                            Note = _partnerRepository.FindId(service.PartnerId).Name + "<br/>" + deadline.Note,
                                            Notify = 5,
                                            PartnerId = service.PartnerId,
                                            ServiceId = 1048,
                                            Repeat = 5,
                                            StaffId = clsPermission.GetUser().StaffID,
                                            StatusId = deadline.StatusId,
                                            Time = deadline.Time ?? DateTime.Now,
                                            Title = deadline.Name,
                                            TourId = tourId
                                        };
                                        await _appointmentHistoryRepository.Create(appointment);
                                    }
                                    // insert tbl_TourOption
                                    var touroption = new tbl_TourOption
                                    {
                                        DeadlineId = deadline.Id,
                                        PartnerId = service.PartnerId,
                                        ServiceId = 1048,
                                        ServicePartnerId = service.Id,
                                        TourId = tourId
                                    };
                                    await _tourOptionRepository.Create(touroption);
                                }
                                else
                                {
                                    // insert tbl_TourOption
                                    var touroption = new tbl_TourOption
                                    {
                                        PartnerId = service.PartnerId,
                                        ServiceId = 1048,
                                        ServicePartnerId = service.Id,
                                        TourId = tourId
                                    };
                                    await _tourOptionRepository.Create(touroption);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            ModelState.Clear();
            var list = _db.tbl_TourOption.AsEnumerable().Where(p => p.TourId == tourId).Where(p => p.IsDelete == false)
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
                                PartnerId = p.PartnerId,
                                ServicePartnerId = p.ServicePartnerId ?? 0,
                                Price = p.tbl_ServicesPartner.Price,
                                Currency = _dictionaryRepository.FindId(p.tbl_ServicesPartner.CurrencyId).Name
                            }).ToList();
            return PartialView("~/Views/TourTabInfo/_DichVu.cshtml", list);

        }

        [HttpPost]
        public async Task<ActionResult> UpdateHotel()
        {
            return PartialView("~/Views/TourTabInfo/_DichVu.cshtml");
        }

        #endregion

        #region Nhà hàng

        /// <summary>
        /// upload file nhà hàng
        /// </summary>
        /// <param name="TransportDocument"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UploadFileRestaurant(HttpPostedFileBase RestaurantDocument, int id)
        {
            Session["RestaurantFile" + id] = RestaurantDocument;
            return Json(JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// thêm mới dịch vụ nhà hàng
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateRestaurant(FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 77);
            int tourId = Convert.ToInt32(Session["idTour"].ToString());
            try
            {
                for (int i = 1; i <= Convert.ToInt32(form["NumberOptionRestaurant"]); i++)
                {
                    // insert dịch vụ khách hàng

                    var service = new tbl_ServicesPartner()
                    {
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        CurrencyId = Convert.ToInt32(form["RestaurantCurrency" + i]),
                        Note = form["RestaurantNote" + i].ToString(),
                        Phone = form["DienThoai" + i].ToString(),
                        Price = form["RestaurantPrice" + i] != "" ? Convert.ToDouble(form["RestaurantPrice" + i].ToString()) : 0,
                        StaffContact = form["NguoiLienHe" + i].ToString(),
                        PartnerId = Convert.ToInt32(form["RestaurantName" + i]),
                        Address = form["RestaurantAddress" + i].ToString()
                    };
                    // tài liệu
                    HttpPostedFileBase FileName = Session["RestaurantFile" + i] as HttpPostedFileBase;
                    String newName = "";
                    string FileSize = "";
                    if (FileName != null && FileName.ContentLength > 0)
                    {
                        FileSize = Common.ConvertFileSize(FileName.ContentLength);
                        newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                        String path = Server.MapPath("~/Upload/file/" + newName);
                        FileName.SaveAs(path);//======<<<=====
                    }
                    if (newName != "" && FileSize != null)
                    {
                        service.FileName = newName;
                        // insert tbl_DocumentFile
                        var document = new tbl_DocumentFile
                        {
                            Code = GenerateCode.DocumentCode(),
                            CreatedDate = DateTime.Now,
                            FileName = newName,
                            FileSize = FileSize,
                            IsCustomer = false,
                            IsRead = false,
                            ModifiedDate = DateTime.Now,
                            PartnerId = service.PartnerId,
                            PermissionStaff = clsPermission.GetUser().StaffID.ToString(),
                            StaffId = clsPermission.GetUser().StaffID,
                            TourId = tourId
                        };
                        await _documentFileRepository.Create(document);
                    }
                    //end file
                    if (await _servicesPartnerRepository.Create(service))
                    {
                        // lưu option --> lịch sử liên hệ
                        var contact = new tbl_ContactHistory
                        {
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            DictionaryId = 1145,
                            Note = service.Note,
                            PartnerId = service.PartnerId,
                            Request = "giá: " + string.Format("{0:0,0}", service.Price).Replace(",", ".") + " " + _dictionaryRepository.FindId(service.CurrencyId).Name,
                            StaffId = clsPermission.GetUser().StaffID,
                            TourId = tourId,
                            ContactDate = DateTime.Now//=========<<<=====
                        };
                        if (await _contactHistoryRepository.Create(contact))
                        {
                            // insert deadline dịch vụ
                            for (int j = 1; j <= Convert.ToInt32(form["NumberDeadlineRestaurant" + i]); j++)
                            {
                                if (form["deadline-name-hotel" + i + j] != "")
                                {
                                    // insert tbl_DeadlineOption
                                    var deadline = new tbl_DeadlineOption
                                    {
                                        CreatedDate = DateTime.Now,
                                        Deposit = form["DeadlineDeposit" + i + j] != null && form["DeadlineDeposit" + i + j] != "" ? Convert.ToDecimal(form["DeadlineDeposit" + i + j].ToString()) : 0,
                                        Name = form["DeadlineTen" + i + j],
                                        Note = form["DeadlineNote" + i + j],
                                        ServicesPartnerId = service.Id,
                                        StaffId = clsPermission.GetUser().StaffID,
                                        StatusId = Convert.ToInt32(form["DeadlineStatus" + i + j].ToString()),
                                        Time = form["DeadlineThoiGian" + i + j] != null && form["DeadlineThoiGian" + i + j] != "" ? Convert.ToDateTime(form["DeadlineThoiGian" + i + j].ToString()) : DateTime.Now,
                                        CurrencyId = Convert.ToInt32(form["deadline-currency-restaurant" + i + j].ToString())
                                    };
                                    if (await _deadlineOptionRepository.Create(deadline))
                                    {
                                        // insert tbl_AppointmentHistory
                                        var appointment = new tbl_AppointmentHistory
                                        {
                                            CreatedDate = DateTime.Now,
                                            ModifiedDate = DateTime.Now,
                                            DictionaryId = 1214,
                                            IsNotify = true,
                                            IsRepeat = true,
                                            Note = deadline.Note,
                                            Notify = 5,
                                            PartnerId = service.PartnerId,
                                            Repeat = 5,
                                            StaffId = clsPermission.GetUser().StaffID,
                                            StatusId = deadline.StatusId,
                                            Time = deadline.Time ?? DateTime.Now,
                                            Title = deadline.Name,
                                            TourId = tourId,
                                            ServiceId = 1047
                                        };
                                        await _appointmentHistoryRepository.Create(appointment);
                                    }
                                    // insert tbl_TourOption
                                    var touroption = new tbl_TourOption
                                    {
                                        DeadlineId = deadline.Id,
                                        PartnerId = service.PartnerId,
                                        ServiceId = 1047,
                                        ServicePartnerId = service.Id,
                                        TourId = tourId
                                    };
                                    await _tourOptionRepository.Create(touroption);
                                }
                                else
                                {
                                    // insert tbl_TourOption
                                    var touroption = new tbl_TourOption
                                    {
                                        PartnerId = service.PartnerId,
                                        ServiceId = 1047,
                                        ServicePartnerId = service.Id,
                                        TourId = tourId
                                    };
                                    await _tourOptionRepository.Create(touroption);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            ModelState.Clear();
            var list = _db.tbl_TourOption.AsEnumerable().Where(p => p.TourId == tourId).Where(p => p.IsDelete == false)
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
                                PartnerId = p.PartnerId,
                                ServicePartnerId = p.ServicePartnerId ?? 0,
                                Price = p.tbl_ServicesPartner.Price,
                                Currency = _dictionaryRepository.FindId(p.tbl_ServicesPartner.CurrencyId).Name
                            }).ToList();
            return PartialView("~/Views/TourTabInfo/_DichVu.cshtml", list);
        }

        [HttpPost]
        public ActionResult UpdateRestaurant()
        {
            return PartialView("~/Views/TourTabInfo/_DichVu.cshtml");
        }

        #endregion

        #region Vận chuyển

        /// <summary>
        /// upload file nhà hàng
        /// </summary>
        /// <param name="RestaurantDocument"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UploadFileTransport(HttpPostedFileBase TransportDocument, int id)
        {
            Session["TransportFile" + id] = TransportDocument;
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateTransport(FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 77);
            int tourId = Convert.ToInt32(Session["idTour"].ToString());
            try
            {
                for (int i = 1; i <= Convert.ToInt32(form["NumberOptionTransport"]); i++)
                {
                    for (int j = 1; j <= Convert.ToInt32(form["NumberDeadlineTranport" + i]); j++)
                    {
                        // insert ServicePartner
                        var service = new tbl_ServicesPartner
                        {
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            StaffContact = form["nguoilienhe-transport" + i] != null && form["nguoilienhe-transport" + i] != "" ? form["nguoilienhe-transport" + i].ToString() : "",
                            Phone = form["phone-transport" + i] != null && form["phone-transport" + i] != "" ? form["phone-transport" + i].ToString() : "",
                            PartnerId = Convert.ToInt32(form["name-transport" + i].ToString()),
                            Name = form["ServiceName" + i + j] != null && form["ServiceName" + i + j] != "" ? form["ServiceName" + i + j].ToString() : "",
                            Price = form["ServicePrice" + i + j] != null && form["ServicePrice" + i + j] != "" ? Convert.ToDouble(form["ServicePrice" + i + j].ToString()) : 0,
                            CurrencyId = Convert.ToInt32(form["ServiceCurrency" + i + j].ToString()),
                            Note = form["ServiceNote" + i + j] != null && form["ServiceNote" + i + j] != "" ? form["ServiceNote" + i + j].ToString() : "",
                        };

                        // tài liệu
                        HttpPostedFileBase FileName = Session["TransportFile" + i] as HttpPostedFileBase;
                        String newName = "";
                        string FileSize = "";
                        if (FileName != null && FileName.ContentLength > 0)
                        {
                            FileSize = Common.ConvertFileSize(FileName.ContentLength);
                            newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                            String path = Server.MapPath("~/Upload/file/" + newName);
                            FileName.SaveAs(path);
                        }
                        if (newName != "" && FileSize != null)
                        {
                            service.FileName = newName;

                            // insert tbl_DocumentFile
                            var document = new tbl_DocumentFile
                            {
                                Code = GenerateCode.DocumentCode(),
                                CreatedDate = DateTime.Now,
                                FileName = newName,
                                FileSize = FileSize,
                                IsCustomer = false,
                                IsRead = false,
                                ModifiedDate = DateTime.Now,
                                PartnerId = service.PartnerId,
                                PermissionStaff = clsPermission.GetUser().StaffID.ToString(),
                                StaffId = clsPermission.GetUser().StaffID,
                                TourId = tourId,
                            };
                            await _documentFileRepository.Create(document);
                        }
                        //end file
                        if (await _servicesPartnerRepository.Create(service))
                        {
                            // lưu option --> lịch sử liên hệ
                            var contact = new tbl_ContactHistory
                            {
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now,
                                ContactDate = DateTime.Now,
                                DictionaryId = 1145,
                                Note = service.Note,
                                PartnerId = service.PartnerId,
                                Request = "dịch vụ: " + service.Name + ", giá: " + string.Format("{0:0,0}", service.Price).Replace(",", ".") + " " + _dictionaryRepository.FindId(service.CurrencyId).Name,
                                StaffId = clsPermission.GetUser().StaffID,
                                TourId = tourId
                            };
                            if (await _contactHistoryRepository.Create(contact))
                            {
                                // insert tbl_TourOption
                                var touroption = new tbl_TourOption
                                {
                                    PartnerId = service.PartnerId,
                                    ServiceId = 1050,
                                    ServicePartnerId = service.Id,
                                    TourId = tourId
                                };
                                await _tourOptionRepository.Create(touroption);
                            }
                        }
                    }
                }
            }
            catch { }
            ModelState.Clear();
            var list = _db.tbl_TourOption.AsEnumerable().Where(p => p.TourId == tourId).Where(p => p.IsDelete == false)
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
                                PartnerId = p.PartnerId,
                                ServicePartnerId = p.ServicePartnerId ?? 0,
                                Price = p.tbl_ServicesPartner.Price,
                                Currency = _dictionaryRepository.FindId(p.tbl_ServicesPartner.CurrencyId).Name
                            }).ToList();
            return PartialView("~/Views/TourTabInfo/_DichVu.cshtml");
        }

        [HttpPost]
        public ActionResult UpdateTransport()
        {
            return PartialView("~/Views/TourTabInfo/_DichVu.cshtml");
        }

        #endregion

        #region Vé máy bay

        /// <summary>
        /// upload file vé máy bay
        /// </summary>
        /// <param name="FileNamePlane"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadFilePlane(HttpPostedFileBase FileNamePlane, int id)
        {
            Session["PlaneFile" + id] = FileNamePlane;
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreatePlane(FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 77);
            int tourId = Convert.ToInt32(Session["idTour"].ToString());
            try
            {
                for (int i = 1; i <= Convert.ToInt32(form["NumberOptionPlane"]); i++)
                {
                    // insert dịch vụ vé máy bay
                    var service = new tbl_ServicesPartner()
                    {
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        CurrencyId = Convert.ToInt32(form["loaitien-plane" + i]),
                        Note = form["note-plane" + i],
                        Phone = form["contacter-phone-plane" + i],
                        Price = form["price-code" + i] != null && form["price-code" + i] != "" ? Convert.ToDouble(form["price-code" + i].ToString()) : 0,
                        StaffContact = form["contacter-plane" + i],
                        PartnerId = Convert.ToInt32(form["hang-plane" + i]),
                        Flight = form["flight-plane" + i] != null && form["flight-plane" + i] != "" ? form["flight-plane" + i].ToString() : null,
                        NumberTicket = form["quantity-plane" + i] != null && form["quantity-plane" + i] != "" ? Convert.ToInt32(form["quantity-plane" + i].ToString()) : 0
                    };
                    if (await _servicesPartnerRepository.Create(service))
                    {
                        // lưu option --> lịch sử liên hệ
                        var contact = new tbl_ContactHistory
                        {
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            ContactDate = DateTime.Now,
                            DictionaryId = 1145,
                            Note = service.Note,
                            PartnerId = service.PartnerId,
                            Request = "Chuyến bay: " + service.Flight + ", số lượng vé: " + service.NumberTicket + ", giá/vé: " + string.Format("{0:0,0}", service.Price).Replace(",", ".") + " " + _dictionaryRepository.FindId(service.CurrencyId).Name,
                            StaffId = clsPermission.GetUser().StaffID,
                            TourId = tourId
                        };
                        if (await _contactHistoryRepository.Create(contact))
                        {
                            // insert deadline dịch vụ
                            for (int j = 1; j <= Convert.ToInt32(form["NumberDeadlinePlane" + i]); j++)
                            {
                                if (form["name-deadline-plane" + i + j] != null && form["name-deadline-plane" + i + j] != "")
                                {
                                    // insert tbl_DeadlineOption
                                    var deadline = new tbl_DeadlineOption
                                    {
                                        CreatedDate = DateTime.Now,
                                        Deposit = form["sotien-deadline-plane" + i + j] != "" ? Convert.ToDecimal(form["sotien-deadline-plane" + i + j].ToString()) : 0,
                                        Name = form["name-deadline-plane" + i + j],
                                        Note = form["PlaneNoteDeadline" + i + j],
                                        ServicesPartnerId = service.Id,
                                        StaffId = clsPermission.GetUser().StaffID,
                                        StatusId = Convert.ToInt32(form["tinhtrang-deadline-plane" + i + j].ToString()),
                                        Time = form["thoigian-deadline-plane" + i + j] != null && form["thoigian-deadline-plane" + i + j] != "" ? Convert.ToDateTime(form["thoigian-deadline-plane" + i + j].ToString()) : DateTime.Now,
                                        CurrencyId = Convert.ToInt32(form["deadline-currency-plane" + i + j].ToString())
                                    };
                                    // tài liệu
                                    HttpPostedFileBase FileName = Session["PlaneFile" + i + j] as HttpPostedFileBase;
                                    String newName = "";
                                    string FileSize = "";
                                    if (FileName != null && FileName.ContentLength > 0)
                                    {
                                        FileSize = Common.ConvertFileSize(FileName.ContentLength);
                                        newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                                        String path = Server.MapPath("~/Upload/file/" + newName);
                                        FileName.SaveAs(path);
                                    }
                                    if (newName != "" && FileSize != null)
                                    {
                                        service.FileName = newName;

                                        // insert tbl_DocumentFile
                                        var document = new tbl_DocumentFile
                                        {
                                            Code = GenerateCode.DocumentCode(),
                                            CreatedDate = DateTime.Now,
                                            FileName = newName,
                                            FileSize = FileSize,
                                            IsCustomer = false,
                                            IsRead = false,
                                            ModifiedDate = DateTime.Now,
                                            PartnerId = service.PartnerId,
                                            PermissionStaff = clsPermission.GetUser().StaffID.ToString(),
                                            StaffId = clsPermission.GetUser().StaffID,
                                            TourId = tourId
                                        };
                                        await _documentFileRepository.Create(document);
                                    }
                                    //end file
                                    if (await _deadlineOptionRepository.Create(deadline))
                                    {
                                        // insert tbl_AppointmentHistory
                                        var appointment = new tbl_AppointmentHistory
                                        {
                                            CreatedDate = DateTime.Now,
                                            ModifiedDate = DateTime.Now,
                                            DictionaryId = 1214,
                                            IsNotify = true,
                                            IsRepeat = true,
                                            Note = deadline.Note,
                                            Notify = 5,
                                            PartnerId = service.PartnerId,
                                            Repeat = 5,
                                            StaffId = clsPermission.GetUser().StaffID,
                                            StatusId = deadline.StatusId,
                                            Time = deadline.Time ?? DateTime.Now,
                                            Title = deadline.Name,
                                            TourId = tourId,
                                            ServiceId = 1049
                                        };
                                        await _appointmentHistoryRepository.Create(appointment);
                                    }
                                    // insert tbl_TourOption
                                    var touroption = new tbl_TourOption
                                    {
                                        DeadlineId = deadline.Id,
                                        PartnerId = service.PartnerId,
                                        ServiceId = 1049,
                                        ServicePartnerId = service.Id,
                                        TourId = tourId
                                    };
                                    await _tourOptionRepository.Create(touroption);
                                }
                                else
                                {
                                    // insert tbl_TourOption
                                    var touroption = new tbl_TourOption
                                    {
                                        PartnerId = service.PartnerId,
                                        ServiceId = 1049,
                                        ServicePartnerId = service.Id,
                                        TourId = tourId
                                    };
                                    await _tourOptionRepository.Create(touroption);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            ModelState.Clear();
            var list = _db.tbl_TourOption.AsEnumerable().Where(p => p.TourId == tourId).Where(p => p.IsDelete == false)
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
                                PartnerId = p.PartnerId,
                                ServicePartnerId = p.ServicePartnerId ?? 0,
                                Price = p.tbl_ServicesPartner.Price,
                                Currency = _dictionaryRepository.FindId(p.tbl_ServicesPartner.CurrencyId).Name
                            }).ToList();
            return PartialView("~/Views/TourTabInfo/_DichVu.cshtml", list);
        }

        [HttpPost]
        public ActionResult UpdatePlane()
        {
            return PartialView("~/Views/TourTabInfo/_DichVu.cshtml");
        }

        #endregion

        #region Sự kiện

        /// <summary>
        /// upload file sự kiện
        /// </summary>
        /// <param name="FileNameEvent"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadFileEvent(HttpPostedFileBase FileNameEvent, int id)
        {
            Session["EventFile" + id] = FileNameEvent;
            return Json(JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// thêm mới sự kiện
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateEvent(FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 77);
            int tourId = Convert.ToInt32(Session["idTour"].ToString());
            try
            {
                for (int i = 1; i <= Convert.ToInt32(form["NumberOptionEvent"]); i++)
                {
                    // insert dịch vụ khách hàng
                    var service = new tbl_ServicesPartner()
                    {
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        CurrencyId = Convert.ToInt32(form["insert-currency-event" + i]),
                        Note = form["insert-note-event" + i],
                        Phone = form["insert-phone-event" + i],
                        Price = form["insert-price-event" + i] != null && form["insert-price-event" + i] != "" ? Convert.ToDouble(form["insert-price-event" + i].ToString()) : 0,
                        StaffContact = form["insert-contact-event" + i],
                        PartnerId = Convert.ToInt32(form["insert-company-event" + i])
                    };
                    // tài liệu
                    HttpPostedFileBase FileName = Session["EventFile" + i] as HttpPostedFileBase;
                    String newName = "";
                    string FileSize = "";
                    if (FileName != null && FileName.ContentLength > 0)
                    {
                        FileSize = Common.ConvertFileSize(FileName.ContentLength);
                        newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                        String path = Server.MapPath("~/Upload/file/" + newName);
                        FileName.SaveAs(path);
                    }
                    if (newName != "" && FileSize != null)
                    {
                        service.FileName = newName;

                        // insert tbl_DocumentFile
                        var document = new tbl_DocumentFile
                        {
                            Code = GenerateCode.DocumentCode(),
                            CreatedDate = DateTime.Now,
                            FileName = newName,
                            FileSize = FileSize,
                            IsCustomer = false,
                            IsRead = false,
                            ModifiedDate = DateTime.Now,
                            PartnerId = service.PartnerId,
                            PermissionStaff = clsPermission.GetUser().StaffID.ToString(),
                            StaffId = clsPermission.GetUser().StaffID,
                            TourId = tourId
                        };
                        await _documentFileRepository.Create(document);
                    }
                    //end file
                    if (await _servicesPartnerRepository.Create(service))
                    {
                        // lưu option --> lịch sử liên hệ
                        var contact = new tbl_ContactHistory
                        {
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            ContactDate = DateTime.Now,
                            DictionaryId = 1145,
                            Note = service.Note,
                            PartnerId = service.PartnerId,
                            Request = "Tổng giá trị: " + string.Format("{0:0,0}", service.Price).Replace(",", ".") + " " + _dictionaryRepository.FindId(service.CurrencyId).Name,
                            StaffId = clsPermission.GetUser().StaffID,
                            TourId = tourId
                        };
                        if (await _contactHistoryRepository.Create(contact))
                        {
                            // insert deadline dịch vụ
                            for (int j = 1; j <= Convert.ToInt32(form["NumberDeadlineEvent" + i]); j++)
                            {
                                if (form["deadline-name-event" + i + j] != null && form["deadline-name-event" + i + j] != "")
                                {
                                    // insert tbl_DeadlineOption
                                    var deadline = new tbl_DeadlineOption
                                    {
                                        CreatedDate = DateTime.Now,
                                        Deposit = form["deadline-total-event" + i + j] != null && form["deadline-total-event" + i + j] != "" ? Convert.ToDecimal(form["deadline-total-event" + i + j].ToString()) : 0,
                                        Name = form["deadline-name-event" + i + j],
                                        Note = form["deadline-note-event" + i + j],
                                        ServicesPartnerId = service.Id,
                                        StaffId = clsPermission.GetUser().StaffID,
                                        StatusId = Convert.ToInt32(form["deadline-status-event" + i + j].ToString()),
                                        Time = form["deadline-thoigian-event" + i + j] != null && form["deadline-thoigian-event" + i + j] != "" ? Convert.ToDateTime(form["deadline-thoigian-event" + i + j].ToString()) : DateTime.Now,
                                        CurrencyId = Convert.ToInt32(form["deadline-currency-event" + i + j].ToString())
                                    };
                                    if (await _deadlineOptionRepository.Create(deadline))
                                    {
                                        // insert tbl_AppointmentHistory
                                        var appointment = new tbl_AppointmentHistory
                                        {
                                            CreatedDate = DateTime.Now,
                                            ModifiedDate = DateTime.Now,
                                            DictionaryId = 1214,
                                            IsNotify = true,
                                            IsRepeat = true,
                                            Note = deadline.Note,
                                            Notify = 5,
                                            PartnerId = service.PartnerId,
                                            Repeat = 5,
                                            StaffId = clsPermission.GetUser().StaffID,
                                            StatusId = deadline.StatusId,
                                            Time = deadline.Time ?? DateTime.Now,
                                            Title = deadline.Name,
                                            TourId = tourId,
                                            ServiceId = 1051
                                        };
                                        await _appointmentHistoryRepository.Create(appointment);
                                    }
                                    // insert tbl_TourOption
                                    var touroption = new tbl_TourOption
                                    {
                                        DeadlineId = deadline.Id,
                                        PartnerId = service.PartnerId,
                                        ServiceId = 1051,
                                        ServicePartnerId = service.Id,
                                        TourId = tourId
                                    };
                                    await _tourOptionRepository.Create(touroption);
                                }
                                else
                                {
                                    // insert tbl_TourOption
                                    var touroption = new tbl_TourOption
                                    {
                                        PartnerId = service.PartnerId,
                                        ServiceId = 1051,
                                        ServicePartnerId = service.Id,
                                        TourId = tourId
                                    };
                                    await _tourOptionRepository.Create(touroption);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            ModelState.Clear();
            var list = _db.tbl_TourOption.AsEnumerable().Where(p => p.TourId == tourId).Where(p => p.IsDelete == false)
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
                                 PartnerId = p.PartnerId,
                                 ServicePartnerId = p.ServicePartnerId ?? 0,
                                 Price = p.tbl_ServicesPartner.Price,
                                 Currency = _dictionaryRepository.FindId(p.tbl_ServicesPartner.CurrencyId).Name
                             }).ToList();
            return PartialView("~/Views/TourTabInfo/_DichVu.cshtml", list);
        }
        #endregion

        #region Landtour

        /// <summary>
        /// upload file Landtour
        /// </summary>
        /// <param name="FileNameLandtour"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadFileLandtour(HttpPostedFileBase FileNameLandtour, int id)
        {
            Session["LandtourFile" + id] = FileNameLandtour;
            return Json(JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// thêm mới Landtour
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateLandtour(FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 77);
            int tourId = Convert.ToInt32(Session["idTour"].ToString());
            try
            {
                for (int i = 1; i <= Convert.ToInt32(form["NumberOptionLandtour"]); i++)
                {
                    // insert dịch vụ khách hàng
                    var service = new tbl_ServicesPartner()
                    {
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        CurrencyId = Convert.ToInt32(form["insert-currency-landtour" + i]),
                        Note = form["insert-note-landtour" + i],
                        Phone = form["insert-phone-landtour" + i],
                        Price = form["insert-price-landtour" + i] != null && form["insert-price-landtour" + i] != "" ? Convert.ToDouble(form["insert-price-landtour" + i].ToString()) : 0,
                        StaffContact = form["insert-contact-landtour" + i],
                        PartnerId = Convert.ToInt32(form["insert-company-landtour" + i])
                    };
                    // tài liệu
                    HttpPostedFileBase FileName = Session["LandtourFile" + i] as HttpPostedFileBase;
                    String newName = "";
                    string FileSize = "";
                    if (FileName != null && FileName.ContentLength > 0)
                    {
                        FileSize = Common.ConvertFileSize(FileName.ContentLength);
                        newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                        String path = Server.MapPath("~/Upload/file/" + newName);
                        FileName.SaveAs(path);
                    }
                    if (newName != "" && FileSize != null)
                    {
                        service.FileName = newName;

                        // insert tbl_DocumentFile
                        var document = new tbl_DocumentFile
                        {
                            Code = GenerateCode.DocumentCode(),
                            CreatedDate = DateTime.Now,
                            FileName = newName,
                            FileSize = FileSize,
                            IsCustomer = false,
                            IsRead = false,
                            ModifiedDate = DateTime.Now,
                            PartnerId = service.PartnerId,
                            PermissionStaff = clsPermission.GetUser().StaffID.ToString(),
                            StaffId = clsPermission.GetUser().StaffID,
                            TourId = tourId
                        };
                        await _documentFileRepository.Create(document);
                    }
                    //end file
                    if (await _servicesPartnerRepository.Create(service))
                    {
                        // lưu option --> lịch sử liên hệ
                        var contact = new tbl_ContactHistory
                        {
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            ContactDate = DateTime.Now,
                            DictionaryId = 1145,
                            Note = service.Note,
                            PartnerId = service.PartnerId,
                            Request = "Tổng giá trị: " + string.Format("{0:0,0}", service.Price).Replace(",", ".") + " " + _dictionaryRepository.FindId(service.CurrencyId).Name,
                            StaffId = clsPermission.GetUser().StaffID,
                            TourId = tourId
                        };
                        if (await _contactHistoryRepository.Create(contact))
                        {
                            // insert deadline dịch vụ
                            for (int j = 1; j <= Convert.ToInt32(form["NumberDeadlineLandtour" + i]); j++)
                            {
                                if (form["deadline-name-landtour" + i + j] != null && form["deadline-name-landtour" + i + j] != "")
                                {
                                    // insert tbl_DeadlineOption
                                    var deadline = new tbl_DeadlineOption
                                    {
                                        CreatedDate = DateTime.Now,
                                        Deposit = form["deadline-total-landtour" + i + j] != null && form["deadline-total-landtour" + i + j] != "" ? Convert.ToDecimal(form["deadline-total-landtour" + i + j].ToString()) : 0,
                                        Name = form["deadline-name-landtour" + i + j],
                                        Note = form["deadline-note-landtour" + i + j],
                                        ServicesPartnerId = service.Id,
                                        StaffId = clsPermission.GetUser().StaffID,
                                        StatusId = Convert.ToInt32(form["deadline-status-landtour" + i + j].ToString()),
                                        Time = form["deadline-thoigian-landtour" + i + j] != null && form["deadline-thoigian-landtour" + i + j] != "" ? Convert.ToDateTime(form["deadline-thoigian-landtour" + i + j].ToString()) : DateTime.Now,
                                        CurrencyId = Convert.ToInt32(form["deadline-currency-landtour" + i + j].ToString())
                                    };
                                    if (await _deadlineOptionRepository.Create(deadline))
                                    {
                                        // insert tbl_AppointmentHistory
                                        var appointment = new tbl_AppointmentHistory
                                        {
                                            CreatedDate = DateTime.Now,
                                            ModifiedDate = DateTime.Now,
                                            DictionaryId = 1214,
                                            IsNotify = true,
                                            IsRepeat = true,
                                            Note = deadline.Note,
                                            Notify = 5,
                                            PartnerId = service.PartnerId,
                                            Repeat = 5,
                                            StaffId = clsPermission.GetUser().StaffID,
                                            StatusId = deadline.StatusId,
                                            Time = deadline.Time ?? DateTime.Now,
                                            Title = deadline.Name,
                                            TourId = tourId,
                                            ServiceId = 1224
                                        };
                                        await _appointmentHistoryRepository.Create(appointment);
                                    }
                                    // insert tbl_TourOption
                                    var touroption = new tbl_TourOption
                                    {
                                        DeadlineId = deadline.Id,
                                        PartnerId = service.PartnerId,
                                        ServiceId = 1224,
                                        ServicePartnerId = service.Id,
                                        TourId = tourId
                                    };
                                    await _tourOptionRepository.Create(touroption);
                                }
                                else
                                {
                                    // insert tbl_TourOption
                                    var touroption = new tbl_TourOption
                                    {
                                        PartnerId = service.PartnerId,
                                        ServiceId = 1224,
                                        ServicePartnerId = service.Id,
                                        TourId = tourId
                                    };
                                    await _tourOptionRepository.Create(touroption);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            ModelState.Clear();
            var list = _db.tbl_TourOption.AsEnumerable().Where(p => p.TourId == tourId).Where(p => p.IsDelete == false)
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
                                 PartnerId = p.PartnerId,
                                 ServicePartnerId = p.ServicePartnerId ?? 0,
                                 Price = p.tbl_ServicesPartner.Price,
                                 Currency = _dictionaryRepository.FindId(p.tbl_ServicesPartner.CurrencyId).Name
                             }).ToList();
            return PartialView("~/Views/TourTabInfo/_DichVu.cshtml", list);
        }
        #endregion

        #region Load Partner
        public JsonResult LoadPartner(int id)
        {
            var model = _partnerRepository.GetAllAsQueryable().Where(p => p.Id == id).SingleOrDefault();
            var obj = new
            {
                Code = model.Code,
                Address = model.Address,
                StaffContact = model.StaffContact,
                Phone = model.Phone
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Edit
        public ActionResult EditService(int touroption, int service, int partner, int servicepartner)
        {
            Permission(clsPermission.GetUser().PermissionID, 77);
            switch (service)
            {
                //Nhà hàng
                case 1047:
                    var restaurant = _servicesPartnerRepository.FindId(servicepartner);
                    return PartialView("_Partial_EditRestaurant", restaurant);
                //Khách sạn
                case 1048:
                    var hotel = _servicesPartnerRepository.FindId(servicepartner);
                    return PartialView("_Partial_EditHotel", hotel);
                //Hàng không
                case 1049:
                    var plane = _servicesPartnerRepository.FindId(servicepartner);
                    return PartialView("_Partial_EditPlane", plane);
                //Vận chuyển
                case 1050:
                    var transport = _servicesPartnerRepository.FindId(servicepartner);
                    return PartialView("_Partial_EditTransport", transport);
                //Event
                case 1051:
                    var events = _servicesPartnerRepository.FindId(servicepartner);
                    return PartialView("_Partial_EditEvent", events);
            }
            return View("Index");
        }
        #endregion
    }
}