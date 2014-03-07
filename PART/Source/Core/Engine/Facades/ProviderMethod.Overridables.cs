using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Engine
{
    public partial class ProviderMethod<TInputElement, TOutputElement, TElement, TContext, TInterface>
        where TInterface : ISingleDataProvider<TInputElement, TOutputElement, TContext>
        where TElement : Element<TInputElement, TOutputElement>, new()
    {
        public virtual bool IsElementSucceeded(TElement element)
        {
            return !Object.Equals(element.Output, default(TOutputElement));
        }

        public virtual bool IsElementUpdatable(TElement element)
        {
            return !Object.Equals(element.Output, default(TOutputElement));
        }

        public virtual void UpdateDatabase(TInputElement[] input, TOutputElement[] output)
        {
        }
    }
}
