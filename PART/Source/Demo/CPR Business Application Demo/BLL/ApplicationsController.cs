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
using CPR_Business_Application_Demo.ApplicationsService;

namespace CPR_Business_Application_Demo.Business
{
    public class ApplicationsController
    {
        #region Construction
        public      ApplicationsController(ApplicationSettingsBase settings)
        {
            this.Settings = settings;

            UserToken = settings["UserToken"].ToString();
            AppToken = settings["AppToken"].ToString();
            AppName = settings["ApplicationName"].ToString();
            CprAdminWebServiceUrl = settings["CPRBrokerWebServiceUrl"].ToString();
            Console.Write("new cpradmController uT=" + UserToken + " , appT=" + AppToken + ", nm=" + AppName + " url=" + CprAdminWebServiceUrl + "\n");
        }
        #endregion

        #region Methods
        public bool TestWSConnection(string url)
        {
            try
            {
                var cprAdministrationAdapter = new ApplicationAdapter(url);

                var pingResult = cprAdministrationAdapter.IsImplementing(GetHeader(),"Read","1.0");

                // If we have a successful connection the server will send some information
                // back stating who it thinks we are, etc. If we cannot connect we will get
                // an exception. Just to be sure - we expect the pingResult to contain at least something
                return pingResult.Item;
            }
            catch (Exception ex)
            {
                Console.WriteLine ( "ping exception for URL:"+url + ":: " + ex ) ;
                return false;
            }
        }

        public string RegisterApplication(string url)
        {
            try
            {
                var cprAdministrationAdapter = new ApplicationAdapter(url);

                var result= cprAdministrationAdapter.RequestAppRegistration(GetHeader(), AppName);

                if (result != null && result.Item!=null)
                    return result.Item.Token;

                return String.Empty;

            }
            catch (Exception ex)
            {

                return "";
            }
        }

        public bool ApproveApplication(string url, string adminAppToken)
        {
            try
            {
                var cprAdministrationAdapter = new ApplicationAdapter(url);

                // Here we change the application to the built-in admin token in CPR Broker, to
                // fool CPR Broker into believing that we are actually allowed to approve
                // an application
                var header = GetHeader();
                header.ApplicationToken = adminAppToken;
                return cprAdministrationAdapter.ApproveAppRegistration(header, AppToken).Item;
            }
            catch (Exception ex)
            {
                Console.WriteLine ( "adm approve exception:" + ex ) ;
                return false;
            }
        }

        /// <summary>
        /// Checks whether the application is registered
        /// </summary>
        /// <returns>null if the application is not registered, otherwise the
        /// registered application is returned
        /// </returns>
        public ApplicationType ApplicationIsRegistered(string url)
        {
            try
            {
                var cprAdministrationAdapter = new ApplicationAdapter(url);

                var applicationTypes = cprAdministrationAdapter.ListAppRegistration(GetHeader());
                if (applicationTypes == null || applicationTypes.Item==null || applicationTypes.Item.Length == 0)
                    return null;

                // Loop through all registered application to see if one of them is ourself
                foreach (var applicationType in applicationTypes.Item)
                {
                    if (applicationType.Token == AppToken)
                    {
                        return applicationType;
                    }
                }
                return null;

            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Checks whether the application is registered
        /// </summary>
        /// <returns>null if the application is not registered</returns>
        public ApplicationType ApplicationIsRegistered()
        {
            try
            {
                return ApplicationIsRegistered(CprAdminWebServiceUrl);

            }
            catch (Exception)
            {
                return null;
            }
        }

        public ServiceVersionType[] GetCapabillities()
        {
            try
            {
                var cprAdministrationAdapter = new ApplicationAdapter(CprAdminWebServiceUrl);
                return cprAdministrationAdapter.GetCapabillities(GetHeader()).Item;

            }
            catch (Exception)
            {
                return new ServiceVersionType[] {};
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
        private readonly string CprAdminWebServiceUrl;
        #endregion


    }
}
