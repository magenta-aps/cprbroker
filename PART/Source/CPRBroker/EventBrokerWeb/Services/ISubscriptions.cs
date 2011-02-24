using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.EventBroker.Web.Services
{
    // NOTE: If you change the interface name "ISubscriptions" here, you must also update the reference to "ISubscriptions" in Web.config.
    [ServiceContract(Namespace = "http://dk.itst")]
    [XmlSerializerFormat]
    public interface ISubscriptions
    {
        [OperationContract]
        BasicOutputType<ChangeSubscriptionType> Subscribe(ApplicationHeader applicationHeader, ChannelBaseType NotificationChannel, Guid[] personUuids);

        [OperationContract]
        BasicOutputType<bool> Unsubscribe(ApplicationHeader applicationHeader, Guid SubscriptionId);

        [OperationContract]
        BasicOutputType<BirthdateSubscriptionType> SubscribeOnBirthdate(ApplicationHeader applicationHeader, ChannelBaseType NotificationChannel, Nullable<int> Years, int PriorDays, Guid[] PersonCivilRegistrationIdentifiers);

        [OperationContract]
        BasicOutputType<bool> RemoveBirthDateSubscription(ApplicationHeader applicationHeader, Guid SubscriptionId);

        [OperationContract]
        BasicOutputType<SubscriptionType[]> GetActiveSubscriptionsList(ApplicationHeader applicationHeader);
    }
}
