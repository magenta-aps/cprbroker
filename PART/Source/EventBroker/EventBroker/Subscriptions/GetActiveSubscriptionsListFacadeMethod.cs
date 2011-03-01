﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Engine;

namespace CprBroker.EventBroker.Subscriptions
{
    /// <summary>
    /// Facade method for GetActiveSubscriptionsList
    /// </summary>
    public class GetActiveSubscriptionsListFacadeMethod : GenericFacadeMethodInfo<SubscriptionType[]>
    {
        public GetActiveSubscriptionsListFacadeMethod(string appToken, string userToken)
            : base(appToken, userToken)
        {
        }

        public override void Initialize()
        {
            SubMethodInfos = new SubMethodInfo[]
            {
                new SubMethodInfo<ISubscriptionDataProvider,SubscriptionType[]>()
                {
                    FailIfNoDataProvider=true,
                    FailOnDefaultOutput=true,
                    LocalDataProviderOption= LocalDataProviderUsageOption.UseFirst,
                    Method= prov=>prov.GetActiveSubscriptionsList(),
                    UpdateMethod=null,
                }
            };
        }

        public override StandardReturType ValidateInput()
        {
            return StandardReturType.OK();
        }

    }
}
