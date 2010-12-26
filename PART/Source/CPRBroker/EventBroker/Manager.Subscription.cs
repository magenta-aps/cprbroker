using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas;
using CPRBroker.Engine;

namespace CprBroker.EventBroker
{
    public static partial class Manager
    {
        /// <summary>
        /// This part contains methods related to admin interface
        /// All methods here simply delegate the code to Manager.CallMethod&lt;&gt;()
        /// </summary>
        public class Subscriptions
        {
            public static TOutput CallMethod<TInterface, TOutput>(string userToken, string appToken, bool failIfNoApp, bool allowLocalProvider, Func<TInterface, TOutput> func, bool failOnDefaultOutput, Action<TOutput> updateMethod) where TInterface : class, IDataProvider
            {
                return CPRBroker.Engine.Manager.CallMethod<TInterface, TOutput>(userToken, appToken, failIfNoApp, allowLocalProvider, func, failOnDefaultOutput, updateMethod);
            }
            #region Subscription
            public static ChangeSubscriptionType Subscribe(string userToken, string appToken, ChannelBaseType notificationChannel, Guid[] PersonCivilRegistrationIdentifiers)
            {
                return CallMethod<ISubscriptionManager, ChangeSubscriptionType>
                 (userToken, appToken, true, true, (admin) => admin.Subscribe(userToken, appToken, notificationChannel, PersonCivilRegistrationIdentifiers), true, null);
            }

            public static bool Unsubscribe(string userToken, string appToken, Guid subscriptionId)
            {
                return CallMethod<ISubscriptionManager, bool>
                 (userToken, appToken, true, true, (admin) => admin.Unsubscribe(userToken, appToken, subscriptionId), true, null);
            }

            public static BirthdateSubscriptionType SubscribeOnBirthdate(string userToken, string appToken, ChannelBaseType notificationChannel, Nullable<int> years, int priorDays, Guid[] PersonCivilRegistrationIdentifiers)
            {
                return CallMethod<ISubscriptionManager, BirthdateSubscriptionType>
                 (userToken, appToken, true, true, (admin) => admin.SubscribeOnBirthdate(userToken, appToken, notificationChannel, years, priorDays, PersonCivilRegistrationIdentifiers), true, null);
            }

            public static bool RemoveBirthDateSubscription(string userToken, string appToken, Guid subscriptionId)
            {
                return CallMethod<ISubscriptionManager, bool>
                 (userToken, appToken, true, true, (admin) => admin.RemoveBirthDateSubscription(userToken, appToken, subscriptionId), true, null);
            }

            public static SubscriptionType[] GetActiveSubscriptionsList(string userToken, string appToken)
            {
                return CallMethod<ISubscriptionManager, CPRBroker.Schemas.SubscriptionType[]>
                 (userToken, appToken, true, true, (admin) => admin.GetActiveSubscriptionsList(userToken, appToken), true, null);
            }

            public static BaseNotificationType GetLatestNotification(string userToken, string appToken, Guid subscriptionId)
            {
                return CallMethod<ISubscriptionManager, BaseNotificationType>
                 (userToken, appToken, true, true, (admin) => admin.GetLatestNotification(userToken, appToken, subscriptionId), true, null);
            }
            #endregion
        }
    }
}
