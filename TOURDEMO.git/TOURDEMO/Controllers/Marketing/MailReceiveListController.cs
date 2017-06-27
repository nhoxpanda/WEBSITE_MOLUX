using CRM.Core;
using CRM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TOURDEMO.Models;

namespace TOURDEMO.Controllers.Marketing
{
    public class MailReceiveListController : BaseController
    {
        // GET: MailReceiveList

        #region Init

        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_MailReceiveList> _mailReceiveListRepository;
        private IGenericRepository<tbl_MailReceives> _mailReceivesRepository;
        private IGenericRepository<tbl_MailTemplates> _mailTemplatesRepository;
        private IGenericRepository<tbl_MailConfig> _mailConfigRepository;
        private IGenericRepository<tbl_MailImport> _mailImportRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private DataContext _db;

        public MailReceiveListController(
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_MailReceiveList> mailReceiveListRepository,
            IGenericRepository<tbl_MailReceives> mailReceivesRepository,
            IGenericRepository<tbl_MailTemplates> mailTemplatesRepository,
            IGenericRepository<tbl_MailConfig> mailConfigRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_MailImport> mailImportRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
            {
                this._customerRepository = customerRepository;
                this._mailReceiveListRepository = mailReceiveListRepository;
                this._mailReceivesRepository = mailReceivesRepository;
                this._mailTemplatesRepository = mailTemplatesRepository;
                this._mailConfigRepository = mailConfigRepository;
                this._staffRepository = staffRepository;
                this._mailImportRepository = mailImportRepository;
                _db = new DataContext();
            }
        #endregion

        #region Add Staff
        [HttpPost]
        public ActionResult AddStaff(int id)
        {
            Session["idReceive"] = id;
            var model = _staffRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false && p.IsVietlike == true).ToList();
            return PartialView("_Partial_StaffList", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InsertStaff(FormCollection fc)
        {
            try
            {
                int idReceive = Int16.Parse(Session["idReceive"].ToString());
                if (fc["listItemIdAddStaff"] != null && fc["listItemIdAddStaff"] != "")
                {
                    var listIds = fc["listItemIdAddStaff"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        foreach (var id in listIds)
                        {
                            int _id = Int16.Parse(id);
                            var item = _staffRepository.FindId(_id);
                            var staff = new tbl_MailReceiveList
                            {
                                Birthday = item.Birthday,
                                Department = item.DepartmentId != null ? item.tbl_DictionaryDepartment.Name : "",
                                Email = item.Email,
                                FullName = item.FullName,
                                HomeAddress = item.Address,
                                IsDelete = false,
                                MailReceiveId = idReceive,
                                Phone = item.Phone,
                                StaffId = item.Id,
                                Vocative = item.NameTypeId != null ? item.tbl_DictionaryNameType.Name : ""
                            };
                            await _mailReceiveListRepository.Create(staff);
                        }
                        return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Lưu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "MailReceive") }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Vui lòng chọn người dùng !" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Lưu không thành công !" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Add Customer
        [HttpPost]
        public ActionResult AddCustomer(int id)
        {
            Session["idReceive"] = id;
            var model = _customerRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.IsDelete == false && p.IsTemp == false).OrderByDescending(p => p.CreatedDate).ToList();
            return PartialView("_Partial_CustomerList", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InsertCustomer(FormCollection fc)
        {
            try
            {
                int idReceive = Int16.Parse(Session["idReceive"].ToString());
                if (fc["listItemIdAddCus"] != null && fc["listItemIdAddCus"] != "")
                {
                    var listIds = fc["listItemIdAddCus"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        foreach (var id in listIds)
                        {
                            int _id = Int16.Parse(id);
                            var item = _customerRepository.FindId(_id);
                            var cus = new tbl_MailReceiveList
                            {
                                Birthday = item.Birthday,
                                Department = item.CareerId != null ? item.tbl_DictionaryCareer.Name : "",
                                Email = item.PersonalEmail,
                                FullName = item.FullName,
                                HomeAddress = item.Address,
                                IsDelete = false,
                                MailReceiveId = idReceive,
                                Phone = item.Phone,
                                CustomerId = item.Id,
                                Vocative = item.NameTypeId != null ? item.tbl_DictionaryNameType.Name : ""
                            };
                            await _mailReceiveListRepository.Create(cus);
                        }
                        return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Lưu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "MailReceive") }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Vui lòng chọn người dùng !" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Lưu không thành công !" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Add Import
        [HttpPost]
        public ActionResult AddImport(int id)
        {
            Session["idReceive"] = id;
            var model = _mailImportRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.IsDelete == false).OrderByDescending(p => p.CreatedDate).ToList();
            return PartialView("_Partial_ImportList", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InsertImport(FormCollection fc)
        {
            try
            {
                int idReceive = Int16.Parse(Session["idReceive"].ToString());
                if (fc["listItemIdAddImport"] != null && fc["listItemIdAddImport"] != "")
                {
                    var listIds = fc["listItemIdAddImport"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        foreach (var id in listIds)
                        {
                            int _id = Int16.Parse(id);
                            var item = _mailImportRepository.FindId(_id);
                            var cus = new tbl_MailReceiveList
                            {
                                Email = item.Email,
                                FullName = item.Name,
                                IsDelete = false,
                                MailReceiveId = idReceive,
                                Phone = item.Phone,
                                MailImportId = item.Id,
                            };
                            await _mailReceiveListRepository.Create(cus);
                        }
                        return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Lưu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "MailReceive") }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Vui lòng chọn người dùng !" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Lưu không thành công !" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}