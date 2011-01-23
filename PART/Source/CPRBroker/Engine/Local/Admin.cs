using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CprBroker.Schemas;
using CprBroker.Engine;
using CprBroker.DAL;
using CprBroker.DAL.DataProviders;
using CprBroker.DAL.Applications;

namespace CprBroker.Engine.Local
{
    /// <summary>
    /// Implementation of administration web methods
    /// </summary>
    public partial class Admin : IDataProviderManager, IVersionManager, ILoggingDataProvider
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

        #region Provider list
        public Schemas.DataProviderType[] GetCPRDataProviderList(string userToken, string appToken)
        {
            // Get data providers from database and convert to the appropriate XML type
            using (var context = new DataProvidersDataContext())
            {
                List<Schemas.DataProviderType> dataProviders = new List<CprBroker.Schemas.DataProviderType>();
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
            using (var context = new DataProvidersDataContext())
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
                DataProviderManager.InitializeDataProviders();
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
            CprBroker.Engine.Local.Admin.AddNewLog(System.Diagnostics.TraceEventType.Information, "LogFunctions", text, null, null);
            return true;
        }

        #endregion
    }
}
