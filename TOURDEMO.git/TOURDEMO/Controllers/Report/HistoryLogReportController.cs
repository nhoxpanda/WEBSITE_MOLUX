using CRM.Core;
using CRM.Infrastructure;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TOURDEMO.Models;
using TOURDEMO.Utilities;

namespace TOURDEMO.Controllers.Report
{
    public class HistoryLogReportController : BaseController
    {
        // GET: HistoryLogReport

        #region Init

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
        private IGenericRepository<tbl_LiabilityCustomer> _liabilityCustomerRepository;
        private IGenericRepository<tbl_LiabilityPartner> _liabilityPartnerRepository;
        private IGenericRepository<tbl_CustomerContact> _customerContactRepository;
        private IGenericRepository<tbl_CustomerContactVisa> _customerContactVisaRepository;
        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
        private IGenericRepository<tbl_Form> _formRepository;
        private IGenericRepository<tbl_Module> _moduleRepository;
        private DataContext _db;

        public HistoryLogReportController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
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
            IGenericRepository<tbl_LiabilityCustomer> liabilityCustomerRepository,
            IGenericRepository<tbl_LiabilityPartner> liabilityPartnerRepository,
            IGenericRepository<tbl_Form> formRepository,
            IGenericRepository<tbl_Module> moduleRepository,
            IGenericRepository<tbl_CustomerContact> customerContactRepository,
            IGenericRepository<tbl_CustomerContactVisa> customerContactVisaRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
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
            this._liabilityCustomerRepository = liabilityCustomerRepository;
            this._liabilityPartnerRepository = liabilityPartnerRepository;
            this._formRepository = formRepository;
            this._moduleRepository = moduleRepository;
            this._customerContactRepository = customerContactRepository;
            this._customerContactVisaRepository = customerContactVisaRepository;
            this._updateHistoryRepository = updateHistoryRepository;
            _db = new DataContext();
        }
        #endregion

        #region Permission

        int SDBID = 6;
        int maPB = 0, maNKD = 0, maNV = 0, maCN = 0;
        void Permission(int PermissionsId, int formId)
        {
            var list = _db.tbl_ActionData.Where(p => p.FormId == formId && p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();

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

        #region List

        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 1120);
            return View();
        }

