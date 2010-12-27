using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data.Linq;
using CPRBroker.DAL;
using CprBroker.EventBroker.DAL;
using CPRBroker.Engine;

namespace CprBroker.EventBroker.Notifications
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
                BrokerContext.Initialize(CPRBroker.DAL.Applications.Application.BaseApplicationToken.ToString(), CPRBroker.Engine.Constants.UserToken, true, false, true);

                // Refresh data provider list so that any changes are reflected here
                DataProviderManager.InitializeDataProviders();

                using (EventBrokerDataContext dataContext = new EventBrokerDataContext())
                {
                    var allPersonDataSubscription = (from s in dataContext.Subscriptions
                                                     where s.DataSubscription != null && s.IsForAllPersons
                                                     select s).FirstOrDefault();

                    IQueryable<GetPersonDataTask> tasks = null;
                    if (allPersonDataSubscription != null)
                    {
                        // TODO: Handle the case of all persons
                        //tasks = from p in dataContext.Persons
                        //        select new Tasks.GetPersonDataTask() { CprNumber = p.PersonNumber };
                    }
                    else
                    {
                        // TODO: Handle the case of specific persons
                        //tasks = from sp in dataContext.SubscriptionPersons
                        //        select new Tasks.GetPersonDataTask() { CprNumber = sp.Person.PersonNumber };
                    }
                    var tasksArray = tasks.Distinct().ToArray();
                    CPRBroker.Engine.Tasks.TaskQueue.Main.Enqueue(tasksArray);
                    CPRBroker.Engine.Tasks.TaskQueue.Main.WaitForFinish();

                    ret.SucceededCprNumbers.AddRange(from task in tasksArray where task.Result != null select task.CprNumber);
                    ret.FailedCprNumbers.AddRange(from task in tasksArray where task.Result == null select task.CprNumber);
                }
            }
            catch (Exception ex)
            {
                CPRBroker.Engine.Local.Admin.LogException(ex);
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
            BrokerContext.Initialize(CPRBroker.DAL.Applications.Application.BaseApplicationToken.ToString(), CPRBroker.Engine.Constants.UserToken, true, false, true);
            DateTime today = now.Date;
            DateTime yesterday = today.AddDays(-1);
            try
            {
                using (EventBrokerDataContext dataContext = new EventBrokerDataContext())
                {
                    // Find all due subscriptions
                    System.Data.Linq.DataLoadOptions loadOptions = new DataLoadOptions();
                    DAL.Subscription.SetLoadOptionsForChildren(loadOptions);
                    dataContext.LoadOptions = loadOptions;

                    // TODO: Get the data from stored procedure
                    //var dueSubscriptions = dataContext.GetDueNotifications(today, yesterday).ToArray();
                    var dueSubscriptions = dataContext.Subscriptions;

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
                                // TODO: Fix this
                                //notif = dataContext.InsertChangeNotificationData(dueSub.SubscriptionId, today, yesterday).SingleOrDefault();
                            }

                            if (notif != null)
                            {
                                Notifications.Channel channel = Notifications.Channel.Create(dueSub.Channels.Single());
                                channel.Notify(notif);
                                ret.SentNotificationIds.Add(dueSub.SubscriptionId);
                                CPRBroker.Engine.Local.Admin.LogNotificationSuccess(dueSub.Application.Token, dueSub.SubscriptionId, dueSub.Channels.Single().Url);
                            }
                        }
                        catch (Exception ex)
                        {
                            ret.FailedNotificationIds.Add(dueSub.SubscriptionId);
                            CPRBroker.Engine.Local.Admin.LogNotificationException(ex, dueSub.Application.Token, dueSub.SubscriptionId, dueSub.Channels.Single().Url);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CPRBroker.Engine.Local.Admin.LogException(ex);
            }
            return ret;
        }
    }
}
