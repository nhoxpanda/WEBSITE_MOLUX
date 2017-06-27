using System;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Threading;
using System.Web.Hosting;
using Humanizer;
using TOURDEMO.Hubs;
using TOURDEMO.Models;
using TOURDEMO.Utilities;
using CRM.Infrastructure;

namespace TOURDEMO
{
    public class BackgroundUptimeServerTimer : IRegisteredObject
    {
        private DataContext _db;
        private readonly IHubContext _uptimeHub;
        private Timer _timer;

        public BackgroundUptimeServerTimer()
        {
            _db = new DataContext();
            _uptimeHub = GlobalHost.ConnectionManager.GetHubContext<UptimeHub>();
            StartTimer();
        }

        private void StartTimer()
        {
            var delayStartby = 1.Seconds();
            var repeatEvery = 5.Minutes();

            _timer = new Timer(BroadcastUptimeToClients, null, delayStartby, repeatEvery);
        }

        private void BroadcastUptimeToClients(object state)
        {
            foreach (var item in Notification.Schedules())
            {
                TimeSpan uptime = item.Time - DateTime.Now;
                if (item.IsDelete == false && Convert.ToInt32(uptime.TotalMinutes) == item.Notify)
                {
                    _uptimeHub.Clients.All.internetUpTime(item.Title + " : " + item.Time);
                }
            }
        }

        public void Stop(bool immediate)
        {
            _timer.Dispose();

            HostingEnvironment.UnregisterObject(this);
        }
    }
}