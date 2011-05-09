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
                if (!applicationsWsUrl.EndsWith("Admin.asmx"))
                    applicationsWsUrl += "/Admin.asmx";
            }
            else
            {
                applicationsWsUrl += "Admin.asmx";
            }

            applicationsHandler = new AdminSoap12Client("AdminSoap12", applicationsWsUrl);

            // Set the timeout to avoid hanging the application for too long when wrong urls were entered
            applicationsHandler.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 15);

        }
        #endregion

        #region Application Registration
        public BasicOutputTypeOfApplicationType RequestAppRegistration(ApplicationHeader applicationHeader, string applicationName)
        {
            return applicationsHandler.RequestAppRegistration(applicationName);
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
        private readonly AdminSoap12Client applicationsHandler;
        #endregion

    }
}
