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
