using CRM.Core;
using CRM.Enum;
using CRM.Infrastructure;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TOURDEMO.Models;
using TOURDEMO.Utilities;

namespace TOURDEMO.Controllers.Visa
{
    public class ListVisaManageController : BaseController
    {
        // GET: VisaManage

        #region Init

        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_StaffVisa> _staffVisaRepository;
        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_CustomerVisa> _customerVisaRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;
        private IGenericRepository<tbl_Task> _taskRepository;
        private IGenericRepository<tbl_TaskStaff> _taskStaffRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;
        private IGenericRepository<tbl_TourCustomerVisa> _tourCustomerVisaRepository;
        private IGenericRepository<tbl_VisaProcedureCustomer> _visaProcedureCustomerRepository;
        private IGenericRepository<tbl_VisaProcedure> _visaProcedureRepository;
        private DataContext _db;

        public ListVisaManageController(IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_CustomerVisa> customerVisaRepository,
            IGenericRepository<tbl_TaskStaff> taskStaffRepository,
            IGenericRepository<tbl_StaffVisa> staffVisaRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IGenericRepository<tbl_Task> taskRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IGenericRepository<tbl_TourCustomerVisa> tourCustomerVisaRepository,
            IGenericRepository<tbl_VisaProcedureCustomer> visaProcedureCustomerRepository,
            IGenericRepository<tbl_VisaProcedure> visaProcedureRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._customerRepository = customerRepository;
            this._customerVisaRepository = customerVisaRepository;
            this._staffVisaRepository = staffVisaRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._staffRepository = staffRepository;
            this._documentFileRepository = documentFileRepository;
            this._tagsRepository = tagsRepository;
            this._taskRepository = taskRepository;
            this._tourRepository = tourRepository;
            this._taskStaffRepository = taskStaffRepository;
            this._tourCustomerVisaRepository = tourCustomerVisaRepository;
            this._visaProcedureCustomerRepository = visaProcedureCustomerRepository;
            this._visaProcedureRepository = visaProcedureRepository;
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
            ViewBag.IsImport = list.Contains(4);
            ViewBag.IsExport = list.Contains(5);

            var ltAccess = _db.tbl_AccessData.Where(p => p.PermissionId == PermissionsId && p.FormId == formId).Select(p => p.ShowDataById).FirstOrDefault();
            if (ltAccess != 0)
                this.SDBID = ltAccess;

            switch (SDBID)
            {
                case 2:
                    maPB = clsPermission.GetUser().DepartmentID;
                    maCN = clsPermission.GetUser().BranchID;
                    break;
                case 3:
                    maNKD = clsPermission.GetUser().GroupID;
                    maCN = clsPermission.GetUser().BranchID; break;
                case 4: maNV = clsPermission.GetUser().StaffID; break;
                case 5: maCN = clsPermission.GetUser().BranchID; break;
            }
        }
        #endregion

