using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Schemas.Part.Events;


namespace CprBroker.Web.Services
{
    // NOTE: If you change the class name "Events" here, you must also update the reference to "Events" in Web.config.
    [ServiceBehavior(Namespace = "http://dk.itst")]
    public class Events : IEvents
    {
        public BasicOutputType<DataChangeEventInfo[]> DequeueDataChangeEvents(ApplicationHeader applicationHeader, int maxCount)
        {
            return Manager.Events.DequeueDataChangeEvents(applicationHeader.UserToken, applicationHeader.ApplicationToken, maxCount);
        }

        public BasicOutputType<PersonBirthdate[]> GetPersonBirthdates(ApplicationHeader applicationHeader, Guid? personUuidToStartAfter, int maxCount)
        {
            return Manager.Events.GetPersonBirthdates(applicationHeader.UserToken, applicationHeader.ApplicationToken, personUuidToStartAfter, maxCount);
        }
    }
}
