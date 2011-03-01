using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace CprBroker.NUnitTester
{
    /// <summary>
    /// Contains members that manage the overall running of unit tests
    /// </summary>
    public static class TestRunner
    {

        public static Admin.Admin AdminService;
        public static Part.Part PartService;
        public static Subscriptions.SubscriptionsClient SubscriptionsService;
        public static Events.Events EventsService;
        public static Part.ApplicationHeader PartApplicationHeader;
        public static Admin.ApplicationHeader AdminApplicationHeader;
        public static Events.ApplicationHeader EventsApplicationHeader;
        public static Subscriptions.ApplicationHeader SubscriptionsApplicationHeader;

        public static void Initialize()
        {

            SubscriptionsApplicationHeader = new CprBroker.NUnitTester.Subscriptions.ApplicationHeader() { ApplicationToken = TestData.BaseAppToken, UserToken = TestData.userToken };

            // Admin
            AdminService = new NUnitTester.Admin.Admin();
            ReplaceServiceUrl(AdminService, SystemType.CprBroker);
            Console.WriteLine(AdminService.Url);

            AdminApplicationHeader = new NUnitTester.Admin.ApplicationHeader() { ApplicationToken = TestData.BaseAppToken, UserToken = TestData.userToken };
            AdminService.ApplicationHeaderValue = AdminApplicationHeader;

            // Part
            PartService = new CprBroker.NUnitTester.Part.Part();
            ReplaceServiceUrl(PartService, SystemType.CprBroker);
            Console.WriteLine(PartService.Url);

            PartApplicationHeader = new Part.ApplicationHeader() { ApplicationToken = TestData.BaseAppToken, UserToken = TestData.userToken };
            PartService.ApplicationHeaderValue = PartApplicationHeader;

            SubscriptionsService = new NUnitTester.Subscriptions.SubscriptionsClient();
            ReplaceServiceUrl(SubscriptionsService, SystemType.EventBroker);
            Console.WriteLine(SubscriptionsService.Endpoint.Address.Uri);

            EventsService = new CprBroker.NUnitTester.Events.Events();
            ReplaceServiceUrl(EventsService, SystemType.CprBroker);
            Console.WriteLine(EventsService.Url);

            EventsApplicationHeader = new CprBroker.NUnitTester.Events.ApplicationHeader() { ApplicationToken = TestData.BaseAppToken, UserToken = TestData.userToken };
            EventsService.ApplicationHeaderValue = EventsApplicationHeader;

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
            string url = uri.ToString();
            if (systemType == SystemType.CprBroker)
            {
                url = url.Replace(hostAndPort, "localhost:1551");
            }
            else
            {
                url = url.Replace(hostAndPort, "localhost:1552");
            }
            service.Url = url;
        }
        private static void ReplaceServiceUrl<T>(System.ServiceModel.ClientBase<T> service, SystemType systemType) where T : class
        {
            Uri uri = new Uri(service.Endpoint.Address.Uri.ToString());
            string hostAndPort = uri.Host;
            if (!uri.IsDefaultPort)
            {
                hostAndPort += ":" + uri.Port;
            }
            string url = uri.ToString();
            if (systemType == SystemType.CprBroker)
            {
                url = url.Replace(hostAndPort, "localhost:1551");
            }
            else
            {
                url = url.Replace(hostAndPort, "localhost:1552");
            }
            service.Endpoint.Address = new EndpointAddress(url);
        }
    }
}
