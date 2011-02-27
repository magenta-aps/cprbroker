using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine
{
    public static partial class Manager
    {
        /// <summary>
        /// This part contains methods related to admin interface
        /// All methods here simply delegate the code to Manager.CallMethod&lt;&gt;()
        /// </summary>
        public class Admin
        {
            #region Versioning management
            public static BasicOutputType<ServiceVersionType[]> GetCapabilities(string userToken, string appToken)
            {
                var facade = new GetCapabilitiesFacadeMethod(appToken, userToken);
                return GetMethodOutput<ServiceVersionType[]>(facade);
            }
            public static BasicOutputType<bool> IsImplementing(string userToken, string appToken, string methodName, string version)
            {
                var facade = new IsImplementingFacadeMethod(appToken, userToken, methodName, version);
                return GetMethodOutput<bool>(facade);
            }
            #endregion

            #region Provider list
            public static BasicOutputType<Schemas.DataProviderType[]> GetDataProviderList(string userToken, string appToken)
            {
                var facade = new DataProviders.GetDataProviderListFacadeMethodInfo(appToken, userToken);
                return GetMethodOutput<DataProviderType[]>(facade);
            }
            public static BasicOutputType<bool> SetDataProviderList(string userToken, string appToken, DataProviderType[] dataProviders)
            {
                var facade = new DataProviders.SetDataProvidersFacadeMethodInfo(dataProviders, appToken, userToken);
                return GetMethodOutput<bool>(facade);
            }
            #endregion

            #region Application
            public static BasicOutputType<ApplicationType> RequestAppRegistration(string userToken, string appToken, string name)
            {
                var facade = new RequestAppRegistrationFacadeMethod(name, appToken, userToken);
                return GetMethodOutput<ApplicationType>(facade);
            }

            public static BasicOutputType<bool> ApproveAppRegistration(string userToken, string appToken, string targetAppToken)
            {
                var facade = new ApproveAppRegistrationFacadeMethod(targetAppToken, appToken, userToken);
                return GetMethodOutput<bool>(facade);
            }

            public static BasicOutputType<ApplicationType[]> ListAppRegistrations(string userToken, string appToken)
            {
                var facade = new ListAppRegistrationsFacadeMethod(appToken, userToken);
                return GetMethodOutput<ApplicationType[]>(facade);
            }

            public static BasicOutputType<bool> UnregisterApp(string userToken, string appToken, string targetAppToken)
            {
                var facade = new UnregisterAppFacadeMethod(targetAppToken, appToken, userToken);
                return GetMethodOutput<bool>(facade);
            }
            #endregion

            #region Logging

            public static BasicOutputType<bool> Log(string userToken, string appToken, string text)
            {
                var facade = new LogFacadeMethod(text, appToken, userToken);
                return GetMethodOutput<bool>(facade);
            }
            #endregion

        }
    }
}
