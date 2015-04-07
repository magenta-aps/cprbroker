using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.CprServices;
using CprBroker.PartInterface;
using CprBroker.Engine.Part;

namespace CprBroker.Tests.CprServices
{
    public class CprServicesDataProviderFactory
    {
        public static CprServicesDataProvider Create()
        {
            var lines = System.IO.File.ReadAllLines(@"..\..\demo.dat");
            var dic = new Dictionary<string, string>();
            foreach (var l in lines)
            {
                var d = l.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (d.Count() > 0)
                {
                    var key = d.First().TrimStart().TrimEnd();
                    var val = d.Count() > 1 ? d[1].TrimStart().TrimEnd() : "";
                    dic[key.ToLower()] = val;
                }
            }

            var prov = new CprServicesDataProvider()
            {
                ConfigurationProperties = new Dictionary<string, string>(),
                Address = dic["Address".ToLower()],
                UserId = dic["UserId".ToLower()],
                Password = dic["Password".ToLower()],
            };

            return prov;
        }
    }
}
