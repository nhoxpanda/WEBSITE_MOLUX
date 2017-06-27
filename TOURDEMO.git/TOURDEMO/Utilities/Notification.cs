using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOURDEMO.Models;
using CRM.Core;
using CRM.Infrastructure;

namespace TOURDEMO.Utilities
{
    public static class Notification
    {
        public static List<tbl_AppointmentHistory> Schedules()
        {
            using (var _db = new DataContext())
            {
                var model = _db.tbl_AppointmentHistory.AsEnumerable()
                             .Where(p => p.IsDelete == false && p.IsNotify == true
                             && (p.StaffId == 9
                             || (p.OtherStaff != null && p.OtherStaff.Contains("9"))))
                             .Select(p => new tbl_AppointmentHistory
                             {
                                 CustomerId = p.CustomerId,
                                 PartnerId = p.PartnerId,
                                 TourId = p.TourId,
                                 OtherStaff = p.OtherStaff,
                                 StaffId = p.StaffId,
                                 Time = p.Time,
                                 tbl_Customer = _db.tbl_Customer.Find(p.CustomerId),
                                 tbl_Partner = _db.tbl_Partner.Find(p.PartnerId),
                                 tbl_Tour = _db.tbl_Tour.Find(p.TourId),
                                 tbl_Staff = _db.tbl_Staff.Find(p.StaffId)
                             }).ToList();

                return model;
            }
        }
    }
}