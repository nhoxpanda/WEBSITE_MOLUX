using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Core;

namespace TOURDEMO.Models
{
    public class CustomerListViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Fullname { get; set; }
        public string Birthday { get; set; }
        public string Company { get; set; }
        public string Phone { get; set; }
        public string OtherPhone { get; set; }
        public string Address { get; set; }
        public string TagsId { get; set; }
        public string Career { get; set; }
        public string Passport { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Email { get; set; }
        public string Skype { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string IdentityCard { get; set; }
        public string NameCustomerContract { get; set; }
        public string PhoneCustomerContract { get; set; }
        public string Note { get; set; }
        public CRM.Enum.CustomerType? CustomerType { get; set; }
        public string Type { get; set; }
        public string TaxCode { get; set; }
        public int? ParentId { get; set; }
        public string DanhXung { get; set; }
        public string NguonKhach { get; set; }
        public string NhomKH { get; set; }
        public string Staff { get; set; }
        public string CreateDate { get; set; }
        public string QuanLy { get; set; }
        public string CMND { get; set; }
        public string NgayCap { get; set; }
        public string NoiCap { get; set; }
        public int Point { get; set; }
        public string MemberCard { get; set; }
    }

    public class ExportCustomerViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Fullname { get; set; }
        public string Birthday { get; set; }
        public string Company { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string TagsId { get; set; }
        public string Career { get; set; }
        public string Origin { get; set; }
        public string Passport { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Email { get; set; }
        public string Skype { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string IdentityCard { get; set; }
        public string NameCustomerContract { get; set; }
        public string PhoneCustomerContract { get; set; }
        public string Note { get; set; }
        public CRM.Enum.CustomerType? CustomerType { get; set; }
        public int Voticate { get; set; }
        public string TaxCode { get; set; }
        public int? ParentId { get; set; }
        public string DanhXung { get; set; }
        public int TinhTP { get; set; }
        public int QuanHuyen { get; set; }
        public int PhuongXa { get; set; }
    }

    public class CustomerExportNew
    {
        public string CODE { get; set; }
        public string DANHXUNG { get; set; }
        public string HOTEN { get; set; }
        public string LOAIKHACH { get; set; }
        public string NGAYSINH { get; set; }
        public string DIENTHOAI { get; set; }
        public string DIDONG { get; set; }
        public string DIACHI { get; set; }
        public string TINHTP { get; set; }
        public string QUANHUYEN { get; set; }
        public string PHUONGXA { get; set; }
        public string NGANHNGHE { get; set; }
        public string NGUONDEN { get; set; }
        public string NHOMKHACH { get; set; }
        public string PASSPORT { get; set; }
        public string NGAYHIEULUC { get; set; }
        public string NGAYHETHAN { get; set; }
        public string EMAIL { get; set; }
        public string GIAMDOC { get; set; }
        public string FAX { get; set; }
        public string MST { get; set; }
        public string GHICHU { get; set; }
        public string NOICAP { get; set; }
        public string NOICAPPASSPORT { get; set; }
        public string QUOCGIAVISA { get; set; }
        public string TRANGTHAIVISA { get; set; }
        public string LOAIVISA { get; set; }
        public string NGUOINHAP { get; set; }
        public string NGAYNHAP { get; set; }
    }

    public class ExportToOffline
    {
        public string MAKH { get; set; }
        public string QUYDANH { get; set; }
        public string TENKH { get; set; }
        public string CANHAN { get; set; }
        public string DIDONG { get; set; }
        public string DIENTHOAI { get; set; }
        public string FAX { get; set; }
        public string SOCMND { get; set; }
        public string MASOTHUE { get; set; }
        public string NGAYCAP { get; set; }
        public string NOICAP { get; set; }
        public string EMAIL { get; set; }
        public string WEBSITE { get; set; }
        public string NGAYSINH { get; set; }
        public string SOGPKD { get; set; }
        public string NGAYTLDN { get; set; }
        public string VONDIEULE { get; set; }
        public string NOTOIDA { get; set; }
        public string HANNO { get; set; }
        public string SOTKNH { get; set; }
        public string NGANHANG { get; set; }
        public string NGANHNGHE { get; set; }
        public string DIACHI { get; set; }
        public string XAPHUONG { get; set; }
        public string QUANHUYEN { get; set; }
        public string TINHTHANH { get; set; }
        public string GHICHU { get; set; }
        public string NGUONDEN { get; set; }
        public string NHOMKH { get; set; }
        public string LOAIKH { get; set; }
        public string QUYDANHNLH { get; set; }
        public string NGUOILIENHE { get; set; }
        public string CHUCVUNLH { get; set; }
        public string DIDONGNLH { get; set; }
        public string EMAILNLH { get; set; }
        public string DIACHINLH { get; set; }
    }

    public class CustomerViewModel
    {
        public tbl_Customer SinglePersonal { get; set; }
        public tbl_Customer SingleCompany { get; set; }
        public tbl_CustomerContact SingleContact { get; set; }
        public string IdentityCard { get; set; }
        public DateTime CreatedDateIdentity { get; set; }
        public int IdentityTagId { get; set; }
        public string PassportCard { get; set; }
        public DateTime CreatedDatePassport { get; set; }
        public DateTime ExpiredDatePassport { get; set; }
        public int PassportTagId { get; set; }
        public SingleVisa SingleVisa { get; set; }
        public List<tbl_CustomerVisa> ListCustomerVisa { get; set; }
        public bool IsTemp { get; set; }
        // other company
        public string OtherCompanyName { get; set; }
        public string OtherCompanyAddress { get; set; }
        public string OtherCompanyEmail { get; set; }
        public string OtherCompanyPhone { get; set; }
        public string OtherCompanyDirector { get; set; }
        public int OtherCompanyCareerId { get; set; }
        public string OtherCompanyTagsId { get; set; }
    }

    public class SingleVisa
    {
        public string VisaNumber { get; set; }
        public int TagsId { get; set; }
        public DateTime CreatedDateVisa { get; set; }
        public DateTime ExpiredDateVisa { get; set; }
    }

    public class InfoCustomerViewModel
    {
        public tbl_Customer SingleCustomer { get; set; }
        public List<tbl_CustomerVisa> ListCustomerVisa { get; set; }
        public List<DocumentFileViewModel> ListCustomerFile { get; set; }
        public List<DocumentFileViewModel> ListCustomerDocument { get; set; }
        public List<tbl_AppointmentHistory> ListCustomerAppointment { get; set; }
        public List<tbl_ContactHistory> ListCustomerContactHistory { get; set; }
        public List<tbl_UpdateHistory> ListCustomerUpdateHistory { get; set; }
    }

}