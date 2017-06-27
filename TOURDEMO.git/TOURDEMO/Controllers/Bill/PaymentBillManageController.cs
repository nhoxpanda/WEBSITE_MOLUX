using CRM.Core;
using CRM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TOURDEMO.Models;
using TOURDEMO.Utilities;

namespace TOURDEMO.Controllers.Bill
{
    public class PaymentBillManageController : BaseController
    {
        // GET: PaymentBillManage

        #region Init

        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_ActionData> _actionDataRepository;
        private IGenericRepository<tbl_PaymentBill> _paymentBillRepository;
        private IGenericRepository<tbl_PaymentBillPeriod> _paymentBillPeriodRepository;
        private DataContext _db;

        public PaymentBillManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_ActionData> actionDataRepository,
            IGenericRepository<tbl_PaymentBill> paymentBillRepository,
            IGenericRepository<tbl_PaymentBillPeriod> paymentBillPeriodRepository,
        IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._actionDataRepository = actionDataRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._paymentBillPeriodRepository = paymentBillPeriodRepository;
            this._paymentBillRepository = paymentBillRepository;
            _db = new DataContext();
        }

        #endregion

        #region Permission
        void Permission(int PermissionsId, int formId)
        {
            var list = _actionDataRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.FormId == formId && p.PermissionsId == PermissionsId)
                .Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);
        }
        #endregion

        #region Index
        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 1143);
            var model = _paymentBillRepository.GetAllAsQueryable()
                .Where(p => p.IsDelete == false).OrderByDescending(p => p.CreatedDate).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult GetIdPayment(int id)
        {
            Session["idPayment"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Create

        public async Task<ActionResult> Create(tbl_PaymentBill model, HttpPostedFileBase FileName)
        {
            Permission(clsPermission.GetUser().PermissionID, 1143);
            model.Code = GenerateCode.PaymentBillCode();
            model.CreatedDate = DateTime.Now;
            model.CurrencyId = 1209;
            model.IsDelete = false;
            model.ModifiedDate = DateTime.Now;
            model.StaffId = clsPermission.GetUser().StaffID;
            if (FileName != null)
            {
                string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                String path = Server.MapPath("~/Upload/file/" + newName);
                FileName.SaveAs(path);
                model.FileName = newName;
            }
            await _paymentBillRepository.Create(model);
            return RedirectToAction("Index");
        }
        #endregion

        #region Edit
        public async Task<ActionResult> Edit(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1143);
            var model = await _paymentBillRepository.GetById(id);
            return PartialView("_Partial_EditPaymentBill", model);
        }

        public async Task<ActionResult> Update(tbl_PaymentBill model, HttpPostedFileBase FileName)
        {
            Permission(clsPermission.GetUser().PermissionID, 1143);
            model.ModifiedDate = DateTime.Now;
            if (FileName != null)
            {
                string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                String path = Server.MapPath("~/Upload/file/" + newName);
                FileName.SaveAs(path);
                model.FileName = newName;
            }
            await _paymentBillRepository.Update(model);
            return RedirectToAction("Index");
        }
        #endregion

        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(FormCollection fc)
        {
            Permission(clsPermission.GetUser().PermissionID, 1);
            try
            {
                if (fc["listItemId"] != null && fc["listItemId"] != "")
                {
                    var listIds = fc["listItemId"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    foreach (var i in listIds)
                    {
                        var customer = _paymentBillRepository.FindId(Convert.ToInt32(i));
                    }
                    if (listIds.Count() > 0)
                    {
                        if (await _paymentBillRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "PaymentBillManage") }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Xóa dữ liệu thất bại !" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Vui lòng chọn những mục cần xóa !" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        #endregion
    }
}