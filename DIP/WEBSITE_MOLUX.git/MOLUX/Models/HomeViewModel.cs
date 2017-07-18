using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MOLUX.Models
{
    public class HomeViewModel
    {
        public List<web_getParentCategory_Result> ParentCode { get; set; }
        public List<web_getLevel3Category_Result> ChildCode { get; set; }
        public List<web_getTop5ItemLevel1_Result> Top5ItemLevel1 { get; set; }
    }

    public class ContactViewModel
    {
        public web_GetCompanyById_Result  Showroom { get; set; }
        public web_GetCompanyById_Result Online { get; set; }
        public web_GetCompanyById_Result Project { get; set; }
        public web_GetCompanyById_Result Building { get; set; }
        public web_GetCompanyById_Result HongKong { get; set; }
        public web_GetCompanyById_Result VietNam { get; set; }
    }
}