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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CPR_Business_Application_Demo.Business;

namespace CPR_Business_Application_Demo
{
    public partial class MainForm : Form, INotificationListener
    {
        public MainForm()
        {
            InitializeComponent();
            this.mainTabControl.TabPages.Remove(this.personTabPage);
            this.mainTabControl.TabPages.Remove(this.searchTabPage);
        }

        private void ConsoleWriteLine(string message)
        {
            notificationResultsConsoleTextBox.Text += message + "\n";
        }

        private bool CheckRegistration(bool showMessageBox)
        {
            try
            {
                var adminController = new ApplicationsController(Properties.Settings.Default);
                var applicationRegistration =
                    adminController.ApplicationIsRegistered(Properties.Settings.Default.CPRBrokerWebServiceUrl);
                if (applicationRegistration != null)
                {
                    mainTabControl.Enabled = true;
                    registeredStatusLabel.Text = "Application is registered";
                    if (showMessageBox)
                    {
                        MessageBox.Show(this,
                                        "The application is registered",
                                        "Success",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    }
                    StartWatcher();
                    return true;
                }
                else
                {
                    mainTabControl.Enabled = false;
                    registeredStatusLabel.Text = "Application is Not registered! \n" + Properties.Settings.Default +
                      "\nurl=" + Properties.Settings.Default.CPRBrokerWebServiceUrl;
                    if (showMessageBox)
                    {
                        MessageBox.Show(this,
                                        "The application is not registered or connection failed",
                                        "Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                    }
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private void OpenOptionsForm()
        {
            var optionsForm = new OptionsForm();
            optionsForm.ShowDialog();
            CheckRegistration(false);
        }

        #region Subscription watching
        private void StartWatcher()
        {
            if (Directory.Exists(Properties.Settings.Default.NotificationFileShare))
            {
                if (notificationWatcher == null)
                {
                    notificationWatcher = new FileSystemWatcher(Properties.Settings.Default.NotificationFileShare);
                    notificationWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite;
                    notificationWatcher.Filter = "*.*";

                    // Add event handlers.
                    notificationWatcher.Changed += new FileSystemEventHandler(OnChanged);
                    notificationWatcher.Created += new FileSystemEventHandler(OnChanged);
                    notificationWatcher.Deleted += new FileSystemEventHandler(OnChanged);

                    // Begin watching.
                    notificationWatcher.EnableRaisingEvents = true;
                }
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            var t = e.ChangeType;
            if (t == WatcherChangeTypes.Created || t == WatcherChangeTypes.Changed)
            {
                var notificationController = new NotificationController();
                notificationController.HandleNotification(Properties.Settings.Default, e.FullPath, this);
            }
        }

        private FileSystemWatcher notificationWatcher = null;

        #endregion


        #region INotificationListener Members

        private delegate void ReportChangedPersonRegistrationIdsDelegate(List<NotificationPerson> changedPersonRegistrationIds);

        public void ReportChangedPersonRegistrationIds(List<NotificationPerson> changedPersonRegistrationIds)
        {
            Invoke(new ReportChangedPersonRegistrationIdsDelegate(ReportChangedPersonRegistrationIdsGuiThread),
                   changedPersonRegistrationIds);
        }

        private void ReportChangedPersonRegistrationIdsGuiThread(List<NotificationPerson> changedPersonRegistrationIds)
        {
            if (notificationsForm == null)
            {
                notificationsForm = new NotificationsForm();
                notificationsForm.FormClosing += ((sender, e) => notificationsForm = null);
            }

            notificationsForm.Show(this);
            notificationsForm.AddNotification(changedPersonRegistrationIds);
        }

        #endregion

        #region Private Members

        private NotificationsForm notificationsForm = null;
        #endregion

        #region Events

        private void checkRegistrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckRegistration(true);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (!CheckRegistration(false))
                OpenOptionsForm();

        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenOptionsForm();
        }

        private void personQueryButton_Click(object sender, EventArgs e)
        {

        }

        private void subscribeCprButton_Click(object sender, EventArgs e)
        {
            ConsoleWriteLine("Subscribing.....");
            var t = Properties.Settings.Default.NotificationMode;
            var adminController = new SubscriptionsController(Properties.Settings.Default);

            string result = adminController.Subscribe(notificationPersonsTextBox.Lines);
            if (result.Length > 0)
            {
                ConsoleWriteLine("Suceeded!");
                ConsoleWriteLine("Subscription ID: " + result);
            }
            else
            {
                ConsoleWriteLine("FAILED!!!");
            }
        }

        private void GetServiceInfoButton_Click(object sender, EventArgs e)
        {
            var adminController = new ApplicationsController(Properties.Settings.Default);
            var capabilities = adminController.GetCapabillities();
            InfoText.Text = "";
            foreach (var capability in capabilities)
            {
                InfoText.Text += "Version: " + capability.Version + "\r\n"
                              + "Details: " + capability.Details + "\r\n"
                              + "Functions: " + String.Join("\r\n", capability.Functions.ToArray()) + "\r\n";
            }
        }

        private void BirthDaySubscriptionButton_Click(object sender, EventArgs e)
        {
            var adminController = new SubscriptionsController(Properties.Settings.Default);
            int? age = (int)AgeSpin.Value;
            if (IgnoreAgeCheckBox.Checked)
                age = null;
            var result = adminController.SubscribeBirthdate(age, (int)PriorDaysSpin.Value,
                                                             notificationPersonsTextBox.Lines);
            if (result.Length > 0)
            {
                ConsoleWriteLine("Suceeded!");
                ConsoleWriteLine("Subscription ID: " + result);
            }
            else
            {
                ConsoleWriteLine("FAILED!!!");
            }
        }


        private void ListActiveSubscriptionsButton_Click(object sender, EventArgs e)
        {
            var adminController = new SubscriptionsController(Properties.Settings.Default);
            var activeSubscriptions = adminController.GetActiveSubscriptions();
            Dictionary<string, Dictionary<string, bool>> subscriptionIds = new Dictionary<string, Dictionary<string, bool>>();

            foreach (var subscription in activeSubscriptions)
            {
                Dictionary<string, bool> personRegistrationIds = new Dictionary<string, bool>();
                foreach (var personRegistrationId in subscription.PersonUuids)
                {
                    if (!personRegistrationIds.ContainsKey(personRegistrationId))
                        personRegistrationIds.Add(personRegistrationId, true);
                }
                subscriptionIds.Add(subscription.SubscriptionId, personRegistrationIds);
            }

            notificationResultsConsoleTextBox.Text = "";
            foreach (var personRegistrationSet in subscriptionIds)
            {
                notificationResultsConsoleTextBox.Text += "ID: " + personRegistrationSet.Key + "\r\n";
                foreach (var id in personRegistrationSet.Value.Keys)
                {
                    notificationResultsConsoleTextBox.Text += id;
                }
            }
        }
        #endregion


        private void search_button_Click(object sender, EventArgs e)
        {
            PartAdapter adapter = new PartAdapter(Properties.Settings.Default.CPRBrokerWebServiceUrl);

            Guid personUuid = new Guid(searchuuid.Text);

            var search_results = adapter.Search(Properties.Settings.Default.AdminAppToken, personUuid.ToString());
            ConsoleWriteLine("  looking up uid:" + personUuid);
            search_result.Text = "  result for uid:" + personUuid + "  " + search_results.Length + " results :\r\n" + String.Join("\r\n", search_results);
        }


        private void cpr_button_Click_1(object sender, EventArgs e)
        {
            uuidTextBox.Text = "tom";

            string personCpr = cpr.Text;

            PartAdapter adapter = new PartAdapter(Properties.Settings.Default.CPRBrokerWebServiceUrl);

            ConsoleWriteLine("  looking up cpr:" + personCpr);
            Guid puid = adapter.GetUuid(Properties.Settings.Default.AdminAppToken, personCpr);
            uuidTextBox.Text = puid.ToString();
            resultXmlTextBox.Text = "  result for cpr:" + personCpr + " : " + puid + "\r\n";
        }

        private void readButton_Click(object sender, EventArgs e)
        {
            PartAdapter adapter = new PartAdapter(Properties.Settings.Default.CPRBrokerWebServiceUrl);

            Guid personUuid = new Guid(uuidTextBox.Text);
            var readResult = adapter.Read(Properties.Settings.Default.AdminAppToken, personUuid);
            ConsoleWriteLine("  looking up uid:" + personUuid);
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(PartService.LaesOutputType));
            StringWriter w = new StringWriter();
            ser.Serialize(w, readResult);            

            //gade.Text = personReg.AttributListe.RegisterOplysning[0]. CprBorger.FolkeregisterAdresse.DanskAdresse.AddressPostal.StreetName;

            resultXmlTextBox.Text = "  result for uid:" + personUuid + "    \r\n" + w.ToString();
            //    " attributes=" + personReg.AttributListe.Egenskab[0].NavnStruktur.PersonNameStructure.PersonGivenName;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label15_Click_1(object sender, EventArgs e)
        {

        }

        private void gade_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
