using CRM.Core;
using CRM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TOURDEMO.Controllers
{
    public class LivechatManageController : BaseController
    {
        //
        // GET: /LivechatManage/

        #region Init

        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_Conversation> _conversationRepository;
        private IGenericRepository<tbl_ConversationReply> _conversationReplyRepository;
        private IGenericRepository<tbl_GroupChat> _groupChatRepository;
        private IGenericRepository<tbl_Message> _messageRepository;
        private DataContext _db;

        public LivechatManageController(IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Conversation> conversationRepository,
            IGenericRepository<tbl_ConversationReply> conversationReplyRepository,
            IGenericRepository<tbl_GroupChat> groupChatRepository,
            IGenericRepository<tbl_Message> messageRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._staffRepository = staffRepository;
            this._conversationRepository = conversationRepository;
            this._conversationReplyRepository = conversationReplyRepository;
            this._groupChatRepository = groupChatRepository;
            this._messageRepository = messageRepository;
            _db = new DataContext();
        }

        #endregion

        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult _Partial_Staff()
        {
            var user = _staffRepository.GetAllAsQueryable().First(u => u.Code == User.Identity.Name);
            return PartialView("_Partial_Staff", user);
        }
    }
}
