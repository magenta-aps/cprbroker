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
using CprBroker.Utilities;
using CprBroker.Schemas;
using CprBroker.Data.DataProviders;
using System.Diagnostics;

namespace CprBroker.Engine
{

    public partial class ClientMethod<TInterface, TInputElement, TIntermediateElement, TOutputElement>
        where TInterface : class,ISingleDataProvider<TInputElement, TOutputElement, object>
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
                var providerMethod = new ProviderMethod<TInputElement, TOutputElement, Element, object, TInterface>();
                var allElements = providerMethod.CallDataProviders(dataProviders, input);

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
