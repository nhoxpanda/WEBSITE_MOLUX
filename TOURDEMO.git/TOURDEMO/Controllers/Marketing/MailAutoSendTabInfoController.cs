using CRM.Core;
using CRM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace TOURDEMO.Controllers.Marketing
{
    public class MailAutoSendTabInfoController : BaseController
    {
        // GET: MailAutoSendTabInfo

        #region Init
        private IGenericRepository<tbl_MailSending> _mailSendingRepository;
        private IGenericRepository<tbl_MailSendingList> _mailSendingListRepository;
        private IGenericRepository<tbl_MailReceiveList> _mailReceiveListRepository;
        private IGenericRepository<tbl_MailReceives> _mailReceivesRepository;
        private IGenericRepository<tbl_MailTemplates> _mailTemplatesRepository;
        private IGenericRepository<tbl_MailConfig> _mailConfigRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private DataContext _db;

        public MailAutoSendTabInfoController(
            IGenericRepository<tbl_MailSendingList> mailSendingListRepository,
            IGenericRepository<tbl_MailSending> mailSendingRepository,
            IGenericRepository<tbl_MailReceiveList> mailReceiveListRepository,
            IGenericRepository<tbl_MailReceives> mailReceivesRepository,
            IGenericRepository<tbl_MailTemplates> mailTemplatesRepository,
            IGenericRepository<tbl_MailConfig> mailConfigRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
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

        #region Chờ gửi
        [ChildActionOnly]
        public ActionResult _ChoGui()
        {
            return PartialView("_ChoGui");
        }

        [HttpPost]
        public async Task<ActionResult> InfoChoGui(int id)
        {
            var model = await _mailSendingListRepository.GetAllAsQueryable().Where(p => p.MailSendingId == id && p.IsDelete == false).ToListAsync();
            return PartialView("_ChoGui", model);
        }
        #endregion

        #region Đã gửi
        [ChildActionOnly]
        public ActionResult _DaGui()
        {
            return PartialView("_DaGui");
        }

        [HttpPost]
        public async Task<ActionResult> InfoDaGui(int id)
        {
            var model = await _mailReceiveListRepository.GetAllAsQueryable().Where(p => p.IsDelete == false).ToListAsync();
            return PartialView("_DaGui", model);
        }
        #endregion

        #region Không gửi được
        [ChildActionOnly]
        public ActionResult _KhongGuiDuoc()
        {
            return PartialView("_KhongGuiDuoc");
        }

        [HttpPost]
        public async Task<ActionResult> InfoKhongGuiDuoc(int id)
        {
            var model = await _mailReceiveListRepository.GetAllAsQueryable().Where(p => p.IsDelete == false).ToListAsync();
            return PartialView("_KhongGuiDuoc", model);
        }
        #endregion

        #region Nội dung
        [ChildActionOnly]
        public ActionResult _NoiDung()
        {
            return PartialView("_NoiDung");
        }

        [HttpPost]
        public async Task<ActionResult> InfoNoiDung(int id)
        {
            var model = await _mailSendingRepository.GetById(id);
            return PartialView("_NoiDung", model);
        }
        #endregion
    }
}