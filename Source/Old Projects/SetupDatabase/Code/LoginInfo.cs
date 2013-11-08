using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CPRBroker.SetupDatabase
{
    /// <summary>
    /// Contains login information for a database connection
    /// </summary>
    [ToolboxItem(true)]
    public partial class LoginInfo : UserControl
    {
        public LoginInfo()
        {
            InitializeComponent();
        }

        private void authenticationRadio_CheckedChanged(object sender, EventArgs e)
        {
            loginInfoPanel.Enabled = sqlAuthenticationRadio.Checked;
            userIdTextBox.TabIndex = userIdTextBox.TabIndex;
            passwordTextBox.TabIndex = passwordTextBox.TabIndex;

            AuthenticationInfo.IntegratedSecurity = windowsAuthenticationRadio.Checked;
        }

        private SetupInfo.AuthenticationInfo _AuthenticationInfo = new SetupInfo.AuthenticationInfo();
        public SetupInfo.AuthenticationInfo AuthenticationInfo
        {
            get
            {
                return _AuthenticationInfo;
            }
            set
            {
                _AuthenticationInfo = value;
                windowsAuthenticationRadio.Checked = value.IntegratedSecurity;
                sqlAuthenticationRadio.Checked = !value.IntegratedSecurity;
                userIdTextBox.Text = value.UserName;
                passwordTextBox.Text = value.Password;
            }
        }

        private void userIdTextBox_TextChanged(object sender, EventArgs e)
        {
            AuthenticationInfo.UserName = userIdTextBox.Text;
        }

        private void passwordTextBox_TextChanged(object sender, EventArgs e)
        {
            AuthenticationInfo.Password = passwordTextBox.Text;
        }

    }
}
