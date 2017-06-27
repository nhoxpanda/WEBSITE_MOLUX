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
using System.Data.Entity;

namespace TOURDEMO.Controllers.Appointment
{
    [Authorize]
    public class AppointmentManageController : BaseController
    {
        // GET: AppointmentManage
        #region Init

        private IGenericRepository<tbl_AppointmentHistory> _appointmentHistoryRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_Program> _programRepository;
        private IGenericRepository<tbl_Task> _taskRepository;
        private IGenericRepository<tbl_Partner> _partnerRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;
        private IGenericRepository<tbl_Form> _formRepository;
        private IGenericRepository<tbl_Module> _moduleRepository;
        private IGenericRepository<tbl_Contract> _contractRepository;
        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;

        private DataContext _db;

        public AppointmentManageController(IGenericRepository<tbl_AppointmentHistory> appointmentHistoryRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Program> programRepository,
            IGenericRepository<tbl_Task> taskRepository,
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IGenericRepository<tbl_Contract> contractRepository,
            IGenericRepository<tbl_Form> formRepository,
            IGenericRepository<tbl_Module> moduleRepository,
        IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._appointmentHistoryRepository = appointmentHistoryRepository;
            this._staffRepository = staffRepository;
            this._customerRepository = customerRepository;
            this._programRepository = programRepository;
            this._taskRepository = taskRepository;
            this._partnerRepository = partnerRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._tourRepository = tourRepository;
            this._contractRepository = contractRepository;
            this._updateHistoryRepository = updateHistoryRepository;
            this._formRepository = formRepository;
            this._moduleRepository = moduleRepository;
            _db = new DataContext();
        }

        #endregion

