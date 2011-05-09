/*
 * Copyright (c) 2011 Danish National IT and Tele agency / IT- og Telestyrelsen

* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:

* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.

* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

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
