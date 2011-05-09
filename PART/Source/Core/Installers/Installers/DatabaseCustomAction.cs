/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 *
 * The Initial Developer of the Original Code is
 * IT- og Telestyrelsen / Danish National IT and Telecom Agency.
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
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
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Data.SqlClient;
using System.Windows.Forms;
using CprBroker.Utilities;

namespace CprBroker.Installers.Installers
{
    public class DatabaseCustomAction
    {
        public static ActionResult TestConnectionString(Session session)
        {
            try
            {
                DatabaseSetupInfo dbInfo = GetDatabaseSetupInfo(session);
                string message = "";
                if (dbInfo.Validate(ref message))
                {
                    session["DB_VALID"] = "True";
                }
                else
                {
                    session["DB_VALID"] = message;
                }
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return ActionResult.Failure;
            }
        }

        private static DatabaseSetupInfo GetDatabaseSetupInfo(Session session)
        {
            DatabaseSetupInfo ret = new DatabaseSetupInfo();
            ret.ServerName = session["DB_SERVERNAME"];
            ret.DatabaseName = session["DB_DATABASENAME"];

            ret.AdminAuthenticationInfo = new DatabaseSetupInfo.AuthenticationInfo();
            ret.AdminAuthenticationInfo.IntegratedSecurity = session["DB_ADMININTEGRATEDSECURITY"] == "SSPI";
            if (!ret.AdminAuthenticationInfo.IntegratedSecurity)
            {
                ret.AdminAuthenticationInfo.UserName = session["DB_ADMINUSERNAME"];
                ret.AdminAuthenticationInfo.Password = session["DB_ADMINPASSWORD"];
            }

            ret.ApplicationAuthenticationSameAsAdmin = !string.IsNullOrEmpty(session["DB_APPSAMEASADMIN"]);
            if (!ret.ApplicationAuthenticationSameAsAdmin)
            {
                ret.ApplicationAuthenticationInfo = new DatabaseSetupInfo.AuthenticationInfo();
                ret.ApplicationAuthenticationInfo.IntegratedSecurity = session["DB_APPINTEGRATEDSECURITY"] == "SSPI";
                if (!ret.ApplicationAuthenticationInfo.IntegratedSecurity)
                {
                    ret.ApplicationAuthenticationInfo.UserName = session["DB_APPUSERNAME"];
                    ret.ApplicationAuthenticationInfo.Password = session["DB_APPPASSWORD"];
                }
            }
            return ret;
        }

        public static ActionResult RemoveCprBrokerDatabase(Session session)
        {
            DatabaseSetupInfo setupInfo = GetDatabaseSetupInfo(session);
             DropDatabaseForm dropDatabaseForm = new DropDatabaseForm()
                {
                    SetupInfo = setupInfo
                };

             if (BaseForm.ShowAsDialog(dropDatabaseForm, session.InstallerWindowWrapper()) == DialogResult.Yes)
             {
                 Server dbServer = new Server(new ServerConnection(new SqlConnection(setupInfo.CreateConnectionString(true, false))));
                 if (!string.IsNullOrEmpty(setupInfo.DatabaseName) && dbServer.Databases.Contains(setupInfo.DatabaseName))
                 {
                     dbServer.KillAllProcesses(setupInfo.DatabaseName);
                     dbServer.KillDatabase(setupInfo.DatabaseName);
                 }
             }
            return ActionResult.Success;
        }

        public static ActionResult FinalizeDatabase(Session session, string createDatabaseObjectsSql, KeyValuePair<string, string>[] lookupDataArray)
        {
            DatabaseSetupInfo setupInfo = GetDatabaseSetupInfo(session);
            //setupInfo.DatabaseCreated = CreateDatabase(setupInfo, createDatabaseObjectsSql, lookupDataArray);
            InsertLookups(setupInfo.CreateConnectionString(true, true), lookupDataArray);
            CreateDatabaseUser(setupInfo);
            //SetConnectionStrings(stateSaver);
            //savedStateWrapper.ClearDatabaseSensitiveDate();

            return ActionResult.Success;
        }


