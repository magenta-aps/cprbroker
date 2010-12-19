using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Engine
{
    public sealed class FacadeMethodInfo<TOutput>
    {
        public Func<object[], TOutput> AggregationMethod =
            (results) => (TOutput)(object)results;

        public Func<TOutput, bool> IsValidResult =
            (result) => !object.Equals(result, default(TOutput));

        public string ApplicationToken;
        public string UserToken;
        public bool ApplicationTokenRequired = true;

        public SubMethodInfo[] SubMethodInfos = new SubMethodInfo[0];
    }
}
