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

namespace TOURDEMO.Controllers.Task
{
    [Authorize]
    public class TaskOtherTabController : BaseController
    {
        //
        // GET: /TaskOtherTab/
        #region Init

        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_Partner> _partnerRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;
        private IGenericRepository<tbl_CustomerContact> _customerContactRepository;
        private IGenericRepository<tbl_StaffVisa> _staffVisaRepository;
        private IGenericRepository<tbl_Task> _taskRepository;

        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
        private IGenericRepository<tbl_ContactHistory> _contactHistoryRepository;
        private IGenericRepository<tbl_AppointmentHistory> _appointmentHistoryRepository;
        private IGenericRepository<tbl_TaskStaff> _taskStaffRepository;
        private IGenericRepository<tbl_TaskHandling> _taskHandlingRepository;
        private IGenericRepository<tbl_TaskNote> _taskNoteRepository;
        private DataContext _db;

        public TaskOtherTabController(
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IGenericRepository<tbl_CustomerContact> customerContactRepository,
            IGenericRepository<tbl_StaffVisa> customerVisaRepository,

            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Task> taskRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IGenericRepository<tbl_ContactHistory> contactHistoryRepository,
            IGenericRepository<tbl_AppointmentHistory> appointmentHistoryRepository,
            IGenericRepository<tbl_TaskStaff> taskStaffRepository,
            IGenericRepository<tbl_TaskHandling> taskHandlingRepository,
            IGenericRepository<tbl_TaskNote> taskNoteRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._staffRepository = staffRepository;
            this._partnerRepository = partnerRepository;
            this._customerRepository = customerRepository;
            this._customerContactRepository = customerContactRepository;
            this._tagsRepository = tagsRepository;
            this._staffVisaRepository = customerVisaRepository;
            this._taskRepository = taskRepository;

            this._dictionaryRepository = dictionaryRepository;
            this._documentFileRepository = documentFileRepository;
            this._contactHistoryRepository = contactHistoryRepository;
            this._appointmentHistoryRepository = appointmentHistoryRepository;
            this._updateHistoryRepository = updateHistoryRepository;
            this._taskStaffRepository = taskStaffRepository;
            this._taskHandlingRepository = taskHandlingRepository;
            this._taskNoteRepository = taskNoteRepository;
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
        }
        #endregion

        #region Nhật ký xử lý
        [HttpPost]
        public async Task<ActionResult> DeleteHandling(int id)
        {
            var h = _taskHandlingRepository.FindId(id);
            int tasId = h.TaskId;
            try
            {
                if (h.File != null)
                {
                    String path = Server.MapPath("~/Upload/file/" + h.File);
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }
                var listId = id.ToString().Split(',').ToArray();
                if (await _taskHandlingRepository.DeleteMany(listId, false))
                {
                    var list = _taskHandlingRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TaskId == tasId).Where(p => p.IsDelete == false).Select(p => new tbl_TaskHandling
                    {
                        Id = p.Id,
                        CreateDate = p.CreateDate,
                        Note = p.Note,
                        File = p.File,
                        PercentFinish = p.PercentFinish,
                        tbl_Staff = _staffRepository.FindId(p.StaffId),
                        tbl_Dictionary = _dictionaryRepository.FindId(p.StatusId)
                    }).ToList();
                    return PartialView("~/Views/TaskTabInfo/_NhatKyXuLy.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TaskTabInfo/_NhatKyXuLy.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TaskTabInfo/_NhatKyXuLy.cshtml");
            }
        }
        #endregion

