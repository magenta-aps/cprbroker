using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using CPRBroker.Schemas;

namespace CPRNotificationServiceDemo
{
    public class NotificationHandler
    {
        public static string RetrieveNotifications()
        {
            string notifications;
            var filename = HttpContext.Current.Server.MapPath("App_Data/notifications.txt");
            lock (Locker)
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    notifications = sr.ReadToEnd();
                }
            }
            return notifications;
        }

        public static void HandleNotification(BaseNotificationType notification)
        {
            if (notification as BirthdateNotificationType != null)
            {
                HandleBirthdateNotification(notification as BirthdateNotificationType);
                return;
            }

            if (notification as ChangeNotificationType != null)
            {
                HandleChangeNotification(notification as ChangeNotificationType);
                return;
            }
        }

        private static void HandleBirthdateNotification(BirthdateNotificationType notification)
        {
            var subscriptionText = new StringBuilder();
            subscriptionText.AppendLine("Notification date: " + notification.NotificationDate.ToString());
            subscriptionText.AppendLine("Subscription age: " + notification.BirthdateSubscription.AgeYears.ToString());
            subscriptionText.AppendLine("Prior days: " + notification.BirthdateSubscription.PriorDays.ToString());
            foreach (var person in notification.Persons)
            {
                var personName = person.SimpleCPRPerson.PersonNameStructure.PersonGivenName
                                 + (person.SimpleCPRPerson.PersonNameStructure.PersonMiddleName.Length > 0
                                     ? " " + person.SimpleCPRPerson.PersonNameStructure.PersonMiddleName + " "
                                     : " ")
                                 + person.SimpleCPRPerson.PersonNameStructure.PersonSurnameName;

                subscriptionText.AppendLine(personName + ":");
                subscriptionText.AppendFormat("Age {0}\r\n", person.Age);
                subscriptionText.AppendFormat("Cpr {0}\r\n", person.SimpleCPRPerson.PersonCivilRegistrationIdentifier);
            }
            AddTextToSubscriptions(subscriptionText.ToString());
        }

        private static void HandleChangeNotification(ChangeNotificationType notification)
        {
            var subscriptionText = new StringBuilder();
            subscriptionText.AppendLine("Notification date: " + notification.NotificationDate.ToString());
            foreach (var person in notification.Persons)
            {
                var personName = string.Format("{0} {1} {2} {3}",
                    person.SimpleCPRPerson.PersonCivilRegistrationIdentifier,
                    person.SimpleCPRPerson.PersonNameStructure.PersonGivenName,
                    person.SimpleCPRPerson.PersonNameStructure.PersonMiddleName,
                    person.SimpleCPRPerson.PersonNameStructure.PersonSurnameName
                    );

                subscriptionText.AppendLine(personName + ":");
                subscriptionText.AppendFormat("Cpr {0}\r\n", person.SimpleCPRPerson.PersonCivilRegistrationIdentifier);
            }
            AddTextToSubscriptions(subscriptionText.ToString());
        }


        private static void AddTextToSubscriptions(string text)
        {
            lock (Locker)
            {
                var filename = HttpContext.Current.Server.MapPath("App_Data/notifications.txt");
                using (var file = File.AppendText(filename))
                {
                    file.Write(text);
                    file.Close();
                }
            }
        }



        private static readonly object Locker = new object();
    }
}
