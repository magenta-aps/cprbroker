using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Providers.DPR;

namespace CprBroker.Tests.DPR.CivilStatusTests
{
    class CivilStatusStub : CivilStatus
    {
        static Dictionary<decimal, Guid> UuidMap = new Dictionary<decimal, Guid>();
        public static Guid CprToUuid(decimal cpr)
        {
            if (!UuidMap.ContainsKey(cpr))
                UuidMap[cpr] = Guid.NewGuid();
            return UuidMap[cpr];
        }
    }
   
}
