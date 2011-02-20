﻿using System;
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

                string pingResult = cprAdministrationAdapter.Ping();

                // If we have a successful connection the server will send some information
                // back stating who it thinks we are, etc. If we cannot connect we will get
                // an exception. Just to be sure - we expect the pingResult to contain at least something
                return (pingResult.Length > 0);
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
                return cprAdministrationAdapter.GetCapabillities(GetHeader());

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
