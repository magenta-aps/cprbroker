using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data.Linq;
using CPRBroker.DAL;

namespace CPRBroker.Engine
{
    /// <summary>
    /// Responsible for finding the subscriptions that should be notified
    /// </summary>
    public class NotificationEngine
    {
        /// <summary>
        /// A simple result class for the notifcication process
        /// </summary>
        public class SendNotificationsResult
        {
            public List<Guid> SentNotificationIds = new List<Guid>();
            public List<Guid> FailedNotificationIds = new List<Guid>();
        }

        public class RefreshPersonsDataResult
        {
            public List<string> SucceededCprNumbers = new List<string>();
            public List<string> FailedCprNumbers = new List<string>();
        }

        /// <summary>
        /// Refreshes data of the persons in the database that are part of a data change subscription
        /// If there is at least one data change subscription that is for all persons, all persons' data is refreshed
        /// </summary>
        public static RefreshPersonsDataResult RefreshPersonsData()
        {
            RefreshPersonsDataResult ret = new RefreshPersonsDataResult();
            try
            {
                BrokerContext.Initialize(Application.BaseApplicationToken.ToString(), Constants.UserToken, true, false, true);

                // Refresh data provider list so that any changes are reflected here
                Manager.InitializeDataProviders();

                using (CPRBrokerDALDataContext dataContext = new CPRBrokerDALDataContext())
                {
                    var allPersonDataSubscription = (from s in dataContext.Subscriptions
                                                     where s.DataSubscription != null && s.IsForAllPersons
                                                     select s).FirstOrDefault();

                    IQueryable<Tasks.GetPersonDataTask> tasks;
                    if (allPersonDataSubscription != null)
                    {
                        tasks = from p in dataContext.Persons
                                select new Tasks.GetPersonDataTask() { CprNumber = p.PersonNumber };
                    }
                    else
                    {
                        tasks = from sp in dataContext.SubscriptionPersons
                                select new Tasks.GetPersonDataTask() { CprNumber = sp.Person.PersonNumber };
                    }
                    var tasksArray = tasks.Distinct().ToArray();
                    Tasks.TaskQueue.Main.Enqueue(tasksArray);
                    Tasks.TaskQueue.Main.WaitForFinish();

                    ret.SucceededCprNumbers.AddRange(from task in tasksArray where task.Result != null select task.CprNumber);
                    ret.FailedCprNumbers.AddRange(from task in tasksArray where task.Result == null select task.CprNumber);
                }
            }
            catch (Exception ex)
            {
                Local.Admin.LogException(ex);
            }
            return ret;
        }

        /// <summary>
        /// Sends the due notifications through their specified channels
        /// </summary>
        /// <param name="now"></param>
        public static SendNotificationsResult SendNotifications(DateTime now)
        {
            // Initialize
            SendNotificationsResult ret = new SendNotificationsResult();
            BrokerContext.Initialize(Application.BaseApplicationToken.ToString(), Constants.UserToken, true, false, true);
            DateTime today = now.Date;
            DateTime yesterday = today.AddDays(-1);
            try
            {
                using (CPRBrokerDALDataContext dataContext = new CPRBrokerDALDataContext())
                {
                    // Find all due subscriptions
                    System.Data.Linq.DataLoadOptions loadOptions = new DataLoadOptions();
                    DAL.Subscription.SetLoadOptionsForChildren(loadOptions);
                    dataContext.LoadOptions = loadOptions;

                    var dueSubscriptions = dataContext.GetDueNotifications(today, yesterday).ToArray();

                    // Loop over all subscriptions and sends notifications through their channels
                    foreach (var dueSub in dueSubscriptions)
                    {
                        try
                        {
                            Notification notif = null;
                            if (dueSub.BirthdateSubscription != null)
                            {
                                notif = dataContext.InsertBirthdateNotificationData(dueSub.SubscriptionId, today).SingleOrDefault();
                            }
                            else if (dueSub.DataSubscription != null)
                            {
                                notif = dataContext.InsertChangeNotificationData(dueSub.SubscriptionId, today, yesterday).SingleOrDefault();
                            }

                            Notifications.Channel channel = Notifications.Channel.Create(dueSub.Channels.Single());
                            channel.Notify(notif);
                            ret.SentNotificationIds.Add(dueSub.SubscriptionId);
                            Local.Admin.LogNotificationSuccess(dueSub.Application.Token, dueSub.SubscriptionId, dueSub.Channels.Single().Url);
                        }
                        catch (Exception ex)
                        {
                            ret.FailedNotificationIds.Add(dueSub.SubscriptionId);
                            Local.Admin.LogNotificationException(ex, dueSub.Application.Token, dueSub.SubscriptionId, dueSub.Channels.Single().Url);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Local.Admin.LogException(ex);
            }
            return ret;
        }
    }
}
