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

namespace CprBroker.Schemas.Part
{
    /// <summary>
    /// Contains service and message names
    /// </summary>
    public partial class ServiceNames
    {
        public const string Namespace = "http://dk.itst";

        public static class Admin
        {
            public const string Service = "Admin";

            public static class MethodNames
            {
                public const string RequestAppRegistration = "RequestAppRegistration";
                public const string ApproveAppRegistration = "ApproveAppRegistration";
                public const string ListAppRegistrations = "ListAppRegistrations";
                public const string UnregisterApp = "UnregisterApp";
                public const string GetCapabilities = "GetCapabilities";
                public const string IsImplementing = "IsImplementing";
                public const string GetDataProviderList = "GetDataProviderList";
                public const string SetDataProviderList = "SetDataProviderList";
                public const string Log = "Log";
            }
        }

        public static class Notification
        {
            public const string Service = "Notification";

            public static class MethodNames
            {
                public const string Notify = "Notify";
                public const string Ping = "Ping";
            }
        }

        public class Part
        {
            public const string Service = "Part";

            public static class Methods
            {
                public const string Read = "Read";
                public const string RefreshRead = "RefreshRead";
                public const string List = "List";
                public const string Search = "Search";

                public const string GetUuid = "GetUuid";
                public const string GetUuidArray = "GetUuidArray";
            }
        }

        public class Subscriptions
        {
            public const string Service = "Subscriptions";

            public static class Methods
            {
                public const string Subscribe = "Subscribe";
                public const string Unsubscribe = "Unsubscribe";
                public const string SubscribeOnBirthdate = "SubscribeOnBirthdate";

                public const string GetActiveSubscriptionsList = "GetActiveSubsciptionsList";
                public const string GetLatestNotification = "GetLatestNotification";

                public const string RemoveBirthDateSubscription = "RemoveBirthDateSubscription";

                public const string Ping = "Ping";

            }
        }

        public class NotificationQueue
        {
            public const string Service = "NotificationQueue";

            public static class Methods
            {
                public const string Enqueue = "Enqueue";

            }
        }

    }
}
