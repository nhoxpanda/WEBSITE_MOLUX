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
    public class StaffOtherTabController : BaseController
    {
        // GET: StaffOtherTab

        #region Init

        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_StaffSalary> _staffSalaryRepository;
        private IGenericRepository<tbl_StaffSalaryDetail> _staffSalaryDetailRepository;
        private IGenericRepository<tbl_StaffDayOff> _staffDayOffRepository;
        private IGenericRepository<tbl_StaffBonusDiscipline> _staffBonusDisciplineRepository;
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
        private IGenericRepository<tbl_TourSchedule> _tourScheduleRepository;
        private IGenericRepository<tbl_Task> _taskRepository;
        private IGenericRepository<tbl_TaskStaff> _taskStaffRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;
        private DataContext _db;

        public StaffOtherTabController(
            IGenericRepository<tbl_StaffBonusDiscipline> staffBonusDisciplineRepository,
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_Staff> staffRepository, 
            IGenericRepository<tbl_StaffSalary> staffSalaryRepository,
            IGenericRepository<tbl_StaffSalaryDetail> staffSalaryDetailRepository,
            IGenericRepository<tbl_StaffDayOff> staffDayOffRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IGenericRepository<tbl_CustomerContact> customerContactRepository,
            IGenericRepository<tbl_CustomerVisa> customerVisaRepository,
            IGenericRepository<tbl_TaskStaff> taskStaffRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_CustomerContactVisa> customerContactVisaRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IGenericRepository<tbl_ContactHistory> contactHistoryRepository,
            IGenericRepository<tbl_AppointmentHistory> appointmentHistoryRepository,
            IGenericRepository<tbl_TourSchedule> tourScheduleRepository,
            IGenericRepository<tbl_Task> taskRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._partnerRepository = partnerRepository;
            this._customerRepository = customerRepository;
            this._customerContactRepository = customerContactRepository;
            this._tagsRepository = tagsRepository;
            this._customerVisaRepository = customerVisaRepository;
            this._customerContactVisaRepository = customerContactVisaRepository;
            this._taskStaffRepository = taskStaffRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._documentFileRepository = documentFileRepository;
            this._contactHistoryRepository = contactHistoryRepository;
            this._appointmentHistoryRepository = appointmentHistoryRepository;
            this._updateHistoryRepository = updateHistoryRepository;
            this._staffRepository = staffRepository;
            this._tourScheduleRepository = tourScheduleRepository;
            this._taskRepository = taskRepository;
            this._tourRepository = tourRepository;
            this._staffSalaryRepository = staffSalaryRepository;
            this._staffBonusDisciplineRepository = staffBonusDisciplineRepository;
            this._staffDayOffRepository = staffDayOffRepository;
            this._staffSalaryDetailRepository = staffSalaryDetailRepository;
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
        }
        #endregion

        #region Lịch hẹn
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateAppointment(tbl_AppointmentHistory model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 58);
                string idStaff = Session["idStaff"].ToString();
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.StaffId = clsPermission.GetUser().StaffID;
                model.OtherStaff = form["OtherStaff"] == null ? idStaff : idStaff + "," + form["OtherStaff"];
                model.PartnerId = model.PartnerId == 0 ? null : model.PartnerId;

                if (await _appointmentHistoryRepository.Create(model))
                {
                    UpdateHistory.SaveHistory(58, "Thêm mới lịch hẹn " + model.Title,
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
                            .Where(p => p.StaffId.ToString() == idStaff ||
                            (p.OtherStaff != null && p.OtherStaff.Contains(idStaff) && p.IsDelete == false))
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
                    return PartialView("~/Views/StaffTabInfo/_LichHen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/StaffTabInfo/_LichHen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/StaffTabInfo/_LichHen.cshtml");
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
            var model = await _appointmentHistoryRepository.GetById(id);
            return PartialView("_Partial_EditAppointmentHistory", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateAppointment(tbl_AppointmentHistory model, FormCollection form)
        {
            try
            {
                string idStaff = model.StaffId.ToString();
                Permission(clsPermission.GetUser().PermissionID, 58);
                model.PartnerId = model.PartnerId == 0 ? null : model.PartnerId;
                model.ModifiedDate = DateTime.Now;
                model.OtherStaff = form["OtherStaff"] == null ? idStaff : idStaff + "," + form["OtherStaff"];

                if (await _appointmentHistoryRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(58, "Cập nhật lịch hẹn: " + model.Title,
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
                            .Where(p => p.StaffId.ToString() == model.OtherStaff || 
                            (p.OtherStaff != null && p.OtherStaff.Contains(model.OtherStaff) && p.IsDelete == false))
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
                    return PartialView("~/Views/StaffTabInfo/_LichHen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/StaffTabInfo/_LichHen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/StaffTabInfo/_LichHen.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAppointment(int id)
        {
            int staffId = _appointmentHistoryRepository.FindId(id).StaffId ?? 0;
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 58);
                var listId = id.ToString().Split(',').ToArray();
                if (await _appointmentHistoryRepository.DeleteMany(listId, false))
                {
                    //
                    var item = _appointmentHistoryRepository.FindId(id);
                    UpdateHistory.SaveHistory(58, "Xóa lịch hẹn: " + item.Title,
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
                    var list = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable()
                            .Where(p => p.StaffId == (staffId == 0 ? p.StaffId : staffId) ||
                            (p.OtherStaff != null && p.OtherStaff.Contains(staffId.ToString()) && p.IsDelete == false))
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
                    return PartialView("~/Views/StaffTabInfo/_LichHen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/StaffTabInfo/_LichHen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/StaffTabInfo/_LichHen.cshtml");
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
                Permission(clsPermission.GetUser().PermissionID, 59);
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.StaffId = clsPermission.GetUser().StaffID;
                model.OtherStaffId = Convert.ToInt32(Session["idStaff"].ToString());
                if (await _contactHistoryRepository.Create(model))
                {
                    UpdateHistory.SaveHistory(59, "Thêm mới lịch sử liên hệ " + model.Request,
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

                    var list = _db.tbl_ContactHistory.AsEnumerable().Where(p => p.OtherStaffId == model.OtherStaffId)
                        .Where(p => p.IsDelete == false)
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
                    return PartialView("~/Views/StaffTabInfo/_LichSuLienHe.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/StaffTabInfo/_LichSuLienHe.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/StaffTabInfo/_LichSuLienHe.cshtml");
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
            return PartialView("_Partial_EditContactHistory", await _contactHistoryRepository.GetById(id));
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateContactHistory(tbl_ContactHistory model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 59);
                model.ModifiedDate = DateTime.Now;
                if (await _contactHistoryRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(59, "Cập nhật lịch sử liên hệ: " + model.Request,
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
                        .Where(p => p.OtherStaffId == model.OtherStaffId).Where(p => p.IsDelete == false)
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
                    return PartialView("~/Views/StaffTabInfo/_LichSuLienHe.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/StaffTabInfo/_LichSuLienHe.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/StaffTabInfo/_LichSuLienHe.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteContactHistory(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 59);
                int staffId = _contactHistoryRepository.FindId(id).OtherStaffId ?? 0;
                var listId = id.ToString().Split(',').ToArray();
                if (await _contactHistoryRepository.DeleteMany(listId, false))
                {
                    //
                    var item = _contactHistoryRepository.FindId(id);
                    UpdateHistory.SaveHistory(59, "Xóa lịch sử liên hệ: " + item.Request,
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
                    var list = _db.tbl_ContactHistory.AsEnumerable()
                        .Where(p => p.OtherStaffId == (staffId == 0 ? p.OtherStaffId : staffId) && p.IsDelete == false)
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
                    return PartialView("~/Views/StaffTabInfo/_LichSuLienHe.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/StaffTabInfo/_LichSuLienHe.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/StaffTabInfo/_LichSuLienHe.cshtml");
            }
        }
        #endregion

        #region Lịch sử đi tour

        [HttpPost]
        public async Task<ActionResult> DeleteTourSchedule(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 92);
                int staffId = _tourScheduleRepository.FindId(id).StaffId ?? 0;
                var listId = id.ToString().Split(',').ToArray();
                if (await _tourScheduleRepository.DeleteMany(listId, false))
                {
                    //
                    var item = _tourScheduleRepository.FindId(id);
                    UpdateHistory.SaveHistory(92, "Xóa lịch đi tour: " + item.tbl_Tour.Name,
                        null, //appointment
                        null, //contract
                        null, //customer
                        null, //partner
                        null, //program
                        null, //task
                        item.TourId, //tour
                        null, //quotation
                        null, //document
                        null, //history
                        null // ticket
                        );
                    //
                    var list = _tourScheduleRepository.GetAllAsQueryable().AsEnumerable()
                        .Where(p => p.IsDelete == false).Where(c => c.StaffId == staffId)
                        .OrderByDescending(p => p.CreatedDate).ToList();
                    return PartialView("~/Views/StaffTabInfo/_LichSuDiTour.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/StaffTabInfo/_LichSuDiTour.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/StaffTabInfo/_LichSuDiTour.cshtml");
            }
        }
        #endregion

        #region Nhiệm vụ

        [HttpPost]
        public async Task<ActionResult> EditTask(int id, int idstaff)
        {
            var model = await _taskRepository.GetById(id);
            int depId = _staffRepository.FindId(Convert.ToInt32(model.Permission)).DepartmentId ?? 0;
            ViewBag.IdStaff = idstaff;
            ViewBag.DepartmentId = depId;
            ViewBag.PermissionList = _staffRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.DepartmentId == depId && p.IsVietlike == true);
            if (clsPermission.GetUser().StaffID == 9 || model.TaskStatusId <= 1195)
            {
                return PartialView("_Partial_EditTaskStaff", model);
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
                Permission(clsPermission.GetUser().PermissionID, 57);
                model.ModifiedDate = DateTime.Now;
                model.IsNotify = true;
                if (model.TourId != null)
                {
                    model.CodeTour = _tourRepository.FindId(model.TourId).Code;
                }
                if (await _taskRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(57, "Cập nhật nhiệm vụ: " + model.Name,
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
                    ViewBag.IdStaff = Convert.ToInt32(form["txtIdStaff"]);
                    var list = _taskRepository.GetAllAsQueryable().AsEnumerable()
                        .Where(p => (p.StaffId == Convert.ToInt32(form["txtIdStaff"]) || p.Permission.Contains(form["txtIdStaff"])) && p.IsDelete == false)
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
                else
                {
                    return PartialView("~/Views/StaffTabInfo/_NhiemVu.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/StaffTabInfo/_NhiemVu.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTask(int id, int idstaff)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 57);
                var listId = id.ToString().Split(',').ToArray();
                if (await _taskRepository.DeleteMany(listId, false))
                {
                    ViewBag.IdStaff = idstaff;
                    //
                    var item = _taskRepository.FindId(id);
                    UpdateHistory.SaveHistory(57, "Xóa nhiệm vụ: " + item.Name,
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
                    var list = _taskRepository.GetAllAsQueryable().AsEnumerable()
                        .Where(p => (p.StaffId == idstaff || p.Permission.Contains(idstaff.ToString())) && p.IsDelete == false)
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
                else
                {
                    return PartialView("~/Views/StaffTabInfo/_NhiemVu.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/StaffTabInfo/_NhiemVu.cshtml");
                
            }
        }
        #endregion

        #region Lương

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateSalary(InsertSalaryDetail model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1128);
                var idStaff = int.Parse(Session["idStaff"].ToString()); 
                var _staffSalary = _staffSalaryRepository.GetAllAsQueryable().Where(x => x.StaffId == idStaff).FirstOrDefault();
                var _lastStaffSalary = _staffSalary.tbl_StaffSalaryDetail.LastOrDefault();
                model.staffSalaryDetail.Value = decimal.Parse(model.SoTienTang.Replace(",", ""));

                model.staffSalaryDetail.StaffSalaryId = _staffSalary.Id;
                model.staffSalaryDetail.CreateDate = DateTime.Now;
                if (_lastStaffSalary != null)
                {
                    if (model.staffSalaryDetail.IsIncrease)
                    {
                        model.staffSalaryDetail.SalaryAfterChange = _lastStaffSalary.SalaryAfterChange + model.staffSalaryDetail.Value;
                    }
                    else
                    {
                        model.staffSalaryDetail.SalaryAfterChange = _lastStaffSalary.SalaryAfterChange - model.staffSalaryDetail.Value;
                    }
                        
                }
                else
                {
                    if (model.staffSalaryDetail.IsIncrease)
                    {
                        model.staffSalaryDetail.SalaryAfterChange = _staffSalary.BasicSalary + model.staffSalaryDetail.Value;
                    }
                    else
                    {
                        model.staffSalaryDetail.SalaryAfterChange = _staffSalary.BasicSalary - model.staffSalaryDetail.Value;
                    }
                   
                }
                
                if (await _staffSalaryDetailRepository.Create(model.staffSalaryDetail))
                {
                    _staffSalary.CurSalary = model.staffSalaryDetail.SalaryAfterChange + _staffSalary.SubsidySalary;
                    await _staffSalaryRepository.Update(_staffSalary);

                    List<tbl_StaffSalaryDetail> _listStaffSalaryDetail = new List<tbl_StaffSalaryDetail>();
                    var _staffSalaryLuong = _staffSalaryRepository.GetAllAsQueryable().AsEnumerable()
                        .Where(c => c.StaffId == idStaff).LastOrDefault();
                    if (_staffSalaryLuong != null)
                    {
                        _listStaffSalaryDetail = _staffSalaryDetailRepository.GetAllAsQueryable().AsEnumerable()
                        .Where(c => c.StaffSalaryId == _staffSalaryLuong.Id).ToList();
                    }


                    var staffSalaryViewModel = new StaffSalaryViewModel()
                    {
                        staffSalary = _staffSalaryLuong,
                        listStaffSalaryDetail = _listStaffSalaryDetail
                    };
                    
                    
                    return PartialView("~/Views/StaffTabInfo/_Luong.cshtml", staffSalaryViewModel);
                }
                else
                {
                    return PartialView("~/Views/StaffTabInfo/_Luong.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/StaffTabInfo/_Luong.cshtml");
            }
        }
        
        [HttpPost]
        public async Task<ActionResult> DeleteSalary(int id)
        {
            //int staffId = _staffSalaryRepository.FindId(id).StaffId;
            //try
            //{
            //    Permission(clsPermission.GetUser().PermissionID, 1128);
            //    var listId = id.ToString().Split(',').ToArray();
            //    if (await _staffSalaryRepository.DeleteMany(listId, false))
            //    {
            //        var list = _staffSalaryRepository.GetAllAsQueryable().AsEnumerable()
            //                      .Where(c => c.StaffId == staffId && c.IsDelete == false)
            //                      .OrderByDescending(p => p.CreatedDate).ToList();
            //        return PartialView("~/Views/StaffTabInfo/_Luong.cshtml", list);
            //    }
            //    else
            //    {
            //        return PartialView("~/Views/StaffTabInfo/_Luong.cshtml");
            //    }
            //}
            //catch
            //{
            return PartialView("~/Views/StaffTabInfo/_Luong.cshtml");
            //}
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateStaffSalaryByID(int id)
        {
            var _staffSalary = _staffSalaryRepository.GetAllAsQueryable()
                        .Where(c => c.Id == id).FirstOrDefault();
            return PartialView("_Partial_EditStaffSalary", _staffSalary);//GetAllAsQueryable().Where(x=>x.StaffId==staffID).LastOrDefault());
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateStaffSalary(tbl_StaffSalary model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1128);
                var staffSalaryDetailTemp = _staffSalaryDetailRepository.GetAllAsQueryable().AsEnumerable()
                        .Where(c => c.StaffSalaryId == model.Id).LastOrDefault();
                if (staffSalaryDetailTemp != null)
                {
                    model.CurSalary = staffSalaryDetailTemp.SalaryAfterChange + model.SubsidySalary;
                }
                else
                {
                    model.CurSalary = model.BasicSalary + model.SubsidySalary;
                }
                
                if (await _staffSalaryRepository.Update(model))
                {
                    List<tbl_StaffSalaryDetail> _listStaffSalaryDetail = new List<tbl_StaffSalaryDetail>();
                    var _staffSalaryLuong = _staffSalaryRepository.GetAllAsQueryable().AsEnumerable()
                        .Where(c => c.StaffId == model.StaffId).LastOrDefault();
                    if (_staffSalaryLuong != null)
                    {
                        _listStaffSalaryDetail = _staffSalaryDetailRepository.GetAllAsQueryable().AsEnumerable()
                        .Where(c => c.StaffSalaryId == _staffSalaryLuong.Id).ToList();
                    }


                    var staffSalaryViewModel = new StaffSalaryViewModel()
                    {
                        staffSalary = _staffSalaryLuong,
                        listStaffSalaryDetail = _listStaffSalaryDetail
                    };
                    return PartialView("~/Views/StaffTabInfo/_Luong.cshtml", staffSalaryViewModel);
                    
                }
                else
                {
                    return PartialView("~/Views/StaffTabInfo/_Luong.cshtml");
                }
            }
            catch(Exception ex)
            {
                var mess = ex.Message;
                return PartialView("~/Views/StaffTabInfo/_Luong.cshtml");
            }
        }

        #endregion

        #region Khen thưởng/Kỉ luật

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateBonusDiscipline(tbl_StaffBonusDiscipline model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1129);
                string idStaff = Session["idStaff"].ToString();
                model.CreatedDate = DateTime.Now;
                model.StaffId = Convert.ToInt32(idStaff);

                if (await _staffBonusDisciplineRepository.Create(model))
                {
                    var list = _staffBonusDisciplineRepository.GetAllAsQueryable().AsEnumerable()
                                .Where(c => c.StaffId == model.StaffId && c.IsDelete == false)
                                .OrderByDescending(p => p.CreatedDate).ToList();
                    return PartialView("~/Views/StaffTabInfo/_KhenThuongKyLuat.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/StaffTabInfo/_KhenThuongKyLuat.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/StaffTabInfo/_KhenThuongKyLuat.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteBonusDiscipline(int id)
        {
            int staffId = _staffBonusDisciplineRepository.FindId(id).StaffId;
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1129);
                var listId = id.ToString().Split(',').ToArray();
                if (await _staffBonusDisciplineRepository.DeleteMany(listId, false))
                {
                    var list = _staffBonusDisciplineRepository.GetAllAsQueryable().AsEnumerable()
                                  .Where(c => c.StaffId == staffId && c.IsDelete == false)
                                  .OrderByDescending(p => p.CreatedDate).ToList();
                    return PartialView("~/Views/StaffTabInfo/_KhenThuongKyLuat.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/StaffTabInfo/_KhenThuongKyLuat.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/StaffTabInfo/_KhenThuongKyLuat.cshtml");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EditBonusDiscipline(int id)
        {
            var model = await _staffBonusDisciplineRepository.GetById(id);
            return PartialView("_Partial_EditBonucisDispline", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateBonusDiscipline(tbl_StaffBonusDiscipline model, FormCollection form)
        {
            try
            {
                string idStaff = model.StaffId.ToString();
                Permission(clsPermission.GetUser().PermissionID, 1129);
               
                
                

                if (await _staffBonusDisciplineRepository.Update(model))
                {
                    var list = _staffBonusDisciplineRepository.GetAllAsQueryable().AsEnumerable()
                                .Where(c => c.StaffId == model.StaffId && c.IsDelete == false)
                                .OrderByDescending(p => p.CreatedDate).ToList();
                    return PartialView("~/Views/StaffTabInfo/_KhenThuongKyLuat.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/StaffTabInfo/_KhenThuongKyLuat.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/StaffTabInfo/_KhenThuongKyLuat.cshtml");
            }
        }
        #endregion

        #region Ngày nghỉ phép
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateNgayNghiPhep(tbl_StaffDayOff model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1130);
                string idStaff = Session["idStaff"].ToString();
               
                model.StaffId = Convert.ToInt32(idStaff);

                if (await _staffDayOffRepository.Create(model))
                {
                    var list = _staffDayOffRepository.GetAllAsQueryable().AsEnumerable()
                                .Where(c => c.StaffId == model.StaffId)
                                .OrderByDescending(p => p.StartDay).ToList();
                    return PartialView("~/Views/StaffTabInfo/_NgayNghiPhep.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/StaffTabInfo/_NgayNghiPhep.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/StaffTabInfo/_NgayNghiPhep.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteNgayNghiPhep(int id)
        {
            int staffId = _staffDayOffRepository.FindId(id).StaffId;
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1130);
                var listId = id.ToString().Split(',').ToArray();
                if (await _staffDayOffRepository.DeleteMany(listId, true))
                {
                    var list = _staffDayOffRepository.GetAllAsQueryable().AsEnumerable()
                                  .Where(c => c.StaffId == staffId)
                                  .OrderByDescending(p => p.StartDay).ToList();
                    return PartialView("~/Views/StaffTabInfo/_NgayNghiPhep.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/StaffTabInfo/_NgayNghiPhep.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/StaffTabInfo/_NgayNghiPhep.cshtml");
            }
        }

        #endregion
    }
}