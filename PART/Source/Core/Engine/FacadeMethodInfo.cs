using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

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
}
