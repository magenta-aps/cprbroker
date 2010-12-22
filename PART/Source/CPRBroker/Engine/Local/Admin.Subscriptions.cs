using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas;
using CPRBroker.DAL;
using CPRBroker.DAL.Events;

namespace CPRBroker.Engine.Local
{
    public partial class Admin
    {
        /// <summary>
        /// Adds a new Subscription object to the <paramref name="dataContext"/>
        /// This method inserts rows in Subscription, Channel (and children) and  SubscriptionPerson
        /// A later call to SubmitChanges() is required to save to the database
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="subscriptionType"></param>
        /// <param name="applicationId"></param>
        /// <param name="PersonCivilRegistrationIdentifiers"></param>
        /// <param name="notificationChannel"></param>
        /// <returns></returns>
        private Subscription AddSubscription(EventBrokerDataContext dataContext, DAL.Events.SubscriptionType.SubscriptionTypes subscriptionType, Guid applicationId, Guid[] personUuids, ChannelBaseType notificationChannel)
        {
            List<Guid> listOfPersonsIDs = new List<Guid>();

            Subscription subscription = new Subscription();
            subscription.SubscriptionId = Guid.NewGuid();
            subscription.SubscriptionTypeId = (int)subscriptionType;
            subscription.ApplicationId = applicationId;

            
            #region Get IDs of the persons that match the person's CPR numbers
            if (personUuids != null && personUuids.Length > 0)
            {                
                subscription.IsForAllPersons = false;
            }
            else
            {
                subscription.IsForAllPersons = true;
            }
            #endregion

            #region Set the channel

            Channel dbChannel = new Channel();
            dbChannel.ChannelId = Guid.NewGuid();
            dbChannel.Subscription = subscription;
            
            if (notificationChannel is WebServiceChannelType)
            {
                WebServiceChannelType webServiceChannel = notificationChannel as WebServiceChannelType;
                dbChannel.ChannelTypeId = (int)ChannelType.ChannelTypes.WebService;
                dbChannel.Url = webServiceChannel.WebServiceUrl;
            }
            else if (notificationChannel is FileShareChannelType)
            {
                FileShareChannelType fileShareChannel = notificationChannel as FileShareChannelType;
                dbChannel.ChannelTypeId = (int)ChannelType.ChannelTypes.FileShare;
                dbChannel.Url = fileShareChannel.Path;
            }            
            else
            {
                return null;
            }

            // Test The channel
            Notifications.Channel channel = Notifications.Channel.Create(dbChannel);
            if (channel == null || !channel.IsAlive())
            {
                return null;
            }
            #endregion

            // Mark the new objects to be inserted later
            dataContext.Subscriptions.InsertOnSubmit(subscription);

            if (!subscription.IsForAllPersons)
            {
                dataContext.SubscriptionPersons.InsertAllOnSubmit(
                    from personId in personUuids
                    select new SubscriptionPerson() { SubscriptionPersonId = Guid.NewGuid(), SubscriptionId = subscription.SubscriptionId, PersonId = personId }
                    );
            }
            dataContext.Channels.InsertOnSubmit(dbChannel);

            return subscription;
        }

