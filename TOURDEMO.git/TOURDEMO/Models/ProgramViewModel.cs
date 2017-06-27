using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOURDEMO.Models;
using CRM.Core;

namespace TOURDEMO.Models
{
    public class ProgramViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public int DocumentId { get; set; }
        public string Permission { get; set; }
        public string FileName { get; set; }
        public string Note { get; set; }
        public string FileSize { get; set; }
        public string Staff { get; set; }
        public string Date { get; set; }
    }

}