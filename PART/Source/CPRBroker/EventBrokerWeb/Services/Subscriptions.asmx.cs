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
using CprBroker.EventBroker;

namespace CprBroker.EventBroker.Web.Services
{
    /// <summary>
    /// Contains web methods related to administration of the system
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/", Name = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Service, Description = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Service)]
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
        public ChangeSubscriptionType Subscribe(ChannelBaseType NotificationChannel, Guid[] PersonCivilRegistrationIdentifiers)
        {
            return Manager.Subscriptions.Subscribe(applicationHeader.UserToken, applicationHeader.ApplicationToken, NotificationChannel, PersonCivilRegistrationIdentifiers);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Methods.Unsubscribe, Description = CprBroker.Schemas.Part.ServiceDescription.Subscriptions.Unsubscribe)]
        public bool Unsubscribe(Guid SubscriptionId)
        {
            return Manager.Subscriptions.Unsubscribe(applicationHeader.UserToken, applicationHeader.ApplicationToken, SubscriptionId);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Methods.SubscribeOnBirthdate, Description = CprBroker.Schemas.Part.ServiceDescription.Subscriptions.SubscribeOnBirthdate)]
        public BirthdateSubscriptionType SubscribeOnBirthdate(ChannelBaseType NotificationChannel, Nullable<int> Years, int PriorDays, Guid[] PersonCivilRegistrationIdentifiers)
        {
            return Manager.Subscriptions.SubscribeOnBirthdate(applicationHeader.UserToken, applicationHeader.ApplicationToken, NotificationChannel, Years, PriorDays, PersonCivilRegistrationIdentifiers);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Methods.RemoveBirthDateSubscription, Description = CprBroker.Schemas.Part.ServiceDescription.Subscriptions.RemoveBirthDateSubscriptions)]
        public bool RemoveBirthDateSubscription(Guid SubscriptionId)
        {
            return Manager.Subscriptions.RemoveBirthDateSubscription(applicationHeader.UserToken, applicationHeader.ApplicationToken, SubscriptionId);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Methods.GetActiveSubscriptionsList, Description = CprBroker.Schemas.Part.ServiceDescription.Subscriptions.GetActiveSubscriptionsList)]
        public SubscriptionType[] GetActiveSubscriptionsList()
        {
            return Manager.Subscriptions.GetActiveSubscriptionsList(applicationHeader.UserToken, applicationHeader.ApplicationToken);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Methods.GetLatestNotification, Description = CprBroker.Schemas.Part.ServiceDescription.Subscriptions.GetLatestNotification)]
        public BaseNotificationType GetLatestNotification(Guid SubscriptionId)
        {
            return Manager.Subscriptions.GetLatestNotification(applicationHeader.UserToken, applicationHeader.ApplicationToken, SubscriptionId);
        }

        
    }
}
