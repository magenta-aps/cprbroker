/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

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
