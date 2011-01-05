using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CprBroker.Engine
{
    public abstract class SubMethodInfo
    {
        public abstract Type InterfaceType { get; }
        public bool FailOnDefaultOutput;
        public LocalDataProviderUsageOption LocalDataProviderOption;
        public bool FailIfNoDataProvider;
        public abstract object Invoke(IDataProvider prov);
        public abstract void InvokeUpdateMethod(object result);
        public abstract bool IsSuccessfulOutput(object o);
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
            CurrentResult = Method(provider as TInterface);
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

        public override bool IsSuccessfulOutput(object o)
        {
            if (FailOnDefaultOutput)
            {
                return !Object.Equals(o, default(TOutput));
            }
            else
            {
                return true;
            }
        }
    }

}
