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
using System.Data.SqlClient;

namespace CprBroker.Installers
{
    /// <summary>
    /// Asks the user if a database should be removed with uninstall operation
    /// </summary>
    public partial class PatchDatabaseForm : BaseForm
    {
        public DatabaseSetupInfo SetupInfo { get; set; }

        public PatchDatabaseForm()
        {
            InitializeComponent();
        }

        private void DropDatabaseForm_Load(object sender, EventArgs e)
        {
            adminLoginInfo.AuthenticationInfo = SetupInfo.AdminAuthenticationInfo;
            databaseNameTextBox.Text = SetupInfo.DatabaseName;
        }

        private bool TestConnection()
        {
            string connectionString = SetupInfo.CreateConnectionString(true, false);

            if (SetupInfo.AdminConnectionHasAdminRights())
            {
                if (SetupInfo.DatabaseExists())
                {
                    return true;
                }
                else
                {
                    MessageBox.Show(Messages.DatabaseDoesNotExist, Messages.Unsuccessful, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                if (SetupInfo.AdminConnectionHasDboRights())
                {
                    return true;
                }
                else
                {
                    MessageBox.Show(Messages.AdminConnectionFailed, Messages.Unsuccessful, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            return false;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            CloseAsOK();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override bool ValidateContents()
        {
            return TestConnection();
        }

    }
}
