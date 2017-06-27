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
    public class AirlineManageController : BaseController
    {

        private DataContext _db;
        private IGenericRepository<tbl_Airline> _airlineRepository;
        private IGenericRepository<tbl_AirlineTicket> _airlineTicketRepository;
        public AirlineManageController(
            IGenericRepository<tbl_Airline> airlineRepository,
            IGenericRepository<tbl_AirlineTicket> airlineTicketRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._airlineRepository = airlineRepository;
            _airlineTicketRepository = airlineTicketRepository;
            _db = new DataContext();
        }


        
        void Permission(int PermissionsId, int formId)
        {
            var list = _db.tbl_ActionData.Where(p => p.FormId == formId && p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);
            ViewBag.IsImport = list.Contains(4);
            ViewBag.IsExport = list.Contains(5);
        }
        // GET: AirlineManage
        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 1138);
            var airlineList = _airlineRepository.GetAllAsQueryable().Where(k => k.IsDelete == false)
                .Select(p => new AirlineViewModel()
                {
                    Id = p.Id,
                    Phone = p.Phone,
                    Address = p.Address,
                    ContactName = p.ContactName,
                    CreateDate = p.CreateDate,
                    Email = p.Email,
                    IsDelete = p.IsDelete,
                    Logo = p.Logo,
                    Mobile = p.Mobile,
                    Name = p.Name,
                    Note = p.Note,
                    ShortName = p.ShortName,
                    StaffId = p.StaffId ?? 0
                }).ToList();
            
            return View(airlineList);
        }
        [HttpPost]
        public ActionResult GetIdAirline(int id)
        {
            Session["idAirlineTicket"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateAirlineTicket(tbl_AirlineTicket model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 1138);
                string id = Session["idAirlineTicket"].ToString();
               
                if (ModelState.IsValid)
                {
                    model.AirlineId = int.Parse(id);
                    model.CreateDate = DateTime.Now;
                    model.IsDelete = false;
                    if (Session["FileUploadTicket"] != null)
                    {
                        HttpPostedFileBase Image = (HttpPostedFileBase)Session["FileUploadTicket"];
                        string FileSize = Common.ConvertFileSize(Image.ContentLength);
                        String newName = Image.FileName.Insert(Image.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                        String path = Server.MapPath("~/Upload/file/" + newName);
                        Image.SaveAs(path);
                        model.FileName = path;
                        //end file
                    }

                    if (await _airlineTicketRepository.Create(model))
                    {
                        
                    }
                }
                var list = _db.tbl_AirlineTicket.AsEnumerable()
                            .Where(p => p.IsDelete == false).Where(p => p.AirlineId.ToString() == id)
                            .OrderByDescending(p => p.CreateDate).ToList();
                return PartialView("_Partial_Detail", list);
            }
            catch { }
            return PartialView("_Partial_Detail");
        }
        public ActionResult Detail(int? id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1138);
            var ticket = _airlineTicketRepository.GetAllAsQueryable().Where(p => p.AirlineId == id).Where(x=>x.IsDelete!=true).OrderByDescending(p => p.CreateDate).ToList();
            return PartialView("_Partial_Detail", ticket);
        }
        [HttpPost]
        public async Task<ActionResult> Create(tbl_Airline model, FormCollection form, HttpPostedFileBase Image)
        {
            Permission(clsPermission.GetUser().PermissionID, 1138);
            try
            {
                model.IsDelete = false;
                model.CreateDate = DateTime.Now;
                model.StaffId = clsPermission.GetUser().StaffID;
                if (Image != null)
                {
                    //file
                    string FileSize = Common.ConvertFileSize(Image.ContentLength);
                    String newName = Image.FileName.Insert(Image.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    Image.SaveAs(path);
                    model.Logo = newName;
                    //end file
                }
                if (await _airlineRepository.Create(model))
                {

                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch { }
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tbl_Airline model, FormCollection form, HttpPostedFileBase Image)
        {
            Permission(clsPermission.GetUser().PermissionID, 1138);
            try
            {
                var airlineTemp = _airlineRepository.GetAllAsQueryable().FirstOrDefault(p => p.Id == model.Id);
                if (Image != null)
                {
                    string FileSize = Common.ConvertFileSize(Image.ContentLength);
                    String newName = Image.FileName.Insert(Image.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    Image.SaveAs(path);
                    model.Logo = newName;
                }
                else
                {
                    model.Logo = airlineTemp.Logo;
                }
                if (await _airlineRepository.Update(model))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch { }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult AirlineInformation(int id)
        {
            var airline = _airlineRepository.GetAllAsQueryable().FirstOrDefault(p => p.Id == id);
            return PartialView("_Partial_EditAirline", airline);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(FormCollection fc)
        {
            Permission(clsPermission.GetUser().PermissionID, 1138);
            try
            {
                if (fc["listItemId"] != null && fc["listItemId"] != "")
                {
                    var listIds = fc["listItemId"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        if (await _airlineRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "AirlineManage") }, JsonRequestBehavior.AllowGet);
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


        [HttpPost]
        
        public async Task<ActionResult> DeleteTicket(int IdTicket)
        {
            Permission(clsPermission.GetUser().PermissionID, 1138);
            try
            {
                var ticketTemp = _db.tbl_AirlineTicket.FirstOrDefault(p => p.Id == IdTicket);
                if (ticketTemp != null)
                {
                    string path = Server.MapPath("~/Upload/file/" + ticketTemp.FileName);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    if (await _airlineTicketRepository.Delete(ticketTemp.Id, false))
                    {
                        return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "AirlineManage") }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Xóa dữ liệu thất bại !" }, JsonRequestBehavior.AllowGet);
                    }
                }
                
                return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Vui lòng chọn những mục cần xóa !" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult UploadFileQuotation(HttpPostedFileBase FileNameQuotation)
        {
            if (FileNameQuotation != null && FileNameQuotation.ContentLength > 0)
            {
                Session["FileUploadTicket"] = FileNameQuotation;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }
    }
}