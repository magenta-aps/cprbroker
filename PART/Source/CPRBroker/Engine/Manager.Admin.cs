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
            public static ServiceVersionType[] GetCapabilities(string userToken, string appToken)
            {
                return CallMethod<IVersionManager, ServiceVersionType[]>
                (userToken, appToken, true, (admin) => admin.GetCapabilities(userToken, appToken), true, null);
            }
            public static bool IsImplementing(string userToken, string appToken, string methodName, string version)
            {
                return CallMethod<IVersionManager, bool>
                (userToken, appToken, true, (admin) => admin.IsImplementing(userToken, appToken, methodName, version), false, null);
            }
            #endregion

            #region Provider list
            public static BasicOutputType<Schemas.DataProviderType[]> GetDataProviderList(string userToken, string appToken)
            {
                var facade = new DataProviders.GetDataProviderListFacadeMethodInfo(appToken, userToken);
                return GetMethodOutput<BasicOutputType<DataProviderType[]>>(facade);
            }
            public static BasicOutputType<bool> SetDataProviderList(string userToken, string appToken, DataProviderType[] dataProviders)
            {
                var facade = new DataProviders.SetDataProvidersFacadeMethodInfo(dataProviders, appToken, userToken);
                return GetMethodOutput<BasicOutputType<bool>>(facade);
            }
            #endregion

            #region Application
            public static ApplicationType RequestAppRegistration(string userToken, string appToken, string name)
            {
                return CallMethod<IApplicationManager, ApplicationType>
                (userToken, appToken, true, (admin) => admin.RequestAppRegistration(userToken, name), true, null);
            }

            public static bool ApproveAppRegistration(string userToken, string appToken, string targetAppToken)
            {
                Console.Write("Manager to approve app registration:  aT=" + appToken + ", uT=" + userToken + ", target=" + targetAppToken + "\n");
                return CallMethod<IApplicationManager, bool>
            (userToken, appToken, true, (admin) => admin.ApproveAppRegistration(userToken, appToken, targetAppToken), true, null);
            }

            public static ApplicationType[] ListAppRegistrations(string userToken, string appToken)
            {
                return CallMethod<IApplicationManager, ApplicationType[]>
                (userToken, appToken, true, (admin) => admin.ListAppRegistration(userToken, appToken), true, null);
            }

            public static bool UnregisterApp(string userToken, string appToken, string targetAppToken)
            {
                return CallMethod<IApplicationManager, bool>
                (userToken, appToken, true, (admin) => admin.UnregisterApp(userToken, appToken, targetAppToken), true, null);
            }
            #endregion

            #region Logging

            public static bool Log(string userToken, string appToken, string text)
            {
                return CallMethod<ILoggingDataProvider, bool>(userToken, appToken, true, (ILoggingDataProvider log) => log.Log(userToken, appToken, text), true, null);
            }
            #endregion

        }
    }
}
