using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOURDEMO.Models
{
    public class TourScheduleViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int StaffId { get; set; }
        public string Staff { get; set; }
        public int GuideId { get; set; }
        public string Guide { get; set; }
    }
}