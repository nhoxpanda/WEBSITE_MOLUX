using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOURDEMO.Models
{
    public class EvaluationCriteriaViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public bool IsDelete { get; set; }
        public int Type { get; set; }
    }
}