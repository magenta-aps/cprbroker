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
using CPR_Business_Application_Demo.SubscriptionsService;

namespace CPR_Business_Application_Demo.Adapters
{
    // The adapter is a feeble attempt at disconnecting the web service and the application
    // Not a very successful one I admit
    public class SubscriptionAdapter
    {
        #region Construction
        public SubscriptionAdapter(string subscriptionsWsUrl)
        {
            // Make sure the provided URL is pointing to the administration web service
            if (!subscriptionsWsUrl.EndsWith("/"))
            {
                if (!subscriptionsWsUrl.EndsWith("Subscriptions.asmx"))
                    subscriptionsWsUrl += "/Subscriptions.asmx";
            }
            else
            {
                subscriptionsWsUrl += "Subscriptions.asmx";
            }

            subscriptionsHandler = new SubscriptionsSoap12Client("SubscriptionsSoap12", subscriptionsWsUrl);

            // Set the timeout to avoid hanging the application for too long when wrong urls were entered
            subscriptionsHandler.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 5);

        }
        #endregion

        #region Subscription
        private Guid[] GetUuids(ApplicationHeader applicationHeader, string[] cprNumbers)
        {
            PartAdapter adpt = new PartAdapter(Properties.Settings.Default.CPRBrokerWebServiceUrl);
            return Array.ConvertAll<string, Guid>(cprNumbers, cpr => adpt.GetUuid(applicationHeader.ApplicationToken, cpr));
        }

        public BasicOutputTypeOfChangeSubscriptionType Subscribe(ApplicationHeader applicationHeader, ChannelBaseType notificationChannel, string[] personCivilRegistrationIdentifiers)
        {
            return subscriptionsHandler.Subscribe(applicationHeader, notificationChannel, GetUuids(applicationHeader, personCivilRegistrationIdentifiers));
        }

        public BasicOutputTypeOfBirthdateSubscriptionType SubscribeOnBirthdate(ApplicationHeader applicationHeader, ChannelBaseType notificationChannel,
                                                              int? age, int priorDays,
                                                              string[] personCivilRegistrationIdentifiers)
        {
            return subscriptionsHandler.SubscribeOnBirthdate(applicationHeader, notificationChannel, age, priorDays, GetUuids(applicationHeader, personCivilRegistrationIdentifiers));
        }

        public BasicOutputTypeOfArrayOfSubscriptionType GetActiveSubscriptions(ApplicationHeader applicationHeader)
        {
            return subscriptionsHandler.GetActiveSubscriptionsList(applicationHeader);
        }
        #endregion

        public bool Test()
        {
            System.Net.WebClient client = new System.Net.WebClient();
            client.DownloadData(subscriptionsHandler.Endpoint.Address.Uri.ToString());
            return true;
        }

        #region Private Fields
        private readonly SubscriptionsService.SubscriptionsSoap12Client subscriptionsHandler;
        #endregion

    }
}
