using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public static partial class ServiceNames
    {
        public const string Namespace = "http://dk.itst";

        public class Part
        {
            public const string Service = "Part";

            public static class Methods
            {
                public const string Read = "Read";
                public const string RefreshRead = "RefreshRead ";
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
                public const string Enqueue= "Enqueue";

            }
        }
    }
}
