using CRM.Core;
using CRM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TOURDEMO.Models;
using TOURDEMO.Utilities;

namespace TOURDEMO.Controllers.Marketing
{
    [Authorize]
    public class MailAutoSendController : BaseController
    {
        // GET: MailAutoSend

        #region Init
        private IGenericRepository<tbl_MailSending> _mailSendingRepository;
        private IGenericRepository<tbl_MailSendingList> _mailSendingListRepository;
        private IGenericRepository<tbl_MailSendingReceives> _mailSendingReceivesRepository;
        private IGenericRepository<tbl_MailReceiveList> _mailReceiveListRepository;
        private IGenericRepository<tbl_MailReceives> _mailReceivesRepository;
        private IGenericRepository<tbl_MailTemplates> _mailTemplatesRepository;
        private IGenericRepository<tbl_MailConfig> _mailConfigRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private DataContext _db;

        public MailAutoSendController(
            IGenericRepository<tbl_MailSendingReceives> mailSendingReceivesRepository,
            IGenericRepository<tbl_MailSending> mailSendingRepository,
            IGenericRepository<tbl_MailSendingList> mailSendingListRepository,
            IGenericRepository<tbl_MailReceiveList> mailReceiveListRepository,
            IGenericRepository<tbl_MailReceives> mailReceivesRepository,
            IGenericRepository<tbl_MailTemplates> mailTemplatesRepository,
            IGenericRepository<tbl_MailConfig> mailConfigRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._mailSendingReceivesRepository = mailSendingReceivesRepository;
            this._mailSendingRepository = mailSendingRepository;
            this._mailSendingListRepository = mailSendingListRepository;
            this._mailReceiveListRepository = mailReceiveListRepository;
            this._mailReceivesRepository = mailReceivesRepository;
            this._mailTemplatesRepository = mailTemplatesRepository;
            this._mailConfigRepository = mailConfigRepository;
            this._staffRepository = staffRepository;
            _db = new DataContext();
        }
        #endregion

