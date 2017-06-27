using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOURDEMO.Models
{
    public class jDataTableParamModel
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public string search { get; set; }
    }
}