using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOURDEMO.Models
{
    public class TourServiceViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int ServiceId { get; set; }
        public int PartnerId { get; set; }
        public string Currency { get; set; }
        public int ServicePartnerId { get; set; }
        public string ServiceName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string StaffContact { get; set; }
        public string Phone { get; set; }
        public double? Price { get; set; }
        public string Note { get; set; }
        public Nullable<DateTime> Deadline { get; set; }
        public int TourId { get; set; }
        public int TourOptionId { get; set; }
    }
}
