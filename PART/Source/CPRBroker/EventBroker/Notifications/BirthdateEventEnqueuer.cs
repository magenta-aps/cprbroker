using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CprBroker.EventBroker.Notifications
{
    public partial class BirthdateEventEnqueuer : Component
    {
        public BirthdateEventEnqueuer()
        {
            InitializeComponent();
            InitializeTimer();
        }

        public BirthdateEventEnqueuer(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            InitializeTimer();
        }

        void InitializeTimer()
        {
            BirthdateTimer.Elapsed += new System.Timers.ElapsedEventHandler(BirthdateTimer_Elapsed);
            BirthdateTimer.AutoReset = false;
            ScheduleNextTimerRun();
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

            BirthdateTimer.Interval = interval.TotalMilliseconds;

            BirthdateTimer.Start();
        }

        private void BirthdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ScheduleNextTimerRun();
            EnqueueBirthdateEventNotifications();
        }

        private void EnqueueBirthdateEventNotifications()
        {
            try
            {
                using (var dataContext = new DAL.EventBrokerDataContext())
                {
                    DateTime today = DateTime.Today;

                    foreach (var subscription in dataContext.Subscriptions)
                    {
                        try
                        {
                            dataContext.EnqueueBirthdateEventNotifications(subscription.SubscriptionId, today);
                        }
                        catch (Exception ex)
                        {
                            string message = string.Format("Failed to enqueue birthdate notifications for {0}", subscription.SubscriptionId);
                            CPRBroker.Engine.Local.Admin.LogException(ex, message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CPRBroker.Engine.Local.Admin.LogException(ex);
            }
        }

    }
}
