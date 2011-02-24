using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPR_Business_Application_Demo.SubscriptionsService;

namespace CPR_Business_Application_Demo.Adapters
{
    // The adapter is a feeble attempt at disconnecting the web service and the application
    // Not a very successful one I admit
    public class SubscriptionAdapter
    {
        #region Construction
        public SubscriptionAdapter(string subscriptionsWsUrl)
        {
            // Make sure the provided URL is pointing to the administration web service
            if (!subscriptionsWsUrl.EndsWith("/"))
            {
                if (!subscriptionsWsUrl.EndsWith("Subscriptions.svc"))
                    subscriptionsWsUrl += "/Subscriptions.svc";
            }
            else
            {
                subscriptionsWsUrl += "Subscriptions.svc";
            }

            subscriptionsHandler = new SubscriptionsClient("WSHttpBinding_ISubscriptions", subscriptionsWsUrl);

            // Set the timeout to avoid hanging the application for too long when wrong urls were entered
            subscriptionsHandler.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 5);

        }
        #endregion

        #region Subscription
        private Guid[] GetUuids(ApplicationHeader applicationHeader, string [] cprNumbers)
        {
            PartAdapter adpt = new PartAdapter(Properties.Settings.Default.CPRBrokerWebServiceUrl);
            return Array.ConvertAll<string, Guid>(cprNumbers, cpr => adpt.GetUuid(applicationHeader.ApplicationToken, cpr));
        }

        public BasicOutputTypeOfChangeSubscriptionType Subscribe(ApplicationHeader applicationHeader, ChannelBaseType notificationChannel, string[] personCivilRegistrationIdentifiers)
        {
            return subscriptionsHandler.Subscribe(applicationHeader, notificationChannel, GetUuids(applicationHeader,personCivilRegistrationIdentifiers));
        }

        public BasicOutputTypeOfBirthdateSubscriptionType SubscribeOnBirthdate(ApplicationHeader applicationHeader, ChannelBaseType notificationChannel,
                                                              int? age, int priorDays,
                                                              string[] personCivilRegistrationIdentifiers)
        {
            return subscriptionsHandler.SubscribeOnBirthdate(applicationHeader, notificationChannel, age, priorDays, GetUuids(applicationHeader, personCivilRegistrationIdentifiers));
        }

        public BasicOutputTypeOfArrayOfSubscriptionType GetActiveSubscriptions(ApplicationHeader applicationHeader)
        {
            return subscriptionsHandler.GetActiveSubscriptionsList(applicationHeader);
        }
        #endregion

        #region Private Fields
        private readonly SubscriptionsService.SubscriptionsClient subscriptionsHandler;
        #endregion

    }
}
