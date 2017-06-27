using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOURDEMO.Models
{
    public class MemberCardViewModel
    {
        public int Id { get; set; }
        public string NameCard { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }

        public decimal Percent { get; set; }
    }
}