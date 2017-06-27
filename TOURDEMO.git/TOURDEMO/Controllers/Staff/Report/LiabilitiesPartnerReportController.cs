using CRM.Core;
using CRM.Infrastructure;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TRAVELPLUS.Models;
using TRAVELPLUS.Utilities;

namespace TRAVELPLUS.Controllers.Report
{
    [Authorize]
    public class LiabilitiesPartnerReportController : BaseController
    {
        // GET: LiabilitiesPartnerReport

        #region Init

        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_ServicesPartner> _servicesPartnerRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;
        private IGenericRepository<tbl_TourOption> _tourOptionRepository;
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
        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_LiabilityCustomer> _liabilityCustomerRepository;
        private IGenericRepository<tbl_LiabilityPartner> _liabilityPartnerRepository;
        private IGenericRepository<tbl_CustomerContact> _customerContactRepository;
        private IGenericRepository<tbl_CustomerContactVisa> _customerContactVisaRepository;
        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
        private DataContext _db;

        public LiabilitiesPartnerReportController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_ServicesPartner> servicesPartnerRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IGenericRepository<tbl_TourOption> tourOptionRepository,
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
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_LiabilityCustomer> liabilityCustomerRepository,
            IGenericRepository<tbl_LiabilityPartner> liabilityPartnerRepository,
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
            Permission(clsPermission.GetUser().PermissionID, 1118);
            return View();
        }