        /*private static bool CreateDatabase(DatabaseSetupInfo setupInfo, string createDatabaseObjectsSql, KeyValuePair<string, string>[] lookupDataArray)
        {
            string adminConnectionString = setupInfo.CreateConnectionString(true, false);
            ServerConnection dbServerConnection = new ServerConnection(new SqlConnection(adminConnectionString));
            Server dbServer = new Server(dbServerConnection);

            if (!dbServer.Databases.Contains(setupInfo.DatabaseName))
            {
                var db = new Microsoft.SqlServer.Management.Smo.Database(dbServer, setupInfo.DatabaseName);
                db.Create();
                string sql = createDatabaseObjectsSql;
                db.ExecuteNonQuery(sql);
                InsertLookups(setupInfo.CreateConnectionString(true, true), lookupDataArray);
                return true;
            }
            else
            {
                return false;
            }
        }*/

        /// <summary>
        /// Creates a new user in the database if needed
        /// </summary>
        private static void CreateDatabaseUser(DatabaseSetupInfo setupInfo)
        {
            if (!setupInfo.ApplicationAuthenticationSameAsAdmin)
            {
                string adminConnectionString = setupInfo.CreateConnectionString(true, false);
                ServerConnection dbServerConnection = new ServerConnection(new SqlConnection(adminConnectionString));
                Server dbServer = new Server(dbServerConnection);


                var db = dbServer.Databases[setupInfo.DatabaseName];

                string userName;
                LoginType loginType;
                Action<Login> createLoginMethod;

                if (setupInfo.EffectiveApplicationAuthenticationInfo.IntegratedSecurity)
                {
                    userName = @"NT AUTHORITY\NETWORK SERVICE";
                    loginType = LoginType.WindowsUser;
                    createLoginMethod = (login) => login.Create();
                }
                else
                {
                    userName = setupInfo.EffectiveApplicationAuthenticationInfo.UserName;
                    loginType = LoginType.SqlLogin;
                    createLoginMethod = (login) => login.Create(setupInfo.EffectiveApplicationAuthenticationInfo.Password);
                }
                User[] usersArray = new User[db.Users.Count];
                db.Users.CopyTo(usersArray, 0);
                var existingUser = (
                    from User user in usersArray
                    where user.Name.ToLower() == userName.ToLower() || user.Login.ToLower() == userName.ToLower()
                    select user
                    ).FirstOrDefault();

                if (existingUser == null)
                {
                    if (!dbServer.Logins.Contains(userName))
                    {
                        Login newLogin = new Login(dbServer, userName);
                        newLogin.PasswordPolicyEnforced = false;
                        newLogin.LoginType = loginType;
                        createLoginMethod(newLogin);
                    }
                    User newUser = new User(db, userName);
                    newUser.Login = newUser.Name;
                    newUser.Create();
                    newUser.AddToRole("db_owner");
                }
            }
        }

        private static void InsertLookups(string connectionString, KeyValuePair<string, string>[] lookupDataArray)
        {
            foreach (var lookupData in lookupDataArray)
            {
                string tableName = lookupData.Key;
                string csv = lookupData.Value;

                string[] lines = csv.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string[] columnNames = lines[0].Split(';');
                columnNames = columnNames.Select(c => string.Format("[{0}]", c)).ToArray();

                string sql = "";
                for (int i = 1; i < lines.Length; i++)
                {
                    sql += string.Format("INSERT INTO [{0}] (", tableName);
                    sql += string.Join(",", columnNames);
                    sql += ") VALUES (";

                    string[] values = lines[i].Split(';');
                    values = values.Select(v => string.Format("'{0}'", v)).ToArray();
                    sql += string.Join(",", values);
                    sql += ")" + Environment.NewLine;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        int result = command.ExecuteNonQuery();
                        object o = "";
                    }
                }
            }
        }
    }
}
