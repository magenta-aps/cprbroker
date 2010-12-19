using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Engine
{
    public sealed class FacadeMethodInfo<TOutput>
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
            (results) => (TOutput)(object)results;

        internal Func<TOutput, bool> IsValidResult =
            (result) => !object.Equals(result, default(TOutput));


        public string ApplicationToken;
        public string UserToken;
        public bool ApplicationTokenRequired = true;

        public SubMethodInfo[] SubMethodInfos = new SubMethodInfo[0];
    }
}
