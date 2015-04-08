using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine.Part;

namespace CprBroker.Tests.ServicePlatform
{
    public class UuidCacheFactory
    {
        static Dictionary<string, Guid> UuidMap = new Dictionary<string, Guid>();
        public static UuidCache Create()
        {
            Func<string, Guid> getter = new Func<string, Guid>(pnr =>
            {
                if (!UuidMap.ContainsKey(pnr))
                    UuidMap[pnr] = Guid.NewGuid();
                return UuidMap[pnr];
            }
            );
            return new UuidCache()
            {
                GetUuidMethod = getter,
                GetUuidArrayMethod = arr => arr.Select(pnr => (Guid?)getter(pnr)).ToArray()
            };
        }
    }
}
