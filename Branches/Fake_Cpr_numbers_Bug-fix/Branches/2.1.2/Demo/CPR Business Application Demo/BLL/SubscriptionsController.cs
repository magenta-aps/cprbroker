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
