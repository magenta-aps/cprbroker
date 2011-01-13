using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Engine
{
    public class FacadeMethodInfo<TOutput>
    {
        public FacadeMethodInfo()
        { }

        public FacadeMethodInfo(string appToken, string userToken, bool appTokenRequired)
        {
            ApplicationToken = appToken;
            UserToken = userToken;
            ApplicationTokenRequired = appTokenRequired;
        }

        //TODO: Write code for this method
        public virtual bool IsValidInput(ref TOutput invaliInputReturnValue)
        {
            return true;
        }

        public virtual void Initialize()
        {
            if (InitializationMethod != null)
            {
                InitializationMethod();
            }
        }
        public Action InitializationMethod = () => { };


        public virtual TOutput Aggregate(object[] results)
        {
            if (AggregationMethod != null)
            {
                return AggregationMethod(results);
            }
            else
            {
                return default(TOutput);
            }
        }

        public Func<object[], TOutput> AggregationMethod =
            (results) =>
            {
                if (results != null && results.Length == 1)
                {
                    return (TOutput)(object)results[0];
                }
                else
                {
                    return default(TOutput);
                }
            };

        public virtual bool IsValidResult(TOutput output)
        {
            return !object.Equals(output, default(TOutput));
        }

        public string ApplicationToken;
        public string UserToken;
        public bool ApplicationTokenRequired = true;

        public SubMethodInfo[] SubMethodInfos = new SubMethodInfo[0];
    }
}
