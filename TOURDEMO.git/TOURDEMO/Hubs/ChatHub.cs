using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using TOURDEMO.Models;
using CRM.Infrastructure;
using CRM.Core;

namespace MVC_demo.Hubs
{
    public class ChatHub : Hub
    {
        #region Data Members

        private DataContext db = new DataContext();

        static List<UserDetail> ConnectedUsers = new List<UserDetail>();
        static List<MessageDetail> CurrentMessage = new List<MessageDetail>();

        #endregion

        #region Methods

        public void Connect(string userName, int id)
        {
            var ConnectionId = Context.ConnectionId;

            //thang dau tien, thi lay danh sach ra
            if (ConnectedUsers.Count == 0)
                ConnectedUsers = GetListUser();

            if (ConnectedUsers.Where(u => u.id == id).FirstOrDefault() != null)// cap nhat connectid cho thang id login zo
                ConnectedUsers.Where(u => u.id == id).FirstOrDefault().ConnectionId = ConnectionId;

             // send to caller
            Clients.Caller.onConnected(ConnectionId ,userName, ConnectedUsers, CurrentMessage);

            // send to all except caller client
            Clients.AllExcept(ConnectionId).onNewUserConnected(ConnectionId, userName, id);


            //if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
            //{
            //    ConnectedUsers.Add(new UserDetail { ConnectionId = id, UserName = userName });

            //    // send to caller
            //    Clients.Caller.onConnected(id, userName, ConnectedUsers, CurrentMessage);

            //    // send to all except caller client
            //    Clients.AllExcept(id).onNewUserConnected(id, userName);

            //}

        }

       public void CheckGroup(string userIdTo, string userId)
       {
           var group_id = CheckOrCreateGroup(userIdTo, userId);

           var user = ConnectedUsers.Where(u=>u.id == Convert.ToInt32(userId)).First();
           var userTo = ConnectedUsers.Where(u=>u.id == Convert.ToInt32(userIdTo)).First();
           // send to 
           if (userTo.ConnectionId != null)
            Clients.Client(userTo.ConnectionId).createGroup(userIdTo, userTo.UserName, userId, group_id);

           // send to caller user
           Clients.Caller.createGroup(userId, user.UserName ,userIdTo, group_id); 
       }

        public void SendMessageToAll(string userName, string message, int id)
        {
            // store last 100 messages in cache
            AddMessageinCache(userName, message);

            // Broad cast message
            Clients.All.messageReceived(userName, message);

            AddNewMessage(userName, message, id, 0);

        }

        public void SendPrivateMessage(int group_id, string userId, string message, string toUserId)
        {

            string fromUserId = Context.ConnectionId;

            var toUser = ConnectedUsers.FirstOrDefault(x => x.id.ToString() == toUserId) ;
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.id.ToString() == userId);

            if (toUser != null && fromUser!=null)
            {
                if (toUser.ConnectionId != null)
                {
                    // send to 
                    Clients.Client(toUser.ConnectionId).sendPrivateMessage(group_id, toUserId, fromUser.UserName, message, userId);
                }

                // send to caller user
                Clients.Caller.sendPrivateMessage(group_id, userId, fromUser.UserName, message, toUserId); 
            }

            AddNewMessage(fromUser.UserName, message, fromUser.id, group_id);
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                ConnectedUsers.Remove(item);

                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.UserName, item.id);

            }

            return base.OnDisconnected(stopCalled);
        }
        #endregion

        #region private Messages

        public int CheckOrCreateGroup(string userIdTo,string userId)
        {
            var group = new tbl_GroupChat();
            if(db.tbl_GroupChat.ToList().Count > 0)
                group = db.tbl_GroupChat.Where(u => (u.UserChat.Contains("#" + userIdTo + "#") && u.UserChat.Contains("#" + userId + "#")) || (u.UserChat.Contains("#" + userId + "#") && u.UserChat.Contains("#" + userIdTo + "#"))).FirstOrDefault();

            if (group == null || group.Id == 0)
            {
                group = new tbl_GroupChat();
                group.DateCreated = DateTime.Now;
                group.UserChat = "#" + userIdTo + "#" + userId + "#";

                db.tbl_GroupChat.Add(group);
                db.SaveChanges();
            }

            return group.Id;
                
        }

        public void AddNewMessage(string userName, string message, int id, int group_id)
        {
            var item = new tbl_Message()
            {
                Date = DateTime.Now,
                UserName = userName,
                UserID = id.ToString(),
                GroupID = group_id,
                Message = message,
                Status = "new"

            };
            db.tbl_Message.Add(item);
            db.SaveChanges();
        }
        //public List<MessageDetail> GetMessageTwoUser(int userIdFrom, int userIdTo)
        //{
        //    var messagesDB = db.tbl_Message.Where(u => u.Group_Id.Contains("#" + userIdFrom + "#") && u.Group_Id.Contains("#" + userIdTo + "#")).ToList();

        //    List<MessageDetail> list = new List<MessageDetail>();

        //    foreach(var i in messagesDB)
        //    {
        //        MessageDetail item = new MessageDetail
        //        {
        //            Message = i.Desc,
        //            UserName = i.User_Name

        //        };
        //        list.Add(item);
        //    }

        //    return list;
        //}
        //public List<MessageDetail> GetMessageAll()
        //{
        //    var messagesDB = db.tbl_Message.Where(u => u.Group_Id == "0").ToList();

        //    List<MessageDetail> list = new List<MessageDetail>();

        //    foreach (var i in messagesDB)
        //    {
        //        MessageDetail item = new MessageDetail
        //        {
        //            Message = i.Desc,
        //            UserName = i.User_Name

        //        };
        //        list.Add(item);
        //    }

        //    return list;
        //}
        public List<UserDetail> GetListUser()
        {
            var list = db.tbl_Staff.ToList();

            List<UserDetail> listU = new List<UserDetail>();
            foreach(var i in list)
            {
                var user = new UserDetail
                {
                    id = i.Id,
                    UserName = i.Code,
                    FullName = i.FullName,
                    Position = i.tbl_DictionaryPosition.Name
                };
                listU.Add(user);
            }
            return listU;
        }
        private void AddMessageinCache(string userName, string message)
        {
            CurrentMessage.Add(new MessageDetail { UserName = userName, Message = message });

            if (CurrentMessage.Count > 100)
                CurrentMessage.RemoveAt(0);
        }

        #endregion
    }

}