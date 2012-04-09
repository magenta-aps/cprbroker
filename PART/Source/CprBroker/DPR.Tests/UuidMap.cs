using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Tests.DPR
{
    public class UuidMap
    {
        static Dictionary<decimal, Guid> _Map = new Dictionary<decimal, Guid>();
        public static Guid CprToUuid(decimal cpr)
        {
            if (!_Map.ContainsKey(cpr))
                _Map[cpr] = Guid.NewGuid();
            return _Map[cpr];
        }
        public static Guid CprStringToUuid(string cpr)
        {
            return CprToUuid(decimal.Parse(cpr));
        }
    }
}
