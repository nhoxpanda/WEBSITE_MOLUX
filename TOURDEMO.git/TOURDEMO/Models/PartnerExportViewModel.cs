using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOURDEMO.Models
{
    public class PartnerExportViewModel
    {
        public int Id { get; set; }
        public string DichVu { get; set; }
        public string Code { get; set; }
        public string TenDoiTac { get; set; }
        public string QuyMo { get; set; }
        public string QuocGia { get; set; }
        public string DiaDiem { get; set; }
        public string NguoiLienHe { get; set; }
        public string DienThoai { get; set; }
        public string Email { get; set; }
        public string NguoiNhap { get; set; }
        public string NgayNhap { get; set; }
    }
}