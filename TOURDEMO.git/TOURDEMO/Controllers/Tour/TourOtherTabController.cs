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
    public class TourOtherTabController : BaseController
    {
        // GET: TourOtherTab
        #region Init
        private IGenericRepository<tbl_LiabilityCustomer> _liabilityCustomerRepository;
        private IGenericRepository<tbl_Program> _programRepository;
        private IGenericRepository<tbl_LiabilityPartner> _liabilityPartnerRepository;
        private IGenericRepository<tbl_ReviewTour> _reviewTourRepository;
        private IGenericRepository<tbl_ReviewTourDetail> _reviewTourDetailRepository;
        private IGenericRepository<tbl_Contract> _contractRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_Partner> _partnerRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;
        private IGenericRepository<tbl_Task> _taskRepository;
        private IGenericRepository<tbl_CustomerContact> _customerContactRepository;
        private IGenericRepository<tbl_CustomerVisa> _customerVisaRepository;
        private IGenericRepository<tbl_CustomerContactVisa> _customerContactVisaRepository;
        private IGenericRepository<tbl_TaskStaff> _taskStaffRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
        private IGenericRepository<tbl_ContactHistory> _contactHistoryRepository;
        private IGenericRepository<tbl_AppointmentHistory> _appointmentHistoryRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;
        private IGenericRepository<tbl_TourGuide> _tourGuideRepository;
        private IGenericRepository<tbl_ServicesPartner> _servicesPartnerRepository;
        private IGenericRepository<tbl_TourCustomerVisa> _tourCustomerVisaRepository;
        private IGenericRepository<tbl_Quotation> _quotationRepository;
        private IGenericRepository<tbl_TourSchedule> _tourScheduleRepository;
        private IGenericRepository<tbl_TourCustomer> _tourCustomerRepository;
        private DataContext _db;

        public TourOtherTabController(
            IGenericRepository<tbl_LiabilityCustomer> liabilityCustomerRepository,
            IGenericRepository<tbl_LiabilityPartner> liabilityPartnerRepository,
            IGenericRepository<tbl_Program> programRepository,
            IGenericRepository<tbl_ReviewTour> reviewTourRepository,
            IGenericRepository<tbl_ReviewTourDetail> reviewTourDetailRepository,
            IGenericRepository<tbl_Contract> contractRepository,
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IGenericRepository<tbl_Task> taskRepository,
            IGenericRepository<tbl_CustomerContact> customerContactRepository,
            IGenericRepository<tbl_CustomerVisa> customerVisaRepository,
            IGenericRepository<tbl_TaskStaff> taskStaffRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_CustomerContactVisa> customerContactVisaRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IGenericRepository<tbl_ContactHistory> contactHistoryRepository,
            IGenericRepository<tbl_AppointmentHistory> appointmentHistoryRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IGenericRepository<tbl_TourGuide> tourGuideRepository,
            IGenericRepository<tbl_ServicesPartner> servicesPartnerRepository,
            IGenericRepository<tbl_TourCustomerVisa> tourCustomerVisaRepository,
            IGenericRepository<tbl_Quotation> quotationRepository,
            IGenericRepository<tbl_TourSchedule> tourScheduleRepository,
            IGenericRepository<tbl_TourCustomer> tourCustomerRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._liabilityCustomerRepository = liabilityCustomerRepository;
            this._liabilityPartnerRepository = liabilityPartnerRepository;
            this._programRepository = programRepository;
            this._reviewTourRepository = reviewTourRepository;
            this._reviewTourDetailRepository = reviewTourDetailRepository;
            this._partnerRepository = partnerRepository;
            this._contractRepository = contractRepository;
            this._customerRepository = customerRepository;
            this._customerContactRepository = customerContactRepository;
            this._tagsRepository = tagsRepository;
            this._taskRepository = taskRepository;
            this._customerVisaRepository = customerVisaRepository;
            this._customerContactVisaRepository = customerContactVisaRepository;
            this._taskStaffRepository = taskStaffRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._documentFileRepository = documentFileRepository;
            this._contactHistoryRepository = contactHistoryRepository;
            this._appointmentHistoryRepository = appointmentHistoryRepository;
            this._updateHistoryRepository = updateHistoryRepository;
            this._staffRepository = staffRepository;
            this._tourRepository = tourRepository;
            this._tourGuideRepository = tourGuideRepository;
            this._servicesPartnerRepository = servicesPartnerRepository;
            this._tourCustomerVisaRepository = tourCustomerVisaRepository;
            this._quotationRepository = quotationRepository;
            this._tourScheduleRepository = tourScheduleRepository;
            this._tourCustomerRepository = tourCustomerRepository;
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

        #region Lịch hẹn
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateAppointment(tbl_AppointmentHistory model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 87);
                model.TourId = Convert.ToInt32(Session["idTour"].ToString());
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.OtherStaff = form["OtherStaff"];
                model.StaffId = clsPermission.GetUser().StaffID;
                model.PartnerId = model.PartnerId == 0 ? null : model.PartnerId;

                if (await _appointmentHistoryRepository.Create(model))
                {
                    UpdateHistory.SaveHistory(87, "Thêm mới lịch hẹn " + model.Title,
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
                            .Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false)
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
                    return PartialView("~/Views/TourTabInfo/_LichHen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_LichHen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_LichHen.cshtml");
            }
        }

        public JsonResult LoadPartner(int id)
        {
            var model = new SelectList(_partnerRepository.GetAllAsQueryable().Where(p => p.DictionaryId == id && p.IsDelete == false).ToList(), "Id", "Name");
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadServicePartner(int id)
        {
            var model = new SelectList(_servicesPartnerRepository.GetAllAsQueryable().Where(p => p.PartnerId == id && p.IsDelete == false).ToList(), "Id", "Name");
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
                Permission(clsPermission.GetUser().PermissionID, 87);
                model.ModifiedDate = DateTime.Now;
                model.OtherStaff = form["OtherStaff"];
                model.PartnerId = model.PartnerId == 0 ? null : model.PartnerId;

                if (await _appointmentHistoryRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(87, "Cập nhật lịch hẹn: " + model.Title,
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
                            .Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false)
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
                    return PartialView("~/Views/TourTabInfo/_LichHen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_LichHen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_LichHen.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAppointment(int id)
        {
            int tourId = _appointmentHistoryRepository.FindId(id).TourId ?? 0;
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 87);
                var listIds = id.ToString().Split(',').ToArray();
                //
                var item = _appointmentHistoryRepository.FindId(id);
                UpdateHistory.SaveHistory(87, "Xóa lịch hẹn: " + item.Title,
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
                if (await _appointmentHistoryRepository.DeleteMany(listIds, false))
                {
                    var list = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable()
                                .Where(p => p.TourId == tourId).Where(p => p.IsDelete == false)
                                .OrderByDescending(p => p.CreatedDate)
                                .Select(p => new tbl_AppointmentHistory
                                {
                                    Id = p.Id,
                                    Title = p.Title,
                                    Time = p.Time,
                                    tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                                    tbl_Staff = _staffRepository.FindId(p.StaffId),
                                    StatusId = p.StatusId,
                                    Note = p.Note,
                                    OtherStaff = p.OtherStaff
                                }).ToList();
                    return PartialView("~/Views/TourTabInfo/_LichHen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_LichHen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_LichHen.cshtml");
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
                Permission(clsPermission.GetUser().PermissionID, 89);
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.StaffId = clsPermission.GetUser().StaffID;
                model.TourId = Int32.Parse(Session["idTour"].ToString());
                model.OtherStaffId = model.StaffId;
                if (await _contactHistoryRepository.Create(model))
                {
                    UpdateHistory.SaveHistory(89, "Thêm mới lịch sử liên hệ " + model.Request,
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
                    var list = _db.tbl_ContactHistory.AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false)
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
                    return PartialView("~/Views/TourTabInfo/_LichSuLienHe.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_LichSuLienHe.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_LichSuLienHe.cshtml");
            }
        }

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
                Permission(clsPermission.GetUser().PermissionID, 89);
                model.ModifiedDate = DateTime.Now;
                model.OtherStaffId = model.StaffId;
                if (await _contactHistoryRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(89, "Cập nhật lịch sử liên hệ: " + model.Request,
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
                    var list = _db.tbl_ContactHistory.AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false)
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
                    return PartialView("~/Views/TourTabInfo/_LichSuLienHe.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_LichSuLienHe.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_LichSuLienHe.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteContactHistory(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 89);
                int tourId = _contactHistoryRepository.FindId(id).TourId ?? 0;
                var listIds = id.ToString().Split(',').ToArray();
                //
                var item = _contactHistoryRepository.FindId(id);
                UpdateHistory.SaveHistory(89, "Xóa lịch sử liên hệ: " + item.Request,
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
                if (await _contactHistoryRepository.DeleteMany(listIds, false))
                {
                    var list = _db.tbl_ContactHistory.AsEnumerable().Where(p => p.TourId == tourId).Where(p => p.IsDelete == false)
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
                    return PartialView("~/Views/TourTabInfo/_LichSuLienHe.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_LichSuLienHe.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_LichSuLienHe.cshtml");
            }
        }
        #endregion

        #region Document
        /********** Quản lý tài liệu ************/

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase FileName)
        {
            if (FileName != null && FileName.ContentLength > 0)
            {
                Session["TourFile"] = FileName;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateDocument(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 88);
                string id = Session["idTour"].ToString();
                if (ModelState.IsValid)
                {
                    model.Code = GenerateCode.DocumentCode();
                    model.TourId = Convert.ToInt32(id);
                    model.CreatedDate = DateTime.Now;
                    model.IsRead = false;
                    model.ModifiedDate = DateTime.Now;
                    if (form["TagsId"] != null && form["TagsId"] != "")
                    {
                        model.TagsId = form["TagsId"].ToString();
                    }
                    model.StaffId = clsPermission.GetUser().StaffID;
                    //file
                    HttpPostedFileBase FileName = Session["TourFile"] as HttpPostedFileBase;
                    string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                    String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
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
                        UpdateHistory.SaveHistory(88, "Thêm mới tài liệu, code: " + model.Code + " - " + model.FileName,
                            null, //appointment
                            model.ContractId, //contract
                            model.CustomerId, //customer
                            model.PartnerId, //partner
                            model.ProgramId, //program
                            null, //task
                            model.TourId, //tour
                            null, //quotation
                            model.Id, //document
                            null, //history
                            null // ticket
                            );

                        Session["TourFile"] = null;
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.TourId.ToString() == id).Where(p => p.IsDelete == false)
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
                        return PartialView("~/Views/TourTabInfo/_TaiLieuMau.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/TourTabInfo/_TaiLieuMau.cshtml");
                    }
                }
            }
            catch { }
            return PartialView("~/Views/TourTabInfo/_TaiLieuMau.cshtml");
        }

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
                Permission(clsPermission.GetUser().PermissionID, 88);
                if (ModelState.IsValid)
                {
                    model.IsRead = true;
                    model.ModifiedDate = DateTime.Now;
                    if (form["TagsId"] != null && form["TagsId"] != "")
                    {
                        model.TagsId = form["TagsId"].ToString();
                    }
                    if (Session["TourFile"] != null)
                    {
                        //file
                        HttpPostedFileBase FileName = Session["TourFile"] as HttpPostedFileBase;
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
                            model.FileSize = FileSize;
                        }
                    }
                    if (await _documentFileRepository.Update(model))
                    {
                        UpdateHistory.SaveHistory(88, "Cập nhật tài liệu của tour: " + model.Code,
                            null, //appointment
                            model.ContractId, //contract
                            model.CustomerId, //customer
                            model.PartnerId, //partner
                            model.ProgramId, //program
                            null, //task
                            model.TourId, //tour
                            null, //quotation
                            model.Id, //document
                            null, //history
                            null // ticket
                            );
                        Session["TourFile"] = null;
                        var list = _documentFileRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false)
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
                        return PartialView("~/Views/TourTabInfo/_TaiLieuMau.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/TourTabInfo/_TaiLieuMau.cshtml");
                    }
                }
            }
            catch
            {
            }

            return PartialView("~/Views/TourTabInfo/_TaiLieuMau.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteDocument(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 88);
                int tourId = _documentFileRepository.FindId(id).TourId ?? 0;
                //file
                tbl_DocumentFile documentFile = _documentFileRepository.FindId(id) ?? new tbl_DocumentFile();
                String path = Server.MapPath("~/Upload/file/" + documentFile.FileName);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                //end file
                var listIds = id.ToString().Split(',').ToArray();
                //
                var item = _documentFileRepository.FindId(id);
                UpdateHistory.SaveHistory(88, "Xóa tài liệu: " + item.Code,
                    null, //appointment
                    item.ContractId, //contract
                    item.CustomerId, //customer
                    item.PartnerId, //partner
                    item.ProgramId, //program
                    null, //task
                    item.TourId, //tour
                    null, //quotation
                    item.Id, //document
                    null, //history
                    null // ticket
                            );
                //
                if (await _documentFileRepository.DeleteMany(listIds, false))
                {
                    var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.TourId == tourId).Where(p => p.IsDelete == false)
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
                    return PartialView("~/Views/TourTabInfo/_TaiLieuMau.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_TaiLieuMau.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_TaiLieuMau.cshtml");
            }
        }

        #endregion

        #region Nhiệm vụ

        [HttpPost]
        public async Task<ActionResult> EditTask(int id)
        {
            var model = await _taskRepository.GetById(id);
            int depId = _staffRepository.FindId(Convert.ToInt32(model.Permission)).DepartmentId ?? 0;
            ViewBag.DepartmentId = depId;
            ViewBag.PermissionList = _staffRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.DepartmentId == depId && p.IsVietlike == true);
            if (clsPermission.GetUser().StaffID == 9 || model.TaskStatusId <= 1195)
            {
                return PartialView("_Partial_EditTaskTour", model);
            }
            else
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateTask(tbl_Task model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 78);
                string oldPermission = _db.tbl_Task.Find(model.Id).Permission;
                model.ModifiedDate = DateTime.Now;
                if (model.EndDate != null)
                {
                    model.Time = Int32.Parse((model.EndDate.Value - model.StartDate.Value).TotalDays.ToString());
                }
                if (await _taskRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(78, "Cập nhật nhiệm vụ: " + model.Name,
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

                    if (oldPermission != model.Permission)
                    {
                        var check = _taskStaffRepository.GetAllAsQueryable().AsEnumerable().FirstOrDefault(p => p.StaffId.ToString() == oldPermission && p.TaskId == model.Id);
                        var update = _db.tbl_TaskStaff.Find(check.Id);
                        update.StaffId = Convert.ToInt32(model.Permission);
                        _db.SaveChanges();
                    }

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
                else
                {
                    return PartialView("~/Views/TourTabInfo/_NhiemVu.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_NhiemVu.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTask(int id)
        {
            int tourId = _taskRepository.FindId(id).TourId ?? 0;
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 78);
                var listIds = id.ToString().Split(',').ToArray();
                //
                var item = _taskRepository.FindId(id);
                UpdateHistory.SaveHistory(78, "Xóa nhiệm vụ: " + item.Name,
                    null, //appointment
                    null, //contract
                    item.CustomerId, //customer
                    null, //partner
                    null, //program
                    item.Id, //task
                    item.TourId, //tour
                    null, //quotation
                    null, //document
                    null, //history
                    null // ticket
                    );
                //
                if (await _taskRepository.DeleteMany(listIds, false))
                {
                    var list = _taskRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == tourId).Where(p => p.IsDelete == false)
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
                                Note = p.Note
                            }).ToList();
                    return PartialView("~/Views/TourTabInfo/_NhiemVu.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_NhiemVu.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_NhiemVu.cshtml");
            }
        }
        #endregion

        #region Chương trình
        /********** Quản lý chương trình ************/

        [HttpPost]
        public ActionResult UploadProgram(HttpPostedFileBase FileName)
        {
            if (FileName != null && FileName.ContentLength > 0)
            {
                Session["ProgramTourFile"] = FileName;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateProgram(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 80);
                string id = Session["idTour"].ToString();
                var tour = _tourRepository.FindId(Convert.ToInt32(id));
                string FileSize = "", newName = "";
                if (ModelState.IsValid)
                {
                    if (Session["ProgramTourFile"] != null)
                    {
                        HttpPostedFileBase FileName = Session["ProgramTourFile"] as HttpPostedFileBase;
                        FileSize = Common.ConvertFileSize(FileName.ContentLength);
                        newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                        String path = Server.MapPath("~/Upload/file/" + newName);
                        FileName.SaveAs(path);
                    }
                    // insert chương trình
                    var ct = new tbl_Program
                    {
                        Code = GenerateCode.ProgramCode(),
                        TourId = Convert.ToInt32(id),
                        CreatedDate = DateTime.Now,
                        CurrencyId = 1209,
                        StartDate = tour.StartDate,
                        EndDate = tour.EndDate,
                        CustomerId = tour.CustomerId,
                        DictionaryId = 30,
                        IsDelete = false,
                        ModifiedDate = DateTime.Now,
                        Name = form["Name"],
                        Note = model.Note,
                        StatusId = Convert.ToInt32(form["StatusProgramId"]),
                        StaffId = clsPermission.GetUser().StaffID
                    };
                    //
                    if (tour.CustomerId != null)
                    {
                        ct.CustomerId = tour.CustomerId;
                    }
                    //
                    if (newName != "")
                    {
                        ct.FileSize = FileSize;
                        ct.FileName = newName;
                    }
                    if (await _programRepository.Create(ct))
                    {
                        UpdateHistory.SaveHistory(80, "Thêm mới chương trình, code: " + ct.Code + " - " + ct.Name,
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
                    model.CustomerId = tour.CustomerId;
                    model.Code = GenerateCode.DocumentCode();
                    model.ProgramId = ct.Id;
                    model.TourId = Convert.ToInt32(id);
                    model.DictionaryId = 30;
                    model.CreatedDate = DateTime.Now;
                    model.IsRead = false;
                    model.ModifiedDate = DateTime.Now;
                    if (form["TagsId"] != null && form["TagsId"] != "")
                    {
                        model.TagsId = form["TagsId"].ToString();
                    }
                    model.StaffId = clsPermission.GetUser().StaffID;
                    //file

                    //end file
                    if (newName != null && FileSize != null)
                    {
                        model.FileName = newName;
                        model.FileSize = FileSize;
                    }

                    if (await _documentFileRepository.Create(model))
                    {
                        Session["ProgramTourFile"] = null;
                        var list = _db.tbl_Program.AsEnumerable().Where(p => p.TourId.ToString() == id && p.IsDelete == false)
                                     .Select(p => new tbl_Program
                                     {
                                         Id = p.Id,
                                         FileName = p.FileName,
                                         FileSize = p.FileSize,
                                         Note = p.Note,
                                         CreatedDate = p.CreatedDate,
                                         ModifiedDate = p.ModifiedDate,
                                         TagsId = p.TagsId,
                                         tbl_Staff = _staffRepository.FindId(p.StaffId),
                                         Name = p.Name,
                                         Code = p.Code
                                     }).OrderByDescending(p => p.CreatedDate).ToList();
                        return PartialView("~/Views/TourTabInfo/_ChuongTrinh.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/TourTabInfo/_ChuongTrinh.cshtml");
                    }
                }
            }
            catch { }
            return PartialView("~/Views/TourTabInfo/_ChuongTrinh.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> EditInfoProgram(int id)
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
                Permission(clsPermission.GetUser().PermissionID, 80);
                string FileSize = "", newName = "";
                if (Session["ProgramTourFile"] != null)
                {
                    //file
                    HttpPostedFileBase FileName = Session["ProgramTourFile"] as HttpPostedFileBase;
                    FileSize = Common.ConvertFileSize(FileName.ContentLength);
                    newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    FileName.SaveAs(path);
                }

                var program = _db.tbl_Program.Find(model.Id);
                // chương trình
                program.Name = form["Name"];
                program.StatusId = Convert.ToInt32(form["StatusId"]);
                program.ModifiedDate = DateTime.Now;
                program.Note = model.Note;
                if (newName != "")
                {
                    program.FileSize = FileSize;
                    program.FileName = newName;
                }
                _db.SaveChanges();
                if (newName != "")
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
                        UpdateHistory.SaveHistory(1106, "Thêm mới tài liệu chương trình: " + model.Code + ", khách hàng: " + (_customerRepository.FindId(model.CustomerId) != null ? _customerRepository.FindId(model.CustomerId).Code : ""),
                            null, //appointment
                            null, //contract
                            null, //customer
                            null, //partner
                            program.Id, //program
                            null, //task
                            program.TourId, //tour
                            null, //quotation
                            model.Id, //document
                            null, //history
                            null // ticket
                            );
                    }
                }
                var list = _db.tbl_Program.AsEnumerable().Where(p => p.TourId == model.TourId && p.IsDelete == false)
                                     .Select(p => new tbl_Program
                                     {
                                         Id = p.Id,
                                         FileName = p.FileName,
                                         FileSize = p.FileSize,
                                         Note = p.Note,
                                         CreatedDate = p.CreatedDate,
                                         ModifiedDate = p.ModifiedDate,
                                         TagsId = p.TagsId,
                                         tbl_Staff = _staffRepository.FindId(p.StaffId),
                                         Name = p.Name,
                                         Code = p.Code
                                     }).OrderByDescending(p => p.CreatedDate).ToList();
                return PartialView("~/Views/TourTabInfo/_ChuongTrinh.cshtml", list);
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_ChuongTrinh.cshtml");
            }

        }

        [HttpPost]
        public async Task<ActionResult> DeleteProgram(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 80);
                var program = _programRepository.FindId(id);
                int? tourid = program.TourId;
                //file
                tbl_DocumentFile documentFile = _documentFileRepository.GetAllAsQueryable().FirstOrDefault(p => p.FileName == program.FileName) ?? new tbl_DocumentFile();
                String path = Server.MapPath("~/Upload/file/" + documentFile.FileName);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                //
                UpdateHistory.SaveHistory(80, "Xóa tài liệu chương trình: " + documentFile.Code,
                    null, //appointment
                            null, //contract
                            null, //customer
                            null, //partner
                            program.Id, //program
                            null, //task
                            program.TourId, //tour
                            null, //quotation
                            documentFile.Id, //document
                            null, //history
                            null // ticket
                            );
                await _documentFileRepository.DeleteMany(documentFile.Id.ToString().Split(',').ToArray(), true);
                //
                if (await _programRepository.DeleteMany(id.ToString().Split(',').ToArray(), false))
                {
                    var list = _db.tbl_Program.AsEnumerable().Where(p => p.TourId == tourid && p.IsDelete == false)
                                     .Select(p => new tbl_Program
                                     {
                                         Id = p.Id,
                                         FileName = p.FileName,
                                         FileSize = p.FileSize,
                                         Note = p.Note,
                                         CreatedDate = p.CreatedDate,
                                         ModifiedDate = p.ModifiedDate,
                                         TagsId = p.TagsId,
                                         tbl_Staff = _staffRepository.FindId(p.StaffId),
                                         Name = p.Name,
                                         Code = p.Code
                                     }).OrderByDescending(p => p.CreatedDate).ToList();
                    return PartialView("~/Views/TourTabInfo/_ChuongTrinh.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_ChuongTrinh.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_ChuongTrinh.cshtml");
            }
        }

        #endregion

        #region Hợp đồng

        [HttpPost]
        public ActionResult UploadContract(HttpPostedFileBase FileNameContract)
        {
            if (FileNameContract != null && FileNameContract.ContentLength > 0)
            {
                Session["ContractTourFile"] = FileNameContract;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

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
        public async Task<ActionResult> CreateContract(tbl_Contract model, FormCollection form)
        {
            string id = Session["idTour"].ToString();
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 81);
                var checkCode = _contractRepository.GetAllAsQueryable().FirstOrDefault(p => p.Code == model.Code && p.IsDelete == false);
                if (checkCode == null)
                {
                    model.TourId = Convert.ToInt32(id);
                    model.CreatedDate = DateTime.Now;
                    model.ModifiedDate = DateTime.Now;
                    model.StaffId = clsPermission.GetUser().StaffID;
                    model.DictionaryId = 28;
                    model.LoiNhuanDuKien = model.TotalPrice - model.TongDuKien;
                    model.CurrencyLNDK = model.CurrencyTDK;
                    //
                    var tour = _tourRepository.FindId(model.TourId);
                    if (tour.CustomerId != null)
                    {
                        model.CustomerId = tour.CustomerId;
                    }
                    //
                    if (await _contractRepository.Create(model))
                    {
                        UpdateHistory.SaveHistory(81, "Thêm mới hợp đồng, code: " + model.Code + " - " + model.Name,
                            null, //appointment
                            model.Id, //contract
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
                        // upload file
                        HttpPostedFileBase FileName = Session["ContractTourFile"] as HttpPostedFileBase;
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
                        Session["ContractTourFile"] = null;
                        //
                    }
                }
            }
            catch
            { }

            var docs = _documentFileRepository.GetAllAsQueryable();
            var list = _contractRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false)
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

            return PartialView("~/Views/TourTabInfo/_HopDong.cshtml", list);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateContract(tbl_Contract model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 81);
                model.ModifiedDate = DateTime.Now;
                model.LoiNhuanDuKien = model.TotalPrice - model.TongDuKien;
                model.CurrencyLNDK = model.CurrencyTDK;
                //
                var tour = _tourRepository.FindId(model.TourId);
                if (tour.CustomerId != null)
                {
                    model.CustomerId = tour.CustomerId;
                }
                //
                if (await _contractRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(81, "Cập nhật hợp đồng: " + model.Code,
                        null, //appointment
                            model.Id, //contract
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
                    // upload file
                    if (form["docId"] != "0")
                    {
                        HttpPostedFileBase FileName = Session["ContractTourFile"] as HttpPostedFileBase;
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
                        HttpPostedFileBase FileName = Session["ContractTourFile"] as HttpPostedFileBase;
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
                        Session["ContractTourFile"] = null;
                    }
                    //
                    var docs = _documentFileRepository.GetAllAsQueryable();
                    var list = _contractRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false)
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

                    return PartialView("~/Views/TourTabInfo/_HopDong.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_HopDong.cshtml");
                }
            }
            catch
            { }

            return PartialView("~/Views/TourTabInfo/_HopDong.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteContract(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 81);
                int tourId = _contractRepository.FindId(id).TourId ?? 0;
                var history = _updateHistoryRepository.GetAllAsQueryable().Where(p => p.ContractId == id).Where(p => p.IsDelete == false).ToList();
                if (history.Count() > 0)
                {
                    foreach (var item in history)
                    {
                        var listId = item.Id.ToString().Split(',').ToArray();
                        await _updateHistoryRepository.DeleteMany(listId, false);
                    }
                }

                var listIds = id.ToString().Split(',').ToArray();
                //
                var items = _contractRepository.FindId(id);
                UpdateHistory.SaveHistory(81, "Xóa hợp đồng: " + items.Code,
                        null, //appointment
                        items.Id, //contract
                        null, //customer
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
                if (await _contractRepository.DeleteMany(listIds, false))
                {
                    var docs = _documentFileRepository.GetAllAsQueryable();
                    var list = _contractRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == tourId).Where(p => p.IsDelete == false)
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
                    return PartialView("~/Views/TourTabInfo/_HopDong.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_HopDong.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_HopDong.cshtml");
            }
        }

        #endregion

        #region Đánh giá
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateReviewTour(tbl_ReviewTour model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 86);
                string id = Session["idTour"].ToString();
                model.TourId = Convert.ToInt32(id);
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.StaffId = clsPermission.GetUser().StaffID;
                int count = Convert.ToInt32(form["countService"].ToString());
                double mark = 0;
                for (int i = 1; i <= count; i++)
                {
                    mark += Convert.ToInt32(form["Mark" + i].ToString());
                }
                model.AverageMark = mark / count;

                if (await _reviewTourRepository.Create(model))
                {
                    UpdateHistory.SaveHistory(86, "Thêm đánh giá tour " + _tourRepository.FindId(model.TourId).Name,
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

                    for (int i = 1; i <= count; i++)
                    {
                        var detail = new tbl_ReviewTourDetail
                        {
                            CreatedDate = DateTime.Now,
                            DictionaryId = Convert.ToInt32(form["DictionaryId" + i]),
                            Mark = Convert.ToInt32(form["Mark" + i].ToString()),
                            ModifiedDate = DateTime.Now,
                            ReviewTourId = model.Id
                        };
                        await _reviewTourDetailRepository.Create(detail);
                    }

                    var list = _db.tbl_ReviewTourDetail.AsEnumerable().Where(p => p.tbl_ReviewTour.TourId.ToString() == id).Where(p => p.IsDelete == false)
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
                    return PartialView("~/Views/TourTabInfo/_DanhGia.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_DanhGia.cshtml");
                }
            }
            catch { }

            return PartialView("~/Views/TourTabInfo/_DanhGia.cshtml");
        }
        #endregion

        #region Công nợ đối tác
        public async Task<ActionResult> EditLiabilityPartner(int id)
        {
            var model = await _liabilityPartnerRepository.GetById(id);
            Session["idTour"] = model.TourId;
            return PartialView("_Partial_EditCongNoDoiTac", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateLiabilityPartner(FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 85);
                string id = Session["idTour"].ToString();
                for (int i = 1; i <= Convert.ToInt32(form["NumberOptionCongNo"]); i++)
                {
                    var model = new tbl_LiabilityPartner()
                    {
                        TourId = Convert.ToInt32(id),
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        StaffId = clsPermission.GetUser().StaffID,
                        ServiceId = Convert.ToInt32(form["ServiceId" + i].ToString()),
                        Note = form["Note" + i] != null && form["Note" + i] != "" ? form["Note" + i].ToString() : "",
                        PaymentMethod = form["PaymentMethod" + i] != null && form["PaymentMethod" + i] != "" ? Convert.ToInt32(form["PaymentMethod" + i].ToString()) : 0,
                        SecondPayment = form["SecondPayment" + i] != null && form["SecondPayment" + i] != "" ? Convert.ToDecimal(form["SecondPayment" + i].ToString()) : 0,
                        FirstPayment = form["FirstPayment" + i] != null && form["FirstPayment" + i] != "" ? Convert.ToDecimal(form["FirstPayment" + i].ToString()) : 0,
                        TotalRemaining = form["TotalRemaining" + i] != null && form["TotalRemaining" + i] != "" ? Convert.ToDecimal(form["TotalRemaining" + i].ToString()) : 0,
                        ServicePrice = form["ServicePrice" + i] != null && form["ServicePrice" + i] != "" ? Convert.ToDecimal(form["ServicePrice" + i].ToString()) : 0,
                        FirstCurrencyType = Convert.ToInt32(form["FirstCurrencyType" + i].ToString()),
                        SecondCurrencyType = Convert.ToInt32(form["FirstCurrencyType" + i].ToString()),
                    };
                    if (Convert.ToInt32(form["PartnerId" + i]) != 0)
                    {
                        model.PartnerId = Convert.ToInt32(form["PartnerId" + i].ToString());
                    }
                    UpdateHistory.SaveHistory(85, "Thêm công nợ đối tác cho tour " + _tourRepository.FindId(model.TourId).Name,
                            null, //appointment
                            null, //contract
                            null, //customer
                            model.PartnerId, //partner
                            null, //program
                            null, //task
                            model.TourId, //tour
                            null, //quotation
                            null, //document
                            null, //history
                            null // ticket
                        );
                    await _liabilityPartnerRepository.Create(model);
                }
                var list = _liabilityPartnerRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId.ToString() == id).Where(p => p.IsDelete == false)
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

                return PartialView("~/Views/TourTabInfo/_CongNoDoiTac.cshtml", list);
            }
            catch
            { }

            return PartialView("~/Views/TourTabInfo/_CongNoDoiTac.cshtml");
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateLiabilityPartner(tbl_LiabilityPartner model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 85);
                model.ModifiedDate = DateTime.Now;
                model.FirstPayment = form["FirstPayment"] != "" ? Convert.ToDecimal(form["FirstPayment"].ToString()) : 0;
                model.SecondPayment = form["SecondPayment"] != "" ? Convert.ToDecimal(form["SecondPayment"].ToString()) : 0;
                model.ServicePrice = form["ServicePrice"] != "" ? Convert.ToDecimal(form["ServicePrice"].ToString()) : 0;
                model.TotalRemaining = form["TotalRemaining"] != "" ? Convert.ToDecimal(form["TotalRemaining"].ToString()) : 0;
                if (await _liabilityPartnerRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(85, "Cập nhật công nợ đối tác cho tour " + _tourRepository.FindId(model.TourId).Name,
                        null, //appointment
                            null, //contract
                            null, //customer
                            model.PartnerId, //partner
                            null, //program
                            null, //task
                            model.TourId, //tour
                            null, //quotation
                            null, //document
                            null, //history
                            null // ticket
                            );
                    var list = _liabilityPartnerRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false)
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
                    return PartialView("~/Views/TourTabInfo/_CongNoDoiTac.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_CongNoDoiTac.cshtml");
                }
            }
            catch
            { }

            return PartialView("~/Views/TourTabInfo/_CongNoDoiTac.cshtml");
        }


        [HttpPost]
        public async Task<ActionResult> DeleteLiabilityPartner(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 85);
                int tourId = _liabilityPartnerRepository.FindId(id).TourId;
                Session["idTour"] = tourId;
                var listIds = id.ToString().Split(',').ToArray();
                //
                var item = _liabilityPartnerRepository.FindId(id);
                UpdateHistory.SaveHistory(85, "Xóa công nợ đối tác của tour: " + _tourRepository.FindId(tourId).Code,
                    null, //appointment
                            null, //contract
                            null, //customer
                            item.PartnerId, //partner
                            null, //program
                            null, //task
                            tourId, //tour
                            null, //quotation
                            null, //document
                            null, //history
                            null // ticket
                            );
                //
                if (await _liabilityPartnerRepository.DeleteMany(listIds, false))
                {
                    var list = _liabilityPartnerRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == tourId).Where(p => p.IsDelete == false)
                       .Select(p => new tbl_LiabilityPartner
                       {
                           Id = p.Id,
                           tbl_Staff = _staffRepository.FindId(p.StaffId),
                           tbl_Partner = _partnerRepository.FindId(p.PartnerId),
                           PaymentMethod = p.PaymentMethod,
                           ServicePrice = p.ServicePrice,
                           FirstPayment = p.FirstPayment,
                           SecondPayment = p.SecondPayment,
                           TotalRemaining = p.TotalRemaining
                       }).ToList();
                    return PartialView("~/Views/TourTabInfo/_CongNoDoiTac.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_CongNoDoiTac.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_CongNoDoiTac.cshtml");
            }
        }
        #endregion

        #region Công nợ khách hàng
        public async Task<ActionResult> EditLiabilityCustomer(int id)
        {
            var model = await _liabilityCustomerRepository.GetById(id);
            Session["idTour"] = model.TourId;
            return PartialView("_Partial_EditCongNoKhachHang", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateLiabilityCustomer(tbl_LiabilityCustomer model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 84);
                string id = Session["idTour"].ToString();
                model.TourId = Convert.ToInt32(id);
                model.CreateDate = DateTime.Now;
                model.CustomerId = _tourRepository.FindId(model.TourId).CustomerId;
                model.StaffId = clsPermission.GetUser().StaffID;

                if (await _liabilityCustomerRepository.Create(model))
                {
                    UpdateHistory.SaveHistory(84, "Thêm công nợ khách hàng cho tour " + _tourRepository.FindId(model.TourId).Name,
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
                    var list = _liabilityCustomerRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false).ToList();
                    return PartialView("~/Views/TourTabInfo/_CongNoKH.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_CongNoKH.cshtml");
                }
            }
            catch
            { }

            return PartialView("~/Views/TourTabInfo/_CongNoKH.cshtml");
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateLiabilityCustomer(tbl_LiabilityCustomer model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 84);
                model.TotalContract = form["TotalContract"] != "" ? Convert.ToDecimal(form["TotalContract"].ToString()) : 0;
                model.FirstPrice = form["FirstPrice"] != "" ? Convert.ToDecimal(form["FirstPrice"].ToString()) : 0;
                model.SecondPrice = form["SecondPrice"] != "" ? Convert.ToDecimal(form["SecondPrice"].ToString()) : 0;
                model.ThirdPrice = form["ThirdPrice"] != "" ? Convert.ToDecimal(form["ThirdPrice"].ToString()) : 0;
                model.TotalLiquidation = form["TotalLiquidation"] != "" ? Convert.ToDecimal(form["TotalLiquidation"].ToString()) : 0;
                model.TotalRemaining = form["TotalRemaining"] != "" ? Convert.ToDecimal(form["TotalRemaining"].ToString()) : 0;

                if (await _liabilityCustomerRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(84, "Cập nhật công nợ khách hàng cho tour " + _tourRepository.FindId(model.TourId).Name,
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
                    var list = _liabilityCustomerRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false).ToList();
                    return PartialView("~/Views/TourTabInfo/_CongNoKH.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_CongNoKH.cshtml");
                }
            }
            catch
            { }

            return PartialView("~/Views/TourTabInfo/_CongNoKH.cshtml");
        }


        [HttpPost]
        public async Task<ActionResult> DeleteLiabilityCustomer(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 84);
                int tourId = _liabilityCustomerRepository.FindId(id).TourId;
                Session["idTour"] = tourId;
                var listIds = id.ToString().Split(',').ToArray();
                //
                var item = _liabilityCustomerRepository.FindId(id);
                UpdateHistory.SaveHistory(84, "Xóa công nợ khách hàng của tour: " + _tourRepository.FindId(tourId).Code,
                    null, //appointment
                            null, //contract
                            null, //customer
                            null, //partner
                            null, //program
                            null, //task
                            tourId, //tour
                            null, //quotation
                            null, //document
                            null, //history
                            null // ticket
                );
                //
                if (await _liabilityCustomerRepository.DeleteMany(listIds, false))
                {
                    var list = _liabilityCustomerRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == tourId).Where(p => p.IsDelete == false).ToList();
                    return PartialView("~/Views/TourTabInfo/_CongNoKH.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_CongNoKH.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_CongNoKH.cshtml");
            }
        }
        #endregion

        #region Onsuccsess
        public JsonResult CNKH()
        {
            int id = Int32.Parse(Session["idTour"].ToString());
            decimal CongNoKhachHang = _liabilityCustomerRepository.GetAllAsQueryable().Where(c => c.TourId == id && c.IsDelete == false).Sum(c => c.TotalRemaining) ?? 0;

            var obj = new
            {
                Id = id,
                CongNo = CongNoKhachHang
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CNDT()
        {
            int id = Int32.Parse(Session["idTour"].ToString());
            decimal CongNoDoiTac = _liabilityPartnerRepository.GetAllAsQueryable().Where(c => c.TourId == id && c.IsDelete == false).Sum(c => c.TotalRemaining) ?? 0;

            var obj = new
            {
                Id = id,
                CongNo = CongNoDoiTac
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Visa
        [HttpPost]
        public async Task<ActionResult> DeleteVisa(int id)
        {
            int tourId = Int16.Parse(Session["idTour"].ToString());
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 82);
                int tourvisa = _tourCustomerVisaRepository.GetAllAsQueryable().AsEnumerable().FirstOrDefault(c => c.CustomerId == id & c.TourId == tourId).Id;
                var listIds = tourvisa.ToString().Split(',').ToArray();
                //
                var item = _tourCustomerVisaRepository.FindId(id).tbl_CustomerVisa.tbl_Customer;
                UpdateHistory.SaveHistory(82, "Xóa visa khách hàng: " + item.FullName,
                    null, //appointment
                    null, //contract
                    item.Id, //customer
                    null, //partner
                    null, //program
                    null, //task
                    tourId, //tour
                    null, //quotation
                    null, //document
                    null, //history
                    null // ticket
                );
                //
                if (await _tourCustomerVisaRepository.DeleteMany(listIds, false))
                {
                    var list = _tourCustomerVisaRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.TourId == tourId).Where(p => p.IsDelete == false).Select(c => c.tbl_CustomerVisa).ToList();
                    return PartialView("~/Views/TourTabInfo/_Visa.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_Visa.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_Visa.cshtml");
            }
        }

        [HttpPost]
        public ActionResult EditVisa(int id)
        {
            int idTour = Int16.Parse(Session["idTour"].ToString());
            var model = _customerVisaRepository.FindId(id);
            return PartialView("_Partial_EditVisaCustomer", model);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateVisaCustomer(tbl_CustomerVisa model, FormCollection form)
        {
            int idTour = Int16.Parse(Session["idTour"].ToString());
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 82);
                model.ModifiedDate = DateTime.Now;
                model.IsDelete = false;
                if (await _customerVisaRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(82, "Cập nhật visa khách hàng " + model.VisaNumber,
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
                var items = _db.tbl_TourCustomerVisa.AsEnumerable()
                            .Where(c => c.TourId == idTour && c.IsDelete == false).Select(c => c.tbl_CustomerVisa).ToList();
                return PartialView("~/Views/TourTabInfo/_Visa.cshtml", items);
            }
            catch
            {
                var items = _db.tbl_TourCustomerVisa.AsEnumerable()
                            .Where(c => c.TourId == idTour && c.IsDelete == false).Select(c => c.tbl_CustomerVisa).ToList();
                return PartialView("~/Views/TourTabInfo/_Visa.cshtml", items);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateVisa(tbl_CustomerVisa model, FormCollection form)
        {
            try
            {
                string note = form["NoteVisa"];
                Permission(clsPermission.GetUser().PermissionID, 82);
                int idTour = Int16.Parse(Session["idTour"].ToString());
                if (form["listVisaId"] != null && form["listVisaId"] != "")
                {
                    var listIds = form["listVisaId"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        foreach (var _id in listIds)
                        {
                            int id = Int16.Parse(_id);
                            var visa = _customerVisaRepository.FindId(id);
                            visa.DictionaryId = model.DictionaryId;
                            visa.tbl_Customer.NoteVisa = note;
                            if (await _customerVisaRepository.Update(visa))
                            {
                                UpdateHistory.SaveHistory(82, "Cập nhật visa khách hàng " + visa.VisaNumber,
                                    null, //appointment
                                    null, //contract
                                    visa.CustomerId, //customer
                                    null, //partner
                                    null, //program
                                    null, //task
                                    visa.TourId, //tour
                                    null, //quotation
                                    null, //document
                                    null, //history
                                    null // ticket
                                    );
                            }
                        }
                    }
                }
                var list = _tourCustomerVisaRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.TourId == idTour).Where(p => p.IsDelete == false).Select(c => c.tbl_CustomerVisa).ToList();

                return PartialView("~/Views/TourTabInfo/_Visa.cshtml", list);
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_Visa.cshtml");
            }
        }

        #endregion

        //#region Báo giá

        //[HttpPost]
        //public ActionResult UploadFileQuotation(HttpPostedFileBase FileNameQuotation)
        //{
        //    if (FileNameQuotation != null && FileNameQuotation.ContentLength > 0)
        //    {
        //        Session["QuotationFile"] = FileNameQuotation;
        //    }
        //    return Json(JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public ActionResult UploadMultipleFileQuotation(IEnumerable<HttpPostedFileBase> FileNameQuotation)
        //{
        //    if (FileNameQuotation != null)
        //    {
        //        Session["QuotationFile"] = FileNameQuotation;
        //    }
        //    return Json(JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public async Task<ActionResult> EditQuotation(int id)
        //{
        //    var model = await _quotationRepository.GetById(id);
        //    return PartialView("_Partial_EditQuotation", model);
        //}

        //[HttpPost]
        //[ValidateInput(false)]
        //public async Task<ActionResult> CreateQuotation(tbl_Quotation model, FormCollection form)
        //{
        //    try
        //    {
        //        Permission(clsPermission.GetUser().PermissionID, 83);
        //        string id = Session["idTour"].ToString();
        //        model.Code = GenerateCode.QuotationCode();
        //        //model.TourId = Convert.ToInt32(id);
        //        model.StartDate = DateTime.Now;
        //        model.EndDate = DateTime.Now;
        //        model.CreatedDate = DateTime.Now;
        //        model.ModifiedDate = DateTime.Now;
        //        model.TagsId = form["TagsId"];
        //        model.DictionaryId = 29;
        //        model.StaffId = clsPermission.GetUser().StaffID;
        //        model.QuotationDate = form["QuotationDate"] != "" ? Convert.ToDateTime(form["QuotationDate"].ToString()) : DateTime.Now;
        //        //
        //        var tour = _tourRepository.FindId(id);
        //        if (tour.CustomerId != null)
        //        {
        //            model.CustomerId = tour.CustomerId;
        //        }
        //        //
        //        if (Session["QuotationFile"] != null)
        //        {
        //            //file
        //            IEnumerable<HttpPostedFileBase> FileName = Session["QuotationFile"] as IEnumerable<HttpPostedFileBase>;
        //            foreach (var file in FileName)
        //            {
        //                string FileSize = Common.ConvertFileSize(file.ContentLength);
        //                String newName = file.FileName.Insert(file.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
        //                String path = Server.MapPath("~/Upload/file/" + newName);
        //                file.SaveAs(path);
        //                //end file

        //                if (file != null && FileSize != null)
        //                {
        //                    String pathOld = Server.MapPath("~/Upload/file/" + model.FileName);
        //                    if (System.IO.File.Exists(pathOld))
        //                        System.IO.File.Delete(pathOld);
        //                    model.FileName = newName;
        //                }
        //                if (await _quotationRepository.Create(model))
        //                {
        //                    UpdateHistory.SaveHistory(83, "Thêm mới hợp đồng, code: " + model.Code,
        //                        null, //appointment
        //                        model.Id, //contract
        //                        null, //customer
        //                        null, //partner
        //                        null, //program
        //                        null, //task
        //                        null, //tour
        //                        null, //quotation
        //                        null, //document
        //                        null, //history
        //                        null // ticket
        //                        );
        //                }
        //            }
        //            Session["QuotationFile"] = null;
        //        }

        //        //var list = _quotationRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false)
        //        //        .Select(p => new tbl_Quotation
        //        //        {
        //        //            Id = p.Id,
        //        //            Code = p.Code,
        //        //            QuotationDate = p.QuotationDate,
        //        //            tbl_StaffQuotation = _staffRepository.FindId(p.StaffQuotationId),
        //        //            tbl_Staff = _staffRepository.FindId(p.StaffId),
        //        //            PriceTour = p.PriceTour,
        //        //            tbl_DictionaryCurrency = _dictionaryRepository.FindId(p.CurrencyId),
        //        //            FileName = p.FileName,
        //        //            Note = p.Note,
        //        //            CreatedDate = p.CreatedDate,
        //        //            StaffId = p.StaffId,
        //        //            StaffQuotationId = p.StaffQuotationId,
        //        //            ModifiedDate = p.ModifiedDate,
        //        //            CountryId = p.CountryId,
        //        //            tbl_TagsCountry = _tagsRepository.FindId(p.CountryId)
        //        //        }).ToList();
        //        return PartialView("~/Views/TourTabInfo/_ViettourBaoGia.cshtml", list);
        //    }
        //    catch
        //    {
        //        return PartialView("~/Views/TourTabInfo/_ViettourBaoGia.cshtml");
        //    }
        //}

        //[HttpPost]
        //[ValidateInput(false)]
        //public async Task<ActionResult> UpdateQuotation(tbl_Quotation model, FormCollection form)
        //{
        //    try
        //    {
        //        Permission(clsPermission.GetUser().PermissionID, 83);
        //        model.ModifiedDate = DateTime.Now;
        //        if (form["TagsId"] != null && form["TagsId"] != "")
        //        {
        //            model.TagsId = form["TagsId"].ToString();
        //        }
        //        model.StaffId = clsPermission.GetUser().StaffID;
        //        //
        //        var tour = _tourRepository.FindId(model.TourId);
        //        if (tour.CustomerId != null)
        //        {
        //            model.CustomerId = tour.CustomerId;
        //        }
        //        //
        //        if (Session["QuotationFile"] != null)
        //        {
        //            //file
        //            HttpPostedFileBase FileName = Session["QuotationFile"] as HttpPostedFileBase;
        //            string FileSize = Common.ConvertFileSize(FileName.ContentLength);
        //            String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
        //            String path = Server.MapPath("~/Upload/file/" + newName);
        //            FileName.SaveAs(path);
        //            //end file

        //            if (FileName != null && FileSize != null)
        //            {
        //                String pathOld = Server.MapPath("~/Upload/file/" + model.FileName);
        //                if (System.IO.File.Exists(pathOld))
        //                    System.IO.File.Delete(pathOld);
        //                model.FileName = newName;
        //            }
        //        }

        //        if (await _quotationRepository.Update(model))
        //        {
        //            UpdateHistory.SaveHistory(83, "Cập nhật báo giá: " + model.Code,
        //                null, //appointment
        //                null, //contract
        //                null, //customer
        //                null, //partner
        //                null, //program
        //                null, //task
        //                model.TourId, //tour
        //                model.Id, //quotation
        //                null, //document
        //                null, //history
        //                null // ticket
        //                );
        //            Session["QuotationFile"] = null;
        //            var list = _quotationRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false)
        //                .Select(p => new tbl_Quotation
        //                {
        //                    Id = p.Id,
        //                    Code = p.Code,
        //                    QuotationDate = p.QuotationDate,
        //                    tbl_StaffQuotation = _staffRepository.FindId(p.StaffQuotationId),
        //                    tbl_Staff = _staffRepository.FindId(p.StaffId),
        //                    PriceTour = p.PriceTour,
        //                    tbl_DictionaryCurrency = _dictionaryRepository.FindId(p.CurrencyId),
        //                    FileName = p.FileName,
        //                    Note = p.Note,
        //                    CreatedDate = p.CreatedDate,
        //                    ModifiedDate = p.ModifiedDate,
        //                    StaffId = p.StaffId,
        //                    StaffQuotationId = p.StaffQuotationId,
        //                    CountryId = p.CountryId,
        //                    tbl_TagsCountry = _tagsRepository.FindId(p.CountryId)
        //                }).ToList();
        //            return PartialView("~/Views/TourTabInfo/_ViettourBaoGia.cshtml", list);
        //        }
        //        else
        //        {
        //            return PartialView("~/Views/TourTabInfo/_ViettourBaoGia.cshtml");
        //        }
        //    }
        //    catch
        //    {
        //        return PartialView("~/Views/TourTabInfo/_ViettourBaoGia.cshtml");
        //    }
        //}

        //[HttpPost]
        //public async Task<ActionResult> DeleteQuotation(int id)
        //{
        //    try
        //    {
        //        Permission(clsPermission.GetUser().PermissionID, 83);
        //        int tourId = _quotationRepository.FindId(id).TourId ?? 0;
        //        //file
        //        tbl_Quotation documentFile = _quotationRepository.FindId(id) ?? new tbl_Quotation();
        //        String path = Server.MapPath("~/Upload/file/" + documentFile.FileName);
        //        if (System.IO.File.Exists(path))
        //            System.IO.File.Delete(path);
        //        //end file
        //        var listIds = id.ToString().Split(',').ToArray();
        //        //
        //        var item = _quotationRepository.FindId(id);
        //        UpdateHistory.SaveHistory(83, "Xóa báo giá: " + item.Code,
        //            null, //appointment
        //                null, //contract
        //                null, //customer
        //                null, //partner
        //                null, //program
        //                null, //task
        //                null, //tour
        //                item.Id, //quotation
        //                null, //document
        //                null, //history
        //                null // ticket
        //                );
        //        //
        //        if (await _quotationRepository.DeleteMany(listIds, false))
        //        {
        //            var list = _quotationRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == tourId && p.IsDelete == false)
        //                .Select(p => new tbl_Quotation
        //                {
        //                    Id = p.Id,
        //                    Code = p.Code,
        //                    QuotationDate = p.QuotationDate,
        //                    tbl_StaffQuotation = _staffRepository.FindId(p.StaffQuotationId),
        //                    tbl_Staff = _staffRepository.FindId(p.StaffId),
        //                    PriceTour = p.PriceTour,
        //                    tbl_DictionaryCurrency = _dictionaryRepository.FindId(p.CurrencyId),
        //                    FileName = p.FileName,
        //                    Note = p.Note,
        //                    CreatedDate = p.CreatedDate,
        //                    ModifiedDate = p.ModifiedDate,
        //                    StaffId = p.StaffId,
        //                    StaffQuotationId = p.StaffQuotationId,
        //                    CountryId = p.CountryId,
        //                    tbl_TagsCountry = _tagsRepository.FindId(p.CountryId)
        //                }).ToList();
        //            return PartialView("~/Views/TourTabInfo/_ViettourBaoGia.cshtml", list);
        //        }
        //        else
        //        {
        //            return PartialView("~/Views/TourTabInfo/_ViettourBaoGia.cshtml");
        //        }
        //    }
        //    catch
        //    {
        //        return PartialView("~/Views/TourTabInfo/_ViettourBaoGia.cshtml");
        //    }
        //}

        //#endregion

        #region Hướng dẫn viên
        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase Image)
        {
            if (Image != null && Image.ContentLength > 0)
            {
                Session["ImageGuide"] = Image;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadMultipleFile(IEnumerable<HttpPostedFileBase> File)
        {
            if (File != null)
            {
                Session["FileGuide"] = File;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateGuide(TourGuideViewModel model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1115);
                string id = Session["idTour"].ToString();
                var tour = _tourRepository.FindId(Convert.ToInt32(id));

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
                        StaffId = clsPermission.GetUser().StaffID,
                    };
                    if (Session["ImageGuide"] != null)
                    {
                        //file
                        HttpPostedFileBase Image = Session["ImageGuide"] as HttpPostedFileBase;
                        string FileSize = Common.ConvertFileSize(Image.ContentLength);
                        String newName = Image.FileName.Insert(Image.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                        String path = Server.MapPath("~/Upload/file/" + newName);
                        Image.SaveAs(path);
                        staff.Image = newName;
                        //end file
                    }
                    await _staffRepository.Create(staff);
                    Session["ImageGuide"] = null;

                    // lưu tourguide
                    var tourguide = new tbl_TourGuide
                    {
                        CreateDate = DateTime.Now,
                        IsDelete = false,
                        StaffId = staff.Id,
                        TourId = Convert.ToInt32(id),
                        TagId = tour.DestinationPlace ?? 0,
                        StartDate = model.SingleGuide.StartDate,
                        EndDate = model.SingleGuide.EndDate,
                    };

                    if (await _tourGuideRepository.Create(tourguide))
                    {
                        // lịch tour của hướng dẫn viên
                        var lichditour = new tbl_TourSchedule()
                        {
                            TourId = Convert.ToInt32(id),
                            StaffId = staff.StaffId,
                            CreatedDate = DateTime.Now,
                            StartDate = model.SingleGuide.StartDate,
                            EndDate = model.SingleGuide.EndDate,
                            IsDelete = false
                        };
                        await _tourScheduleRepository.Create(lichditour);
                    }
                }
                else // chon nhan vien cong ty
                {
                    // update thong tin nhan vien
                    var staff = _db.tbl_Staff.Find(Convert.ToInt32(form["StaffId"]));
                    staff.Birthday = model.SingleStaff.Birthday;
                    staff.CodeGuide = model.SingleStaff.CodeGuide;
                    staff.FullName = model.SingleStaff.FullName;
                    if (Session["ImageGuide"] != null)
                    {
                        var Image = Session["ImageGuide"] as HttpPostedFileBase;
                        //file
                        string FileSize = Common.ConvertFileSize(Image.ContentLength);
                        String newName = Image.FileName.Insert(Image.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                        String path = Server.MapPath("~/Upload/file/" + newName);
                        Image.SaveAs(path);
                        staff.Image = newName;
                        //end file
                    }
                    _db.SaveChanges();
                    Session["ImageGuide"] = null;

                    // lưu tourguide
                    var tourguide = new tbl_TourGuide
                    {
                        CreateDate = DateTime.Now,
                        IsDelete = false,
                        StaffId = Convert.ToInt32(form["StaffId"]),
                        TourId = Convert.ToInt32(id),
                        TagId = tour.DestinationPlace ?? 0,
                        StartDate = model.SingleGuide.StartDate,
                        EndDate = model.SingleGuide.EndDate,
                    };

                    if (await _tourGuideRepository.Create(tourguide))
                    {
                        // lịch tour của hướng dẫn viên
                        var lichditour = new tbl_TourSchedule()
                        {
                            TourId = Convert.ToInt32(id),
                            StaffId = Convert.ToInt32(form["StaffId"]),
                            CreatedDate = DateTime.Now,
                            StartDate = model.SingleGuide.StartDate,
                            EndDate = model.SingleGuide.EndDate,
                            IsDelete = false
                        };
                        await _tourScheduleRepository.Create(lichditour);
                    }
                }

                // lưu tài liệu của hướng dẫn viên
                if (Session["FileGuide"] != null)
                {
                    var File = Session["FileGuide"] as IEnumerable<HttpPostedFileBase>;
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
                                PermissionStaff = model.SingleGuide.StaffId.ToString(),
                                StaffId = clsPermission.GetUser().StaffID,
                                TourId = Convert.ToInt32(id)
                            };
                            await _documentFileRepository.Create(f);
                            Session["FileGuide"] = null;
                        }
                    }
                }

                var list = _tourGuideRepository.GetAllAsQueryable().AsEnumerable()
                            .Where(p => p.TourId == tour.Id && p.IsDelete == false)
                            .Select(p => new GuideListViewModel
                            {
                                StaffId = p.StaffId ?? 0,
                                GuideId = p.Id,
                                Name = _staffRepository.FindId(p.StaffId).FullName,
                                Birthday = _staffRepository.FindId(p.StaffId).Birthday != null ? string.Format("{0:dd-MM-yyyy}", _staffRepository.FindId(p.StaffId).Birthday) : "",
                                CodeGuide = _staffRepository.FindId(p.StaffId).CodeGuide,
                                File = _documentFileRepository.GetAllAsQueryable().Where(x => x.PermissionStaff != null && x.PermissionStaff.Contains(p.StaffId.ToString()) && x.IsDelete == false).ToList(),
                                Image = _staffRepository.FindId(p.StaffId).Image,
                                StartDate = p.StartDate != null ? string.Format("{0:dd-MM-yyyy}", p.StartDate) : "",
                                EndDate = p.EndDate != null ? string.Format("{0:dd-MM-yyyy}", p.EndDate) : "",
                                CreateDate = _staffRepository.FindId(p.StaffId).CreatedDate
                            }).OrderByDescending(p => p.CreateDate).ToList();
                return PartialView("~/Views/TourTabInfo/_HuongDanVien.cshtml", list);
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_HuongDanVien.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditInfoGuide(int id)
        {
            var item = await _tourGuideRepository.GetById(id);
            var model = new TourGuideViewModel()
            {
                SingleGuide = item,
                SingleStaff = item.tbl_Staff
            };
            return PartialView("_Partial_EditGuide", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateGuide(TourGuideViewModel model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1115);
                var staff = _db.tbl_Staff.Find(model.SingleStaff.Id);

                // sửa nhân viên
                staff.ModifiedDate = DateTime.Now;
                staff.StaffId = clsPermission.GetUser().StaffID;
                staff.IsDelete = false;
                staff.Code = model.SingleStaff.CodeGuide;
                staff.Birthday = model.SingleStaff.Birthday;
                staff.CodeGuide = model.SingleStaff.CodeGuide;
                staff.FullName = model.SingleStaff.FullName;

                // image
                if (Session["ImageGuide"] != null)
                {
                    var Image = Session["ImageGuide"] as HttpPostedFileBase;
                    //file
                    string FileSize = Common.ConvertFileSize(Image.ContentLength);
                    String newName = Image.FileName.Insert(Image.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    Image.SaveAs(path);
                    staff.Image = newName;
                    //end file
                }
                _db.SaveChanges();
                Session["ImageGuide"] = null;

                // sửa tourguide
                model.SingleGuide.StaffId = model.SingleStaff.Id;
                if (await _tourGuideRepository.Update(model.SingleGuide))
                {
                    UpdateHistory.SaveHistory(1115, "Cập nhật hướng dẫn viên " + _staffRepository.FindId(model.SingleGuide.StaffId).FullName,
                        null, //appointment
                        null, //contract
                        null, //customer
                        null, //partner
                        null, //program
                        null, //task
                        model.SingleGuide.TourId, //tour
                        null, //quotation
                        null, //document
                        null, //history
                        null // ticket
                        );
                }

                // sửa tourschedule

                // thêm file tài liệu
                if (Session["FileGuide"] != null)
                {
                    var File = Session["FileGuide"] as IEnumerable<HttpPostedFileBase>;
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
                                PermissionStaff = model.SingleStaff.Id.ToString(),
                                StaffId = clsPermission.GetUser().StaffID,
                                TourId = Convert.ToInt32(model.SingleGuide.TourId)
                            };
                            await _documentFileRepository.Create(f);
                        }
                    }
                    Session["FileGuide"] = null;
                }

                var list = _tourGuideRepository.GetAllAsQueryable().AsEnumerable()
                            .Where(p => p.TourId == model.SingleGuide.TourId && p.IsDelete == false)
                             .Select(p => new GuideListViewModel
                             {
                                 StaffId = p.StaffId ?? 0,
                                 GuideId = p.Id,
                                 Name = _staffRepository.FindId(p.StaffId).FullName,
                                 Birthday = _staffRepository.FindId(p.StaffId).Birthday != null ? string.Format("{0:dd-MM-yyyy}", _staffRepository.FindId(p.StaffId).Birthday) : "",
                                 CodeGuide = _staffRepository.FindId(p.StaffId).CodeGuide,
                                 File = _documentFileRepository.GetAllAsQueryable().Where(x => x.PermissionStaff != null && x.PermissionStaff.Contains(p.StaffId.ToString()) && x.IsDelete == false).ToList(),
                                 Image = _staffRepository.FindId(p.StaffId).Image,
                                 StartDate = p.StartDate != null ? string.Format("{0:dd-MM-yyyy}", p.StartDate) : "",
                                 EndDate = p.EndDate != null ? string.Format("{0:dd-MM-yyyy}", p.EndDate) : "",
                                 CreateDate = _staffRepository.FindId(p.StaffId).CreatedDate
                             }).OrderByDescending(p => p.CreateDate).ToList();
                return PartialView("~/Views/TourTabInfo/_HuongDanVien.cshtml", list);
            }
            catch
            {
            }

            return PartialView("~/Views/TourTabInfo/_HuongDanVien.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteGuide(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1115);
                var tourguide = _tourGuideRepository.FindId(id);

                // xóa staff
                //await _staffRepository.DeleteMany(tourguide.StaffId.ToString().Split(',').ToArray(), false);

                // xóa document
                var docs = _documentFileRepository.GetAllAsQueryable().AsEnumerable()
                            .Where(p => p.PermissionStaff != null && p.PermissionStaff.Contains(tourguide.StaffId.ToString())).ToList();
                if (docs.Count() > 0)
                {
                    foreach (var item in docs)
                    {
                        String path = Server.MapPath("~/Upload/file/" + item.FileName);
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                        await _documentFileRepository.DeleteMany(item.Id.ToString().Split(',').ToArray(), false);
                    }
                }

                // xóa lịch hẹn tour
                var schedule = _tourScheduleRepository.GetAllAsQueryable().AsEnumerable()
                    .Where(p => p.StaffId == tourguide.StaffId && p.TourId == tourguide.TourId).ToList();
                if (schedule.Count() > 0)
                {
                    foreach (var item in schedule)
                    {
                        await _tourScheduleRepository.DeleteMany(item.Id.ToString().Split(',').ToArray(), false);
                    }
                }

                //
                var items = _staffRepository.FindId(tourguide.StaffId);
                UpdateHistory.SaveHistory(1115, "Xóa hướng dẫn viên: " + items.FullName,
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
                //

                // xóa tourguide
                await _tourGuideRepository.DeleteMany(id.ToString().Split(',').ToArray(), false);

                var list = _tourGuideRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.TourId == tourguide.TourId && p.IsDelete == false)
                 .Select(p => new GuideListViewModel
                 {
                     StaffId = p.StaffId ?? 0,
                     GuideId = p.Id,
                     Name = p.tbl_Staff.FullName,
                     Birthday = p.tbl_Staff.Birthday != null ? string.Format("{0:dd-MM-yyyy}", p.tbl_Staff.Birthday) : "",
                     CodeGuide = p.tbl_Staff.CodeGuide,
                     File = _documentFileRepository.GetAllAsQueryable().Where(x => x.PermissionStaff != null && x.PermissionStaff.Contains(p.StaffId.ToString()) && x.IsDelete == false).ToList(),
                     Image = p.tbl_Staff.Image,
                     StartDate = p.StartDate != null ? string.Format("{0:dd-MM-yyyy}", p.StartDate) : "",
                     EndDate = p.EndDate != null ? string.Format("{0:dd-MM-yyyy}", p.EndDate) : "",
                     CreateDate = _staffRepository.FindId(p.StaffId).CreatedDate
                 }).OrderByDescending(p => p.CreateDate).ToList();

                return PartialView("~/Views/TourTabInfo/_HuongDanVien.cshtml", list);
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_HuongDanVien.cshtml");
            }
        }
        #endregion
    }
}