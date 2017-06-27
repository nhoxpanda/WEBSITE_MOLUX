using CRM.Core;
using CRM.Infrastructure;
using TOURDEMO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TOURDEMO.Utilities;

namespace TOURDEMO.Controllers.Visa
{
    [Authorize]
    public class InfomationVisaManageController : BaseController
    {
        // GET: InfomationVisaManage
        #region Init

        private IGenericRepository<tbl_VisaInfomation> _visaInfomationRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;

        private DataContext _db;

        public InfomationVisaManageController(IGenericRepository<tbl_VisaInfomation> visaInfomationRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._visaInfomationRepository = visaInfomationRepository;
            this._tagsRepository = tagsRepository;
            _db = new DataContext();
        }

        #endregion
        void Permission(int PermissionsId, int formId)
        {
            var list = _db.tbl_ActionData.Where(p => p.FormId == formId & p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsEdit = list.Contains(3);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsImport = list.Contains(4);
        }
        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 14);
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(tbl_VisaInfomation model, FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 14);
            try
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                if (form["TagId"] != null && form["TagId"] != "")
                {
                    model.TagId = Convert.ToInt32(form["TagId"].ToString());
                }

                if (ModelState.IsValid)
                {

                    if (await _visaInfomationRepository.Create(model))
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Dữ liệu đầu vào không đúng định dạng!");
                    }
                }
            }
            catch { }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> EditInfoVisa(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 14);
            var model = await _visaInfomationRepository.GetById(id);
            List<SelectListItem> lstTag = new List<SelectListItem>();
            foreach (var t in _db.tbl_Tags.Where(p => p.TypeTag == 3).ToList())
            {
                lstTag.Add(new SelectListItem()
                {
                    Text = t.Tag,
                    Value = t.Id.ToString(),
                    Selected = model.TagId == t.Id ? true : false
                });
            }
            ViewBag.TagId = lstTag;
            return PartialView("_Partial_EditInfomationVisa", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tbl_VisaInfomation model, FormCollection form)
        {
            Permission(clsPermission.GetUser().PermissionID, 14);
            try
            {
                if (ModelState.IsValid)
                {
                    model.ModifiedDate = DateTime.Now;
                    if (form["TagId"] != null && form["TagId"] != "")
                    {
                        model.TagId = Convert.ToInt32(form["TagId"].ToString());
                    }
                    if (await _visaInfomationRepository.Update(model))
                    {
                        UpdateHistory.SaveHistory(14, "Cập nhật thông tin visa " + _tagsRepository.FindId(model.TagId).Tag,
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
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Dữ liệu đầu vào không đúng định dạng!");
                    }
                }
            }
            catch
            {
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(FormCollection fc)
        {
            Permission(clsPermission.GetUser().PermissionID, 14);
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
                            var item = _visaInfomationRepository.FindId(Convert.ToInt32(i));
                            UpdateHistory.SaveHistory(14, "Xóa thông tin visa, quốc gia: " + _tagsRepository.FindId(_visaInfomationRepository.FindId(i).TagId).Tag,
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
                        //
                        if (await _visaInfomationRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "InfomationVisaManage") }, JsonRequestBehavior.AllowGet);
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

        [ChildActionOnly]
        public ActionResult _Partial_ListInfomation()
        {
            Permission(clsPermission.GetUser().PermissionID, 14);
            var model = _visaInfomationRepository.GetAllAsQueryable().Where(p => p.IsDelete == false)
                .OrderByDescending(p => p.CreatedDate).ToList();
            return PartialView("_Partial_ListInfomation", model);
        }

        [HttpPost]
        public ActionResult SearchCountryVisa(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 14);
            var model = _visaInfomationRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TagId == id).Where(p => p.IsDelete == false).ToList();
            return PartialView("_Partial_ListInfomation", model);

        }
    }
}