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
        // TODO: Convert to use generic XML types
        public Schemas.DataProviderType[] GetCPRDataProviderList(string userToken, string appToken)
        {
            // Get data providers from database and convert to the appropriate XML type
            using (var context = new DataProvidersDataContext())
            {
                DataProvider.SetChildLoadOptions(context);
                List<Schemas.DataProviderType> dataProviders = new List<CprBroker.Schemas.DataProviderType>();
                return (
                    from prov in context.DataProviders
                    select new DataProviderType()
                    {
                        TypeName = prov.TypeName,
                        Attributes = Array.ConvertAll<DataProviderProperty, AttributeType>(
                            prov.DataProviderProperties.OrderBy(p => p.Ordinal).ToArray(),
                            p => new AttributeType()
                            {
                                Name = p.Name,
                                Value = p.Value
                            }
                        )
                    }).ToArray();
            }
        }

        // TODO: Convert to use generic XML types and new DB structure
        public bool SetCPRDataProviderList(string userToken, string appToken, Schemas.DataProviderType[] dataProviders)
        {
            using (var context = new DataProvidersDataContext())
            {
                #region Delete existing data
                var currentDataProviders =
                    from dataProvider in context.DataProviders
                    //where dataProvider.DataProviderType.IsExternal == true
                    select dataProvider;

                if (currentDataProviders.Count() > 0)
                {
                    context.DataProviders.DeleteAllOnSubmit(currentDataProviders);
                }
                #endregion
                // Add new data providers
                for (int iProv = 0; iProv < dataProviders.Length; iProv++)
                {
                    DataProviderType oio = dataProviders[iProv];
                    DataProvider dbProv = new DataProvider()
                    {
                        TypeName = oio.TypeName,
                        DataProviderTypeId = 1,
                        IsEnabled = true,
                        IsExternal = true,
                        Ordinal = iProv,
                    };
                    var provObj = Util.Reflection.CreateInstance<IExternalDataProvider>(oio.TypeName);
                    var keys = provObj.ConfigurationKeys;
                    for (int iProp = 0; iProp < keys.Length; iProp++)
                    {
                        var propName = keys[iProp];
                        var oioProp = oio.Attributes.FirstOrDefault(p => p.Name == propName);
                        dbProv[keys[iProp]] = oioProp.Value;
                    }
                    context.DataProviders.InsertOnSubmit(dbProv);
                }
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
