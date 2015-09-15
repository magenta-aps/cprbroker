using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Tests.DBR.Comparison
{
    class LoadCache
    {
        public static LoadCache Root = new LoadCache();

        Dictionary<string, object> Cache = new Dictionary<string, object>();

        public T GetOrDefault<T>(string key)
        {
            if (Cache.ContainsKey(key))
                return (T)Cache[key];
            else
                return default(T);
        }

        public bool GetBoolean(string key)
        {
            return GetOrDefault<bool>(key);
        }
    }
}
