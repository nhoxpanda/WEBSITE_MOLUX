using CRM.Core;
using CRM.Infrastructure;
using TOURDEMO.Models;
using TOURDEMO.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using CRM.Enum;

namespace TOURDEMO.Controllers.Quotation
{
    [Authorize]
    public class QuotationManageController : BaseController
    {
        // GET: QuotationManage

        #region Init

        private IGenericRepository<tbl_Tags> _tagRepository;
        private IGenericRepository<tbl_CustomerVisa> _customerVisaRepository;
        private IGenericRepository<tbl_PartnerNote> _partnerNoteRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_Quotation> _quotationRepository;
        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_Form> _formRepository;
        private IGenericRepository<tbl_Module> _moduleRepository;
        private DataContext _db;

        public QuotationManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_PartnerNote> partnerNoteRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_Tags> tagRepository,
            IGenericRepository<tbl_Quotation> quotationRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Form> formRepository,
            IGenericRepository<tbl_Module> moduleRepository,
              IGenericRepository<tbl_CustomerVisa> customerVisaRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._customerRepository = customerRepository;
            this._partnerNoteRepository = partnerNoteRepository;
            this._documentFileRepository = documentFileRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._tagRepository = tagRepository;
            this._quotationRepository = quotationRepository;
            this._updateHistoryRepository = updateHistoryRepository;
            this._formRepository = formRepository;
            this._moduleRepository = moduleRepository;
            this._staffRepository = staffRepository;
            this._customerVisaRepository = customerVisaRepository;
            _db = new DataContext();
        }

        #endregion

        #region Permission
        int SDBID = 6;
        int maPB = 0, maNKD = 0, maNV = 0, maCN = 0;
        void Permission(int PermissionsId, int formId)
        {
            var list = _db.tbl_ActionData.Where(p => p.FormId == formId && p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);
            ViewBag.IsImport = list.Contains(4);
            ViewBag.IsExport = list.Contains(5);
            ViewBag.IsLock = list.Contains(6);
            ViewBag.IsUnLock = list.Contains(7);

            var ltAccess = _db.tbl_AccessData.Where(p => p.PermissionId == PermissionsId && p.FormId == formId).Select(p => p.ShowDataById).FirstOrDefault();
            if (ltAccess != 0)
                this.SDBID = ltAccess;

            switch (SDBID)
            {
                case 2: maPB = clsPermission.GetUser().DepartmentID;
                    maCN = clsPermission.GetUser().BranchID;
                    break;
                case 3: maNKD = clsPermission.GetUser().GroupID;
                    maCN = clsPermission.GetUser().BranchID; break;
                case 4: maNV = clsPermission.GetUser().StaffID; break;
                case 5: maCN = clsPermission.GetUser().BranchID; break;
            }
        }
        #endregion

