using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CprBroker.Utilities;
using CprBroker.Data.DataProviders;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using System.Diagnostics;

namespace CprBroker.Engine
{
    /// <summary>
    /// Contains the logic for calling available data providers from a certain type
    /// </summary>
    /// <typeparam name="TInputElement"></typeparam>
    /// <typeparam name="TOutputElement"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TInterface"></typeparam>
    public partial class ProviderMethod<TInputElement, TOutputElement, TElement, TContext, TInterface>
        where TInterface : class, ISingleDataProvider<TInputElement, TOutputElement, TContext>
        where TElement : Element<TInputElement, TOutputElement>, new()
    {
        public TContext Context { get; private set; }

        public ProviderMethod()
        { }

        public ProviderMethod(TContext context)
        {
            this.Context = context;
        }

        public IEnumerable<TInterface> CreateDataProviders(SourceUsageOrder sourceUsageOrder)
        {
            DataProvidersConfigurationSection section = DataProvidersConfigurationSection.GetCurrent();
            DataProvider[] dbProviders = DataProviderManager.ReadDatabaseDataProviders();

            var dataProviders = DataProviderManager.GetDataProviderList(section, dbProviders, typeof(TInterface), sourceUsageOrder)
                .Select(p => p as TInterface);
            return dataProviders;
        }

        public TElement[] CallDataProviders(IEnumerable<TInterface> dataProviders, TInputElement[] input)
        {
            var allElements = input
                .Select(inp => new TElement() { Input = inp })
                .ToArray();

            foreach (var prov in dataProviders)
            {
                var currentElements = allElements
                    .Where(s => !IsElementSucceeded(s)).ToArray();

                if (currentElements.Length == 0)
                    break;

                TElement[] elementsToUpdate = null;

                if (prov is IBatchDataProvider<TInputElement, TOutputElement, TContext>)
                {
                    CallBatch(prov as IBatchDataProvider<TInputElement, TOutputElement, TContext>, currentElements, out elementsToUpdate);
                }
                else
                {
                    CallSingle(prov, currentElements, out elementsToUpdate);
                }

                // Exceptions here are logged separately
                if (prov is IExternalDataProvider && elementsToUpdate.Length > 0)
                {
                    BaseUpdateDatabase(elementsToUpdate.ToArray());
                }
            }
            return allElements;
        }

        public void CallBatch(IBatchDataProvider<TInputElement, TOutputElement, TContext> prov, TElement[] currentElements, out TElement[] elementsToUpdate)
        {
            var elementsToUpdateList = new List<TElement>();

            try
            {
                // Exceptions here will cause going to next data provider
                var currentOutput = prov.GetBatch(currentElements.Select(elm => elm.Input).ToArray(), Context);

                if (currentOutput.Length != currentElements.Length)
                    throw new Exception(string.Format("Output count mismatch when calling <{0}>, expected=<{1}, found=<{2}>>", prov.GetType().Name, currentElements.Length, currentOutput.Length));

                // Smooth code, no exceptions expected
                for (int i = 0; i < currentElements.Length; i++)
                {
                    var elm = currentElements[i];
                    elm.Output = currentOutput[i];

                    if (IsElementUpdatable(elm))
                    {
                        elementsToUpdateList.Add(elm);
                    }
                }
            }
            catch (Exception ex)
            {
                Local.Admin.LogException(ex);
            }
            elementsToUpdate = elementsToUpdateList.ToArray();
        }

        public void CallSingle(TInterface prov, TElement[] currentElements, out TElement[] elementsToUpdate)
        {
            var elementsToUpdateList = new List<TElement>();

            using (var elemnetLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion))
            {
                var threadStarts = currentElements.Select(elm => new ThreadStart(() =>
                {
                    try
                    {
                        elm.Output = prov.GetOne(elm.Input, Context);

                        if (IsElementUpdatable(elm))
                        {
                            if (prov is IExternalDataProvider && prov.ImmediateUpdatePreferred)
                            {
                                // TODO: Shall this be removed to avoid thread abortion in data update phase?
                                BaseUpdateDatabase(new TElement[] { elm });
                            }
                            else
                            {
                                try
                                {
                                    elemnetLock.EnterWriteLock();
                                    elementsToUpdateList.Add(elm);
                                }
                                finally
                                {
                                    elemnetLock.ExitWriteLock();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Local.Admin.LogException(ex);
                    }
                })).ToArray();

                ThreadRunner.RunThreads(threadStarts, TimeSpan.FromMilliseconds(Config.Properties.Settings.Default.DataProviderMillisecondsTimeout));
            }
            elementsToUpdate = elementsToUpdateList.ToArray();
        }

        public void BaseUpdateDatabase(TElement[] elementsToUpdate)
        {
            try
            {
                UpdateDatabase(
                    elementsToUpdate.Select(s => s.Input).ToArray(),
                    elementsToUpdate.Select(s => s.Output).ToArray()
                    );
            }
            catch (Exception updateException)
            {
                string xml = Strings.SerializeObject(elementsToUpdate);
                Local.Admin.LogException(updateException);
            }
        }
    }
}
