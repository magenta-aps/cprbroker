using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas;

namespace CPRBroker.Engine.Notifications
{
    /// <summary>
    /// This class contains extension methods that convert between types in Schemas project and the corresponding types in the NotificationService web reference
    /// The sole reason is to call WebServiceChannels with detailed information
    /// </summary>
    public static class Extensions
    {
        public static NotificationService.BaseNotificationType ToWsdl(this Schemas.BaseNotificationType oioNotif)
        {
            NotificationService.BaseNotificationType ret = null;
            if (oioNotif is BirthdateNotificationType)
            {
                ret = (oioNotif as BirthdateNotificationType).ToWsdl();
            }
            else if (oioNotif is ChangeNotificationType)
            {
                ret = (oioNotif as ChangeNotificationType).ToWsdl();
            }
            return ret;
        }

        public static NotificationService.BirthdateNotificationType ToWsdl(this BirthdateNotificationType oioBithdateNotif)
        {
            return new CPRBroker.Engine.NotificationService.BirthdateNotificationType()
            {
                ApplicationToken = oioBithdateNotif.ApplicationToken,
                BirthdateSubscription = oioBithdateNotif.BirthdateSubscription.ToWsdl(),
                NotificationDate = oioBithdateNotif.NotificationDate,
                Persons = (from p in oioBithdateNotif.Persons.ToArray() select p.ToWsdl()).ToArray(),
            };
        }

        public static NotificationService.BirthdateSubscriptionType ToWsdl(this BirthdateSubscriptionType birthdateSubscription)
        {
            return new CPRBroker.Engine.NotificationService.BirthdateSubscriptionType()
            {
                AgeYears = birthdateSubscription.AgeYears,
                ApplicationToken = birthdateSubscription.ApplicationToken,
                ForAllPersons = birthdateSubscription.ForAllPersons,
                NotificationChannel = birthdateSubscription.NotificationChannel.ToWsdl(),
                PersonUuids = birthdateSubscription.PersonUuids.ToArray(),
                PriorDays = birthdateSubscription.PriorDays,
                SubscriptionId = birthdateSubscription.SubscriptionId,
            };
        }

        public static NotificationService.BirthdateNotificationPersonType ToWsdl(this BirthdateNotificationPersonType oioPerson)
        {
            return new CPRBroker.Engine.NotificationService.BirthdateNotificationPersonType()
            {
                Age = oioPerson.Age,
                SimpleCPRPerson = oioPerson.SimpleCPRPerson.ToWsdl(),
            };
        }

        public static NotificationService.SimpleCPRPersonType ToWsdl(this SimpleCPRPersonType oioPerson)
        {
            return new CPRBroker.Engine.NotificationService.SimpleCPRPersonType()
            {
                PersonCivilRegistrationIdentifier = oioPerson.PersonCivilRegistrationIdentifier,
                PersonNameStructure = oioPerson.PersonNameStructure.ToWsdl(),
            };
        }

        public static NotificationService.PersonNameStructureType ToWsdl(this PersonNameStructureType oioName)
        {
            return new CPRBroker.Engine.NotificationService.PersonNameStructureType()
            {
                PersonGivenName = oioName.PersonGivenName,
                PersonMiddleName = oioName.PersonMiddleName,
                PersonSurnameName = oioName.PersonSurnameName,
            };
        }

        public static NotificationService.ChannelBaseType ToWsdl(this ChannelBaseType oioChannel)
        {
            if (oioChannel is WebServiceChannelType)
            {
                return (oioChannel as WebServiceChannelType).ToWsdl();
            }
            else if (oioChannel is FileShareChannelType)
            {
                return (oioChannel as FileShareChannelType).ToWsdl();
            }
            else
            {
                return null;
            }
        }

        public static NotificationService.WebServiceChannelType ToWsdl(this WebServiceChannelType oioChannel)
        {
            return new CPRBroker.Engine.NotificationService.WebServiceChannelType()
            {
                WebServiceUrl = oioChannel.WebServiceUrl,
            };
        }

        public static NotificationService.FileShareChannelType ToWsdl(this FileShareChannelType oioChannel)
        {
            return new CPRBroker.Engine.NotificationService.FileShareChannelType()
            {
                Path = oioChannel.Path,
            };
        }

        public static NotificationService.ChangeNotificationType ToWsdl(this ChangeNotificationType oioChangeNotif)
        {
            return new CPRBroker.Engine.NotificationService.ChangeNotificationType()
            {
                ApplicationToken = oioChangeNotif.ApplicationToken,
                ChangeSubscription = oioChangeNotif.ChangeSubscription.ToWsdl(),
                NotificationDate = oioChangeNotif.NotificationDate,
                Persons = (from p in oioChangeNotif.Persons.AsQueryable() select p.ToWsdl()).ToArray(),
            };
        }

        public static NotificationService.ChangeSubscriptionType ToWsdl(this ChangeSubscriptionType oioChangeSubscription)
        {
            return new CPRBroker.Engine.NotificationService.ChangeSubscriptionType()
            {
                ApplicationToken = oioChangeSubscription.ApplicationToken,
                ForAllPersons = oioChangeSubscription.ForAllPersons,
                NotificationChannel = oioChangeSubscription.NotificationChannel.ToWsdl(),
                PersonUuids = oioChangeSubscription.PersonUuids.ToArray(),
                SubscriptionId = oioChangeSubscription.SubscriptionId
            };
        }

        public static NotificationService.ChangeNotificationPersonType ToWsdl(this ChangeNotificationPersonType oioChangeNotificationPerson)
        {
            return new CPRBroker.Engine.NotificationService.ChangeNotificationPersonType()
            {
                SimpleCPRPerson = oioChangeNotificationPerson.SimpleCPRPerson.ToWsdl(),
            };
        }


    }
}
