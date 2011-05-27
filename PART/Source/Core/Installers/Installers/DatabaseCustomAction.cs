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
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using CprBroker.Utilities;

namespace CprBroker.Installers
{
    public static partial class DatabaseCustomAction
    {

        public static ActionResult TestConnectionString(Session session)
        {
            return TestConnectionString(session, true);
        }

        public static ActionResult TestConnectionString(Session session, bool databaseShouldBeNew)
        {
            try
            {
                DatabaseSetupInfo dbInfo = DatabaseSetupInfo.FromSession(session);
                string message = "";
                dbInfo.UseExistingDatabase = false;

                if (dbInfo.Validate(ref message))
                {
                    if (databaseShouldBeNew)
                    {
                        if (!dbInfo.DatabaseExists()) // Normal case
                        {
                            session["DB_VALID"] = "True";
                        }
                        else if (MessageBox.Show(session.InstallerWindowWrapper(), Messages.DatabaseAlreadyExists, "", MessageBoxButtons.YesNo) == DialogResult.Yes) // Database exists and won't be created
                        {
                            dbInfo.UseExistingDatabase = true;
                            session["DB_VALID"] = "True";
                        }
                        else  // Database exists and user should change its name
                        {
                            session["DB_VALID"] = "False";
                        }
                    }
                    else
                    {
                        if (dbInfo.DatabaseExists())
                        {
                            session["DB_VALID"] = "True";
                        }
                        else
                        {
                            session["DB_VALID"] = Messages.DatabaseDoesNotExist;
                        }
                    }
                }
                else
                {
                    session["DB_VALID"] = message;
                }

                dbInfo.CopyToSession(session);
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return ActionResult.Failure;
            }
        }

        public static ActionResult DeployDatabase(Session session, string createDatabaseObjectsSql, KeyValuePair<string, string>[] lookupDataArray)
        {
            DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.FromSession(session);
            if (CreateDatabase(databaseSetupInfo, createDatabaseObjectsSql, lookupDataArray))
            {
                ExecuteDDL(createDatabaseObjectsSql, databaseSetupInfo);
                InsertLookups(lookupDataArray, databaseSetupInfo);
            }
            CreateDatabaseUser(databaseSetupInfo, null);

            return ActionResult.Success;
        }

        public static ActionResult RemoveDatabase(Session session, bool askUser)
        {
            DatabaseSetupInfo setupInfo = DatabaseSetupInfo.FromSession(session);
            if (!setupInfo.UseExistingDatabase)
            {
                DropDatabaseForm dropDatabaseForm = new DropDatabaseForm()
                {
                    SetupInfo = setupInfo
                };

                if (!askUser || BaseForm.ShowAsDialog(dropDatabaseForm, session.InstallerWindowWrapper()) == DialogResult.Yes)
                {
                    DropDatabase(setupInfo);
                    DropDatabaseUser(setupInfo);
                }
            }
            return ActionResult.Success;
        }
    }
}