        #region Index
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
        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 26);
            return View();
        }

        [ChildActionOnly]
        public ActionResult _Partial_AppointmentList()
        {
            Permission(clsPermission.GetUser().PermissionID, 26);

            if (SDBID == 6)
                return PartialView("_Partial_AppointmentList", new List<tbl_AppointmentHistory>());
            var model = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable()
                    .Where(p => ((p.OtherStaff != null && p.OtherStaff.Contains(maNV.ToString()))
                            || (p.StaffId == maNV | maNV == 0)
                            || ((p.TourId != null) && (_tourRepository.FindId(p.TourId).Permission != null && _tourRepository.FindId(p.TourId).Permission.Contains(maNV.ToString())
                            || _tourRepository.FindId(p.TourId).StaffId == maNV || _tourRepository.FindId(p.TourId).CreateStaffId == maNV)))
                            & (_staffRepository.FindId(p.StaffId).DepartmentId == maPB | maPB == 0)
                            & (_staffRepository.FindId(p.StaffId).StaffGroupId == maNKD | maNKD == 0)
                            & (_staffRepository.FindId(p.StaffId).HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false))
                    .OrderByDescending(p => p.CreatedDate)
                    .Select(p => new tbl_AppointmentHistory
                    {
                        Id = p.Id,
                        Title = p.Title,
                        CustomerId = p.CustomerId,
                        Time = p.Time,
                        tbl_Customer = _customerRepository.FindId(p.CustomerId),
                        tbl_Program = _programRepository.FindId(p.ProgramId),
                        tbl_Task = _taskRepository.FindId(p.TaskId),
                        tbl_DictionaryService = _dictionaryRepository.FindId(p.tbl_DictionaryService),
                        tbl_Partner = _partnerRepository.FindId(p.PartnerId),
                        tbl_Tour = _tourRepository.FindId(p.TourId),
                        Note = p.Note,
                        OtherStaff = p.OtherStaff,
                        tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                        tbl_Staff = _staffRepository.FindId(p.StaffId),
                        StaffId = p.StaffId,
                        CreatedDate = p.CreatedDate
                    }).ToList();
            return PartialView("_Partial_AppointmentList", model);
        }
        #endregion

        #region Create
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateAppointment(tbl_AppointmentHistory model, FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 26);
            try
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.StaffId = clsPermission.GetUser().StaffID;
                model.OtherStaff = form["OtherStaff"];
                model.PartnerId = model.PartnerId == 0 ? null : model.PartnerId;

                if (await _appointmentHistoryRepository.Create(model))
                {
                    UpdateHistory.SaveHistory(26, "Thêm mới lịch hẹn: " + model.Title,
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
                        .Where(p => p.IsDelete == false)
                        .OrderByDescending(p => p.CreatedDate)
                        .Select(p => new tbl_AppointmentHistory
                        {
                            Id = p.Id,
                            Title = p.Title,
                            CustomerId = p.CustomerId,
                            Time = p.Time,
                            StatusId = p.StatusId,
                            tbl_Customer = _customerRepository.FindId(p.CustomerId),
                            tbl_Program = _programRepository.FindId(p.ProgramId),
                            tbl_Task = _taskRepository.FindId(p.TaskId),
                            tbl_DictionaryService = _dictionaryRepository.FindId(p.tbl_DictionaryService),
                            tbl_Partner = _partnerRepository.FindId(p.PartnerId),
                            tbl_Tour = _tourRepository.FindId(p.TourId),
                            Note = p.Note,
                            OtherStaff = p.OtherStaff,
                            tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                            tbl_Staff = _staffRepository.FindId(p.StaffId),
                            StaffId = p.StaffId,
                            CreatedDate = p.CreatedDate
                        }).ToList();
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Update
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EditAppointment(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 26);
            if (ViewBag.IsEdit)
            {
                var model = await _appointmentHistoryRepository.GetById(id);
                return PartialView("_Partial_EditAppointmentHistory", model);
            }
            return null;
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateAppointment(tbl_AppointmentHistory model, FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 26);
            try
            {
                model.ModifiedDate = DateTime.Now;
                model.OtherStaff = form["OtherStaff"];
                model.PartnerId = model.PartnerId == 0 ? null : model.PartnerId;
                if (await _appointmentHistoryRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(26, "Cập nhật lịch hẹn: " + model.Title,
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
                        .Where(p => p.IsDelete == false)
                        .OrderByDescending(p => p.CreatedDate)
                        .Select(p => new tbl_AppointmentHistory
                        {
                            Id = p.Id,
                            Time = p.Time,
                            Title = p.Title,
                            CustomerId = p.CustomerId,
                            tbl_Customer = _customerRepository.FindId(p.CustomerId),
                            tbl_Program = _programRepository.FindId(p.ProgramId),
                            tbl_Task = _taskRepository.FindId(p.TaskId),
                            tbl_DictionaryService = _dictionaryRepository.FindId(p.tbl_DictionaryService),
                            tbl_Partner = _partnerRepository.FindId(p.PartnerId),
                            tbl_Tour = _tourRepository.FindId(p.TourId),
                            Note = p.Note,
                            OtherStaff = p.OtherStaff,
                            tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                            tbl_Staff = _staffRepository.FindId(p.StaffId),
                            StaffId = p.StaffId,
                            CreatedDate = p.CreatedDate,
                            StatusId = p.StatusId,
                        }).ToList();
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(FormCollection fc)
        {
            Permission(clsPermission.GetUser().PermissionID, 26);
            try
            {
                if (fc["listItemId"] != null && fc["listItemId"] != "")
                {
                    var listIds = fc["listItemId"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        foreach (var i in listIds)
                        {
                            var appointment = _appointmentHistoryRepository.FindId(Convert.ToInt32(i));
                            UpdateHistory.SaveHistory(26, "Xóa lịch hẹn: " + appointment.Title,
                                appointment.Id, //appointment
                                appointment.ContractId, //contract
                                appointment.CustomerId, //customer
                                appointment.PartnerId, //partner
                                appointment.ProgramId, //program
                                appointment.TaskId, //task
                                appointment.TourId, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                null // ticket
                                );
                        }

                        if (await _appointmentHistoryRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "AppointmentManage") }, JsonRequestBehavior.AllowGet);
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

        #region Filter

        [HttpPost]
        public ActionResult FilterStatusTypeDate(int statusId, int typeId, DateTime? start, DateTime? end)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 26);

                if (SDBID == 6)
                    return PartialView("_Partial_AppointmentList", new List<tbl_AppointmentHistory>());
                var list = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable().Where(p =>
                                (start != null ? p.Time >= start : p.Id != 0) && (end != null ? p.Time <= end : p.Id != 0)
                                && (statusId != -1 ? p.StatusId == statusId : p.Id != 0)
                                && (typeId != -1 ? p.DictionaryId == typeId : p.Id != 0)
                                && (p.StaffId == maNV | maNV == 0 || (p.OtherStaff != null && p.OtherStaff.Contains(maNV.ToString()))
                                || ((p.TourId != null) && (_tourRepository.FindId(p.TourId).Permission != null && _tourRepository.FindId(p.TourId).Permission.Contains(maNV.ToString())
                                || _tourRepository.FindId(p.TourId).StaffId == maNV || _tourRepository.FindId(p.TourId).CreateStaffId == maNV)))
                                & (_staffRepository.FindId(p.StaffId).DepartmentId == maPB | maPB == 0)
                                & (_staffRepository.FindId(p.StaffId).StaffGroupId == maNKD | maNKD == 0)
                                & (_staffRepository.FindId(p.StaffId).HeadquarterId == maCN | maCN == 0)
                                & (p.IsDelete == false))
                               .Select(p => new tbl_AppointmentHistory
                               {
                                   Id = p.Id,
                                   Title = p.Title,
                                   Time = p.Time,
                                   tbl_Customer = _customerRepository.FindId(p.CustomerId),
                                   tbl_Program = _programRepository.FindId(p.ProgramId),
                                   tbl_Task = _taskRepository.FindId(p.TaskId),
                                   tbl_DictionaryService = _dictionaryRepository.FindId(p.tbl_DictionaryService),
                                   tbl_Partner = _partnerRepository.FindId(p.PartnerId),
                                   tbl_Tour = _tourRepository.FindId(p.TourId),
                                   Note = p.Note,
                                   OtherStaff = p.OtherStaff,
                                   tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                                   tbl_Staff = _staffRepository.FindId(p.StaffId),
                                   CreatedDate = p.CreatedDate
                               }).ToList();
                return PartialView("_Partial_AppointmentList", list);
            }
            catch
            {
                return PartialView("_Partial_AppointmentList");
            }
        }
        #endregion

        #region JsonCalendar
        public JsonResult JsonCalendar()
        {
            Permission(clsPermission.GetUser().PermissionID, 26);

            if (SDBID == 6)
                return Json(JsonRequestBehavior.AllowGet);
            var model = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable()
                    .Where(p => (p.StaffId == maNV | maNV == 0)
                    & (_staffRepository.FindId(p.StaffId).DepartmentId == maPB | maPB == 0)
                    & (_staffRepository.FindId(p.StaffId).StaffGroupId == maNKD | maNKD == 0)
                    & (_staffRepository.FindId(p.StaffId).HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false))
               .Select(p => new tbl_AppointmentHistory
               {
                   Id = p.Id,
                   Time = p.Time,
                   Title = p.Title,
                   StatusId = p.StatusId,
                   Note = p.StatusId != null ? _dictionaryRepository.FindId(p.StatusId).Note : ""
               }).ToList();

            var eventList = from e in model
                            select new
                            {
                                id = e.Id,
                                title = e.Title,
                                start = e.Time.ToString("s"),
                                constraint = e.Id,
                                color = e.Note
                            };
            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region AppointmentDetail
        [ChildActionOnly]
        public ActionResult _ChiTiet()
        {
            return PartialView("_ChiTiet", new tbl_AppointmentHistory());
        }

        public ActionResult AppointmentDetail(int id)
        {
            var item = _appointmentHistoryRepository.FindId(id);
            var model = new tbl_AppointmentHistory()
            {
                tbl_Customer = item.CustomerId != null ? _customerRepository.FindId(item.CustomerId) : null,
                tbl_DictionaryService = item.ServiceId != null ? _dictionaryRepository.FindId(item.ServiceId) : null,
                tbl_Program = item.ProgramId != null ? _programRepository.FindId(item.ProgramId) : null,
                tbl_Task = item.TaskId != null ? _taskRepository.FindId(item.TaskId) : null,
                Note = item.Note
            };
            return PartialView("_ChiTiet", model);
        }
        #endregion

        #region Notification
        public ActionResult Notification(string time)
        {
            var notice = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable()
                                .Where(p => p.IsDelete == false && p.IsNotify == true
                                && (p.Time != null && p.Time.ToShortDateString() == DateTime.Now.ToShortDateString()
                                && Convert.ToInt32((p.Time - DateTime.Now).TotalMinutes) <= p.Notify && Convert.ToInt32((p.Time - DateTime.Now).TotalMinutes) >= 0)
                                && (p.StaffId == clsPermission.GetUser().StaffID
                                || (p.OtherStaff != null && p.OtherStaff.Contains(clsPermission.GetUser().StaffID.ToString()))))
                                .Select(p => new tbl_AppointmentHistory()
                                {
                                    Title = p.Title,
                                    CustomerId = p.CustomerId,
                                    TourId = p.TourId,
                                    Time = p.Time,
                                    Note = p.Note,
                                    tbl_Customer = _customerRepository.FindId(p.CustomerId),
                                    tbl_Tour = _tourRepository.FindId(p.TourId),
                                    Id = p.Id
                                }).FirstOrDefault();
            if (notice != null)
            {
                return PartialView("_Partial_Notification", notice);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DetailAppointment(int id)
        {
            var item = _appointmentHistoryRepository.FindId(id);
            var model = new tbl_AppointmentHistory()
            {
                Time = item.Time,
                Title = item.Title,
                StaffId = item.StaffId,
                ContractId = item.ContractId,
                CustomerId = item.CustomerId,
                OtherStaff = item.OtherStaff,
                PartnerId = item.PartnerId,
                ProgramId = item.ProgramId,
                TaskId = item.TaskId,
                TourId = item.TourId,
                Note = item.Note,
                tbl_Contract = _contractRepository.FindId(item.ContractId),
                tbl_Partner = _partnerRepository.FindId(item.PartnerId),
                tbl_Customer = _customerRepository.FindId(item.CustomerId),
                tbl_Program = _programRepository.FindId(item.ProgramId),
                tbl_Staff = _staffRepository.FindId(item.StaffId),
                tbl_Task = _taskRepository.FindId(item.TaskId),
                tbl_Tour = _tourRepository.FindId(item.TourId)
            };
            return PartialView("_Partial_DetailAppointment", model);
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
                .Where(p => p.IsDelete == false && p.AppointmentId == id)
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