        #region DS nhân viên làm nv
        [HttpPost]
        public async Task<ActionResult> DeleteWork(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1126);
            int tasId = _taskStaffRepository.FindId(id).TaskId;
            try
            {
                var listId = id.ToString().Split(',').ToArray();
                //
                var item = _taskRepository.FindId(tasId);
                UpdateHistory.SaveHistory(1126, "Xóa nhân viên làm nhiệm vụ: " + item.Name,
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
                if (await _taskStaffRepository.DeleteMany(listId, false))
                {
                    var list = _taskStaffRepository.GetAllAsQueryable().Where(p => p.TaskId == tasId).Where(p => p.IsDelete == false).ToList();
                    return PartialView("~/Views/TaskTabInfo/_DSNhanVienDangLamNhiemVu.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TaskTabInfo/_DSNhanVienDangLamNhiemVu.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TaskTabInfo/_DSNhanVienDangLamNhiemVu.cshtml");
            }
        }
        #endregion

        #region Lịch Hẹn
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateAppointment(tbl_AppointmentHistory model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 73);
                model.TaskId = Convert.ToInt32(Session["idTask"].ToString());
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.StaffId = clsPermission.GetUser().StaffID;
                model.OtherStaff = form["OtherStaff"];
                model.PartnerId = model.PartnerId == 0 ? null : model.PartnerId;

                if (await _appointmentHistoryRepository.Create(model))
                {
                    UpdateHistory.SaveHistory(73, "Thêm mới lịch hẹn: " + model.Title,
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
                            .Where(p => p.TaskId == model.TaskId && p.IsDelete == false)
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
                    return PartialView("~/Views/TaskTabInfo/_LichHen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TaskTabInfo/_LichHen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TaskTabInfo/_LichHen.cshtml");
            }
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
                Permission(clsPermission.GetUser().PermissionID, 73);
                model.ModifiedDate = DateTime.Now;
                model.OtherStaff = form["OtherStaff"];
                model.PartnerId = model.PartnerId == 0 ? null : model.PartnerId;

                if (await _appointmentHistoryRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(73, "Cập nhật lịch hẹn: " + model.Title,
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
                            .Where(p => p.TaskId == model.TaskId && p.IsDelete == false)
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
                    return PartialView("~/Views/TaskTabInfo/_LichHen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TaskTabInfo/_LichHen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TaskTabInfo/_LichHen.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAppointment(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 73);
            int tasId = _appointmentHistoryRepository.FindId(id).TaskId ?? 0;
            try
            {
                var listId = id.ToString().Split(',').ToArray();
                //
                var item = _appointmentHistoryRepository.FindId(tasId);
                UpdateHistory.SaveHistory(73, "Xóa lịch hẹn: " + item.Title,
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
                            .Where(p => p.TaskId == tasId && p.IsDelete == false)
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
                    return PartialView("~/Views/TaskTabInfo/_LichHen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TaskTabInfo/_LichHen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TaskTabInfo/_LichHen.cshtml");
            }
        }
        #endregion

        #region Tài liệu mẫu
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase FileName)
        {
            if (FileName != null && FileName.ContentLength > 0)
            {
                Session["TaskDocFile"] = FileName;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateDocument(tbl_DocumentFile model, FormCollection form)
        {
            //try
            //{
            Permission(clsPermission.GetUser().PermissionID, 74);
            string id = Session["idTask"].ToString();
            if (ModelState.IsValid)
            {
                model.Code = GenerateCode.DocumentCode();
                model.TaskId = Convert.ToInt32(id);
                model.CreatedDate = DateTime.Now;
                model.IsRead = false;
                model.ModifiedDate = DateTime.Now;
                if (form["TagsId"] != null && form["TagsId"] != "")
                {
                    model.TagsId = form["TagsId"].ToString();
                }
                model.StaffId = clsPermission.GetUser().StaffID;
                //file
                if (Session["TaskDocFile"] != null)
                {
                    HttpPostedFileBase FileName = Session["TaskDocFile"] as HttpPostedFileBase;
                    string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                    String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    FileName.SaveAs(path);
                    if (newName != null && FileSize != null)
                    {
                        model.FileName = newName;
                        model.FileSize = FileSize;
                    }
                }
                //end file

                if (await _documentFileRepository.Create(model))
                {
                    UpdateHistory.SaveHistory(74, "Thêm mới tài liệu, code: " + model.Code + " - " + model.FileName,
                        null, //appointment
                        model.ContractId, //contract
                        model.CustomerId, //customer
                        model.PartnerId, //partner
                        model.ProgramId, //program
                        model.TaskId, //task
                        model.TourId, //tour
                        null, //quotation
                        model.Id, //document
                        null, //history
                        null // ticket
                        );
                    Session["TaskDocFile"] = null;
                    var list = _documentFileRepository.GetAllAsQueryable().AsEnumerable()
                        .Where(p => p.IsDelete == false && p.TaskId.ToString() == id)
                        .Select(p => new tbl_DocumentFile
                        {
                            Id = p.Id,
                            TagsId = p.TagsId,
                            FileName = p.FileName,
                            Note = p.Note,
                            FileSize = p.FileSize,
                            tbl_Staff = _staffRepository.FindId(p.StaffId),
                            StaffId = p.StaffId,
                            PermissionStaff = p.PermissionStaff,
                            CreatedDate = p.CreatedDate
                        }).ToList();
                    return PartialView("~/Views/TaskTabInfo/_TaiLieuMau.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TaskTabInfo/_TaiLieuMau.cshtml");
                }
            }
            return PartialView("~/Views/TaskTabInfo/_TaiLieuMau.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> EditInfoDocument(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 74);
            var model = await _documentFileRepository.GetById(id);
            ViewBag.DictionaryId = new SelectList(_dictionaryRepository.GetAllAsQueryable().Where(p => p.DictionaryCategoryId == 1 && p.IsDelete == false), "Id", "Name", model.DictionaryId);
            return PartialView("~/Views/TaskOtherTab/_Partial_EditDocument.cshtml", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateDocument(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 74);
                if (ModelState.IsValid)
                {
                    model.IsRead = true;
                    model.ModifiedDate = DateTime.Now;
                    if (form["TagsId"] != null && form["TagsId"] != "")
                    {
                        model.TagsId = form["TagsId"].ToString();
                    }
                    if (Session["TaskDocFile"] != null)
                    {
                        //file
                        HttpPostedFileBase FileName = Session["TaskDocFile"] as HttpPostedFileBase;
                        string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                        String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
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
                        UpdateHistory.SaveHistory(74, "Cập nhật tài liệu của nhiệm vụ: " + model.Code,
                            null, //appointment
                            model.ContractId, //contract
                            model.CustomerId, //customer
                            model.PartnerId, //partner
                            model.ProgramId, //program
                            model.TaskId, //task
                            model.TourId, //tour
                            null, //quotation
                            model.Id, //document
                            null, //history
                            null // ticket
                            );
                        Session["TaskDocFile"] = null;
                        //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.CustomerId == model.CustomerId).ToList();
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.TaskId == model.TaskId).Where(p => p.IsDelete == false)
                             .Select(p => new tbl_DocumentFile
                             {
                                 Id = p.Id,
                                 TagsId = p.TagsId,
                                 FileName = p.FileName,
                                 Note = p.Note,
                                 FileSize = p.FileSize,
                                 tbl_Staff = _staffRepository.FindId(p.StaffId),
                                 StaffId = p.StaffId,
                                 PermissionStaff = p.PermissionStaff,
                                 CreatedDate = p.CreatedDate
                             }).ToList();
                        return PartialView("~/Views/TaskTabInfo/_TaiLieuMau.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/TaskTabInfo/_TaiLieuMau.cshtml");
                    }
                }
            }
            catch
            {
            }

            return PartialView("~/Views/TaskTabInfo/_TaiLieuMau.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteDocument(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 74);
                int tasId = _documentFileRepository.FindId(id).TaskId ?? 0;
                //file
                tbl_DocumentFile documentFile = _documentFileRepository.FindId(id) ?? new tbl_DocumentFile();
                String path = Server.MapPath("~/Upload/file/" + documentFile.FileName);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                //end file
                var listId = id.ToString().Split(',').ToArray();
                //
                var item = _documentFileRepository.FindId(tasId);
                UpdateHistory.SaveHistory(74, "Xóa tài liệu: " + item.Code,
                        null, //appointment
                        item.ContractId, //contract
                        item.CustomerId, //customer
                        item.PartnerId, //partner
                        item.ProgramId, //program
                        item.TaskId, //task
                        item.TourId, //tour
                        null, //quotation
                        item.Id, //document
                        null, //history
                        null // ticket
                        );
                //
                if (await _documentFileRepository.DeleteMany(listId, false))
                {
                    var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.TaskId == tasId).Where(p => p.IsDelete == false)
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
                    return PartialView("~/Views/TaskTabInfo/_TaiLieuMau.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TaskTabInfo/_TaiLieuMau.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TaskTabInfo/_TaiLieuMau.cshtml");
            }
        }
        #endregion

        #region Ghi chú
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateNote(tbl_TaskNote model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 75);
                string id = Session["idTask"].ToString();
                if (ModelState.IsValid)
                {
                    model.TaskId = Convert.ToInt32(id);
                    model.CreatedDate = DateTime.Now;
                    model.StaffId = clsPermission.GetUser().StaffID;

                    if (await _taskNoteRepository.Create(model))
                    {
                        var task = _taskRepository.FindId(model.TaskId);
                        UpdateHistory.SaveHistory(75, "Thêm ghi chú cho nhiệm vụ " + task.Name,
                            null, //appointment
                            null, //contract
                            task.CustomerId, //customer
                            null, //partner
                            null, //program
                            model.TaskId, //task
                            task.TourId, //tour
                            null, //quotation
                            null, //document
                            null, //history
                            null // ticket
                            );
                        var list = _taskNoteRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TaskId.ToString() == id).Where(p => p.IsDelete == false).Select(p => new tbl_TaskNote
                        {
                            Id = p.Id,
                            Note = p.Note,
                            tbl_Staff = _staffRepository.FindId(p.StaffId),
                            CreatedDate = p.CreatedDate
                        }).ToList();
                        return PartialView("~/Views/TaskTabInfo/_GhiChu.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/TaskTabInfo/_GhiChu.cshtml");
                    }
                }
            }
            catch { }
            return PartialView("~/Views/TaskTabInfo/_GhiChu.cshtml");
        }
        [HttpPost]
        public async Task<ActionResult> DeleteNote(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 75);
            int tasId = _taskNoteRepository.FindId(id).TaskId;
            try
            {
                var listId = id.ToString().Split(',').ToArray();
                //
                var item = _taskRepository.FindId(tasId);
                UpdateHistory.SaveHistory(1126, "Xóa ghi chú của nhiệm vụ: " + item.Name,
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
                if (await _taskNoteRepository.DeleteMany(listId, false))
                {
                    var list = _taskNoteRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TaskId == tasId).Where(p => p.IsDelete == false).Select(p => new tbl_TaskNote
                    {
                        Id = p.Id,
                        Note = p.Note,
                        tbl_Staff = _staffRepository.FindId(p.StaffId),
                        CreatedDate = p.CreatedDate
                    }).ToList();
                    return PartialView("~/Views/TaskTabInfo/_GhiChu.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TaskTabInfo/_GhiChu.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TaskTabInfo/_GhiChu.cshtml");
            }
        }

        [HttpPost]
        public ActionResult EditNote(int id)
        {
            var model = _taskNoteRepository.FindId(id);
            return PartialView("_Partial_EditNote", model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateTaskNote(tbl_TaskNote model, FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 75);
            if (await _taskNoteRepository.Update(model))
            {
                var task = _taskRepository.FindId(model.TaskId);
                UpdateHistory.SaveHistory(75, "Cập nhật ghi chú của nhiệm vụ: " + task.Name,
                            null, //appointment
                            null, //contract
                            task.CustomerId, //customer
                            null, //partner
                            null, //program
                            task.Id, //task
                            task.TourId, //tour
                            null, //quotation
                            null, //document
                            null, //history
                            null // ticket
                            );
                var list = _taskNoteRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TaskId == model.TaskId).Where(p => p.IsDelete == false).Select(p => new tbl_TaskNote
                {
                    Id = p.Id,
                    Note = p.Note,
                    tbl_Staff = _staffRepository.FindId(p.StaffId),
                    CreatedDate = p.CreatedDate
                }).ToList();
                return PartialView("~/Views/TaskTabInfo/_GhiChu.cshtml", list);
            }
            else
                return PartialView("~/Views/TaskTabInfo/_GhiChu.cshtml");
        }
        #endregion
    }
}