using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    /// <summary>
    /// Contains service and message names
    /// </summary>
    public partial class ServiceNames
    {
        public const string Namespace = "http://dk.itst";

        public static class Admin
        {
            public const string Service = "Admin";

            public static class MethodNames
            {
                public const string RequestAppRegistration = "RequestAppRegistration";
                public const string ApproveAppRegistration = "ApproveAppRegistration";
                public const string ListAppRegistrations = "ListAppRegistrations";
                public const string UnregisterApp = "UnregisterApp";
                public const string GetCapabilities = "GetCapabilities";
                public const string IsImplementing = "IsImplementing";
                public const string GetDataProviderList = "GetDataProviderList";
                public const string SetDataProviderList = "SetDataProviderList";
                public const string Log = "Log";
            }
        }

        public static class Notification
        {
            public const string Service = "Notification";

            public static class MethodNames
            {
                public const string Notify = "Notify";
                public const string Ping = "Ping";
            }
        }

        public class Part
        {
            public const string Service = "Part";

            public static class Methods
            {
                public const string Read = "Read";
                public const string RefreshRead = "RefreshRead";
                public const string List = "List";
                public const string Search = "Search";

                public const string GetUuid = "GetUuid";
            }
        }

        public class Subscriptions
        {
            public const string Service = "Subscriptions";

            public static class Methods
            {
                public const string Subscribe = "Subscribe";
                public const string Unsubscribe = "Unsubscribe";
                public const string SubscribeOnBirthdate = "SubscribeOnBirthdate";

                public const string GetActiveSubscriptionsList = "GetActiveSubsciptionsList";
                public const string GetLatestNotification = "GetLatestNotification";

                public const string RemoveBirthDateSubscription = "RemoveBirthDateSubscription";

            }
        }

        public class NotificationQueue
        {
            public const string Service = "NotificationQueue";

            public static class Methods
            {
                public const string Enqueue = "Enqueue";

            }
        }

    }
}
