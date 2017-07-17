using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM_TTV.Models
{
    public class rolesSS
    {
        public class roleObject
        {
            public string id { get; set; }
            public string text { get; set; }
            public string icon { get; set; }
            public string redirect { get; set; }
            public State state { get; set; }
            public List<roleObject> children { get; set; } = null;

        }
        public class State
        {
            public string urlRequest { get; set; } = "#";
            public int actionID { get; set; } = 0;
            public bool selected { get; set; } = false;
            public bool opened { get; set; } = false; //chỉ dành cho metronic trạng thái của tree
        }
        public class Roles
        {
            public int ID;

            public string name;

            public string roles;
        }
    }
}