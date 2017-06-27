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

namespace TOURDEMO.Controllers.Evaluation
{
    public class EvaluationCriteriaController : BaseController
    {
        #region Init
        private DataContext _db;
        private IGenericRepository<tbl_EvaluationCriteria> _evaluationRepository;
        private IGenericRepository<tbl_ActionData> _actionDataRepository;

        public EvaluationCriteriaController(
           IGenericRepository<tbl_EvaluationCriteria> evaluationRepository,
           IGenericRepository<tbl_ActionData> actionDataRepository,
           IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._evaluationRepository = evaluationRepository;
            this._actionDataRepository = actionDataRepository;
            _db = new DataContext();
        }
        #endregion

        void Permission(int PermissionsId, int formId)
        {
            var list = _actionDataRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.FormId == formId && p.PermissionsId == PermissionsId)
                .Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);  
        }

        // GET: EvaluationCriteria
        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 1133);
            
            var evalCritList = _evaluationRepository.GetAllAsQueryable().Where(k => k.IsDelete == false)
                .Select(p => new EvaluationCriteriaViewModel()
                {
                    Id = p.Id,
                    IsDelete=p.IsDelete,
                    Name=p.Name,
                    Note=p.Note,
                    Type=p.Type
                }).ToList();
            return View(evalCritList);
        }
        [HttpPost]
        public ActionResult SaveData(int id, string name)
        {
            try
            {
                if (id == 0) // insert
                {
                    tbl_EvaluationCriteria evalCrit = new tbl_EvaluationCriteria()
                    {
                        Name = name,
                        IsDelete = false,
                        Type=1
                    };

                    _db.tbl_EvaluationCriteria.Add(evalCrit);
                }
                else // update
                {
                    var item = _db.tbl_EvaluationCriteria.Find(id);
                    item.Name = name;
                }

                _db.SaveChanges();
            }
            catch
            {
            }

            return Json(JsonRequestBehavior.AllowGet);
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
                    if (await _evaluationRepository.DeleteMany(listIds, false))
                    {
                        return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "EvaluationCriteria") }, JsonRequestBehavior.AllowGet);
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