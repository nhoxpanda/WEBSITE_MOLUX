using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOURDEMO.Models
{
    public class TimeWorkingDayOff
    {
        public TimeWorkingDayOff()
        {

        }
        public TimeWorkingDayOff(int _year)//, bool isLocked)
        {
            //if (isLocked)
            //{
            //    DayOffPerYear = 0;
            //}
            //else
            //{
            if (_year >= 3)
            {
                DayOffPerYear = 13;
            }
            else
            {
                DayOffPerYear = 12;
            }
            //}

        }
        
        public decimal DayOffPerYear { get; set; }
        public int Years { get; set; }
        public int Months { get; set; }
        public int Days { get; set; }
    }
}