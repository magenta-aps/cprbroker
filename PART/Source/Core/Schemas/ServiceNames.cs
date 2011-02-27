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
        

        public static class Administrator
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
       
    }
}
