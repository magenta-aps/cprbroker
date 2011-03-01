using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.EventBroker.Subscriptions;

namespace CprBroker.EventBroker.Web.Services
{
    /// <summary>
    /// Contains web methods related to administration of the system
    /// </summary>
    [WebService(Namespace = CprBroker.Schemas.Part.ServiceNames.Namespace, Name = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Service, Description = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Service)]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class Subscriptions : GKApp.WS.wsBaseV2
    {
        public ApplicationHeader applicationHeader;
        private const string ApplicationHeaderName = "applicationHeader";

        public Subscriptions()
        {
            BaseInit();
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Methods.Subscribe, Description = CprBroker.Schemas.Part.ServiceDescription.Subscriptions.Subscribe)]
        public BasicOutputType<ChangeSubscriptionType> Subscribe(ChannelBaseType NotificationChannel, Guid[] personUuids)
        {
            return SubscriptionManager.Subscribe(applicationHeader.UserToken, applicationHeader.ApplicationToken, NotificationChannel, personUuids);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Methods.Unsubscribe, Description = CprBroker.Schemas.Part.ServiceDescription.Subscriptions.Unsubscribe)]
        public BasicOutputType<bool> Unsubscribe(Guid SubscriptionId)
        {
            return SubscriptionManager.Unsubscribe(applicationHeader.UserToken, applicationHeader.ApplicationToken, SubscriptionId);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Methods.SubscribeOnBirthdate, Description = CprBroker.Schemas.Part.ServiceDescription.Subscriptions.SubscribeOnBirthdate)]
        public BasicOutputType<BirthdateSubscriptionType> SubscribeOnBirthdate(ChannelBaseType NotificationChannel, Nullable<int> Years, int PriorDays, Guid[] PersonCivilRegistrationIdentifiers)
        {
            return SubscriptionManager.SubscribeOnBirthdate(applicationHeader.UserToken, applicationHeader.ApplicationToken, NotificationChannel, Years, PriorDays, PersonCivilRegistrationIdentifiers);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Methods.RemoveBirthDateSubscription, Description = CprBroker.Schemas.Part.ServiceDescription.Subscriptions.RemoveBirthDateSubscriptions)]
        public BasicOutputType<bool> RemoveBirthDateSubscription(Guid SubscriptionId)
        {
            return SubscriptionManager.RemoveBirthDateSubscription(applicationHeader.UserToken, applicationHeader.ApplicationToken, SubscriptionId);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Methods.GetActiveSubscriptionsList, Description = CprBroker.Schemas.Part.ServiceDescription.Subscriptions.GetActiveSubscriptionsList)]
        public BasicOutputType<SubscriptionType[]> GetActiveSubscriptionsList()
        {
            return SubscriptionManager.GetActiveSubscriptionsList(applicationHeader.UserToken, applicationHeader.ApplicationToken);
        }

    }
}
