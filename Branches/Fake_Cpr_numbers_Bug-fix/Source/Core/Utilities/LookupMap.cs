using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Utilities
{
    public class LookupMap<TKey, TValue>
    {
        private Dictionary<TKey, TValue> Mappings = new Dictionary<TKey, TValue>();

        protected void AddMapping(TKey key, TValue value)
        {
            Mappings[key] = value;
        }

        public bool ContainsKey(TKey key)
        {
            return Mappings.ContainsKey(key);
        }

        public TValue Map(TKey c)
        {
            return Mappings[c];
        }

        public TKey UnMap(TValue status)
        {
            return Mappings.Where((kvp) => kvp.Value.Equals(status)).First().Key;
        }
    }
}
