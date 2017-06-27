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
    public class EvaluationPartnerManageController : BaseController
    {
        // GET: EvaluationPartner

        #region Init
        private DataContext _db;
        private IGenericRepository<tbl_EvaluationCriteria> _evaluationCriteriaRepository;
        private IGenericRepository<tbl_Evaluation> _evaluationRepository;
        private IGenericRepository<tbl_EvaluationDetail> _evaluationDetailRepository;
        private IGenericRepository<tbl_ActionData> _actionDataRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;

        public EvaluationPartnerManageController(
           IGenericRepository<tbl_EvaluationCriteria> evaluationCriteriaRepository,
           IGenericRepository<tbl_Evaluation> evaluationRepository,
           IGenericRepository<tbl_EvaluationDetail> evaluationDetailRepository,
           IGenericRepository<tbl_ActionData> actionDataRepository,
           IGenericRepository<tbl_Tags> tagsRepository,
           IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._evaluationCriteriaRepository = evaluationCriteriaRepository;
            this._evaluationDetailRepository = evaluationDetailRepository;
            this._evaluationRepository = evaluationRepository;
            this._actionDataRepository = actionDataRepository;
            this._tagsRepository = tagsRepository;
            _db = new DataContext();
        }
        #endregion

        #region Index

        void Permission(int PermissionsId, int formId)
        {
            var list = _actionDataRepository.GetAllAsQueryable().AsEnumerable()
                .Where(p => p.FormId == formId && p.PermissionsId == PermissionsId)
                .Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);
        }

        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 1134);
            return View();
        }

        public ActionResult _Partial_List()
        {
            var model = _evaluationRepository.GetAllAsQueryable().AsEnumerable()
                        .Where(p => p.IsDelete == false &&  p.CreateStaffId == clsPermission.GetUser().StaffID)
                        .OrderByDescending(p => p.CreatedDate).ToList();
            return PartialView("_Partial_List", model);
        }

        public ActionResult Search(int partner, int tour)
        {
            var model = _evaluationRepository.GetAllAsQueryable().AsEnumerable()
                           .Where(p => p.PartnerId == (partner == 0 ? p.PartnerId : partner)
                           && p.TourId == (tour == 0 ? p.TourId : tour)
                           && p.IsDelete == false && p.CreateStaffId == clsPermission.GetUser().StaffID)
                           .OrderByDescending(p => p.CreatedDate).ToList();
            return PartialView("_Partial_List", model);
        }

        #endregion

        #region Create

        public JsonResult ProvinceList(int id)
        {
            return Json(new SelectList(_tagsRepository.GetAllAsQueryable().Where(p => p.ParentId == id).ToList(), "Id", "Tag"), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create(tbl_Evaluation model, FormCollection fc)
        {
            Permission(clsPermission.GetUser().PermissionID, 1134);
            decimal? total = 0;
            model.CreateStaffId = clsPermission.GetUser().StaffID;
            model.CreatedDate = DateTime.Now;
            model.IsDelete = false;
            model.Type = 1;
            var check = _evaluationRepository.Create(model);
            if (await _evaluationRepository.Create(model))
            {
                if (!string.IsNullOrEmpty(fc["NumberEvaluation"]))
                {
                    for (int i = 1; i <= Convert.ToInt32(fc["NumberEvaluation"]); i++)
                    {
                        var detail = new tbl_EvaluationDetail()
                        {
                            EvaluationCriteriaId = Convert.ToInt32(fc["EvaluationCriteriaId" + i]),
                            EvaluationId = model.Id,
                            Note = fc["EvaluationNote" + i],
                            Point = !string.IsNullOrEmpty(fc["EvaluationPoint" + i]) ? Convert.ToInt32(fc["EvaluationPoint" + i]) : 0,
                        };
                        await _evaluationDetailRepository.Create(detail);
                        total += detail.Point;
                    }
                    // update tổng tiền
                    UpdateDatabase.UpdateEvaluation(model.Id, total ?? 0);
                }
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region Update

        public async Task<ActionResult> Edit(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 1134);
            ViewBag.DetailList = _evaluationDetailRepository.GetAllAsQueryable().Where(p => p.EvaluationId == id).ToList();
            return PartialView("_Partial_Edit", await _evaluationRepository.GetById(id));
        }

        public async Task<ActionResult> Update(tbl_Evaluation model, FormCollection fc)
        {
            Permission(clsPermission.GetUser().PermissionID, 1134);
            decimal? total = 0;
            await _evaluationRepository.Update(model);
            // xóa hết điểm đã có 
            foreach (var item in _evaluationDetailRepository.GetAllAsQueryable().Where(p => p.EvaluationId == model.Id).ToList())
            {
                await _evaluationDetailRepository.DeleteMany(item.Id.ToString().Split(',').ToArray(), true);
            }
            for (int i = 1; i <= Convert.ToInt32(fc["EditNumberEvaluation"]); i++)
            {
                var detail = new tbl_EvaluationDetail()
                {
                    EvaluationCriteriaId = Convert.ToInt32(fc["EditEvaluationCriteriaId"]),
                    EvaluationId = model.Id,
                    Note = fc["EditEvaluationNote"],
                    Point = !string.IsNullOrEmpty(fc["EditEvaluationPoint"]) ? Convert.ToInt32(fc["EditEvaluationPoint"]) : 0,
                };
                await _evaluationDetailRepository.Create(detail);
                total += detail.Point;
            }
            // update tổng tiền
            UpdateDatabase.UpdateEvaluation(model.Id, total ?? 0);

            return RedirectToAction("Index");
        }

        #endregion

        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(FormCollection fc)
        {
            Permission(clsPermission.GetUser().PermissionID, 1134);
            try
            {
                if (fc["listItemId"] != null && fc["listItemId"] != "")
                {
                    var listIds = fc["listItemId"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        //foreach (var i in listIds)
                        //{
                        //    var appointment = _appointmentHistoryRepository.FindId(Convert.ToInt32(i));
                        //    UpdateHistory.SaveHistory(26, "Xóa lịch hẹn: " + appointment.Title,
                        //        appointment.Id, //appointment
                        //        appointment.ContractId, //contract
                        //        appointment.CustomerId, //customer
                        //        appointment.PartnerId, //partner
                        //        appointment.ProgramId, //program
                        //        appointment.TaskId, //task
                        //        appointment.TourId, //tour
                        //        null, //quotation
                        //        null, //document
                        //        null, //history
                        //        null // ticket
                        //        );
                        //}

                        if (await _evaluationRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "EvaluationPartnerManage") }, JsonRequestBehavior.AllowGet);
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

        #region Detail

        public ActionResult Detail(int? id)
        {
            return PartialView("_Partial_Detail", _evaluationDetailRepository.GetAllAsQueryable().Where(p => p.EvaluationId == id).ToList());
        }

        #endregion
    }
}