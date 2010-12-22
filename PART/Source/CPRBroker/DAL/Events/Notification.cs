using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas;
using System.Data.Linq;

namespace CPRBroker.DAL.Events
{
    public partial class Notification
    {
        /// <summary>
        /// Sets the load options to load child object with a main Notification object
        /// </summary>
        /// <param name="loadOptions"></param>
        public static void SetLoadOptionsForChildren(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<Notification>((notif) => notif.NotificationPersons);
            loadOptions.LoadWith<Notification>((notif) => notif.BirthdateNotification);
            loadOptions.LoadWith<Notification>((notif) => notif.Subscription);
            loadOptions.LoadWith<NotificationPerson>((notifPerson) => notifPerson.BirthdateNotificationPerson);
            // TODO: remove this
            //loadOptions.LoadWith<NotificationPerson>((notifPerson) => notifPerson.Person);
        }

        /// <summary>
        /// Converts the object to an OIO object
        /// </summary>
        /// <returns></returns>
        public Schemas.BaseNotificationType ToOioNotification()
        {
            BaseNotificationType ret = null;
            if (this.Subscription.BirthdateSubscription != null)
            {
                BirthdateNotificationType oioBirthdateNotif = new BirthdateNotificationType();
                oioBirthdateNotif.BirthdateSubscription = this.Subscription.ToOioSubscription() as BirthdateSubscriptionType;
                oioBirthdateNotif.Persons.AddRange(
                    from np in this.NotificationPersons
                    select new BirthdateNotificationPersonType()
                    {
                        // TODO: Event format
                        //SimpleCPRPerson = np.Person.ToSimpleCPRPerson(),
                        Age = np.BirthdateNotificationPerson.Age
                    }
                    );
                ret = oioBirthdateNotif;
            }
            else if (this.Subscription.DataSubscription != null)
            {
                ChangeNotificationType oioChangeNotif = new ChangeNotificationType();
                oioChangeNotif.ChangeSubscription = this.Subscription.ToOioSubscription() as ChangeSubscriptionType;
                oioChangeNotif.Persons.AddRange(
                    from np in this.NotificationPersons
                    select new ChangeNotificationPersonType()
                    {
                        // TODO: Event format
                        //SimpleCPRPerson = np.Person.ToSimpleCPRPerson()
                    }
                    );
                ret = oioChangeNotif;
            }
            if (ret != null)
            {
                ret.ApplicationToken = this.Subscription.Application.Token;
                ret.NotificationDate = this.NotificationDate;
            }
            return ret;
        }
    }
}
