using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TOURDEMO.Models;
using CRM.Core;
using CRM.Infrastructure;
using System.Threading.Tasks;
using CRM.Enum;
using TOURDEMO.Utilities;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TOURDEMO.Controllers
{
    [Authorize]
    public class CustomersManageController : BaseController
    {
        //
        // GET: /CustomersManage/

        #region Init

        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_Partner> _partnerRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;
        private IGenericRepository<tbl_CustomerContact> _customerContactRepository;
        private IGenericRepository<tbl_CustomerVisa> _customerVisaRepository;
        private IGenericRepository<tbl_CustomerContactVisa> _customerContactVisaRepository;
        private IGenericRepository<tbl_Contract> _contractRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
        private IGenericRepository<tbl_ContactHistory> _contactHistoryRepository;
        private IGenericRepository<tbl_AppointmentHistory> _appointmentHistoryRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private DataContext _db;

        public CustomersManageController(
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IGenericRepository<tbl_CustomerContact> customerContactRepository,
            IGenericRepository<tbl_CustomerVisa> customerVisaRepository,
            IGenericRepository<tbl_Contract> contractRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_CustomerContactVisa> customerContactVisaRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IGenericRepository<tbl_ContactHistory> contactHistoryRepository,
            IGenericRepository<tbl_AppointmentHistory> appointmentHistoryRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._partnerRepository = partnerRepository;
            this._customerRepository = customerRepository;
            this._customerContactRepository = customerContactRepository;
            this._tagsRepository = tagsRepository;
            this._customerVisaRepository = customerVisaRepository;
            this._customerContactVisaRepository = customerContactVisaRepository;
            this._contractRepository = contractRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._documentFileRepository = documentFileRepository;
            this._contactHistoryRepository = contactHistoryRepository;
            this._appointmentHistoryRepository = appointmentHistoryRepository;
            this._updateHistoryRepository = updateHistoryRepository;
            this._staffRepository = staffRepository;
            _db = new DataContext();
        }

        #endregion

        #region Index
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
            ViewBag.IsManage = list.Contains(10);

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

        [HttpPost]
        public ActionResult GetIdCustomer(int id)
        {
            Session["idCustomer"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 1);

            if (SDBID == 6)
                return View(new List<CustomerListViewModel>());
            return View();
        }


        public JsonResult checkCodeCustomer(string code)
        {
            var _codeTemp = _db.tbl_Customer.Where(p => p.Code.ToLower() == code.ToLower()).Where(x => x.IsDelete == false).SingleOrDefault();
            if (_codeTemp != null)
            {
                return Json(new { check = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { check = false }, JsonRequestBehavior.AllowGet);
        }


        [ChildActionOnly]
        public ActionResult _Partial_CustomerList()
        {
            Permission(clsPermission.GetUser().PermissionID, 1);

            var model = _customerRepository.GetAllAsQueryable().AsEnumerable()
               .Where(p => ((p.StaffId == maNV | maNV == 0) || (p.StaffManager == maNV | maNV == 0)
                   || p.tbl_Tour.Where(x => (x.Permission != null && x.Permission.Contains(maNV.ToString())) || x.StaffId == maNV || x.CreateStaffId == maNV).ToList().Count() > 0)
                  & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                  & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                  & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false) & (p.IsTemp == false))
                  .OrderByDescending(p => p.CreatedDate)
                  .Select(p => new CustomerListViewModel
                  {
                      Id = p.Id,
                      Type = p.CustomerType == CustomerType.Organization ? "Doanh nghiệp" : "Cá nhân",
                      Address = p.Address,
                      Birthday = p.Birthday == null ? "" : p.Birthday.Value.ToString("dd-MM-yyyy"),
                      Career = p.CareerId != null ? p.tbl_DictionaryCareer.Name : "",
                      Code = p.Code == null ? "" : p.Code,
                      Email = p.CompanyEmail == null ? p.PersonalEmail : p.CompanyEmail,
                      StartDate = p.CreatedDatePassport == null ? "" : p.CreatedDatePassport.Value.ToString("dd-MM-yyyy"),
                      EndDate = p.ExpiredDatePassport == null ? "" : p.ExpiredDatePassport.Value.ToString("dd-MM-yyyy"),
                      Fullname = p.FullName == null ? "" : p.FullName,
                      Phone = p.Phone == null ? "" : p.Phone,
                      OtherPhone = p.MobilePhone == null ? "" : p.MobilePhone,
                      Passport = p.PassportCard == null ? "" : p.PassportCard,
                      Skype = p.Skype == null ? "" : p.Skype,
                      TagsId = p.TagsId,
                      Position = p.Position,
                      Department = p.Department,
                      DanhXung = p.NameTypeId != null ? p.tbl_DictionaryNameType.Name : "",
                      NguonKhach = p.OriginId != null ? p.tbl_DictionaryOrigin.Name : "",
                      NhomKH = p.CustomerGroupId != null ? p.tbl_DictionaryCustomerGroup.Name : "",
                      Staff = p.StaffId != null ? p.tbl_Staff.FullName : "",
                      CreateDate = p.ModifiedDate == null ? "" : p.ModifiedDate.ToString("dd-MM-yyyy"),
                      QuanLy = p.StaffManager != null ? p.tbl_StaffManager.FullName : "",
                      Note = p.Note,
                      Point = p.Point,
                      MemberCard = p.tbl_MemberCard != null ? p.tbl_MemberCard.Name : "",
                      CustomerType=p.CustomerType
                  }).ToList();

            return PartialView("_Partial_CustomerList", model);
        }

        [HttpPost]
        public ActionResult FilterCustomerList(int id, int nguon, int nhom, int kyhopdong, DateTime tungay, DateTime denngay)
        {
            Permission(clsPermission.GetUser().PermissionID, 1);

            switch (kyhopdong)
            {
                case 0:
                    {
                        var model = _customerRepository.GetAllAsQueryable().AsEnumerable().Where(p =>
                                   (nguon == 0 ? p.OriginId == p.OriginId : p.OriginId == nguon)
                                   & (nhom == 0 ? p.CustomerGroupId == p.CustomerGroupId : p.CustomerGroupId == nhom)
                                   & (id == 2 ? p.CustomerType == p.CustomerType : Convert.ToInt32(p.CustomerType) == id)
                                   & (tungay == denngay ? tungay.ToString("dd-MM-yyyy") == p.CreatedDate.ToString("dd-MM-yyyy") : tungay <= p.CreatedDate && p.CreatedDate <= denngay)
                                   & ((p.StaffId == maNV | maNV == 0) || (p.StaffManager == maNV | maNV == 0)
                                   || p.tbl_Tour.Where(x => (x.Permission != null && x.Permission.Contains(maNV.ToString())) || x.StaffId == maNV || x.CreateStaffId == maNV).ToList().Count() > 0)
                                   & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                                   & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                                   & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false) & (p.IsTemp == false))
                                   .OrderByDescending(p => p.CreatedDate)
                                   .Select(p => new CustomerListViewModel
                                   {
                                       Id = p.Id,
                                       Address = p.Address,
                                       Type = p.CustomerType == CustomerType.Organization ? "Doanh nghiệp" : "Cá nhân",
                                       Birthday = p.Birthday == null ? "" : p.Birthday.Value.ToString("dd-MM-yyyy"),
                                       Career = p.CareerId != null ? p.tbl_DictionaryCareer.Name : "",
                                       Code = p.Code == null ? "" : p.Code,
                                       Email = p.CompanyEmail == null ? p.PersonalEmail : p.CompanyEmail,
                                       StartDate = p.CreatedDatePassport == null ? "" : p.CreatedDatePassport.Value.ToString("dd-MM-yyyy"),
                                       EndDate = p.ExpiredDatePassport == null ? "" : p.ExpiredDatePassport.Value.ToString("dd-MM-yyyy"),
                                       Fullname = p.FullName == null ? "" : p.FullName,
                                       Phone = p.Phone == null ? "" : p.Phone,
                                       OtherPhone = p.MobilePhone == null ? "" : p.MobilePhone,
                                       Passport = p.PassportCard == null ? "" : p.PassportCard,
                                       Skype = p.Skype == null ? "" : p.Skype,
                                       TagsId = p.TagsId,
                                       Position = p.Position,
                                       Department = p.Department,
                                       DanhXung = p.NameTypeId != null ? p.tbl_DictionaryNameType.Name : "",
                                       NguonKhach = p.OriginId != null ? p.tbl_DictionaryOrigin.Name : "",
                                       NhomKH = p.CustomerGroupId != null ? p.tbl_DictionaryCustomerGroup.Name : "",
                                       Staff = p.StaffId != null ? p.tbl_Staff.FullName : "",
                                       CreateDate = p.ModifiedDate == null ? "" : p.ModifiedDate.ToString("dd-MM-yyyy"),
                                       QuanLy = p.StaffManager != null ? p.tbl_StaffManager.FullName : "",
                                       Note = p.Note,
                                       Point = p.Point,
                                       MemberCard = p.tbl_MemberCard != null ? p.tbl_MemberCard.Name : "",
                                       CustomerType = p.CustomerType
                                   }).ToList();

                        return PartialView("_Partial_CustomerList", model);
                    }
                case 1:
                    {
                        var customer = _customerRepository.GetAllAsQueryable().AsEnumerable().Where(p =>
                                       (nguon == 0 ? p.OriginId == p.OriginId : p.OriginId == nguon)
                                       & (nhom == 0 ? p.CustomerGroupId == p.CustomerGroupId : p.CustomerGroupId == nhom)
                                       & (id == 2 ? p.CustomerType == p.CustomerType : Convert.ToInt32(p.CustomerType) == id)
                                       & (tungay == denngay ? tungay.ToString("dd-MM-yyyy") == p.CreatedDate.ToString("dd-MM-yyyy") : tungay <= p.CreatedDate && p.CreatedDate <= denngay)
                                       & (p.StaffId == maNV | maNV == 0)
                                       & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                                       & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                                       & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false) & (p.IsTemp == false))
                                       .OrderByDescending(p => p.CreatedDate).ToList();

                        var model = (from cs in customer
                                     join ct in _contractRepository.GetAllAsQueryable().Where(p => p.IsDelete == false)
                                     on cs.Id equals ct.CustomerId
                                     select cs).Distinct().Select(p => new CustomerListViewModel
                                     {
                                         Id = p.Id,
                                         Address = p.Address,
                                         CreateDate = p.ModifiedDate == null ? "" : p.ModifiedDate.ToString("dd-MM-yyyy"),
                                         Type = p.CustomerType == CustomerType.Organization ? "Doanh nghiệp" : "Cá nhân",
                                         Birthday = p.Birthday == null ? "" : p.Birthday.Value.ToString("dd-MM-yyyy"),
                                         Career = p.CareerId != null ? p.tbl_DictionaryCareer.Name : "",
                                         Code = p.Code == null ? "" : p.Code,
                                         Email = p.CompanyEmail == null ? p.PersonalEmail : p.CompanyEmail,
                                         StartDate = p.CreatedDatePassport == null ? "" : p.CreatedDatePassport.Value.ToString("dd-MM-yyyy"),
                                         EndDate = p.ExpiredDatePassport == null ? "" : p.ExpiredDatePassport.Value.ToString("dd-MM-yyyy"),
                                         Fullname = p.FullName == null ? "" : p.FullName,
                                         Phone = p.Phone == null ? "" : p.Phone,
                                         OtherPhone = p.MobilePhone == null ? "" : p.MobilePhone,
                                         Passport = p.PassportCard == null ? "" : p.PassportCard,
                                         Skype = p.Skype == null ? "" : p.Skype,
                                         TagsId = p.TagsId,
                                         Position = p.Position,
                                         Department = p.Department,
                                         DanhXung = p.NameTypeId != null ? p.tbl_DictionaryNameType.Name : "",
                                         NguonKhach = p.OriginId != null ? p.tbl_DictionaryOrigin.Name : "",
                                         NhomKH = p.CustomerGroupId != null ? p.tbl_DictionaryCustomerGroup.Name : "",
                                         Staff = p.StaffId != null ? p.tbl_Staff.FullName : "",
                                         Note = p.Note,
                                         Point = p.Point,
                                         MemberCard = p.tbl_MemberCard != null ? p.tbl_MemberCard.Name : "",
                                         CustomerType = p.CustomerType
                                     }).ToList();

                        return PartialView("_Partial_CustomerList", model);
                    }
                case 2:
                    {
                        var model = _customerRepository.GetAllAsQueryable().AsEnumerable().Where(p =>
                                   (nguon == 0 ? p.OriginId == p.OriginId : p.OriginId == nguon)
                                   & (nhom == 0 ? p.CustomerGroupId == p.CustomerGroupId : p.CustomerGroupId == nhom)
                                   & (id == 2 ? p.CustomerType == p.CustomerType : Convert.ToInt32(p.CustomerType) == id)
                                   & (tungay == denngay ? tungay.ToString("dd-MM-yyyy") == p.CreatedDate.ToString("dd-MM-yyyy") : tungay <= p.CreatedDate && p.CreatedDate <= denngay)
                                   & (p.StaffId == maNV | maNV == 0)
                                   & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                                   & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                                   & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false) & (p.IsTemp == false))
                                   .OrderByDescending(p => p.CreatedDate)
                                   .Where(p => !_contractRepository.GetAllAsQueryable().Any(c => c.CustomerId == p.Id))
                                   .Distinct()
                                    .Select(p => new CustomerListViewModel
                                    {
                                        Id = p.Id,
                                        Address = p.Address,
                                        CreateDate = p.ModifiedDate == null ? "" : p.ModifiedDate.ToString("dd-MM-yyyy"),
                                        Type = p.CustomerType == CustomerType.Organization ? "Doanh nghiệp" : "Cá nhân",
                                        Birthday = p.Birthday == null ? "" : p.Birthday.Value.ToString("dd-MM-yyyy"),
                                        Career = p.CareerId != null ? p.tbl_DictionaryCareer.Name : "",
                                        Code = p.Code == null ? "" : p.Code,
                                        Email = p.CompanyEmail == null ? p.PersonalEmail : p.CompanyEmail,
                                        StartDate = p.CreatedDatePassport == null ? "" : p.CreatedDatePassport.Value.ToString("dd-MM-yyyy"),
                                        EndDate = p.ExpiredDatePassport == null ? "" : p.ExpiredDatePassport.Value.ToString("dd-MM-yyyy"),
                                        Fullname = p.FullName == null ? "" : p.FullName,
                                        Phone = p.Phone == null ? "" : p.Phone,
                                        OtherPhone = p.MobilePhone == null ? "" : p.MobilePhone,
                                        Passport = p.PassportCard == null ? "" : p.PassportCard,
                                        Skype = p.Skype == null ? "" : p.Skype,
                                        TagsId = p.TagsId,
                                        Position = p.Position,
                                        Department = p.Department,
                                        DanhXung = p.NameTypeId != null ? p.tbl_DictionaryNameType.Name : "",
                                        NguonKhach = p.OriginId != null ? p.tbl_DictionaryOrigin.Name : "",
                                        NhomKH = p.CustomerGroupId != null ? p.tbl_DictionaryCustomerGroup.Name : "",
                                        Staff = p.StaffId != null ? p.tbl_Staff.FullName : "",
                                        Note = p.Note,
                                        Point = p.Point,
                                        MemberCard = p.tbl_MemberCard != null ? p.tbl_MemberCard.Name : "",
                                        CustomerType = p.CustomerType
                                    }).ToList();

                        return PartialView("_Partial_CustomerList", model);
                    }
            }
            return PartialView("_Partial_CustomerList");
        }
        #endregion

        #region Create
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(CustomerViewModel model, FormCollection form)
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

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
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
            Permission(clsPermission.GetUser().PermissionID, 1);
            try
            {
                if (fc["listItemId"] != null && fc["listItemId"] != "")
                {
                    var listIds = fc["listItemId"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    foreach (var i in listIds)
                    {
                        var customer = _customerRepository.FindId(Convert.ToInt32(i));
                        UpdateHistory.SaveHistory(1, "Xóa khách hàng: " + customer.Code + " - " + customer.FullName,
                                null, //appointment
                                null, //contract
                                customer.Id, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                null // ticket
                                );
                    }
                    if (listIds.Count() > 0)
                    {
                        if (await _customerRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "CustomersManage") }, JsonRequestBehavior.AllowGet);
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

        #region Document
        /********** Quản lý tài liệu ************/

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase FileName)
        {
            if (FileName != null && FileName.ContentLength > 0)
            {
                Session["CustomerFile"] = FileName;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateDocument(tbl_DocumentFile model, FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 55);
            //try
            //{

            string id = Session["idCustomer"].ToString();
            if (ModelState.IsValid)
            {
                model.Code = GenerateCode.DocumentCode();
                model.CustomerId = Convert.ToInt32(id);
                model.CreatedDate = DateTime.Now;
                model.IsRead = false;
                model.ModifiedDate = DateTime.Now;
                if (form["TagsId"] != null && form["TagsId"] != "")
                {
                    model.TagsId = form["TagsId"].ToString();
                }
                model.StaffId = clsPermission.GetUser().StaffID;
                //file
                HttpPostedFileBase FileName = Session["CustomerFile"] as HttpPostedFileBase;
                string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                String path = Server.MapPath("~/Upload/file/" + newName);
                FileName.SaveAs(path);
                //end file
                if (newName != null && FileSize != null)
                {
                    model.FileName = newName;
                    model.FileSize = FileSize;
                }

                if (await _documentFileRepository.Create(model))
                {
                    UpdateHistory.SaveHistory(55, "Thêm mới tài liệu, code: " + model.Code + " - " + model.FileName,
                                null, //appointment
                                null, //contract
                                model.CustomerId, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                model.Id, //document
                                null, //history
                                null // ticket
                                );

                    Session["CustomerFile"] = null;
                    //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.CustomerId.ToString() == id).ToList();
                    var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.CustomerId.ToString() == id).Where(p => p.IsDelete == false)
                            .OrderByDescending(p => p.CreatedDate)
                            .Select(p => new tbl_DocumentFile
                            {
                                Id = p.Id,
                                FileName = p.FileName,
                                ModifiedDate = p.ModifiedDate,
                                FileSize = p.FileSize,
                                Note = p.Note,
                                CreatedDate = p.CreatedDate,
                                TagsId = p.TagsId,
                                tbl_Staff = _staffRepository.FindId(p.StaffId)
                            }).ToList();
                    return PartialView("~/Views/CustomerTabInfo/_HoSoLienQuan.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_HoSoLienQuan.cshtml");
                }
            }
            //}
            //catch { }
            return PartialView("~/Views/CustomerTabInfo/_HoSoLienQuan.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> EditInfoDocument(int id)
        {
            var model = await _documentFileRepository.GetById(id);
            ViewBag.DictionaryId = new SelectList(_dictionaryRepository.GetAllAsQueryable().Where(p => p.DictionaryCategoryId == 1 && p.IsDelete == false), "Id", "Name", model.DictionaryId);
            return PartialView("_Partial_EditDocument", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateDocument(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 55);
                if (ModelState.IsValid)
                {
                    model.IsRead = true;
                    model.ModifiedDate = DateTime.Now;
                    if (form["TagsId"] != null && form["TagsId"] != "")
                    {
                        model.TagsId = form["TagsId"].ToString();
                    }
                    if (Session["CustomerFile"] != null && Session["CustomerFile"] != "")
                    {
                        //file
                        HttpPostedFileBase FileName = Session["CustomerFile"] as HttpPostedFileBase;
                        string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                        String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                        String path = Server.MapPath("~/Upload/file/" + newName);
                        FileName.SaveAs(path);
                        //end file

                        if (FileName != null && FileSize != null)
                        {
                            String pathOld = Server.MapPath("~/Upload/file/" + model.FileName);
                            if (System.IO.File.Exists(pathOld))
                                System.IO.File.Delete(pathOld);
                            model.FileName = newName;
                            model.FileSize = FileSize;
                        }
                    }
                    if (await _documentFileRepository.Update(model))
                    {
                        UpdateHistory.SaveHistory(55, "Cập nhật tài liệu của khách hàng: " + model.Code,
                                null, //appointment
                                null, //contract
                                model.CustomerId, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                model.Id, //document
                                null, //history
                                null // ticket
                                );

                        Session["CustomerFile"] = null;
                        //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.CustomerId == model.CustomerId).ToList();
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.CustomerId == model.CustomerId).Where(p => p.IsDelete == false)
                            .OrderByDescending(p => p.CreatedDate)
                            .Select(p => new tbl_DocumentFile
                            {
                                Id = p.Id,
                                FileName = p.FileName,
                                FileSize = p.FileSize,
                                Note = p.Note,
                                CreatedDate = p.CreatedDate,
                                ModifiedDate = p.ModifiedDate,
                                TagsId = p.TagsId,
                                tbl_Staff = _staffRepository.FindId(p.StaffId)
                            }).ToList();
                        return PartialView("~/Views/CustomerTabInfo/_HoSoLienQuan.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/CustomerTabInfo/_HoSoLienQuan.cshtml");
                    }
                }
            }
            catch
            {
            }

            return PartialView("~/Views/CustomerTabInfo/_HoSoLienQuan.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteDocument(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 55);
                int cusId = _documentFileRepository.FindId(id).CustomerId ?? 0;
                //file
                tbl_DocumentFile documentFile = _documentFileRepository.FindId(id) ?? new tbl_DocumentFile();
                String path = Server.MapPath("~/Upload/file/" + documentFile.FileName);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                //end file
                var listId = id.ToString().Split(',').ToArray();
                //
                var item = _documentFileRepository.FindId(id);
                UpdateHistory.SaveHistory(55, "Xóa tài liệu: " + item.Code,
                                null, //appointment
                                null, //contract
                                item.CustomerId, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                item.Id, //document
                                null, //history
                                null // ticket
                                );
                //
                if (await _documentFileRepository.DeleteMany(listId, false))
                {
                    var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.CustomerId == cusId).Where(p => p.IsDelete == false)
                        .OrderByDescending(p => p.CreatedDate)
                        .Select(p => new tbl_DocumentFile
                        {
                            Id = p.Id,
                            FileName = p.FileName,
                            FileSize = p.FileSize,
                            Note = p.Note,
                            CreatedDate = p.CreatedDate,
                            ModifiedDate = p.ModifiedDate,
                            TagsId = p.TagsId,
                            tbl_Staff = _staffRepository.FindId(p.StaffId)
                        }).ToList();
                    return PartialView("~/Views/CustomerTabInfo/_HoSoLienQuan.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_HoSoLienQuan.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_HoSoLienQuan.cshtml");
            }
        }

        #endregion

        #region Visa
        /********** Quản lý visa ************/
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateVisa(tbl_CustomerVisa model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 54);
                string id = Session["idCustomer"].ToString();
                var checkcode = _customerVisaRepository.GetAllAsQueryable().FirstOrDefault(p => p.VisaNumber == model.VisaNumber && p.IsDelete == false);
                if (checkcode == null)
                {
                    if (ModelState.IsValid)
                    {
                        model.CustomerId = Convert.ToInt32(id);
                        model.CreatedDate = DateTime.Now;
                        model.ModifiedDate = DateTime.Now;

                        if (model.CreatedDateVisa != null)
                        {
                            model.CreatedDateVisa = Convert.ToDateTime(form["CreatedDateVisa"]);
                        }
                        if (model.ExpiredDateVisa != null)
                        {
                            model.ExpiredDateVisa = Convert.ToDateTime(form["ExpiredDateVisa"]);
                        }
                        if (model.ExpiredDateVisa != null && model.CreatedDateVisa != null)
                        {
                            int age = model.ExpiredDateVisa.Value.Year - model.CreatedDateVisa.Value.Year;
                            if (model.CreatedDateVisa > model.ExpiredDateVisa.Value.AddYears(-age)) age--;
                            model.Deadline = age;
                        }
                        if (await _customerVisaRepository.Create(model))
                        {
                            UpdateHistory.SaveHistory(54, "Thêm visa " + model.VisaNumber + " cho khách hàng " + _customerRepository.FindId(model.CustomerId).FullName,
                                null, //appointment
                                null, //contract
                                model.CustomerId, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                null // ticket
                                );
                        }
                    }
                }
                var list = _db.tbl_CustomerVisa.AsEnumerable()
                            .Where(p => p.IsDelete == false).Where(p => p.CustomerId.ToString() == id)
                            .OrderByDescending(p => p.CreatedDate).ToList();
                return PartialView("~/Views/CustomerTabInfo/_Visa.cshtml", list);
            }
            catch { }
            return PartialView("~/Views/CustomerTabInfo/_Visa.cshtml");
        }

        //[ChildActionOnly]
        //public ActionResult _Partial_EditVisa()
        //{
        //    List<SelectListItem> lstTag = new List<SelectListItem>();
        //    List<SelectListItem> lstDictionary = new List<SelectListItem>();
        //    ViewData["TagsId"] = lstTag;
        //    ViewBag.DictionaryId = lstDictionary;
        //    return PartialView("_Partial_EditVisa", new tbl_CustomerVisa());
        //}

        [HttpPost]
        public async Task<ActionResult> EditInfoVisa(int id)
        {
            var model = await _customerVisaRepository.GetById(id);
            List<SelectListItem> lstTag = new List<SelectListItem>();
            foreach (var t in LoadData.DropdownlistCountry().ToList())
            {
                lstTag.Add(new SelectListItem()
                {
                    Text = t.Tags,
                    Value = t.Id.ToString(),
                    Selected = model.TagsId == t.Id ? true : false
                });
            }
            ViewBag.TagsId = lstTag;
            ViewBag.DictionaryId = new SelectList(_dictionaryRepository.GetAllAsQueryable().Where(p => p.DictionaryCategoryId == 14 && p.IsDelete == false), "Id", "Name", model.DictionaryId);
            return PartialView("_Partial_EditVisa", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateVisa(tbl_CustomerVisa model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 54);
                if (ModelState.IsValid)
                {
                    model.CustomerId = Convert.ToInt32(model.CustomerId);
                    model.ModifiedDate = DateTime.Now;
                    if (model.CreatedDateVisa != null)
                    {
                        model.CreatedDateVisa = Convert.ToDateTime(form["CreatedDateVisa"]);
                    }
                    if (model.ExpiredDateVisa != null)
                    {
                        model.ExpiredDateVisa = Convert.ToDateTime(form["ExpiredDateVisa"]);
                    }
                    if (model.ExpiredDateVisa != null && model.CreatedDateVisa != null)
                    {
                        int age = model.ExpiredDateVisa.Value.Year - model.CreatedDateVisa.Value.Year;
                        if (model.CreatedDateVisa > model.ExpiredDateVisa.Value.AddYears(-age)) age--;
                        model.Deadline = age;
                    }

                    if (await _customerVisaRepository.Update(model))
                    {
                        UpdateHistory.SaveHistory(54, "Cập nhật visa của khách hàng: " + model.VisaNumber,
                                null, //appointment
                                null, //contract
                                model.CustomerId, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                null // ticket
                                );

                        var list = _db.tbl_CustomerVisa.AsEnumerable()
                            .Where(p => p.IsDelete == false).Where(p => p.CustomerId == model.CustomerId)
                            .OrderByDescending(p => p.CreatedDate).ToList();
                        return PartialView("~/Views/CustomerTabInfo/_Visa.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/CustomerTabInfo/_Visa.cshtml");
                    }
                }
            }
            catch
            {
            }

            return PartialView("~/Views/CustomerTabInfo/_Visa.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteVisa(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 54);
                int visaId = _customerVisaRepository.FindId(id).CustomerId;
                var listId = id.ToString().Split(',').ToArray();
                //
                var item = _customerVisaRepository.FindId(id);
                UpdateHistory.SaveHistory(66, "Xóa visa " + item.VisaNumber + " của khách hàng: " + item.tbl_Customer.FullName,
                    null, //appointment
                    null, //contract
                    item.CustomerId, //customer
                    null, //partner
                    null, //program
                    null, //task
                    null, //tour
                    null, //quotation
                    null, //document
                    null, //history
                    null // ticket
                    );
                //
                if (await _customerVisaRepository.DeleteMany(listId, false))
                {
                    var list = _db.tbl_CustomerVisa.AsEnumerable()
                        .Where(p => p.IsDelete == false).Where(p => p.CustomerId == visaId)
                        .OrderByDescending(p => p.CreatedDate).ToList();
                    return PartialView("~/Views/CustomerTabInfo/_Visa.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_Visa.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_Visa.cshtml");
            }
        }

        #endregion

        #region Update
        /// <summary>
        /// load thông tin khách hàng
        /// </summary>
        /// <param name="model"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CustomerInfomation(int id)
        {
            var model = new CustomerViewModel();
            var customer = _customerRepository.GetAllAsQueryable().FirstOrDefault(p => p.Id == id);
            var customerVisa = _customerVisaRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.IsDelete == false).Where(p => p.CustomerId == id).ToList();
            if (customer.CustomerType == 0) // doanh nghiep
            {
                model.SingleCompany = customer;
                model.CreatedDateIdentity = customer.CreatedDateIdentity ?? DateTime.Now;
                model.CreatedDatePassport = customer.CreatedDatePassport ?? DateTime.Now;
                model.ExpiredDatePassport = customer.ExpiredDatePassport ?? DateTime.Now;
                model.IdentityCard = customer.IdentityCard;
                model.IdentityTagId = customer.IdentityTagId ?? 0;
                model.PassportCard = customer.PassportCard;
                model.PassportTagId = customer.PassportTagId ?? 0;
            }
            else // ca nhan
            {
                model.SinglePersonal = customer;
                model.CreatedDateIdentity = customer.CreatedDateIdentity ?? DateTime.Now;
                model.CreatedDatePassport = customer.CreatedDatePassport ?? DateTime.Now;
                model.ExpiredDatePassport = customer.ExpiredDatePassport ?? DateTime.Now;
                model.IdentityCard = customer.IdentityCard;
                model.IdentityTagId = customer.IdentityTagId ?? 0;
                model.PassportCard = customer.PassportCard;
                model.PassportTagId = customer.PassportTagId ?? 0;
                model.IsTemp = customer.IsTemp;

                var other = _customerRepository.FindId(model.SinglePersonal.ParentId);
                if (other != null)
                {
                    model.OtherCompanyAddress = other.Address;
                    model.OtherCompanyCareerId = other.CareerId ?? 0;
                    model.OtherCompanyDirector = other.Director;
                    model.OtherCompanyEmail = other.CompanyEmail;
                    model.OtherCompanyName = other.FullName;
                    model.OtherCompanyPhone = other.Phone;
                    model.OtherCompanyTagsId = other.TagsId;
                }
            }
            return PartialView("_Partial_EditCustomer", model);
        }

        /// <summary>
        /// cập nhật khách hàng
        /// </summary>
        /// <param name="model"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(CustomerViewModel model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1);

                if (form["radioCustomerType"] == "0" && model.SingleCompany.FullName != null) // doanh nghiệp
                {
                    model.SingleCompany.CustomerType = CustomerType.Organization;
                    model.SingleCompany.TagsId = form["SingleCompany.TagsId"];
                    model.SingleCompany.ModifiedDate = DateTime.Now;
                    model.SingleCompany.IdentityCard = model.IdentityCard;
                    model.SingleCompany.IdentityTagId = model.IdentityTagId;
                    model.SingleCompany.ParentId = 0;
                    model.SingleCompany.PassportCard = model.PassportCard;
                    model.SingleCompany.PassportTagId = model.PassportTagId;
                    model.SingleCompany.NameTypeId = 47;
                    //model.SingleCompany.FullName = _db.tbl_Company.Find(model.SingleCompany.CompanyId).Name;
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

                    if (await _customerRepository.Update(model.SingleCompany))
                    {
                        UpdateHistory.SaveHistory(1, "Cập nhật khách hàng, code: " + model.SingleCompany.Code + " - " + model.SingleCompany.FullName,
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

                        // xóa tất cả visa của customer
                        var visaList = _customerVisaRepository.GetAllAsQueryable().Where(p => p.IsDelete == false && p.CustomerId == model.SingleCompany.Id).ToList();
                        if (visaList.Count() > 0)
                        {
                            foreach (var v in visaList)
                            {
                                var listId = v.Id.ToString().Split(',').ToArray();
                                await _customerVisaRepository.DeleteMany(listId, false);
                            }
                        }

                        // add các visa mới
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
                                    DictionaryId = 1069
                                };
                                if (form["CreatedDateVisa" + i] != "" && Convert.ToDateTime(form["CreatedDateVisa" + i]).Year >= 1980)
                                {
                                    visa.CreatedDateVisa = Convert.ToDateTime(form["CreatedDateVisa" + i]);
                                }
                                if (form["ExpiredDateVisa" + i] != "" && Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year >= 1980)
                                {
                                    visa.ExpiredDateVisa = Convert.ToDateTime(form["ExpiredDateVisa" + i]);
                                }
                                if (form["CreatedDateVisa" + i] != "" && Convert.ToDateTime(form["CreatedDateVisa" + i]).Year >= 1980 && (form["ExpiredDateVisa" + i] != "" && Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year >= 1980))
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
                    // update other company
                    if (model.SinglePersonal.ParentId != null && model.SinglePersonal.ParentId != 0)
                    {
                        // đã có công ty khác
                        var other = _db.tbl_Customer.Find(model.SinglePersonal.ParentId);
                        other.Address = form["OtherCompanyAddress"];
                        other.CompanyEmail = form["OtherCompanyEmail"];
                        other.Director = form["OtherCompanyDirector"];
                        other.Phone = form["OtherCompanyPhone"];
                        other.FullName = form["OtherCompanyName"];
                        other.ModifiedDate = DateTime.Now;
                        other.StaffId = clsPermission.GetUser().StaffID;
                        other.StaffManager = clsPermission.GetUser().StaffID;
                        other.TagsId = form["OtherCompanyTagsId"];
                        other.CareerId = Convert.ToInt32(form["OtherCompanyCareerId"]);
                        _db.SaveChanges();

                        model.SinglePersonal.OtherCompany = other.FullName;
                    }
                    else
                    {
                        // chưa có công ty khác
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
                            NameTypeId = 47,
                            CareerId = Convert.ToInt32(form["OtherCompanyCareerId"]),
                            PassportTagId = 11,
                            IdentityTagId = 11,
                            Code = "OTHERCOMPANY"
                        };
                        if (await _customerRepository.Create(other))
                        {
                            UpdateHistory.SaveHistory(1, "Thêm công ty mới " + other.FullName,
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
                    model.SinglePersonal.CustomerType = CustomerType.Personal;
                    model.SinglePersonal.TagsId = form["SinglePersonal.TagsId"];
                    model.SinglePersonal.ModifiedDate = DateTime.Now;
                    model.SinglePersonal.IdentityCard = model.IdentityCard;
                    model.SinglePersonal.IdentityTagId = model.IdentityTagId;
                    model.SinglePersonal.PassportCard = model.PassportCard;
                    model.SinglePersonal.PassportTagId = model.PassportTagId;
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

                    if (await _customerRepository.Update(model.SinglePersonal))
                    {
                        UpdateHistory.SaveHistory(1, "Cập nhật khách hàng, code: " + model.SinglePersonal.Code + " - " + model.SinglePersonal.FullName,
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

                        // xóa tất cả visa của customer
                        var visaList = _customerVisaRepository.GetAllAsQueryable().Where(p => p.IsDelete == false && p.CustomerId == model.SinglePersonal.Id).ToList();
                        if (visaList.Count() > 0)
                        {
                            foreach (var v in visaList)
                            {
                                var listId = v.Id.ToString().Split(',').ToArray();
                                await _customerVisaRepository.DeleteMany(listId, false);
                            }
                        }

                        // add các visa mới
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
                                if (Convert.ToDateTime(form["CreatedDateVisa" + i]).Year >= 1980 && Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year >= 1980)
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
            }
            catch { }
            return RedirectToAction("Index");
        }
        #endregion

        #region Import
        [HttpPost]
        public ActionResult ImportFile(HttpPostedFileBase FileName)
        {
            try
            {

                using (var excelPackage = new ExcelPackage(FileName.InputStream))
                {
                    List<tbl_Customer> list = new List<tbl_Customer>();
                    var worksheet = excelPackage.Workbook.Worksheets[1];
                    var lastRow = worksheet.Dimension.End.Row;
                    for (int row = 7; row <= lastRow; row++)
                    {
                        if (worksheet.Cells["D" + row].Value == null || worksheet.Cells["D" + row].Text == "")
                            continue;
                        var cus = new tbl_Customer
                        {
                            Code = worksheet.Cells["B" + row].Value != null ? worksheet.Cells["B" + row].Text : "",
                            FullName = worksheet.Cells["D" + row].Value != null ? worksheet.Cells["D" + row].Text : null,
                            Phone = worksheet.Cells["G" + row].Value != null ? worksheet.Cells["G" + row].Text : null,
                            MobilePhone = worksheet.Cells["H" + row].Value != null ? worksheet.Cells["H" + row].Text : null,
                            Address = worksheet.Cells["I" + row].Value != null ? worksheet.Cells["I" + row].Text : null,
                            PassportCard = worksheet.Cells["P" + row].Value != null ? worksheet.Cells["P" + row].Text : null,
                            PersonalEmail = worksheet.Cells["S" + row].Value != null ? worksheet.Cells["S" + row].Text : null,
                            CompanyEmail = worksheet.Cells["S" + row].Value != null ? worksheet.Cells["S" + row].Text : null,
                            Director = worksheet.Cells["T" + row].Value != null ? worksheet.Cells["T" + row].Text : null,
                            Fax = worksheet.Cells["U" + row].Value != null ? worksheet.Cells["U" + row].Text : null,
                            TaxCode = worksheet.Cells["V" + row].Value != null ? worksheet.Cells["V" + row].Text : null,
                            Note = worksheet.Cells["W" + row].Value != null ? worksheet.Cells["W" + row].Text : null,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                        };
                        // staff
                        cus.StaffId = cus.StaffManager = clsPermission.GetUser().StaffID;
                        string cel = "";
                        try
                        {
                            if (worksheet.Cells["F" + row].Value != null && worksheet.Cells["F" + row].Text != "")
                            {
                                cus.Birthday = Convert.ToDateTime(worksheet.Cells["F" + row].Text);
                            }
                            if (worksheet.Cells["Q" + row].Value != null && worksheet.Cells["Q" + row].Text != "")
                            {
                                cus.CreatedDatePassport = Convert.ToDateTime(worksheet.Cells["Q" + row].Text);
                            }
                            if (worksheet.Cells["R" + row].Value != null && worksheet.Cells["R" + row].Text != "")
                            {
                                cus.ExpiredDatePassport = Convert.ToDateTime(worksheet.Cells["R" + row].Text);
                            }
                        }
                        catch { }
                        // danh xưng
                        try
                        {
                            cel = "C";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string danhxung = worksheet.Cells[cel + row].Text;
                                cus.NameTypeId = _dictionaryRepository.GetAllAsQueryable().AsEnumerable().FirstOrDefault(c => c.Name == danhxung && c.DictionaryCategoryId == 7).Id;
                            }
                        }
                        catch { }

                        // loại khách
                        try
                        {
                            cel = "E";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string loaikhach = worksheet.Cells[cel + row].Text;
                                cus.CustomerType = loaikhach == "0" ? CustomerType.Organization : CustomerType.Personal;
                            }
                        }
                        catch { }
                        // nguồn khách
                        try
                        {
                            cel = "N";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string nguonkhach = worksheet.Cells[cel + row].Text;
                                cus.OriginId = _dictionaryRepository.GetAllAsQueryable().AsEnumerable().FirstOrDefault(c => c.Name == nguonkhach && c.DictionaryCategoryId == 4).Id;
                            }
                        }
                        catch { }
                        //tagid dia chi
                        try
                        {
                            // tinh thanh
                            cel = "J";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string tinhtp = worksheet.Cells[cel + row].Text;
                                cus.TagsId = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == tinhtp && c.TypeTag == 5).Select(c => c.Id).SingleOrDefault().ToString();
                            }
                            // quan huyen
                            cel = "K";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string quanhuyen = worksheet.Cells[cel + row].Text;
                                var tagid = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == quanhuyen && c.TypeTag == 6).SingleOrDefault();
                                if (tagid != null)
                                    if (cus.TagsId != null)
                                        cus.TagsId += "," + tagid.Id;
                                    else
                                        cus.TagsId = tagid.Id.ToString();
                            }
                            // phuong xa
                            cel = "L";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string phuongxa = worksheet.Cells[cel + row].Text;
                                var tagid = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == phuongxa && c.TypeTag == 7).SingleOrDefault();
                                if (tagid != null)
                                    if (cus.TagsId != null)
                                        cus.TagsId += "," + tagid.Id;
                                    else
                                        cus.TagsId = tagid.Id.ToString();
                            }
                        }
                        catch { }
                        //nhóm khách hàng
                        try
                        {
                            cel = "O";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string nhomkh = worksheet.Cells[cel + row].Text;
                                cus.CustomerGroupId = _dictionaryRepository.GetAllAsQueryable().AsEnumerable().FirstOrDefault(c => c.Name == nhomkh && c.DictionaryCategoryId == 3).Id;
                            }
                        }
                        catch { }
                        //ngành nghề
                        try
                        {
                            cel = "M";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string nganhnghe = worksheet.Cells[cel + row].Text;
                                cus.CareerId = _dictionaryRepository.GetAllAsQueryable().AsEnumerable().FirstOrDefault(c => c.Name == nganhnghe && c.DictionaryCategoryId == 2).Id;
                            }
                        }
                        catch { }
                        list.Add(cus);
                    }
                    Session["listCustomerImport"] = list;
                    return PartialView("_Partial_ImportDataList", list);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveImport(int id)
        {
            try
            {
                List<tbl_Customer> list = Session["listCustomerImport"] as List<tbl_Customer>;
                int i = 0;
                if (id == 1) // cap nhat
                {
                    foreach (var item in list)
                    {
                        var check = _db.tbl_Customer.FirstOrDefault(p => p.Code == item.Code || p.FullName == item.FullName);
                        if (check != null)
                        {
                            // đã có khách hàng này --> cập nhật
                            item.Id = check.Id;
                            item.IsDelete = false;
                            item.CreatedDate = check.CreatedDate;
                            item.ModifiedDate = DateTime.Now;
                            await _customerRepository.Update(item);
                            i++;
                        }
                    }
                }
                else // them moi
                {
                    foreach (var item in list)
                    {
                        //item.Code = GenerateCode.CustomerCode();
                        await _customerRepository.Create(item);
                        i++;
                    }
                }
                Session["listCustomerImport"] = null;
                if (i != 0)
                {
                    UpdateHistory.SaveHistory(1, "Import danh sách khách hàng",
                        null, //appointment
                                null, //contract
                                null, //customer
                                null, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                null // ticket
                                );
                    return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Đã import thành công " + i + " dòng dữ liệu !", IsPartialView = false, RedirectTo = Url.Action("Index", "CustomersManage") }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Chưa có dữ liệu nào được import !" }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                Session["listCustomerImport"] = null;
                return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Import dữ liệu lỗi !" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteImport(String listItemId)
        {
            try
            {
                List<tbl_Customer> list = Session["listCustomerImport"] as List<tbl_Customer>;
                if (listItemId != null && listItemId != "")
                {
                    var listIds = listItemId.Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        int[] listIdsint = new int[listIds.Length];
                        for (int i = 0; i < listIds.Length; i++)
                        {
                            listIdsint[i] = Int32.Parse(listIds[i]);
                        }
                        for (int i = 0; i < listIdsint.Length; i++)
                        {
                            for (int j = i; j < listIdsint.Length; j++)
                            {
                                if (listIdsint[i] < listIdsint[j])
                                {
                                    int temp = listIdsint[i];
                                    listIdsint[i] = listIdsint[j];
                                    listIdsint[j] = temp;
                                }
                            }
                        }
                        foreach (var item in listIdsint)
                        {
                            list.RemoveAt(item);
                        }
                    }
                }
                Session["listCustomerImport"] = list;
                return PartialView("_Partial_ImportDataList", list);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Export

        public tbl_Tags Tags(string ids, int type)
        {
            return _tagsRepository.GetAllAsQueryable().AsEnumerable().FirstOrDefault(x => ids != null && ids.Contains(x.Id.ToString()) && x.TypeTag == type);
        }

        /// <summary>
        /// Export file excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportFile(FormCollection form)
        {
            var cusContact = _customerContactRepository.GetAllAsQueryable();
            var model = new List<CustomerExportNew>();
            switch (Convert.ToInt32(form["kyhopdong"]))
            {
                case 0:
                    {
                        model = _customerRepository.GetAllAsQueryable().AsEnumerable().Where(p =>
                                   (form["nguonden"] == "0" ? p.OriginId == p.OriginId : p.OriginId == Convert.ToInt32(form["nguonden"]))
                                   & (form["nhomkh"] == "0" ? p.CustomerGroupId == p.CustomerGroupId : p.CustomerGroupId == Convert.ToInt32(form["nhomkh"]))
                                   & (form["loaikh"] == "2" ? p.CustomerType == p.CustomerType : Convert.ToInt32(p.CustomerType) == Convert.ToInt32(form["loaikh"]))
                                   & (form["tungay"] == form["denngay"] ? Convert.ToDateTime(form["tungay"]).ToString("dd-MM-yyyy") == p.CreatedDate.ToString("dd-MM-yyyy") : Convert.ToDateTime(form["tungay"]) <= p.CreatedDate && p.CreatedDate <= Convert.ToDateTime(form["denngay"]))
                                   & ((p.StaffId == maNV | maNV == 0) || (p.StaffManager == maNV | maNV == 0)
                                   || p.tbl_Tour.Where(x => (x.Permission != null && x.Permission.Contains(maNV.ToString())) || x.StaffId == maNV || x.CreateStaffId == maNV).ToList().Count() > 0)
                                   & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                                   & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                                   & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false) & (p.IsTemp == false))
                                   .OrderByDescending(p => p.CreatedDate)
                                   .Select(p => new CustomerExportNew
                                   {
                                       CODE = p.Code,
                                       HOTEN = p.FullName,
                                       DANHXUNG = p.NameTypeId != null ? p.tbl_DictionaryNameType.Name : "",
                                       DIACHI = p.Address,
                                       DIDONG = p.MobilePhone,
                                       DIENTHOAI = p.Phone,
                                       EMAIL = p.PersonalEmail == null ? p.CompanyEmail : p.PersonalEmail,
                                       FAX = p.Fax,
                                       GHICHU = p.Note,
                                       GIAMDOC = p.Director,
                                       LOAIKHACH = p.CustomerType == CustomerType.Organization ? "Doanh nghiệp" : "Cá nhân",
                                       MST = p.TaxCode,
                                       NGANHNGHE = p.CareerId != null ? p.tbl_DictionaryCareer.Name : "",
                                       NGAYHETHAN = p.ExpiredDatePassport == null ? "" : p.ExpiredDatePassport.Value.ToString("dd/MM/yyyy"),
                                       NGAYHIEULUC = p.CreatedDatePassport == null ? "" : p.CreatedDatePassport.Value.ToString("dd/MM/yyyy"),
                                       NGAYSINH = p.Birthday == null ? "" : p.Birthday.Value.ToString("dd/MM/yyyy"),
                                       NGUONDEN = p.OriginId != null ? p.tbl_DictionaryOrigin.Name : "",
                                       NHOMKHACH = p.CustomerGroupId != null ? p.tbl_DictionaryCustomerGroup.Name : "",
                                       PASSPORT = p.PassportCard,
                                       PHUONGXA = Tags(p.TagsId, 7) != null ? Tags(p.TagsId, 7).Tag : "",
                                       QUANHUYEN = Tags(p.TagsId, 6) != null ? Tags(p.TagsId, 6).Tag : "",
                                       TINHTP = Tags(p.TagsId, 5) != null ? Tags(p.TagsId, 5).Tag : "",
                                       NGAYNHAP = p.ModifiedDate.ToString("dd/MM/yyyy"),
                                       NGUOINHAP = p.tbl_Staff.FullName
                                   }).ToList();
                        break;
                    }
                case 1:
                    {
                        var customer = _customerRepository.GetAllAsQueryable().AsEnumerable().Where(p =>
                                       (form["nguonden"] == "0" ? p.OriginId == p.OriginId : p.OriginId == Convert.ToInt32(form["nguonden"]))
                                       & (form["nhomkh"] == "0" ? p.CustomerGroupId == p.CustomerGroupId : p.CustomerGroupId == Convert.ToInt32(form["nhomkh"]))
                                       & (form["loaikh"] == "2" ? p.CustomerType == p.CustomerType : Convert.ToInt32(p.CustomerType) == Convert.ToInt32(form["loaikh"]))
                                       & (form["tungay"] == form["denngay"] ? Convert.ToDateTime(form["tungay"]).ToString("dd-MM-yyyy") == p.CreatedDate.ToString("dd-MM-yyyy") : Convert.ToDateTime(form["tungay"]) <= p.CreatedDate && p.CreatedDate <= Convert.ToDateTime(form["denngay"]))
                                       & (p.StaffId == maNV | maNV == 0)
                                       & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                                       & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                                       & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false) & (p.IsTemp == false))
                                       .OrderByDescending(p => p.CreatedDate).ToList();

                        model = (from cs in customer
                                 join ct in _contractRepository.GetAllAsQueryable().Where(p => p.IsDelete == false)
                                 on cs.Id equals ct.CustomerId
                                 select cs).Distinct().Select(p => new CustomerExportNew
                                 {
                                     CODE = p.Code,
                                     HOTEN = p.FullName,
                                     DANHXUNG = p.NameTypeId != null ? p.tbl_DictionaryNameType.Name : "",
                                     DIACHI = p.Address,
                                     DIDONG = p.MobilePhone,
                                     DIENTHOAI = p.Phone,
                                     EMAIL = p.PersonalEmail == null ? p.CompanyEmail : p.PersonalEmail,
                                     FAX = p.Fax,
                                     GHICHU = p.Note,
                                     GIAMDOC = p.Director,
                                     LOAIKHACH = p.CustomerType == CustomerType.Organization ? "Doanh nghiệp" : "Cá nhân",
                                     MST = p.TaxCode,
                                     NGANHNGHE = p.CareerId != null ? p.tbl_DictionaryCareer.Name : "",
                                     NGAYHETHAN = p.ExpiredDatePassport == null ? "" : p.ExpiredDatePassport.Value.ToString("dd/MM/yyyy"),
                                     NGAYHIEULUC = p.CreatedDatePassport == null ? "" : p.CreatedDatePassport.Value.ToString("dd/MM/yyyy"),
                                     NGAYSINH = p.Birthday == null ? "" : p.Birthday.Value.ToString("dd/MM/yyyy"),
                                     NGUONDEN = p.OriginId != null ? p.tbl_DictionaryOrigin.Name : "",
                                     NHOMKHACH = p.CustomerGroupId != null ? p.tbl_DictionaryCustomerGroup.Name : "",
                                     PASSPORT = p.PassportCard,
                                     PHUONGXA = Tags(p.TagsId, 7) != null ? Tags(p.TagsId, 7).Tag : "",
                                     QUANHUYEN = Tags(p.TagsId, 6) != null ? Tags(p.TagsId, 6).Tag : "",
                                     TINHTP = Tags(p.TagsId, 5) != null ? Tags(p.TagsId, 5).Tag : "",
                                     NGAYNHAP = p.ModifiedDate.ToString("dd/MM/yyyy"),
                                     NGUOINHAP = p.tbl_Staff.FullName
                                 }).ToList();
                        break;
                    }
                case 2:
                    {
                        model = _customerRepository.GetAllAsQueryable().AsEnumerable().Where(p =>
                                   (form["nguonden"] == "0" ? p.OriginId == p.OriginId : p.OriginId == Convert.ToInt32(form["nguonden"]))
                                   & (form["nhomkh"] == "0" ? p.CustomerGroupId == p.CustomerGroupId : p.CustomerGroupId == Convert.ToInt32(form["nhomkh"]))
                                   & (form["loaikh"] == "2" ? p.CustomerType == p.CustomerType : Convert.ToInt32(p.CustomerType) == Convert.ToInt32(form["loaikh"]))
                                   & (form["tungay"] == form["denngay"] ? Convert.ToDateTime(form["tungay"]).ToString("dd-MM-yyyy") == p.CreatedDate.ToString("dd-MM-yyyy") : Convert.ToDateTime(form["tungay"]) <= p.CreatedDate && p.CreatedDate <= Convert.ToDateTime(form["denngay"]))
                                   & (p.StaffId == maNV | maNV == 0)
                                   & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                                   & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                                   & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false) & (p.IsTemp == false))
                                   .OrderByDescending(p => p.CreatedDate)
                                   .Where(p => !_contractRepository.GetAllAsQueryable().Any(c => c.CustomerId == p.Id))
                                   .Distinct().Select(p => new CustomerExportNew
                                   {
                                       CODE = p.Code,
                                       HOTEN = p.FullName,
                                       DANHXUNG = p.NameTypeId != null ? p.tbl_DictionaryNameType.Name : "",
                                       DIACHI = p.Address,
                                       DIDONG = p.MobilePhone,
                                       DIENTHOAI = p.Phone,
                                       EMAIL = p.PersonalEmail == null ? p.CompanyEmail : p.PersonalEmail,
                                       FAX = p.Fax,
                                       GHICHU = p.Note == null ? "" : Encoding.UTF8.GetString(Encoding.Default.GetBytes(p.Note)),
                                       GIAMDOC = p.Director,
                                       LOAIKHACH = p.CustomerType == CustomerType.Organization ? "Doanh nghiệp" : "Cá nhân",
                                       MST = p.TaxCode,
                                       NGANHNGHE = p.CareerId != null ? p.tbl_DictionaryCareer.Name : "",
                                       NGAYHETHAN = p.ExpiredDatePassport == null ? "" : p.ExpiredDatePassport.Value.ToString("dd/MM/yyyy"),
                                       NGAYHIEULUC = p.CreatedDatePassport == null ? "" : p.CreatedDatePassport.Value.ToString("dd/MM/yyyy"),
                                       NGAYSINH = p.Birthday == null ? "" : p.Birthday.Value.ToString("dd/MM/yyyy"),
                                       NGUONDEN = p.OriginId != null ? p.tbl_DictionaryOrigin.Name : "",
                                       NHOMKHACH = p.CustomerGroupId != null ? p.tbl_DictionaryCustomerGroup.Name : "",
                                       PASSPORT = p.PassportCard,
                                       PHUONGXA = Tags(p.TagsId, 7) != null ? Tags(p.TagsId, 7).Tag : "",
                                       QUANHUYEN = Tags(p.TagsId, 6) != null ? Tags(p.TagsId, 6).Tag : "",
                                       TINHTP = Tags(p.TagsId, 5) != null ? Tags(p.TagsId, 5).Tag : "",
                                       NGAYNHAP = p.ModifiedDate.ToString("dd/MM/yyyy"),
                                       NGUOINHAP = p.tbl_Staff.FullName
                                   }).ToList();
                        break;
                    }
            }
            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    ExportCustomersToXlsx(stream, model);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "Danh sách khách hàng.xlsx");
            }
            catch (Exception)
            {

            }
            return RedirectToAction("Index");
        }

        public virtual void ExportCustomersToXlsx(Stream stream, IList<CustomerExportNew> customers)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.Add("Customers");
                var properties = new[]
                    {
                        "STT",
                        "CODE",
                        "DANH XƯNG",
                        "TÊN KHÁCH HÀNG",
                        "LOẠI KHÁCH",
                        "NGÀY SINH",
                        "ĐIỆN THOẠI",
                        "DI ĐỘNG",
                        "ĐỊA CHỈ",
                        "TỈNH TP",
                        "QUẬN HUYỆN",
                        "PHƯỜNG XÃ",
                        "NGÀNH NGHỀ",
                        "NGUỒN KHÁCH",
                        "NHÓM KHÁCH",
                        "PASSPORT",
                        "NGÀY HIỆU LỰC",
                        "NGÀY HẾT HẠN",
                        "EMAIL",
                        "GIÁM ĐỐC",
                        "FAX",
                        "MÃ SỐ THUẾ",
                        "GHI CHÚ",
                        "NGƯỜI NHẬP",
                        "NGÀY NHẬP"
                    };

                worksheet.Cells["A1:AJ1"].Value = "DANH SÁCH KHÁCH HÀNG";
                worksheet.Cells["A1:AJ1"].Style.Font.SetFromFont(new Font("Tahoma", 16));
                worksheet.Cells["A1:AJ1"].Style.Font.Bold = true;
                worksheet.Cells["A1:AJ1"].Merge = true;
                worksheet.Cells["A1:AJ1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A1:AJ1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells["A1:AJ1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:AJ1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));

                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[6, i + 1].Value = properties[i];
                    worksheet.Cells[6, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[6, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[6, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(191, 191, 191));
                }

                int row = 7;
                foreach (var customer in customers)
                {
                    int col = 1;

                    worksheet.Cells[row, col].Value = row - 6;
                    col++;

                    worksheet.Cells[row, col].Value = customer.CODE;
                    col++;

                    worksheet.Cells[row, col].Value = customer.DANHXUNG;
                    col++;

                    worksheet.Cells[row, col].Value = customer.HOTEN;
                    col++;

                    worksheet.Cells[row, col].Value = customer.LOAIKHACH;
                    col++;

                    worksheet.Cells[row, col].Value = customer.NGAYSINH;
                    col++;

                    worksheet.Cells[row, col].Value = customer.DIENTHOAI;
                    col++;

                    worksheet.Cells[row, col].Value = customer.DIDONG;
                    col++;

                    worksheet.Cells[row, col].Value = customer.DIACHI;
                    col++;

                    worksheet.Cells[row, col].Value = customer.TINHTP;
                    col++;

                    worksheet.Cells[row, col].Value = customer.QUANHUYEN;
                    col++;

                    worksheet.Cells[row, col].Value = customer.PHUONGXA;
                    col++;

                    worksheet.Cells[row, col].Value = customer.NGANHNGHE;
                    col++;

                    worksheet.Cells[row, col].Value = customer.NGUONDEN;
                    col++;

                    worksheet.Cells[row, col].Value = customer.NHOMKHACH;
                    col++;

                    worksheet.Cells[row, col].Value = customer.PASSPORT;
                    col++;

                    worksheet.Cells[row, col].Value = customer.NGAYHIEULUC;
                    col++;

                    worksheet.Cells[row, col].Value = customer.NGAYHETHAN;
                    col++;

                    worksheet.Cells[row, col].Value = customer.EMAIL;
                    col++;

                    worksheet.Cells[row, col].Value = customer.GIAMDOC;
                    col++;

                    worksheet.Cells[row, col].Value = customer.FAX;
                    col++;

                    worksheet.Cells[row, col].Value = customer.MST;
                    col++;

                    worksheet.Cells[row, col].Value = customer.GHICHU != null ? HttpUtility.HtmlDecode(Regex.Replace(customer.GHICHU, "<.*?>", String.Empty)) : "";
                    col++;

                    worksheet.Cells[row, col].Value = customer.NGUOINHAP;
                    col++;

                    worksheet.Cells[row, col].Value = customer.NGAYNHAP;
                    col++;

                    row++;
                }
                row--;
                worksheet.Cells["A6:Y" + row].Style.Font.SetFromFont(new Font("Tahoma", 8));
                worksheet.Cells["A6:Y" + row].AutoFitColumns();
                xlPackage.Save();
            }
        }
        #endregion

        #region Check Trùng Dữ liệu

        // trùng họ tên
        [HttpPost]
        public ActionResult CheckFullname(string text)
        {
            if (text != null)
            {
                var cus = _customerRepository.GetAllAsQueryable().AsEnumerable()
                    .FirstOrDefault(p => p.FullName == text && p.IsDelete == false);
                if (cus != null)
                {
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        // trùng số điện thoại
        [HttpPost]
        public ActionResult CheckPhone(string text)
        {
            var cus = _customerRepository.GetAllAsQueryable().AsEnumerable()
                .FirstOrDefault(p => (p.Phone == text || p.MobilePhone == text) && p.IsDelete == false);
            if (cus != null)
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        // trùng Email
        [HttpPost]
        public ActionResult CheckEmail(string text)
        {
            var cus = _customerRepository.GetAllAsQueryable().AsEnumerable()
                .FirstOrDefault(p => (p.PersonalEmail == text || p.CompanyEmail == text) && p.IsDelete == false);
            if (cus != null)
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        // trùng CMND
        [HttpPost]
        public ActionResult CheckCMND(string text)
        {
            var cus = _customerRepository.GetAllAsQueryable().AsEnumerable()
                .FirstOrDefault(p => p.IdentityCard == text && p.IsDelete == false);
            if (cus != null)
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        // trùng Passport
        [HttpPost]
        public ActionResult CheckPassport(string text)
        {
            var cus = _customerRepository.GetAllAsQueryable().AsEnumerable()
                .FirstOrDefault(p => p.PassportCard == text && p.IsDelete == false);
            if (cus != null)
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        // trùng Visa
        [HttpPost]
        public ActionResult CheckVisa(string text)
        {
            var cus = _customerVisaRepository.GetAllAsQueryable().AsEnumerable()
                .FirstOrDefault(p => p.VisaNumber == text && p.IsDelete == false);
            if (cus != null)
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Chuyển quyền nhân viên
        public async Task<ActionResult> ChangeStaffManage(int id)
        {
            var model = await _customerRepository.GetById(id);
            return PartialView("_Partial_ChangeStaffManage", model);
        }

        [HttpPost]
        public ActionResult ChangeManager(tbl_Customer model)
        {
            var item = _db.tbl_Customer.Find(model.Id);
            item.StaffManager = model.StaffManager;
            _db.SaveChanges();

            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Excel Sample

        public void ExportExcelTemplateCustomer(MemoryStream stream, string templateFile, IDictionary<string, string> header = null)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream, new MemoryStream(System.IO.File.ReadAllBytes(templateFile))))
            {
                var ws = xlPackage.Workbook.Worksheets[1]; //first worksheet
                var valWs = xlPackage.Workbook.Worksheets.Add("Validation");
                valWs.Hidden = eWorkSheetHidden.VeryHidden;

                //ws.AddHeader(header);

                // danh xung
                var danhxung = LoadData.NameTypeList()
                    .Select(p => new ExportItem
                    {
                        Text = p.Name,
                        Value = p.Id
                    });
                var columnIndex = ws.GetColumnIndex(CustomerColumn.DANHXUNG.ToString());
                ws.AddListValidation(valWs, danhxung, columnIndex, "Lỗi", "Lỗi", "DANHXUNG", "DANHXUNGName");

                // loai khach
                var loaikhach = new List<ExportItem>();
                loaikhach.Add(new ExportItem { Text = "Cá nhân", Value = 1 });
                loaikhach.Add(new ExportItem { Text = "Doanh nghiệp", Value = 0 });
                columnIndex = ws.GetColumnIndex(CustomerColumn.LOAIKHACH.ToString());
                ws.AddListValidation(valWs, loaikhach, columnIndex, "Lỗi", "Lỗi", "LOAIKHACH", "LOAIKHACHName");

                // tinh tp
                var tinhthanh = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false && p.TypeTag == 5)
                    .Select(p => new ExportItem
                    {
                        Text = p.Tag,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(CustomerColumn.TINHTP.ToString());
                ws.AddListValidation(valWs, tinhthanh, columnIndex, "Lỗi", "Lỗi", "TINHTP", "TINHTPName");

                // quan huyen
                var quanhuyen = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false && p.TypeTag == 6)
                    .Select(p => new ExportItem
                    {
                        Text = p.Tag,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(CustomerColumn.QUANHUYEN.ToString());
                ws.AddListValidation(valWs, quanhuyen, columnIndex, "Lỗi", "Lỗi", "QUANHUYEN", "QUANHUYENName");

                // phuong xa
                var phuongxa = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false && p.TypeTag == 7)
                    .Select(p => new ExportItem
                    {
                        Text = p.Tag,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(CustomerColumn.PHUONGXA.ToString());
                ws.AddListValidation(valWs, phuongxa, columnIndex, "Lỗi", "Lỗi", "PHUONGXA", "PHUONGXAName");

                // nghe nghiep
                var nghenghiep = LoadData.CareerList()
                    .Select(p => new ExportItem
                    {
                        Text = p.Name,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(CustomerColumn.NGANHNGHE.ToString());
                ws.AddListValidation(valWs, nghenghiep, columnIndex, "Lỗi", "Lỗi", "NGANHNGHE", "NGANHNGHEName");


                // nguon đến
                var nguonden = LoadData.OriginList()
                    .Select(p => new ExportItem
                    {
                        Text = p.Name,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(CustomerColumn.NGUONDEN.ToString());
                ws.AddListValidation(valWs, nguonden, columnIndex, "Lỗi", "Lỗi", "NGUONDEN", "NGUONDENName");

                // nhom khach
                var nhomkhach = LoadData.CustomerGroupList()
                    .Select(p => new ExportItem
                    {
                        Text = p.Name,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(CustomerColumn.NHOMKHACH.ToString());
                ws.AddListValidation(valWs, nhomkhach, columnIndex, "Lỗi", "Lỗi", "NHOMKHACH", "NHOMKHACHName");

                xlPackage.Save();
            }
        }

        public ActionResult ExcelSample()
        {
            try
            {
                IDictionary<string, string> header = new Dictionary<string, string>();
                header.Add("TITLE", "DANH SÁCH KHÁCH HÀNG");

                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    string templateFile = System.Web.HttpContext.Current.Server.MapPath("~\\Upload\\ImportExport\\Import_KhachHangTOURDEMO.xlsx");
                    ExportExcelTemplateCustomer(stream, templateFile, header);
                    bytes = stream.ToArray();
                }

                string fileName = "Mau-import-khach-hang-TOURDEMO.xlsx";
                return File(bytes, "text/xls", fileName);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Export File Offline

        /// <summary>
        /// danh sách người liên hệ
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public tbl_CustomerContact CustomerContact(int id)
        {
            return _customerContactRepository.GetAllAsQueryable().AsEnumerable().FirstOrDefault(x => id != null && x.CustomerId == id && x.IsDelete);
        }

        /// <summary>
        /// Export file excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportFileOffline(FormCollection form)
        {
            var cusContact = _customerContactRepository.GetAllAsQueryable();
            var model = new List<ExportToOffline>();
            switch (Convert.ToInt32(form["kyhopdong"]))
            {
                case 0:
                    {
                        model = _customerRepository.GetAllAsQueryable().AsEnumerable().Where(p =>
                                   (form["nguonden"] == "0" ? p.OriginId == p.OriginId : p.OriginId == Convert.ToInt32(form["nguonden"]))
                                   & (form["nhomkh"] == "0" ? p.CustomerGroupId == p.CustomerGroupId : p.CustomerGroupId == Convert.ToInt32(form["nhomkh"]))
                                   & (form["loaikh"] == "2" ? p.CustomerType == p.CustomerType : Convert.ToInt32(p.CustomerType) == Convert.ToInt32(form["loaikh"]))
                                   & (form["tungay"] == form["denngay"] ? Convert.ToDateTime(form["tungay"]).ToString("dd-MM-yyyy") == p.CreatedDate.ToString("dd-MM-yyyy") : Convert.ToDateTime(form["tungay"]) <= p.CreatedDate && p.CreatedDate <= Convert.ToDateTime(form["denngay"]))
                                   & ((p.StaffId == maNV | maNV == 0) || (p.StaffManager == maNV | maNV == 0)
                                   || p.tbl_Tour.Where(x => (x.Permission != null && x.Permission.Contains(maNV.ToString())) || x.StaffId == maNV || x.CreateStaffId == maNV).ToList().Count() > 0)
                                   & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                                   & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                                   & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false) & (p.IsTemp == false))
                                   .OrderByDescending(p => p.CreatedDate)
                                   .Select(p => new ExportToOffline
                                   {
                                       MAKH = p.Code,
                                       QUYDANH = p.NameTypeId != null ? p.tbl_DictionaryNameType.Name : "",
                                       TENKH = p.FullName,
                                       CANHAN = p.CustomerType.Value == CustomerType.Personal ? "x" : "",
                                       DIDONG = p.MobilePhone,
                                       DIENTHOAI = p.Phone,
                                       FAX = p.Fax,
                                       SOCMND = p.IdentityCard,
                                       MASOTHUE = p.TaxCode,
                                       NGAYCAP = p.CreatedDateTaxCode != null ? string.Format("{0:dd/MM/yyyy}", p.CreatedDateTaxCode) : "",
                                       NOICAP = p.CreatedPlaceTaxCode != null ? p.CreatedPlaceTaxCode : "",
                                       EMAIL = p.PersonalEmail,
                                       WEBSITE = "",
                                       NGAYSINH = p.Birthday != null ? string.Format("{0:dd/MM/yyyy}", p.Birthday) : "",
                                       SOGPKD = "",
                                       NGAYTLDN = p.FoundingDate != null ? string.Format("{0:dd/MM/yyyy}", p.FoundingDate) : "",
                                       VONDIEULE = "",
                                       NOTOIDA = "",
                                       HANNO = "",
                                       SOTKNH = p.AccountNumber,
                                       NGANHANG = p.Bank,
                                       NGANHNGHE = p.CareerId != null ? p.tbl_DictionaryCareer.Name : "",
                                       DIACHI = p.Address,
                                       XAPHUONG = Tags(p.TagsId, 7) != null ? Tags(p.TagsId, 7).Tag : "",
                                       QUANHUYEN = Tags(p.TagsId, 6) != null ? Tags(p.TagsId, 6).Tag : "",
                                       TINHTHANH = Tags(p.TagsId, 5) != null ? Tags(p.TagsId, 5).Tag : "",
                                       GHICHU = p.Note,
                                       NGUONDEN = p.OriginId != null ? p.tbl_DictionaryOrigin.Name : "",
                                       NHOMKH = p.CustomerGroupId != null ? p.tbl_DictionaryCustomerGroup.Name : "",
                                       LOAIKH = "",
                                       QUYDANHNLH = CustomerContact(p.Id) != null && CustomerContact(p.Id).NameTypeId != null ? CustomerContact(p.Id).tbl_DictionaryNameType.Name : "",
                                       NGUOILIENHE = CustomerContact(p.Id) != null ? CustomerContact(p.Id).FullName : "",
                                       CHUCVUNLH = CustomerContact(p.Id) != null ? CustomerContact(p.Id).Position : "",
                                       DIDONGNLH = CustomerContact(p.Id) != null ? CustomerContact(p.Id).Mobile : "",
                                       EMAILNLH = CustomerContact(p.Id) != null ? CustomerContact(p.Id).Email : "",
                                       DIACHINLH = CustomerContact(p.Id) != null ? CustomerContact(p.Id).Address : ""
                                   }).ToList();
                        break;
                    }
                case 1:
                    {
                        var customer = _customerRepository.GetAllAsQueryable().AsEnumerable().Where(p =>
                                       (form["nguonden"] == "0" ? p.OriginId == p.OriginId : p.OriginId == Convert.ToInt32(form["nguonden"]))
                                       & (form["nhomkh"] == "0" ? p.CustomerGroupId == p.CustomerGroupId : p.CustomerGroupId == Convert.ToInt32(form["nhomkh"]))
                                       & (form["loaikh"] == "2" ? p.CustomerType == p.CustomerType : Convert.ToInt32(p.CustomerType) == Convert.ToInt32(form["loaikh"]))
                                       & (form["tungay"] == form["denngay"] ? Convert.ToDateTime(form["tungay"]).ToString("dd-MM-yyyy") == p.CreatedDate.ToString("dd-MM-yyyy") : Convert.ToDateTime(form["tungay"]) <= p.CreatedDate && p.CreatedDate <= Convert.ToDateTime(form["denngay"]))
                                       & (p.StaffId == maNV | maNV == 0)
                                       & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                                       & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                                       & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false) & (p.IsTemp == false))
                                       .OrderByDescending(p => p.CreatedDate).ToList();

                        model = (from cs in customer
                                 join ct in _contractRepository.GetAllAsQueryable().Where(p => p.IsDelete == false)
                                 on cs.Id equals ct.CustomerId
                                 select cs).Distinct().Select(p => new ExportToOffline
                                 {
                                     MAKH = p.Code,
                                     QUYDANH = p.NameTypeId != null ? p.tbl_DictionaryNameType.Name : "",
                                     TENKH = p.FullName,
                                     CANHAN = p.CustomerType.Value == CustomerType.Personal ? "x" : "",
                                     DIDONG = p.MobilePhone,
                                     DIENTHOAI = p.Phone,
                                     FAX = p.Fax,
                                     SOCMND = p.IdentityCard,
                                     MASOTHUE = p.TaxCode,
                                     NGAYCAP = p.CreatedDateTaxCode != null ? string.Format("{0:dd/MM/yyyy}", p.CreatedDateTaxCode) : "",
                                     NOICAP = p.CreatedPlaceTaxCode != null ? p.CreatedPlaceTaxCode : "",
                                     EMAIL = p.PersonalEmail,
                                     WEBSITE = "",
                                     NGAYSINH = p.Birthday != null ? string.Format("{0:dd/MM/yyyy}", p.Birthday) : "",
                                     SOGPKD = "",
                                     NGAYTLDN = p.FoundingDate != null ? string.Format("{0:dd/MM/yyyy}", p.FoundingDate) : "",
                                     VONDIEULE = "",
                                     NOTOIDA = "",
                                     HANNO = "",
                                     SOTKNH = p.AccountNumber,
                                     NGANHANG = p.Bank,
                                     NGANHNGHE = p.CareerId != null ? p.tbl_DictionaryCareer.Name : "",
                                     DIACHI = p.Address,
                                     XAPHUONG = Tags(p.TagsId, 7) != null ? Tags(p.TagsId, 7).Tag : "",
                                     QUANHUYEN = Tags(p.TagsId, 6) != null ? Tags(p.TagsId, 6).Tag : "",
                                     TINHTHANH = Tags(p.TagsId, 5) != null ? Tags(p.TagsId, 5).Tag : "",
                                     GHICHU = p.Note,
                                     NGUONDEN = p.OriginId != null ? p.tbl_DictionaryOrigin.Name : "",
                                     NHOMKH = p.CustomerGroupId != null ? p.tbl_DictionaryCustomerGroup.Name : "",
                                     LOAIKH = "",
                                     QUYDANHNLH = CustomerContact(p.Id) != null && CustomerContact(p.Id).NameTypeId != null ? CustomerContact(p.Id).tbl_DictionaryNameType.Name : "",
                                     NGUOILIENHE = CustomerContact(p.Id) != null ? CustomerContact(p.Id).FullName : "",
                                     CHUCVUNLH = CustomerContact(p.Id) != null ? CustomerContact(p.Id).Position : "",
                                     DIDONGNLH = CustomerContact(p.Id) != null ? CustomerContact(p.Id).Mobile : "",
                                     EMAILNLH = CustomerContact(p.Id) != null ? CustomerContact(p.Id).Email : "",
                                     DIACHINLH = CustomerContact(p.Id) != null ? CustomerContact(p.Id).Address : ""
                                 }).ToList();
                        break;
                    }
                case 2:
                    {
                        model = _customerRepository.GetAllAsQueryable().AsEnumerable().Where(p =>
                                   (form["nguonden"] == "0" ? p.OriginId == p.OriginId : p.OriginId == Convert.ToInt32(form["nguonden"]))
                                   & (form["nhomkh"] == "0" ? p.CustomerGroupId == p.CustomerGroupId : p.CustomerGroupId == Convert.ToInt32(form["nhomkh"]))
                                   & (form["loaikh"] == "2" ? p.CustomerType == p.CustomerType : Convert.ToInt32(p.CustomerType) == Convert.ToInt32(form["loaikh"]))
                                   & (form["tungay"] == form["denngay"] ? Convert.ToDateTime(form["tungay"]).ToString("dd-MM-yyyy") == p.CreatedDate.ToString("dd-MM-yyyy") : Convert.ToDateTime(form["tungay"]) <= p.CreatedDate && p.CreatedDate <= Convert.ToDateTime(form["denngay"]))
                                   & (p.StaffId == maNV | maNV == 0)
                                   & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                                   & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                                   & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false) & (p.IsTemp == false))
                                   .OrderByDescending(p => p.CreatedDate)
                                   .Where(p => !_contractRepository.GetAllAsQueryable().Any(c => c.CustomerId == p.Id))
                                   .Distinct().Select(p => new ExportToOffline
                                   {
                                       MAKH = p.Code,
                                       QUYDANH = p.NameTypeId != null ? p.tbl_DictionaryNameType.Name : "",
                                       TENKH = p.FullName,
                                       CANHAN = p.CustomerType.Value == CustomerType.Personal ? "x" : "",
                                       DIDONG = p.MobilePhone,
                                       DIENTHOAI = p.Phone,
                                       FAX = p.Fax,
                                       SOCMND = p.IdentityCard,
                                       MASOTHUE = p.TaxCode,
                                       NGAYCAP = p.CreatedDateTaxCode != null ? string.Format("{0:dd/MM/yyyy}", p.CreatedDateTaxCode) : "",
                                       NOICAP = p.CreatedPlaceTaxCode != null ? p.CreatedPlaceTaxCode : "",
                                       EMAIL = p.PersonalEmail,
                                       WEBSITE = "",
                                       NGAYSINH = p.Birthday != null ? string.Format("{0:dd/MM/yyyy}", p.Birthday) : "",
                                       SOGPKD = "",
                                       NGAYTLDN = p.FoundingDate != null ? string.Format("{0:dd/MM/yyyy}", p.FoundingDate) : "",
                                       VONDIEULE = "",
                                       NOTOIDA = "",
                                       HANNO = "",
                                       SOTKNH = p.AccountNumber,
                                       NGANHANG = p.Bank,
                                       NGANHNGHE = p.CareerId != null ? p.tbl_DictionaryCareer.Name : "",
                                       DIACHI = p.Address,
                                       XAPHUONG = Tags(p.TagsId, 7) != null ? Tags(p.TagsId, 7).Tag : "",
                                       QUANHUYEN = Tags(p.TagsId, 6) != null ? Tags(p.TagsId, 6).Tag : "",
                                       TINHTHANH = Tags(p.TagsId, 5) != null ? Tags(p.TagsId, 5).Tag : "",
                                       GHICHU = p.Note,
                                       NGUONDEN = p.OriginId != null ? p.tbl_DictionaryOrigin.Name : "",
                                       NHOMKH = p.CustomerGroupId != null ? p.tbl_DictionaryCustomerGroup.Name : "",
                                       LOAIKH = "",
                                       QUYDANHNLH = CustomerContact(p.Id) != null && CustomerContact(p.Id).NameTypeId != null ? CustomerContact(p.Id).tbl_DictionaryNameType.Name : "",
                                       NGUOILIENHE = CustomerContact(p.Id) != null ? CustomerContact(p.Id).FullName : "",
                                       CHUCVUNLH = CustomerContact(p.Id) != null ? CustomerContact(p.Id).Position : "",
                                       DIDONGNLH = CustomerContact(p.Id) != null ? CustomerContact(p.Id).Mobile : "",
                                       EMAILNLH = CustomerContact(p.Id) != null ? CustomerContact(p.Id).Email : "",
                                       DIACHINLH = CustomerContact(p.Id) != null ? CustomerContact(p.Id).Address : ""
                                   }).ToList();
                        break;
                    }
            }
            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    ExportCustomersToOfflineXlsx(stream, model);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "Danh sách khách hàng.xlsx");
            }
            catch (Exception)
            {

            }
            return RedirectToAction("Index");
        }

        public virtual void ExportCustomersToOfflineXlsx(Stream stream, IList<ExportToOffline> customers)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.Add("KhachHang");
                var properties = new[]
                    {
                        "MaKhachHang",
                        "QuyDanh",
                        "TenKhachHang",
                        "CaNhan",
                        "DiDong",
                        "DienThoai",
                        "Fax",
                        "SoCMND",
                        "MaSoThue",
                        "NgayCap",
                        "NoiCap",
                        "Email",
                        "Website",
                        "NgaySinh",
                        "SoGPKD",
                        "NgayTLDN",
                        "VonDieuLe",
                        "NoToiDa",
                        "HanNo",
                        "SoTKNH",
                        "NganHang",
                        "NganhNghe",
                        "DiaChi",
                        "Xa/Phuong",
                        "Quan/Huyen",
                        "Tinh/TP",
                        "GhiChu",
                        "NguonDen",
                        "NhomKH",
                        "LoaiKH",
                        "QuyDanhNLH",
                        "NguoiLienHe",
                        "ChucVuNLH",
                        "DiDongNLH",
                        "EmailNLH",
                        "DiaChiNLH"
                    };
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i];
                    worksheet.Cells[1, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(197, 217, 241));
                }
                int row = 2;
                foreach (var customer in customers)
                {
                    int col = 1;
                    worksheet.Cells[row, col].Value = customer.MAKH;
                    col++;
                    worksheet.Cells[row, col].Value = customer.QUYDANH;
                    col++;
                    worksheet.Cells[row, col].Value = customer.TENKH;
                    col++;
                    worksheet.Cells[row, col].Value = customer.CANHAN;
                    col++;
                    worksheet.Cells[row, col].Value = customer.DIDONG;
                    col++;
                    worksheet.Cells[row, col].Value = customer.DIENTHOAI;
                    col++;
                    worksheet.Cells[row, col].Value = customer.FAX;
                    col++;
                    worksheet.Cells[row, col].Value = customer.SOCMND;
                    col++;
                    worksheet.Cells[row, col].Value = customer.MASOTHUE;
                    col++;
                    worksheet.Cells[row, col].Value = customer.NGAYCAP;
                    col++;
                    worksheet.Cells[row, col].Value = customer.NOICAP;
                    col++;
                    worksheet.Cells[row, col].Value = customer.EMAIL;
                    col++;
                    worksheet.Cells[row, col].Value = customer.WEBSITE;
                    col++;
                    worksheet.Cells[row, col].Value = customer.NGAYSINH;
                    col++;
                    worksheet.Cells[row, col].Value = customer.SOGPKD;
                    col++;
                    worksheet.Cells[row, col].Value = customer.NGAYTLDN;
                    col++;
                    worksheet.Cells[row, col].Value = customer.VONDIEULE;
                    col++;
                    worksheet.Cells[row, col].Value = customer.NOTOIDA;
                    col++;
                    worksheet.Cells[row, col].Value = customer.HANNO;
                    col++;
                    worksheet.Cells[row, col].Value = customer.SOTKNH;
                    col++;
                    worksheet.Cells[row, col].Value = customer.NGANHANG;
                    col++;
                    worksheet.Cells[row, col].Value = customer.NGANHNGHE;
                    col++;
                    worksheet.Cells[row, col].Value = customer.DIACHI;
                    col++;
                    worksheet.Cells[row, col].Value = customer.XAPHUONG;
                    col++;
                    worksheet.Cells[row, col].Value = customer.QUANHUYEN;
                    col++;
                    worksheet.Cells[row, col].Value = customer.TINHTHANH;
                    col++;
                    worksheet.Cells[row, col].Value = customer.GHICHU != null ? HttpUtility.HtmlDecode(Regex.Replace(customer.GHICHU, "<.*?>", String.Empty)) : "";
                    col++;
                    worksheet.Cells[row, col].Value = customer.NGUONDEN;
                    col++;
                    worksheet.Cells[row, col].Value = customer.NHOMKH;
                    col++;
                    worksheet.Cells[row, col].Value = customer.LOAIKH;
                    col++;
                    worksheet.Cells[row, col].Value = customer.QUYDANHNLH;
                    col++;
                    worksheet.Cells[row, col].Value = customer.NGUOILIENHE;
                    col++;
                    worksheet.Cells[row, col].Value = customer.CHUCVUNLH;
                    col++;
                    worksheet.Cells[row, col].Value = customer.DIDONGNLH;
                    col++;
                    worksheet.Cells[row, col].Value = customer.EMAILNLH;
                    col++;
                    worksheet.Cells[row, col].Value = customer.DIACHINLH;
                    col++;
                    row++;
                }
                row--;
                worksheet.Cells["A1:AJ" + row].Style.Font.SetFromFont(new Font("Tahoma", 8));
                worksheet.Cells["A1:AJ" + row].AutoFitColumns();
                xlPackage.Save();
            }
        }
        #endregion

    }

}
