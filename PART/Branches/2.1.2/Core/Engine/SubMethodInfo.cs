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
using CprBroker.Schemas;
using CprBroker.Data.DataProviders;

namespace CprBroker.Engine
{
    /// <summary>
    /// Represents a facade sub method
    /// A sub method is responsible for getting one part of the needed output of a facade method
    /// </summary>
    public abstract class SubMethodInfo
    {
        public abstract IEnumerable<IDataProvider> GetDataProviderList(DataProvidersConfigurationSection section, DataProvider[] dbProviders);
        public bool FailOnDefaultOutput;
        public SourceUsageOrder LocalDataProviderOption;
        public bool FailIfNoDataProvider;
        public abstract object Invoke(IDataProvider prov);
        public abstract void InvokeUpdateMethod(object result);
        public abstract bool IsSuccessfulOutput(object o);
        public abstract bool IsUpdatableOutput(object o);
        public abstract string InputToString();
        public abstract string PossibleErrorReason();
    }

    public class SubMethodInfo<TInterface, TOutput> : SubMethodInfo where TInterface : class, IDataProvider
    {
        public Func<TInterface, TOutput> Method;

        public Action<TOutput> UpdateMethod;

        public TOutput CurrentResult;

        public Type InterfaceType
        {
            get { return typeof(TInterface); }
        }

        public override IEnumerable<IDataProvider> GetDataProviderList(DataProvidersConfigurationSection section, DataProvider[] dbProviders)
        {
            return DataProviderManager.GetDataProviderList(section, dbProviders, this.InterfaceType, this.LocalDataProviderOption);
        }

        public override sealed object Invoke(IDataProvider prov)
        {
            var provider = prov as TInterface;
            CurrentResult = RunMainMethod(provider as TInterface);
            return CurrentResult;
        }

        public virtual TOutput RunMainMethod(TInterface prov)
        {
            if (Method != null)
            {
                return Method(prov);
            }
            else
            {
                return default(TOutput);
            }
        }

        public override void InvokeUpdateMethod(object result)
        {
            if (UpdateMethod != null)
            {
                UpdateMethod((TOutput)result);
            }
        }

        public sealed override bool IsSuccessfulOutput(object o)
        {
            if (typeof(TOutput).IsInstanceOfType(o))
            {
                return IsValidResult((TOutput)o);
            }
            else
            {
                return false;
            }
        }

        public virtual bool IsValidResult(TOutput result)
        {
            return IsNonEmptyResult(result);
        }

        public sealed override bool IsUpdatableOutput(object o)
        {
            if (typeof(TOutput).IsInstanceOfType(o))
            {
                return IsUpdatableResult((TOutput)o);
            }
            else
            {
                return false;
            }
        }

        public virtual bool IsUpdatableResult(TOutput result)
        {
            return IsNonEmptyResult(result);
        }

        protected bool IsNonEmptyResult(TOutput result)
        {
            if (FailOnDefaultOutput)
            {
                return !Object.Equals(result, default(TOutput));
            }
            else
            {
                return true;
            }
        }

        public override string InputToString()
        {
            return this.ToString();
        }

        public override string PossibleErrorReason()
        {
            return Schemas.Part.StandardReturType.DataProviderFailedText;
        }
    }

}