        #region Index
        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 22);

            if (SDBID == 6)
                return View(new List<tbl_Quotation>());

            var model = _quotationRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => (p.StaffId == maNV | maNV == 0) || (p.StaffQuotationId == maNV | maNV == 0)
                    
                    & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                    & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                    & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false))
                    .OrderByDescending(p => p.CreatedDate).ToList();
            return View(model);
        }
        #endregion

        #region Create
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(tbl_Quotation model, FormCollection form, IEnumerable<HttpPostedFileBase> FileName)
        {
            Permission(clsPermission.GetUser().PermissionID, 22);

            try
            {

                var _priceTour = "";
                if (form["PriceTour"] != null)
                {
                    _priceTour = form["PriceTour"];
                    _priceTour = _priceTour.Replace(",", "");
                    model.PriceTour = int.Parse(_priceTour);
                };
                model.Code = GenerateCode.QuotationCode();
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.TagsId = form["TagsId"] != null && form["TagsId"] != "" ? form["TagsId"].ToString() : null;
                model.StaffId = clsPermission.GetUser().StaffID;
                model.StartDate = DateTime.Now;
                model.EndDate = DateTime.Now;
                model.DictionaryId = 29;
                if (form["QuotationDate"] != null && form["QuotationDate"] != "" && form["QuotationDate"] != "")
                {
                    model.QuotationDate = Convert.ToDateTime(form["QuotationDate"].ToString());
                }

                if (FileName != null)
                {
                    foreach (var file in FileName)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            String path = Server.MapPath("~/Upload/file/" + file.FileName);
                            file.SaveAs(path);
                            model.FileName = file.FileName;
                        }
                        if (await _quotationRepository.Create(model))
                        {
                            UpdateHistory.SaveHistory(22, "Thêm mới báo giá, code: " + model.Code,
                                null, //appointment
                                null, //contract
                                model.CustomerId, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                model.Id, //quotation
                                null, //document
                                null, //history
                                null // ticket
                                );
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            catch { }
            return RedirectToAction("Index");
        }
        #endregion

        #region Update

        [HttpPost]
        public async Task<ActionResult> EditInfoQuotation(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 22);

            var model = await _quotationRepository.GetById(id);
            return PartialView("_Partial_Edit_Quotation", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tbl_Quotation model, FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 22);

            try
            {
                var _priceTour = "";
                if (form["PriceTour"] != null)
                {
                    _priceTour = form["PriceTour"];
                    _priceTour = _priceTour.Replace(",", "");
                    model.PriceTour = int.Parse(_priceTour);
                };
                model.ModifiedDate = DateTime.Now;
                if (form["TagsId"] != null && form["TagsId"] != "")
                {
                    model.TagsId = form["TagsId"].ToString();
                }
                if (form["QuotationDate"] != null && form["QuotationDate"] != "")
                {
                    model.QuotationDate = Convert.ToDateTime(form["QuotationDate"].ToString());
                }

                HttpPostedFileBase file = Request.Files["FileName"];
                if (file != null && file.ContentLength > 0)
                {
                    String path = Server.MapPath("~/Upload/file/" + file.FileName);
                    file.SaveAs(path);
                    model.FileName = file.FileName;
                }

                if (await _quotationRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(22, "Cập nhật báo giá: " + model.Code,
                        null, //appointment
                                null, //contract
                                model.CustomerId, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                model.Id, //quotation
                                null, //document
                                null, //history
                                null // ticket
                                );
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Dữ liệu đầu vào không đúng định dạng!");
                }
            }
            catch
            {
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(FormCollection fc)
        {
            Permission(clsPermission.GetUser().PermissionID, 22);

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
                            var item = _quotationRepository.FindId(Convert.ToInt32(i));
                            UpdateHistory.SaveHistory(22, "Xóa báo giá: " + item.Code,
                                null, //appointment
                                null, //contract
                                item.CustomerId, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                item.Id, //quotation
                                null, //document
                                null, //history
                                null // ticket
                                );
                        }
                        //
                        if (await _quotationRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "QuotationManage") }, JsonRequestBehavior.AllowGet);
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

        #region Cập nhật thay đổi
        [ChildActionOnly]
        public ActionResult _CapNhatThayDoi()
        {
            return PartialView("_CapNhatThayDoi");
        }

        [HttpPost]
        public ActionResult InfoCapNhatThayDoi(int id)
        {
            var model = _updateHistoryRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.IsDelete == false && p.AppointmentId == id)
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new HistoryLogViewModel()
                {
                    Id = p.Id,
                    Staff = _staffRepository.FindId(p.StaffId).FullName,
                    Date = string.Format("{0:dd-MM-yyyy hh:mm tt}", p.CreatedDate),
                    Note = p.Note,
                    StaffId = p.StaffId,
                    Form = p.FormId != null ? _formRepository.FindId(p.FormId).Name : "",
                    Module = p.ModuleId != null ? _moduleRepository.FindId(p.ModuleId).Name : ""
                }).ToList();
            return PartialView("_CapNhatThayDoi", model);
        }
        #endregion

        #region Create Customer
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateCus(CustomerViewModel model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1);

                if (form["radioCustomerType"] == "0" && model.SingleCompany.FullName != null) // doanh nghiệp
                {
                    //model.SingleCompany.Code = GenerateCode.CustomerCode();
                    model.SingleCompany.CustomerType = CustomerType.Organization;
                    model.SingleCompany.TagsId = form["SingleCompany.TagsId"];
                    model.SingleCompany.CreatedDate = DateTime.Now;
                    model.SingleCompany.ModifiedDate = DateTime.Now;
                    model.SingleCompany.IdentityCard = model.IdentityCard;
                    model.SingleCompany.IdentityTagId = model.IdentityTagId;
                    model.SingleCompany.ParentId = 0;
                    model.SingleCompany.PassportCard = model.PassportCard;
                    model.SingleCompany.PassportTagId = model.PassportTagId;
                    model.SingleCompany.NameTypeId = 47;
                    model.SingleCompany.StaffId = clsPermission.GetUser().StaffID;
                    model.SingleCompany.StaffManager = clsPermission.GetUser().StaffID;
                    if (model.CreatedDateIdentity != null && model.CreatedDateIdentity.Year >= 1980)
                    {
                        model.SingleCompany.CreatedDateIdentity = model.CreatedDateIdentity;
                    }
                    if (model.CreatedDatePassport != null && model.CreatedDatePassport.Year >= 1980)
                    {
                        model.SingleCompany.CreatedDatePassport = model.CreatedDatePassport;
                    }
                    if (model.ExpiredDatePassport != null && model.ExpiredDatePassport.Year >= 1980)
                    {
                        model.SingleCompany.ExpiredDatePassport = model.ExpiredDatePassport;
                    }

                    if (await _customerRepository.Create(model.SingleCompany))
                    {
                        UpdateHistory.SaveHistory(1, "Thêm mới khách hàng doanh nghiệp, code: " + model.SingleCompany.Code + " - " + model.SingleCompany.FullName,
                                null, //appointment
                                null, //contract
                                model.SingleCompany.Id, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                null // ticket
                                );

                        for (int i = 1; i < 6; i++)
                        {
                            if (form["VisaNumber" + i] != null && form["VisaNumber" + i] != "")
                            {
                                var visa = new tbl_CustomerVisa
                                {
                                    VisaNumber = form["VisaNumber" + i].ToString(),
                                    TagsId = Convert.ToInt32(form["TagsId" + i].ToString()),
                                    CreatedDate = DateTime.Now,
                                    ModifiedDate = DateTime.Now,
                                    CustomerId = model.SingleCompany.Id,
                                    DictionaryId = 1069,
                                };
                                if (Convert.ToDateTime(form["CreatedDateVisa" + i]).Year >= 1980)
                                {
                                    visa.CreatedDateVisa = Convert.ToDateTime(form["CreatedDateVisa" + i]);
                                }
                                if (Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year >= 1980)
                                {
                                    visa.ExpiredDateVisa = Convert.ToDateTime(form["ExpiredDateVisa" + i]);
                                }
                                if (Convert.ToDateTime(form["CreatedDateVisa" + i]).Year >= 1980 && (Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year >= 1980))
                                {
                                    int age = Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year - Convert.ToDateTime(form["CreatedDateVisa" + i]).Year;
                                    if (Convert.ToDateTime(form["CreatedDateVisa" + i]) > Convert.ToDateTime(form["ExpiredDateVisa" + i]).AddYears(-age)) age--;
                                    visa.Deadline = age;
                                }
                                await _customerVisaRepository.Create(visa);

                            }
                        }

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                if (form["radioCustomerType"] == "1" && model.SinglePersonal.FullName != null) // cá nhân
                {
                    // insert other company
                    if (form["OtherCompanyName"] != null && form["OtherCompanyName"] != "")
                    {
                        var other = new tbl_Customer
                        {
                            Address = form["OtherCompanyAddress"],
                            CompanyEmail = form["OtherCompanyEmail"],
                            CreatedDate = DateTime.Now,
                            Director = form["OtherCompanyDirector"],
                            Phone = form["OtherCompanyPhone"],
                            FullName = form["OtherCompanyName"],
                            IsDelete = false,
                            IsTemp = true,
                            ModifiedDate = DateTime.Now,
                            ParentId = 0,
                            StaffId = clsPermission.GetUser().StaffID,
                            StaffManager = clsPermission.GetUser().StaffID,
                            TagsId = form["OtherCompanyTagsId"],
                            SubscribeSMS = true,
                            SubscribeEmail = true,
                            Code = "OTHERCOMPANY",
                            CareerId = Convert.ToInt32(form["OtherCompanyCareerId"]),
                            CustomerType = CustomerType.Organization,
                            NameTypeId = 47,
                            PassportTagId = 11,
                            IdentityTagId = 11
                        };
                        if (await _customerRepository.Create(other))
                        {
                            UpdateHistory.SaveHistory(1, "Thêm công ty mới: " + other.FullName,
                                null, //appointment
                                null, //contract
                                other.Id, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                null // ticket
                                );

                            model.SinglePersonal.OtherCompany = other.FullName;
                            model.SinglePersonal.ParentId = other.Id;
                        }
                    }
                    //
                    //model.SinglePersonal.Code = GenerateCode.CustomerCode();
                    model.SinglePersonal.CustomerType = CustomerType.Personal;
                    model.SinglePersonal.TagsId = form["SinglePersonal.TagsId"];
                    model.SinglePersonal.CreatedDate = DateTime.Now;
                    model.SinglePersonal.ModifiedDate = DateTime.Now;
                    model.SinglePersonal.IdentityCard = model.IdentityCard;
                    model.SinglePersonal.IdentityTagId = model.IdentityTagId;
                    model.SinglePersonal.PassportCard = model.PassportCard;
                    model.SinglePersonal.PassportTagId = model.PassportTagId;
                    model.SinglePersonal.StaffId = clsPermission.GetUser().StaffID;
                    model.SinglePersonal.StaffManager = clsPermission.GetUser().StaffID;

                    if (model.CreatedDateIdentity != null && model.CreatedDateIdentity.Year >= 1980)
                    {
                        model.SinglePersonal.CreatedDateIdentity = model.CreatedDateIdentity;
                    }
                    if (model.CreatedDatePassport != null && model.CreatedDatePassport.Year >= 1980)
                    {
                        model.SinglePersonal.CreatedDatePassport = model.CreatedDatePassport;
                    }
                    if (model.ExpiredDatePassport != null && model.ExpiredDatePassport.Year >= 1980)
                    {
                        model.SinglePersonal.ExpiredDatePassport = model.ExpiredDatePassport;
                    }

                    if (await _customerRepository.Create(model.SinglePersonal))
                    {
                        UpdateHistory.SaveHistory(1, "Thêm mới khách hàng cá nhân, code: " + model.SinglePersonal.Code + " - " + model.SinglePersonal.FullName,
                                null, //appointment
                                null, //contract
                                model.SinglePersonal.Id, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                null // ticket
                                );

                        for (int i = 1; i < 6; i++)
                        {
                            if (form["VisaNumber" + i] != null && form["VisaNumber" + i] != "")
                            {
                                var visa = new tbl_CustomerVisa
                                {
                                    VisaNumber = form["VisaNumber" + i].ToString(),
                                    TagsId = Convert.ToInt32(form["TagsId" + i].ToString()),
                                    CreatedDate = DateTime.Now,
                                    ModifiedDate = DateTime.Now,
                                    CustomerId = model.SinglePersonal.Id,
                                    DictionaryId = 1069
                                };
                                if (Convert.ToDateTime(form["CreatedDateVisa" + i]).Year >= 1980)
                                {
                                    visa.CreatedDateVisa = Convert.ToDateTime(form["CreatedDateVisa" + i]);
                                }
                                if (Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year >= 1980)
                                {
                                    visa.ExpiredDateVisa = Convert.ToDateTime(form["ExpiredDateVisa" + i]);
                                }
                                if (Convert.ToDateTime(form["CreatedDateVisa" + i]).Year >= 1980 && (Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year >= 1980))
                                {
                                    int age = Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year - Convert.ToDateTime(form["CreatedDateVisa" + i]).Year;
                                    if (Convert.ToDateTime(form["CreatedDateVisa" + i]) > Convert.ToDateTime(form["ExpiredDateVisa" + i]).AddYears(-age)) age--;
                                    visa.Deadline = age;
                                }
                                await _customerVisaRepository.Create(visa);
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            var cusList = _db.tbl_Customer.AsEnumerable()
                                        .Where(p => p.IsDelete == false &&
                                        (p.StaffId == clsPermission.GetUser().StaffID || p.StaffManager == clsPermission.GetUser().StaffID)
                                        || p.tbl_Tour.Where(x => (x.Permission != null && x.Permission.Contains(clsPermission.GetUser().StaffID.ToString()))
                                        || x.StaffId == clsPermission.GetUser().StaffID || x.CreateStaffId == clsPermission.GetUser().StaffID).ToList().Count() > 0)
                                        .OrderByDescending(p => p.CreatedDate)
                                        .Select(p => new tbl_Customer
                                        {
                                            Id = p.Id,
                                            FullName = p.FullName,
                                            Code = p.Code
                                        }).ToList();
            return PartialView("_Partial_CustomerList", cusList);
        }

        [ChildActionOnly]
        public ActionResult _Partial_CustomerList()
        {
            return PartialView("_Partial_CustomerList", LoadData.CustomerList());
        }
        #endregion

        [HttpPost]
        public JsonResult _getQuotation(int id)
        {
            var _quotation = _db.tbl_Quotation.Where(x => x.Id == id && x.IsDelete == false).FirstOrDefault();
            if (_quotation != null)
            {
                QuotationViewModel quotation = new QuotationViewModel()
                {
                    Code = _quotation.Code,
                    PriceTour = _quotation.PriceTour
                    //CountryId=_quotation.CountryId,
                    //CreatedDate=_quotation.CreatedDate,
                    //CurrencyId=_quotation.CurrencyId,
                    //CustomerId=_quotation.CustomerId,
                    //DictionaryId=_quotation.DictionaryId,
                    //EndDate=_quotation.EndDate,
                    //ExpiredDate=_quotation.ExpiredDate,
                    //ExpiredDay=_quotation.ExpiredDay,
                    //FileName=_quotation.FileName,
                    //Id=_quotation.Id,
                    //IsDelete=_quotation.IsDelete,
                    //ModifiedDate=_quotation.ModifiedDate,
                    //Note=_quotation.Note
                };
                return Json(new { result = 1, quotation = quotation }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}