namespace CprBroker.Installers.EventBrokerInstallers.Properties
{
    using System;


    public static class ResourcesExtensions
    {
        public static String AllEventBrokerDatabaseObjectsSql()
        {
            var arr = new string[] { 
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

}
