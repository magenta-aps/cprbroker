using System.Collections.Generic;
using CprBroker.EventBroker.Data;

namespace CprBroker.Installers.EventBrokerInstallers.Properties
{
    using System;


    public static class ResourcesExtensions
    {
        public static String AllEventBrokerDatabaseObjectsSql
        {
            get
            {
                var arr = new string[] { 
                    Resources.SubscriptionType_Create,
                    // subscription
                    Resources.DataSubscription,
                    Resources.BirthdateSubscription,
                    Resources.ChannelType_Create,
                    Resources.Channel,
                    //event notification
                    Resources.BirthdateEventNotification,
                    Resources.CreateEventBrokerDatabaseObjects,
                    Resources.EnqueueBirthdateEventNotifications,
                    Resources.EnqueueDataChangeEventNotifications,
                    Resources.IsBirthdateEvent,
                    Resources.UpdatePersonLists
                };

                return string.Join(
                    Environment.NewLine + "GO" + Environment.NewLine,
                    arr);
            }
        }

        public static KeyValuePair<string, string>[] Lookups
        {
            get { 
                List<KeyValuePair<string, string>> eventLookups = new List<KeyValuePair<string, string>>();

                eventLookups.Add(new KeyValuePair<string, string>(typeof(ChannelType).Name, CprBroker.Installers.EventBrokerInstallers.Properties.Resources.ChannelType));
                eventLookups.Add(new KeyValuePair<string, string>(typeof(SubscriptionType).Name, CprBroker.Installers.EventBrokerInstallers.Properties.Resources.SubscriptionType));
                return eventLookups.ToArray();
            }
        }
    }

}
