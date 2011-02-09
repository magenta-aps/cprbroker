using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPR_Business_Application_Demo.ApplicationsService;

namespace CPR_Business_Application_Demo.Adapters
{
    // The adapter is a feeble attempt at disconnecting the web service and the application
    // Not a very successful one I admit
    public class ApplicationAdapter
    {
        #region Construction
        public ApplicationAdapter(string applicationsWsUrl)
        {
            // Make sure the provided URL is pointing to the applications web service
            if (!applicationsWsUrl.EndsWith("/"))
	      {
                if (!applicationsWsUrl.EndsWith("CPRAdministrationWS.asmx"))
                    applicationsWsUrl += "/CPRAdministrationWS.asmx";
            }
            else
            {
                applicationsWsUrl += "CPRAdministrationWS.asmx";
            }

            applicationsHandler = new AdminSoap12Client("CPRAdministrationWSSoap", applicationsWsUrl);

            // Set the timeout to avoid hanging the application for too long when wrong urls were entered
            applicationsHandler.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 5);

        }
        #endregion

        #region Application Registration
        public ApplicationType RequestAppRegistration(ApplicationHeader applicationHeader, string applicationName)
        {
            return applicationsHandler.RequestAppRegistration(applicationHeader, applicationName);
        }

        public bool ApproveAppRegistration(ApplicationHeader applicationHeader, string applicationToken)
        {
            return applicationsHandler.ApproveAppRegistration(applicationHeader, applicationToken);
        }

        public ApplicationType[] ListAppRegistration(ApplicationHeader applicationHeader)
        {
            return applicationsHandler.ListAppRegistrations(applicationHeader);
        }
        #endregion

        #region Misc
        public bool CreateTestCitizen(ApplicationHeader applicationheader, PersonFullStructureType person)
        {
            return applicationsHandler.CreateTestCitizen(applicationheader, person);
        }

        public ServiceVersionType[] GetCapabillities(ApplicationHeader applicationHeader)
        {
            return applicationsHandler.GetCapabilities(applicationHeader);
        }

        public string Ping()
        {
            try
            {
                var pingResult = applicationsHandler.Ping();
                return pingResult;
            }
            catch (Exception eException)
            {
                return null;
            }
        }
        #endregion

        #region Private Fields
        private readonly AdminSoap12Client applicationsHandler;
        #endregion

    }
}
