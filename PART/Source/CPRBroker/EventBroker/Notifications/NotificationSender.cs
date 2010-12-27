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
    public partial class NotificationSender : Component
    {
        public NotificationSender()
        {
            InitializeComponent();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            PollInterval = TimeSpan.FromMilliseconds(CprBroker.Config.Properties.Settings.Default.EventBrokerPollIntervalMilliseconds);
            NotificationTimer.AutoReset = true;
            NotificationTimer.Elapsed += new ElapsedEventHandler(NotificationTimer_Elapsed);
        }

        public NotificationSender(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            InitializeTimer();
        }

        public void Start()
        {
            NotificationTimer.Start();
        }

        public void Stop()
        {
            NotificationTimer.Stop();
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
            try
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
            catch (Exception ex)
            {
                CprBroker.Engine.Local.Admin.LogException(ex);
            }
        }
    }
}
