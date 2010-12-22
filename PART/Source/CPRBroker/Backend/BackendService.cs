using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using CPRBroker.Engine;
using CPRBroker.DAL.Applications;

namespace Backend
{
    /// <summary>
    /// This service is responsible for the scheduling of publishing events to subscribers
    /// </summary>
    public partial class BackendService : GKApp.WinSvc.winsvcBase
    {
        System.Timers.Timer NotificationsTimer = new System.Timers.Timer();
        public BackendService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Initialize the timer
            NotificationsTimer.Elapsed += new System.Timers.ElapsedEventHandler(NotificationsTimer_Elapsed);
            NotificationsTimer.AutoReset = false;
            ScheduleNextTimerRun();
            BrokerContext.Initialize(Application.BaseApplicationToken.ToString(), CPRBroker.Engine.Constants.UserToken, true, false, true);
            CPRBroker.Engine.Local.Admin.LogSuccess(CPRBroker.Engine.TextMessages.BackendServiceStarted);
        }

        /// <summary>
        /// Schedules the timer to run at the beginning of tomorrow
        /// </summary>
        void ScheduleNextTimerRun()
        {
            DateTime endToday = DateTime.Today.AddDays(1);

            // Take care of the case if service is started at the very end of a day
            TimeSpan interval = endToday - DateTime.Now;
            if (interval < TimeSpan.FromMinutes(1))
                interval = TimeSpan.FromMinutes(1);

            NotificationsTimer.Interval = interval.TotalMilliseconds;

            NotificationsTimer.Start();
        }

        void NotificationsTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.EventLog.WriteEntry(CPRBroker.Engine.TextMessages.BackendTimerEventStarted);
            // Set next timer start
            ScheduleNextTimerRun();

            // Refresh persons
            CPRBroker.Engine.Notifications.NotificationEngine.RefreshPersonsData();

            // Send notifications
            DateTime today = e.SignalTime.Date;
            CPRBroker.Engine.Notifications.NotificationEngine.SendNotifications(DateTime.Now);
            this.EventLog.WriteEntry(CPRBroker.Engine.TextMessages.BackendTimerEventFinished);
        }
    }
}
