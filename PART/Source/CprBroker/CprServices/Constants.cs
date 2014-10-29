using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CprServices
{
    public static class Constants
    {
        public static class OperationKeys
        {
            public const string ADRSOG1 = "ADRSOG1";
            public const string newpass = "newpass";
            public const string signon = "signon";
        }

        public static class ConfigKeys
        {
            public const string Address = "Address";
            public const string UserId = "User Id";
            public const string Password = "Password";
        }

        public const string XmlNamespace = "http://www.cpr.dk"; 
        public static readonly Encoding XmlEncoding = Encoding.GetEncoding("iso-8859-1");
        
        public const string UserAgent = "CPR/1.0";
        public const string TokenCookieName = "TOKEN";
        public const string DefaultToken = "ZZZxxxxxxxx";

        public static readonly short DenmarkCountryCode = 5100;
        
    }
}
