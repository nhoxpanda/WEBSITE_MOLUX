using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOURDEMO.Models
{
    public class DictionaryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public int DictionaryCategoryId { get; set; }
        public string Icon { get; set; }
    }
}