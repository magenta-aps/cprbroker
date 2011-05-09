﻿/*
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
using System.Configuration;
using CPR_Business_Application_Demo.Adapters;
using CPR_Business_Application_Demo.SubscriptionsService;

namespace CPR_Business_Application_Demo.Business
{
    public class SubscriptionsController
    {
        #region Construction
        public SubscriptionsController(ApplicationSettingsBase settings)
        {
            this.Settings = settings;

            UserToken = settings["UserToken"].ToString();
            AppToken = settings["AppToken"].ToString();
            AppName = settings["ApplicationName"].ToString();
            CprSubscriptionsWebServiceUrl = settings["EventBrokerWebServiceUrl"].ToString();
            Console.Write("new cpradmController uT=" + UserToken + " , appT=" + AppToken + ", nm=" + AppName + " url=" + CprSubscriptionsWebServiceUrl + "\n");
        }
        #endregion

        #region Methods

        public bool TestWSConnection(string url)
        {
            try
            {
                var cprSubscriptionAdapter = new SubscriptionAdapter(url);
                return cprSubscriptionAdapter.Test();
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public SubscriptionType[] GetActiveSubscriptions()
        {
            try
            {
                var cprAdministrationAdapter = new SubscriptionAdapter(CprSubscriptionsWebServiceUrl);
                return cprAdministrationAdapter.GetActiveSubscriptions(GetHeader()).Item;

            }
            catch (Exception ex)
            {
                return new SubscriptionType[] { };
            }
        }

        public string Subscribe(string[] personCivilRegistrationIdentifiers)
        {
            try
            {
                var cprAdministrationAdapter = new SubscriptionAdapter(CprSubscriptionsWebServiceUrl);

                var notificationMode = (int)Settings["NotificationMode"];

                switch (notificationMode)
                {
                    case 0:
                        return string.Empty;
                    case 1:
                        throw new NotImplementedException();
                    case 2:
                        var fileShareChannel = new FileShareChannelType();
                        fileShareChannel.Path = Settings["NotificationFileShare"].ToString();
                        var result = cprAdministrationAdapter.Subscribe(GetHeader(), fileShareChannel, personCivilRegistrationIdentifiers);
                        return result.Item.SubscriptionId;
                }

                return string.Empty;

            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public string SubscribeBirthdate(int? age, int priorDays, string[] personCivilRegistrationIdentifiers)
        {
            try
            {
                var cprAdministrationAdapter = new SubscriptionAdapter(CprSubscriptionsWebServiceUrl);
                var notificationMode = (int)Settings["NotificationMode"];

                switch (notificationMode)
                {
                    case 0:
                        return string.Empty;
                    case 1:
                        throw new NotImplementedException();
                    case 2:
                        var fileShareChannel = new FileShareChannelType();
                        fileShareChannel.Path = Settings["NotificationFileShare"].ToString();
                        var result = cprAdministrationAdapter.SubscribeOnBirthdate(GetHeader(), fileShareChannel, age, priorDays, personCivilRegistrationIdentifiers);
                        return result.Item.SubscriptionId;
                }
                return string.Empty;

            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        #endregion

        #region Private Methods

        ApplicationHeader GetHeader()
        {
            return new ApplicationHeader()
                {
                    UserToken = UserToken,
                    ApplicationToken = AppToken
                };
        }
        #endregion

        #region Private Fields
        private readonly ApplicationSettingsBase Settings;

        private readonly string UserToken;
        private readonly string AppToken;
        private readonly string AppName;
        private readonly string CprSubscriptionsWebServiceUrl;
        #endregion


    }
}
