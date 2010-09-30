using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas;

namespace CPRBroker.DAL
{
    public partial class Subscription
    {
        /// <summary>
        /// Sets the load options to load the child entities with a root Subscription object
        /// </summary>
        /// <param name="loadOptions"></param>
        public static void SetLoadOptionsForChildren(System.Data.Linq.DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<Subscription>((Subscription sub) => sub.DataSubscription);
            loadOptions.LoadWith<Subscription>((Subscription sub) => sub.BirthdateSubscription);
            loadOptions.LoadWith<Subscription>((Subscription sub) => sub.Channels);
            loadOptions.LoadWith<Channel>((Channel chnl) => chnl.GpacChannel);
            loadOptions.LoadWith<Subscription>((Subscription sub) => sub.Application);
            loadOptions.LoadWith<Subscription>((Subscription sub) => sub.SubscriptionPersons);
            loadOptions.LoadWith<SubscriptionPerson>((SubscriptionPerson subPerson) => subPerson.Person);
        }
        public Schemas.SubscriptionType ToOioSubscription()
        {
            Schemas.SubscriptionType ret = null;
            if (this.DataSubscription != null)
            {
                ChangeSubscriptionType dataSubscription = new ChangeSubscriptionType();
                ret = dataSubscription;
            }
            else // birthdate subscription
            {
                BirthdateSubscriptionType birthdateSubscription = new BirthdateSubscriptionType();
                birthdateSubscription.AgeYears = this.BirthdateSubscription.AgeYears;
                birthdateSubscription.PriorDays = this.BirthdateSubscription.PriorDays;
                ret = birthdateSubscription;
            }
            ret.SubscriptionId = this.SubscriptionId.ToString();
            ret.ApplicationToken = Application.Token;
            ret.ForAllPersons = this.IsForAllPersons;

            Channel channel = this.Channels.Single();
            switch ((ChannelType.ChannelTypes)channel.ChannelTypeId)
            {
                case ChannelType.ChannelTypes.WebService:
                    Schemas.WebServiceChannelType webServiceChannel = new WebServiceChannelType();
                    webServiceChannel.WebServiceUrl = channel.Url;
                    ret.NotificationChannel = webServiceChannel;
                    break;
                case ChannelType.ChannelTypes.FileShare:
                    Schemas.FileShareChannelType fileShareChannel = new FileShareChannelType();
                    fileShareChannel.Path = channel.Url;
                    ret.NotificationChannel = fileShareChannel;
                    break;
                case ChannelType.ChannelTypes.GPAC:
                    Schemas.GPACChannelType gpacChannel = new GPACChannelType();
                    gpacChannel.ServiceUrl = channel.Url;
                    gpacChannel.SourceUri = channel.GpacChannel.SourceUri;
                    gpacChannel.ObjectType = channel.GpacChannel.ObjectType;
                    gpacChannel.NotifyType = channel.GpacChannel.NotifyType;
                    ret.NotificationChannel = gpacChannel;
                    break;
            }

            ret.PersonCivilRegistrationIdentifiers.AddRange(
                from subPerson in this.SubscriptionPersons
                select subPerson.Person.PersonNumber
                );
            return ret;
        }
    }
}
