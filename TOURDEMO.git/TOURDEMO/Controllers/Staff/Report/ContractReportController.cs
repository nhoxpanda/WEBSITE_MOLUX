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
    public class ContractReportController : BaseController
    {
        //
        // GET: /ContractReport/

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

        public ContractReportController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
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
            Permission(clsPermission.GetUser().PermissionID, 1117);
            return View();
        }

        [ChildActionOnly]
        public ActionResult _Partial_ListContractReport()
        {
            Permission(clsPermission.GetUser().PermissionID, 1117);

            var model = _contractRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.IsDelete == false
                    & (p.StaffId == maNV | maNV == 0)
                    & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                    & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                    & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0))
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new ContractReportViewModel()
                {
                    Id = p.Id,
                    Code = p.Code,
                    KhachHang = p.CustomerId != null ? p.tbl_Customer.FullName : "",
                    NgayKy = string.Format("{0:dd-MM-yyyy}", p.ContractDate),
                    NhanVien = p.StaffId != null ? p.tbl_Staff.FullName : "",
                    TinhTrang = p.StatusContractId != null ? p.tbl_DictionaryStatus.Name : "",
                    Tour = p.TourId != null ? p.tbl_Tour.Code + " - " + p.tbl_Tour.Name : "",
                    LoaiTien = p.tbl_DictionaryCurrency.Name,
                    LoiNhuan = p.LoiNhuanDuKien,
                    ChiPhi = p.TongDuKien,
                    TongGT = p.TotalPrice
                }).ToList();
            return PartialView("_Partial_ListContractReport", model);
        }

        public string LoadHDV(int idtour)
        {
            string rs = "";
            var hdv = _tourGuideRepository.GetAllAsQueryable().Where(p => p.TourId == idtour && p.IsDelete == false).ToList();
            foreach (var item in hdv)
            {
                rs += item.tbl_Staff.FullName + "<br/>";
            }
            return rs;
        }

        public ActionResult GetStartEndDate(int id)
        {
            return Json(LoadData.GetDate(id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FilterDate(DateTime start, DateTime end, int staff)
        {
            Permission(clsPermission.GetUser().PermissionID, 1117);

            var model = _contractRepository.GetAllAsQueryable().AsEnumerable()
                            .Where(p => (p.StaffId == maNV | maNV == 0)
                                & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                                & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                                & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0))
                            .Where(p => p.IsDelete == false
                                && (start == end ? p.ContractDate.Value.ToString("dd-MM-yyyy") == start.ToString("dd-MM-yyyy") : start <= p.ContractDate && p.ContractDate <= end)
                                && (staff != 0 ? p.StaffId == staff : p.Id != 0))
                            .OrderByDescending(p => p.CreatedDate)
                            .Select(p => new ContractReportViewModel()
                            {
                                Id = p.Id,
                                Code = p.Code,
                                KhachHang = p.CustomerId != null ? p.tbl_Customer.FullName : "",
                                NgayKy = string.Format("{0:dd-MM-yyyy}", p.ContractDate),
                                NhanVien = p.StaffId != null ? p.tbl_Staff.FullName : "",
                                TinhTrang = p.StatusContractId != null ? p.tbl_DictionaryStatus.Name : "",
                                Tour = p.TourId != null ? p.tbl_Tour.Code + " - " + p.tbl_Tour.Name : "",
                                LoaiTien = p.tbl_DictionaryCurrency.Name,
                                LoiNhuan = p.LoiNhuanDuKien,
                                ChiPhi = p.TongDuKien,
                                TongGT = p.TotalPrice
                            }).ToList();
            return PartialView("_Partial_ListContractReport", model);
        }
        #endregion

        #region ExportExcel

        public string LoadHDVExport(int idtour)
        {
            string rs = "";
            var hdv = _tourGuideRepository.GetAllAsQueryable().Where(p => p.TourId == idtour
                    && p.IsDelete == false).ToList();
            foreach (var item in hdv)
            {
                rs += item.tbl_Staff.FullName + ", ";
            }
            return rs;
        }

        public ActionResult ExportExcel(FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1117);

                var contract = _contractRepository.GetAllAsQueryable();
                HtmlToText converthtml = new HtmlToText();
                string filename = "";
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    if (form["listItemId"] != null && form["listItemId"] != "")
                    {
                        var lstExport = new List<tbl_Contract>();
                        var listIds = form["listItemId"].Split(',');
                        listIds = listIds.Take(listIds.Count() - 1).ToArray();
                        if (listIds.Count() > 0)
                        {
                            foreach (var i in listIds)
                            {
                                lstExport.Add(_contractRepository.FindId(Convert.ToInt32(i)));
                            }
                            var exp = lstExport.Select(p => new ContractReportViewModel()
                                            {
                                                Id = p.Id,
                                                Code = p.Code,
                                                KhachHang = p.CustomerId != null ? p.tbl_Customer.FullName : "",
                                                NgayKy = string.Format("{0:dd-MM-yyyy}", p.ContractDate),
                                                NhanVien = p.StaffId != null ? p.tbl_Staff.FullName : "",
                                                TinhTrang = p.StatusContractId != null ? p.tbl_DictionaryStatus.Name : "",
                                                Tour = p.TourId != null ? p.tbl_Tour.Code + " - " + p.tbl_Tour.Name : "",
                                                LoaiTien = p.tbl_DictionaryCurrency.Name,
                                                LoiNhuan = p.LoiNhuanDuKien,
                                                ChiPhi = p.TongDuKien,
                                                TongGT = p.TotalPrice
                                            }).ToList();
                            filename = "[TRAVELPLUS] Thống kê báo cáo số lượng hợp đồng";
                            ExportContractToXlsx(stream, exp, filename);
                        }
                    }
                    else
                    {
                        var model = _contractRepository.GetAllAsQueryable().AsEnumerable()
                                .Where(p => (p.StaffId == maNV | maNV == 0)
                                    & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                                    & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                                    & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0))
                                .Where(p => p.IsDelete == false
                                    && (form["tungay"] == form["denngay"] ? p.ContractDate.Value.ToString("dd-MM-yyyy") == Convert.ToDateTime(form["tungay"]).ToString("dd-MM-yyyy") : Convert.ToDateTime(form["tungay"]) <= p.ContractDate && p.ContractDate <= Convert.ToDateTime(form["denngay"]))
                                    && (form["nhanvien"] != "0" ? p.StaffId == Convert.ToInt32(form["nhanvien"]) : p.StaffId == p.StaffId))
                                    .OrderByDescending(p => p.CreatedDate)
                                    .Select(p => new ContractReportViewModel()
                                    {
                                        Id = p.Id,
                                        Code = p.Code,
                                        KhachHang = p.CustomerId != null ? p.tbl_Customer.FullName : "",
                                        NgayKy = string.Format("{0:dd-MM-yyyy}", p.ContractDate),
                                        NhanVien = p.StaffId != null ? p.tbl_Staff.FullName : "",
                                        TinhTrang = p.StatusContractId != null ? p.tbl_DictionaryStatus.Name : "",
                                        Tour = p.TourId != null ? p.tbl_Tour.Code + " - " + p.tbl_Tour.Name : "",
                                        LoaiTien = p.tbl_DictionaryCurrency.Name,
                                        LoiNhuan = p.LoiNhuanDuKien,
                                        ChiPhi = p.TongDuKien,
                                        TongGT = p.TotalPrice
                                    }).ToList();
                        filename = "[TRAVELPLUS] Thống kê báo cáo số lượng hợp đồng từ " + Convert.ToDateTime(form["tungay"]).ToString("dd-MM-yyyy") + " đến " + Convert.ToDateTime(form["denngay"]).ToString("dd-MM-yyyy");
                        ExportContractToXlsx(stream, model, filename);
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

        public virtual void ExportContractToXlsx(Stream stream, IList<ContractReportViewModel> contract, string headername)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {

                var worksheet = xlPackage.Workbook.Worksheets.Add("Tours");

                var properties = new[]
                    {
                        "STT",
                        "Mã hợp đồng",
                        "Tour",
                        "Khách hàng",
                        "Tình trạng",
                        "Tổng giá trị (VNĐ)",
                        "Tổng chi phí dự kiến (VNĐ)",
                        "Lợi nhuận dự kiến (VNĐ)",
                        "Ngày ký",
                        "Người lập"
                    };

                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[3, i + 1].Value = properties[i];
                }

                worksheet.Cells["a1:j2"].Value = headername.ToUpper();
                worksheet.Cells["a1:j2"].Style.Font.SetFromFont(new Font("Tahoma", 15));
                worksheet.Cells["a1:j2"].Style.Font.Bold = true;
                worksheet.Cells["a1:j2"].Merge = true;
                worksheet.Cells["a1:j2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["a1:j2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                int row = 3;
                int stt = 1;
                foreach (var t in contract)
                {
                    row++;
                    int col = 1;

                    worksheet.Cells[row, col].Value = stt;
                    col++;

                    worksheet.Cells[row, col].Value = t.Code;
                    col++;

                    worksheet.Cells[row, col].Value = t.Tour;
                    col++;

                    worksheet.Cells[row, col].Value = t.KhachHang;
                    col++;

                    worksheet.Cells[row, col].Value = t.TinhTrang;
                    col++;

                    worksheet.Cells[row, col].Value = t.TongGT;
                    worksheet.Cells[row, col].Style.Numberformat.Format = "#,###,###";
                    col++;

                    worksheet.Cells[row, col].Value = t.ChiPhi;
                    worksheet.Cells[row, col].Style.Numberformat.Format = "#,###,###";
                    col++;

                    worksheet.Cells[row, col].Value = t.LoiNhuan;
                    worksheet.Cells[row, col].Style.Numberformat.Format = "#,###,###";
                    col++;

                    worksheet.Cells[row, col].Value = t.NgayKy;
                    col++;

                    worksheet.Cells[row, col].Value = t.KhachHang;
                    col++;

                    stt++;
                }
                worksheet.Cells["a3:j" + row].Style.Font.SetFromFont(new Font("Tahoma", 8));

                worksheet.Cells["a3:j3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["a3:j3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["a3:j3"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
                worksheet.Cells["a3:j3"].Style.Font.Bold = true;
                worksheet.Cells["a3:j3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Row(3).Height = 20;

                worksheet.Cells["a3:j" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a3:j" + row].Style.Border.Top.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["a3:j" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a3:j" + row].Style.Border.Left.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["a3:j" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a3:j" + row].Style.Border.Bottom.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["a3:j" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a3:j" + row].Style.Border.Right.Color.SetColor(Color.FromArgb(169, 169, 169));

                row++;

                worksheet.Cells["a" + row + ":j" + row].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["a" + row + ":j" + row].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
                // tong gt hop dong
                worksheet.Cells["f" + row].Style.Font.Bold = true;
                worksheet.Cells["f" + row].Style.Numberformat.Format = "#,###,###";
                worksheet.Cells["f" + row].Value = contract.Sum(p => p.TongGT);
                // tong gt thanh toan
                worksheet.Cells["g" + row].Style.Font.Bold = true;
                worksheet.Cells["g" + row].Style.Numberformat.Format = "#,###,###";
                worksheet.Cells["g" + row].Value = contract.Sum(p => p.ChiPhi);
                // tong gt con lai
                worksheet.Cells["h" + row].Style.Font.Bold = true;
                worksheet.Cells["h" + row].Style.Numberformat.Format = "#,###,###";
                worksheet.Cells["h" + row].Value = contract.Sum(p => p.LoiNhuan);
                worksheet.Cells["a3:j" + row].AutoFitColumns();

                xlPackage.Save();
            }
        }

        #endregion
    }
}
