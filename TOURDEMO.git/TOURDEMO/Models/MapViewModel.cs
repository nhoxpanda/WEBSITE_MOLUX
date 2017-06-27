using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOURDEMO.Models
{
    public class MapViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string xMap { get; set; }
        public string yMap { get; set; }
        public string AddressMap { get; set; }
        public string Icon { get; set; }
    }
}