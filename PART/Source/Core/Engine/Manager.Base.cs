using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Data.Linq;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine
{
    /// <summary>
    /// The main class of the system's engine.
    /// Manages calls to data providers
    /// </summary>
    public static partial class Manager
    {

        public static BasicOutputType<TItem> GetMethodOutput<TItem>(GenericFacadeMethodInfo<TItem> facade)
        {
            return GetMethodOutput<BasicOutputType<TItem>, TItem>(facade);
        }

        public static TOutput GetMethodOutput<TOutput, TItem>(FacadeMethodInfo<TOutput, TItem> facade) where TOutput : class, IBasicOutput<TItem>, new()
        {
            try
            {
                #region Initialization and loading of clearData providers
                // Initialize context
                try
                {
                    BrokerContext.Initialize(facade.ApplicationToken, facade.UserToken);
                }
                catch (Exception ex)
                {
                    return new TOutput()
                    {
                        StandardRetur = StandardReturType.InvalidApplicationToken(facade.ApplicationToken)
                    };
                }

                // Validate
                StandardReturType validationRet = facade.ValidateInput();
                if (!StandardReturType.IsSucceeded(validationRet))
                {
                    Local.Admin.AddNewLog(TraceEventType.Error, BrokerContext.Current.WebMethodMessageName, TextMessages.InvalidInput, null, null);
                    if (validationRet == null)
                    {
                        validationRet = StandardReturType.UnspecifiedError("Validation failed");
                    }
                    return new TOutput()
                    {
                        StandardRetur = validationRet
                    };
                }

                // Initialize facade method
                facade.Initialize();

                // have a list of clearData provider types and corresponding methods to call
                var subMethodRunStates = facade.SubMethodInfos
                    .Select(mi => new SubMethodRunState()
                    {
                        SubMethodInfo = mi,
                        DataProviders = DataProviderManager.GetDataProviderList(mi.InterfaceType, mi.LocalDataProviderOption)
                    })
                    .ToArray();

                // Now check that each method call info either has at least one clearData provider implementation or can be safely ignored. 
                var missingDataProvidersExist = subMethodRunStates.Where(mi => mi.SubMethodInfo.FailIfNoDataProvider && mi.DataProviders.Count == 0).FirstOrDefault() != null;

                if (missingDataProvidersExist)
                {
                    Local.Admin.AddNewLog(TraceEventType.Warning, BrokerContext.Current.WebMethodMessageName, TextMessages.NoDataProvidersFound, null, null);
                    return new TOutput()
                    {
                        StandardRetur = StandardReturType.Create(HttpErrorCode.DATASOURCE_UNAVAILABLE)
                    };
                }
                #endregion

                #region creation of sub results in threads
                // Catch the current broker context in a local variable
                var currentBrokerContext = BrokerContext.Current;

                // Now create the sub results
                for (int iSubMethod = 0; iSubMethod < subMethodRunStates.Length; iSubMethod++)
                {

                    subMethodRunStates[iSubMethod].ThreadStart = new ParameterizedThreadStart((o) =>
                        {
                            // Copy the broker context to this new thread
                            BrokerContext.Current = currentBrokerContext;

                            SubMethodRunState subMethodInfo = subMethodRunStates[(int)o];
                            // Loop over clearData providers until one succeeds
                            foreach (IDataProvider prov in subMethodInfo.DataProviders)
                            {
                                try
                                {
                                    object subResult = subMethodInfo.SubMethodInfo.Invoke(prov);
                                    // See if result can be used to update local database
                                    if (prov is IExternalDataProvider && subMethodInfo.SubMethodInfo.IsUpdatableOutput(subResult))
                                    {
                                        try
                                        {
                                            subMethodInfo.SubMethodInfo.InvokeUpdateMethod(subResult);
                                        }
                                        catch (Exception updateException)
                                        {
                                            string xml = DAL.Utilities.SerializeObject(subResult);
                                            Local.Admin.LogException(updateException);
                                        }
                                    }
                                    // Exit loop if succeeded
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

                            if (!subMethodInfo.Succeeded)
                            {
                                Local.Admin.AddNewLog(TraceEventType.Information, BrokerContext.Current.WebMethodMessageName, TextMessages.AllDataProvidersFailed + subMethodInfo.SubMethodInfo, null, null);
                            }

                            // Signal the end of processing
                            subMethodInfo.WaitHandle.Set();
                        }
                    );

                    if (subMethodRunStates.Length > 1)
                    {
                        subMethodRunStates[iSubMethod].Thread = new Thread(subMethodRunStates[iSubMethod].ThreadStart);
                        subMethodRunStates[iSubMethod].Thread.Start(iSubMethod);
                    }
                }
                #endregion

                #region Threads control
                if (subMethodRunStates.Length > 1)
                {
                    // Wait for sub results to continue
                    var waitHandles = (from mi in subMethodRunStates select mi.WaitHandle).ToArray();
                    WaitHandle.WaitAll(waitHandles, Config.Properties.Settings.Default.DataProviderMillisecondsTimeout);
                    foreach (var mi in subMethodRunStates)
                    {
                        mi.Thread.Abort();
                    }
                }
                else
                {
                    subMethodRunStates[0].ThreadStart.Invoke(0);
                }
                #endregion

                #region Final aggregation

                var subResultsFinished = (from mi in subMethodRunStates where !mi.Succeeded select mi).Count() == 0;
                if (subResultsFinished)
                {
                    var subResults = (from mi in subMethodRunStates select mi.Result).ToArray();
                    var outputMainItem = facade.Aggregate(subResults);
                    if (facade.IsValidResult(outputMainItem))
                    {
                        Local.Admin.AddNewLog(TraceEventType.Information, BrokerContext.Current.WebMethodMessageName, TextMessages.Succeeded, null, null);
                        var output = new TOutput() { StandardRetur = StandardReturType.OK() };
                        output.SetMainItem(outputMainItem);
                        return output;
                    }
                    else
                    {
                        string xml = DAL.Utilities.SerializeObject(outputMainItem);
                        Local.Admin.AddNewLog(TraceEventType.Error, BrokerContext.Current.WebMethodMessageName, TextMessages.ResultGatheringFailed, typeof(TOutput).ToString(), xml);
                        return new TOutput() { StandardRetur = StandardReturType.UnspecifiedError("Aggregation failed") };
                    }
                }
                else
                {
                    return new TOutput() { StandardRetur = StandardReturType.Create(HttpErrorCode.DATASOURCE_UNAVAILABLE) };
                }
                #endregion

            }
            catch (Exception ex)
            {
                Local.Admin.LogException(ex);
                return new TOutput() { StandardRetur = StandardReturType.UnspecifiedError() };
            }
        }

        private class SubMethodRunState
        {
            public SubMethodInfo SubMethodInfo;
            public List<IDataProvider> DataProviders;
            public object Result = null;
            public ParameterizedThreadStart ThreadStart;
            public Thread Thread;
            public bool Succeeded = false;
            public ManualResetEvent WaitHandle = new ManualResetEvent(false);
        }
    }
}
