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

namespace TOURDEMO.Controllers
{
    [Authorize]
    public class TaskManageController : BaseController
    {
        //
        // GET: /TaskManage/

        #region Init

        private IGenericRepository<tbl_Task> _taskRepository;
        private IGenericRepository<tbl_Tags> _tagRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;
        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_TaskStaff> _taskstaffRepository;
        private IGenericRepository<tbl_TaskHandling> _taskHandlingRepository;
        private DataContext _db;

        public TaskManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Task> taskRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_Tags> tagRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_TaskStaff> taskstaffRepository,
            IGenericRepository<tbl_TaskHandling> taskHandlingRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._taskRepository = taskRepository;
            this._documentFileRepository = documentFileRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._tagRepository = tagRepository;
            this._staffRepository = staffRepository;
            this._tourRepository = tourRepository;
            this._customerRepository = customerRepository;
            this._taskstaffRepository = taskstaffRepository;
            this._taskHandlingRepository = taskHandlingRepository;
            _db = new DataContext();
        }

        #endregion

        #region GetIdTask
        [HttpPost]
        public ActionResult GetIdTask(int id)
        {
            Session["idTask"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region List
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

            var listGV = _db.tbl_ActionData.Where(p => p.FormId == 93 && p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAddGV = listGV.Contains(1);

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
            Permission(clsPermission.GetUser().PermissionID, 28);
            return View();
        }

        [ChildActionOnly]
        public ActionResult _Partial_TaskList()
        {
            Permission(clsPermission.GetUser().PermissionID, 28);

            if (SDBID == 6)
                return PartialView("_Partial_TaskList", new List<tbl_Task>());

            var model = _taskRepository.GetAllAsQueryable().AsEnumerable()
                    .Where(p => ((p.StaffId == maNV | maNV == 0) || (p.Permission != null && p.Permission.Contains(maNV.ToString()))
                            || ((p.TourId != null) && (_tourRepository.FindId(p.TourId).Permission != null && _tourRepository.FindId(p.TourId).Permission.Contains(maNV.ToString())
                            || _tourRepository.FindId(p.TourId).StaffId == maNV || _tourRepository.FindId(p.TourId).CreateStaffId == maNV)))
                            & (_staffRepository.FindId(p.StaffId).DepartmentId == maPB | maPB == 0)
                            & (_staffRepository.FindId(p.StaffId).StaffGroupId == maNKD | maNKD == 0)
                            & (_staffRepository.FindId(p.StaffId).HeadquarterId == maCN | maCN == 0)
                            & (p.IsDelete == false))
                    .OrderByDescending(p => p.CreatedDate)
                    .Select(p => new tbl_Task
                        {
                            Id = p.Id,
                            CodeTour = p.CodeTour != null ? p.CodeTour : "",
                            Name = p.Name,
                            tbl_Staff = _staffRepository.FindId(p.StaffId),
                            tbl_DictionaryTaskType = _dictionaryRepository.FindId(p.TaskTypeId),
                            Email = p.Email != null ? p.Email : "",
                            Phone = p.Phone != null ? p.Phone : "",
                            StartDate = p.StartDate,
                            EndDate = p.EndDate,
                            Time = p.Time,
                            TimeType = p.TimeType,
                            TaskStatusId = p.TaskStatusId,
                            tbl_DictionaryTaskStatus = _dictionaryRepository.FindId(p.TaskStatusId)
                        }).ToList();
            return PartialView("_Partial_TaskList", model);
        }
        #endregion

        #region Create
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(tbl_Task model, FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 28);
            try
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.StaffId = clsPermission.GetUser().StaffID;
                model.Permission = clsPermission.GetUser().StaffID.ToString();
                model.Code = new Random().Next(1000, 9999).ToString();
                model.CodeTour = _tourRepository.FindId(model.TourId).Code;
                var cus = _customerRepository.FindId(model.CustomerId);
                model.Email = cus.PersonalEmail != null ? cus.PersonalEmail : cus.CompanyEmail != null ? cus.CompanyEmail : null;
                model.Phone = cus.Phone != null ? cus.Phone : null;
                if (model.EndDate != null && model.StartDate != null)
                {
                    model.Time = Int32.Parse((model.EndDate.Value - model.StartDate.Value).TotalDays.ToString());
                    model.TimeType = "ngày";
                }

                if (await _taskRepository.Create(model))
                {
                    UpdateHistory.SaveHistory(28, "Thêm mới nhiệm vụ " + model.Code + " - " + model.Name,
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
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch { }
            return RedirectToAction("Index");
        }
        #endregion

        #region Update
        [HttpPost]
        public ActionResult TaskInfomation(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 28);
            var model = _db.tbl_Task.Find(id);
            if (clsPermission.GetUser().StaffID == 9 || model.TaskStatusId <= 1195)
            {
                return PartialView("_Partial_EditTask", model);
            }
            else
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tbl_Task model, FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 28);
            try
            {
                model.ModifiedDate = DateTime.Now;
                model.CodeTour = _tourRepository.FindId(model.TourId).Code;
                var cus = _customerRepository.FindId(model.CustomerId);
                model.Email = cus.PersonalEmail != null ? cus.PersonalEmail : cus.CompanyEmail != null ? cus.CompanyEmail : null;
                model.Phone = cus.Phone != null ? cus.Phone : null;
                if (model.StartDate != null && model.EndDate != null)
                {
                    model.Time = Int32.Parse((model.EndDate.Value - model.StartDate.Value).TotalDays.ToString());
                    model.TimeType = "ngày";
                }
                if (await _taskRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(28, "Cập nhật nhiệm vụ: " + model.Name,
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

                    return RedirectToAction("Index");
                }
            }
            catch { }

            return RedirectToAction("Index");
        }
        #endregion

        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(FormCollection fc)
        {
            Permission(clsPermission.GetUser().PermissionID, 28);
            try
            {
                if (fc["listItemId"] != null && fc["listItemId"] != "")
                {
                    var listIds = fc["listItemId"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        //
                        foreach (var i in listIds)
                        {
                            var item = _taskRepository.FindId(Convert.ToInt32(i));
                            UpdateHistory.SaveHistory(28, "Xóa nhiệm vụ: " + item.Name,
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
                        }
                        //
                        if (await _taskRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "TaskManage") }, JsonRequestBehavior.AllowGet);
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

        #region CreateAssign
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateAssign(tbl_TaskStaff model, FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 28);
            try
            {
                int id = Int32.Parse(Session["idTask"].ToString());
                var _count = form["countAssign"];
                for (int i = 1; i <= Convert.ToInt32(form["countAssign"]); i++)
                {
                    var cusI = form["Customer" + i];
                    if (form["Customer" + i] != null && form["Customer" + i].ToString() != "")
                    {
                        var check = _taskstaffRepository.GetAllAsQueryable().AsEnumerable()
                            .FirstOrDefault(p => p.TaskId == id && p.StaffId == Int32.Parse(form["Customer" + i].ToString()));
                        if (check == null)
                        {
                            var _isMain = false;
                            if (form["IsMain" + i] != null)
                            {
                                if (form["IsMain" + i].ToString().ToLower() == "off")
                                {
                                    _isMain = false;
                                }
                                else
                                {
                                    _isMain = true;
                                }
                            }
                            var ts = new tbl_TaskStaff
                                                   {
                                                       TaskId = id,
                                                       IsUse = true,
                                                       CreateStaffId = clsPermission.GetUser().StaffID,
                                                       CreateDate = DateTime.Now,
                                                       StaffId = Int32.Parse(form["Customer" + i].ToString()),
                                                       Role = form["Role" + i].ToString(),
                                                       IsMain= _isMain,
                                                       Note = form["Note" + i].ToString(),
                                                   };
                            if (await _taskstaffRepository.Create(ts))
                            {
                                UpdateHistory.UpdatePermissionTask(id, ts.StaffId);
                            }
                        }
                    }
                }
            }
            catch(Exception ex) { var mes = ex.Message; }
            return RedirectToAction("Index");
        }
        #endregion

        #region Finish
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Finish(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 28);
            try
            {
                if (id != 0)
                {
                    var model = _taskRepository.FindId(id);
                    model.TaskStatusId = 1196;
                    model.PercentFinish = 100;
                    model.FinishDate = DateTime.Now;
                    if (await _taskRepository.Update(model))
                    {
                        UpdateHistory.SaveHistory(28, "Cập nhật nhiệm vụ: " + model.Name,
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

                        return Json(JsonRequestBehavior.AllowGet);
                    }
                    // return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Hoàn thành nhiệm vụ thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "TaskManage") }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(JsonRequestBehavior.AllowGet);
                    //return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Xác nhận hoàn thành nhiệm vụ thất bại !" }, JsonRequestBehavior.AllowGet);
                }


                //return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Vui lòng chọn những mục cần hoàn thành !" }, JsonRequestBehavior.AllowGet);

            }
            catch { }
            return RedirectToAction("Index");
        }
        #endregion

        #region Work
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase FileName)
        {
            if (FileName != null && FileName.ContentLength > 0)
            {
                Session["WorkTaskFile"] = FileName;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> WorkTask(tbl_TaskHandling model, FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 28);
            int id = Int32.Parse(Session["idTask"].ToString());
            if (ModelState.IsValid)
            {
                model.TaskId = id;
                model.CreateDate = DateTime.Now;
                model.StaffId = clsPermission.GetUser().StaffID;
                if (Session["WorkTaskFile"] != null)
                {
                    //file
                    HttpPostedFileBase FileName = Session["WorkTaskFile"] as HttpPostedFileBase;
                    String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    FileName.SaveAs(path);
                    //end file
                    model.File = newName;
                }
                if (await _taskHandlingRepository.Create(model))
                {
                    Session["WorkTaskFile"] = null;
                    var m = _taskRepository.FindId(id);
                    m.ModifiedDate = model.CreateDate;
                    m.TaskStatusId = model.StatusId;
                    m.PercentFinish = model.PercentFinish;
                    if (await _taskRepository.Update(m))
                    {
                        UpdateHistory.SaveHistory(28, "Xử lý nhiệm vụ, code: " + m.Code + " - " + m.Name,
                            null, //appointment
                            null, //contract
                            m.CustomerId, //customer
                            null, //partner
                            null, //program
                            m.Id, //task
                            m.TourId, //tour
                            null, //quotation
                            null, //document
                            null, //history
                            null // ticket
                            );
                    }
                }

            }
            return RedirectToAction("Index");
        }
        #endregion

        #region Filter
        public ActionResult FilterTask(int statusId, int typeId, int priorId, DateTime? start, DateTime? end)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 28);

                if (SDBID == 6)
                    return PartialView("_Partial_TaskList", new List<tbl_Task>());
                var list = _taskRepository.GetAllAsQueryable().AsEnumerable()
                    .Where(p => (start != null ? p.CreatedDate >= start : p.Id != 0)
                        && (end != null ? p.CreatedDate <= end : p.Id != 0)
                        && (statusId != 0 ? p.TaskStatusId == statusId : p.Id != 0)
                        && (typeId != 0 ? p.TaskTypeId == typeId : p.Id != 0)
                        && (priorId != 0 ? p.TaskPriorityId == priorId : p.Id != 0))
                    .Where(p => ((p.StaffId == maNV | maNV == 0) || (p.Permission != null && p.Permission.Contains(maNV.ToString()))
                        || ((p.TourId != null) && (_tourRepository.FindId(p.TourId).Permission != null && _tourRepository.FindId(p.TourId).Permission.Contains(maNV.ToString())
                            || _tourRepository.FindId(p.TourId).StaffId == maNV || _tourRepository.FindId(p.TourId).CreateStaffId == maNV)))
                    & (_staffRepository.FindId(p.StaffId).DepartmentId == maPB | maPB == 0)
                    & (_staffRepository.FindId(p.StaffId).StaffGroupId == maNKD | maNKD == 0)
                    & (_staffRepository.FindId(p.StaffId).HeadquarterId == maCN | maCN == 0)
                    & (p.IsDelete == false))
                    .OrderByDescending(p => p.CreatedDate)
                    .Select(p => new tbl_Task
                        {
                            Id = p.Id,
                            CodeTour = p.CodeTour != null ? p.CodeTour : "",
                            Name = p.Name,
                            tbl_Staff = _staffRepository.FindId(p.StaffId),
                            tbl_DictionaryTaskType = _dictionaryRepository.FindId(p.TaskTypeId),
                            Email = p.Email != null ? p.Email : "",
                            Phone = p.Phone != null ? p.Phone : "",
                            StartDate = p.StartDate,
                            EndDate = p.EndDate,
                            Time = p.Time,
                            TimeType = p.TimeType,
                            tbl_DictionaryTaskStatus = _dictionaryRepository.FindId(p.TaskStatusId)
                        }).ToList();
                return PartialView("_Partial_TaskList", list);
            }
            catch
            {
                return PartialView("_Partial_TaskList");
            }
        }
        #endregion

        #region Notification
        public ActionResult Notification(string time)
        {
            try
            {
                var notice = _taskRepository.GetAllAsQueryable().AsEnumerable()
                            .Where(p => ((p.StaffId == maNV | maNV == 0) || p.Permission.Contains(maNV.ToString()))
                                & p.IsNotify == true && p.NotifyDate != null
                                & Convert.ToInt32((p.NotifyDate.Value - DateTime.Now).TotalSeconds) == 0
                                & (_staffRepository.FindId(p.StaffId).DepartmentId == maPB | maPB == 0)
                                & (_staffRepository.FindId(p.StaffId).StaffGroupId == maNKD | maNKD == 0)
                                & (_staffRepository.FindId(p.StaffId).HeadquarterId == maCN | maCN == 0)
                                & (p.IsDelete == false))
                            .Select(p => new tbl_Task()
                            {
                                Name = p.Name,
                                CustomerId = p.CustomerId,
                                TourId = p.TourId,
                                StartDate = p.StartDate,
                                Note = p.Note,
                                tbl_Staff = _staffRepository.FindId(p.StaffId),
                                tbl_Customer = _customerRepository.FindId(p.CustomerId),
                                tbl_Tour = _tourRepository.FindId(p.TourId),
                                Permission = p.Permission,
                                Id = p.Id
                            }).FirstOrDefault();
                if (notice != null)
                {
                    return PartialView("_Partial_NotificationTask", notice);
                }
            }
            catch { }
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DetailTask(int id)
        {
            var item = _taskRepository.FindId(id);
            var model = new tbl_Task()
            {
                Name = item.Name,
                StaffId = item.StaffId,
                CustomerId = item.CustomerId,
                TourId = item.TourId,
                Note = item.Note,
                StartDate = item.StartDate,
                Permission = item.Permission,
                tbl_Customer = _customerRepository.FindId(item.CustomerId),
                tbl_Staff = _staffRepository.FindId(item.StaffId),
                tbl_Tour = _tourRepository.FindId(item.TourId)
            };
            return PartialView("_Partial_DetailTask", model);
        }
        #endregion
    }
}
