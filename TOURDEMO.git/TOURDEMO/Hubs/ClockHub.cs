using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOURDEMO.Hubs
{
    public class ClockHub : Hub
    {
        public void getTime()
        {
            Clients.Caller.setTime(DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"));
        }
    }
}