using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOURDEMO.Models
{
    public class UserDetail
    {
        public int id { set; get; }
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
    }
}