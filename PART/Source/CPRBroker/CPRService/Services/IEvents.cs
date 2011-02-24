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
    // NOTE: If you change the interface name "IEvents" here, you must also update the reference to "IEvents" in Web.config.
    [ServiceContract]
    [XmlSerializerFormat]
    public interface IEvents
    {
        [OperationContract]
        BasicOutputType<DataChangeEventInfo[]> DequeueDataChangeEvents(ApplicationHeader applicationHeader, int maxCount);

        [OperationContract]
        BasicOutputType<PersonBirthdate[]> GetPersonBirthdates(ApplicationHeader applicationHeader, Guid? personUuidToStartAfter, int maxCount);
    }
}
