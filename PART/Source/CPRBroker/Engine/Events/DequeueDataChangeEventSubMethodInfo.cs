using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Engine.Events
{
    public class DequeueDataChangeEventSubMethodInfo : SubMethodInfo<IDataChangeEventManager, Schemas.Part.Events.DataChangeEventInfo>
    {
        public int MaxCount;

        public DequeueDataChangeEventSubMethodInfo(int maxCount)
        {
            this.MaxCount = maxCount;
        }


    }
}
