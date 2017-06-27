using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOURDEMO.Models
{
    public class VisaListViewModel
    {
        public int Id { get; set; }
        public Boolean StaffCustomer { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Country { get; set; }
        public int CountryId { get; set; }
        public string StartDate { get; set; }
        public DateTime CreateDatePP { get; set; }
        public string EndDate { get; set; }
        public DateTime ExpiredDatePP { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public string Type { get; set; }
        public int TypeId { get; set; }
        public DateTime? CreateDate { get; set; }
        public int IsCustomer { get; set; }

        public string RefNumber { get; set; }
        public string ReceiptDate { get; set; }
        public int DeadlineCollect { get; set; }
        public int DeadlineSubmission { get; set; }

    }

    public class VisaExportNew
    {
        public string KHACHHANG { get; set; }
        public string HOTEN { get; set; }
        public string VISA { get; set; }
        public string QUOCGIA { get; set; }
        public string LOAIVISA { get; set; }
        public string TINHTRANG { get; set; }
        public string NGAYHIEULUC { get; set; }
        public string NGAYHETHAN { get; set; }
        public string NGUOINHAP { get; set; }
        public string NGAYNHAP { get; set; }
    }
}