using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CprBroker.Utilities;

namespace CprBroker.Engine
{
    public class ProviderMethod<TInputElement, TOutputElement, TInterface> : ProviderMethod<TInputElement, TOutputElement, Element<TInputElement, TOutputElement>, TInterface>
        where TInterface : ISingleDataProvider<TInputElement, TOutputElement>
    { }

    public partial class ProviderMethod<TInputElement, TOutputElement, TElement, TInterface>
        where TInterface : ISingleDataProvider<TInputElement, TOutputElement>
        where TElement : Element<TInputElement, TOutputElement>, new()
    {

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

                if (prov is IBatchDataProvider<TInputElement, TOutputElement>)
                {
                    CallBatch(prov as IBatchDataProvider<TInputElement, TOutputElement>, currentElements, out elementsToUpdate);
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

        public void CallBatch(IBatchDataProvider<TInputElement, TOutputElement> prov, TElement[] currentElements, out TElement[] elementsToUpdate)
        {
            var elementsToUpdateList = new List<TElement>();

            try
            {
                // Exceptions here will cause going to next data provider
                var currentOutput = prov.GetBatch(currentElements.Select(elm => elm.Input).ToArray());

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
                        elm.Output = prov.GetOne(elm.Input);

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
