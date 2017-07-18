using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MOLUX.Models;

namespace MOLUX.Helper
{
    public static class News
    {
        private static BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();

        public static List<web_getAllNewsByCate_Result> NewsList(int id)
        {
            return _db.web_getAllNewsByCate(id).Take(11).ToList();
        }
    }
}