        [ChildActionOnly]
        public ActionResult _Partial_ListTourReport()
        {
            Permission(clsPermission.GetUser().PermissionID, 1118);

            var model = _liabilityPartnerRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.IsDelete == false
                    && (p.StaffId == maNV | maNV == 0)
                    & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                    & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                    & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0))
                .Select(p => new LiabilitiesPartnerReportViewModel()
                {
                    Id = p.Id,
                    Code = _tourRepository.FindId(p.TourId).Code,
                    Name = _tourRepository.FindId(p.TourId).Name,
                    Partner = p.PartnerId != null ? _partnerRepository.FindId(p.PartnerId).Name : "",
                    Service = p.ServiceId != null ? _dictionaryRepository.FindId(p.ServiceId).Name : "",
                    NgayKhoiHanh = _tourRepository.FindId(p.TourId).StartDate != null ? _tourRepository.FindId(p.TourId).StartDate.Value.ToString("dd/MM/yyyy") : "",
                    NguoiDieuHanh = _tourRepository.FindId(p.TourId).StaffId != null ? _staffRepository.FindId(_tourRepository.FindId(p.TourId).StaffId).FullName : "",
                    Tag = _tourRepository.FindId(p.TourId).DestinationPlace != null ? _tagsRepository.FindId(_tourRepository.FindId(p.TourId).DestinationPlace).Tag : "",
                    TongGTDichVu = p.ServicePrice != null ? p.ServicePrice : 0,
                    TongGTConLai = p.TotalRemaining != null ? p.TotalRemaining : 0,
                    TongGTThanhToan = p.FirstPayment + p.SecondPayment,
                    Currency = _dictionaryRepository.FindId(p.FirstCurrencyType).Name
                }).ToList();
            return PartialView("_Partial_ListTourReport", model);
        }

        public ActionResult FilterTour(DateTime? start, DateTime? end, int? loai, int? vitri)
        {
            Permission(clsPermission.GetUser().PermissionID, 1118);

            var model = _liabilityPartnerRepository.GetAllAsQueryable().AsEnumerable()
                            .Where(p => (p.StaffId == maNV | maNV == 0)
                                & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                                & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                                & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0)
                                && (start == end ? start.Value.ToString("dd-MM-yyyy") == _tourRepository.FindId(p.TourId).StartDate.Value.ToString("dd-MM-yyyy") : start <= _tourRepository.FindId(p.TourId).StartDate && _tourRepository.FindId(p.TourId).StartDate <= end)
                                && _tourRepository.FindId(p.TourId).TypeTourId == (loai == 0 ? _tourRepository.FindId(p.TourId).TypeTourId : loai)
                                && _tourRepository.FindId(p.TourId).DestinationPlace == (vitri == 0 ? _tourRepository.FindId(p.TourId).DestinationPlace : vitri)
                                && p.IsDelete == false)
                            .Select(p => new LiabilitiesPartnerReportViewModel()
                            {
                                Id = p.Id,
                                Code = _tourRepository.FindId(p.TourId).Code,
                                Name = _tourRepository.FindId(p.TourId).Name,
                                Partner = p.PartnerId != null ? _partnerRepository.FindId(p.PartnerId).Name : "",
                                Service = p.ServiceId != null ? _dictionaryRepository.FindId(p.ServiceId).Name : "",
                                NgayKhoiHanh = _tourRepository.FindId(p.TourId).StartDate != null ? _tourRepository.FindId(p.TourId).StartDate.Value.ToString("dd/MM/yyyy") : "",
                                NguoiDieuHanh = _tourRepository.FindId(p.TourId).StaffId != null ? _staffRepository.FindId(_tourRepository.FindId(p.TourId).StaffId).FullName : "",
                                Tag = _tourRepository.FindId(p.TourId).DestinationPlace != null ? _tagsRepository.FindId(_tourRepository.FindId(p.TourId).DestinationPlace).Tag : "",
                                TongGTDichVu = p.ServicePrice != null ? p.ServicePrice : 0,
                                TongGTConLai = p.TotalRemaining != null ? p.TotalRemaining : 0,
                                TongGTThanhToan = p.FirstPayment + p.SecondPayment,
                                Currency = _dictionaryRepository.FindId(p.FirstCurrencyType).Name
                            }).ToList();

            return PartialView("_Partial_ListTourReport", model);
        }

        #endregion

        #region ExportExcel

        public ActionResult ExportExcel(FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1118);

                HtmlToText converthtml = new HtmlToText();
                string filename = "";
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    if (form["listItemId"] != null && form["listItemId"] != "")
                    {
                        var lstExport = new List<tbl_LiabilityPartner>();
                        var listIds = form["listItemId"].Split(',');
                        listIds = listIds.Take(listIds.Count() - 1).ToArray();
                        if (listIds.Count() > 0)
                        {
                            foreach (var i in listIds)
                            {
                                lstExport.Add(_liabilityPartnerRepository.FindId(Convert.ToInt32(i)));
                            }
                            var exp = lstExport.Select(p => new LiabilitiesPartnerReportViewModel()
                                        {
                                            Id = p.Id,
                                            Code = _tourRepository.FindId(p.TourId).Code,
                                            Name = _tourRepository.FindId(p.TourId).Name,
                                            Partner = p.PartnerId != null ? _partnerRepository.FindId(p.PartnerId).Name : "",
                                            Service = p.ServiceId != null ? _dictionaryRepository.FindId(p.ServiceId).Name : "",
                                            NgayKhoiHanh = _tourRepository.FindId(p.TourId).StartDate != null ? _tourRepository.FindId(p.TourId).StartDate.Value.ToString("dd/MM/yyyy") : "",
                                            NguoiDieuHanh = _tourRepository.FindId(p.TourId).StaffId != null ? _staffRepository.FindId(_tourRepository.FindId(p.TourId).StaffId).FullName : "",
                                            Tag = _tourRepository.FindId(p.TourId).DestinationPlace != null ? _tagsRepository.FindId(_tourRepository.FindId(p.TourId).DestinationPlace).Tag : "",
                                            TongGTDichVu = p.ServicePrice != null ? p.ServicePrice : 0,
                                            TongGTConLai = p.TotalRemaining != null ? p.TotalRemaining : 0,
                                            //TongGTThanhToan = (p.FirstPayment != null ? p.FirstPayment : 0) + (p.SecondPayment != null ? p.SecondPayment : 0)
                                            TongGTThanhToan = p.FirstPayment + p.SecondPayment,
                                            Currency = _dictionaryRepository.FindId(p.FirstCurrencyType).Name
                                        }).ToList();
                            filename = "[TRAVELPLUS] Thống kê doanh thu - công nợ đối tác từ";
                            ExportToursToXlsx(stream, exp, filename);
                        }
                    }
                    else
                    {
                        var tours = _liabilityPartnerRepository.GetAllAsQueryable().AsEnumerable()
                                        .Where(p => (p.StaffId == maNV | maNV == 0)
                                            & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                                            & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                                            & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0)
                                            && (form["tungay"] == form["denngay"] ? Convert.ToDateTime(form["tungay"]).ToString("dd-MM-yyyy") == _tourRepository.FindId(p.TourId).StartDate.Value.ToString("dd-MM-yyyy") : Convert.ToDateTime(form["tungay"]) <= _tourRepository.FindId(p.TourId).StartDate && _tourRepository.FindId(p.TourId).StartDate <= Convert.ToDateTime(form["denngay"]))
                                            && _tourRepository.FindId(p.TourId).TypeTourId == (form["loai"] == "0" ? _tourRepository.FindId(p.TourId).TypeTourId : Convert.ToInt32(form["loai"].ToString()))
                                            && _tourRepository.FindId(p.TourId).DestinationPlace == (form["vitri"] == "0" ? _tourRepository.FindId(p.TourId).DestinationPlace : Convert.ToInt32(form["vitri"].ToString()))
                                            && p.IsDelete == false)
                                            .Select(p => new LiabilitiesPartnerReportViewModel()
                                            {
                                                Id = p.Id,
                                                Code = _tourRepository.FindId(p.TourId).Code,
                                                Name = _tourRepository.FindId(p.TourId).Name,
                                                Partner = p.PartnerId != null ? _partnerRepository.FindId(p.PartnerId).Name : "",
                                                Service = p.ServiceId != null ? _dictionaryRepository.FindId(p.ServiceId).Name : "",
                                                NgayKhoiHanh = _tourRepository.FindId(p.TourId).StartDate != null ? _tourRepository.FindId(p.TourId).StartDate.Value.ToString("dd/MM/yyyy") : "",
                                                NguoiDieuHanh = _tourRepository.FindId(p.TourId).StaffId != null ? _staffRepository.FindId(_tourRepository.FindId(p.TourId).StaffId).FullName : "",
                                                Tag = _tourRepository.FindId(p.TourId).DestinationPlace != null ? _tagsRepository.FindId(_tourRepository.FindId(p.TourId).DestinationPlace).Tag : "",
                                                TongGTDichVu = p.ServicePrice != null ? p.ServicePrice : 0,
                                                TongGTConLai = p.TotalRemaining != null ? p.TotalRemaining : 0,
                                                //TongGTThanhToan = (p.FirstPayment != null ? p.FirstPayment : 0) + (p.SecondPayment != null ? p.SecondPayment : 0)
                                                TongGTThanhToan = p.FirstPayment + p.SecondPayment,
                                                Currency = _dictionaryRepository.FindId(p.FirstCurrencyType).Name
                                            }).ToList();
                        filename = "[TRAVELPLUS] Thống kê doanh thu - công nợ đối tác từ " + Convert.ToDateTime(form["tungay"]).ToString("dd-MM-yyyy") + " đến " + Convert.ToDateTime(form["denngay"]).ToString("dd-MM-yyyy");
                        ExportToursToXlsx(stream, tours, filename);
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

        public virtual void ExportToursToXlsx(Stream stream, IList<LiabilitiesPartnerReportViewModel> tours, string headername)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {

                var worksheet = xlPackage.Workbook.Worksheets.Add("Tours");

                var properties = new[]
                    {
                        "STT",
                        "Code tour",
                        "Tên tour",
                        "Điểm đến",
                        "Tên đối tác",
                        "Dịch vụ",
                        "Ngày khởi hành",
                        "Tổng GT dịch vụ (VNĐ)",
                        "Tổng GT thanh toán (VNĐ)",
                        "Tổng giá trị còn lại (VNĐ)",
                        "Điều hành"
                    };

                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[3, i + 1].Value = properties[i];
                }

                worksheet.Cells["a1:k2"].Value = headername.ToUpper();
                worksheet.Cells["a1:k2"].Style.Font.SetFromFont(new Font("Tahoma", 15));
                worksheet.Cells["a1:k2"].Style.Font.Bold = true;
                worksheet.Cells["a1:k2"].Merge = true;
                worksheet.Cells["a1:k2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["a1:k2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["a1:k2"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));

                int row = 3;
                int stt = 1;
                foreach (var t in tours)
                {
                    row++;
                    int col = 1;

                    worksheet.Cells[row, col].Value = stt;
                    col++;

                    worksheet.Cells[row, col].Value = t.Code;
                    col++;

                    worksheet.Cells[row, col].Value = t.Name;
                    col++;

                    worksheet.Cells[row, col].Value = t.Tag;
                    col++;

                    worksheet.Cells[row, col].Value = t.Partner;
                    col++;

                    worksheet.Cells[row, col].Value = t.Service;
                    col++;

                    worksheet.Cells[row, col].Value = t.NgayKhoiHanh;
                    col++;

                    worksheet.Cells[row, col].Value = t.TongGTDichVu;
                    worksheet.Cells[row, col].Style.Numberformat.Format = "#,###,###";
                    col++;

                    worksheet.Cells[row, col].Value = t.TongGTThanhToan;
                    worksheet.Cells[row, col].Style.Numberformat.Format = "#,###,###";
                    col++;

                    worksheet.Cells[row, col].Value = t.TongGTConLai;
                    worksheet.Cells[row, col].Style.Numberformat.Format = "#,###,###";
                    col++;

                    worksheet.Cells[row, col].Value = t.NguoiDieuHanh;
                    col++;

                    stt++;
                }
                worksheet.Cells["a3:k" + row].Style.Font.SetFromFont(new Font("Tahoma", 8));

                worksheet.Cells["a3:k3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["a3:k3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["a3:k3"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
                worksheet.Cells["a3:k3"].Style.Font.Bold = true;
                worksheet.Cells["a3:k3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Row(3).Height = 20;

                worksheet.Cells["a3:k" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a3:k" + row].Style.Border.Top.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["a3:k" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a3:k" + row].Style.Border.Left.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["a3:k" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a3:k" + row].Style.Border.Bottom.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["a3:k" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a3:k" + row].Style.Border.Right.Color.SetColor(Color.FromArgb(169, 169, 169));

                row++;

                worksheet.Cells["a" + row + ":k" + row].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["a" + row + ":k" + row].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
                // tong gt hop dong
                worksheet.Cells["h" + row].Style.Font.Bold = true;
                worksheet.Cells["h" + row].Style.Numberformat.Format = "#,###,###";
                worksheet.Cells["h" + row].Value = tours.Sum(p => p.TongGTDichVu);
                // tong gt thanh toan
                worksheet.Cells["i" + row].Style.Font.Bold = true;
                worksheet.Cells["i" + row].Style.Numberformat.Format = "#,###,###";
                worksheet.Cells["i" + row].Value = tours.Sum(p => p.TongGTThanhToan);
                // tong gt con lai
                worksheet.Cells["j" + row].Style.Font.Bold = true;
                worksheet.Cells["j" + row].Style.Numberformat.Format = "#,###,###";
                worksheet.Cells["j" + row].Value = tours.Sum(p => p.TongGTConLai);

                worksheet.Cells["a3:k" + row].AutoFitColumns();

                xlPackage.Save();
            }
        }

        #endregion
    }
}