using CRM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOURDEMO.Models
{
    public class InsertSalaryDetail
    {
        public InsertSalaryDetail()
        {
            staffSalaryDetail = new tbl_StaffSalaryDetail();
        }
        public tbl_StaffSalaryDetail staffSalaryDetail { get; set; }
        public string SoTienTang { get; set; }
    }
}