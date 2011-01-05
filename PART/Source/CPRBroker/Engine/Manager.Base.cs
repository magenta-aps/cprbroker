using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Data.Linq;

namespace CprBroker.Engine
{
    /// <summary>
    /// The main class of the system's engine.
    /// Manages calls to data providers
    /// </summary>
    public static partial class Manager
    {
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
        public static TOutput CallMethod<TInterface, TOutput>(string userToken, string appToken, bool failIfNoApp, bool allowLocalProvider, Func<TInterface, TOutput> func, bool failOnDefaultOutput, Action<TOutput> updateMethod) where TInterface : class, IDataProvider
        {
            try
            {
                // Initialize the context
                BrokerContext.Initialize(appToken, userToken, failIfNoApp, true, false);

                List<IDataProvider> availableProviders = DataProviderManager.GetDataProviderList<TInterface>(allowLocalProvider);
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
                    workerThread.Join(CprBroker.Config.Properties.Settings.Default.DataProviderMillisecondsTimeout);
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

        public static TOutput GetMethodOutput<TOutput>(FacadeMethodInfo<TOutput> facade)
        {
            try
            {
                #region Initialization and loading of data providers
                // Initialize context
                BrokerContext.Initialize(facade.ApplicationToken, facade.UserToken, facade.ApplicationTokenRequired, true, false);

                // Initialize facade method
                facade.Initialize();

                // have a list of data provider types and corresponding methods to call
                var subMethodRunStates =
                    (
                        from mi in facade.SubMethodInfos
                        select new SubMethodRunState()
                       {
                           SubMethodInfo = mi,
                           DataProviders = DataProviderManager.GetDataProviderList(mi.InterfaceType, mi.LocalDataProviderOption)
                       }
                   ).ToArray();

                // Now check that each method call info either has at least one data provider implementation or can be safely ignored. 
                var missingDataProvidersExist = (
                        (
                            from mi in subMethodRunStates
                            where mi.SubMethodInfo.FailIfNoDataProvider && mi.DataProviders.Count == 0
                            select mi
                        ).FirstOrDefault() != null
                    );

                if (missingDataProvidersExist)
                {
                    Local.Admin.AddNewLog(TraceEventType.Warning, BrokerContext.Current.WebMethodMessageName, TextMessages.NoDataProvidersFound, null, null);
                    return default(TOutput);
                }
                #endregion

                #region creation of sub results in threads
                // Catch the current broker context in a local variable
                var currentBrokerContext = BrokerContext.Current;

                // Now create the sub results
                for (int iSubMethod = 0; iSubMethod < subMethodRunStates.Length; iSubMethod++)
                {

                    subMethodRunStates[iSubMethod].Thread = new Thread(new ParameterizedThreadStart((o) =>
                        {
                            // Copy the broker context to this new thread
                            BrokerContext.Current = currentBrokerContext;

                            SubMethodRunState subMethodInfo = subMethodRunStates[(int)o];
                            // Loop over data providers until one succeeds
                            foreach (IDataProvider prov in subMethodInfo.DataProviders)
                            {
                                try
                                {
                                    subMethodInfo.UsedDataProvider = prov;
                                    object subResult = subMethodInfo.SubMethodInfo.Invoke(prov);
                                    if (subMethodInfo.SubMethodInfo.IsSuccessfulOutput(subResult))
                                    {
                                        subMethodInfo.Result = subResult;
                                        subMethodInfo.Succeeded = true;
                                        break;
                                    }
                                }
                                catch (Exception dataProviderException)
                                {
                                    Local.Admin.LogException(dataProviderException);
                                }
                            }

                            if (subMethodInfo.Succeeded)
                            {
                                if (subMethodInfo.UsedDataProvider is IExternalDataProvider)
                                {
                                    try
                                    {
                                        subMethodInfo.SubMethodInfo.InvokeUpdateMethod(subMethodInfo.Result);
                                    }
                                    catch (Exception updateException)
                                    {
                                        string xml = Util.Strings.SerializeObject(subMethodInfo.Result);
                                        Local.Admin.LogException(updateException);
                                    }
                                }
                            }
                            else
                            {
                                Local.Admin.AddNewLog(TraceEventType.Information, BrokerContext.Current.WebMethodMessageName, TextMessages.AllDataProvidersFailed + subMethodInfo.SubMethodInfo, null, null);
                            }

                            // Signal the end of processing
                            subMethodInfo.WaitHandle.Set();
                        }
                    ));
                    subMethodRunStates[iSubMethod].Thread.Start(iSubMethod);
                }
                #endregion

                #region Threads control

                // Wait for sub results to continue
                var waitHandles = (from mi in subMethodRunStates select mi.WaitHandle).ToArray();
                WaitHandle.WaitAll(waitHandles, Config.Properties.Settings.Default.DataProviderMillisecondsTimeout);
                foreach (var mi in subMethodRunStates)
                {
                    mi.Thread.Abort();
                }
                #endregion

                #region Final aggregation

                var subResultsFinished = (from mi in subMethodRunStates where !mi.Succeeded select mi).Count() == 0;
                if (subResultsFinished)
                {
                    var subResults = (from mi in subMethodRunStates select mi.Result).ToArray();
                    var output = facade.Aggregate(subResults);
                    if (facade.IsValidResult(output))
                    {
                        Local.Admin.AddNewLog(TraceEventType.Information, BrokerContext.Current.WebMethodMessageName, TextMessages.Succeeded, null, null);
                        return output;
                    }
                    else
                    {
                        string xml = Util.Strings.SerializeObject(output);
                        Local.Admin.AddNewLog(TraceEventType.Error, BrokerContext.Current.WebMethodMessageName, TextMessages.ResultGatheringFailed, typeof(TOutput).ToString(), xml);
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                Local.Admin.LogException(ex);
            }
            return default(TOutput);
        }

        private class SubMethodRunState
        {
            public SubMethodInfo SubMethodInfo;
            public List<IDataProvider> DataProviders;
            public object Result = null;
            public Thread Thread;
            public bool Succeeded = false;
            public ManualResetEvent WaitHandle = new ManualResetEvent(false);
            public IDataProvider UsedDataProvider;
        }

    }


}
