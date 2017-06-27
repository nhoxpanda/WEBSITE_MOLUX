using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Core;

namespace TOURDEMO.Models
{
    public class TourGuideViewModel
    {
        public tbl_Staff SingleStaff { get; set; }
        public tbl_TourGuide SingleGuide { get; set; }
    }

    public class GuideListViewModel
    {
        public int StaffId { get; set; }
        public int GuideId { get; set; }
        public string Name { get; set; }
        public string Birthday { get; set; }
        public string CodeGuide { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Image { get; set; }
        public List<tbl_DocumentFile> File { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
