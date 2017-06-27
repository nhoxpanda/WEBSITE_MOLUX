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
    public class ReceiptBillPeriodManageController : BaseController
    {
        // GET: ReceiptBillPeriodManage

        #region Init

        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_ActionData> _actionDataRepository;
        private IGenericRepository<tbl_ReceiptBillPeriod> _receiptBillPeriodRepository;
        private DataContext _db;

        public ReceiptBillPeriodManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_ActionData> actionDataRepository,
            IGenericRepository<tbl_ReceiptBillPeriod> receiptBillPeriodRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._actionDataRepository = actionDataRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._receiptBillPeriodRepository = receiptBillPeriodRepository;
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
            var model = await _receiptBillPeriodRepository.GetAllAsQueryable().Where(p => p.ReceiptId == id).ToListAsync();
            return PartialView("_DotThanhToan", model);
        }

        #endregion

        #region Create

        public async Task<ActionResult> Create(tbl_ReceiptBillPeriod model)
        {
            int receipId = Convert.ToInt32(Session["idReceipt"]);
            model.ReceiptId = receipId;
            await _receiptBillPeriodRepository.Create(model);
            var list = await _receiptBillPeriodRepository.GetAllAsQueryable()
                .Where(p => p.ReceiptId == receipId).OrderBy(p => p.Period).ToListAsync();
            return PartialView("_DotThanhToan", list);
        }
        #endregion

        #region Edit
        public async Task<ActionResult> Edit(int id)
        {
            var model = await _receiptBillPeriodRepository.GetById(id);
            return PartialView("_Partial_EditReceiptPeriod", model);
        }

        public async Task<ActionResult> Update(tbl_ReceiptBillPeriod model)
        {
            await _receiptBillPeriodRepository.Update(model);
            var list = await _receiptBillPeriodRepository.GetAllAsQueryable()
                .Where(p => p.ReceiptId == model.Id).OrderBy(p => p.Period).ToListAsync();
            return PartialView("_DotThanhToan", list);
        }
        #endregion

        #region Delete
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            await _receiptBillPeriodRepository.DeleteMany(id.ToString().Split(',').ToArray(), true);
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}