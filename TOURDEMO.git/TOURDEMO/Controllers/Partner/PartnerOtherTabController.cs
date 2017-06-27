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
    public class PartnerOtherTabController : BaseController
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
        private IGenericRepository<tbl_Tour> _tourRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
        private IGenericRepository<tbl_ContactHistory> _contactHistoryRepository;
        private IGenericRepository<tbl_AppointmentHistory> _appointmentHistoryRepository;
        private DataContext _db;

        public PartnerOtherTabController(
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
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._partnerRepository = partnerRepository;
            this._customerRepository = customerRepository;
            this._customerContactRepository = customerContactRepository;
            this._tagsRepository = tagsRepository;
            this._customerVisaRepository = customerVisaRepository;
            this._customerContactVisaRepository = customerContactVisaRepository;
            this._tourRepository = tourRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._documentFileRepository = documentFileRepository;
            this._contactHistoryRepository = contactHistoryRepository;
            this._appointmentHistoryRepository = appointmentHistoryRepository;
            this._updateHistoryRepository = updateHistoryRepository;
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

        #region Lịch hẹn
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateAppointment(tbl_AppointmentHistory model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 63);

                model.PartnerId = Convert.ToInt32(Session["idPartner"].ToString());
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.StaffId = clsPermission.GetUser().StaffID;
                model.OtherStaff = form["OtherStaff"];
                model.PartnerId = model.PartnerId == 0 ? null : model.PartnerId;

                if (await _appointmentHistoryRepository.Create(model))
                {
                    UpdateHistory.SaveHistory(63, "Thêm mới lịch hẹn đối tác: " + model.Title,
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
                                .Where(p => p.PartnerId == model.PartnerId && p.IsDelete == false)
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
                                    OtherStaff = p.OtherStaff,
                                    TourId = p.TourId,
                                    tbl_Tour = _tourRepository.FindId(p.TourId)
                                }).ToList();
                    return PartialView("~/Views/PartnerTabInfo/_LichHen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/PartnerTabInfo/_LichHen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/PartnerTabInfo/_LichHen.cshtml");
            }
        }

        public JsonResult LoadPartner(int id)
        {
            var model = new SelectList(_partnerRepository.GetAllAsQueryable().Where(p => p.DictionaryId == id && p.IsDelete == false).ToList(), "Id", "Name");
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        //[ChildActionOnly]
        //public ActionResult _Partial_EditAppointmentHistory()
        //{
        //    return PartialView("_Partial_EditAppointmentHistory", new tbl_AppointmentHistory());
        //}

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EditAppointment(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 63);
            var model = await _appointmentHistoryRepository.GetById(id);
            return PartialView("_Partial_EditAppointmentHistory", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateAppointment(tbl_AppointmentHistory model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 63);
                model.ModifiedDate = DateTime.Now;
                model.OtherStaff = form["OtherStaff"];
                model.PartnerId = model.PartnerId == 0 ? null : model.PartnerId;

                if (await _appointmentHistoryRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(63, "Cập nhật lịch hẹn: " + model.Title,
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
                            .Where(p => p.PartnerId == model.PartnerId && p.IsDelete == false)
                            .OrderByDescending(p => p.CreatedDate)
                            .Select(p => new tbl_AppointmentHistory
                            {
                                Id = p.Id,
                                StatusId = p.StatusId,
                                Title = p.Title,
                                Time = p.Time,
                                tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                                tbl_Staff = _staffRepository.FindId(p.StaffId),
                                Note = p.Note,
                                OtherStaff = p.OtherStaff,
                                TourId = p.TourId,
                                tbl_Tour = _tourRepository.FindId(p.TourId)
                            }).ToList();
                    return PartialView("~/Views/PartnerTabInfo/_LichHen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/PartnerTabInfo/_LichHen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/PartnerTabInfo/_LichHen.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAppointment(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 63);
            int parId = _appointmentHistoryRepository.FindId(id).PartnerId ?? 0;
            try
            {
                var listId = id.ToString().Split(',').ToArray();
                if (await _appointmentHistoryRepository.DeleteMany(listId, false))
                {
                    //
                    var item = _contactHistoryRepository.FindId(id);
                    UpdateHistory.SaveHistory(63, "Xóa lịch sử liên hệ: " + item.Request,
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
                    var list = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable()
                            .Where(p => p.PartnerId == parId && p.IsDelete == false)
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
                                TourId = p.TourId,
                                tbl_Tour = _tourRepository.FindId(p.TourId)
                            }).ToList();
                    return PartialView("~/Views/PartnerTabInfo/_LichHen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/PartnerTabInfo/_LichHen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/PartnerTabInfo/_LichHen.cshtml");
            }
        }
        #endregion

        #region Lịch sử liên hệ
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateContactHistory(tbl_ContactHistory model, FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 64);
            try
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.PartnerId = Int32.Parse(Session["idPartner"].ToString());
                model.StaffId = clsPermission.GetUser().StaffID;
                model.OtherStaffId = model.StaffId;
                if (await _contactHistoryRepository.Create(model))
                {
                    UpdateHistory.SaveHistory(64, "Thêm mới lịch sử liên hệ đối tác: " + model.Request,
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

                    var list = _db.tbl_ContactHistory.AsEnumerable().Where(p => p.PartnerId == model.PartnerId).Where(p => p.IsDelete == false)
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
                    return PartialView("~/Views/PartnerTabInfo/_LichSuLienHe.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/PartnerTabInfo/_LichSuLienHe.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/PartnerTabInfo/_LichSuLienHe.cshtml");
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
            Permission(clsPermission.GetUser().PermissionID, 64);
            return PartialView("_Partial_EditContactHistory", await _contactHistoryRepository.GetById(id));
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateContactHistory(tbl_ContactHistory model, FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 64);
            try
            {
                model.ModifiedDate = DateTime.Now;
                model.OtherStaffId = model.StaffId;
                if (await _contactHistoryRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(64, "Cập nhật lịch sử liên hệ: " + model.Request,
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
                    var list = _db.tbl_ContactHistory.AsEnumerable().Where(p => p.PartnerId == model.PartnerId).Where(p => p.IsDelete == false)
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
                    return PartialView("~/Views/PartnerTabInfo/_LichSuLienHe.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/PartnerTabInfo/_LichSuLienHe.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/PartnerTabInfo/_LichSuLienHe.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteContactHistory(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 64);
            try
            {
                int parId = _contactHistoryRepository.FindId(id).PartnerId ?? 0;
                var listId = id.ToString().Split(',').ToArray();
                if (await _contactHistoryRepository.DeleteMany(listId, false))
                { 
                    //
                    var item = _contactHistoryRepository.FindId(id);
                    UpdateHistory.SaveHistory(64, "Xóa lịch sử liên hệ: " + item.Request,
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
                    var list = _db.tbl_ContactHistory.AsEnumerable().Where(p => p.PartnerId == parId).Where(p => p.IsDelete == false)
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
                    return PartialView("~/Views/PartnerTabInfo/_LichSuLienHe.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/PartnerTabInfo/_LichSuLienHe.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/PartnerTabInfo/_LichSuLienHe.cshtml");
            }
        }
        #endregion
    }
}