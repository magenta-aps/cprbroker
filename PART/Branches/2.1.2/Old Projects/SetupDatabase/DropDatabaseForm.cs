using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace CprBroker.SetupDatabase
{
    public partial class DropDatabaseForm : BaseForm
    {
        public SetupInfo SetupInfo { get; set; }

        public DropDatabaseForm()
        {
            InitializeComponent();
        }

        private void DropDatabaseForm_Load(object sender, EventArgs e)
        {
            adminLoginInfo.AuthenticationInfo = SetupInfo.AdminAuthenticationInfo;
            databaseNameTextBox.Text = SetupInfo.DatabaseName;
        }


        private void EnableLoginInfo()
        {
            loginInfoGroupBox.Enabled = yesRadioButton.Checked && TestConnectionRights();
        }

        private bool TestConnectionRights()
        {
            string connectionString = SetupInfo.CreateConnectionString(true, false);
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                ServerConnection serverConnection = new ServerConnection(conn);
                if (((int)serverConnection.FixedServerRoles & (int)FixedServerRoles.SysAdmin) > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                object o = ex;
            }
            MessageBox.Show(Messages.AdminConnectionFailed, Messages.Unsuccessful, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = yesRadioButton.Checked ? DialogResult.Yes : DialogResult.No;
            CloseAsOK();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override bool ValidateContents()
        {
            if (noRadioButton.Checked)
            {
                return true;
            }
            else
            {
                return TestConnectionRights();
            }
        }

        private void dropDatabaseRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            adminLoginInfo.Enabled = yesRadioButton.Checked;
        }

    }
}
