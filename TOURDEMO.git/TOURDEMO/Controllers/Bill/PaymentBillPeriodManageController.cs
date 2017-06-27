using CRM.Core;
using CRM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;

namespace TOURDEMO.Controllers.Bill
{
    public class PaymentBillPeriodManageController : BaseController
    {
        // GET: PaymentBillPeriodManage

        #region Init

        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_ActionData> _actionDataRepository;
        private IGenericRepository<tbl_PaymentBillPeriod> _paymentBillPeriodRepository;
        private DataContext _db;

        public PaymentBillPeriodManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_ActionData> actionDataRepository,
            IGenericRepository<tbl_PaymentBillPeriod> paymentBillPeriodRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._actionDataRepository = actionDataRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._paymentBillPeriodRepository = paymentBillPeriodRepository;
            _db = new DataContext();
        }

        #endregion

        #region List
        [ChildActionOnly]
        public ActionResult _DotThanhToan()
        {
            return PartialView("_DotThanhToan");
        }

        public async Task<ActionResult> InfoDotThanhToan(int id)
        {
            var model = await _paymentBillPeriodRepository.GetAllAsQueryable().Where(p => p.PaymentId == id).ToListAsync();
            return PartialView("_DotThanhToan", model);
        }

        #endregion

        #region Create

        public async Task<ActionResult> Create(tbl_PaymentBillPeriod model)
        {
            int receipId = Convert.ToInt32(Session["idPayment"]);
            model.PaymentId = receipId;
            await _paymentBillPeriodRepository.Create(model);
            var list = await _db.tbl_PaymentBillPeriod.Where(p => p.PaymentId == receipId).OrderBy(p => p.Period).ToListAsync();
            return PartialView("_DotThanhToan", list);
        }
        #endregion

        #region Edit
        public async Task<ActionResult> Edit(int id)
        {
            var model = await _paymentBillPeriodRepository.GetById(id);
            return PartialView("_Partial_EditPaymentPeriod", model);
        }

        public async Task<ActionResult> Update(tbl_PaymentBillPeriod model)
        {
            await _paymentBillPeriodRepository.Update(model);
            var list = await _db.tbl_PaymentBillPeriod.Where(p => p.PaymentId == model.Id).OrderBy(p => p.Period).ToListAsync();
            return PartialView("_DotThanhToan", list);
        }
        #endregion

        #region Delete
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            await _paymentBillPeriodRepository.DeleteMany(id.ToString().Split(',').ToArray(), true);
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}