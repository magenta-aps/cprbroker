using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Engine.Events
{
    /// <summary>
    /// Sub method for DequeueDataChangeEvent
    /// </summary>
    public class DequeueDataChangeEventSubMethodInfo : SubMethodInfo<IDataChangeEventManager, Schemas.Part.Events.DataChangeEventInfo[]>
    {
        public int MaxCount;

        public DequeueDataChangeEventSubMethodInfo(int maxCount)
        {
            this.MaxCount = maxCount;
        }

        public override CprBroker.Schemas.Part.Events.DataChangeEventInfo[] RunMainMethod(IDataChangeEventManager prov)
        {
            return prov.DequeueEvents(MaxCount);
        }

    }
}
