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
    /// Facade method for SubscribeOnBirthdate web method
    /// </summary>
    public class SubscribeOnBirthdateFacadeMethod : GenericFacadeMethodInfo<BirthdateSubscriptionType>
    {
        ChannelBaseType NotificationChannel;
        Guid[] PersonUuids;
        Nullable<int> Years;
        int PriorDays;

        public SubscribeOnBirthdateFacadeMethod(ChannelBaseType notificationChannel, int? years, int priordays, Guid[] personUuids, string appToken, string userToken)
            : base(appToken, userToken)
        {
            NotificationChannel = notificationChannel;
            PersonUuids = personUuids;
            Years = years;
            PriorDays = priordays;
        }

        public override void Initialize()
        {
            SubMethodInfos = new SubMethodInfo[]
            {
                new SubMethodInfo<ISubscriptionDataProvider,BirthdateSubscriptionType>()
                {
                    FailIfNoDataProvider=true,
                    FailOnDefaultOutput=true,
                    LocalDataProviderOption= LocalDataProviderUsageOption.UseFirst,
                    Method= prov=>prov.SubscribeOnBirthdate(NotificationChannel,Years,PriorDays,PersonUuids),
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

            if (Years.HasValue)
            {
                if (Years.Value < 0 || Years.Value > 200)
                {
                    return StandardReturType.ValueOutOfRange("Years", Years.Value);
                }
            }

            if (PriorDays < 0 || PriorDays > 365)
            {
                return StandardReturType.ValueOutOfRange("PriorDays", Years.Value);
            }

            return StandardReturType.OK();
        }

    }
}
