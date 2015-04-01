using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CprBroker.Providers.ServicePlatform;

namespace CprBroker.Tests.ServicePlatform
{
    public class ServicePlatformDataProviderFactory
    {
        public static ServicePlatformDataProvider Create()
        {
            var lines = File.ReadAllLines(@"..\..\demo.dat");
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

            var prov = new CprBroker.Providers.ServicePlatform.ServicePlatformDataProvider()
            {
                ConfigurationProperties = new Dictionary<string, string>(),
                Url = "https://exttest.serviceplatformen.dk",
                UserSystemUUID = dic["UserSystemUUID".ToLower()],
                ServiceAgreementUuid = dic["ServiceAgreementUuid".ToLower()],
                UserUUID = dic["UserUUID".ToLower()],
                CertificateSerialNumber = dic["CertificateSerialNumber".ToLower()]
            };

            return prov;
        }
    }
}
