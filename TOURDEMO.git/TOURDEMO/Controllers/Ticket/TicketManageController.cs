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

namespace TOURDEMO.Controllers.Ticket
{
    [Authorize]
    public class TicketManageController : BaseController
    {
        // GET: TicketManage

        #region Init

        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;
        private IGenericRepository<tbl_Contract> _contractRepository;
        private IGenericRepository<tbl_Ticket> _ticketRepository;
        private DataContext _db;

        public TicketManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IGenericRepository<tbl_Contract> contractRepository,
            IGenericRepository<tbl_Ticket> ticketRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._dictionaryRepository = dictionaryRepository;
            this._customerRepository = customerRepository;
            this._staffRepository = staffRepository;
            this._tourRepository = tourRepository;
            this._contractRepository = contractRepository;
            this._ticketRepository = ticketRepository;
            _db = new DataContext();
        }

        #endregion

        #region List

        int SDBID = 6;
        int maPB = 0, maNKD = 0, maNV = 0, maCN = 0;
        void Permission(int PermissionsId, int formId)
        {
            var list = _db.tbl_ActionData.Where(p => p.FormId == formId && p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);

            //cập nhật trạng thái
            var listUS = _db.tbl_ActionData.Where(p => p.FormId == 1098 & p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsUpdateStatus = list.Contains(1);

            var ltAccess = _db.tbl_AccessData.Where(p => p.PermissionId == PermissionsId && p.FormId == formId).Select(p => p.ShowDataById).FirstOrDefault();
            if (ltAccess != 0)
                this.SDBID = ltAccess;

            switch (SDBID)
            {
                case 2:
                    maPB = clsPermission.GetUser().DepartmentID;
                    maCN = clsPermission.GetUser().BranchID;
                    break;
                case 3:
                    maNKD = clsPermission.GetUser().GroupID;
                    maCN = clsPermission.GetUser().BranchID; break;
                case 4: maNV = clsPermission.GetUser().StaffID; break;
                case 5: maCN = clsPermission.GetUser().BranchID; break;
            }
        }

        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 1098);

            if (SDBID == 6)
                return View(new List<tbl_Ticket>());

            var model = _ticketRepository.GetAllAsQueryable().Where(p => (p.Staff == maNV | maNV == 0)
                    & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                    & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                    & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false))
                    .OrderByDescending(p => p.CreateDate)
                    .ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult GetIdTicket(int id)
        {
            Session["idTicket"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Create

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(tbl_Ticket model)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1098);
                var checkcode = _ticketRepository.GetAllAsQueryable().FirstOrDefault(p => p.Code == model.Code && p.IsDelete == false);
                if (checkcode == null)
                {
                    model.CreateDate = DateTime.Now;
                    model.ModifyDate = DateTime.Now;
                    model.IsDelete = false;
                    model.Staff = clsPermission.GetUser().StaffID;
                    if (model.CustomerId != null)
                    {
                        model.Name = _customerRepository.FindId(model.CustomerId).FullName;
                        // cập nhật thông tin KH
                        var item = _db.tbl_Customer.Find(model.CustomerId);
                        item.MobilePhone = model.Phone;
                        item.Skyteam = model.Skyteam;
                        _db.SaveChanges();
                    }
                    if (await _ticketRepository.Create(model))
                    {
                        UpdateHistory.SaveHistory(1098, "Thêm mới vé, code: " + model.Code + " - " + model.Name,
                            null, //appointment
                            model.ContractId, //contract
                            model.CustomerId, //customer
                            null, //partner
                            model.ProgramId, //program
                            null, //task
                            model.TourId, //tour
                            null, //quotation
                            null, //document
                            null, //history
                            model.Id // ticket
                        );
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region Update

        public async Task<ActionResult> EditInfoTicket(int id)
        {
            var model = await _ticketRepository.GetById(id);
            return PartialView("_Partial_EditTicket", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tbl_Ticket model)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1098);
                model.ModifyDate = DateTime.Now;
                model.Staff = clsPermission.GetUser().StaffID;
                if (model.CustomerId != null)
                {
                    model.Name = _customerRepository.FindId(model.CustomerId).FullName;
                    // cập nhật thông tin KH
                    var item = _db.tbl_Customer.Find(model.CustomerId);
                    item.MobilePhone = model.Phone;
                    item.Skyteam = model.Skyteam;
                    _db.SaveChanges();
                }
                if (await _ticketRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(1098, "Cập nhật vé: " + model.Code,
                        null, //appointment
                            model.ContractId, //contract
                            model.CustomerId, //customer
                            null, //partner
                            model.ProgramId, //program
                            null, //task
                            model.TourId, //tour
                            null, //quotation
                            null, //document
                            null, //history
                            model.Id // ticket
                            );
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(FormCollection fc)
        {
            Permission(clsPermission.GetUser().PermissionID, 1098);
            try
            {
                if (fc["listItemId"] != null && fc["listItemId"] != "")
                {
                    var listIds = fc["listItemId"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        //
                        foreach (var i in listIds)
                        {
                            var item = _ticketRepository.FindId(Convert.ToInt32(i));
                            UpdateHistory.SaveHistory(1098, "Xóa Vé máy bay: " + item.Code + " - " + item.Name,
                                null, //appointment
                                item.ContractId, //contract
                                item.CustomerId, //customer
                                null, //partner
                                item.ProgramId, //program
                                null, //task
                                item.TourId, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                item.Id // ticket
                                );
                        }
                        //
                        if (await _ticketRepository.DeleteMany(listIds, false))
                        {
                            return Json(1, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(0, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Other Tab
        public ActionResult _Partial_TabInfoTicket()
        {
            return PartialView("_Partial_TabInfoTicket");
        }

        public ActionResult InfoNote(int id)
        {
            var model = _ticketRepository.FindId(id);
            return PartialView("_Partial_TabInfoTicket", model);
        }
        #endregion

        #region LoadCustomer
        public ActionResult LoadCustomer(int id)
        {
            var model = _customerRepository.FindId(id);
            return Json(new { name = model.FullName, phone = model.MobilePhone, skyteam = model.Skyteam }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}