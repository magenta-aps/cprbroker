using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;

namespace CprBroker.Tests.Engine.Stubs
{
    public class GuidFacade : GenericFacadeMethodInfo<Guid[]>
    {
        public Guid[] InputGuids;

        private GuidFacade() { }
        public GuidFacade(uint count)
        {
            ApplicationToken = CprBroker.Utilities.Constants.BaseApplicationToken.ToString();

            InputGuids = new Guid[count];
            for (int i = 0; i < count; i++)
            {
                InputGuids[i] = Guid.NewGuid();
            }
        }

        public override void Initialize()
        {
            SubMethodInfos = InputGuids.Select(id => new GuidMethodInfo() { Input = id }).ToArray();
        }

        public override Guid[] Aggregate(object[] results)
        {
            return results.Select(r => (Guid)r).ToArray();
        }

    }
}
