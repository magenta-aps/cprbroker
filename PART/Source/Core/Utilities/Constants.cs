using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Utilities
{
    public static class Constants
    {
        

        public static readonly DateTime MinSqlDate = new DateTime(1753, 1, 1);
        public static readonly DateTime MaxSqlDate = new DateTime(9999, 12, 31);

        /// <summary>
        /// Token of preapproved base application that comes with a first installation
        /// </summary>
        public static readonly Guid BaseApplicationId = new Guid("3E9890FF-0038-42A4-987A-99B63E8BC865");
        public static readonly Guid BaseApplicationToken = new Guid("07059250-E448-4040-B695-9C03F9E59E38");
        public static readonly string UserToken = "";

        public static class Logging
        {
            public static readonly string Category = "General";
            public static readonly string ApplicationId = "ApplicationId";
            public static readonly string ApplicationToken = "ApplicationToken";
            public static readonly string ApplicationName = "ApplicationName";
            public static readonly string UserToken = "UserToken";
            public static readonly string UserName = "UserName";
            public static readonly string MethodName = "MethodName";
            public static readonly string DataObjectType = "DataObjectType";
            public static readonly string DataObjectXml = "DataObjectXml";
        }

        public sealed class Versioning
        {
            public static readonly int Major = 1;
            public static readonly int Minor = 0;
        }

        public static readonly Guid EventBrokerApplicationId = new Guid("{C98F9BE7-2DDE-404a-BAB5-5A7B1BBC3063}");
        public static readonly Guid EventBrokerApplicationToken = new Guid("{FCD568A0-8F18-4b6f-8691-C09239F158F3}");
    }
}
