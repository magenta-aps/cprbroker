using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Data.Linq;
using System.IO;
using CprBroker.DAL.DataProviders;

namespace CprBroker.Engine
{
    public static class DataProviderManager
    {
        private static List<IDataProvider> DataProviders = new List<IDataProvider>();
        private static ReaderWriterLock DataProvidersLock = new ReaderWriterLock();

        #region Initialization

        static DataProviderManager()
        {
            InitializeDataProviders();
            SetDataProviderRefreshTimer();
        }

        /// <summary>
        /// Initializes the list of all data providers
        /// </summary>
        public static void InitializeDataProviders()
        {
            BrokerContext.Initialize(DAL.Applications.Application.BaseApplicationToken.ToString(), Constants.UserToken);
            try
            {
                // Load fromDate database                
                var external = LoadExternalDataProviders();

                // Append local data providers
                var local = LoadLocalDataProviders();

                // Now refresh the list
                DataProvidersLock.AcquireWriterLock(Timeout.Infinite);
                DataProviders.Clear();
                DataProviders.AddRange(local);
                DataProviders.AddRange(external);
                DataProvidersLock.ReleaseWriterLock();
            }
            catch (Exception ex)
            {
                Local.Admin.LogException(ex);
            }
        }

        /// <summary>
        /// Initializes a timer that refreshes the working data provider list. Mainly to re-ping the external data providers
        /// </summary>
        private static void SetDataProviderRefreshTimer()
        {
            System.Timers.Timer timer = new System.Timers.Timer(Config.Properties.Settings.Default.DataProviderRefreshPeriodMilliseconds);
            timer.AutoReset = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler((sender, e) => InitializeDataProviders());
            timer.Start();
        }

        #endregion

        #region Creators
        /// <summary>
        /// Converts the current DataProvider (database object) to the appropriate IDataProvider object based on its type
        /// </summary>
        /// <param name="dbDataProvider">The database object that represents the data provider</param>
        /// <returns>The newly created IDataProvider</returns>
        public static IExternalDataProvider CreateDataProvider(CprBroker.DAL.DataProviders.DataProvider dbDataProvider)
        {
            IExternalDataProvider dataProvider = CreateDataProvider(dbDataProvider.TypeName) as IExternalDataProvider;
            if (dataProvider is IExternalDataProvider)
            {
                try
                {
                    IExternalDataProvider externalProvider = dataProvider as IExternalDataProvider;
                    externalProvider.ConfigurationProperties = dbDataProvider.ToPropertiesDictionary(externalProvider.ConfigurationKeys.Select(p=>p.Name).ToArray());
                }
                catch (Exception ex)
                {
                    Local.Admin.LogException(ex);
                }
            }
            return dataProvider;
        }

        private static IDataProvider CreateDataProvider(string typeName)
        {
            return Util.Reflection.CreateInstance<IDataProvider>(typeName);
        }

        #endregion

        #region Loaders

        public static Type[] GetAvailableDataProviderTypes(bool isExternal)
        {
            // Load available types
            List<Type> neededTypes = new List<Type>();
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var assemblyTypes = asm.GetTypes();
                    foreach (Type t in assemblyTypes)
                    {
                        if (t.IsClass && !t.IsAbstract)
                        {
                            if (isExternal && typeof(IExternalDataProvider).IsAssignableFrom(t))
                            {
                                neededTypes.Add(t);
                            }
                            else if (!isExternal && typeof(IDataProvider).IsAssignableFrom(t) && !typeof(IExternalDataProvider).IsAssignableFrom(t))
                            {
                                neededTypes.Add(t);
                            }
                        }
                    }
                }
                catch (System.Reflection.ReflectionTypeLoadException ex)
                {
                    Local.Admin.LogException(ex, ex.LoaderExceptions);
                }
                catch (Exception ex)
                {
                    Local.Admin.LogException(ex);
                }
            }
            return neededTypes.ToArray();
        }

        private static IExternalDataProvider[] LoadExternalDataProviders()
        {
            using (var dataContext = new CprBroker.DAL.DataProviders.DataProvidersDataContext())
            {
                DataProvider.SetChildLoadOptions(dataContext);

                var dbProviders = (from prov in dataContext.DataProviders
                                   where prov.IsEnabled
                                   orderby prov.Ordinal
                                   select prov);

                List<IExternalDataProvider> providers = new List<IExternalDataProvider>();

                // Append external clearData providers
                foreach (var dbProv in dbProviders)
                {
                    try
                    {
                        IExternalDataProvider dataProvider = CreateDataProvider(dbProv) as IExternalDataProvider;
                        if (dataProvider != null)
                        {
                            if (dataProvider.IsAlive())
                            {
                                providers.Add(dataProvider);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Local.Admin.LogException(ex);
                    }
                }
                return providers.ToArray();
            }
        }

        private static IDataProvider[] LoadLocalDataProviders()
        {
            return Array.ConvertAll<Type, IDataProvider>(GetAvailableDataProviderTypes(false), t => CreateDataProvider(t.AssemblyQualifiedName));
        }

        #endregion

        #region Filtration by type

        internal static List<IDataProvider> GetDataProviderList<TInterface>(bool allowLocalProvider)
        {
            return GetDataProviderList<TInterface>(allowLocalProvider ? LocalDataProviderUsageOption.UseFirst : LocalDataProviderUsageOption.Forbidden);
        }

        internal static List<IDataProvider> GetDataProviderList<TInterface>(LocalDataProviderUsageOption localOption)
        {
            return GetDataProviderList(typeof(TInterface), localOption);
        }

        internal static List<IDataProvider> GetDataProviderList(Type interfaceType, bool allowLocalProvider)
        {
            return GetDataProviderList(interfaceType, allowLocalProvider);
        }

        internal static List<IDataProvider> GetDataProviderList(Type interfaceType, LocalDataProviderUsageOption localOption)
        {
            // GetPropertyValuesOfType list of all available clearData providers that are of type TInterface
            // First copy to local defined list to avoid threading issues
            List<IDataProvider> dataProviders = new List<IDataProvider>();
            DataProvidersLock.AcquireReaderLock(Timeout.Infinite);
            dataProviders.AddRange(DataProviders);
            DataProvidersLock.ReleaseReaderLock();
            // Now filter the list
            List<IDataProvider> availableProviders =
                (
                    from dp in dataProviders
                    where interfaceType.IsInstanceOfType(dp)
                    && dp is IExternalDataProvider
                    select dp
                 ).ToList();

            // Now add the local clearData providers if needed
            if (localOption != LocalDataProviderUsageOption.Forbidden)
            {
                var availableLocalProviders =
                (
                    from dp in dataProviders
                    where interfaceType.IsInstanceOfType(dp)
                    && !(dp is IExternalDataProvider)
                    select dp
                 ).ToList();

                if (localOption == LocalDataProviderUsageOption.UseFirst)
                {
                    availableProviders.InsertRange(0, availableLocalProviders);
                }
                else
                {
                    availableProviders.AddRange(availableLocalProviders);
                }
            }
            return availableProviders;
        }

        #endregion
    }
}
