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
                var arr = new List<string>();

                arr.AddRange(new EventBrokerDataContext().DDL);

                return string.Join(
                    Environment.NewLine + "GO" + Environment.NewLine,
                    arr);
            }
        }

        public static string AllEventBrokerStoredProceduresSql
        {
            get
            {
                var arr = new EventBrokerDataContext().DDL_Logic;

                return string.Join(
                    Environment.NewLine + "GO" + Environment.NewLine,
                    arr);
            }
        }

        public static KeyValuePair<string, string>[] Lookups
        {
            get
            {
                List<KeyValuePair<string, string>> eventLookups = new List<KeyValuePair<string, string>>();

                eventLookups.AddRange(new EventBrokerDataContext().Lookups);

                return eventLookups.ToArray();
            }
        }
    }

}
