using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.EventBroker.Subscriptions;

namespace CprBroker.EventBroker.Web.Services
{
    // NOTE: If you change the class name "Subscriptions" here, you must also update the reference to "Subscriptions" in Web.config.
    [ServiceBehavior(Namespace = "http://dk.itst")]
    public class Subscriptions : ISubscriptions
    {
        public BasicOutputType<ChangeSubscriptionType> Subscribe(ApplicationHeader applicationHeader, ChannelBaseType NotificationChannel, Guid[] personUuids)
        {
            return Manager.Subscriptions.Subscribe(applicationHeader.UserToken, applicationHeader.ApplicationToken, NotificationChannel, personUuids);
        }

        public BasicOutputType<bool> Unsubscribe(ApplicationHeader applicationHeader, Guid SubscriptionId)
        {
            return Manager.Subscriptions.Unsubscribe(applicationHeader.UserToken, applicationHeader.ApplicationToken, SubscriptionId);
        }

        public BasicOutputType<BirthdateSubscriptionType> SubscribeOnBirthdate(ApplicationHeader applicationHeader, ChannelBaseType NotificationChannel, Nullable<int> Years, int PriorDays, Guid[] PersonCivilRegistrationIdentifiers)
        {
            return Manager.Subscriptions.SubscribeOnBirthdate(applicationHeader.UserToken, applicationHeader.ApplicationToken, NotificationChannel, Years, PriorDays, PersonCivilRegistrationIdentifiers);
        }

        public BasicOutputType<bool> RemoveBirthDateSubscription(ApplicationHeader applicationHeader, Guid SubscriptionId)
        {
            return Manager.Subscriptions.RemoveBirthDateSubscription(applicationHeader.UserToken, applicationHeader.ApplicationToken, SubscriptionId);
        }

        public BasicOutputType<SubscriptionType[]> GetActiveSubscriptionsList(ApplicationHeader applicationHeader)
        {
            return Manager.Subscriptions.GetActiveSubscriptionsList(applicationHeader.UserToken, applicationHeader.ApplicationToken);
        }
    }
}
