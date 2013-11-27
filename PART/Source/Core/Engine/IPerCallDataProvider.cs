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
using CprBroker.Data.Applications;

namespace CprBroker.Engine
{
    /// <summary>
    /// Represents an external PAID data provider (DPR Diversion, P-date and CPR Direct)
    /// </summary> 
    public interface IPerCallDataProvider : IExternalDataProvider
    {
        string[] OperationKeys { get; }
    }

    public static class IPerCallDataProviderHelper
    {
        public static string ToOperationCostPropertyName(string operationName)
        {
            return string.Format("{0} - cost", operationName);
        }

        public static decimal GetOperationCost(this IPerCallDataProvider provider, string operation)
        {
            var ret = 0m;
            var propName = ToOperationCostPropertyName(operation);
            if (provider.ConfigurationProperties.ContainsKey(propName))
            {
                decimal.TryParse(provider.ConfigurationProperties[propName], out ret);
            }
            return ret;
        }

        public static void LogAction(this IPerCallDataProvider provider, String operation, string input, Boolean success)
        {
            //We find the cost for this call
            decimal cost = provider.GetOperationCost(operation);

            //We put a row into the DataProviderCall table
            using (var dataContext = new ApplicationDataContext())
            {
                var call = new DataProviderCall
                {
                    DataProviderCallId = System.Guid.NewGuid(),
                    ActivityId = BrokerContext.Current.ActivityId,
                    CallTime = DateTime.Now,
                    Cost = cost,
                    Input = input,
                    DataProviderType = provider.GetType().ToString(),
                    Operation = operation,
                    Success = success,
                };
                dataContext.DataProviderCalls.InsertOnSubmit(call);
                dataContext.SubmitChanges();
            }
        }

        public static bool CanCallOnline(string pnr)
        {
            return CanCallOnline(Config.Properties.Settings.Default.Modulus11LowLevelEnabled, pnr);
        }

        public static bool CanCallOnline(bool modulus11Enabled, string pnr)
        {
            return !modulus11Enabled
                || Utilities.Strings.IsModulus11OK(pnr);
        }

        public static DataProviderConfigPropertyInfo ToDataProviderConfigPropertyInfo(string operationName)
        {
            return new DataProviderConfigPropertyInfo()
            {
                Name = ToOperationCostPropertyName(operationName),
                Confidential = false,
                Required = true,
                Type = DataProviderConfigPropertyInfoTypes.Decimal
            };
        }

        public static DataProviderConfigPropertyInfo[] ToDataProviderConfigPropertyInfo(string[] operationNames)
        {
            return operationNames
                .Select(opName => ToDataProviderConfigPropertyInfo(opName))
                .ToArray();
        }

        public static DataProviderConfigPropertyInfo[] ToOperationConfigPropertyInfo(this IPerCallDataProvider prov)
        {
            return ToDataProviderConfigPropertyInfo(prov.OperationKeys);
        }

        public static DataProviderConfigPropertyInfo[] ToAllPropertyInfo(this IExternalDataProvider prov)
        {
            var configKeys = prov.ConfigurationKeys;

            if (prov is IPerCallDataProvider)
            {
                configKeys = configKeys.Union((prov as IPerCallDataProvider).ToOperationConfigPropertyInfo()).ToArray();
            }

            return configKeys;
        }
    }
}
