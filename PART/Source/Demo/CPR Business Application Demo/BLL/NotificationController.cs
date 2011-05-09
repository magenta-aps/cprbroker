/*
 * Copyright (c) 2011 Danish National IT and Tele agency / IT- og Telestyrelsen

* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:

* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.

* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

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
