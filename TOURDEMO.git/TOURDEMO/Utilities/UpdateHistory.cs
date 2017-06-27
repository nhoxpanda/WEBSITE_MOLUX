using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Core;
using CRM.Infrastructure;

namespace TOURDEMO.Utilities
{
    public static class UpdateHistory
    {
        private static DataContext _db = new DataContext();

        public static void SaveCustomer(int customerId, int formid, int staffId, string note)
        {
            var item = new tbl_UpdateHistory
            {
                CustomerId = customerId,
                StaffId = staffId,
                Note = note,
                CreatedDate = DateTime.Now,
                DictionaryId = 1148,
                IsDelete = false,
                FormId = formid
            };
            _db.tbl_UpdateHistory.Add(item);
            _db.SaveChanges();
        }

        public static void SavePartner(int partnerId, int formid, int staffId, string note)
        {
            var item = new tbl_UpdateHistory
            {
                PartnerId = partnerId,
                StaffId = staffId,
                Note = note,
                CreatedDate = DateTime.Now,
                DictionaryId = 1148,
                IsDelete = false,
                FormId = formid
            };
            _db.tbl_UpdateHistory.Add(item);
            _db.SaveChanges();
        }

        public static void SaveStaff(int staffId, int formid, string note)
        {
            var item = new tbl_UpdateHistory
            {
                StaffId = staffId,
                Note = note,
                CreatedDate = DateTime.Now,
                DictionaryId = 1148,
                IsDelete = false,
                FormId = formid
            };
            _db.tbl_UpdateHistory.Add(item);
            _db.SaveChanges();
        }

        public static void SaveProgram(int programId, int formid, int staffId, string note)
        {
            var item = new tbl_UpdateHistory
            {
                ProgramId = programId,
                StaffId = staffId,
                Note = note,
                CreatedDate = DateTime.Now,
                DictionaryId = 1148,
                IsDelete = false,
                FormId = formid
            };
            _db.tbl_UpdateHistory.Add(item);
            _db.SaveChanges();
        }

        public static void SaveTour(int tourId, int formid, int staffId, string note)
        {
            var item = new tbl_UpdateHistory
            {
                TourId = tourId,
                StaffId = staffId,
                Note = note,
                CreatedDate = DateTime.Now,
                DictionaryId = 1148,
                IsDelete = false,
                FormId = formid
            };
            _db.tbl_UpdateHistory.Add(item);
            _db.SaveChanges();
        }

        public static void SaveContract(int contractId, int formid, int staffId, string note)
        {
            var item = new tbl_UpdateHistory
            {
                ContractId = contractId,
                StaffId = staffId,
                Note = note,
                CreatedDate = DateTime.Now,
                DictionaryId = 1148,
                IsDelete = false,
                FormId = formid
            };
            _db.tbl_UpdateHistory.Add(item);
            _db.SaveChanges();
        }

        public static void SaveHistory(int formid, string note, int? appointment, int? contract, int? customer,
            int? partner, int? program, int? task, int? tour, int? quotation, int? document, int? history, int? ticket)
        {
            var item = new tbl_UpdateHistory
            {
                StaffId = clsPermission.GetUser().StaffID,
                Note = note,
                CreatedDate = DateTime.Now,
                DictionaryId = 1148,
                IsDelete = false,
                FormId = formid,
                ModuleId = LoadData.GetModule(formid),
                AppointmentId = appointment,
                ContractId = contract,
                CustomerId = customer,
                PartnerId = partner,
                ProgramId = program,
                TourId = tour,
                TaskdId = task,
                QuotationId = quotation,
                DocumentFileId = document,
                HistoryId = history,
                TicketId = ticket,
            };
            _db.tbl_UpdateHistory.Add(item);
            _db.SaveChanges();
        }
        

        /// <summary>
        ///  update nhân viên làm nhiệm vụ
        /// </summary>
        /// <param name="idTask"></param>
        /// <param name="idStaff"></param>
        public static void UpdatePermissionTask(int idTask, int idStaff)
        {
            var check = _db.tbl_Task.AsEnumerable().FirstOrDefault(p => p.Id == idTask && p.Permission.Contains(idStaff.ToString()));
            if (check == null)
            {
                var task = _db.tbl_Task.Find(idTask);
                string rs = "," + idStaff;
                task.Permission += rs;
                _db.SaveChanges();
            }
        }

        public static void UpdateCustomerTour(int idCustomer, int tourId)
        {
            // contract
            var contract = _db.tbl_Contract.Where(p => p.TourId == tourId).ToList();
            if (contract.Count() > 0)
            {
                foreach (var c in contract)
                {
                    var item = _db.tbl_Contract.Find(c.Id);
                    item.CustomerId = idCustomer;
                }
            }

            // quotation
            //var quotation = _db.tbl_Quotation.Where(p => p.TourId == tourId).ToList();
            //if (quotation.Count() > 0)
            //{
            //    foreach (var c in quotation)
            //    {
            //        var item = _db.tbl_Quotation.Find(c.Id);
            //        item.CustomerId = idCustomer;
            //    }
            //}

            // program
            var program = _db.tbl_Program.Where(p => p.TourId == tourId).ToList();
            if (program.Count() > 0)
            {
                foreach (var c in program)
                {
                    var item = _db.tbl_Program.Find(c.Id);
                    item.CustomerId = idCustomer;
                }
            }

            // liability
            var liability = _db.tbl_LiabilityCustomer.Where(p => p.TourId == tourId).ToList();
            if (liability.Count() > 0)
            {
                foreach (var c in liability)
                {
                    var item = _db.tbl_LiabilityCustomer.Find(c.Id);
                    item.CustomerId = idCustomer;
                }
            }

            // ticket
            var ticket = _db.tbl_Ticket.Where(p => p.TourId == tourId).ToList();
            if (ticket.Count() > 0)
            {
                foreach (var c in ticket)
                {
                    var item = _db.tbl_Ticket.Find(c.Id);
                    item.CustomerId = idCustomer;
                }
            }

            _db.SaveChanges();
            
        }
    }
}