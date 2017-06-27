using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOURDEMO.Models;
using CRM.Core;

namespace TOURDEMO.Models
{
    public class PartnerViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string Tags { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public string xMap { get; set; }
        public string yMap { get; set; }
        public string AddressMap { get; set; }
        public string QuyMo { get; set; }
        public string KhuVuc { get; set; }
    }

    public class PartnerListViewModel
    {
        public tbl_Partner SinglePartner { get; set; }
        public List<DocumentFileViewModel> ListDocument { get; set; }
        public List<tbl_PartnerNote> ListPartnerNote { get; set; }
    }

    public class PartnerExportNew
    {
        public string CODE { get; set; }
        public string DICHVU { get; set; }
        public string TENDOITAC { get; set; }
        public string QUYMO { get; set; }
        public string QUOCGIA { get; set; }
        public string DIACHI { get; set; }
        public string TINHTP { get; set; }
        public string QUANHUYEN { get; set; }
        public string PHUONGXA { get; set; }
        public string NGUOILIENHE { get; set; }
        public string DIENTHOAI { get; set; }
        public string EMAIL { get; set; }
        public string NGUOINHAP { get; set; }
        public string NGAYNHAP { get; set; }
    }
}