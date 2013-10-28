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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CPR_Business_Application_Demo.Business;

namespace CPR_Business_Application_Demo
{
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();

            AcceptButton = okButton;
            CancelButton = cancelButton;
        }


        #region Private Methods
        private void VerifyRegistrationAndApproval()
        {
            var adminController = new ApplicationsController(Properties.Settings.Default);
            var applicationRegistration = adminController.ApplicationIsRegistered(cprBrokerWebServiceUrlTextBox.Text);
            if (applicationRegistration != null)
            {
                appRegistrationLabel.Text = "Application is registered.\n";
                string approveLabelText = "";
                if (applicationRegistration.IsApproved)
                {
                    approveLabelText = "Application is approved";
                    Properties.Settings.Default.AppToken = applicationRegistration.Token;
                }
                else
                {
                    approveLabelText = "Application is NOT approved.\n"
                                 + "You need to manually approve it on: "
                                 + GetApplicationsUrl();
                }
                appRegistrationLabel.Text += approveLabelText
                                             + "\n"
                                             + "Application Token: " + Properties.Settings.Default.AppToken;
            }
        }

        private string GetPageBasicUrl()
        {
            var serviceBasicUrl = cprBrokerWebServiceUrlTextBox.Text;
            var pageBasicUrl = serviceBasicUrl.Replace("Services", "Pages");
            if (pageBasicUrl.IndexOfAny(new char[] { '/', '\\' }, pageBasicUrl.Length - 1) < 0)
                pageBasicUrl += "/";
            return pageBasicUrl;
        }

        private string GetWebLogUrl()
        {
            return GetPageBasicUrl() + "LogEntries.aspx";
        }

        private string GetApplicationsUrl()
        {
            return GetPageBasicUrl() + "Applications.aspx";
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.CPRBrokerWebServiceUrl = cprBrokerWebServiceUrlTextBox.Text;
            Properties.Settings.Default.AdminAppToken = adminAppTokenTextBox.Text;

            // Notifications
            if (notificationModeDisabledRadioButton.Checked)
                Properties.Settings.Default.NotificationMode = 0;
            else if (notificationModeCallbackWebServiceRadioButton.Checked)
            {
                Properties.Settings.Default.NotificationMode = 1;
                Properties.Settings.Default.NotificationCallbackWebServiceUrl =
                    notificationCallBackWebServiceUrlTextBox.Text;
            }
            else if (notificationModeFileShareRadioButton.Checked)
            {
                Properties.Settings.Default.NotificationMode = 2;
                Properties.Settings.Default.NotificationFileShare = notificationFileShareTextBox.Text;
            }


            Properties.Settings.Default.Save();
        }

        #endregion

        private bool TestBrokerUrl()
        {
            var adminController = new ApplicationsController(Properties.Settings.Default);
            bool testOk = adminController.TestWSConnection(cprBrokerWebServiceUrlTextBox.Text);
            if (!testOk)
            {
                MessageBox.Show(this, "Connection failed. The CPR Broker Web Service URL is incorrect or the service is unavailable", "Connection failure", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return testOk;
        }

        private bool TestEventUrl()
        {
            var subscriptionsController = new SubscriptionsController(Properties.Settings.Default);
            bool testOk = subscriptionsController.TestWSConnection(eventBrokerWebServiceUrlTextBox.Text);
            if (!testOk)
            {
                MessageBox.Show(this, "Connection failed. The Event Broker Web Service URL is incorrect or the service is unavailable", "Connection failure", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return testOk;
        }

        #region Events

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            switch (Properties.Settings.Default.NotificationMode)
            {
                case 0:
                    notificationModeDisabledRadioButton.Checked = true;
                    callbackWebServiceGroupBox.Enabled = false;
                    fileShareGroupBox.Enabled = false;
                    break;
                case 1:
                    notificationModeCallbackWebServiceRadioButton.Checked = true;
                    callbackWebServiceGroupBox.Enabled = true;
                    fileShareGroupBox.Enabled = false;
                    break;
                case 2:
                    notificationModeFileShareRadioButton.Checked = true;
                    callbackWebServiceGroupBox.Enabled = false;
                    fileShareGroupBox.Enabled = true;
                    break;
            }
        }

        private void OptionsForm_Shown(object sender, EventArgs e)
        {
            VerifyRegistrationAndApproval();
        }

        /// <summary>
        /// Verifies whether the url in in Web Service URL text box is pointing to a CPR Broker installation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void testConnectionButton_Click(object sender, EventArgs e)
        {
            CPRBrokerLogPage.Links.Clear();
            if (TestBrokerUrl() && TestEventUrl())
            {
                appRegistrationGroupBox.Enabled = true;
                MessageBox.Show(this, "Connection succeeded. The Web Service URLs are correct.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Properties.Settings.Default.EventBrokerWebServiceUrl = eventBrokerWebServiceUrlTextBox.Text;
                Properties.Settings.Default.CPRBrokerWebServiceUrl = cprBrokerWebServiceUrlTextBox.Text;
                CPRBrokerLogPage.Links.Add(0, CPRBrokerLogPage.Text.Length, GetWebLogUrl());

                VerifyRegistrationAndApproval();
            }
            else
            {
                appRegistrationGroupBox.Enabled = false;
            }

        }

        private void registerApplicationButton_Click(object sender, EventArgs e)
        {
            appRegistrationLabel.Text = "";
            var adminController = new ApplicationsController(Properties.Settings.Default);

            string appToken = adminController.RegisterApplication(cprBrokerWebServiceUrlTextBox.Text);

            if (appToken.Length > 0)
            {
                Properties.Settings.Default.AppToken = appToken;
                var labelText = "Application is registered.\n";
                var approveController = new ApplicationsController(Properties.Settings.Default);
                bool applicationIsApproved = approveController.ApproveApplication(cprBrokerWebServiceUrlTextBox.Text,
                                                                                  adminAppTokenTextBox.Text);
                if (applicationIsApproved)
                {
                    labelText += "Application is approved.\n";
                    appRegistrationLabel.Text = labelText
                                                + "Application Token: " + appToken;
                }
                else
                {
                    labelText += "Application is NOT approved.\n"
                                 + "You need to manually approve it on: "
                                 + GetApplicationsUrl();
                }
                appRegistrationLabel.Text = labelText;
            }
            else
            {
                appRegistrationLabel.Text = "Registration did not succeed. Please see the weblog for CPR Broker: "
                                             + GetWebLogUrl();
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void notificationModeCallbackWebServiceRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            callbackWebServiceGroupBox.Enabled = true;
            fileShareGroupBox.Enabled = false;
        }

        private void notificationModeFileShareRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            callbackWebServiceGroupBox.Enabled = false;
            fileShareGroupBox.Enabled = true;
        }

        private void notificationModeDisabledRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            callbackWebServiceGroupBox.Enabled = false;
            fileShareGroupBox.Enabled = false;
        }

        private void fileShareBrowseButton_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = true;

            dialog.ShowDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
                notificationFileShareTextBox.Text = dialog.SelectedPath;
        }

        private void CPRBrokerLogPage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var processInfo = new ProcessStartInfo(e.Link.LinkData.ToString());
            Process.Start(processInfo);
        }

        #endregion



    }
}
