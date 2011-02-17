﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;
using CprBroker.Schemas.Part.Events;

namespace CprBroker.Engine
{
    public partial class Manager
    {
        public class Events
        {
            public static BasicOutputType<DataChangeEventInfo[]> DequeueDataChangeEvents(string userToken, string appToken, int maxCount)
            {
                var methodInfo = new CprBroker.Engine.Events.DequeueDataChangeEventsFacadeMethod(maxCount, appToken, userToken);
                return GetMethodOutput<BasicOutputType<DataChangeEventInfo[]>>(methodInfo);
            }

            public static BasicOutputType<PersonBirthdate[]> GetPersonBirthdates(string userToken, string appToken, Guid? personUuidToStartAfter, int maxCount)
            {
                var methodInfo = new CprBroker.Engine.Events.GetPersonBirthdatesFacadeMethodInfo(personUuidToStartAfter, maxCount, appToken, userToken);
                return GetMethodOutput<BasicOutputType<PersonBirthdate[]>>(methodInfo);
            }
        }
    }
}
