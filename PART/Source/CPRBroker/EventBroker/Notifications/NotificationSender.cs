using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;

namespace CprBroker.EventBroker.Notifications
{
    /// <summary>
    /// Reads the notification queue and sends the notifications to subscribers
    /// </summary>
    public partial class NotificationSender : PeriodicTaskExecuter
    {
        public NotificationSender()
            : base()
        {
            InitializeComponent();
        }

        public NotificationSender(IContainer container)
            : base(container)
        {
            container.Add(this);
            InitializeComponent();
        }

        protected override TimeSpan CalculateActionTimerInterval(TimeSpan currentInterval)
        {
            return TimeSpan.FromMilliseconds(CprBroker.Config.Properties.Settings.Default.EventBrokerPollIntervalMilliseconds);
        }
        
        protected override void PerformTimerAction()
        {
            using (var dataContext = new DAL.EventBrokerDataContext())
            {
                int batchSize = CprBroker.Config.Properties.Settings.Default.EventBrokerNotificationBatchSize;
                DAL.EventNotification[] dueNotifications = new CprBroker.EventBroker.DAL.EventNotification[0];
                do
                {
                    dueNotifications =
                        (
                            from eventNotification in dataContext.EventNotifications
                            where eventNotification.Succeeded == null
                            orderby eventNotification.CreatedDate
                            select eventNotification
                        ).Take(batchSize).ToArray();


                    foreach (var eventNotification in dueNotifications)
                    {
                        eventNotification.NotificationDate = DateTime.Now;
                        try
                        {
                            Channel channel = Channel.Create(eventNotification.Subscription.Channels.Single());
                            // TODO: Change this method call to use EventNotification object
                            channel.Notify(null);
                            eventNotification.Succeeded = true;
                        }
                        catch (Exception ex)
                        {
                            string message = string.Format("Notification {0} failed", eventNotification.EventNotificationId);
                            //TODO: use LogNotificationFailure after simplifying its parameters
                            CprBroker.Engine.Local.Admin.LogException(ex, message);
                            eventNotification.Succeeded = false;
                        }
                    }
                    dataContext.SubmitChanges();

                } while (dueNotifications.Length == batchSize);
            }
        }
    }
}
