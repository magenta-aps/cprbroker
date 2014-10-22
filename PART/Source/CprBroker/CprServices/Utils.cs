using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;

namespace CprBroker.Providers.CprServices
{
    public class Utils
    {
        public static string GetToken(WebHeaderCollection headers)
        {
            string token = null;
            var cookieHeader = headers["Set-Cookie"];
            if (cookieHeader != null)
            {
                var pat = @"\AToken=(?<token>[^;]+);";
                var m = Regex.Match(cookieHeader, pat);
                if (m.Success)
                    token = m.Groups["token"].Value;
            }
            return token;
        }
    }
}
