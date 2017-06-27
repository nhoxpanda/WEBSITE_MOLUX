using Microsoft.AspNet.SignalR;
using TOURDEMO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace TOURDEMO.Hubs
{
    [Authorize]
    public class MyHub : Hub
    {
        public MyHub()
        {
            // Create a Long running task to do an infinite loop which will keep sending the server time
            // to the clients every 3 seconds.
            var taskTimer = Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    string timeNow = DateTime.Now.ToString();
                    //Sending the server time to all the connected clients on the client method SendServerTime()
                    Clients.All.SendServerTime(timeNow);
                   
                    //Delaying by 1 seconds.
                    await Task.Delay(1000);
                }
            }, TaskCreationOptions.LongRunning
                );
        }
    }
}