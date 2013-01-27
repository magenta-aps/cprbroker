using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Engine;

namespace CprBroker.Tests.Engine.Stubs
{
    public class GuidMethodInfo : SubMethodInfo<GuidDataProvider, Guid>
    {
        public Guid Input;
        public GuidMethodInfo()
        {
            this.LocalDataProviderOption = SourceUsageOrder.LocalThenExternal;
        }

        public override Guid RunMainMethod(GuidDataProvider prov)
        {
            System.Threading.Thread.Sleep(50);
            return prov.AAAA(Input);
        }
    }
}
