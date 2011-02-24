using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using CprBroker.Schemas;

namespace CprBroker.NUnitTester
{
    /// <summary>
    /// Contains members that manage the overall running of unit tests
    /// </summary>
    public static class TestRunner
    {

        public static Admin.AdminClient AdminService;
        public static Part.PartClient PartService;
        public static Subscriptions.Subscriptions SubscriptionsService;
        public static Events.EventsClient EventsService;
        public static Part.ApplicationHeader PartApplicationHeader;
        public static Admin.ApplicationHeader AdminApplicationHeader;
        public static Events.ApplicationHeader EventsApplicationHeader;

        public static void Initialize()
        {
            PartApplicationHeader = new Part.ApplicationHeader() { ApplicationToken = TestData.BaseAppToken, UserToken = TestData.userToken };
            AdminApplicationHeader = new NUnitTester.Admin.ApplicationHeader() { ApplicationToken = TestData.BaseAppToken, UserToken = TestData.userToken };
            EventsApplicationHeader = new CprBroker.NUnitTester.Events.ApplicationHeader() { ApplicationToken = TestData.BaseAppToken, UserToken = TestData.userToken };
            
            AdminService = new NUnitTester.Admin.AdminClient("WSHttpBinding_IAdmin");
            ReplaceServiceUrl(AdminService, SystemType.CprBroker);
            Console.WriteLine(AdminService.Endpoint.Address.Uri);
            
            PartService = new CprBroker.NUnitTester.Part.PartClient("WSHttpBinding_IPart");
            ReplaceServiceUrl(PartService, SystemType.CprBroker);
            Console.WriteLine(PartService.Endpoint.Address.Uri);

            SubscriptionsService = new NUnitTester.Subscriptions.Subscriptions();
            SubscriptionsService.ApplicationHeaderValue = new NUnitTester.Subscriptions.ApplicationHeader()
            {
                ApplicationToken = TestData.BaseAppToken,
                UserToken = TestData.userToken
            };
            ReplaceServiceUrl(SubscriptionsService, SystemType.EventBroker);
            Console.WriteLine(SubscriptionsService.Url);

            EventsService = new CprBroker.NUnitTester.Events.EventsClient();            
            ReplaceServiceUrl(EventsService, SystemType.CprBroker);
            Console.WriteLine(EventsService.Endpoint.Address.Uri);

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
