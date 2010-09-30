using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPR_Business_Application_Demo.Adapters.CPRAdministrationWS;

namespace CPR_Business_Application_Demo.Adapters
{
    // The adapter is a feeble attempt at disconnecting the web service and the application
    // Not a very successful one I admit
    public class CPRAdministrationAdapter
    {
        #region Construction
        public CPRAdministrationAdapter(string cprAdministrationWsUrl)
        {
            // Make sure the provided URL is pointing to the administration web service
            if (!cprAdministrationWsUrl.EndsWith("/"))
            {
                if (!cprAdministrationWsUrl.EndsWith("CPRAdministrationWS.asmx"))
                    cprAdministrationWsUrl += "/CPRAdministrationWS.asmx";
            }
            else
            {
                cprAdministrationWsUrl += "CPRAdministrationWS.asmx";
            }

            administrationHandler = new CPRAdministrationWSSoapClient("CPRAdministrationWSSoap", cprAdministrationWsUrl);

            // Set the timeout to avoid hanging the application for too long when wrong urls were entered
            administrationHandler.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 5);

        }
        #endregion

        #region Application Registration
        public ApplicationType RequestAppRegistration(ApplicationHeader applicationHeader, string applicationName)
        {
            return administrationHandler.RequestAppRegistration(applicationHeader, applicationName);
        }

        public bool ApproveAppRegistration(ApplicationHeader applicationHeader, string applicationToken)
        {
            return administrationHandler.ApproveAppRegistration(applicationHeader, applicationToken);
        }

        public ApplicationType[] ListAppRegistration(ApplicationHeader applicationHeader)
        {
            return administrationHandler.ListAppRegistrations(applicationHeader);
        }
        #endregion

        #region Subscription
        public ChangeSubscriptionType Subscribe(ApplicationHeader applicationHeader, ChannelBaseType notificationChannel, string[] personCivilRegistrationIdentifiers)
        {
            return administrationHandler.Subscribe(applicationHeader, notificationChannel, personCivilRegistrationIdentifiers);
        }
        
        public BirthdateSubscriptionType SubscribeOnBirthdate(ApplicationHeader applicationHeader, ChannelBaseType notificationChannel, 
                                                              int? age, int priorDays,
                                                              string[] personCivilRegistrationIdentifiers)
        {
            return administrationHandler.SubscribeOnBirthdate(applicationHeader, notificationChannel, age, priorDays, personCivilRegistrationIdentifiers);
        }

        public SubscriptionType[] GetActiveSubscriptions(ApplicationHeader applicationHeader)
        {
            return administrationHandler.GetActiveSubscriptionsList(applicationHeader);
        }
        #endregion

        #region Misc
        public bool CreateTestCitizen(ApplicationHeader applicationheader, PersonFullStructureType person)
        {
            return administrationHandler.CreateTestCitizen(applicationheader, person);
        }

        public ServiceVersionType[] GetCapabillities(ApplicationHeader applicationHeader)
        {
            return administrationHandler.GetCapabilities(applicationHeader);
        }

        public string Ping()
        {
            try
            {
                var pingResult = administrationHandler.Ping();
                return pingResult;
            }
            catch (Exception eException)
            {
                return null;
            }
        }
        #endregion

        #region Private Fields
        private readonly CPRAdministrationWSSoapClient administrationHandler;
        #endregion

    }
}
