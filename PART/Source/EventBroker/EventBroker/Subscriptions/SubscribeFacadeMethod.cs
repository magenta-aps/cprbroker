using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Engine;
using CprBroker.EventBroker.Data;
using CprBroker.EventBroker.Notifications;
using CprBroker.EventBroker.Subscriptions;

namespace CprBroker.EventBroker
{
    /// <summary>
    /// Facade method for Subscribe web method
    /// </summary>
    public class SubscribeFacadeMethod : GenericFacadeMethodInfo<ChangeSubscriptionType>
    {
        ChannelBaseType NotificationChannel;
        Guid[] PersonUuids;

        public SubscribeFacadeMethod(ChannelBaseType notificationChannel, Guid[] personUuids, string appToken, string userToken)
            : base(appToken, userToken)
        {
            NotificationChannel = notificationChannel;
            PersonUuids = personUuids;
        }

        public override void Initialize()
        {
            SubMethodInfos = new SubMethodInfo[]
            {
                new SubMethodInfo<ISubscriptionDataProvider,ChangeSubscriptionType>()
                {
                    FailIfNoDataProvider=true,
                    FailOnDefaultOutput=true,
                    LocalDataProviderOption= LocalDataProviderUsageOption.UseFirst,
                    Method= prov=>prov.Subscribe(NotificationChannel,PersonUuids),
                    UpdateMethod=null,
                }
            };
        }

        public override StandardReturType ValidateInput()
        {
            if (NotificationChannel == null)
            {
                return StandardReturType.NullInput("NotificationChannel");
            }

            var dbChannel = Data.Channel.FromXmlType(NotificationChannel);
            if (dbChannel == null)
            {
                return StandardReturType.UnknownObject("NotificationChannel");
            }
            
            var channel = Notifications.Channel.Create(dbChannel);
            if (channel == null || !channel.IsAlive())
            {
                return StandardReturType.Create(HttpErrorCode.BAD_CLIENT_REQUEST, "Unreachable channel");
            }
            
            return StandardReturType.OK();
        }

    }
}
