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

        public static StandardReturType ValidateChannel(ChannelBaseType notificationChannel)
        {
            if (notificationChannel == null)
            {
                return StandardReturType.NullInput("NotificationChannel");
            }

            if (notificationChannel is WebServiceChannelType)
            {
                if (string.IsNullOrEmpty((notificationChannel as WebServiceChannelType).WebServiceUrl))
                {
                    return StandardReturType.NullInput("WebServiceUrl");
                }
            }
            else if (notificationChannel is FileShareChannelType)
            {
                if (string.IsNullOrEmpty((notificationChannel as FileShareChannelType).Path))
                {
                    return StandardReturType.NullInput("Path");
                }
            }
            else
            {
                return StandardReturType.UnknownObject("Unknown channel type");
            }

            var dbChannel = Data.Channel.FromXmlType(notificationChannel);
            if (dbChannel == null)
            {
                return StandardReturType.UnknownObject("NotificationChannel");
            }

            var channel = Notifications.Channel.Create(dbChannel);
            try
            {
                if (channel == null || !channel.IsAlive())
                {
                    return StandardReturType.Create(HttpErrorCode.BAD_CLIENT_REQUEST, "Unreachable channel");
                }
            }
            catch (Exception ex)
            {
                Engine.Local.Admin.LogException(ex);

                return StandardReturType.Create(HttpErrorCode.BAD_CLIENT_REQUEST, "Unreachable channel");
            }
            return StandardReturType.OK();
        }

        public override StandardReturType ValidateInput()
        {
            var channelValidationResult = ValidateChannel(NotificationChannel);
            if (!StandardReturType.IsSucceeded(channelValidationResult))
            {
                return channelValidationResult;
            }            
            return StandardReturType.OK();
        }

    }
}
