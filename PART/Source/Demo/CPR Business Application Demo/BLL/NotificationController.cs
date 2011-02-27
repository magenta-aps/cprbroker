using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.ComponentModel;
using CPR_Business_Application_Demo.ApplicationsService;

namespace CPR_Business_Application_Demo.Business
{
    public class NotificationController
    {
        #region Methods
        public void HandleNotification(ApplicationSettingsBase settings, string pathToNotificationFile, INotificationListener listener)
        {
            Settings = settings;
            PathToNotificationFile = pathToNotificationFile;
            Listener = listener;

            // Setup a worker to process the notification in the background to avoid locking up the
            // application
            var notificationWorker = new BackgroundWorker();
            notificationWorker.DoWork += new DoWorkEventHandler(FetchNotifications);
            notificationWorker.RunWorkerAsync(this);
        }
        #endregion

        #region Private Methods


        /// <summary>
        /// Please see the design spec for a thorough description of the format of the notification file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FetchNotifications(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                var controller = e.Argument as NotificationController;
                XDocument notification = XDocument.Load(PathToNotificationFile);

                var applicationToken = from appToken in notification.Descendants("ApplicationToken")
                                       select appToken.ToString();

                if (applicationToken.Single() == Settings["AppToken"].ToString())
                {
                    var notificationPersons = from person in notification.Descendants("PersonNameStructure")
                            select new NotificationPerson()
                                       {
                                           FirstName = (string) person.Element("PersonGivenName"),
                                           MiddleName = (string) person.Element("PersonMiddleName"),
                                           LastName = (string) person.Element("PersonMiddleName"),
                                           Cpr = (string) person.Parent.Element("PersonCivilRegistrationIdentifier")
                                       };
                    Listener.ReportChangedPersonRegistrationIds(notificationPersons.ToList());
                }
            }
            catch
            { }
        }

        #endregion


        #region Private Fields
        private string PathToNotificationFile;
        private INotificationListener Listener;
        private ApplicationSettingsBase Settings;
        private readonly Dictionary<string, bool> SubscribedPersonRegistrationIds = new Dictionary<string,bool>();
        #endregion
    }

}
