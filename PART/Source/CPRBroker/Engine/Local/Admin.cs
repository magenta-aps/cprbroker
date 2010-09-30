using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CPRBroker.Schemas;
using CPRBroker.Engine;
using CPRBroker.DAL;

namespace CPRBroker.Engine.Local
{
    /// <summary>
    /// Implementation of administration web methods
    /// </summary>
    public partial class Admin : ISubscriptionManager, IApplicationManager, IDataProviderManager, IVersionManager, ILoggingDataProvider
    {
        #region IDataProvider Members

        public bool IsAlive()
        {
            return true;
        }

        public Version Version
        {
            get { return new Version(Versioning.Major, Versioning.Minor); }
        }
        #endregion

        #region Application
        public ApplicationType RequestAppRegistration(string userToken, string name)
        {
            // Create a new application and assign a new app token
            using (CPRBrokerDALDataContext context = new CPRBrokerDALDataContext())
            {
                var application = new Application();
                application.ApplicationId = Guid.NewGuid();
                application.Token = Guid.NewGuid().ToString();
                context.Applications.InsertOnSubmit(application);

                application.Name = name;
                application.RegistrationDate = DateTime.Now;
                application.IsApproved = false;

                context.SubmitChanges();
                return application.ToXmlType();
            }
        }

        public bool ApproveAppRegistration(string userToken, string appToken, string targetAppToken)
        {
            // Mark the application as approved
            using (CPRBrokerDALDataContext context = new CPRBrokerDALDataContext())
            {
                var application = context.Applications.SingleOrDefault(app => app.Token == targetAppToken);
                if (application != null)
                {
                    application.IsApproved = true;
                    application.ApprovedDate = DateTime.Now;
                    context.SubmitChanges();
                    return true;
                }
                return false;
            }
        }

        public ApplicationType[] ListAppRegistration(string userToken, string appToken)
        {
            List<ApplicationType> applications = new List<ApplicationType>();
            // Retrieve list of applications and convert to XML
            using (CPRBrokerDALDataContext context = new CPRBrokerDALDataContext())
            {
                var businessApps = from app in context.Applications where app.IsApproved == true select app;
                foreach (var application in businessApps)
                {
                    applications.Add(application.ToXmlType());
                }
                return applications.ToArray();
            }
        }

        public bool UnregisterApp(string userToken, string appToken, string targetAppToken)
        {
            // Mark the application as unregistered
            using (CPRBrokerDALDataContext context = new CPRBrokerDALDataContext())
            {
                var application = context.Applications.SingleOrDefault(app => app.Token == targetAppToken);
                if (application != null)
                {
                    application.IsApproved = false;
                    context.SubmitChanges();
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region Provider list
        public Schemas.DataProviderType[] GetCPRDataProviderList(string userToken, string appToken)
        {
            // Get data providers from database and convert to the appropriate XML type
            using (CPRBrokerDALDataContext context = new CPRBrokerDALDataContext())
            {
                List<Schemas.DataProviderType> dataProviders = new List<CPRBroker.Schemas.DataProviderType>();
                var providers = from prov in context.DataProviders select prov;
                foreach (var provider in providers)
                {
                    Schemas.DataProviderType newProvider = null;
                    switch ((DataProviderTypes)provider.DataProviderTypeId)
                    {
                        case DataProviderTypes.DPR:
                            DprDataProviderType dpr = new DprDataProviderType();
                            dpr.Port = provider.Port.Value;
                            dpr.ConnectionString = provider.ConnectionString;
                            newProvider = dpr;
                            break;
                        case DataProviderTypes.KMD:
                            KmdDataProviderType kmd = new KmdDataProviderType();
                            kmd.Username = provider.UserName;
                            kmd.Password = provider.Password;
                            newProvider = kmd;
                            break;
                    }
                    if (newProvider != null)
                    {
                        newProvider.Address = provider.Address;
                        dataProviders.Add(newProvider);
                    }
                }
                return dataProviders.ToArray();
            }
        }

        public bool SetCPRDataProviderList(string userToken, string appToken, Schemas.DataProviderType[] dataProviders)
        {
            using (CPRBrokerDALDataContext context = new CPRBrokerDALDataContext())
            {
                #region Delete existing data
                var currentDataProviders =
                    from dataProvider in context.DataProviders
                    where dataProvider.DataProviderType.IsExternal == true
                    select dataProvider;

                if (currentDataProviders.Count() > 0)
                {
                    context.DataProviders.DeleteAllOnSubmit(currentDataProviders);
                }
                #endregion
                // Add new data providers
                Array.ForEach(dataProviders,
                    (Schemas.DataProviderType provider) =>
                    {
                        DataProvider local = new DataProvider();
                        local.Address = provider.Address;

                        if (provider is Schemas.DprDataProviderType)
                        {
                            local.DataProviderTypeId = (int)Schemas.DataProviderTypes.DPR;
                            Schemas.DprDataProviderType dpr = provider as Schemas.DprDataProviderType;
                            local.Port = dpr.Port;
                            local.ConnectionString = dpr.ConnectionString;
                        }
                        else if (provider is Schemas.KmdDataProviderType)
                        {
                            local.DataProviderTypeId = (int)Schemas.DataProviderTypes.KMD;
                            Schemas.KmdDataProviderType kmd = provider as Schemas.KmdDataProviderType;
                            local.UserName = kmd.Username;
                            local.Password = kmd.Password;
                        }

                        context.DataProviders.InsertOnSubmit(local);
                    });
                context.SubmitChanges();
                // Refresh current data provider list
                Manager.InitializeDataProviders();
                return true;
            }
        }
        #endregion

        #region Versioning managment

        private IQueryable<string> GetMethodNames()
        {
            // Use reflection to get a list of all methods from MethodNames classes
            List<System.Reflection.FieldInfo> fields = new List<System.Reflection.FieldInfo>();
            fields.AddRange(typeof(ServiceNames.Person.MethodNames).GetFields());
            fields.AddRange(typeof(ServiceNames.Administrator.MethodNames).GetFields());
            return
                from f in fields.AsQueryable()
                select f.Name;
        }
        public ServiceVersionType[] GetCapabilities(string userToken, string appToken)
        {
            ServiceVersionType cprVersion = new ServiceVersionType();

            cprVersion.Version = string.Format("{0}.{1}", Versioning.Major, Versioning.Minor);
            cprVersion.Functions.AddRange(GetMethodNames());

            return new ServiceVersionType[] { cprVersion };
        }

        public bool IsImplementing(string userToken, string appToken, string methodName, string version)
        {
            IQueryable<string> fields = GetMethodNames();
            foreach (var field in fields)
            {
                if (string.Equals(field.ToLower(), methodName.ToLower()) &&
                    string.Equals(version.ToLower(), string.Format("{0}.{1}", Versioning.Major, Versioning.Minor).ToLower()))
                    return true;
            }
            return false;
        }
        #endregion

        #region ILoggingDataProvider Members
        public bool Log(string userToken, string appToken, string text)
        {
            CPRBroker.Engine.Local.Admin.AddNewLog(System.Diagnostics.TraceEventType.Information, "LogFunctions", text, null, null);
            return true;
        }

        #endregion
    }
}
