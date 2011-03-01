using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.EventBroker;
using CprBroker.EventBroker.Data;
using CprBroker.Engine;

namespace CprBroker.EventBroker.Subscriptions
{
    /// <summary>
    /// Facade method for Unsubscribe & RemoveBirthdateSubscription web methods
    /// </summary>
    public class UnsubscribeFacadeMethod : GenericFacadeMethodInfo<bool>
    {
        Guid SubscriptionId;
        Data.SubscriptionType.SubscriptionTypes SubscriptionType;

        public UnsubscribeFacadeMethod(Guid subscriptionId, Data.SubscriptionType.SubscriptionTypes subscriptionType, string appToken, string userToken)
            : base(appToken, userToken)
        {
            SubscriptionId = subscriptionId;
            SubscriptionType = subscriptionType;
        }

        public override void Initialize()
        {
            SubMethodInfos = new SubMethodInfo[]
            {
                new SubMethodInfo<ISubscriptionDataProvider,bool>()
                {
                    FailIfNoDataProvider=true,
                    FailOnDefaultOutput=true,
                    LocalDataProviderOption= LocalDataProviderUsageOption.UseFirst,
                    Method= prov=>
                        SubscriptionType== CprBroker.EventBroker.Data.SubscriptionType.SubscriptionTypes.Birthdate?
                            prov.RemoveBirthDateSubscription(SubscriptionId)
                            : prov.Unsubscribe(SubscriptionId),
                    UpdateMethod=null,
                }
            };
        }

        public override StandardReturType ValidateInput()
        {
            if (SubscriptionId == Guid.Empty)
            {
                return StandardReturType.NullInput();
            }

            using (var dataContext = new Data.EventBrokerDataContext())
            {
                var subscription = (from sub in dataContext.Subscriptions
                                    where sub.SubscriptionId == SubscriptionId && sub.SubscriptionTypeId == (int)this.SubscriptionType
                                    select sub
                                    ).SingleOrDefault();

                if (subscription == null)
                {
                    return StandardReturType.InvalidUuid(SubscriptionId.ToString());
                }
            }
            return StandardReturType.OK();
        }
       
    }
}
