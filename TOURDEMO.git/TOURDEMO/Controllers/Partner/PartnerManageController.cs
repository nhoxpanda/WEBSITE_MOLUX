using CRM.Core;
using CRM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TOURDEMO.Models;
using System.Threading.Tasks;
using TOURDEMO.Utilities;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using CRM.Enum;
using System.Text.RegularExpressions;

namespace TOURDEMO.Controllers
{
    [Authorize]
    public class PartnerManageController : BaseController
    {
        //
        // GET: /PartnerManage/

        #region Init

        private IGenericRepository<tbl_Tags> _tagRepository;
        private IGenericRepository<tbl_Contract> _contractRepository;
        private IGenericRepository<tbl_PartnerNote> _partnerNoteRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_Partner> _partnerRepository;
        private IGenericRepository<tbl_ServicesPartner> _servicesPartnerRepository;
        private IGenericRepository<tbl_Bank> _bankRepository;
        private IGenericRepository<tbl_BankDetail> _bankDetailRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private DataContext _db;

        public PartnerManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_PartnerNote> partnerNoteRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_Tags> tagRepository,
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_ServicesPartner> servicesPartnerRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Bank> bankRepository,
            IGenericRepository<tbl_BankDetail> bankDetailRepository,
            IGenericRepository<tbl_Contract> contractRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._partnerNoteRepository = partnerNoteRepository;
            this._documentFileRepository = documentFileRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._partnerRepository = partnerRepository;
            this._servicesPartnerRepository = servicesPartnerRepository;
            this._tagRepository = tagRepository;
            this._staffRepository = staffRepository;
            this._bankRepository = bankRepository;
            this._bankDetailRepository = bankDetailRepository;
            this._contractRepository = contractRepository;
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

        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 16);
            return View();
        }

        [ChildActionOnly]
        public ActionResult _Partial_ListPartner()
        {
            Permission(clsPermission.GetUser().PermissionID, 16);

            if (SDBID == 6)
                return PartialView("_Partial_ListPartner", new List<PartnerViewModel>());
            var model = _partnerRepository.GetAllAsQueryable().AsEnumerable().Where(p => (p.StaffId == maNV | maNV == 0)
                    & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                    & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                    & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false))
                    .OrderByDescending(p => p.CreatedDate)
                    .Select(p => new PartnerViewModel
                    {
                        Code = p.Code,
                        Contact = p.StaffContact,
                        Country = p.tbl_TagsCountry == null ? "" : p.tbl_TagsCountry.Tag,
                        Email = p.Email,
                        Id = p.Id,
                        Name = p.Name,
                        Phone = p.Phone,
                        Tags = p.TagsLocationId == null && p.TagsLocationId == "" ? "" : LoadData.LocationTags(p.TagsLocationId),
                        QuyMo = p.QuyMoDoiTac,
                        KhuVuc = p.Outbound == true ? "Outbound" : "Nội địa"
                    });
            return PartialView("_Partial_ListPartner", model);
        }

        #endregion

        #region Filter
        [HttpPost]
        public ActionResult FilterService(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 16);

            if (SDBID == 6)
                return PartialView("_Partial_ListPartner", new List<PartnerViewModel>());
            var model = _partnerRepository.GetAllAsQueryable().AsEnumerable()
                            .Where(p => p.DictionaryId == (id == 0 ? p.DictionaryId : id) && (p.StaffId == maNV | maNV == 0)
                            & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                            & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                            & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false))
                            .OrderByDescending(p => p.CreatedDate)
                            .Select(p => new PartnerViewModel
                             {
                                 Code = p.Code,
                                 Contact = p.StaffContact,
                                 Country = p.tbl_TagsCountry == null ? "" : p.tbl_TagsCountry.Tag,
                                 Email = p.Email,
                                 Id = p.Id,
                                 Name = p.Name,
                                 Phone = p.Phone,
                                 Tags = p.TagsLocationId == null && p.TagsLocationId == "" ? "" : LoadData.LocationTags(p.TagsLocationId),
                                 QuyMo = p.QuyMoDoiTac,
                                 KhuVuc = p.Outbound == true ? "Outbound" : "Nội địa"
                             });
            return PartialView("_Partial_ListPartner", model);
        }

        [HttpPost]
        public ActionResult FilterTags(string tags)
        {
            Permission(clsPermission.GetUser().PermissionID, 16);

            if (SDBID == 6)
                return PartialView("_Partial_ListPartner", new List<PartnerViewModel>());
            var model = new List<PartnerViewModel>();
            foreach (var item in tags.ToString().Split(','))
            {
                model.AddRange(_partnerRepository.GetAllAsQueryable().AsEnumerable()
                    .Where(p => p.TagsLocationId != null && p.TagsLocationId.Split(',').Contains(item)
                    && (p.StaffId == maNV | maNV == 0)
                    & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                    & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                    & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false))
                    .OrderByDescending(p => p.CreatedDate)
                    .Select(p => new PartnerViewModel
                        {
                            Code = p.Code,
                            Contact = p.StaffContact,
                            Country = p.tbl_TagsCountry == null ? "" : p.tbl_TagsCountry.Tag,
                            Email = p.Email,
                            Id = p.Id,
                            Name = p.Name,
                            Phone = p.Phone,
                            Tags = p.TagsLocationId == null ? "" : LoadData.LocationTags(p.TagsLocationId),
                            QuyMo = p.QuyMoDoiTac,
                            KhuVuc = p.Outbound == true ? "Outbound" : "Nội địa"
                        }).Distinct().ToList());
            }
            return PartialView("_Partial_ListPartner", model);
        }
        #endregion

        #region Create
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(tbl_Partner model, FormCollection form, HttpPostedFileBase FileName)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 16);
                model.Code = GenerateCode.PartnerCode(model.DictionaryId);
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                if (form["TagsLocationId"] != null && form["TagsLocationId"] != "")
                {
                    model.TagsLocationId = form["TagsLocationId"].ToString();
                }
                model.StaffId = clsPermission.GetUser().StaffID;

                if (FileName != null)
                {
                   
                    string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                    String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    FileName.SaveAs(path);
                    model.BusinessLicense = newName;
                }
                if (await _partnerRepository.Create(model))
                {
                    UpdateHistory.SaveHistory(16, "Thêm mới đối tác, code: " + model.Code + " - " + model.Name,
                                null, //appointment
                                null, //contract
                                null, //customer
                                model.Id, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                null // ticket
                                );
                    
                    for (int i = 1; i <= Convert.ToInt32(form["countService"]); i++)
                    {
                        if (form["PartnerServiceName" + i] != "" && form["PartnerServiceName" + i] != null)
                        {
                            var sv = new tbl_ServicesPartner
                            {
                                Name = form["PartnerServiceName" + i].ToString(),
                                PartnerId = model.Id,
                                Price = form["PartnerServicePrice" + i] != "" ? Convert.ToDouble(form["PartnerServicePrice" + i]) : 0,
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now,
                                Note = form["PartnerServiceNote" + i].ToString(),
                                CurrencyId = Convert.ToInt32(form["PartnerServiceCurrency" + i].ToString()),
                            };
                            await _servicesPartnerRepository.Create(sv);
                        }
                    }
                    for (int i = 1; i <= Convert.ToInt32(form["countSTK"]); i++)
                    {
                        if (form["TenNganHang" + i] != "" && form["TenNganHang" + i] != null)
                        {
                            var bank = new tbl_Bank
                            {
                                Name = form["TenNganHang" + i].ToString(),
                                Account = form["TenNganHang" + i].ToString(),
                                CreateDate = DateTime.Now,
                                IsDelete = false,
                                Note = form["BankNote" + i].ToString()
                            };
                            await _bankRepository.Create(bank);
                            var bankDetail = new tbl_BankDetail
                            {
                                BankId=bank.Id,
                                CreateDate=DateTime.Now,
                                PartnerId= model.Id
                            };
                            await _bankDetailRepository.Create(bankDetail);
                        }
                    }
                }
            }
            catch(Exception ex) { var mes = ex.Message; }

            return RedirectToAction("Index");
        }

        
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateContactPartner(tbl_Contract model, FormCollection form, HttpPostedFileBase fileUpload)
        {
            Permission(clsPermission.GetUser().PermissionID, 20);
            try
            {
                var checkCode = _contractRepository.GetAllAsQueryable().FirstOrDefault(p => p.Code == model.Code && p.IsDelete == false);
                if (checkCode == null)
                {
                    var idPartner = int.Parse(Session["idPartner"].ToString());
                    model.PartnerId = idPartner;
                    model.CreatedDate = DateTime.Now;
                    model.ModifiedDate = DateTime.Now;
                    model.Permission = clsPermission.GetUser().StaffID.ToString();
                    model.DictionaryId = 28;
                    model.StaffId = clsPermission.GetUser().StaffID;
                    if (model.TongDuKien != null)
                    {
                        model.LoiNhuanDuKien = model.TotalPrice - model.TongDuKien;
                        model.CurrencyLNDK = model.CurrencyTDK;
                    }
                    if (form["TagsId"] != null && form["TagsId"] != "")
                    {
                        model.TagsId = form["TagsId"].ToString();
                    }
                    if (fileUpload != null)
                    {

                        string FileSize = Common.ConvertFileSize(fileUpload.ContentLength);
                        String newName = fileUpload.FileName.Insert(fileUpload.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                        String path = Server.MapPath("~/Upload/file/" + newName);
                        fileUpload.SaveAs(path);
                        model.FileName = newName;

                    }
                    if (await _contractRepository.Create(model))
                    {
                        UpdateHistory.SaveHistory(20, "Thêm mới hợp đồng: " + model.Code + " - " + model.Name,
                            null, //appointment
                            model.Id, //contract
                            model.CustomerId, //customer
                            null, //partner
                            null, //program
                            null, //task
                            model.TourId, //tour
                            null, //quotation
                            null, //document
                            null, //history
                            null // ticket
                            );




                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Index");
                }
            }
            catch { }

            return RedirectToAction("Index");
        }

        #endregion

        [HttpPost]
        public ActionResult UploadBussinessLicense(HttpPostedFileBase FileNameBussinessLicense)
        {
            if (FileNameBussinessLicense != null && FileNameBussinessLicense.ContentLength > 0)
            {
                Session["PartnerBussinessLicenseFile"] = FileNameBussinessLicense;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }


        #region Update
        //[ChildActionOnly]
        //public ActionResult _Partial_EditPartner()
        //{
        //    return PartialView("_Partial_EditPartner", new tbl_Partner());
        //}



        [HttpPost]
        public ActionResult PartnerInfomation(int id)
        {
            var model = _db.tbl_Partner.Find(id);
            ViewBag.Services = _servicesPartnerRepository.GetAllAsQueryable().Where(p => p.PartnerId == id && p.IsDelete == false).ToList();
            var listBankDetail=_bankDetailRepository.GetAllAsQueryable().Where(p=>p.PartnerId==id).ToList();
            var listBanks = new List<tbl_Bank>();
            foreach(var i in listBankDetail)
            {
                var bank = _bankRepository.GetAllAsQueryable().Where(p => p.Id == i.BankId).FirstOrDefault();
                if (bank != null)
                {
                    listBanks.Add(bank);
                }
                
            }
            ViewBag.Banks = listBanks;

            ViewBag.Services = _servicesPartnerRepository.GetAllAsQueryable().Where(p => p.PartnerId == id && p.IsDelete == false).ToList();
            return PartialView("_Partial_EditPartner", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tbl_Partner model, FormCollection form, HttpPostedFileBase FileName)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 16);
                model.ModifiedDate = DateTime.Now;
                if (form["TagsLocationId"] != null && form["TagsLocationId"] != "")
                {
                    model.TagsLocationId = form["TagsLocationId"].ToString();
                }
                if (FileName != null)
                {

                    string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                    String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    FileName.SaveAs(path);
                    model.BusinessLicense = newName;
                }
                if (await _partnerRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(16, "Cập nhật đối tác: " + model.Code, 
                        null, //appointment
                        null, //contract
                        null, //customer
                        model.Id, //partner
                        null, //program
                        null, //task
                        null, //tour
                        null, //quotation
                        null, //document
                        null, //history
                        null // ticket
                        );
                    // delete all service
                    var service = _servicesPartnerRepository.GetAllAsQueryable().Where(p => p.PartnerId == model.Id && p.IsDelete == false).ToList();
                    if (service.Count() > 0)
                    {
                        foreach (var item in service)
                        {
                            var listId = item.Id.ToString().Split(',').ToArray();
                            await _servicesPartnerRepository.DeleteMany(listId, false);
                        }
                    }

                    for (int i = 1; i <= Convert.ToInt32(form["countServiceE"].Split(',')[0].ToString()); i++)
                    {
                        if (form["PartnerServiceNameE" + i] != null && form["PartnerServiceNameE" + i] != "")
                        {
                            var xx = form["PartnerServiceNameE" + i].ToString();
                                var xx1 = model.Id;
                                var xx2 = form["PartnerServicePriceE" + i] != "" ? Convert.ToDouble(form["PartnerServicePriceE" + i]) : 0;
                                var xx3 = DateTime.Now;
                                var xx4 = DateTime.Now;
                                var xx5 = form["PartnerServiceNoteE" + i].ToString();
                                var xx6 = Convert.ToInt32(form["PartnerServiceCurrencyE" + i].ToString());

                            var sv = new tbl_ServicesPartner
                            {
                                Name = form["PartnerServiceNameE" + i].ToString(),
                                PartnerId = model.Id,
                                Price = form["PartnerServicePriceE" + i] != "" ? Convert.ToDouble(form["PartnerServicePriceE" + i]) : 0,
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now,
                                Note = form["PartnerServiceNoteE" + i].ToString(),
                                CurrencyId = Convert.ToInt32(form["PartnerServiceCurrencyE" + i].ToString())
                            };
                            await _servicesPartnerRepository.Create(sv);
                        }
                    }
                    for (int i = 1; i <= Convert.ToInt32(form["countSTK"]); i++)
                    {
                        if (form["TenNganHang" + i] != "" && form["TenNganHang" + i] != null)
                        {
                            var bank = new tbl_Bank
                            {
                                Name = form["TenNganHang" + i].ToString(),
                                Account = form["TenNganHang" + i].ToString(),
                                CreateDate = DateTime.Now,
                                IsDelete = false,
                                Note = form["BankNote" + i].ToString()
                            };
                            await _bankRepository.Create(bank);
                            var bankDetail = new tbl_BankDetail
                            {
                                BankId = bank.Id,
                                CreateDate = DateTime.Now,
                                PartnerId = model.Id
                            };
                            await _bankDetailRepository.Create(bankDetail);
                        }
                    }
                }
            }
            catch(Exception ex) { var mes = ex.Message; }

            return RedirectToAction("Index");
        }
        #endregion

        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(FormCollection fc)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 16);
                if (fc["listItemId"] != null && fc["listItemId"] != "")
                {
                    var listIds = fc["listItemId"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        //
                        foreach (var i in listIds)
                        {
                            var item = _partnerRepository.FindId(Convert.ToInt32(i));
                            UpdateHistory.SaveHistory(16, "Xóa đối tác: " + item.Code + " - " + item.Name,
                                null, //appointment
                                null, //contract
                                null, //customer
                                item.Id, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                null // ticket
                                );
                        }
                        //
                        if (await _partnerRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "PartnerManage") }, JsonRequestBehavior.AllowGet);
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
        /********** Quản lý tài liệu **********/

        [HttpPost]
        public ActionResult GetIdPartner(int id)
        {
            Session["idPartner"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase FileName)
        {
            if (FileName != null && FileName.ContentLength > 0)
            {
                Session["PartnerFile"] = FileName;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateDocument(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 62);
                string id = Session["idPartner"].ToString();
                if (ModelState.IsValid)
                {
                    model.Code = GenerateCode.DocumentCode();
                    model.PartnerId = Convert.ToInt32(id);
                    model.CreatedDate = DateTime.Now;
                    model.IsRead = false;
                    model.ModifiedDate = DateTime.Now;
                    if (form["TagsId"] != null && form["TagsId"] != "")
                    {
                        model.TagsId = form["TagsId"].ToString();
                    }
                    model.StaffId = clsPermission.GetUser().StaffID;

                    //file
                    HttpPostedFileBase FileName = Session["PartnerFile"] as HttpPostedFileBase;
                    string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                    String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
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
                        UpdateHistory.SaveHistory(62, "Thêm mới tài liệu đối tác, code: " + model.Code + " - " + model.FileName,
                                null, //appointment
                                null, //contract
                                null, //customer
                                model.PartnerId, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                model.Id, //document
                                null, //history
                                null // ticket
                                );

                        Session["PartnerFile"] = null;
                        //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.PartnerId.ToString() == id).ToList();
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.PartnerId.ToString() == id).Where(p => p.IsDelete == false)
                    .Select(p => new tbl_DocumentFile
                    {
                        Id = p.Id,
                        FileName = p.FileName,
                        FileSize = p.FileSize,
                        Note = p.Note,
                        CreatedDate = p.CreatedDate,
                        TagsId = p.TagsId,
                        tbl_Staff = _staffRepository.FindId(p.StaffId)
                    }).ToList();
                        return PartialView("~/Views/PartnerTabInfo/_TaiLieuMau.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/PartnerTabInfo/_TaiLieuMau.cshtml");
                    }
                }
            }
            catch { }
            return RedirectToAction("Index");
        }

        [ChildActionOnly]
        public ActionResult _Partial_EditDocument()
        {
            Permission(clsPermission.GetUser().PermissionID, 62);
            List<SelectListItem> lstTag = new List<SelectListItem>();
            List<SelectListItem> lstDictionary = new List<SelectListItem>();
            ViewData["TagsId"] = lstTag;
            ViewBag.DictionaryId = lstDictionary;
            return PartialView("_Partial_EditDocument", new tbl_DocumentFile());
        }

        [HttpPost]
        public async Task<ActionResult> EditInfoDocument(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 62);
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
                Permission(clsPermission.GetUser().PermissionID, 62);
                if (ModelState.IsValid)
                {
                    model.IsRead = true;
                    model.ModifiedDate = DateTime.Now;
                    if (form["TagsId"] != null && form["TagsId"] != "")
                    {
                        model.TagsId = form["TagsId"].ToString();
                    }
                    if (Session["PartnerFile"] != null)
                    {
                        //file
                        HttpPostedFileBase FileName = Session["PartnerFile"] as HttpPostedFileBase;
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
                        UpdateHistory.SaveHistory(62, "Cập nhật tài liệu của đối tác: " + model.Code,
                                null, //appointment
                                null, //contract
                                null, //customer
                                model.PartnerId, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                model.Id, //document
                                null, //history
                                null // ticket
                                );
                        Session["PartnerFile"] = null;
                        //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.PartnerId == model.PartnerId).ToList();
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.PartnerId == model.PartnerId).Where(p => p.IsDelete == false)
                    .Select(p => new tbl_DocumentFile
                    {
                        Id = p.Id,
                        FileName = p.FileName,
                        FileSize = p.FileSize,
                        Note = p.Note,
                        CreatedDate = p.CreatedDate,
                        TagsId = p.TagsId,
                        tbl_Staff = _staffRepository.FindId(p.StaffId)
                    }).ToList();
                        return PartialView("~/Views/PartnerTabInfo/_TaiLieuMau.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/PartnerTabInfo/_TaiLieuMau.cshtml");
                    }
                }
            }
            catch
            {
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteDocument(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 62);
                int partnerId = _documentFileRepository.FindId(id).PartnerId ?? 0;
                //file
                tbl_DocumentFile documentFile = _documentFileRepository.FindId(id) ?? new tbl_DocumentFile();
                String path = Server.MapPath("~/Upload/file/" + documentFile.FileName);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                //end file
                var listId = id.ToString().Split(',').ToArray();
                //
                var item = _documentFileRepository.FindId(id);
                UpdateHistory.SaveHistory(62, "Xóa tài liệu: " + item.Code,
                    null, //appointment
                    null, //contract
                    null, //customer
                    item.PartnerId, //partner
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
                    //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.CustomerId == partnerId).ToList();
                    var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.PartnerId == partnerId).Where(p => p.IsDelete == false)
                    .Select(p => new tbl_DocumentFile
                    {
                        Id = p.Id,
                        FileName = p.FileName,
                        FileSize = p.FileSize,
                        Note = p.Note,
                        CreatedDate = p.CreatedDate,
                        TagsId = p.TagsId,
                        tbl_Staff = _staffRepository.FindId(p.StaffId)
                    }).ToList();
                    return PartialView("~/Views/PartnerTabInfo/_TaiLieuMau.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/PartnerTabInfo/_TaiLieuMau.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/PartnerTabInfo/_TaiLieuMau.cshtml");
            }
        }

        #endregion

        #region Note
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateNote(tbl_PartnerNote model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 65);
                string id = Session["idPartner"].ToString();
                if (ModelState.IsValid)
                {
                    model.PartnerId = Convert.ToInt32(id);
                    model.CreatedDate = DateTime.Now;
                    model.StaffId = clsPermission.GetUser().StaffID;

                    if (await _partnerNoteRepository.Create(model))
                    {
                        UpdateHistory.SaveHistory(65, "Thêm ghi chú của đối tác " + _partnerRepository.FindId(model.PartnerId).Name,
                                null, //appointment
                                null, //contract
                                null, //customer
                                model.PartnerId, //partner
                                null, //program
                                null, //task
                                null, //tour
                                null, //quotation
                                null, //document
                                null, //history
                                null // ticket
                                );
                        var list = _db.tbl_PartnerNote.AsEnumerable().Where(p => p.PartnerId == model.PartnerId).Where(p => p.IsDelete == false).ToList();
                        return PartialView("~/Views/PartnerTabInfo/_GhiChu.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/PartnerTabInfo/_GhiChu.cshtml");
                    }
                }
            }
            catch { }
            return PartialView("~/Views/PartnerTabInfo/_GhiChu.cshtml");
        }

        [ChildActionOnly]
        public ActionResult _Partial_EditNote()
        {
            return PartialView("_Partial_EditNote", new tbl_PartnerNote());
        }

        [HttpPost]
        public async Task<ActionResult> EditInfoNote(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 65);
            var model = await _partnerNoteRepository.GetById(id);
            return PartialView("_Partial_EditNote", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateNote(tbl_PartnerNote model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 65);
                if (ModelState.IsValid)
                {
                    model.CreatedDate = DateTime.Now;

                    if (await _partnerNoteRepository.Update(model))
                    {
                        UpdateHistory.SaveHistory(65, "Cập nhật ghi chú của đối tác: " + model.tbl_Partner.Name,
                            null, //appointment
                            null, //contract
                            null, //customer
                            model.PartnerId, //partner
                            null, //program
                            null, //task
                            null, //tour
                            null, //quotation
                            null, //document
                            null, //history
                            null // ticket
                            );
                        var list = _db.tbl_PartnerNote.AsEnumerable().Where(p => p.PartnerId == model.PartnerId).Where(p => p.IsDelete == false).ToList();
                        return PartialView("~/Views/PartnerTabInfo/_GhiChu.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/PartnerTabInfo/_GhiChu.cshtml");
                    }
                }
            }
            catch { }
            return PartialView("~/Views/PartnerTabInfo/_GhiChu.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteNote(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 65);
                int partnerId = _partnerNoteRepository.FindId(id).PartnerId;
                var listId = id.ToString().Split(',').ToArray();
                //
                var item = _partnerNoteRepository.FindId(id);
                UpdateHistory.SaveHistory(65, "Xóa ghi chú của đối tác: " + _partnerRepository.FindId(partnerId).Name,
                    null, //appointment
                    null, //contract
                    null, //customer
                    item.PartnerId, //partner
                    null, //program
                    null, //task
                    null, //tour
                    null, //quotation
                    null, //document
                    null, //history
                    null // ticket
                    );
                //
                if (await _partnerNoteRepository.DeleteMany(listId, false))
                {
                    var list = _db.tbl_PartnerNote.AsEnumerable().Where(p => p.PartnerId == partnerId).Where(p => p.IsDelete == false).ToList();
                    return PartialView("~/Views/PartnerTabInfo/_GhiChu.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/PartnerTabInfo/_GhiChu.cshtml");
                }
            }

            catch { }
            return PartialView("~/Views/PartnerTabInfo/_GhiChu.cshtml");
        }
        #endregion

        #region Create Map

        [HttpPost]
        public ActionResult EditLocation(int id)
        {
            var model = _partnerRepository.FindId(id);
            return PartialView("_Partial_CreateMap", model);
        }

        public ActionResult CreateMap(tbl_Partner model)
        {
            var item = _db.tbl_Partner.Find(model.Id);
            item.xMap = model.xMap;
            item.yMap = model.yMap;
            item.AddressMap = model.AddressMap;
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
        #endregion

        #region GetCode
        public ActionResult GetCodePartner(int id)
        {
            return Json(GenerateCode.PartnerCode(id), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Import

        public void ExportExcelTemplateCustomer(MemoryStream stream, string templateFile, IDictionary<string, string> header = null)
        {
            Permission(clsPermission.GetUser().PermissionID, 16);
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream, new MemoryStream(System.IO.File.ReadAllBytes(templateFile))))
            {
                var ws = xlPackage.Workbook.Worksheets[1]; //first worksheet
                var valWs = xlPackage.Workbook.Worksheets.Add("Validation");
                valWs.Hidden = eWorkSheetHidden.VeryHidden;

                // Dịch vụ
                var dichvu = _dictionaryRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.DictionaryCategoryId == 13 && p.IsDelete == false)
                    .Select(p => new ExportItem
                    {
                        Text = p.Name,
                        Value = p.Id
                    });
                var columnIndex = ws.GetColumnIndex(PartnerColumn.DICHVU.ToString());
                ws.AddListValidation(valWs, dichvu, columnIndex, "Lỗi", "Lỗi", "DICHVU", "DICHVUName");

                // Tỉnh TP
                var tinhtp = _tagRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TypeTag == 5 && p.IsDelete == false)
                    .Select(p => new ExportItem
                    {
                        Text = p.Tag,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(PartnerColumn.TINHTP.ToString());
                ws.AddListValidation(valWs, tinhtp, columnIndex, "Lỗi", "Lỗi", "TINHTP", "TINHTPName");

                // Quận huyện
                var quanhuyen = _tagRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TypeTag == 6 && p.IsDelete == false)
                    .Select(p => new ExportItem
                    {
                        Text = p.Tag,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(PartnerColumn.QUANHUYEN.ToString());
                ws.AddListValidation(valWs, quanhuyen, columnIndex, "Lỗi", "Lỗi", "QUANHUYEN", "QUANHUYENName");

                // Phường xã
                var phuongxa = _tagRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TypeTag == 7 && p.IsDelete == false)
                    .Select(p => new ExportItem
                    {
                        Text = p.Tag,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(PartnerColumn.PHUONGXA.ToString());
                ws.AddListValidation(valWs, phuongxa, columnIndex, "Lỗi", "Lỗi", "PHUONGXA", "PHUONGXAName");

                // Quốc gia
                var quocgiavisa = _tagRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TypeTag == 3 && p.IsDelete == false)
                    .Select(p => new ExportItem
                    {
                        Text = p.Tag,
                        Value = p.Id
                    });
                columnIndex = ws.GetColumnIndex(PartnerColumn.QUOCGIA.ToString());
                ws.AddListValidation(valWs, quocgiavisa, columnIndex, "Lỗi", "Lỗi", "QUOCGIA", "QUOCGIAName");

                xlPackage.Save();
            }
        }

        public ActionResult ExcelSample()
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 16);
                IDictionary<string, string> header = new Dictionary<string, string>();
                header.Add("", "");
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    string templateFile = System.Web.HttpContext.Current.Server.MapPath("~\\Upload\\ImportExport\\Import_DoiTacTOURDEMO.xlsx");
                    ExportExcelTemplateCustomer(stream, templateFile, header);
                    bytes = stream.ToArray();
                }
                string fileName = "Mau-import-doi-tac.xlsx";
                return File(bytes, "text/xls", fileName);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult ImportFile(HttpPostedFileBase FileName)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 16);
                using (var excelPackage = new ExcelPackage(FileName.InputStream))
                {
                    var listPartner = new List<tbl_Partner>();
                    var model = new List<PartnerExportViewModel>();
                    var worksheet = excelPackage.Workbook.Worksheets[1];
                    var lastRow = worksheet.Dimension.End.Row;
                    for (int row = 7; row <= lastRow; row++)
                    {
                        string cel = "";
                        var dt = new tbl_Partner()
                        {
                            Code = worksheet.Cells["a" + row].Value != null ? worksheet.Cells["a" + row].Text : null,
                            Address = worksheet.Cells["f" + row].Value != null ? worksheet.Cells["f" + row].Text : null,
                            CreatedDate = DateTime.Now,
                            Email = worksheet.Cells["l" + row].Value != null ? worksheet.Cells["l" + row].Text : null,
                            IsDelete = false,
                            ModifiedDate = DateTime.Now,
                            Name = worksheet.Cells["c" + row].Value != null ? worksheet.Cells["c" + row].Text : null,
                            Phone = worksheet.Cells["k" + row].Value != null ? worksheet.Cells["k" + row].Text : null,
                            QuyMoDoiTac = worksheet.Cells["d" + row].Value != null ? worksheet.Cells["d" + row].Text : null,
                            StaffContact = worksheet.Cells["j" + row].Value != null ? worksheet.Cells["j" + row].Text : null,
                            StaffId = clsPermission.GetUser().StaffID,
                        };
                        try//dịch vụ
                        {
                            cel = "b";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string dichvu = worksheet.Cells[cel + row].Text;
                                dt.DictionaryId = _dictionaryRepository.GetAllAsQueryable().AsEnumerable().SingleOrDefault(c => c.Name == dichvu && c.IsDelete == false).Id;
                                dt.Code = GenerateCode.PartnerCode(dt.DictionaryId);
                            }
                        }
                        catch { }
                        try//tagid địa chỉ
                        {
                            cel = "f";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string tinhtp = worksheet.Cells[cel + row].Text;
                                dt.TagsLocationId = _tagRepository.GetAllAsQueryable().AsEnumerable().SingleOrDefault(c => c.Tag == tinhtp && c.TypeTag == 5 && c.IsDelete == false).Id.ToString();
                            }
                            cel = "g";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string quanhuyen = worksheet.Cells[cel + row].Text;
                                var tagid = _tagRepository.GetAllAsQueryable().AsEnumerable().FirstOrDefault(c => c.Tag == quanhuyen && c.TypeTag == 6 && c.IsDelete == false);
                                if (tagid != null)
                                    if (dt.TagsLocationId != null)
                                        dt.TagsLocationId += "," + tagid.Id;
                                    else
                                        dt.TagsLocationId = tagid.Id.ToString();
                            }
                            cel = "h";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string phuongxa = worksheet.Cells[cel + row].Text;
                                var tagid = _tagRepository.GetAllAsQueryable().AsEnumerable().FirstOrDefault(c => c.Tag == phuongxa && c.TypeTag == 7 && c.IsDelete == false);
                                if (tagid != null)
                                    if (dt.TagsLocationId != null)
                                        dt.TagsLocationId += "," + tagid.Id;
                                    else
                                        dt.TagsLocationId = tagid.Id.ToString();
                            }
                        }
                        catch { }
                        try//tag id
                        {
                            cel = "e";
                            if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                            {
                                string tag = worksheet.Cells[cel + row].Text;
                                dt.CountryId = _tagRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == tag && c.TypeTag == 3 && c.IsDelete == false).Select(c => c.Id).SingleOrDefault();
                            }
                        }
                        catch { }
                        // 
                        var item = new PartnerExportViewModel()
                        {
                            Code = dt.Code,
                            DiaDiem = dt.Address + "," + LoadData.LocationTags(dt.TagsLocationId),
                            DienThoai = dt.Phone,
                            Email = dt.Email,
                            NguoiLienHe = dt.StaffContact,
                            QuocGia = dt.CountryId != null ? _tagRepository.FindId(dt.CountryId).Tag : "",
                            QuyMo = dt.QuyMoDoiTac,
                            TenDoiTac = dt.Name,
                            DichVu = dt.DictionaryId != null ? _dictionaryRepository.FindId(dt.DictionaryId).Name : ""
                        };

                        model.Add(item);
                        listPartner.Add(dt);
                    }
                    Session["listPartnerImport"] = listPartner;
                    return PartialView("_Partial_ImportDataList", model);
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
            Permission(clsPermission.GetUser().PermissionID, 16);
            try
            {
                List<tbl_Partner> list = Session["listPartnerImport"] as List<tbl_Partner>;
                if (id == 1) // cap nhat
                {
                    foreach (var item in list)
                    {
                        var check = _db.tbl_Partner.FirstOrDefault(p => p.Code == item.Code || p.Name == item.Name);
                        if (check != null)
                        {
                            // đã có đối tác này --> cập nhật
                            item.Id = check.Id;
                            item.IsDelete = false;
                            item.CreatedDate = check.CreatedDate;
                            item.ModifiedDate = DateTime.Now;
                            await _partnerRepository.Update(item);
                        }
                    }
                }
                else // them moi
                {
                    foreach (var item in list)
                    {
                        item.Code = item.DictionaryId != null ? GenerateCode.PartnerCode(item.DictionaryId) : GenerateCode.PartnerCode(0);
                        await _partnerRepository.Create(item);
                    }
                }
                Session["listPartnerImport"] = null;
            }
            catch
            {
                Session["listPartnerImport"] = null;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteImport(String listItemId)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 16);
                List<tbl_Partner> list = Session["listPartnerImport"] as List<tbl_Partner>;
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
                Session["listPartnerImport"] = list;
                var model = new List<PartnerExportViewModel>();
                foreach (var dt in list)
                {
                    var item = new PartnerExportViewModel()
                    {
                        Code = dt.Code,
                        DiaDiem = dt.Address + "," + LoadData.LocationTags(dt.TagsLocationId),
                        DienThoai = dt.Phone,
                        Email = dt.Email,
                        NguoiLienHe = dt.StaffContact,
                        QuocGia = dt.CountryId != null ? _tagRepository.FindId(dt.CountryId).Tag : "",
                        QuyMo = dt.QuyMoDoiTac,
                        TenDoiTac = dt.Name,
                        DichVu = dt.DictionaryId != null ? _dictionaryRepository.FindId(dt.DictionaryId).Name : ""
                    };
                    model.Add(item);
                }
                return PartialView("_Partial_ImportDataList", model);
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
            return _tagRepository.GetAllAsQueryable().AsEnumerable().FirstOrDefault(x => ids != null && ids.Contains(x.Id.ToString()) && x.TypeTag == type);
        }

        /// <summary>
        /// Export file excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportFile(FormCollection form)
        {
            byte[] bytes;
            try
            {
                int id = form["idService"] == "0" ? 0 : Convert.ToInt32(form["idService"]);
                var model = _partnerRepository.GetAllAsQueryable().AsEnumerable()
                                  .Where(p => p.DictionaryId == (id == 0 ? p.DictionaryId : id) && (p.StaffId == maNV | maNV == 0)
                                            & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                                            & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                                            & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false))
                                  .OrderByDescending(p => p.CreatedDate)
                                  .Select(p => new PartnerExportNew
                                  {
                                      CODE = p.Code,
                                      DICHVU = p.DictionaryId != null ? p.tbl_Dictionary.Name : "",
                                      TENDOITAC = p.Name,
                                      QUYMO = p.QuyMoDoiTac != null ? p.QuyMoDoiTac : "",
                                      QUOCGIA = Tags(p.CountryId.ToString(), 7) != null ? Tags(p.CountryId.ToString(), 3).Tag : "",
                                      DIACHI = p.Address,
                                      PHUONGXA = Tags(p.TagsLocationId, 7) != null ? Tags(p.TagsLocationId, 7).Tag : "",
                                      QUANHUYEN = Tags(p.TagsLocationId, 6) != null ? Tags(p.TagsLocationId, 6).Tag : "",
                                      TINHTP = Tags(p.TagsLocationId, 5) != null ? Tags(p.TagsLocationId, 5).Tag : "",
                                      NGUOILIENHE = p.StaffContact,
                                      DIENTHOAI = p.Phone,
                                      EMAIL = p.Email,
                                      NGAYNHAP = p.ModifiedDate.ToString("dd/MM/yyyy"),
                                      NGUOINHAP = p.tbl_Staff.FullName
                                  }).ToList();
                using (var stream = new MemoryStream())
                {
                    ExportPartnerToXlsx(stream, model);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "Danh sách đối tác " + (id == 0 ? "" : _dictionaryRepository.FindId(id).Name) + ".xlsx");
            }
            catch (Exception)
            {

            }
            return RedirectToAction("Index");
        }

        public virtual void ExportPartnerToXlsx(Stream stream, IList<PartnerExportNew> partner)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.Add("Đối tác");
                var properties = new[]
                    {
                        "CODE",
                        "DỊCH VỤ",
                        "TÊN ĐỐI TÁC",
                        "QUY MÔ",
                        "QUỐC GIA",
                        "ĐỊA CHỈ",
                        "TỈNH TP",
                        "QUẬN HUYỆN",
                        "PHƯỜNG XÃ",
                        "NGƯỜI LIÊN HỆ",
                        "ĐIỆN THOẠI",
                        "EMAIL",
                        "NGƯỜI NHẬP",
                        "NGÀY NHẬP"
                    };

                worksheet.Cells["A3:W4"].Value = "DANH SÁCH ĐỐI TÁC";
                worksheet.Cells["A3:W4"].Style.Font.SetFromFont(new Font("Tahoma", 16));
                worksheet.Cells["A3:W4"].Style.Font.Bold = true;
                worksheet.Cells["A3:W4"].Merge = true;
                worksheet.Cells["A3:W4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A3:W4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells["A3:W4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A3:W4"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));

                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[6, i + 1].Value = properties[i];
                    worksheet.Cells[6, i + 1].Style.Font.SetFromFont(new Font("Tahoma", 8));
                    worksheet.Cells[6, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[6, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[6, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(191, 191, 191));
                }

                int row = 7;
                foreach (var v in partner)
                {
                    int col = 1;
                    worksheet.Cells[row, col].Value = v.CODE;
                    col++;
                    worksheet.Cells[row, col].Value = v.DICHVU;
                    col++;
                    worksheet.Cells[row, col].Value = v.TENDOITAC;
                    col++;
                    worksheet.Cells[row, col].Value = Regex.Replace(v.QUYMO, "<.*?>", String.Empty);
                    col++;
                    worksheet.Cells[row, col].Value = v.QUOCGIA;
                    col++;
                    worksheet.Cells[row, col].Value = v.DIACHI;
                    col++;
                    worksheet.Cells[row, col].Value = v.TINHTP;
                    col++;
                    worksheet.Cells[row, col].Value = v.QUANHUYEN;
                    col++;
                    worksheet.Cells[row, col].Value = v.PHUONGXA;
                    col++;
                    worksheet.Cells[row, col].Value = v.NGUOILIENHE;
                    col++;
                    worksheet.Cells[row, col].Value = v.DIENTHOAI;
                    col++;
                    worksheet.Cells[row, col].Value = v.EMAIL;
                    col++;
                    worksheet.Cells[row, col].Value = v.NGUOINHAP;
                    col++;
                    worksheet.Cells[row, col].Value = v.NGAYNHAP;
                    col++;
                    row++;
                }
                row--;
                worksheet.Cells["A6:W" + row].Style.Font.SetFromFont(new Font("Tahoma", 8));
                worksheet.Cells["A6:W" + row].AutoFitColumns();
                worksheet.Cells["A6:W6"].Style.Font.Bold = true;
                xlPackage.Save();
            }
        }
        #endregion
        
    }
}