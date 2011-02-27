using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine
{
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
