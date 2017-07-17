using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM_TTV.Models
{
    public class MenuFreeView
    {
        public MenuFreeView()
        {
            children = new List<MenuFreeView>();
        }
        [JsonIgnore]

        public int idDict { get; set; }
        public string id { get; set; }
        [JsonIgnore]
        public int ParentId { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public string redirect { get; set; }
        public State state { get; set; }
        public List<MenuFreeView> children { get; set; } = null;

        public class State
        {
            public string urlRequest { get; set; } = "#";
            public int actionID { get; set; } = 0;
            public bool selected { get; set; } = false;
            //public bool opened { get; set; } = false; //chỉ dành cho metronic trạng thái của tree
        }
    }
    
}