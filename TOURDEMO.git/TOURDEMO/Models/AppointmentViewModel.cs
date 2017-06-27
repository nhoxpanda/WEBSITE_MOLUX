using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOURDEMO.Models
{
    public class AppointmentViewModel
    {
        public List<SingleAppoinment> TodayList { get; set; }
        public List<SingleAppoinment> TomorrowList { get; set; }
    }

    public class SingleAppoinment
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Time { get; set; }
    }

    public class TaskViewModel
    {
        public List<SingleTask> TodayList { get; set; }
        public List<SingleTask> TomorrowList { get; set; }
    }

    public class SingleTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Time { get; set; }
    }

}