        /// <summary>
        /// Deletes the given subscription from the database
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="subscriptionType"></param>
        /// <returns></returns>
        private bool DeleteSubscription(Guid subscriptionId, DAL.Events.SubscriptionType.SubscriptionTypes subscriptionType)
        {
            // Find the Subscription object and delete it and its children
            using (EventBrokerDataContext dataContext = new EventBrokerDataContext())
            {
                System.Data.Linq.DataLoadOptions loadOptions = new System.Data.Linq.DataLoadOptions();
                Subscription.SetLoadOptionsForChildren(loadOptions);
                dataContext.LoadOptions = loadOptions;

                var subscription = (from sub in dataContext.Subscriptions
                                    where sub.SubscriptionId == subscriptionId && sub.SubscriptionTypeId == (int)subscriptionType
                                    select sub
                                    ).SingleOrDefault();

                if (subscription != null)
                {
                    switch (subscriptionType)
                    {
                        case CPRBroker.DAL.Events.SubscriptionType.SubscriptionTypes.DataChange:
                            dataContext.DataSubscriptions.DeleteOnSubmit(subscription.DataSubscription);
                            break;
                        case CPRBroker.DAL.Events.SubscriptionType.SubscriptionTypes.Birthdate:
                            dataContext.BirthdateSubscriptions.DeleteOnSubmit(subscription.BirthdateSubscription);
                            break;
                    }
                    dataContext.SubscriptionPersons.DeleteAllOnSubmit(subscription.SubscriptionPersons);                    
                    dataContext.Channels.DeleteAllOnSubmit(subscription.Channels);
                    dataContext.Subscriptions.DeleteOnSubmit(subscription);
                    dataContext.SubmitChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Interface implementation
        /// </summary>
        public ChangeSubscriptionType Subscribe(string userToken, string appToken, ChannelBaseType notificationChannel, Guid[] personUuids)
        {
            using (EventBrokerDataContext dataContext = new EventBrokerDataContext())
            {
                DAL.Events.Subscription subscription = AddSubscription(dataContext, CPRBroker.DAL.Events.SubscriptionType.SubscriptionTypes.DataChange, BrokerContext.Current.ApplicationId.Value, personUuids, notificationChannel);
                if (subscription != null)
                {
                    subscription.DataSubscription = new DataSubscription();
                }
                dataContext.SubmitChanges();
                if (subscription != null)
                {
                    // Now get the subscription from the database to make sure everything is OK
                    return GetActiveSubscriptionsList(userToken, appToken, subscription.SubscriptionId).SingleOrDefault() as ChangeSubscriptionType;
                }
            }
            return null;
        }

        /// <summary>
        /// Interface implementation
        /// </summary>
        public bool Unsubscribe(string userToken, string appToken, Guid subscriptionId)
        {
            return DeleteSubscription(subscriptionId, CPRBroker.DAL.Events.SubscriptionType.SubscriptionTypes.DataChange);
        }

        /// <summary>
        /// Interface implementation
        /// </summary>
        public BirthdateSubscriptionType SubscribeOnBirthdate(string userToken, string appToken, ChannelBaseType notificationChannel, Nullable<int> years, int priorDays, Guid[] personUuids)
        {
            using (EventBrokerDataContext dataContext = new EventBrokerDataContext())
            {
                DAL.Events.Subscription subscription = AddSubscription(dataContext, CPRBroker.DAL.Events.SubscriptionType.SubscriptionTypes.Birthdate, BrokerContext.Current.ApplicationId.Value, personUuids, notificationChannel);
                if (subscription != null)
                {
                    subscription.BirthdateSubscription = new BirthdateSubscription();
                    subscription.BirthdateSubscription.AgeYears = years;
                    subscription.BirthdateSubscription.PriorDays = priorDays;
                }

                dataContext.SubmitChanges();
                if (subscription != null)
                {
                    // Now get the subscription from the database to make sure everything is OK
                    return GetActiveSubscriptionsList(userToken, appToken, subscription.SubscriptionId).SingleOrDefault() as BirthdateSubscriptionType;
                }
            }
            return null;
        }

        /// <summary>
        /// Interface implementation
        /// </summary>
        public bool RemoveBirthDateSubscription(string userToken, string appToken, Guid subscriptionId)
        {
            return DeleteSubscription(subscriptionId, CPRBroker.DAL.Events.SubscriptionType.SubscriptionTypes.Birthdate);
        }

        /// <summary>
        /// Interface implementation
        /// </summary>
        public Schemas.SubscriptionType[] GetActiveSubscriptionsList(string userToken, string appToken)
        {
            return GetActiveSubscriptionsList(userToken, appToken, null);
        }

        /// <summary>
        /// Retrieves the subscriptions that match the supplied criteria
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="appToken"></param>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        private Schemas.SubscriptionType[] GetActiveSubscriptionsList(string userToken, string appToken, Nullable<Guid> subscriptionId)
        {
            List<Schemas.SubscriptionType> listType = new List<CPRBroker.Schemas.SubscriptionType>();
            using (EventBrokerDataContext context = new EventBrokerDataContext())
            {
                System.Data.Linq.DataLoadOptions loadOptions = new System.Data.Linq.DataLoadOptions();
                Subscription.SetLoadOptionsForChildren(loadOptions);
                context.LoadOptions = loadOptions;

                // Create basic LINQ expression
                System.Linq.Expressions.Expression<Func<IQueryable<Subscription>>> exp =
                    () =>
                    from sub in context.Subscriptions
                    where sub.ApplicationId == BrokerContext.Current.ApplicationId
                    select sub;

                // Add filter for subscription id (if required)
                IQueryable<Subscription> subscriptions;
                if (subscriptionId.HasValue)
                {
                    subscriptions = exp.Compile()().Where((Subscription sub) => sub.SubscriptionId == subscriptionId);
                }
                else
                {
                    subscriptions = exp.Compile()();
                }

                // Now create list of OIO subscriptions
                foreach (var sub in subscriptions)
                {
                    CPRBroker.Schemas.SubscriptionType subscriptionType = sub.ToOioSubscription();
                    listType.Add(subscriptionType);
                }
                return listType.ToArray();
            }
        }

        /// <summary>
        /// Interface implementation
        /// </summary>
        public BaseNotificationType GetLatestNotification(string userToken, string appToken, Guid subscriptionId)
        {
            using (EventBrokerDataContext dataContext = new EventBrokerDataContext())
            {
                return (
                    from n in dataContext.Notifications
                    where n.SubscriptionId == subscriptionId
                    orderby n.NotificationDate descending, n.CreatedDate descending
                    select n.ToOioNotification()
                    ).FirstOrDefault();
            }
        }
    }
}