        [ChildActionOnly]
        public ActionResult _Partial_ListHistory()
        {
            Permission(clsPermission.GetUser().PermissionID, 1120);
            var model = _updateHistoryRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.IsDelete == false)
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
            return PartialView("_Partial_ListHistory", model);
        }

        public ActionResult FilterDate(DateTime start, DateTime end, int staff)
        {
            Permission(clsPermission.GetUser().PermissionID, 1120);
            var model = _updateHistoryRepository.GetAllAsQueryable().AsEnumerable()
                            .Where(p => p.IsDelete == false
                                && (start == end ? p.CreatedDate.ToString("dd-MM-yyyy") == start.ToString("dd-MM-yyyy") : start <= p.CreatedDate && p.CreatedDate <= end)
                                && p.StaffId == (staff == 0 ? p.StaffId : staff))
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
            return PartialView("_Partial_ListHistory", model);
        }

        public ActionResult GetStartEndDate(int id)
        {
            return Json(LoadData.GetDateModal(id), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region ExportExcel

        public ActionResult ExportExcel(FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1120);

                HtmlToText converthtml = new HtmlToText();
                string filename = "";
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    if (form["listItemId"] != null && form["listItemId"] != "")
                    {
                        var lstExport = new List<tbl_UpdateHistory>();
                        var listIds = form["listItemId"].Split(',');
                        listIds = listIds.Take(listIds.Count() - 1).ToArray();
                        if (listIds.Count() > 0)
                        {
                            foreach (var i in listIds)
                            {
                                lstExport.Add(_updateHistoryRepository.FindId(Convert.ToInt32(i)));
                            }
                            var exp = lstExport.Select(p => new HistoryLogViewModel()
                            {
                                Id = p.Id,
                                Staff = _staffRepository.FindId(p.StaffId).FullName,
                                Date = string.Format("{0:dd-MM-yyyy hh:mm tt}", p.CreatedDate),
                                Note = p.Note,
                                StaffId = p.StaffId,
                                Form = p.FormId != null ? _formRepository.FindId(p.FormId).Name : "",
                                Module = p.ModuleId != null ? _moduleRepository.FindId(p.ModuleId).Name : ""
                            }).ToList();
                            filename = "[TOURDEMO] Thống kê nhật ký hệ thống";
                            ExportCustomerToXlsx(stream, exp, filename);
                        }
                    }
                    else
                    {
                        var model = _updateHistoryRepository.GetAllAsQueryable().AsEnumerable()
                                        .Where(p => p.IsDelete == false
                                            && (form["tungay"] == form["denngay"] ? p.CreatedDate.ToString("dd-MM-yyyy") == Convert.ToDateTime(form["tungay"]).ToString("dd-MM-yyyy") : Convert.ToDateTime(form["tungay"]) <= p.CreatedDate && p.CreatedDate <= Convert.ToDateTime(form["denngay"]))
                                            && p.StaffId == (form["nhanvien"] == "0" ? p.StaffId : Convert.ToInt32(form["nhanvien"])))
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
                        filename = "[TOURDEMO] Thống kê nhật ký hệ thống từ ngày " + Convert.ToDateTime(form["tungay"]).ToString("dd-MM-yyyy") + " đến " + Convert.ToDateTime(form["denngay"]).ToString("dd-MM-yyyy");
                        ExportCustomerToXlsx(stream, model, filename);
                    }
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", filename + ".xlsx");
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Index");
        }

        public virtual void ExportCustomerToXlsx(Stream stream, IList<HistoryLogViewModel> tours, string headername)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {

                var worksheet = xlPackage.Workbook.Worksheets.Add("Customers");

                var properties = new[]
                    {
                        "STT",
                        "Module",
                        "Form",
                        "Ngày xử lý",
                        "Diễn giải",
                        "Nhân viên"
                    };

                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[3, i + 1].Value = properties[i];
                }

                worksheet.Cells["a1:f2"].Value = headername.ToUpper();
                worksheet.Cells["a1:f2"].Style.Font.SetFromFont(new Font("Tahoma", 15));
                worksheet.Cells["a1:f2"].Style.Font.Bold = true;
                worksheet.Cells["a1:f2"].Merge = true;
                worksheet.Cells["a1:f2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["a1:f2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                //worksheet.Cells["a1:f2"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));

                int row = 3;
                int stt = 1;
                foreach (var t in tours)
                {
                    row++;
                    int col = 1;

                    worksheet.Cells[row, col].Value = stt;
                    col++;

                    worksheet.Cells[row, col].Value = t.Module;
                    col++;

                    worksheet.Cells[row, col].Value = t.Form;
                    col++;

                    worksheet.Cells[row, col].Value = t.Date;
                    col++;

                    worksheet.Cells[row, col].Value = t.Note != null ? HttpUtility.HtmlDecode(Regex.Replace(t.Note, "<.*?>", String.Empty)) : "";
                    col++;

                    worksheet.Cells[row, col].Value = t.Staff;
                    col++;

                    stt++;
                }
                worksheet.Cells["a3:f" + row].Style.Font.SetFromFont(new Font("Tahoma", 8));

                worksheet.Cells["a3:f3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["a3:f3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["a3:f3"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
                worksheet.Cells["a3:f3"].Style.Font.Bold = true;
                worksheet.Cells["a3:f3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Row(3).Height = 20;

                worksheet.Cells["a3:f" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a3:f" + row].Style.Border.Top.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["a3:f" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a3:f" + row].Style.Border.Left.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["a3:f" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a3:f" + row].Style.Border.Bottom.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["a3:f" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a3:f" + row].Style.Border.Right.Color.SetColor(Color.FromArgb(169, 169, 169));

                row++;

                worksheet.Cells["a3:f" + row].AutoFitColumns();

                xlPackage.Save();
            }
        }

        #endregion

        #region Delete
        public async Task<ActionResult> Delete(string listItem)
        {
            Permission(clsPermission.GetUser().PermissionID, 1120);
            try
            {
                var listIds = listItem.Split(',');
                listIds = listIds.Take(listIds.Count() - 1).ToArray();
                if (listIds.Count() > 0)
                {
                    if (await _updateHistoryRepository.DeleteMany(listIds, true))
                    {
                        return Json(1, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(0, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult DeleteTime(FormCollection form)
        {
            var model = _updateHistoryRepository.GetAllAsQueryable().AsEnumerable()
                            .Where(p => p.IsDelete == false
                                && (form["tungay"] == form["denngay"] ? p.CreatedDate.ToString("dd-MM-yyyy") == Convert.ToDateTime(form["tungay"]).ToString("dd-MM-yyyy") : Convert.ToDateTime(form["tungay"]) <= p.CreatedDate && p.CreatedDate <= Convert.ToDateTime(form["denngay"])))
                            .ToList();
            if (model.Count() > 0)
            {
                foreach (var item in model)
                {
                    _updateHistoryRepository.DeleteMany(item.Id.ToString().Split(',').ToArray(), true);
                }
            }

            return RedirectToAction("Index");
        }
        #endregion
    }
}