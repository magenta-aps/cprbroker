using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NUnitTester
{
    [NUnit.Framework.TestFixture]
    public class TestAdminProviderListVersion
    {
        private string appToken = "1de09be5-8382-4dc1-a89b-8f86fc67f97a";
        private string userToken = "peter";
        
        private NUnitTester.CPRAdministrationWS.CPRAdministrationWS CreateAdminWebService()
        {
            CPRAdministrationWS.CPRAdministrationWS ws = new NUnitTester.CPRAdministrationWS.CPRAdministrationWS();
            ws.ApplicationHeaderValue = new  NUnitTester.CPRAdministrationWS.ApplicationHeader()
            {
                ApplicationToken = appToken,
                UserToken = userToken
            };
            return ws;
        }

        [NUnit.Framework.Test]
        public void GetCapabilities()
        {
            CPRAdministrationWS.ServiceVersionType[] list;
            CPRAdministrationWS.CPRAdministrationWS admin = CreateAdminWebService();
            list=admin.GetCapabilities();
            Assert.IsNotNull(list);
            Assert.Greater(list.Length, 0);
        }

        [NUnit.Framework.Test]
        public void IsImplementing()
        {
            CPRAdministrationWS.CPRAdministrationWS admin = CreateAdminWebService();
            bool output = admin.IsImplementing( "GetCitizenChildren", "1.0");
            Assert.IsTrue(output);
        }

        [NUnit.Framework.Test]
        public void GetCPRDataProviderList()
        {
            CPRAdministrationWS.DataProviderType[] list = null;
            CPRAdministrationWS.CPRAdministrationWS admin = CreateAdminWebService();
            Console.WriteLine(admin.Url);
            list = admin.GetDataProviderList();
            Assert.Greater(list.Length, 0);
        }

        [NUnit.Framework.Test]
        public void SetCPRDataProverList()
        {
            CPRAdministrationWS.CPRAdministrationWS admin = CreateAdminWebService();
            bool codeType = admin.SetDataProviderList(CreateSampleDataProviderList());
            Assert.IsTrue(codeType);
        }

        private NUnitTester.CPRAdministrationWS.DataProviderType[] CreateSampleDataProviderList()
        {
            List<CPRAdministrationWS.DataProviderType> list = new List<NUnitTester.CPRAdministrationWS.DataProviderType>();

            CPRAdministrationWS.DprDataProviderType provider1 = new NUnitTester.CPRAdministrationWS.DprDataProviderType();
            provider1.Address = "1.0.0.127";
            provider1.Port = 1000;
            provider1.ConnectionString = "aklsjdklasjd";
            list.Add(provider1);


            CPRAdministrationWS.KmdDataProviderType provider2 = new NUnitTester.CPRAdministrationWS.KmdDataProviderType();
            provider2.Address = "1.0.0.127";
            list.Add(provider2);

            return list.ToArray();
        }
    }
}
