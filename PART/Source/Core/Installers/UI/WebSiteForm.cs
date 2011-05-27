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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration.Install;
using System.DirectoryServices;

namespace CprBroker.Installers
{
    /// <summary>
    /// Allows the user to select where to install a website
    /// </summary>
    public partial class WebSiteForm : CprBroker.Installers.BaseForm
    {
        public WebInstallationInfo InstallationInfo;

        public WebSiteForm()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            PopulateInstallationInfo();
            CloseAsOK();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void TargetWebSiteForm_Load(object sender, EventArgs e)
        {
            // Default site name
            if (InstallationInfo is VirtualDirectoryInstallationInfo)
            {
                virtualDirectoryNameTextBox.Text = (InstallationInfo as VirtualDirectoryInstallationInfo).VirtualDirectoryName;
            }
            newWebSiteNameTextBox.Text = InstallationInfo.WebsiteName;

            // enumerate web sites
            DirectoryEntry w3svc = new DirectoryEntry(WebInstallationInfo.ServerRoot);
            List<DirectoryEntry> websites = new List<DirectoryEntry>();
            foreach (DirectoryEntry de in w3svc.Children)
            {
                if (de.SchemaClassName == "IIsWebServer")
                {
                    websites.Add(de);
                }
            }

            var sitesData = (from site in websites
                             select new { Name = site.Properties["ServerComment"].Value, Path = site.Path + "/Root" }).ToArray();

            sitesComboBox.DisplayMember = "Name";
            sitesComboBox.ValueMember = "Path";
            sitesComboBox.DataSource = sitesData;

            websitePanel.Enabled = Convert.ToInt32(w3svc.Properties["MaxConnections"].Value) == 0;
        }

        private void radio_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton thisRadioButton = sender as RadioButton;
            if (thisRadioButton.Checked)
            {
                RadioButton otherRadioButton = (thisRadioButton == virtualDirectoryRadio) ? newWebSiteRadio : virtualDirectoryRadio;
                otherRadioButton.Checked = false;

                virtualDirectoryGroupBox.Enabled = virtualDirectoryRadio.Checked;
                newWebSiteGroupBox.Enabled = newWebSiteRadio.Checked;
            }
        }

        void PopulateInstallationInfo()
        {
            if (newWebSiteRadio.Checked)
            {
                InstallationInfo = new WebsiteInstallationInfo()
                {
                    WebsiteName = newWebSiteNameTextBox.Text,
                };
            }
            else
            {
                InstallationInfo = new VirtualDirectoryInstallationInfo()
                {
                    WebsiteName = sitesComboBox.Text,
                    VirtualDirectoryName = virtualDirectoryNameTextBox.Text,
                };
            }

        }

        protected override bool ValidateContents()
        {
            if (!base.ValidateContents())
                return false;
            PopulateInstallationInfo();
            if (InstallationInfo.TargetEntryExists)
            {
                string message = InstallationInfo.CreateAsWebsite ? Messages.WebsiteExists : Messages.WebAppExists;
                DialogResult result = MessageBox.Show(message, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                    return false;
            }
            return true;
        }
    }
}
