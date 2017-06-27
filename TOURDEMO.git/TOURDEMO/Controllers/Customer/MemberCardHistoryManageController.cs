using CRM.Core;
using CRM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TOURDEMO.Utilities;

namespace TOURDEMO.Controllers.Customer
{
    public class MemberCardHistoryManageController : BaseController
    {
        // GET: MemberCardHistoryManage

        #region Init

        private IGenericRepository<tbl_MemberCard> _memberCardRepository;
        private IGenericRepository<tbl_MemberCardHistory> _memberCardHistoryRepository;
        private IGenericRepository<tbl_ActionData> _actionDataRepository;
        private IGenericRepository<tbl_Customer> _customerRepository;
        private DataContext _db;

        public MemberCardHistoryManageController(
            IGenericRepository<tbl_MemberCard> memberCardRepository,
            IGenericRepository<tbl_MemberCardHistory> memberCardHistoryRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_ActionData> actionDataRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._memberCardRepository = memberCardRepository;
            this._memberCardHistoryRepository = memberCardHistoryRepository;
            this._actionDataRepository = actionDataRepository;
            this._customerRepository = customerRepository;
            _db = new DataContext();
        }

        #endregion

        #region Cập nhật loại thẻ thành viên

        public ActionResult EditCard(int id)
        {
            var customer = _customerRepository.FindId(id);
            ViewBag.MemberCard = customer.MemberCardId;
            ViewBag.CustomerId = id;
            return PartialView("_Partial_UpdateCard");
        }

        public async Task<ActionResult> UpdateCard(FormCollection fc)
        {
            int cusId = Convert.ToInt32(fc["CustomerId"]);
            int cardId = Convert.ToInt32(fc["MemberCardId"]);
            var customer = _db.tbl_Customer.Find(cusId);
            customer.MemberCardId = cardId;
            _db.SaveChanges();
            // thêm lịch sử thẻ
            var member = new tbl_MemberCardHistory()
            {
                CreatedDate = DateTime.Now,
                Point = customer.Point,
                CustomerId = cusId,
                IsDelete = false,
                StaffId = clsPermission.GetUser().StaffID,
                MemberCardId = customer.MemberCardId ?? 0,
                Name = "Cập nhật thẻ thành viên thành: " + _memberCardRepository.FindId(cardId).Name,
                Note = fc["Note"]
            };
            await _memberCardHistoryRepository.Create(member);

            var model = _db.tbl_MemberCardHistory.Where(p => p.CustomerId == cusId).OrderByDescending(p => p.CreatedDate).ToList();
            return PartialView("~/Views/CustomerTabInfo/_TheThanhVien.cshtml", model);
        }

        #endregion

        #region Tích lũy điểm

        public ActionResult EditPoint(int id)
        {
            var customer = _customerRepository.FindId(id);
            ViewBag.Point = customer.Point;
            ViewBag.CustomerId = id;
            return PartialView("_Partial_UpdatePoint");
        }

        public async Task<ActionResult> UpdatePoint(FormCollection fc)
        {
            int cusId = Convert.ToInt32(fc["CustomerId"]);
            int point = Convert.ToInt32(fc["Point"]);
            var customer = _db.tbl_Customer.Find(cusId);
            int oldPoint = customer.Point;
            customer.Point += point;
            _db.SaveChanges();
            // thêm lịch sử thẻ
            var member = new tbl_MemberCardHistory()
            {
                CreatedDate = DateTime.Now,
                Point = point,
                CustomerId = cusId,
                IsDelete = false,
                StaffId = clsPermission.GetUser().StaffID,
                MemberCardId = customer.MemberCardId ?? 0,
                Name = oldPoint < customer.Point ? "Tăng thêm " + point +" điểm" : "Giảm đi " + point + " điểm",
                Note = fc["Note"]
            };
            await _memberCardHistoryRepository.Create(member);

            var model = _db.tbl_MemberCardHistory.Where(p => p.CustomerId == cusId).OrderByDescending(p => p.CreatedDate).ToList();
            return PartialView("~/Views/CustomerTabInfo/_TheThanhVien.cshtml", model);
        }

        #endregion


    }
}