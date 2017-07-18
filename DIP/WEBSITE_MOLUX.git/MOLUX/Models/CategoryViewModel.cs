using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MOLUX.Models
{
    public class CategoryViewModel
    {
        public int RowID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ParentCode { get; set; }
        public List<Item_Category> ChildCategory { get; set; }
    }

    public class CategoryIdViewModel
    {
        public int RowID { get; set; }
    }
}