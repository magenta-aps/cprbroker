using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace CprBroker.EventBroker.Notifications
{
    public class NotificationSender
    {
        private Timer NotificationTimer;


        public NotificationSender()
        {
            NotificationTimer = new Timer();
            PollInterval = TimeSpan.FromSeconds(60);
            NotificationTimer.AutoReset = true;
            NotificationTimer.Elapsed += new ElapsedEventHandler(NotificationTimer_Elapsed);
            NotificationTimer.Start();
            
        }

        public TimeSpan PollInterval
        {
            get { return TimeSpan.FromMilliseconds(this.NotificationTimer.Interval); }
            set { NotificationTimer.Interval = value.TotalMilliseconds; }
        }
        
        private void NotificationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            EnqueueDataChangeEvents();
            SendNotifications();
        }

        private void EnqueueDataChangeEvents()
        {
            using (var dataContext = new DAL.EventBrokerDataContext())
            {
                DateTime startDate = DateTime.Now;
                DateTime endDate = startDate - PollInterval;

                dataContext.EnqueueDataChangeEventNotifications(startDate, endDate, DateTime.Now, (int)DAL.SubscriptionType.SubscriptionTypes.DataChange);
            }
        }

        private void SendNotifications()
        {
 
        }
                
        

        
    }
}
