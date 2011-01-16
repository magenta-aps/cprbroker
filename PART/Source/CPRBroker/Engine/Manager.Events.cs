using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part.Events;

namespace CprBroker.Engine
{
    public partial class Manager
    {
        public class Events
        {
            public static DataChangeEventInfo[] DequeueDataChangeEvents(string userToken, string appToken, int maxCount)
            {
                var methodInfo = new CprBroker.Engine.Events.DequeueDataChangeEventsFacadeMethod(maxCount, appToken, userToken, true);
                return GetMethodOutput<Schemas.Part.Events.DataChangeEventInfo[]>(methodInfo);
            }

            public static PersonBirthdate[] GetPersonBirthdates(string userToken, string appToken, Guid? personUuidToStartAfter, int maxCount)
            {
                var methodInfo = new CprBroker.Engine.Events.GetPersonBirthdatesFacadeMethodInfo(personUuidToStartAfter, maxCount, appToken, userToken, true);
                return GetMethodOutput<PersonBirthdate[]>(methodInfo);
            }
        }
    }
}
