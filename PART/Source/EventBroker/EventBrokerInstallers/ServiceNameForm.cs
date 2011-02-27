using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CprBroker.Installers.EventBrokerInstallers
{
    public partial class ServiceNameForm : BaseForm
    {
        public string ServiceName = "Event broker service";
        public string CprEventsServiceUrl = "";

        public DatabaseSetupInfo CprBrokerDatabaseInfo { get; set; }

        public ServiceNameForm()
        {
            InitializeComponent();

            serviceNameTextBox.Text = ServiceName;

            CprBrokerDatabaseInfo = new DatabaseSetupInfo();
            CprBrokerDatabaseInfo.ApplicationAuthenticationSameAsAdmin = false;
            CprBrokerDatabaseInfo.ApplicationAuthenticationInfo.IntegratedSecurity = true;

            applicationLoginInfo.AuthenticationInfo = CprBrokerDatabaseInfo.ApplicationAuthenticationInfo;
            serverNameTextBox.Text = CprBrokerDatabaseInfo.ServerName;
            databaseNameTextBox.Text = CprBrokerDatabaseInfo.DatabaseName;
        }

        private void serverNameTextBox_TextChanged(object sender, EventArgs e)
        {
            CprBrokerDatabaseInfo.ServerName = serverNameTextBox.Text;
        }

        private void databaseNameTextBox_TextChanged(object sender, EventArgs e)
        {
            CprBrokerDatabaseInfo.DatabaseName = databaseNameTextBox.Text;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            CloseAsOK();
        }

        protected override bool ValidateContents()
        {
            if (!base.ValidateContents())
                return false;

            ServiceName = serviceNameTextBox.Text;
            CprEventsServiceUrl = cprBrokerEventsServiceUrlCustomTextBox.Text + cprBrokerEventsServiceUrlLabel.Text;

            var services = System.ServiceProcess.ServiceController.GetServices();
            if (services.Where(sc => sc.ServiceName.ToLower() == ServiceName.ToLower()).Count() > 0)
            {
                MessageBox.Show(this, "Service name exists");
                return false;
            }

            string url = CprEventsServiceUrl + "?wsdl";
            try
            {
                System.Net.WebClient cl = new System.Net.WebClient();
                cl.DownloadData(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to connect to Cpr Broker. Please validate the URL");
                return false;
            }

            string connectionString = CprBrokerDatabaseInfo.CreateConnectionString(false, true);
            try
            {
                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString);
                conn.Open();
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to connect to Cpr Broker database. Please validate the database parameters");
                return false;
            }

            return true;
        }






    }
}
