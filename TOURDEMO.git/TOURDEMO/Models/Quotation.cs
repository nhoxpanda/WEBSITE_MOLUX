using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOURDEMO.Models
{
    public class QuotationViewModel
    {
        public string Code { get; set; }
        public int? CountryId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CurrencyId { get; set; }
       
        public int? CustomerId { get; set; }
       
        public int DictionaryId { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public int? ExpiredDay { get; set; }
        public string FileName { get; set; }
        
        public int Id { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Note { get; set; }
        public int? NumberDay { get; set; }
        public string Permission { get; set; }
        public double? PriceTour { get; set; }
        public DateTime? QuotationDate { get; set; }
        public int? StaffId { get; set; }
        public int? StaffQuotationId { get; set; }
        public DateTime? StartDate { get; set; }
        public string TagsId { get; set; }

    }
}