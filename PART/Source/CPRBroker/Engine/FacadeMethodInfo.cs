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

        public Action InitializationMethod = () => { };

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

        internal Func<TOutput, bool> IsValidResult =
            (result) => !object.Equals(result, default(TOutput));


        public string ApplicationToken;
        public string UserToken;
        public bool ApplicationTokenRequired = true;

        public SubMethodInfo[] SubMethodInfos = new SubMethodInfo[0];
    }
}
