using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Data.Linq;

namespace CPRBroker.Engine
{
    /// <summary>
    /// The main class of the system's engine.
    /// Manages calls to data providers
    /// </summary>
    public static partial class Manager
    {
        private static List<IDataProvider> DataProviders = new List<IDataProvider>();
        private static ReaderWriterLock DataProvidersLock = new ReaderWriterLock();

        static Manager()
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
                            providers.Add(dataProvider);
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

        /// <summary>
        /// Searches for the appropriate data provider tha can implement a given method; then uses it to execute it
        /// </summary>
        /// <typeparam name="TInterface">Type of interface to look for, should inherit IDataProvider</typeparam>
        /// <typeparam name="TOutput">Return type of the method</typeparam>
        /// <param name="userToken">Token passed from the client system</param>
        /// <param name="appToken">Application token passed from the client system</param>
        /// <param name="failIfNoApp">Whether to throw an exception if no application matches the given application token</param>
        /// <param name="func">The method to be called on the interface</param>
        /// <param name="failOnDefaultOutput">Whether to treat a return value that is the default for TOutput (like null) as a failure or not </param>
        /// <param name="updateMethod">The method to call in order to update the system's database after data is retrieved</param>
        /// <returns>The output returned after calling the appropriate method on the found data provider, or the default for TOutput</returns>
        private static TOutput CallMethod<TInterface, TOutput>(string userToken, string appToken, bool failIfNoApp, bool allowLocalProvider, Func<TInterface, TOutput> func, bool failOnDefaultOutput, Action<TOutput> updateMethod) where TInterface : class, IDataProvider
        {
            try
            {
                // Initialize the context
                BrokerContext.Initialize(appToken, userToken, failIfNoApp, true, false);

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
                        where dp is TInterface
                        && dp.IsAlive()
                        && (dp is IExternalDataProvider || allowLocalProvider)
                        select dp
                     ).ToList();
                // Log an error if no provider is found
                if (availableProviders.Count == 0)
                {
                    Local.Admin.AddNewLog(TraceEventType.Warning, BrokerContext.Current.WebMethodMessageName, TextMessages.NoDataProvidersFound, null, null);
                }

                // Initialize variables
                TOutput output = default(TOutput);
                bool succeeded = false;
                TInterface usedProvider = null;

                // Now loop over all possible providers until a success is made
                foreach (TInterface provider in availableProviders)
                {
                    // Call the data provider
                    output = default(TOutput);
                    BrokerContext currentBrokerContext = BrokerContext.Current;
                    Thread workerThread = new Thread(new ThreadStart(() =>
                        {
                            try
                            {
                                // Copy the broker context to this new thread
                                BrokerContext.Current = currentBrokerContext;

                                usedProvider = provider;
                                output = func(provider);
                            }
                            catch (Exception ex)
                            {
                                Local.Admin.LogException(ex);
                            }
                        }));
                    workerThread.Start();
                    workerThread.Join(CPRBroker.Config.Properties.Settings.Default.DataProviderMillisecondsTimeout);
                    workerThread.Abort();

                    // Check for success
                    if (!object.Equals(output, default(TOutput)) || !failOnDefaultOutput)
                    {
                        succeeded = true;
                        break;
                    }
                }
                // Log and update local database
                if (succeeded)
                {
                    Local.Admin.AddNewLog(TraceEventType.Information, BrokerContext.Current.WebMethodMessageName, null, null, null);

                    // Now update the database if required (ignore this if the data came from our local database)
                    if (updateMethod != null && usedProvider is IExternalDataProvider)
                    {
                        try
                        {
                            updateMethod(output);
                        }
                        catch (Exception updateException)
                        {
                            string xml = Util.Strings.SerializeObject(output);
                            Local.Admin.AddNewLog(TraceEventType.Error, BrokerContext.Current.WebMethodMessageName, updateException.ToString(), typeof(TOutput).ToString(), xml);
                        }
                    }
                }
                else
                {
                    Local.Admin.AddNewLog(TraceEventType.Error, BrokerContext.Current.WebMethodMessageName, TextMessages.AllDataProvidersFailed, null, null);
                }
                return output;
            }
            catch (Exception ex)
            {
                Local.Admin.LogException(ex);
            }
            // Default return value
            return default(TOutput);
        }

    }


}
