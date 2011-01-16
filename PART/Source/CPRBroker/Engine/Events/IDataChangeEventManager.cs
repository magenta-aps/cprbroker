using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Engine.Events
{
    public interface IDataChangeEventManager : IDataProvider
    {
        Schemas.Part.Events.DataChangeEventInfo[] DequeueEvents(int maxCount);
    }
}
