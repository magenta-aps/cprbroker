using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.NUnitTester
{
    /// <summary>
    /// Contains members that manage the overall running of unit tests
    /// </summary>
    public static class TestRunner
    {

        public static Admin.Admin AdminService;
        public static Part.Part PartService;
        public static Subscriptions.Subscriptions SubscriptionsService;
        public static Events.Events EventsService;

        public static void Initialize()
        {
            AdminService = new NUnitTester.Admin.Admin();
            AdminService.ApplicationHeaderValue = new NUnitTester.Admin.ApplicationHeader()
            {
                ApplicationToken = TestData.BaseAppToken,
                UserToken = TestData.userToken
            };
            ReplaceServiceUrl(AdminService, SystemType.CprBroker);
            Console.WriteLine(AdminService.Url);

            PartService = new NUnitTester.Part.Part();
            PartService.ApplicationHeaderValue = new NUnitTester.Part.ApplicationHeader()
            {
                ApplicationToken = TestData.BaseAppToken,
                UserToken = TestData.userToken
            };

            ReplaceServiceUrl(PartService, SystemType.CprBroker);
            Console.WriteLine(PartService.Url);


            SubscriptionsService = new NUnitTester.Subscriptions.Subscriptions();
            SubscriptionsService.ApplicationHeaderValue = new NUnitTester.Subscriptions.ApplicationHeader()
            {
                ApplicationToken = TestData.BaseAppToken,
                UserToken = TestData.userToken
            };
            ReplaceServiceUrl(SubscriptionsService, SystemType.EventBroker);
            Console.WriteLine(SubscriptionsService.Url);

            EventsService = new CprBroker.NUnitTester.Events.Events();
            EventsService.ApplicationHeaderValue = new NUnitTester.Events.ApplicationHeader()
            {
                ApplicationToken = TestData.BaseAppToken,
                UserToken = TestData.userToken
            };
            ReplaceServiceUrl(EventsService, SystemType.CprBroker);
            Console.WriteLine(EventsService.Url);

        }

        private enum SystemType
        {
            CprBroker,
            EventBroker
        }
        private static void ReplaceServiceUrl(System.Web.Services.Protocols.SoapHttpClientProtocol service, SystemType systemType)
        {
            Uri uri = new Uri(service.Url);
            string hostAndPort = uri.Host;
            if (!uri.IsDefaultPort)
            {
                hostAndPort += ":" + uri.Port;
            }

            if (systemType == SystemType.CprBroker)
            {
                service.Url = service.Url.Replace(hostAndPort, "localhost:1551");
            }
            else
            {
                service.Url = service.Url.Replace(hostAndPort, "localhost:1552");
            }
        }
    }
}
