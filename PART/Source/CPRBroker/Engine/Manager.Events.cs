using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Engine
{
    public partial class Manager
    {
        public class Events
        {
            public static Schemas.Part.Events.DataChangeEventInfo[] DequeueDataChangeEvents(string userToken, string appToken, int maxCount)
            {
                var methodInfo = new CprBroker.Engine.Events.DequeueDataChangeEventsFacadeMethod(maxCount, appToken, userToken, true);
                return GetMethodOutput<Schemas.Part.Events.DataChangeEventInfo[]>(methodInfo);
            }
        }
    }
}