        #region List
        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 1101);
            return View();
        }

        [ChildActionOnly]
        public ActionResult _Partial_ListVisa()
        {
            Session["ListVisa"] = null;
            Permission(clsPermission.GetUser().PermissionID, 1101);
            var model = new List<VisaListViewModel>();
            var staff = _staffVisaRepository.GetAllAsQueryable().Where(p => p.IsDelete == false).ToList();
            foreach (var s in staff)
            {
                model.Add(new VisaListViewModel
                {
                    Code = s.VisaNumber,
                    Country = s.TagsId != null ? s.tbl_Tags.Tag : "",
                    EndDate = s.ExpiredDateVisa != null ? string.Format("{0:dd-MM-yyyy}", s.ExpiredDateVisa) : "",
                    Id = s.Id,
                    Name = s.tbl_Staff.FullName,
                    StaffCustomer = true,
                    StartDate = s.CreatedDateVisa != null ? string.Format("{0:dd-MM-yyyy}", s.CreatedDateVisa) : "",
                    Status = s.DictionaryId != null ? s.tbl_Dictionary.Name : "",
                    Type = s.VisaType != null ? s.tbl_DictionaryType.Name : "",
                    CreateDate = s.CreatedDate,
                    IsCustomer = 0
                });
            }
            var customer = _customerVisaRepository.GetAllAsQueryable()
                .Where(p => p.IsDelete == false && p.tbl_Customer.IsTemp == false).ToList();
            foreach (var c in customer)
            {
                model.Add(new VisaListViewModel
                {
                    Code = c.VisaNumber,
                    Country = c.TagsId != null ? c.tbl_Tags.Tag : "",
                    EndDate = c.ExpiredDateVisa != null ? string.Format("{0:dd-MM-yyyy}", c.ExpiredDateVisa) : "",
                    Id = c.Id,
                    Name = c.tbl_Customer.FullName,
                    StaffCustomer = false,
                    StartDate = c.CreatedDateVisa != null ? string.Format("{0:dd-MM-yyyy}", c.CreatedDateVisa) : "",
                    Status = c.DictionaryId != null ? c.tbl_Dictionary.Name : "",
                    Type = c.VisaTypeId != null ? c.tbl_DictionaryType.Name : "",
                    CreateDate = c.CreatedDate,
                    IsCustomer = 1,
                    RefNumber = c.RefNumber,
                    ReceiptDate = c.ReceiptDate != null ? string.Format("{0:dd-MM-yyyy}", c.ReceiptDate) : "",

                    DeadlineCollect = c.DeadlineCollect,
                    DeadlineSubmission = c.DeadlineSubmission
                });
            }
            Session["ListVisa"] = model.OrderByDescending(p => p.CreateDate).ToList();
            return PartialView("_Partial_ListVisa", model.OrderByDescending(p => p.CreateDate).ToList());
        }

        [HttpPost]
        public ActionResult FilterCustomerStaff(int id, int idtag, int type, int status, DateTime? startDate, DateTime? endDate, int personal)
        {
            Session["ListVisa"] = null;
            Permission(clsPermission.GetUser().PermissionID, 1101);
            var model = new List<VisaListViewModel>();
            switch (id)
            {
                case 1: // nhân viên
                    var staff = _staffVisaRepository.GetAllAsQueryable().Where(p => p.IsDelete == false
                        && p.TagsId == (idtag == 0 ? p.TagsId : idtag)
                        && p.VisaType == (type == 0 ? p.VisaType : type)
                        && p.DictionaryId == (status == 0 ? p.DictionaryId : status)
                        && (startDate != null ? p.CreatedDateVisa >= startDate : p.Id != 0) && (endDate != null ? p.CreatedDateVisa <= endDate : p.Id != 0)
                        && (startDate != null ? p.ExpiredDateVisa >= startDate : p.Id != 0) && (endDate != null ? p.ExpiredDateVisa <= endDate : p.Id != 0))
                        .ToList();
                    foreach (var s in staff)
                    {
                        model.Add(new VisaListViewModel
                        {
                            Code = s.VisaNumber,
                            Country = s.TagsId != null ? s.tbl_Tags.Tag : "",
                            EndDate = s.ExpiredDateVisa != null ? string.Format("{0:dd-MM-yyyy}", s.ExpiredDateVisa) : "",
                            Id = s.Id,
                            Name = s.tbl_Staff.FullName,
                            StaffCustomer = true,
                            StartDate = s.CreatedDateVisa != null ? string.Format("{0:dd-MM-yyyy}", s.CreatedDateVisa) : "",
                            Status = s.DictionaryId != null ? s.tbl_Dictionary.Name : "",
                            Type = s.VisaType != null ? s.tbl_DictionaryType.Name : "",
                            CreateDate = s.CreatedDate,
                            IsCustomer = 0
                        });
                    }
                    break;
                case 2: // khách hàng
                    var customer = _customerVisaRepository.GetAllAsQueryable().Where(p => p.IsDelete == false
                        && p.tbl_Customer.IsTemp == false
                        && p.TagsId == (idtag == 0 ? p.TagsId : idtag)
                        && p.VisaTypeId == (type == 0 ? p.VisaTypeId : type)
                        && p.DictionaryId == (status == 0 ? p.DictionaryId : status)
                        && (personal == 0 ? p.Id != 0 : (personal == 1 ? p.IsPersonal == false : p.IsPersonal == true))
                        && (startDate != null ? p.CreatedDateVisa >= startDate : p.Id != 0) && (endDate != null ? p.CreatedDateVisa <= endDate : p.Id != 0)
                        && (startDate != null ? p.ExpiredDateVisa >= startDate : p.Id != 0) && (endDate != null ? p.ExpiredDateVisa <= endDate : p.Id != 0))
                        .ToList();
                    foreach (var c in customer)
                    {
                        model.Add(new VisaListViewModel
                        {
                            Code = c.VisaNumber,
                            Country = c.TagsId != null ? c.tbl_Tags.Tag : "",
                            EndDate = c.ExpiredDateVisa != null ? string.Format("{0:dd-MM-yyyy}", c.ExpiredDateVisa) : "",
                            Id = c.Id,
                            Name = c.tbl_Customer.FullName,
                            StaffCustomer = false,
                            StartDate = c.CreatedDateVisa != null ? string.Format("{0:dd-MM-yyyy}", c.CreatedDateVisa) : "",
                            Status = c.DictionaryId != null ? c.tbl_Dictionary.Name : "",
                            Type = c.VisaTypeId != null ? c.tbl_DictionaryType.Name : "",
                            CreateDate = c.CreatedDate,
                            IsCustomer = 1
                        });
                    }
                    break;
                default: // tất cả
                    var staff1 = _staffVisaRepository.GetAllAsQueryable().Where(p => p.IsDelete == false
                        && p.TagsId == (idtag == 0 ? p.TagsId : idtag)
                        && p.VisaType == (type == 0 ? p.VisaType : type)
                        && p.DictionaryId == (status == 0 ? p.DictionaryId : status)
                        && (startDate != null ? p.CreatedDateVisa >= startDate : p.Id != 0) && (endDate != null ? p.CreatedDateVisa <= endDate : p.Id != 0)
                        && (startDate != null ? p.ExpiredDateVisa >= startDate : p.Id != 0) && (endDate != null ? p.ExpiredDateVisa <= endDate : p.Id != 0))
                        .ToList();
                    foreach (var s in staff1)
                    {
                        model.Add(new VisaListViewModel
                        {
                            Code = s.VisaNumber,
                            Country = s.TagsId != null ? s.tbl_Tags.Tag : "",
                            EndDate = s.ExpiredDateVisa != null ? string.Format("{0:dd-MM-yyyy}", s.ExpiredDateVisa) : "",
                            Id = s.Id,
                            Name = s.tbl_Staff.FullName,
                            StaffCustomer = true,
                            StartDate = s.CreatedDateVisa != null ? string.Format("{0:dd-MM-yyyy}", s.CreatedDateVisa) : "",
                            Status = s.DictionaryId != null ? s.tbl_Dictionary.Name : "",
                            Type = s.VisaType != null ? s.tbl_DictionaryType.Name : "",
                            CreateDate = s.CreatedDate,
                            IsCustomer = 0
                        });
                    }
                    //
                    var customer1 = _customerVisaRepository.GetAllAsQueryable().Where(p => p.IsDelete == false
                         && p.tbl_Customer.IsTemp == false
                        && p.TagsId == (idtag == 0 ? p.TagsId : idtag)
                        && p.VisaTypeId == (type == 0 ? p.VisaTypeId : type)
                        && p.DictionaryId == (status == 0 ? p.DictionaryId : status)
                        && (personal == 0 ? p.Id != 0 : (personal == 1 ? p.IsPersonal == false : p.IsPersonal == true))
                        && (startDate != null ? p.CreatedDateVisa >= startDate : p.Id != 0) && (endDate != null ? p.CreatedDateVisa <= endDate : p.Id != 0)
                        && (startDate != null ? p.ExpiredDateVisa >= startDate : p.Id != 0) && (endDate != null ? p.ExpiredDateVisa <= endDate : p.Id != 0))
                        .ToList();
                    foreach (var c in customer1)
                    {
                        model.Add(new VisaListViewModel
                        {
                            Code = c.VisaNumber,
                            Country = c.TagsId != null ? c.tbl_Tags.Tag : "",
                            EndDate = c.ExpiredDateVisa != null ? string.Format("{0:dd-MM-yyyy}", c.ExpiredDateVisa) : "",
                            Id = c.Id,
                            Name = c.tbl_Customer.FullName,
                            StaffCustomer = false,
                            StartDate = c.CreatedDateVisa != null ? string.Format("{0:dd-MM-yyyy}", c.CreatedDateVisa) : "",
                            Status = c.DictionaryId != null ? c.tbl_Dictionary.Name : "",
                            Type = c.VisaTypeId != null ? c.tbl_DictionaryType.Name : "",
                            CreateDate = c.CreatedDate,
                            IsCustomer = 1
                        });
                    }
                    break;
            }
            Session["ListVisa"] = model.OrderByDescending(p => p.CreateDate).ToList();
            return PartialView("_Partial_ListVisa", model.OrderByDescending(p => p.CreateDate).ToList());
        }
        #endregion

        #region Detail visa procedure

        public ActionResult VisaProcedureList(int id)
        {
            var model = _visaProcedureCustomerRepository.GetAllAsQueryable().Where(p => p.VisaId == id).ToList();
            ViewBag.AllProcedure = _visaProcedureRepository.GetAllAsQueryable().Where(p => p.IsDelete == false).ToList();
            ViewBag.VisaId = id;
            return PartialView("_Partial_VisaProcedure", model);
        }

        public ActionResult UpdateVisaProcedure(int id, int check, int visaId)
        {
            if (check == 1)
            {
                // thêm mới
                var item = new tbl_VisaProcedureCustomer
                {
                    StaffId = clsPermission.GetUser().StaffID,
                    VisaId = visaId,
                    VisaProcedureId = id
                };
                _db.tbl_VisaProcedureCustomer.Add(item);
            }
            else
            {
                // xóa
                var item = _db.tbl_VisaProcedureCustomer.FirstOrDefault(p => p.VisaId == visaId && p.VisaProcedureId == id);
                if (item != null)
                {
                    _db.tbl_VisaProcedureCustomer.Remove(item);
                }
            }
            _db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Export
        /// <summary>
        /// Export file excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportFile(FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 1101);
            var model = new List<VisaListViewModel>();
            byte[] bytes;
            try
            {
                switch (form["id"])
                {
                    case "1": // nhân viên
                        var staff = _staffVisaRepository.GetAllAsQueryable().AsEnumerable()
                            .Where(p => p.IsDelete == false
                            && p.TagsId == (form["idtag"] == "0" ? p.TagsId : Convert.ToInt32(form["idtag"]))
                            && p.VisaType == (form["type"] == "0" ? p.VisaType : Convert.ToInt32(form["type"]))
                            && p.DictionaryId == (form["status"] == "0" ? p.DictionaryId : Convert.ToInt32(form["status"]))
                            && (form["startDate"] != "" ? p.CreatedDateVisa >= Convert.ToDateTime(form["startDate"]) : p.Id != 0)
                            && (form["endDate"] != "" ? p.CreatedDateVisa <= Convert.ToDateTime(form["endDate"]) : p.Id != 0)
                            && (form["startDate"] != "" ? p.ExpiredDateVisa >= Convert.ToDateTime(form["startDate"]) : p.Id != 0)
                            && (form["endDate"] != "" ? p.ExpiredDateVisa <= Convert.ToDateTime(form["endDate"]) : p.Id != 0))
                            .OrderByDescending(p => p.CreatedDate).ToList();
                        foreach (var s in staff)
                        {
                            model.Add(new VisaListViewModel
                            {
                                Code = s.VisaNumber,
                                Country = s.TagsId != null ? s.tbl_Tags.Tag : "",
                                EndDate = s.ExpiredDateVisa != null ? string.Format("{0:dd-MM-yyyy}", s.ExpiredDateVisa) : "",
                                Id = s.Id,
                                Name = s.tbl_Staff.FullName,
                                StaffCustomer = true,
                                StartDate = s.CreatedDate != null ? string.Format("{0:dd-MM-yyyy}", s.CreatedDate) : "",
                                Status = s.DictionaryId != null ? s.tbl_Dictionary.Name : "",
                                Type = s.VisaType != null ? s.tbl_DictionaryType.Name : "",
                            });
                        }
                        using (var stream = new MemoryStream())
                        {
                            ExportVisaToXlsx(stream, model);
                            bytes = stream.ToArray();
                        }
                        return File(bytes, "text/xls", "Danh sách visa của nhân viên.xlsx");
                    case "2": // khách hàng
                        var customer = _customerVisaRepository.GetAllAsQueryable().AsEnumerable()
                            .Where(p => p.IsDelete == false && p.tbl_Customer.IsTemp == false
                            && p.TagsId == (form["idtag"] == "0" ? p.TagsId : Convert.ToInt32(form["idtag"]))
                            && p.VisaTypeId == (form["type"] == "0" ? p.VisaTypeId : Convert.ToInt32(form["type"]))
                            && p.DictionaryId == (form["status"] == "0" ? p.DictionaryId : Convert.ToInt32(form["status"]))
                            && (form["personal"] == "0" ? p.Id != 0 : (form["personal"] == "0" ? p.IsPersonal == false : p.IsPersonal == true))
                            && (form["startDate"] != "" ? p.CreatedDateVisa >= Convert.ToDateTime(form["startDate"]) : p.Id != 0)
                            && (form["endDate"] != "" ? p.CreatedDateVisa <= Convert.ToDateTime(form["endDate"]) : p.Id != 0)
                            && (form["startDate"] != "" ? p.ExpiredDateVisa >= Convert.ToDateTime(form["startDate"]) : p.Id != 0)
                            && (form["endDate"] != "" ? p.ExpiredDateVisa <= Convert.ToDateTime(form["endDate"]) : p.Id != 0))
                            .OrderByDescending(p => p.CreatedDate).ToList();
                        foreach (var c in customer)
                        {
                            model.Add(new VisaListViewModel
                            {
                                Code = c.VisaNumber,
                                Country = c.TagsId != null ? c.tbl_Tags.Tag : "",
                                EndDate = c.ExpiredDateVisa != null ? string.Format("{0:dd-MM-yyyy}", c.ExpiredDateVisa) : "",
                                Id = c.Id,
                                Name = c.tbl_Customer.FullName,
                                StaffCustomer = false,
                                StartDate = c.CreatedDate != null ? string.Format("{0:dd-MM-yyyy}", c.CreatedDate) : "",
                                Status = c.DictionaryId != null ? c.tbl_Dictionary.Name : "",
                                Type = c.VisaTypeId != null ? c.tbl_DictionaryType.Name : ""
                            });
                        }
                        using (var stream = new MemoryStream())
                        {
                            ExportVisaToXlsx(stream, model);
                            bytes = stream.ToArray();
                        }
                        return File(bytes, "text/xls", "Danh sách visa của khách hàng.xlsx");
                    default: // tất cả
                        var staff1 = _staffVisaRepository.GetAllAsQueryable().AsEnumerable()
                            .Where(p => p.IsDelete == false
                            && p.TagsId == (form["idtag"] == "0" ? p.TagsId : Convert.ToInt32(form["idtag"]))
                            && p.VisaType == (form["type"] == "0" ? p.VisaType : Convert.ToInt32(form["type"]))
                            && p.DictionaryId == (form["status"] == "0" ? p.DictionaryId : Convert.ToInt32(form["status"]))
                            && (form["startDate"] != "" ? p.CreatedDateVisa >= Convert.ToDateTime(form["startDate"]) : p.Id != 0)
                            && (form["endDate"] != "" ? p.CreatedDateVisa <= Convert.ToDateTime(form["endDate"]) : p.Id != 0)
                            && (form["startDate"] != "" ? p.ExpiredDateVisa >= Convert.ToDateTime(form["startDate"]) : p.Id != 0)
                            && (form["endDate"] != "" ? p.ExpiredDateVisa <= Convert.ToDateTime(form["endDate"]) : p.Id != 0))
                            .OrderByDescending(p => p.CreatedDate).ToList();
                        foreach (var s in staff1)
                        {
                            model.Add(new VisaListViewModel
                            {
                                Code = s.VisaNumber,
                                Country = s.TagsId != null ? s.tbl_Tags.Tag : "",
                                EndDate = s.ExpiredDateVisa != null ? string.Format("{0:dd-MM-yyyy}", s.ExpiredDateVisa) : "",
                                Id = s.Id,
                                Name = s.tbl_Staff.FullName,
                                StaffCustomer = true,
                                StartDate = s.CreatedDate != null ? string.Format("{0:dd-MM-yyyy}", s.CreatedDate) : "",
                                Status = s.DictionaryId != null ? s.tbl_Dictionary.Name : "",
                                Type = s.VisaType != null ? s.tbl_DictionaryType.Name : ""
                            });
                        }
                        //
                        var customer1 = _customerVisaRepository.GetAllAsQueryable().AsEnumerable()
                            .Where(p => p.IsDelete == false && p.tbl_Customer.IsTemp == false
                            && p.TagsId == (form["idtag"] == "0" ? p.TagsId : Convert.ToInt32(form["idtag"]))
                            && p.VisaTypeId == (form["type"] == "0" ? p.VisaTypeId : Convert.ToInt32(form["type"]))
                            && p.DictionaryId == (form["status"] == "0" ? p.DictionaryId : Convert.ToInt32(form["status"]))
                            && (form["personal"] == "0" ? p.Id != 0 : (form["personal"] == "0" ? p.IsPersonal == false : p.IsPersonal == true))
                            && (form["startDate"] != "" ? p.CreatedDateVisa >= Convert.ToDateTime(form["startDate"]) : p.Id != 0)
                            && (form["endDate"] != "" ? p.CreatedDateVisa <= Convert.ToDateTime(form["endDate"]) : p.Id != 0)
                            && (form["startDate"] != "" ? p.ExpiredDateVisa >= Convert.ToDateTime(form["startDate"]) : p.Id != 0)
                            && (form["endDate"] != "" ? p.ExpiredDateVisa <= Convert.ToDateTime(form["endDate"]) : p.Id != 0))
                            .OrderByDescending(p => p.CreatedDate).ToList();
                        foreach (var c in customer1)
                        {
                            model.Add(new VisaListViewModel
                            {
                                Code = c.VisaNumber,
                                Country = c.TagsId != null ? c.tbl_Tags.Tag : "",
                                EndDate = c.ExpiredDateVisa != null ? string.Format("{0:dd-MM-yyyy}", c.ExpiredDateVisa) : "",
                                Id = c.Id,
                                Name = c.tbl_Customer.FullName,
                                StaffCustomer = false,
                                StartDate = c.CreatedDate != null ? string.Format("{0:dd-MM-yyyy}", c.CreatedDate) : "",
                                Status = c.DictionaryId != null ? c.tbl_Dictionary.Name : "",
                                Type = c.VisaTypeId != null ? c.tbl_DictionaryType.Name : ""
                            });
                        }
                        using (var stream = new MemoryStream())
                        {
                            ExportVisaToXlsx(stream, model);
                            bytes = stream.ToArray();
                        }
                        return File(bytes, "text/xls", "Danh sách visa của nhân viên và khách hàng.xlsx");
                }
            }
            catch (Exception)
            {

            }
            return RedirectToAction("Index");
        }

        public virtual void ExportVisaToXlsx(Stream stream, IList<VisaListViewModel> visa)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.Add("DS Visa");
                var properties = new[]
                    {
                        "STT",
                        "HỌ TÊN",
                        "VISA",
                        "QUỐC GIA",
                        "LOẠI VISA",
                        "TÌNH TRẠNG",
                        "NGÀY HIỆU LỰC",
                        "NGÀY HẾT HẠN"
                    };

                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[3, i + 1].Value = properties[i];
                }

                worksheet.Cells["a1:h2"].Value = "DANH SÁCH VISA";
                worksheet.Cells["a1:h2"].Style.Font.SetFromFont(new Font("Tahoma", 15));
                worksheet.Cells["a1:h2"].Style.Font.Bold = true;
                worksheet.Cells["a1:h2"].Merge = true;
                worksheet.Cells["a1:h2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["a1:h2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                int row = 3;
                int stt = 1;
                foreach (var v in visa)
                {
                    row++;
                    int col = 1;

                    worksheet.Cells[row, col].Value = stt;
                    col++;

                    worksheet.Cells[row, col].Value = v.Name;
                    col++;

                    worksheet.Cells[row, col].Value = v.Code;
                    col++;

                    worksheet.Cells[row, col].Value = v.Country;
                    col++;

                    worksheet.Cells[row, col].Value = v.Type;
                    col++;

                    worksheet.Cells[row, col].Value = v.Status;
                    col++;

                    worksheet.Cells[row, col].Value = v.StartDate;
                    col++;

                    worksheet.Cells[row, col].Value = v.EndDate;
                    col++;

                    stt++;
                }

                worksheet.Cells["a3:h" + row].Style.Font.SetFromFont(new Font("Tahoma", 8));
                worksheet.Cells["a3:h3"].Style.Font.Bold = true;
                worksheet.Cells["a3:h3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                worksheet.Cells["a3:a" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["a3:h3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["a3:h3"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 217, 217));

                worksheet.Cells["a3:h" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a3:h" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a3:h" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a3:h" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a3:h" + row].AutoFitColumns();

                worksheet.Row(1).Height = 18;

                xlPackage.Save();
            }
        }
        #endregion

        #region Create

        /// <summary>
        /// thêm visa nhân viên
        /// </summary>
        /// <param name="model"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        public async Task<ActionResult> CreateVisaStaff(tbl_StaffVisa model, FormCollection form)
        {
            var checkcode = _staffVisaRepository.GetAllAsQueryable().FirstOrDefault(p => p.VisaNumber == model.VisaNumber && p.IsDelete == false);
            if (checkcode == null)
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.IsDelete = false;
                await _staffVisaRepository.Create(model);
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// thêm visa khách hàng
        /// </summary>
        /// <param name="model"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        public async Task<ActionResult> CreateVisaCustomer(tbl_CustomerVisa model, FormCollection form)
        {
            var checkcode = _customerVisaRepository.GetAllAsQueryable().FirstOrDefault(p => p.VisaNumber == model.VisaNumber && p.IsDelete == false);
            if (checkcode == null)
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.IsDelete = false;
                await _customerVisaRepository.Create(model);
                // lưu customer
                if (form["TourId"] != "")
                {
                    var item = new tbl_TourCustomerVisa()
                    {
                        CustomerId = model.Id,
                        IsDelete = false,
                        TourId = Convert.ToInt32(form["TourId"])
                    };
                    await _tourCustomerVisaRepository.Create(item);
                }
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region Update
        public ActionResult Edit(int id, int iscus)
        {
            if (iscus == 0) // nhan vien
            {
                var model = _staffVisaRepository.FindId(id);
                return PartialView("_Partial_EditVisaStaff", model);
            }
            else // khach hang
            {
                var model = _customerVisaRepository.FindId(id);
                return PartialView("_Partial_EditVisaCustomer", model);
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateVisaCustomer(tbl_CustomerVisa model, FormCollection form)
        {
            model.ModifiedDate = DateTime.Now;
            if (model.CreatedDateVisa != null)
            {
                model.CreatedDateVisa = Convert.ToDateTime(form["CreatedDateVisa"]);
            }
            if (model.ExpiredDateVisa != null)
            {
                model.ExpiredDateVisa = Convert.ToDateTime(form["ExpiredDateVisa"]);
            }
            if (model.ExpiredDateVisa != null && model.CreatedDateVisa != null)
            {
                int age = model.ExpiredDateVisa.Value.Year - model.CreatedDateVisa.Value.Year;
                if (model.CreatedDateVisa > model.ExpiredDateVisa.Value.AddYears(-age)) age--;
                model.Deadline = age;
            }
            if (await _customerVisaRepository.Update(model))
            {
                UpdateHistory.SaveHistory(1101, "Cập nhật visa cho khách hàng (code: " + _customerRepository.FindId(model.CustomerId).Code + ")",
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
            }

            //if (Session["ListVisa"] != null)
            //{

            //    var items = Session["ListVisa"] as List<VisaListViewModel>;
            //    return PartialView("_Partial_ListVisa", items);
            //}
            //else
            //{
            return RedirectToAction("Index");
            // }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateVisaStaff(tbl_StaffVisa model, FormCollection form)
        {
            model.ModifiedDate = DateTime.Now;
            if (model.CreatedDateVisa != null)
            {
                model.CreatedDateVisa = Convert.ToDateTime(form["CreatedDateVisa"]);
            }
            if (model.ExpiredDateVisa != null)
            {
                model.ExpiredDateVisa = Convert.ToDateTime(form["ExpiredDateVisa"]);
            }
            if (model.ExpiredDateVisa != null && model.CreatedDateVisa != null)
            {
                int age = model.ExpiredDateVisa.Value.Year - model.CreatedDateVisa.Value.Year;
                if (model.CreatedDateVisa > model.ExpiredDateVisa.Value.AddYears(-age)) age--;
                model.Deadline = age;
            }
            if (await _staffVisaRepository.Update(model))
            {
                UpdateHistory.SaveHistory(1101, "Cập nhật visa cho nhân viên " + _staffRepository.FindId(model.StaffId).Code,
                        null, //appointment
                        null, //contract
                        null, //customer
                        null, //partner
                        null, //program
                        null, //task
                        null, //tour
                        null, //quotation
                        null, //document
                        null, //history
                        null // ticket
                        );
            }

            //if (Session["ListVisa"] != null)
            //{

            //    var items = Session["ListVisa"] as List<VisaListViewModel>;
            //    return PartialView("_Partial_ListVisa", items);
            //}
            //else
            //{
            return RedirectToAction("Index");
            //}
        }
        #endregion

        #region Delete
        [HttpPost]
        public async Task<ActionResult> DeleteVisa(string id, int iscus)
        {
            if (iscus == 0) // nhan vien
            {
                await _staffVisaRepository.DeleteMany(id.Split(',').ToArray(), false);
            }
            else // khach hang
            {
                await _customerVisaRepository.DeleteMany(id.Split(',').ToArray(), false);
            }

            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Import

        [HttpPost]
        public ActionResult ImportFile(HttpPostedFileBase FileName)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1101);
                using (var excelPackage = new ExcelPackage(FileName.InputStream))
                {
                    var model = new List<VisaListViewModel>();
                    var worksheet = excelPackage.Workbook.Worksheets[1];
                    var lastRow = worksheet.Dimension.End.Row;
                    for (int row = 7; row <= lastRow; row++)
                    {
                        string cel = "";
                        var dt = new VisaListViewModel()
                        {
                            Code = worksheet.Cells["C" + row].Value != null ? worksheet.Cells["C" + row].Text : "",
                            CreateDate = DateTime.Now,
                            Name = worksheet.Cells["D" + row].Value != null ? worksheet.Cells["D" + row].Text : null,
                        };
                        // khách hàng or nhân viên
                        try
                        {
                            cel = "B";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string khnv = worksheet.Cells[cel + row].Text;
                                dt.StaffCustomer = khnv == "Khách hàng" ? true : false;
                                dt.IsCustomer = 1;
                            }
                        }
                        catch { }
                        // loại visa
                        try
                        {
                            cel = "F";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                dt.Type = worksheet.Cells[cel + row].Text;
                                dt.TypeId = LoadData.VisaTypeList().AsEnumerable().FirstOrDefault(c => c.Name == dt.Type).Id;
                            }
                        }
                        catch { }
                        // tình trạng visa
                        try
                        {
                            cel = "G";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                dt.Status = worksheet.Cells[cel + row].Text;
                                dt.StatusId = LoadData.VisaStatusList().AsEnumerable().FirstOrDefault(c => c.Name == dt.Status).Id;
                            }
                        }
                        catch { }
                        // ngày hiệu lực
                        try
                        {
                            cel = "H";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                dt.CreateDatePP = DateTime.ParseExact(worksheet.Cells[cel + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                            }
                        }
                        catch { }
                        // ngày hết hạn
                        try
                        {
                            cel = "I";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                dt.ExpiredDatePP = DateTime.ParseExact(worksheet.Cells[cel + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                            }
                        }
                        catch { }
                        // quốc gia
                        try
                        {
                            cel = "E";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                dt.Country = worksheet.Cells[cel + row].Text;
                                dt.CountryId = _tagsRepository.GetAllAsQueryable().AsEnumerable().FirstOrDefault(c => c.Tag == dt.Country && c.TypeTag == 3 && c.IsDelete == false).Id;
                            }
                        }
                        catch { }
                        //
                        model.Add(dt);
                    }
                    Session["listPartnerImport"] = model;
                    return PartialView("_Partial_ImportDataList", model);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveImport(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1101);
            try
            {
                List<VisaListViewModel> list = Session["listPartnerImport"] as List<VisaListViewModel>;

                if (id == 1) // cap nhat
                {
                    foreach (var item in list)
                    {
                        if (item.StaffCustomer == true) // visa của khách hàng
                        {
                            var cus = _customerRepository.GetAllAsQueryable().FirstOrDefault(p => p.FullName == item.Name);
                            var visa = _db.tbl_CustomerVisa.FirstOrDefault(p => p.VisaNumber == item.Code && p.CustomerId == cus.Id);
                            if (visa != null)
                            {
                                // đã có visa --> cập nhật
                                visa.CreatedDate = visa.CreatedDate;
                                visa.CreatedDateVisa = item.CreateDatePP;
                                visa.CustomerId = cus.Id;
                                visa.DictionaryId = item.StatusId;
                                visa.ExpiredDateVisa = item.ExpiredDatePP;
                                visa.IsDelete = false;
                                visa.ModifiedDate = DateTime.Now;
                                visa.TagsId = item.CountryId;
                                visa.VisaTypeId = item.TypeId;
                                visa.VisaNumber = item.Code;
                                await _customerVisaRepository.Update(visa);
                            }
                        }
                        else
                        {
                            var st = _staffRepository.GetAllAsQueryable().FirstOrDefault(p => p.FullName == item.Name);
                            var visa = _db.tbl_StaffVisa.FirstOrDefault(p => p.VisaNumber == item.Code && p.StaffId == st.Id);
                            if (visa != null)
                            {
                                // đã có visa --> cập nhật
                                visa.CreatedDate = visa.CreatedDate;
                                visa.CreatedDateVisa = item.CreateDatePP;
                                visa.StaffId = st.Id;
                                visa.DictionaryId = item.StatusId;
                                visa.ExpiredDateVisa = item.ExpiredDatePP;
                                visa.IsDelete = false;
                                visa.ModifiedDate = DateTime.Now;
                                visa.TagsId = item.CountryId;
                                visa.VisaType = item.TypeId;
                                visa.VisaNumber = item.Code;
                                await _staffVisaRepository.Update(visa);
                            }
                        }
                    }
                }
                else // them moi
                {
                    foreach (var item in list)
                    {
                        if (item.StaffCustomer == true) // visa của khách hàng
                        {
                            var cus = _customerRepository.GetAllAsQueryable().FirstOrDefault(p => p.FullName == item.Name);
                            var vc = new tbl_CustomerVisa
                            {
                                CreatedDate = DateTime.Now,
                                CreatedDateVisa = item.CreateDatePP,
                                CustomerId = cus.Id,
                                DictionaryId = item.StatusId,
                                ExpiredDateVisa = item.ExpiredDatePP,
                                IsDelete = false,
                                ModifiedDate = DateTime.Now,
                                TagsId = item.CountryId,
                                VisaTypeId = item.TypeId,
                                VisaNumber = item.Code
                            };
                            await _customerVisaRepository.Create(vc);
                        }
                        else
                        {
                            // chưa có visa --> thêm mới
                            var st = _staffRepository.GetAllAsQueryable().FirstOrDefault(p => p.FullName == item.Name);
                            var vc = new tbl_StaffVisa
                            {
                                CreatedDate = DateTime.Now,
                                CreatedDateVisa = item.CreateDatePP,
                                StaffId = st.Id,
                                DictionaryId = item.StatusId,
                                ExpiredDateVisa = item.ExpiredDatePP,
                                IsDelete = false,
                                ModifiedDate = DateTime.Now,
                                TagsId = item.CountryId,
                                VisaType = item.TypeId,
                                VisaNumber = item.Code
                            };
                            await _staffVisaRepository.Create(vc);
                        }
                    }
                }
                Session["listPartnerImport"] = null;
            }
            catch
            {
                Session["listPartnerImport"] = null;
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region Excel Sample

        public void ExportExcelTemplateVisa(MemoryStream stream, string templateFile, IDictionary<string, string> header = null)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream, new MemoryStream(System.IO.File.ReadAllBytes(templateFile))))
            {
                var ws = xlPackage.Workbook.Worksheets[1]; //first worksheet
                var valWs = xlPackage.Workbook.Worksheets.Add("Validation");
                valWs.Hidden = eWorkSheetHidden.VeryHidden;

                //ws.AddHeader(header);

                // khách hàng or nhân viên
                var khnv = new List<ExportItem>();
                khnv.Add(new ExportItem { Text = "Khách hàng", Value = 1 });
                khnv.Add(new ExportItem { Text = "Nhân viên", Value = 0 });
                var columnIndex = ws.GetColumnIndex(VisaColumn.KHACHHANG.ToString());
                ws.AddListValidation(valWs, khnv, columnIndex, "Lỗi", "Lỗi", "KHACHHANG", "KHACHHANGName");

                // quốc gia
                var noicappassport = LoadData.DropdownlistCountry()
                    .Select(p => new ExportItem
                    {
                        Text = p.Tags,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(VisaColumn.QUOCGIA.ToString());
                ws.AddListValidation(valWs, noicappassport, columnIndex, "Lỗi", "Lỗi", "QUOCGIA", "QUOCGIAName");

                // loại visa
                var loaivisa = LoadData.VisaTypeList()
                    .Select(p => new ExportItem
                    {
                        Text = p.Name,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(VisaColumn.LOAIVISA.ToString());
                ws.AddListValidation(valWs, loaivisa, columnIndex, "Lỗi", "Lỗi", "LOAIVISA", "LOAIVISAName");

                // tình trạng
                var tinhtrang = LoadData.VisaStatusList()
                    .Select(p => new ExportItem
                    {
                        Text = p.Name,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(VisaColumn.TINHTRANG.ToString());
                ws.AddListValidation(valWs, tinhtrang, columnIndex, "Lỗi", "Lỗi", "TINHTRANG", "TINHTRANGName");

                xlPackage.Save();
            }
        }

        public ActionResult ExcelSample()
        {
            try
            {
                IDictionary<string, string> header = new Dictionary<string, string>();
                header.Add("TITLE", "DANH SÁCH VISA");

                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    string templateFile = System.Web.HttpContext.Current.Server.MapPath("~\\Upload\\ImportExport\\Import_VisaTOURDEMO.xlsx");
                    ExportExcelTemplateVisa(stream, templateFile, header);
                    bytes = stream.ToArray();
                }

                string fileName = "Mau-import-visa-TOURDEMO.xlsx";
                return File(bytes, "text/xls", fileName);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        #endregion
    }
}