using System;
using System.Configuration;
using CPR_Business_Application_Demo.Adapters;
using CPR_Business_Application_Demo.Adapters.CPRAdministrationWS;

namespace CPR_Business_Application_Demo.Business
{
    public class CPRAdministrationController
    {
        #region Construction
        public CPRAdministrationController(ApplicationSettingsBase settings)
        {
            this.Settings = settings;

            UserToken = settings["UserToken"].ToString();
            AppToken = settings["AppToken"].ToString();
            AppName = settings["ApplicationName"].ToString();
            CprAdminWebServiceUrl = settings["CPRBrokerWebServiceUrl"].ToString();
        }
        #endregion

        #region Methods
        public bool TestWSConnection(string url)
        {
            try
            {
                var cprAdministrationAdapter = new CPRAdministrationAdapter(url);

                string pingResult = cprAdministrationAdapter.Ping();

                // If we have a successful connection the server will send some information
                // back stating who it thinks we are, etc. If we cannot connect we will get
                // an exception. Just to be sure - we expect the pingResult to contain at least something
                return (pingResult.Length > 0);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string RegisterApplication(string url)
        {
            try
            {
                var cprAdministrationAdapter = new CPRAdministrationAdapter(url);

                ApplicationType applicationType = cprAdministrationAdapter.RequestAppRegistration(GetHeader(), AppName);

                if (applicationType != null)
                    return applicationType.Token;

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
                var cprAdministrationAdapter = new CPRAdministrationAdapter(url);

                // Here we change the application to the built-in admin token in CPR Broker, to
                // fool CPR Broker into believing that we are actually allowed to approve
                // an application
                var header = GetHeader();
                header.ApplicationToken = adminAppToken;
                return cprAdministrationAdapter.ApproveAppRegistration(header, AppToken);
            }
            catch (Exception)
            {

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
                var cprAdministrationAdapter = new CPRAdministrationAdapter(url);

                ApplicationType[] applicationTypes = cprAdministrationAdapter.ListAppRegistration(GetHeader());
                if (applicationTypes == null || applicationTypes.Length == 0)
                    return null;

                // Loop through all registered application to see if one of them is ourself
                foreach (var applicationType in applicationTypes)
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

        public SubscriptionType[] GetActiveSubscriptions()
        {
            try
            {
                var cprAdministrationAdapter = new CPRAdministrationAdapter(Settings["CPRBrokerWebServiceUrl"].ToString());
                return cprAdministrationAdapter.GetActiveSubscriptions(GetHeader());

            }
            catch (Exception)
            {
                
                return new SubscriptionType[]{};
            }
        }

        public string Subscribe(string[] personCivilRegistrationIdentifiers)
        {
            try
            {
                var cprAdministrationAdapter = new CPRAdministrationAdapter(CprAdminWebServiceUrl);

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
                        return result.SubscriptionId;
                }

                return string.Empty;

            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string SubscribeBirthdate(int? age, int priorDays, string[] personCivilRegistrationIdentifiers)
        {
            try
            {
                var cprAdministrationAdapter = new CPRAdministrationAdapter(CprAdminWebServiceUrl);
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
                        return result.SubscriptionId;
                }
                return string.Empty;

            }
            catch (Exception)
            {
                return String.Empty;
            }
        }


        public bool CreateTestCitizen(PersonFullStructureType person)
        {
            try
            {
                var cprAdministrationAdapter = new CPRAdministrationAdapter(Settings["CPRBrokerWebServiceUrl"].ToString());
                return cprAdministrationAdapter.CreateTestCitizen(GetHeader(), person);

            }
            catch (Exception)
            {
                return false;
            }
        }

        public ServiceVersionType[] GetCapabillities()
        {
            try
            {
                var cprAdministrationAdapter = new CPRAdministrationAdapter(CprAdminWebServiceUrl);
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
