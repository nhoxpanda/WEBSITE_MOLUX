using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Core;
using CRM.Infrastructure;

namespace TOURDEMO.Utilities
{
    public static class UpdateDatabase
    {
        private static DataContext _db = new DataContext();

        public static void UpdateEvaluation(int id, decimal total)
        {
            var model = _db.tbl_Evaluation.Find(id);
            model.Point = total;
            _db.SaveChanges();
        }
    }
}