using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOURDEMO.Models
{
    public class AirlineViewModel
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string ContactName { get; set; }
        public DateTime CreateDate { get; set; }
        public string Email { get; set; }
        public bool IsDelete { get; set; }
        public string Logo { get; set; }
        public string Mobile { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public string ShortName{ get; set; }
        public int StaffId { get; set; }
    }
}