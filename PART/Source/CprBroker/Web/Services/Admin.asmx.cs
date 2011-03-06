using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Engine;

namespace CprBroker.Web.Services
{
    /// <summary>
    /// Contains web methods related to administration of the system
    /// </summary>
    [WebService(Namespace = CprBroker.Schemas.Part.ServiceNames.Namespace, Name = ServiceNames.Admin.Service, Description = CprBroker.Schemas.ServiceDescription.Admin.Service)]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class Admin : GKApp.WS.wsBaseV2
    {
        public ApplicationHeader applicationHeader;
        private const string ApplicationHeaderName = "applicationHeader";

        public Admin()
        {
            BaseInit();
        }

        #region Application manager
        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(
            MessageName = ServiceNames.Admin.MethodNames.RequestAppRegistration, Description = CprBroker.Schemas.ServiceDescription.Admin.RequestAppRegistration)]
        public BasicOutputType<ApplicationType> RequestAppRegistration(string ApplicationName)
        {
            return Manager.Admin.RequestAppRegistration(applicationHeader.UserToken, applicationHeader.ApplicationToken, ApplicationName);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Admin.MethodNames.ApproveAppRegistration, Description = CprBroker.Schemas.ServiceDescription.Admin.ApproveAppRegistration)]
        public BasicOutputType<bool> ApproveAppRegistration(string ApplicationToken)
        {
            return Manager.Admin.ApproveAppRegistration(applicationHeader.UserToken, applicationHeader.ApplicationToken, ApplicationToken);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Admin.MethodNames.ListAppRegistrations, Description = CprBroker.Schemas.ServiceDescription.Admin.ListAppRegistrations)]
        public BasicOutputType<ApplicationType[]> ListAppRegistrations()
        {
            return Manager.Admin.ListAppRegistrations(applicationHeader.UserToken, applicationHeader.ApplicationToken);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Admin.MethodNames.UnregisterApp, Description = CprBroker.Schemas.ServiceDescription.Admin.UnregisterApp)]
        public BasicOutputType<bool> UnregisterApp(string ApplicationToken)
        {
            return Manager.Admin.UnregisterApp(applicationHeader.UserToken, applicationHeader.ApplicationToken, ApplicationToken);
        }
        #endregion

        #region Versioning
        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(
            MessageName = ServiceNames.Admin.MethodNames.GetCapabilities,
            Description = CprBroker.Schemas.ServiceDescription.Admin.GetCapabilities)]
        public BasicOutputType<ServiceVersionType[]> GetCapabilities()
        {
            return Manager.Admin.GetCapabilities(applicationHeader.UserToken, applicationHeader.ApplicationToken);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Admin.MethodNames.IsImplementing, Description = CprBroker.Schemas.ServiceDescription.Admin.IsImplementing)]
        public BasicOutputType<bool> IsImplementing(string serviceName, string serviceVersion)
        {
            return Manager.Admin.IsImplementing(applicationHeader.UserToken, applicationHeader.ApplicationToken, serviceName, serviceVersion);
        }
        #endregion

        #region Provier list
        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Admin.MethodNames.GetDataProviderList, Description = CprBroker.Schemas.ServiceDescription.Admin.GetDataProviderList)]
        public BasicOutputType<DataProviderType[]> GetDataProviderList()
        {
            return Manager.Admin.GetDataProviderList(applicationHeader.UserToken, applicationHeader.ApplicationToken);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Admin.MethodNames.SetDataProviderList, Description = CprBroker.Schemas.ServiceDescription.Admin.SetDataProviderList)]
        public BasicOutputType<bool> SetDataProviderList(DataProviderType[] DataProviders)
        {
            return Manager.Admin.SetDataProviderList(applicationHeader.UserToken, applicationHeader.ApplicationToken, DataProviders);
        }
        #endregion

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Admin.MethodNames.Log, Description = CprBroker.Schemas.ServiceDescription.Admin.Log)]
        public BasicOutputType<bool> Log(string Text)
        {
            return Manager.Admin.Log(applicationHeader.UserToken, applicationHeader.ApplicationToken, Text);
        }

    }
}
