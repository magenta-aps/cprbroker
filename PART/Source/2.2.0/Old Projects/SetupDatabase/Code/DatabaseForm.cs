using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CPRBroker.SetupDatabase
{
    /// <summary>
    /// Gets the system's database information from the user
    /// </summary>
    public partial class DatabaseForm : BaseForm
    {
        public DatabaseForm()
        {
            InitializeComponent();
            SetupInfo = new SetupInfo();
        }
        public SetupInfo SetupInfo { get; set; }

        private void DatabaseForm_Load(object sender, EventArgs e)
        {
            // Initialize contents
            adminLoginInfo.AuthenticationInfo = SetupInfo.AdminAuthenticationInfo;
            applicationLoginInfo.AuthenticationInfo = SetupInfo.ApplicationAuthenticationInfo;
            serverNameTextBox.Text = SetupInfo.ServerName;
            databaseNameTextBox.Text = SetupInfo.DatabaseName;
            sameAsAdminCheckBox.Checked = SetupInfo.ApplicationAuthenticationSameAsAdmin;
        }

        private void serverNameTextBox_TextChanged(object sender, EventArgs e)
        {
            SetupInfo.ServerName = serverNameTextBox.Text;
        }

        private void databaseNameTextBox_TextChanged(object sender, EventArgs e)
        {
            SetupInfo.DatabaseName = databaseNameTextBox.Text;
        }

        private void sameAsAdminCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetupInfo.ApplicationAuthenticationSameAsAdmin = sameAsAdminCheckBox.Checked;
            applicationLoginInfo.Enabled = !sameAsAdminCheckBox.Checked;
        }

        private void testConnectionButton_Click(object sender, EventArgs e)
        {
            if (ValidateChildren(ValidationConstraints.Enabled))
            {
                TestInformation(true);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            CloseAsOK();
        }

        bool TestInformation(bool showSuccess)
        {
            string message = "";
            bool result = SetupInfo.Validate(ref message);
            if (!result)
            {
                MessageBox.Show(message);
            }
            else if (showSuccess)
            {
                MessageBox.Show(Messages.Succeeded);
            }
            return result;
        }
        protected override bool ValidateContents()
        {
            return base.ValidateContents() && TestInformation(false);
        }

    }
}
