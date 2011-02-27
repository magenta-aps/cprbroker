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

namespace CprBroker.SetupCpr
{
    public partial class WebSiteForm : CprBroker.SetupDatabase.BaseForm
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
            InstallationInfo.CreateAsWebsite = newWebSiteRadio.Checked;
            InstallationInfo.WebsitePath = newWebSiteRadio.Checked ? "" : sitesComboBox.SelectedValue.ToString();
            InstallationInfo.WebsiteName = newWebSiteRadio.Checked ? newWebSiteNameTextBox.Text : sitesComboBox.SelectedText;
            InstallationInfo.VirtualDirectoryName = virtualDirectoryRadio.Checked ? virtualDirectoryNameTextBox.Text : "";
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
