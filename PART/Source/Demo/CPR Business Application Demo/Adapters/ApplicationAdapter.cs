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
                if (!applicationsWsUrl.EndsWith("Admin.svc"))
                    applicationsWsUrl += "/Admin.svc";
            }
            else
            {
                applicationsWsUrl += "Admin.svc";
            }

            applicationsHandler = new AdminClient("WSHttpBinding_IAdmin", applicationsWsUrl);

            // Set the timeout to avoid hanging the application for too long when wrong urls were entered
            applicationsHandler.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 15);

        }
        #endregion

        #region Application Registration
        public BasicOutputTypeOfApplicationType RequestAppRegistration(ApplicationHeader applicationHeader, string applicationName)
        {
            return applicationsHandler.RequestAppRegistration(applicationHeader, applicationName);
        }

        public BasicOutputTypeOfBoolean ApproveAppRegistration(ApplicationHeader applicationHeader, string applicationToken)
        {
            return applicationsHandler.ApproveAppRegistration(applicationHeader, applicationToken);
        }

        public BasicOutputTypeOfArrayOfApplicationType ListAppRegistration(ApplicationHeader applicationHeader)
        {
            return applicationsHandler.ListAppRegistrations(applicationHeader);
        }
        #endregion

        #region Misc
        
        public BasicOutputTypeOfArrayOfServiceVersionType GetCapabillities(ApplicationHeader applicationHeader)
        {
            return applicationsHandler.GetCapabilities(applicationHeader);
        }

        public BasicOutputTypeOfBoolean IsImplementing(ApplicationHeader applicationHeader, string method,string version)
        {
            return applicationsHandler.IsImplementing(applicationHeader, method, version);
        }

     
        #endregion

        #region Private Fields
        private readonly AdminClient applicationsHandler;
        #endregion

    }
}
