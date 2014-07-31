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
using CprBroker.Data.DataProviders;

namespace CprBroker.Engine
{
    /// <summary>
    /// Represents a facade method
    /// A web method maps to a facade method
    /// </summary>
    /// <typeparam name="TOutput">Type of object returned from the web method call. It must implement IBasicOutput (contains StandardRetur)</typeparam>
    /// <typeparam name="TItem">Type of the inner item of the result</typeparam>
    public class FacadeMethodInfo<TOutput, TItem> where TOutput : class, IBasicOutput<TItem>, new()
    {
        public AggregationFailOption AggregationFailOption = AggregationFailOption.FailOnAny;

        public FacadeMethodInfo()
        { }

        public FacadeMethodInfo(string appToken, string userToken)
        {
            ApplicationToken = appToken;
            UserToken = userToken;
        }

        public virtual StandardReturType ValidateInput()
        {
            return StandardReturType.OK();
        }

        public virtual void Initialize()
        {
            if (InitializationMethod != null)
            {
                InitializationMethod();
            }
        }
        public Action InitializationMethod = () => { };

        public virtual SubMethodRunState[] CreateSubMethodRunStates(out bool missingDataProvidersExist)
        {
            DataProvidersConfigurationSection section = Config.ConfigManager.Current.DataProvidersSection;
            DataProvider[] dbProviders = DataProviderManager.ReadDatabaseDataProviders();

            var subMethodRunStates = this.SubMethodInfos
                .Select(mi => new SubMethodRunState()
                {
                    SubMethodInfo = mi,
                    DataProviders = mi.GetDataProviderList(section, dbProviders)
                })
                .ToArray();

            // Now check that each method call info either has at least one clearData provider implementation or can be safely ignored. 
            missingDataProvidersExist = subMethodRunStates.Where(mi => mi.SubMethodInfo.FailIfNoDataProvider && mi.DataProviders.FirstOrDefault() == null).FirstOrDefault() != null;

            return subMethodRunStates;
        }

        public virtual TItem Aggregate(object[] results)
        {
            if (results != null && results.Length == 1)
            {
                return (TItem)(object)results[0];
            }
            else
            {
                return default(TItem);
            }
        }

        public virtual bool IsValidResult(TItem output)
        {
            return !object.Equals(output, default(TItem));
        }

        public string ApplicationToken;
        public string UserToken;

        public SubMethodInfo[] SubMethodInfos = new SubMethodInfo[0];
    }

    // TODO: Delete this enumeration because only FailOnAll is used
    public enum AggregationFailOption
    {
        // Fail if any submethod has failed
        FailOnAny,
        // Fail only if all submethods have failed
        FailOnAll,
        // Never fail, even if all submethods have failed
        FailNever
    }
}
