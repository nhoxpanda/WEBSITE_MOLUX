using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOURDEMO.Models
{
    public class TourReportViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public string NgayKhoiHanh { get; set; }
        public string NgayKetThuc { get; set; }
        public string NguoiDieuHanh { get; set; }
        public string ChiTietChuyenBay { get; set; }
        public string HuongDanVien { get; set; }
        public int SLKhach { get; set; }
        public string NhanVien { get; set; }
        public string KhachHang { get; set; }
        public string LoaiTour { get; set; }
        public double? TongGiaTriHD { get; set; }
        public string LoaiTien { get; set; }
    }

    public class LiabilitiesCustomerReportViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Customer { get; set; }
        public string NgayKhoiHanh { get; set; }
        public string NgayKetThuc { get; set; }
        public decimal TongGTHopDong { get; set; }
        public decimal TongGTThanhLy { get; set; }
        public decimal TongGTThanhToan { get; set; }
        public decimal TongGTConLai { get; set; }
        public string NguoiDieuHanh { get; set; }
        public string Currency { get; set; }
    }

    public class LiabilitiesPartnerReportViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Partner { get; set; }
        public string Service { get; set; }
        public string NgayKhoiHanh { get; set; }
        public decimal? TongGTDichVu { get; set; }
        public decimal? TongGTThanhToan { get; set; }
        public decimal? TongGTConLai { get; set; }
        public string NguoiDieuHanh { get; set; }
        public string Currency { get; set; }
    }

    public class ContractReportViewModel
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
}