using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Core;
using CRM.Infrastructure;
using System.Data.SqlClient;
using TOURDEMO.Hubs;
using TOURDEMO.Utilities;

namespace TOURDEMO.Models
{
    public class MessagesRepository
    {
        private DataContext _db = new DataContext();

        public IEnumerable<tbl_AppointmentHistory> GetAllMessages()
        {
            var model = _db.tbl_AppointmentHistory.AsEnumerable()
                            .Where(p => p.IsDelete == false && p.IsNotify == true
                            && p.Time.ToString("dd/MM/yyyy hh:mm") == DateTime.Now.ToString("dd/MM/yyyy hh:mm")
                            && (p.StaffId == clsPermission.GetUser().StaffID
                            || (p.OtherStaff != null && p.OtherStaff.Contains(clsPermission.GetUser().StaffID.ToString()))))
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