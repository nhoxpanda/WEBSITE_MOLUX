using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOURDEMO.Models
{
    public class CustomerReportViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Tour { get; set; }
        public string NgayKy { get; set; }
        public string TinhTrang { get; set; }
        public string NhanVien { get; set; }
        public string KhachHang { get; set; }
        public double? TongGT { get; set; }
        public double? ChiPhi { get; set; }
        public double? LoiNhuan { get; set; }
        public string LoaiTien { get; set; }
    }

    public class ContractCustomerReportViewModel
    {
        public int Id { get; set; }
        public string Customer { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string ContractCode { get; set; }
        public string ContractDate { get; set; }
        public string TotalPrice { get; set; }
        public double? Price { get; set; }
        public string Staff { get; set; }
        public string CreateDate { get; set; }
    }
}