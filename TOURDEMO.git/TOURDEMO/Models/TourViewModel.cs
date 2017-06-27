using CRM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOURDEMO.Models
{
    public class TourListViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CustomerName { get; set; }
        public string DestinationPlace { get; set; }
        public int NumberDay { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public int NumberCustomer { get; set; }
        public string TourGuide { get; set; }
        public string TourType { get; set; }
        public decimal CongNoKhachHang { get; set; }
        public decimal CongNoDoiTac { get; set; }
        public string Status { get; set; }
        public string Color { get; set; }
        public string Staff { get; set; }
        public string Manager { get; set; }
        public string Permission { get; set; }
        public tbl_Tour SingleTour { get; set; }
        public string Currency { get; set; }
    }

    public class TourViewModel
    {
        public tbl_Tour SingleTour { get; set; }
        public tbl_TourGuide SingleTourGuide { get; set; }
        public Nullable<DateTime> StartDateTour { get; set; }
        public Nullable<DateTime> EndDateTour { get; set; }
        public Nullable<DateTime> StartDateTourGuide { get; set; }
        public Nullable<DateTime> EndDateTourGuide { get; set; }
        public string PriceString { get; set; }
    }

    public class TourInfoViewModel
    {
        public int CustomerId { get; set; }
        public string TagId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int CurrencyId { get; set; }
        public int NumberDay { get; set; }
        public float Price { get; set; }
    }

    public class TourUpdateViewModel
    {
        public List<tbl_TourOption> listOption { get; set; }
        public tbl_ServicesPartner servicePartner { get; set; }
        public tbl_Partner partner { get; set; }
    }

    public class TourVisaViewModel
    {
        public tbl_CustomerVisa Visa { get; set; }
        public tbl_TourCustomerVisa TourCustomer { get; set; }
        public tbl_TourCustomerVisa TourVisa { get; set; }
    }

    public class ContractTourViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime? ContractDate { get; set; }
        public string Permission { get; set; }
        public DateTime? StartDate { get; set; }
        public int NumberDay { get; set; }
        public int? StatusContractId { get; set; }
        public tbl_Dictionary tbl_DictionaryStatus { get; set; }
        public int DocumentId { get; set; }
        public string FileName { get; set; }
        public tbl_Staff tbl_Staff { get; set; }
        public DateTime? CreatedDate { get; set; }
        public double? TotalPrice { get; set; }
        public double? TongDuKien { get; set; }
        public double? LoiNhuanDuKien { get; set; }
        public int PartnerId { get; set; }
        public tbl_Dictionary tbl_DictionaryCurrencyTDK { get; set; }
        public tbl_Dictionary tbl_DictionaryCurrency { get; set; }
        public tbl_Dictionary tbl_DictionaryCurrencyLNDK { get; set; }
    }

}