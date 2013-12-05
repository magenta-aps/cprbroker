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
using CprBroker.Schemas.Part;
using System.Threading;
using CprBroker.Utilities;
using CprBroker.Schemas;
using CprBroker.Data.DataProviders;
using System.Diagnostics;

namespace CprBroker.Engine
{

    public partial class ClientMethod<TInterface, TInputElement, TIntermediateElement, TOutputElement>
        where TInterface : class,ISingleDataProvider<TInputElement, TOutputElement>
        where TIntermediateElement : ClientMethod<TInterface, TInputElement, TIntermediateElement, TOutputElement>.Element
    {

        public SourceUsageOrder LocalDataProviderOption = SourceUsageOrder.LocalThenExternal;

        /// <summary>
        /// Gets result for a single input element.
        /// It is basically a wrapper for the more general array version GetBatch<>()
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <typeparam name="TBatchOutput"></typeparam>
        /// <param name="header"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public TOutput GetSingle<TOutput, TBatchOutput>(MethodHeader header, TInputElement input)
            where TOutput : IBasicOutput<TOutputElement>, new()
            where TBatchOutput : IBasicOutput<TOutputElement[]>, new()
        {
            var batchRet = GetBatch<TBatchOutput>(header, new TInputElement[] { input });

            return new TOutput()
            {
                StandardRetur = batchRet.StandardRetur,
                Item = batchRet.Item.Length == 1 ? batchRet.Item[0] : default(TOutputElement)
            };
        }

        /// <summary>
        /// Gets the result for an array of input items
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="header">Header tokens</param>
        /// <param name="input">Array of input items</param>
        /// <returns>Composite result for the operation</returns>
        public TOutput GetBatch<TOutput>(MethodHeader header, TInputElement[] input)
            where TOutput : IBasicOutput<TOutputElement[]>, new()
        {
            try
            {
                // Validate
                var ret = BaseValidate<TOutput>(header, input);

                if (!StandardReturType.IsSucceeded(ret))
                    return new TOutput() { StandardRetur = ret };

                // Data providers
                IEnumerable<TInterface> dataProviders;

                ret = CreateDataProviders(out dataProviders);

                if (!StandardReturType.IsSucceeded(ret))
                    return new TOutput() { StandardRetur = ret };

                // Call data providers
                var allElements = CallDataProviders(dataProviders, input);

                // Aggregate
                return Aggregate<TOutput>(allElements);
            }
            catch (Exception ex)
            {
                Local.Admin.LogException(ex);
                return new TOutput() { StandardRetur = StandardReturType.UnspecifiedError() };
            }
        }


        public virtual StandardReturType BaseValidate<TOutput>(MethodHeader header, TInputElement[] input)
        {
            // Initialize context
            try
            {
                BrokerContext.Initialize(header.ApplicationToken, header.UserToken);
            }
            catch (Exception ex)
            {
                return StandardReturType.InvalidApplicationToken(header.ApplicationToken);
            }

            // Validate input
            if (input == null || input.Length == 0)
            {
                return StandardReturType.NullInput();
            }

            var ret = Validate(input);
            if (ret == null)
            {
                ret = StandardReturType.UnspecifiedError("Validation failed");
            }

            return ret;
        }

        public StandardReturType CreateDataProviders(out IEnumerable<TInterface> dataProviders)
        {
            DataProvidersConfigurationSection section = DataProvidersConfigurationSection.GetCurrent();
            DataProvider[] dbProviders = DataProviderManager.ReadDatabaseDataProviders();

            dataProviders = DataProviderManager.GetDataProviderList(section, dbProviders, typeof(TInterface), this.LocalDataProviderOption)
                .Select(p => p as TInterface);
            if (dataProviders.FirstOrDefault() == null)
            {
                Local.Admin.AddNewLog(TraceEventType.Warning, BrokerContext.Current.WebMethodMessageName, TextMessages.NoDataProvidersFound, null, null);
                return StandardReturType.Create(HttpErrorCode.DATASOURCE_UNAVAILABLE);
            }
            return StandardReturType.OK();
        }

        public Element[] CallDataProviders(IEnumerable<TInterface> dataProviders, TInputElement[] input)
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

        public void CallSingle(TInterface prov, Element[] currentElements, out Element[] elementsToUpdate)
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

        public TOutput Aggregate<TOutput>(Element[] elements)
            where TOutput : IBasicOutput<TOutputElement[]>, new()
        {
            // Set output item - only copy succeeded elements
            var ret = new TOutput();
            ret.Item = elements.Select(
                s => this.IsElementSucceeded(s) ? s.Output : default(TOutputElement)
                ).ToArray();

            // Set standard return
            var failed = elements.Where(s => !IsElementSucceeded(s)).ToArray();
            var succeededCount = elements.Length - failed.Length;

            if (succeededCount == 0)
            {
                ret.StandardRetur = StandardReturType.Create(HttpErrorCode.DATASOURCE_UNAVAILABLE);
            }
            else if (succeededCount < elements.Length)
            {
                var failuresAndReasons = failed
                    .GroupBy(s => s.PossibleErrorReason)
                    .ToDictionary(
                        g => g.Key,
                        g => g.ToArray()
                            .Select(s => string.Format("{0}", s.Input))
                    );
                ret.StandardRetur = StandardReturType.PartialSuccess(failuresAndReasons);
            }
            else
            {
                ret.StandardRetur = StandardReturType.OK();
            }

            // final return
            return ret;
        }
    }

}
