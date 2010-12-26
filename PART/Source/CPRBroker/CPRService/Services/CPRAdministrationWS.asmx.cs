using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using CPRBroker.Schemas;
using CPRBroker.Engine;

namespace CPRService.Services
{
    /// <summary>
    /// Contains web methods related to administration of the system
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/", Name = ServiceNames.Administrator.Service, Description = ServiceDescription.Administrator.Service)]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class CPRAdministrationWS : GKApp.WS.wsBaseV2
    {
        public ApplicationHeader applicationHeader;
        private const string ApplicationHeaderName = "applicationHeader";

        public CPRAdministrationWS()
        {
            BaseInit();
        }

        #region Application manager
        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Administrator.MethodNames.RequestAppRegistration, Description = ServiceDescription.Administrator.RequestAppRegistration)]
        public ApplicationType RequestAppRegistration(string ApplicationName)
        {
            return Manager.Admin.RequestAppRegistration(applicationHeader.UserToken, ApplicationName);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Administrator.MethodNames.ApproveAppRegistration, Description = ServiceDescription.Administrator.ApproveAppRegistration)]
        public bool ApproveAppRegistration(string ApplicationToken)
        {
            return Manager.Admin.ApproveAppRegistration(applicationHeader.UserToken, applicationHeader.ApplicationToken, ApplicationToken);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Administrator.MethodNames.ListAppRegistrations, Description = ServiceDescription.Administrator.ListAppRegistrations)]
        public ApplicationType[] ListAppRegistrations()
        {
            return Manager.Admin.ListAppRegistrations(applicationHeader.UserToken, applicationHeader.ApplicationToken);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Administrator.MethodNames.UnregisterApp, Description = ServiceDescription.Administrator.UnregisterApp)]
        public bool UnregisterApp(string ApplicationToken)
        {
            return Manager.Admin.UnregisterApp(applicationHeader.UserToken, applicationHeader.ApplicationToken, ApplicationToken);
        }
        #endregion

        #region Versioning
        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Administrator.MethodNames.GetCapabilities, Description = ServiceDescription.Administrator.GetCapabilities)]
        public ServiceVersionType[] GetCapabilities()
        {
            return Manager.Admin.GetCapabilities(applicationHeader.UserToken, applicationHeader.ApplicationToken);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Administrator.MethodNames.IsImplementing, Description = ServiceDescription.Administrator.IsImplementing)]
        public bool IsImplementing(string serviceName, string serviceVersion)
        {
            return Manager.Admin.IsImplementing(applicationHeader.UserToken, applicationHeader.ApplicationToken, serviceName, serviceVersion);
        }
        #endregion

        #region Provier list
        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Administrator.MethodNames.GetDataProviderList, Description = ServiceDescription.Administrator.GetDataProviderList)]
        public DataProviderType[] GetDataProviderList()
        {
            return Manager.Admin.GetDataProviderList(applicationHeader.UserToken, applicationHeader.ApplicationToken);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Administrator.MethodNames.SetDataProviderList, Description = ServiceDescription.Administrator.SetDataProviderList)]
        public bool SetDataProviderList(DataProviderType[] DataProviders)
        {
            return Manager.Admin.SetDataProviderList(applicationHeader.UserToken, applicationHeader.ApplicationToken, DataProviders);
        }
        #endregion
        
        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Administrator.MethodNames.Log, Description = ServiceDescription.Administrator.Log)]
        public bool Log(string Text)
        {
            return Manager.Admin.Log(applicationHeader.UserToken, applicationHeader.ApplicationToken, Text);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Administrator.MethodNames.CreateTestCitizen, Description = ServiceDescription.Administrator.CreateTestCitizen)]
        public bool CreateTestCitizen(PersonFullStructureType OioPerson)
        {
            return Manager.Admin.CreateTestCitizen(applicationHeader.UserToken, applicationHeader.ApplicationToken, OioPerson);
        }
    }
}
