using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NUnitTester
{
    /// <summary>
    /// Contains members that manage the overall running of unit tests
    /// </summary>
    public static class TestRunner
    {

        public static CPRAdministrationWS.CPRAdministrationWS AdminService;
        public static CPRPersonWS.CPRPersonWS PersonService;
        public static Access.Access AccessService;
        public static Part.Part PartService;

        public static void Initialize()
        {
            AdminService = new NUnitTester.CPRAdministrationWS.CPRAdministrationWS();
            AdminService.ApplicationHeaderValue = new NUnitTester.CPRAdministrationWS.ApplicationHeader()
            {
                ApplicationToken = TestData.BaseAppToken,
                UserToken = TestData.userToken
            };
            ReplaceServiceUrl(AdminService);
            Console.WriteLine(AdminService.Url);


            PersonService = new NUnitTester.CPRPersonWS.CPRPersonWS();
            PersonService.ApplicationHeaderValue = new NUnitTester.CPRPersonWS.ApplicationHeader()
            {
                ApplicationToken = TestData.BaseAppToken,
                UserToken = TestData.userToken
            };
            ReplaceServiceUrl(PersonService);
            Console.WriteLine(PersonService.Url);

            
            PartService = new NUnitTester.Part.Part();
            PartService.ApplicationHeaderValue = new NUnitTester.Part.ApplicationHeader()
            {
                ApplicationToken = TestData.BaseAppToken,
                UserToken = TestData.userToken
            };
            PartService.EffectDateHeaderValue = new NUnitTester.Part.EffectDateHeader() { EffectDate = null };
            PartService.RegistrationDateHeaderValue = new NUnitTester.Part.RegistrationDateHeader() { RegistrationDate = null};
            ReplaceServiceUrl(PartService);
            Console.WriteLine(PartService.Url);

            
            AccessService = new NUnitTester.Access.Access();
            ReplaceServiceUrl(AccessService);
            Console.WriteLine(AccessService.Url);
            
            
        }

        private static void ReplaceServiceUrl(System.Web.Services.Protocols.SoapHttpClientProtocol service)
        {
            Uri uri = new Uri(service.Url);
            string hostAndPort = uri.Host;
            if (!uri.IsDefaultPort)
            {
                hostAndPort += ":" + uri.Port;
            }
            service.Url = service.Url.Replace(hostAndPort, "localhost:1551");
        }
    }
}
