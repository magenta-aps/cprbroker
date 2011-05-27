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

namespace CprBroker.Installers.EventBrokerInstallers
{
    /// <summary>
    /// Gets information from user about the backend service name and how to connect to CPR Broker
    /// </summary>
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
