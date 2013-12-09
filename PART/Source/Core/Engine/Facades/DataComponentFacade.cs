/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Data.DataProviders;
using CprBroker.Engine;
using CprBroker.Utilities;

namespace CprBroker.Engine
{
    public abstract partial class DataComponentFacade<TInputElement, TOutputElement> : DataComponentFacade
    {
        public override Type InterfaceType
        {
            get { return typeof(ISingleDataProvider<TInputElement, TOutputElement>); }
        }

        public StandardReturType CreateDataProviders(out IEnumerable<ISingleDataProvider<TInputElement, TOutputElement>> dataProviders, SourceUsageOrder sourceUsageOrder)
        {
            DataProvidersConfigurationSection section = DataProvidersConfigurationSection.GetCurrent();
            DataProvider[] dbProviders = DataProviderManager.ReadDatabaseDataProviders();

            dataProviders = DataProviderManager.GetDataProviderList(section, dbProviders, InterfaceType, sourceUsageOrder)
                .Select(p => p as ISingleDataProvider<TInputElement, TOutputElement>);
            if (dataProviders.FirstOrDefault() == null)
            {
                Local.Admin.AddNewLog(TraceEventType.Warning, BrokerContext.Current.WebMethodMessageName, TextMessages.NoDataProvidersFound, null, null);
                return StandardReturType.Create(HttpErrorCode.DATASOURCE_UNAVAILABLE);
            }
            return StandardReturType.OK();
        }

        public Element[] CallDataProviders(IEnumerable<ISingleDataProvider<TInputElement, TOutputElement>> dataProviders, TInputElement[] input)
        {
            var allElements = input
                .Select(inp => new Element() { Input = inp })
                .ToArray();

            foreach (var prov in dataProviders)
            {
                var currentElements = allElements
                    .Where(s => !IsElementSucceeded(s)).ToArray();

                if (currentElements.Length == 0)
                    break;

                Element[] elementsToUpdate = null;

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

        public void CallBatch(IBatchDataProvider<TInputElement, TOutputElement> prov, Element[] currentElements, out Element[] elementsToUpdate)
        {
            var elementsToUpdateList = new List<Element>();

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

        public void CallSingle(ISingleDataProvider<TInputElement, TOutputElement> prov, Element[] currentElements, out Element[] elementsToUpdate)
        {
            var elementsToUpdateList = new List<Element>();

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
                                BaseUpdateDatabase(new Element[] { elm });
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

        public void BaseUpdateDatabase(Element[] elementsToUpdate)
        {
            try
            {
                UpdateLocal(
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

        public override Array GetChanges(IDataProvider prov, int c)
        {
            return GetChanges(prov as IAutoUpdateDataProvider<TInputElement, TOutputElement>, c);
        }

        public TInputElement[] GetChanges(IAutoUpdateDataProvider<TInputElement, TOutputElement> dataProvider, int c)
        {
            return dataProvider.GetChanges(c);
        }

        public override Array GetObjects(IDataProvider prov, Array keys)
        {
            return GetObjects(prov as IAutoUpdateDataProvider<TInputElement, TOutputElement>, keys as TInputElement[]);
        }

        public Array GetObjects(IAutoUpdateDataProvider<TInputElement, TOutputElement> prov, TInputElement[] keys)
        {
            return prov.GetBatch(keys);
        }

        public override void DeleteChanges(IDataProvider prov, Array keys)
        {
            DeleteChanges(prov as IAutoUpdateDataProvider<TInputElement, TOutputElement>, keys);
        }

        public void DeleteChanges(IAutoUpdateDataProvider<TInputElement, TOutputElement> prov, Array keys)
        {
            prov.DeleteChanges(keys as TInputElement[]);
        }

        public override sealed void UpdateLocal(Array keys, Array values)
        {
            var keys2 = keys.OfType<TInputElement>().ToArray();
            var values2 = values.OfType<TOutputElement>().ToArray();
            UpdateLocal(keys2, values2);
        }

    }

}
