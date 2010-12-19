using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.Linq;

namespace CPRBroker.Engine
{
    public static class DataProviderManager
    {
        private static List<IDataProvider> DataProviders = new List<IDataProvider>();
        private static ReaderWriterLock DataProvidersLock = new ReaderWriterLock();

        static DataProviderManager()
        {
            InitializeDataProviders();
        }



        /// <summary>
        /// Converts the current DataProvider (database object) to the appropriate IDataProvider object based on its type
        /// </summary>
        /// <param name="dbDataProvider">The database object that represents the data provider</param>
        /// <returns>The newly created IDataProvider</returns>
        public static IDataProvider ToIDataProvider(this CPRBroker.DAL.DataProvider dbDataProvider)
        {
            Type dataProviderType = Type.GetType(dbDataProvider.DataProviderType.TypeName);
            object providerObj = dataProviderType.InvokeMember(null, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, null, null);
            IDataProvider dataProvider = providerObj as IDataProvider;
            if (dataProvider is IExternalDataProvider)
            {
                (dataProvider as IExternalDataProvider).DatabaseObject = dbDataProvider;
            }
            return dataProvider;
        }

        /// <summary>
        /// Initializes the list of all data providers
        /// </summary>
        public static void InitializeDataProviders()
        {
            BrokerContext.Initialize(DAL.Application.BaseApplicationToken.ToString(), Constants.UserToken, true, false, false);
            // Load from database
            using (DAL.CPRBrokerDALDataContext dataContext = new CPRBroker.DAL.CPRBrokerDALDataContext())
            {
                DataLoadOptions loadOptions = new DataLoadOptions();
                loadOptions.LoadWith<DAL.DataProvider>((dp) => dp.DataProviderType);
                dataContext.LoadOptions = loadOptions;
                var dbProviders = (from prov in dataContext.DataProviders
                                   where prov.DataProviderType.Enabled == true
                                   orderby prov.DataProviderType.IsExternal descending, prov.DataProviderId
                                   select prov);

                List<IDataProvider> providers = new List<IDataProvider>();
                foreach (var dbProv in dbProviders)
                {
                    try
                    {
                        IDataProvider dataProvider = dbProv.ToIDataProvider();
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

                // Now refresh the list
                DataProvidersLock.AcquireWriterLock(Timeout.Infinite);
                DataProviders.Clear();
                DataProviders.AddRange(providers);
                DataProvidersLock.ReleaseWriterLock();
            }
        }

        private static void SetDataProviderTimer()
        {
            System.Timers.Timer refreshTimer = new System.Timers.Timer(Config.Properties.Settings.Default.DataProviderSecondsRefreshPeriod);
            refreshTimer.AutoReset = true;
            refreshTimer.Elapsed += new System.Timers.ElapsedEventHandler((sender, e) => InitializeDataProviders());
        }

        internal static List<IDataProvider> GetDataProviderList<TInterface>(bool allowLocalProvider)
        {
            // Get list of all available data providers that are of type TInterface
            // First copy to local defined list to avoid threading issues
            List<IDataProvider> dataProviders = new List<IDataProvider>();
            DataProvidersLock.AcquireReaderLock(Timeout.Infinite);
            dataProviders.AddRange(DataProviders);
            DataProvidersLock.ReleaseReaderLock();
            // Now filter the list
            // TODO: Optimize calling of IsAlive to save waiting time when the DP is dead
            List<IDataProvider> availableProviders =
                (
                    from dp in dataProviders
                    where dp is TInterface
                    && (dp is IExternalDataProvider || allowLocalProvider)
                    select dp
                 ).ToList();
            return availableProviders;
        }

        internal static List<IDataProvider> GetDataProviderList(Type interfaceType, bool allowLocalProvider)
        {
            // Get list of all available data providers that are of type TInterface
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
                    && dp.IsAlive()
                    && (dp is IExternalDataProvider || allowLocalProvider)
                    select dp
                 ).ToList();
            return availableProviders;
        }
    }
}
