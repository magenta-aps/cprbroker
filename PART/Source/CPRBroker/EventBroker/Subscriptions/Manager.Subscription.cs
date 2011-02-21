using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Engine;

namespace CprBroker.EventBroker.Subscriptions
{
    public static partial class Manager
    {
        /// <summary>
        /// This part contains methods related to admin interface
        /// All methods here simply delegate the code to Manager.CallMethod&lt;&gt;()
        /// </summary>
        public class Subscriptions
        {
            public static BasicOutputType<TItem> GetMethodOutput<TItem>(GenericFacadeMethodInfo<TItem> facade)
            {
                return CprBroker.Engine.Manager.GetMethodOutput<TItem>(facade);
            }

            #region Subscription
            public static BasicOutputType<ChangeSubscriptionType> Subscribe(string userToken, string appToken, ChannelBaseType notificationChannel, Guid[] personUuids)
            {
                SubscribeFacadeMethod facade = new SubscribeFacadeMethod(notificationChannel, personUuids, appToken, userToken);
                return GetMethodOutput<ChangeSubscriptionType>(facade);

            }

            public static BasicOutputType<bool> Unsubscribe(string userToken, string appToken, Guid subscriptionId)
            {
                UnsubscribeFacadeMethod facade = new UnsubscribeFacadeMethod(subscriptionId, CprBroker.EventBroker.DAL.SubscriptionType.SubscriptionTypes.DataChange, appToken, userToken);
                return GetMethodOutput<bool>(facade);
            }

            public static BasicOutputType<BirthdateSubscriptionType> SubscribeOnBirthdate(string userToken, string appToken, ChannelBaseType notificationChannel, Nullable<int> years, int priorDays, Guid[] PersonCivilRegistrationIdentifiers)
            {
                SubscribeOnBirthdateFacadeMethod facade = new SubscribeOnBirthdateFacadeMethod(notificationChannel, years, priorDays, PersonCivilRegistrationIdentifiers, appToken, userToken);
                return GetMethodOutput<BirthdateSubscriptionType>(facade);
            }

            public static BasicOutputType<bool> RemoveBirthDateSubscription(string userToken, string appToken, Guid subscriptionId)
            {
                UnsubscribeFacadeMethod facade = new UnsubscribeFacadeMethod(subscriptionId, CprBroker.EventBroker.DAL.SubscriptionType.SubscriptionTypes.Birthdate, appToken, userToken);
                return GetMethodOutput<bool>(facade);
            }

            public static BasicOutputType<SubscriptionType[]> GetActiveSubscriptionsList(string userToken, string appToken)
            {
                GetActiveSubscriptionsListFacadeMethod facade = new GetActiveSubscriptionsListFacadeMethod(appToken, userToken);
                return GetMethodOutput<SubscriptionType[]>(facade);
            }

            #endregion

        }
    }
}
