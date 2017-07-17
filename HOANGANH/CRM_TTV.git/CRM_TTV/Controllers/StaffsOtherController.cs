using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRM_TTV.Models;
using System.Threading.Tasks;
using CRM_TTV.Helper;
using System.IO;
using System.Web.UI;
using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CRM_TTV.Controllers
{

    public class StaffsOtherController : Controller
    {
        // GET: StaffsOther
        private CRM_TTVEntities _db = new CRM_TTVEntities();

        #region Đổi MK

        public ActionResult Password(int? id)
        {
            ViewBag.idUser = id;
            return PartialView();
        }

        public async Task<ActionResult> ChangePassword(ChangePassword model)
        {
            var user = _db.tbUsers.Find(model.Id);
            //user.password = Common.MaHoa(model.Password);
            user.password = model.Password;
            await _db.SaveChangesAsync();
            ViewBag.idUser = model.Id;
            return Json(JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Khóa/Mở khóa TK

        public async Task<ActionResult> Lock(int i, int id)
        {
            var model = _db.tbUsers.Find(id);
            if (i == 1)
            {
                model.locked = true;
                model.dateLock = DateTime.Now;
            }
            else
            {
                model.locked = false;
                model.dateUnlock = DateTime.Now;
            }
            await _db.SaveChangesAsync();
            return Json(JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Import

        public ActionResult Import()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult ImportFile(HttpPostedFileBase FileName)
        {
            try
            {

                using (var excelPackage = new ExcelPackage(FileName.InputStream))
                {
                    List<tbUser> list = new List<tbUser>();
                    var worksheet = excelPackage.Workbook.Worksheets[1];
                    var lastRow = worksheet.Dimension.End.Row;
                    for (int row = 7; row <= lastRow; row++)
                    {
                        if (worksheet.Cells["C" + row].Value == null || worksheet.Cells["C" + row].Text == "")
                            continue;
                        var cus = new tbUser
                        {
                            code = worksheet.Cells["B" + row].Value != null ? worksheet.Cells["B" + row].Text : "",
                            fullName = worksheet.Cells["C" + row].Value != null ? worksheet.Cells["C" + row].Text : "",
                            passport = worksheet.Cells["E" + row].Value != null ? worksheet.Cells["E" + row].Text : "",
                            locationIssued = worksheet.Cells["G" + row].Value != null ? worksheet.Cells["G" + row].Text : "",
                            homeTown = worksheet.Cells["H" + row].Value != null ? worksheet.Cells["H" + row].Text : "",
                            resident = worksheet.Cells["I" + row].Value != null ? worksheet.Cells["I" + row].Text : "",
                            address = worksheet.Cells["J" + row].Value != null ? worksheet.Cells["J" + row].Text : "",
                            skype = worksheet.Cells["L" + row].Value != null ? worksheet.Cells["L" + row].Text : "",
                            facebook = worksheet.Cells["M" + row].Value != null ? worksheet.Cells["M" + row].Text : "",
                            phone = worksheet.Cells["N" + row].Value != null ? worksheet.Cells["N" + row].Text : "",
                            phone2 = worksheet.Cells["O" + row].Value != null ? worksheet.Cells["O" + row].Text : "",
                            email = worksheet.Cells["P" + row].Value != null ? worksheet.Cells["P" + row].Text : "",
                            codeBHXH = worksheet.Cells["X" + row].Value != null ? worksheet.Cells["X" + row].Text : "",
                            taxCode = worksheet.Cells["Y" + row].Value != null ? worksheet.Cells["Y" + row].Text : "",
                            locationRegisterTaxCode = worksheet.Cells["AA" + row].Value != null ? worksheet.Cells["AA" + row].Text : "",
                            banking = (worksheet.Cells["AB" + row].Value != null ? worksheet.Cells["AB" + row].Text : "") + " " + (worksheet.Cells["AC" + row].Value != null ? worksheet.Cells["AC" + row].Text : ""),
                            experience = worksheet.Cells["AE" + row].Value != null ? worksheet.Cells["AE" + row].Text : "",
                        };

                        // sinh nhật
                        if (worksheet.Cells["D" + row].Value != null && worksheet.Cells["D" + row].Text != "")
                        {
                            cus.birthDay = Convert.ToDateTime(worksheet.Cells["D" + row].Value);
                        }

                        // ngày hiệu lực CMND/Passport
                        if (worksheet.Cells["F" + row].Value != null && worksheet.Cells["F" + row].Text != "")
                        {
                            cus.dateIssued = Convert.ToDateTime(worksheet.Cells["F" + row].Value);
                        }

                        // ngày làm việc 
                        if (worksheet.Cells["AF" + row].Value != null && worksheet.Cells["AF" + row].Text != "")
                        {
                            cus.dateToWork = Convert.ToDateTime(worksheet.Cells["AF" + row].Value);
                        }

                        // ngày nghỉ việc
                        if (worksheet.Cells["AG" + row].Value != null && worksheet.Cells["AG" + row].Text != "")
                        {
                            cus.dateUnlock = Convert.ToDateTime(worksheet.Cells["AG" + row].Value);
                        }

                        // ngày bắt đầu HĐLĐ
                        if (worksheet.Cells["V" + row].Value != null && worksheet.Cells["V" + row].Text != "")
                        {
                            cus.HDLDStartDate = Convert.ToDateTime(worksheet.Cells["V" + row].Value);
                        }


                        // ngày kết thúc HĐLĐ
                        if (worksheet.Cells["W" + row].Value != null && worksheet.Cells["W" + row].Text != "")
                        {
                            cus.HDLDEndDate = Convert.ToDateTime(worksheet.Cells["W" + row].Value);
                        }

                        // ngày đăng ký MST
                        if (worksheet.Cells["Z" + row].Value != null && worksheet.Cells["Z" + row].Text != "")
                        {
                            cus.DateRegisterTaxCode = Convert.ToDateTime(worksheet.Cells["Z" + row].Value);
                        }

                        // quốc tịch
                        string cel = "";
                        try
                        {
                            cel = "K";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string quoctich = worksheet.Cells[cel + row].Text;
                                cus.citizenship = _db.tbRegions.FirstOrDefault(c => c.name == quoctich && c.type == 2095).idRegion;
                            }
                        }
                        catch { }

                        // ngành nghề - hình thức làm việc
                        try
                        {
                            cel = "Q";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string hinhthuc = worksheet.Cells[cel + row].Text;
                                cus.idJob = _db.tbCategories.FirstOrDefault(p => p.name == hinhthuc && p.idCategoryType == 17).idCategory;
                            }
                        }
                        catch { }

                        // trạng thái làm việc
                        try
                        {
                            cel = "R";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string trangthai = worksheet.Cells[cel + row].Text;
                                cus.idJob = _db.tbCategories.FirstOrDefault(p => p.name == trangthai && p.idCategoryType == 14).idCategory;
                            }
                        }
                        catch { }

                        // chi nhánh
                        try
                        {
                            cel = "R";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string chinhanh = worksheet.Cells[cel + row].Text;
                                cus.idCompany = _db.tbCompanies.FirstOrDefault(p => p.name == chinhanh).idCompany;
                            }
                        }
                        catch { }

                        // phòng ban
                        try
                        {
                            cel = "T";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string phongban = worksheet.Cells[cel + row].Text;
                                cus.department = _db.tbRoles.FirstOrDefault(p => p.name == phongban && p.idRoleType == 2).roleID;
                            }
                        }
                        catch { }

                        // nhóm bộ phận
                        try
                        {
                            cel = "U";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string bophan = worksheet.Cells[cel + row].Text;
                                cus.groupMemb = _db.tbRoles.FirstOrDefault(p => p.name == bophan && p.idRoleType == 3).roleID;
                            }
                        }
                        catch { }

                        // tình trạng sức khỏe
                        try
                        {
                            cel = "AD";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string suckhoe = worksheet.Cells[cel + row].Text;
                                cus.healthStatus = _db.tbCategories.FirstOrDefault(c => c.name == suckhoe && c.idCategoryType == 13).idCategory;
                            }
                        }
                        catch { }
                        list.Add(cus);
                    }
                    Session["listStaffImport"] = list;
                    return PartialView("ImportDataList", list);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveImport()
        {
            try
            {
                List<tbUser> list = Session["listStaffImport"] as List<tbUser>;
                int i = 0;

                foreach (var item in list)
                {
                    _db.tbUsers.Add(item);
                    i++;
                }
                await _db.SaveChangesAsync();
                Session["listStaffImport"] = null;
                return Json(i, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                Session["listStaffImport"] = null;
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteImport(String listItemId)
        {
            try
            {
                List<tbUser> list = Session["listStaffImport"] as List<tbUser>;
                if (listItemId != null && listItemId != "")
                {
                    var listIds = listItemId.Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        int[] listIdsint = new int[listIds.Length];
                        for (int i = 0; i < listIds.Length; i++)
                        {
                            listIdsint[i] = Int32.Parse(listIds[i]);
                        }
                        for (int i = 0; i < listIdsint.Length; i++)
                        {
                            for (int j = i; j < listIdsint.Length; j++)
                            {
                                if (listIdsint[i] < listIdsint[j])
                                {
                                    int temp = listIdsint[i];
                                    listIdsint[i] = listIdsint[j];
                                    listIdsint[j] = temp;
                                }
                            }
                        }
                        foreach (var item in listIdsint)
                        {
                            list.RemoveAt(item);
                        }
                    }
                }
                Session["listStaffImport"] = list;
                return PartialView("ImportDataList", list);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Excel Sample

        public void ExportExcelTemplateStaff(MemoryStream stream, string templateFile, IDictionary<string, string> header = null)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream, new MemoryStream(System.IO.File.ReadAllBytes(templateFile))))
            {
                var ws = xlPackage.Workbook.Worksheets[1]; //first worksheet
                var valWs = xlPackage.Workbook.Worksheets.Add("Validation");
                valWs.Hidden = eWorkSheetHidden.VeryHidden;

                //ws.AddHeader(header);

                // quốc tịch
                var quoctich = _db.tbRegions.AsEnumerable().Where(p => p.type == 2095)
                    .Select(p => new ExportItem
                    {
                        Text = p.name,
                        Value = p.idRegion
                    });
                var columnIndex = ws.GetColumnIndex(EnumStaff.QUOCTICH.ToString());
                ws.AddListValidation(valWs, quoctich, columnIndex, "Lỗi", "Lỗi", "QUOCTICH", "QUOCTICHNAME");

                // hình thức nhân viên
                var hinhthuc = _db.tbCategories.AsEnumerable().Where(p => p.idCategoryType == 17)
                    .Select(p => new ExportItem
                    {
                        Text = p.name,
                        Value = p.idCategory
                    });
                columnIndex = ws.GetColumnIndex(EnumStaff.HINHTHUCNV.ToString());
                ws.AddListValidation(valWs, hinhthuc, columnIndex, "Lỗi", "Lỗi", "HINHTHUCNV", "HINHTHUCNVNAME");

                // trạng thái làm việc
                var trangthai = _db.tbCategories.AsEnumerable().Where(p => p.idCategoryType == 14)
                    .Select(p => new ExportItem
                    {
                        Text = p.name,
                        Value = p.idCategory
                    });
                columnIndex = ws.GetColumnIndex(EnumStaff.TRANGTHAILAMVIEC.ToString());
                ws.AddListValidation(valWs, trangthai, columnIndex, "Lỗi", "Lỗi", "TRANGTHAILAMVIEC", "TRANGTHAILAMVIECNAME");

                // chi nhánh
                var chinhanh = _db.tbCompanies.AsEnumerable()
                    .Select(p => new ExportItem
                    {
                        Text = p.name,
                        Value = p.idCompany
                    });
                columnIndex = ws.GetColumnIndex(EnumStaff.CHINHANH.ToString());
                ws.AddListValidation(valWs, chinhanh, columnIndex, "Lỗi", "Lỗi", "CHINHANH", "CHINHANHNAME");

                // phòng ban
                var phongban = _db.tbRoles.AsEnumerable().Where(p => p.idRoleType == 2)
                    .Select(p => new ExportItem
                    {
                        Text = p.name,
                        Value = p.roleID
                    });
                columnIndex = ws.GetColumnIndex(EnumStaff.PHONGBAN.ToString());
                ws.AddListValidation(valWs, phongban, columnIndex, "Lỗi", "Lỗi", "PHONGBAN", "PHONGBANNAME");

                // bộ phận
                var bophan = _db.tbRoles.AsEnumerable().Where(p => p.idRoleType == 3)
                    .Select(p => new ExportItem
                    {
                        Text = p.name,
                        Value = p.roleID
                    });
                columnIndex = ws.GetColumnIndex(EnumStaff.BOPHAN.ToString());
                ws.AddListValidation(valWs, bophan, columnIndex, "Lỗi", "Lỗi", "BOPHAN", "BOPHANNAME");

                // tình trạng sức khỏe
                var suckhoe = _db.tbCategories.AsEnumerable().Where(p => p.idCategoryType == 13)
                    .Select(p => new ExportItem
                    {
                        Text = p.name,
                        Value = p.idCategory
                    });
                columnIndex = ws.GetColumnIndex(EnumStaff.SUCKHOE.ToString());
                ws.AddListValidation(valWs, suckhoe, columnIndex, "Lỗi", "Lỗi", "SUCKHOE", "SUCKHOENAME");


                xlPackage.Save();
            }
        }

        public ActionResult ExcelSample()
        {
            try
            {
                IDictionary<string, string> header = new Dictionary<string, string>();
                header.Add("TITLE", "DANH SÁCH NHÂN VIÊN");

                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    string templateFile = System.Web.HttpContext.Current.Server.MapPath("~\\Upload\\ExcelSample\\ImportStaffs.xlsx");
                    ExportExcelTemplateStaff(stream, templateFile, header);
                    bytes = stream.ToArray();
                }

                string fileName = "[TRÁI TIM VÀNG] Import Danh Sách Nhân Viên.xlsx";
                return File(bytes, "text/xls", fileName);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return RedirectToAction("Index", "Staffs");
            }
        }
        #endregion

        #region Export
        /// <summary>
        /// Export file excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportFile()
        {
            var tbUsers = _db.tbUsers.AsEnumerable().OrderByDescending(x => x.userID)
                .Select(p => new StaffExport()
                {
                    BOPHAN = p.tbRole2 != null ? p.tbRole2.name : "",
                    CHINHANH = p.tbCompany != null ? p.tbCompany.name : "",
                    CODE = p.code,
                    PHONGBAN = p.tbRole != null ? p.tbRole.name : "",
                    DIENTHOAI1 = p.phone,
                    DIENTHOAI2 = p.phone2,
                    FACEBOOK = p.facebook,
                    HDLDDENNGAY = string.Format("{0:dd-MM-yyyy}", p.HDLDStartDate),
                    HDLDTUNGAY = string.Format("{0:dd-MM-yyyy}", p.HDLDEndDate),
                    HINHTHUCNV = p.tbCategory1 != null ? p.tbCategory1.name : "",
                    HOTEN = p.fullName,
                    KINHNGHIEM = p.experience,
                    MST = p.taxCode,
                    NGAYDK = string.Format("{0:dd-MM-yyyy}", p.DateRegisterTaxCode),
                    NOIDK = p.locationRegisterTaxCode,
                    TAIKHOAN = p.banking,
                    NGANHANG = p.banking,
                    EMAIL = p.email,
                    CMND = p.passport,
                    NGAYCAP = string.Format("{0:dd-MM-yyyy}", p.dateIssued),
                    NOICAP = p.locationIssued,
                    THUONGTRU = p.resident,
                    NOIOHIENTAI = p.address,
                    NGAYVAOLAM = string.Format("{0:dd-MM-yyyy}", p.dateToWork),
                    NGAYKETTHUC = string.Format("{0:dd-MM-yyyy}", p.dateTrialEnds),
                    NGAYSINH = string.Format("{0:dd-MM-yyyy}", p.birthDay),
                    QUEQUAN = p.homeTown,
                    QUOCTICH = p.tbCategory3 != null ? p.tbCategory3.name : "",
                    SKYPE = p.skype,
                    SOBHXH = p.codeBHXH,
                    TRANGTHAILAMVIEC = p.tbCategory != null ? p.tbCategory.name : "",
                    SUCKHOE = p.tbCategory2 != null ? p.tbCategory2.name : ""
                }).ToList();
            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    ExporStaffToXlsx(stream, tbUsers);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "[TRÁI TIM VÀNG] Danh sách nhân viên (" + String.Format("{0:dd/MM/yyyy}", DateTime.Now) + ").xlsx");
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            return RedirectToAction("Index", "Staffs");
        }

        public virtual void ExporStaffToXlsx(Stream stream, IList<StaffExport> staffs)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.Add("DS Nhân viên");
                var properties = new[]
                    {
                        "STT",
                        "MÃ NHÂN VIÊN",
                        "HỌ TÊN",
                        "NGÀY SINH",
                        "CMND/PASSPORT",
                        "NGÀY CẤP",
                        "NƠI CẤP",
                        "QUÊ QUÁN",
                        "THƯỜNG TRÚ",
                        "NƠI Ở HIỆN TẠI",
                        "QUỐC TỊCH",
                        "SKYPE",
                        "FACEBOOK",
                        "ĐIỆN THOẠI 1",
                        "ĐIỆN THOẠI 2",
                        "EMAIL",
                        "HÌNH THỨC NV",
                        "TRẠNG THÁI LÀM VIỆC",
                        "CHI NHÁNH",
                        "PHÒNG BAN",
                        "BỘ PHẬN",
                        "HĐLĐ TỪ NGÀY",
                        "HĐLĐ ĐẾN NGÀY",
                        "SỔ BHXH",
                        "MÃ SỐ THUẾ",
                        "NGÀY ĐĂNG KÝ",
                        "NƠI ĐĂNG KÝ",
                        "TÀI KHOẢN",
                        "NGÂN HÀNG",
                        "TÌNH TRẠNG SỨC KHỎE",
                        "KINH NGHIỆM",
                        "NGÀY VÀO LÀM",
                        "NGÀY NGHỈ VIỆC"
                    };

                //logo
                worksheet.Cells["A1:C4"].Merge = true;

                worksheet.Cells["D2:M3"].Value = "DANH SÁCH NHÂN VIÊN";
                worksheet.Cells["D2:M3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                worksheet.Cells["D2:M3"].Style.Font.Bold = true;
                worksheet.Cells["D2:M3"].Merge = true;
                worksheet.Cells["D2:M3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["D2:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["D2:M3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["D2:M3"].Style.Fill.BackgroundColor.SetColor(Color.White);

                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[5, i + 1].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells[5, i + 1].Value = properties[i];
                    worksheet.Cells[5, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[5, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[5, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[5, i + 1].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                    worksheet.Cells[5, i + 1].AutoFitColumns();
                    worksheet.Cells[5, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[5, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                int row = 6;
                foreach (var staff in staffs)
                {
                    int col = 1;

                    worksheet.Cells[row, col].Value = row - 5;
                    col++;
                    worksheet.Cells[row, col].Value = staff.CODE;
                    col++;
                    worksheet.Cells[row, col].Value = staff.HOTEN;
                    col++;
                    worksheet.Cells[row, col].Value = staff.NGAYSINH;
                    col++;
                    worksheet.Cells[row, col].Value = staff.CMND;
                    col++;
                    worksheet.Cells[row, col].Value = staff.NGAYCAP;
                    col++;
                    worksheet.Cells[row, col].Value = staff.NOICAP;
                    col++;
                    worksheet.Cells[row, col].Value = staff.QUEQUAN;
                    col++;
                    worksheet.Cells[row, col].Value = staff.THUONGTRU;
                    col++;
                    worksheet.Cells[row, col].Value = staff.NOIOHIENTAI;
                    col++;
                    worksheet.Cells[row, col].Value = staff.QUOCTICH;
                    col++;
                    worksheet.Cells[row, col].Value = staff.SKYPE;
                    col++;
                    worksheet.Cells[row, col].Value = staff.FACEBOOK;
                    col++;
                    worksheet.Cells[row, col].Value = staff.DIENTHOAI1;
                    col++;
                    worksheet.Cells[row, col].Value = staff.DIENTHOAI2;
                    col++;
                    worksheet.Cells[row, col].Value = staff.EMAIL;
                    col++;
                    worksheet.Cells[row, col].Value = staff.HINHTHUCNV;
                    col++;
                    worksheet.Cells[row, col].Value = staff.TRANGTHAILAMVIEC;
                    col++;
                    worksheet.Cells[row, col].Value = staff.CHINHANH;
                    col++;
                    worksheet.Cells[row, col].Value = staff.PHONGBAN;
                    col++;
                    worksheet.Cells[row, col].Value = staff.BOPHAN;
                    col++;
                    worksheet.Cells[row, col].Value = staff.HDLDTUNGAY;
                    col++;
                    worksheet.Cells[row, col].Value = staff.HDLDDENNGAY;
                    col++;
                    worksheet.Cells[row, col].Value = staff.SOBHXH;
                    col++;
                    worksheet.Cells[row, col].Value = staff.MST;
                    col++;
                    worksheet.Cells[row, col].Value = staff.NGAYDK;
                    col++;
                    worksheet.Cells[row, col].Value = staff.NOIDK;
                    col++;
                    worksheet.Cells[row, col].Value = staff.TAIKHOAN;
                    col++;
                    worksheet.Cells[row, col].Value = staff.NGANHANG;
                    col++;
                    worksheet.Cells[row, col].Value = staff.SUCKHOE;
                    col++;
                    worksheet.Cells[row, col].Value = staff.KINHNGHIEM;
                    col++;
                    worksheet.Cells[row, col].Value = staff.NGAYVAOLAM;
                    col++;
                    worksheet.Cells[row, col].Value = staff.NGAYKETTHUC;
                    col++;
                    row++;
                }
                row--;
                worksheet.Cells["A6:AG1000"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                xlPackage.Save();
            }
        }
        #endregion
    }
}