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

namespace TOURDEMO.Controllers.Customer
{
    [Authorize]
    public class MemberCardManageController : BaseController
    {
        #region Init

        private IGenericRepository<tbl_MemberCard> _memberCardRepository;
        private IGenericRepository<tbl_ActionData> _actionDataRepository;

        private DataContext _db;

        public MemberCardManageController(
            IGenericRepository<tbl_MemberCard> memberCardRepository,
            IGenericRepository<tbl_ActionData> actionDataRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._memberCardRepository = memberCardRepository;
            this._actionDataRepository = actionDataRepository;

            _db = new DataContext();
        }

        #endregion

        #region Phân quyền
        void Permission(int PermissionsId, int formId)
        {
            var list = _actionDataRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.FormId == formId && p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);
            ViewBag.IsImport = list.Contains(4);
            ViewBag.IsExport = list.Contains(5);
        }
        #endregion
        // GET: MemberCardManage
        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 1131);
            var memCardList = _memberCardRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.IsDelete == false)
                .Select(p => new MemberCardViewModel()
                {
                    Id = p.Id,
                    NameCard = p.Name,
                    MaxValue = p.MaxValue,
                    MinValue = p.MinValue,
                    Percent = p.Percent ?? 0
                }).ToList();

            return View(memCardList);
        }
        public JsonResult AddMemberCard(int ID, string _name, int _min, int _max, string _percent)
        {
            _percent.Replace(".", ",");
            decimal percent = decimal.Parse(_percent);
            bool check = false;
            try
            {
                if (ID > 0)
                {
                    var mbCard = _db.tbl_MemberCard.Find(ID);
                    if (mbCard != null)
                    {
                        mbCard.Name = _name;
                        mbCard.MinValue = _min;
                        mbCard.MaxValue = _max;
                        mbCard.Percent = percent;
                    }
                }
                else
                {
                    _db.tbl_MemberCard.Add(new tbl_MemberCard()
                    {
                        Name = _name,
                        MinValue = _min,
                        MaxValue = _max,
                        Percent = percent,
                        IsDelete = false
                    });
                }

                _db.SaveChanges();
                check = true;
            }
            catch (Exception ex)
            {
                var _message = ex.Message;
            }
            return Json(new { result = check }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadMBCardByID(int ID)
        {
            var mbCard = _memberCardRepository.FindId(ID);
            if (mbCard != null)
            {
                return Json(new { result = true, _name = mbCard.Name, _min = mbCard.MinValue, _max = mbCard.MaxValue, _percent=mbCard.Percent }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(FormCollection fc)
        {
            if (fc["listItemId"] != null && fc["listItemId"] != "")
            {
                var listIds = fc["listItemId"].Split(',');
                listIds = listIds.Take(listIds.Count() - 1).ToArray();
                if (listIds.Count() > 0)
                {
                    if (await _memberCardRepository.DeleteMany(listIds, false))
                    {
                        return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "MemberCardManage") }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Xóa dữ liệu thất bại !" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Vui lòng chọn những mục cần xóa !" }, JsonRequestBehavior.AllowGet);
        }
    }
}

