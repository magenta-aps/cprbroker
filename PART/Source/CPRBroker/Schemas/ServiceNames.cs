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
        public static class Person
        {
            public const string Service = "CPRPersonWS";

            public static class MethodNames
            {
                public const string GetCitizenBasic = "GetCitizenBasic";
                public const string GetCitizenNameAndAddress = "GetCitizenNameAndAddress";
                public const string GetCitizenFull = "GetCitizenFull";
                public const string GetCitizenRelations = "GetCitizenRelations";
                public const string GetCitizenChildren = "GetCitizenChildren";
                public const string RemoveParentAuthorityOverChild = "RemoveParentAuthorityOverChild";
                public const string SetParentAuthorityOverChild = "SetParentAuthorityOverChild";
                public const string GetParentAuthorityOverChildChanges = "GetParentAuthorityOverChildChanges";
            }
        }

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
                public const string CreateTestCitizen = "CreateTestCitizen";
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

        public static class Access
        {
            public const string Service = "Access";

            public static class MethodNames
            {
                public const string SendNotifications = "SendNotifications";
                public const string RefreshPersonsData = "RefreshPersonsData";
            }
        }
    }
}
