using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOURDEMO.Models
{
    public class HistoryLogViewModel
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public string Staff { get; set; }
        public int StaffId { get; set; }
        public string Form { get; set; }
        public string Module { get; set; }
    }
}