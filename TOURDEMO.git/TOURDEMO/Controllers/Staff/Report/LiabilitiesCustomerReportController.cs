using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TRAVELPLUS.Models;
using TRAVELPLUS.Utilities;
using CRM.Core;
using CRM.Infrastructure;
using System.IO;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;

namespace TRAVELPLUS.Controllers
{
    [Authorize]
    public class LiabilitiesCustomerReportController : BaseController
    {
        //
        // GET: /LiabilitiesReport/

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
        private DataContext _db;

        public LiabilitiesCustomerReportController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
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
            Permission(clsPermission.GetUser().PermissionID, 1119);
            return View();
        }

        [ChildActionOnly]
        public ActionResult _Partial_ListTourReport()
        {
            Permission(clsPermission.GetUser().PermissionID, 1119);

            var congnokh = _liabilityCustomerRepository.GetAllAsQueryable().Where(p => p.IsDelete == false);
            var model = _tourRepository.GetAllAsQueryable().AsEnumerable()
                    .Where(p => p.IsDelete == false
                    & (p.StaffId == maNV | maNV == 0)
                    & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                    & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                    & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0))
                .Select(p => new LiabilitiesCustomerReportViewModel()
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    NgayKetThuc = p.EndDate != null ? p.EndDate.Value.ToString("dd/MM/yyyy") : "",
                    NgayKhoiHanh = p.StartDate != null ? p.StartDate.Value.ToString("dd/MM/yyyy") : "",
                    NguoiDieuHanh = p.StaffId != null ? p.tbl_Staff.FullName : "",
                    Tag = p.DestinationPlace != null ? p.tbl_TagsDestinationPlace.Tag : "",
                    Customer = p.CustomerId != null ? p.tbl_Customer.FullName : "",
                    TongGTConLai = congnokh.Where(x => x.TourId == p.Id).Sum(x => x.TotalRemaining) ?? 0,
                    TongGTHopDong = congnokh.Where(x => x.TourId == p.Id).Sum(x => x.TotalContract) ?? 0,
                    TongGTThanhLy = congnokh.Where(x => x.TourId == p.Id).Sum(x => x.TotalLiquidation) ?? 0,
                    TongGTThanhToan = congnokh.Where(x => x.TourId == p.Id).Sum(x => x.FirstPrice + x.SecondPrice + x.ThirdPrice) ?? 0,
                    Currency = congnokh.FirstOrDefault(x => x.TourId == p.Id) != null ? congnokh.FirstOrDefault(x => x.TourId == p.Id).tbl_DictionaryCurrency.Name : ""
                }).ToList();
            return PartialView("_Partial_ListTourReport", model);
        }

        public ActionResult FilterTour(DateTime? start, DateTime? end, int? loai, int? vitri)
        {
            Permission(clsPermission.GetUser().PermissionID, 1119);
            var congnokh = _liabilityCustomerRepository.GetAllAsQueryable().Where(p => p.IsDelete == false);

            var model = _tourRepository.GetAllAsQueryable().AsEnumerable()
                            .Where(p => (p.StaffId == maNV | maNV == 0)
                                & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                                & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                                & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0))
                                .Where(p => (start == end ? p.StartDate.Value.ToString("dd/MM/yyyy") == start.Value.ToString("dd/MM/yyyy") : start <= p.StartDate && p.StartDate <= end)
                                && p.TypeTourId == (loai == 0 ? p.TypeTourId : loai)
                                && p.DestinationPlace == (vitri == 0 ? p.DestinationPlace : vitri)
                                && p.IsDelete == false)
                            .Select(p => new LiabilitiesCustomerReportViewModel()
                            {
                                Id = p.Id,
                                Code = p.Code,
                                Name = p.Name,
                                NgayKetThuc = p.EndDate != null ? p.EndDate.Value.ToString("dd/MM/yyyy") : "",
                                NgayKhoiHanh = p.StartDate != null ? p.StartDate.Value.ToString("dd/MM/yyyy") : "",
                                NguoiDieuHanh = p.StaffId != null ? p.tbl_Staff.FullName : "",
                                Tag = p.DestinationPlace != null ? p.tbl_TagsDestinationPlace.Tag : "",
                                Customer = p.CustomerId != null ? p.tbl_Customer.FullName : "",
                                TongGTConLai = congnokh.Where(x => x.TourId == p.Id).Sum(x => x.TotalRemaining) ?? 0,
                                TongGTHopDong = congnokh.Where(x => x.TourId == p.Id).Sum(x => x.TotalContract) ?? 0,
                                TongGTThanhLy = congnokh.Where(x => x.TourId == p.Id).Sum(x => x.TotalLiquidation) ?? 0,
                                TongGTThanhToan = congnokh.Where(x => x.TourId == p.Id).Sum(x => x.FirstPrice + x.SecondPrice + x.ThirdPrice) ?? 0,
                                Currency = congnokh.FirstOrDefault(x => x.TourId == p.Id) != null ? congnokh.FirstOrDefault(x => x.TourId == p.Id).tbl_DictionaryCurrency.Name : ""
                            }).ToList();
            return PartialView("_Partial_ListTourReport", model);
        }

        #endregion

        #region ExportExcel

        public ActionResult ExportExcel(FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1119);
                var congnokh = _liabilityCustomerRepository.GetAllAsQueryable().Where(p => p.IsDelete == false);
                HtmlToText converthtml = new HtmlToText();
                string filename = "";
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    if (form["listItemId"] != null && form["listItemId"] != "")
                    {
                        var lstExport = new List<tbl_Tour>();
                        var listIds = form["listItemId"].Split(',');
                        listIds = listIds.Take(listIds.Count() - 1).ToArray();
                        if (listIds.Count() > 0)
                        {
                            foreach (var i in listIds)
                            {
                                lstExport.Add(_tourRepository.FindId(Convert.ToInt32(i)));
                            }
                            var exp = lstExport.Select(p => new LiabilitiesCustomerReportViewModel()
                                            {
                                                Id = p.Id,
                                                Code = p.Code,
                                                Name = p.Name,
                                                NgayKetThuc = p.EndDate != null ? p.EndDate.Value.ToString("dd/MM/yyyy") : "",
                                                NgayKhoiHanh = p.StartDate != null ? p.StartDate.Value.ToString("dd/MM/yyyy") : "",
                                                NguoiDieuHanh = p.StaffId != null ? p.tbl_Staff.FullName : "",
                                                Tag = p.DestinationPlace != null ? p.tbl_TagsDestinationPlace.Tag : "",
                                                Customer = p.CustomerId != null ? p.tbl_Customer.FullName : "",
                                                TongGTConLai = congnokh.Where(x => x.TourId == p.Id).Sum(x => x.TotalRemaining) ?? 0,
                                                TongGTHopDong = congnokh.Where(x => x.TourId == p.Id).Sum(x => x.TotalContract) ?? 0,
                                                TongGTThanhLy = congnokh.Where(x => x.TourId == p.Id).Sum(x => x.TotalLiquidation) ?? 0,
                                                TongGTThanhToan = congnokh.Where(x => x.TourId == p.Id).Sum(x => x.FirstPrice + x.SecondPrice + x.ThirdPrice) ?? 0,
                                                Currency = congnokh.FirstOrDefault(x => x.TourId == p.Id) != null ? congnokh.FirstOrDefault(x => x.TourId == p.Id).tbl_DictionaryCurrency.Name : ""
                                            }).ToList();
                            filename = "[TRAVELPLUS] Thống kê doanh thu - công nợ khách hàng";
                            ExportToursToXlsx(stream, exp, filename);
                        }
                    }
                    else
                    {
                        var tours = _tourRepository.GetAllAsQueryable().AsEnumerable()
                                            .Where(p => (p.StaffId == maNV | maNV == 0)
                                                & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                                                & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                                                & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0))
                                           .Where(p => (form["tungay"] == form["denngay"] ? p.StartDate.Value.ToString("dd/MM/yyyy") == Convert.ToDateTime(form["tungay"]).ToString("dd/MM/yyyy") : Convert.ToDateTime(form["tungay"]) <= p.StartDate && p.StartDate <= Convert.ToDateTime(form["denngay"]))
                                               && p.TypeTourId == (form["loai"] == "0" ? p.TypeTourId : Convert.ToInt32(form["loai"].ToString()))
                                               && p.DestinationPlace == (form["vitri"] == "0" ? p.DestinationPlace : Convert.ToInt32(form["vitri"].ToString()))
                                               && p.IsDelete == false)
                                           .Select(p => new LiabilitiesCustomerReportViewModel()
                                           {
                                               Id = p.Id,
                                               Code = p.Code,
                                               Name = p.Name,
                                               NgayKetThuc = p.EndDate != null ? p.EndDate.Value.ToString("dd/MM/yyyy") : "",
                                               NgayKhoiHanh = p.StartDate != null ? p.StartDate.Value.ToString("dd/MM/yyyy") : "",
                                               NguoiDieuHanh = p.StaffId != null ? p.tbl_Staff.FullName : "",
                                               Tag = p.DestinationPlace != null ? p.tbl_TagsDestinationPlace.Tag : "",
                                               Customer = p.CustomerId != null ? p.tbl_Customer.FullName : "",
                                               TongGTConLai = congnokh.Where(x => x.TourId == p.Id).Sum(x => x.TotalRemaining) ?? 0,
                                               TongGTHopDong = congnokh.Where(x => x.TourId == p.Id).Sum(x => x.TotalContract) ?? 0,
                                               TongGTThanhLy = congnokh.Where(x => x.TourId == p.Id).Sum(x => x.TotalLiquidation) ?? 0,
                                               TongGTThanhToan = congnokh.Where(x => x.TourId == p.Id).Sum(x => x.FirstPrice + x.SecondPrice + x.ThirdPrice) ?? 0,
                                               Currency = congnokh.FirstOrDefault(x => x.TourId == p.Id) != null ? congnokh.FirstOrDefault(x => x.TourId == p.Id).tbl_DictionaryCurrency.Name : ""
                                           }).ToList();
                        filename = "[TRAVELPLUS] Thống kê doanh thu - công nợ khách hàng từ " + Convert.ToDateTime(form["tungay"]).ToString("dd-MM-yyyy") + " đến " + Convert.ToDateTime(form["denngay"]).ToString("dd-MM-yyyy");
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

        public virtual void ExportToursToXlsx(Stream stream, IList<LiabilitiesCustomerReportViewModel> tours, string headername)
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
                        "Khách hàng",
                        "Ngày khởi hành",
                        "Ngày kết thúc",
                        "Tổng GT hợp đồng (VNĐ)",
                        "Tổng GT thanh lý (VNĐ)",
                        "Tổng GT thanh toán (VNĐ)",
                        "Tổng giá trị còn lại (VNĐ)",
                        "Điều hành"
                    };

                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[3, i + 1].Value = properties[i];
                }

                worksheet.Cells["a1:l2"].Value = headername.ToUpper();
                worksheet.Cells["a1:l2"].Style.Font.SetFromFont(new Font("Tahoma", 15));
                worksheet.Cells["a1:l2"].Style.Font.Bold = true;
                worksheet.Cells["a1:l2"].Merge = true;
                worksheet.Cells["a1:l2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["a1:l2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                //worksheet.Cells["a1:l2"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));

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

                    worksheet.Cells[row, col].Value = t.Customer;
                    col++;

                    worksheet.Cells[row, col].Value = t.NgayKhoiHanh;
                    col++;

                    worksheet.Cells[row, col].Value = t.NgayKetThuc;
                    col++;

                    worksheet.Cells[row, col].Value = t.TongGTHopDong;
                    worksheet.Cells[row, col].Style.Numberformat.Format = "#,###,###";
                    col++;

                    worksheet.Cells[row, col].Value = t.TongGTThanhLy;
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
                worksheet.Cells["a3:l" + row].Style.Font.SetFromFont(new Font("Tahoma", 8));

                worksheet.Cells["a3:l3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["a3:l3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["a3:l3"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
                worksheet.Cells["a3:l3"].Style.Font.Bold = true;
                worksheet.Cells["a3:l3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Row(3).Height = 20;

                worksheet.Cells["a3:l" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a3:l" + row].Style.Border.Top.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["a3:l" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a3:l" + row].Style.Border.Left.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["a3:l" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a3:l" + row].Style.Border.Bottom.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["a3:l" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a3:l" + row].Style.Border.Right.Color.SetColor(Color.FromArgb(169, 169, 169));

                row++;

                worksheet.Cells["a" + row + ":l" + row].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["a" + row + ":l" + row].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
                // tong gt hop dong
                worksheet.Cells["h" + row].Style.Font.Bold = true;
                worksheet.Cells["h" + row].Style.Numberformat.Format = "#,###,###";
                worksheet.Cells["h" + row].Value = tours.Sum(p => p.TongGTHopDong);
                // tong gt thanh ly
                worksheet.Cells["i" + row].Style.Font.Bold = true;
                worksheet.Cells["i" + row].Style.Numberformat.Format = "#,###,###";
                worksheet.Cells["i" + row].Value = tours.Sum(p => p.TongGTThanhLy);
                // tong gt thanh toan
                worksheet.Cells["j" + row].Style.Font.Bold = true;
                worksheet.Cells["j" + row].Style.Numberformat.Format = "#,###,###";
                worksheet.Cells["j" + row].Value = tours.Sum(p => p.TongGTThanhToan);
                // tong gt con lai
                worksheet.Cells["k" + row].Style.Font.Bold = true;
                worksheet.Cells["k" + row].Style.Numberformat.Format = "#,###,###";
                worksheet.Cells["k" + row].Value = tours.Sum(p => p.TongGTConLai);

                worksheet.Cells["a3:l" + row].AutoFitColumns();

                xlPackage.Save();
            }
        }

        #endregion
    }
}
