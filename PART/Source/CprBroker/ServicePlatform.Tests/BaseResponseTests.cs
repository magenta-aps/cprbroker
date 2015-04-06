using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CprBroker.Tests.ServicePlatform
{
    public class BaseResponseTests
    {
        public string[] PNRs { get { return CPRDirect.Utilities.PNRs; } }
        public string GetResponse(string pnr, string methodName)
        {
            var txt = File.ReadAllText(string.Format(@"Resources\{0}.{1}.Response.OK.xml", pnr,methodName), Encoding.UTF8);
            return txt;
        }
    }
}
