using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Engine;

namespace CprBroker.Web.Services
{
    /// <summary>
    /// Contains web methods related to administration of the system
    /// </summary>    
    public class Admin : IAdmin
    {
        
        #region Application manager        
        public BasicOutputType<ApplicationType> RequestAppRegistration(ApplicationHeader applicationHeader, string ApplicationName)
        {
            return Manager.Admin.RequestAppRegistration(applicationHeader.UserToken, applicationHeader.ApplicationToken, ApplicationName);
        }

        public BasicOutputType<bool> ApproveAppRegistration(ApplicationHeader applicationHeader, string ApplicationToken)
        {
            return Manager.Admin.ApproveAppRegistration(applicationHeader.UserToken, applicationHeader.ApplicationToken, ApplicationToken);
        }

        public BasicOutputType<ApplicationType[]> ListAppRegistrations(ApplicationHeader applicationHeader)
        {
            return Manager.Admin.ListAppRegistrations(applicationHeader.UserToken, applicationHeader.ApplicationToken);
        }

        public BasicOutputType<bool> UnregisterApp(ApplicationHeader applicationHeader, string ApplicationToken)
        {
            return Manager.Admin.UnregisterApp(applicationHeader.UserToken, applicationHeader.ApplicationToken, ApplicationToken);
        }
        #endregion

        #region Versioning
        public BasicOutputType<ServiceVersionType[]> GetCapabilities(ApplicationHeader applicationHeader)
        {
            return Manager.Admin.GetCapabilities(applicationHeader.UserToken, applicationHeader.ApplicationToken);
        }

        public BasicOutputType<bool> IsImplementing(ApplicationHeader applicationHeader, string serviceName, string serviceVersion)
        {
            return Manager.Admin.IsImplementing(applicationHeader.UserToken, applicationHeader.ApplicationToken, serviceName, serviceVersion);
        }
        #endregion

        #region Provier list
        public BasicOutputType<DataProviderType[]> GetDataProviderList(ApplicationHeader applicationHeader)
        {
            return Manager.Admin.GetDataProviderList(applicationHeader.UserToken, applicationHeader.ApplicationToken);
        }

        public BasicOutputType<bool> SetDataProviderList(ApplicationHeader applicationHeader, DataProviderType[] DataProviders)
        {
            return Manager.Admin.SetDataProviderList(applicationHeader.UserToken, applicationHeader.ApplicationToken, DataProviders);
        }
        #endregion

        public BasicOutputType<bool> Log(ApplicationHeader applicationHeader, string Text)
        {
            return Manager.Admin.Log(applicationHeader.UserToken, applicationHeader.ApplicationToken, Text);
        }

    }
}
