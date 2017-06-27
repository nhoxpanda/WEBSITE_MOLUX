using CRM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOURDEMO.Models
{
    public class StaffSalaryViewModel
    {
        public StaffSalaryViewModel()
        {
            staffSalary = new tbl_StaffSalary();
            listStaffSalaryDetail = new List<tbl_StaffSalaryDetail>();
        }
        public tbl_StaffSalary staffSalary { get; set; }
        public IList<tbl_StaffSalaryDetail> listStaffSalaryDetail { get; set; }
    }
}