using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
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
            public static TOutput CallMethod<TInterface, TOutput>(string userToken, string appToken, bool failIfNoApp, bool allowLocalProvider, Func<TInterface, TOutput> func, bool failOnDefaultOutput, Action<TOutput> updateMethod) where TInterface : class, IDataProvider
            {
                return CprBroker.Engine.Manager.CallMethod<TInterface, TOutput>(userToken, appToken, failIfNoApp, allowLocalProvider, func, failOnDefaultOutput, updateMethod);
            }
            #region Subscription
            public static ChangeSubscriptionType Subscribe(string userToken, string appToken, ChannelBaseType notificationChannel, Guid[] PersonCivilRegistrationIdentifiers)
            {
                return CallMethod<ISubscriptionDataProvider, ChangeSubscriptionType>
                 (userToken, appToken, true, true, (admin) => admin.Subscribe(userToken, appToken, notificationChannel, PersonCivilRegistrationIdentifiers), true, null);
            }

            public static bool Unsubscribe(string userToken, string appToken, Guid subscriptionId)
            {
                return CallMethod<ISubscriptionDataProvider, bool>
                 (userToken, appToken, true, true, (admin) => admin.Unsubscribe(userToken, appToken, subscriptionId), true, null);
            }

            public static BirthdateSubscriptionType SubscribeOnBirthdate(string userToken, string appToken, ChannelBaseType notificationChannel, Nullable<int> years, int priorDays, Guid[] PersonCivilRegistrationIdentifiers)
            {
                return CallMethod<ISubscriptionDataProvider, BirthdateSubscriptionType>
                 (userToken, appToken, true, true, (admin) => admin.SubscribeOnBirthdate(userToken, appToken, notificationChannel, years, priorDays, PersonCivilRegistrationIdentifiers), true, null);
            }

            public static bool RemoveBirthDateSubscription(string userToken, string appToken, Guid subscriptionId)
            {
                return CallMethod<ISubscriptionDataProvider, bool>
                 (userToken, appToken, true, true, (admin) => admin.RemoveBirthDateSubscription(userToken, appToken, subscriptionId), true, null);
            }

            public static SubscriptionType[] GetActiveSubscriptionsList(string userToken, string appToken)
            {
                return CallMethod<ISubscriptionDataProvider, CprBroker.Schemas.SubscriptionType[]>
                 (userToken, appToken, true, true, (admin) => admin.GetActiveSubscriptionsList(userToken, appToken), true, null);
            }

            public static BaseNotificationType GetLatestNotification(string userToken, string appToken, Guid subscriptionId)
            {
                return CallMethod<ISubscriptionDataProvider, BaseNotificationType>
                 (userToken, appToken, true, true, (admin) => admin.GetLatestNotification(userToken, appToken, subscriptionId), true, null);
            }
            #endregion

            /*public static bool Enqueue(string userToken, string appToken,Guid personUuid)
            {
                return CallMethod<INotificationManager, bool>
                    (userToken, appToken, true, true, (prov) => prov.Enqueue(personUuid), true, null);
            }*/
        }
    }
}