        #region List & Permission

        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 1113);
            var model = _mailSendingRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false)
                .OrderByDescending(p => p.CreateDate).ToList();
            return View(model);
        }

        int SDBID = 6;
        int maPB = 0, maNKD = 0, maNV = 0, maCN = 0;
        void Permission(int PermissionsId, int formId)
        {
            var list = _db.tbl_ActionData.Where(p => p.FormId == formId && p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);
            ViewBag.IsSendMail = list.Contains(9);

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

        [HttpPost]
        public ActionResult GetIdAutoSend(int id)
        {
            Session["idAutoSend"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Create

        [HttpPost]
        public ActionResult LoadListReceive()
        {
            Permission(clsPermission.GetUser().PermissionID, 1113);
            var model = _mailReceivesRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.IsDelete == false).OrderByDescending(p => p.CreateDate).ToList();
            return PartialView("_Partial_ListReceive", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsertReceive(FormCollection fc)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1113);
                var listReceive = new List<tbl_MailReceives>();
                if (Session["listReceive"] != null)
                {
                    listReceive = Session["listReceive"] as List<tbl_MailReceives>;
                }
                if (fc["listItemIdAdd"] != null && fc["listItemIdAdd"] != "")
                {
                    var listIds = fc["listItemIdAdd"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        foreach (var id in listIds)
                        {
                            listReceive.Add(_mailReceivesRepository.FindId(Int16.Parse(id)));
                        }
                        Session["listReceive"] = listReceive;
                    }
                }
                return PartialView("_Partial_InsertReceiveMail", listReceive);
            }
            catch
            {
                return PartialView("_Partial_InsertReceiveMail");
            }
        }

        [ValidateInput(false)]
        public async Task<ActionResult> Create(tbl_MailSending model, FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 1113);
            if (Request["btnLuu"] != null)
            {
                // save tbl_MailSending
                model.Active = true;
                model.CreateDate = DateTime.Now;
                model.IsDelete = false;
                model.StaffId = clsPermission.GetUser().StaffID;
                model.Content = _mailTemplatesRepository.FindId(model.MailTemplateId).Content;

                await _mailSendingRepository.Create(model);

                if (Session["listReceive"] != null)
                {
                    var listReceive = Session["listReceive"] as List<tbl_MailReceives>;
                    foreach (var item in listReceive)
                    {
                        // save tbl_MailSendingReceives
                        var sr = new tbl_MailSendingReceives()
                        {
                            ReceiveId = item.Id,
                            SendingId = model.Id
                        };
                        await _mailSendingReceivesRepository.Create(sr);
                        Session["listReceive"] = null;
                        // save tbl_MailSendingList
                        var list = _mailReceiveListRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.MailReceiveId == item.Id && p.IsDelete == false).ToList();
                        if (list.Count() > 0)
                        {
                            foreach (var l in list)
                            {
                                var sm = new tbl_MailSendingList()
                                              {
                                                  Birthday = l.Birthday,
                                                  Department = l.Department,
                                                  Email = l.Email,
                                                  FullName = l.FullName,
                                                  HomeAddress = l.HomeAddress,
                                                  IsDelete = false,
                                                  JobTitle = l.JobTitte,
                                                  MailSendingId = model.Id,
                                                  Phone = l.Phone,
                                                  Vocative = l.Vocative,
                                                  ListId = l.MailReceiveId
                                              };
                                await _mailSendingListRepository.Create(sm);
                            }
                        }
                    }
                }

                return RedirectToAction("Index");
            }
            ViewBag.MailTemplate = _mailTemplatesRepository.GetAllAsQueryable().Where(p => p.IsDelete == false);
            return View();
        }

        #endregion

        #region Update

        [HttpPost]
        public ActionResult LoadListReceiveUpdate(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1113);
            var model = _mailReceivesRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false).ToList();
            return PartialView("_Partial_ListReceive", model);
        }

        public async Task<ActionResult> Update(int id)
        {
            Session["listReceive"] = null;
            Permission(clsPermission.GetUser().PermissionID, 1113);
            var model = await _mailSendingRepository.GetById(id);
            var listReceives = _mailSendingReceivesRepository.GetAllAsQueryable().Where(p => p.SendingId == id).Select(p => p.tbl_MailReceives).ToList();
            ViewBag.MailReceiveList = listReceives;
            ViewBag.MailTemplate = _mailTemplatesRepository.GetAllAsQueryable().Where(p => p.IsDelete == false).ToList();
            Session["listReceive"] = listReceives;
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        public async Task<ActionResult> Update(tbl_MailSending model)
        {
            Permission(clsPermission.GetUser().PermissionID, 1113);

            if (Request["btnLuu"] != null)
            {
                model.Active = true;
                model.IsDelete = false;
                model.StaffId = clsPermission.GetUser().StaffID;
                model.Content = _mailTemplatesRepository.FindId(model.MailTemplateId).Content;
                if (await _mailSendingRepository.Update(model))
                {
                    UpdateHistory.SaveHistory(1113, "Cập nhật gửi mail: " + model.Title,
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
                }

                if (Session["listReceive"] != null)
                {
                    var listReceive = Session["listReceive"] as List<tbl_MailReceives>;
                    foreach (var i in listReceive)
                    {
                        var check = _mailSendingReceivesRepository.GetAllAsQueryable().FirstOrDefault(p => p.ReceiveId == i.Id && p.SendingId == model.Id);
                        if (check != null)
                        {
                            // xóa mailSendingReceive
                            await _mailSendingReceivesRepository.DeleteMany(check.Id.ToString().Split(','), true);
                        }
                        // save tbl_MailSendingReceives
                        var sr = new tbl_MailSendingReceives()
                        {
                            ReceiveId = i.Id,
                            SendingId = model.Id
                        };
                        await _mailSendingReceivesRepository.Create(sr);
                        Session["listReceive"] = null;
                        // save tbl_MailSendingList
                        var list = _mailReceiveListRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.MailReceiveId == i.Id && p.IsDelete == false).ToList();
                        if (list.Count() > 0)
                        {
                            foreach (var l in list)
                            {
                                var checkReceive = _mailSendingListRepository.GetAllAsQueryable().FirstOrDefault(p => p.ListId == l.MailReceiveId && p.MailSendingId == model.Id && p.IsDelete == false);
                                if (checkReceive == null)
                                {
                                    var sm = new tbl_MailSendingList()
                                    {
                                        Birthday = l.Birthday,
                                        Department = l.Department,
                                        Email = l.Email,
                                        FullName = l.FullName,
                                        HomeAddress = l.HomeAddress,
                                        IsDelete = false,
                                        JobTitle = l.JobTitte,
                                        MailSendingId = model.Id,
                                        Phone = l.Phone,
                                        Vocative = l.Vocative,
                                        ListId = l.MailReceiveId
                                    };
                                    await _mailSendingListRepository.Create(sm);
                                }
                            }
                        }
                    }
                }
                return RedirectToAction("Index");
            }

            return View(model);
        }

        #endregion

        #region Delete

        [HttpPost]
        public ActionResult DeleteReceive(int stt, int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1113);
            if (Session["listReceive"] != null)
            {
                var listReceive = Session["listReceive"] as List<tbl_MailReceives>;
                listReceive.RemoveAt(stt - 1);
                Session["listReceive"] = listReceive;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteReceiveInUpdate(int stt, int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1113);
            if (Session["listReceive"] != null)
            {
                var listReceive = Session["listReceive"] as List<tbl_MailReceives>;
                listReceive.RemoveAt(stt - 1);
                Session["listReceive"] = listReceive;
                // xóa sendingreceive 
                var sr = _mailSendingReceivesRepository.GetAllAsQueryable().AsEnumerable().FirstOrDefault(p => p.SendingId.ToString() == Session["idAutoSend"].ToString() && p.ReceiveId == id);
                if (sr != null)
                {
                    await _mailSendingReceivesRepository.DeleteMany(sr.Id.ToString().Split(',').ToArray(), true);
                }
                // xóa sendinglist nếu có
                var check = _mailSendingListRepository.GetAllAsQueryable().Where(p => p.ListId == id).ToList();
                if (check.Count() > 0)
                {
                    foreach (var c in check)
                    {
                        await _mailSendingListRepository.DeleteMany(c.Id.ToString().Split(',').ToArray(), true);
                    }
                }

            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveFromList(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1113);
            var item = _db.tbl_MailSendingList.Find(id);
            _db.tbl_MailSendingList.Remove(item);
            _db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(FormCollection fc)
        {
            Permission(clsPermission.GetUser().PermissionID, 1113);
            try
            {
                if (fc["listItemId"] != null && fc["listItemId"] != "")
                {
                    var listIds = fc["listItemId"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    ////
                    foreach (var i in listIds)
                    {
                        var mail = _mailSendingRepository.FindId(Convert.ToInt32(i));
                        UpdateHistory.SaveHistory(1113, "Xóa danh sách gửi mail: " + mail.Title,
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
                    }
                    if (listIds.Count() > 0)
                    {
                        if (await _mailSendingRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "MailAutoSend") }, JsonRequestBehavior.AllowGet);
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

        #region SendMail

        public ActionResult LoadingSendMail()
        {
            return View();
        }

        public async Task<ActionResult> SendMail(int id)
        {
            try
            {
                var items = _mailSendingRepository.FindId(id);
                // mail gửi
                var mailconfig = items.tbl_MailConfig;
                // mail nhận
                var sendingMail = _mailSendingListRepository.GetAllAsQueryable().Where(p => p.IsDelete == false && p.MailSendingId == id).ToList();
                if (sendingMail.Count() > 0)
                {
                    var message = new MailMessage();
                    foreach (var m in sendingMail)
                    {
                        message.To.Add(new MailAddress(m.Email));  // replace with valid value    
                    }
                    message.From = new MailAddress(mailconfig.Email);  // replace with valid value
                    message.Subject = "[TOURDEMO] " + items.Title;
                    message.Body = items.Content;
                    message.IsBodyHtml = true;

                    //SendEmail.Email_With_CCandBCC(mailconfig.Email, Common.GiaiMa(mailconfig.Password), "voaiduy261291@gmail.com", "demo", "demo");

                    using (var smtp = new SmtpClient())
                    {
                        var credential = new NetworkCredential
                        {
                            UserName = mailconfig.Email,  // replace with valid value
                            Password = Common.GiaiMa(mailconfig.Password)  // replace with valid value
                        };
                        smtp.Host = mailconfig.Server;
                        smtp.Port = mailconfig.Port ?? 0;
                        smtp.EnableSsl = true;
                        smtp.Credentials = credential;

                        await smtp.SendMailAsync(message);
                    }
                }

                return Json(JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        #endregion
    }
}