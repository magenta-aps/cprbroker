using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas;

namespace CPRBroker.Engine
{
    public static partial class Manager
    {
        /// <summary>
        /// This part contains methods related to admin interface
        /// All methods here simply delegate the code to Manager.CallMethod&lt;&gt;()
        /// </summary>
        public class Admin
        {
            #region Versioning management
            public static ServiceVersionType[] GetCapabilities(string userToken, string appToken)
            {
                return CallMethod<IVersionManager, ServiceVersionType[]>
                (userToken, appToken, true, true, (admin) => admin.GetCapabilities(userToken, appToken), true, null);
            }
            public static bool IsImplementing(string userToken, string appToken, string methodName, string version)
            {
                return CallMethod<IVersionManager, bool>
                (userToken, appToken, true, true, (admin) => admin.IsImplementing(userToken, appToken, methodName, version), false, null);
            }
            #endregion

            #region Provider list
            public static Schemas.DataProviderType[] GetDataProviderList(string userToken, string appToken)
            {
                return CallMethod<IDataProviderManager, DataProviderType[]>
                (userToken, appToken, true, true, (admin) => admin.GetCPRDataProviderList(userToken, appToken), true, null);
            }
            public static bool SetDataProviderList(string userToken, string appToken, DataProviderType[] dataProviders)
            {
                return CallMethod<IDataProviderManager, bool>
                (userToken, appToken, true, true, (admin) => admin.SetCPRDataProviderList(userToken, appToken, dataProviders), true, null);
            }
            #endregion

            #region Test User
            public static bool CreateTestCitizen(string userToken, string appToken, PersonFullStructureType oioPerson)
            {
                return CallMethod<ITestCitizenManager, bool>
                (userToken, appToken, true, true, (admin) => admin.CreateTestCitizen(userToken, appToken, oioPerson), true, null);
            }
            #endregion

            #region Application
            public static ApplicationType RequestAppRegistration(string userToken, string name)
            {
                return CallMethod<IApplicationManager, ApplicationType>
                (userToken, "", false, true, (admin) => admin.RequestAppRegistration(userToken, name), true, null);
            }

            public static bool ApproveAppRegistration(string userToken, string appToken, string targetAppToken)
            {
                return CallMethod<IApplicationManager, bool>
                (userToken, appToken, true, true, (admin) => admin.ApproveAppRegistration(userToken, appToken, targetAppToken), true, null);
            }

            public static ApplicationType[] ListAppRegistrations(string userToken, string appToken)
            {
                return CallMethod<IApplicationManager, ApplicationType[]>
                (userToken, appToken, true, true, (admin) => admin.ListAppRegistration(userToken, appToken), true, null);
            }

            public static bool UnregisterApp(string userToken, string appToken, string targetAppToken)
            {
                return CallMethod<IApplicationManager, bool>
                (userToken, appToken, true, true, (admin) => admin.UnregisterApp(userToken, appToken, targetAppToken), true, null);
            }
            #endregion

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
                return CallMethod<ISubscriptionManager, Schemas.SubscriptionType[]>
                 (userToken, appToken, true, true, (admin) => admin.GetActiveSubscriptionsList(userToken, appToken), true, null);
            }

            public static BaseNotificationType GetLatestNotification(string userToken, string appToken, Guid subscriptionId)
            {
                return CallMethod<ISubscriptionManager, BaseNotificationType>
                 (userToken, appToken, true, true, (admin) => admin.GetLatestNotification(userToken, appToken, subscriptionId), true, null);
            }
            #endregion

            #region Logging

            public static bool Log(string userToken, string appToken, string text)
            {
                return CallMethod<ILoggingDataProvider, bool>(userToken, appToken, true, true, (ILoggingDataProvider log) => log.Log(userToken, appToken, text), true, null);
            }
            #endregion

        }
    }
}
