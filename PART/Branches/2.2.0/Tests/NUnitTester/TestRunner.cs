/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

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
        public static Subscriptions.Subscriptions SubscriptionsService;
        public static Events.Events EventsService;
        public static Notification.Notification NotificationService;

        public static Part.ApplicationHeader PartApplicationHeader;
        public static Admin.ApplicationHeader AdminApplicationHeader;
        public static Events.ApplicationHeader EventsApplicationHeader;
        public static Subscriptions.ApplicationHeader SubscriptionsApplicationHeader;
        

        public static void Initialize()
        {

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

            // Subscriptions
            SubscriptionsService = new NUnitTester.Subscriptions.Subscriptions();
            ReplaceServiceUrl(SubscriptionsService, SystemType.EventBroker);
            Console.WriteLine(SubscriptionsService.Url);

            SubscriptionsApplicationHeader = new CprBroker.NUnitTester.Subscriptions.ApplicationHeader() { ApplicationToken = TestData.BaseAppToken, UserToken = TestData.userToken };
            SubscriptionsService.ApplicationHeaderValue = SubscriptionsApplicationHeader;

            // Events
            EventsService = new CprBroker.NUnitTester.Events.Events();
            ReplaceServiceUrl(EventsService, SystemType.CprBroker);
            Console.WriteLine(EventsService.Url);

            EventsApplicationHeader = new CprBroker.NUnitTester.Events.ApplicationHeader() { ApplicationToken = TestData.BaseAppToken, UserToken = TestData.userToken };
            EventsService.ApplicationHeaderValue = EventsApplicationHeader;

            // Notification
            NotificationService = new CprBroker.NUnitTester.Notification.Notification();
            ReplaceServiceUrl(NotificationService, SystemType.EventBroker);
            Console.WriteLine(NotificationService.Url);

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
