using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CprBroker.Engine
{
    /// <summary>
    /// Represents a facade sub method
    /// A sub method is responsible for getting one part of the needed output of a facade method
    /// </summary>
    public abstract class SubMethodInfo
    {
        public abstract Type InterfaceType { get; }
        public bool FailOnDefaultOutput;
        public LocalDataProviderUsageOption LocalDataProviderOption;
        public bool FailIfNoDataProvider;
        public abstract object Invoke(IDataProvider prov);
        public abstract void InvokeUpdateMethod(object result);
        public abstract bool IsSuccessfulOutput(object o);
        public abstract bool IsUpdatableOutput(object o);
    }

    public class SubMethodInfo<TInterface, TOutput> : SubMethodInfo where TInterface : class, IDataProvider
    {
        public Func<TInterface, TOutput> Method;

        public Action<TOutput> UpdateMethod;

        public TOutput CurrentResult;

        public override Type InterfaceType
        {
            get { return typeof(TInterface); }
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

        public sealed override bool  IsUpdatableOutput(object o)
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
    }

}
