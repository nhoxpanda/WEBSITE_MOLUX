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
    public class ReceiptBillManageController : BaseController
    {
        // GET: ReceiptBillManage

        #region Init

        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_ActionData> _actionDataRepository;
        private IGenericRepository<tbl_ReceiptBill> _receiptBillRepository;
        private IGenericRepository<tbl_ReceiptBillPeriod> _receiptBillPeriodRepository;
        private DataContext _db;

        public ReceiptBillManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_ActionData> actionDataRepository,
            IGenericRepository<tbl_ReceiptBill> receiptBillRepository,
            IGenericRepository<tbl_ReceiptBillPeriod> receiptBillPeriodRepository,
        IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._actionDataRepository = actionDataRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._receiptBillPeriodRepository = receiptBillPeriodRepository;
            this._receiptBillRepository = receiptBillRepository;
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
            Permission(clsPermission.GetUser().PermissionID, 1142);
            var model = _receiptBillRepository.GetAllAsQueryable()
                .Where(p => p.IsDelete == false).OrderByDescending(p => p.CreatedDate).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult GetIdReceipt(int id)
        {
            Session["idReceipt"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Create

        public async Task<ActionResult> Create(tbl_ReceiptBill model, HttpPostedFileBase FileName)
        {
            Permission(clsPermission.GetUser().PermissionID, 1142);
            model.Code = GenerateCode.ReceiptBillCode();
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
            await _receiptBillRepository.Create(model);
            return RedirectToAction("Index");
        }
        #endregion

        #region Edit
        public async Task<ActionResult> Edit(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1142);
            var model = await _receiptBillRepository.GetById(id);
            return PartialView("_Partial_EditReceiptBill", model);
        }

        public async Task<ActionResult> Update(tbl_ReceiptBill model, HttpPostedFileBase FileName)
        {
            Permission(clsPermission.GetUser().PermissionID, 1142);
            model.ModifiedDate = DateTime.Now;
            if (FileName != null)
            {
                string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                String path = Server.MapPath("~/Upload/file/" + newName);
                FileName.SaveAs(path);
                model.FileName = newName;
            }
            await _receiptBillRepository.Update(model);
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
                        var customer = _receiptBillRepository.FindId(Convert.ToInt32(i));
                    }
                    if (listIds.Count() > 0)
                    {
                        if (await _receiptBillRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "ReceiptBillManage") }, JsonRequestBehavior.AllowGet);
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