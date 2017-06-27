using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TOURDEMO.Models;
using CRM.Core;
using CRM.Infrastructure;

namespace TOURDEMO.Controllers.Contract
{
    [Authorize]
    public class FormContractController : BaseController
    {
        // GET: FormContract

        #region Init

        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
        private IGenericRepository<tbl_Contract> _contractRepository;
        private IGenericRepository<tbl_Tags> _tagRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private DataContext _db;

        public FormContractController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IGenericRepository<tbl_Contract> contractRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Tags> tagRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._updateHistoryRepository = updateHistoryRepository;
            this._staffRepository = staffRepository;
            this._contractRepository = contractRepository;
            this._documentFileRepository = documentFileRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._tagRepository = tagRepository;
            _db = new DataContext();
        }

        #endregion

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExportExcel(FormCollection form)
        {
            return View();
        }
    }
}