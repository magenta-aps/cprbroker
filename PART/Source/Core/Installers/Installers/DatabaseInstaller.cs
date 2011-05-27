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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.OleDb;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.IO;
using System.Xml;
using CprBroker.Data;
using System.Reflection;
using CprBroker.Utilities;

namespace CprBroker.Installers
{
    /// <summary>
    /// Installs the system's database
    /// </summary>
    public partial class DatabaseInstaller : Installer
    {

        protected virtual string SuggestedDatabaseName
        {
            get
            {
                return "";
            }
        }

        public DatabaseInstaller()
        {
            InitializeComponent();
        }

        #region Overrides

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            GetInstallInfoFromUser(savedState);
        }
        public void GetInstallInfoFromUser(IDictionary stateSaver)
        {
            DatabaseForm frm = new DatabaseForm();
            var setupInfo = new DatabaseSetupInfo() { DatabaseName = SuggestedDatabaseName };
            var savedStateWrapper = new SavedStateWrapper(stateSaver);
            frm.SetupInfo = setupInfo;


        ShowForm:
            BaseForm.ShowAsDialog(frm, this.InstallerWindowWrapper());

            string adminConnectionString = setupInfo.CreateConnectionString(true, false);
            ServerConnection dbServerConnection = new ServerConnection(new SqlConnection(adminConnectionString));
            Server dbServer = new Server(dbServerConnection);

            if (dbServer.Databases.Contains(setupInfo.DatabaseName))
            {
                DialogResult useExisting = MessageBox.Show(this.InstallerWindowWrapper(), Messages.DatabaseAlreadyExists, "", MessageBoxButtons.YesNo);
                if (useExisting == DialogResult.No)
                {
                    goto ShowForm;
                }
            }

            if (setupInfo.EffectiveApplicationAuthenticationInfo.IntegratedSecurity)
            {
                DialogResult useWindowsAuth = MessageBox.Show(this.InstallerWindowWrapper(), Messages.WindowsAuthenticationContactAdmin, "", MessageBoxButtons.YesNo);
                if (useWindowsAuth == DialogResult.No)
                {
                    goto ShowForm;
                }
            }
            savedStateWrapper.SetDatabaseSetupInfo(setupInfo);
        }

        protected virtual Dictionary<string, Dictionary<string, string>> GetConnectionStringsToConfigure(System.Collections.IDictionary savedState)
        {
            return new Dictionary<string, Dictionary<string, string>>();
        }

        /// <summary>
        /// Creates the system's database based on user parameters
        /// Impersonates NetworkService account when accessing the database server
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            try
            {
                base.Install(stateSaver);
                this.LoadAllAssemlies();

                var savedStateWrapper = new SavedStateWrapper(stateSaver);
                DatabaseSetupInfo setupInfo = savedStateWrapper.GetDatabaseSetupInfo();
                setupInfo.DatabaseCreated = CreateDatabase(setupInfo);
                savedStateWrapper.SetDatabaseSetupInfo(setupInfo);
                CreateDatabaseUser(setupInfo);
                SetConnectionStrings(stateSaver);
                savedStateWrapper.ClearDatabaseSensitiveDate();
            }
            catch (InstallException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new InstallException(Messages.AnErrorHasOccurredAndInstallationWillBeCancelled, ex);
            }
        }

        public override void Rollback(IDictionary savedState)
        {
            try
            {
                base.Rollback(savedState);
                var savedStateWrapper = new SavedStateWrapper(savedState);
                var setupInfo = savedStateWrapper.GetDatabaseSetupInfo();
                if (setupInfo.DatabaseCreated)
                {
                    DeleteDatabase(savedStateWrapper.GetDatabaseSetupInfo(), false);
                }
            }
            catch (Exception ex)
            {
                Messages.ShowException(this, "", ex);
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            try
            {
                base.Uninstall(savedState);
                var savedStateWrapper = new SavedStateWrapper(savedState);
                var setupInfo = savedStateWrapper.GetDatabaseSetupInfo();
                DeleteDatabase(setupInfo, true);
            }
            catch (Exception ex)
            {
                Messages.ShowException(this, "", ex);
            }
        }

        #endregion

        #region Install

        /// <summary>
        /// Loads all Dlls while still in LocalSystem to avoid security exceptions on Vista
        /// </summary>
        private void LoadAllAssemlies()
        {
            string[] fileNames = Directory.GetFiles(this.GetInstallerAssemblyFolderPath(), "*.dll");
            foreach (string fileName in fileNames)
            {
                AssemblyName asmName = AssemblyName.GetAssemblyName(fileName);
                AppDomain.CurrentDomain.Load(asmName);
            }
        }

        /// <summary>
        /// Creates a new user in the database if needed
        /// </summary>
        private void CreateDatabaseUser(DatabaseSetupInfo setupInfo)
        {
            string adminConnectionString = setupInfo.CreateConnectionString(true, false);
            ServerConnection dbServerConnection = new ServerConnection(new SqlConnection(adminConnectionString));
            Server dbServer = new Server(dbServerConnection);


            Database db = dbServer.Databases[setupInfo.DatabaseName];

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

        private void SetConnectionStrings(System.Collections.IDictionary stateSaver)
        {
            foreach (var configFile in this.GetConnectionStringsToConfigure(stateSaver))
            {
                var configFileName = configFile.Key;
                foreach (var connStr in configFile.Value)
                {
                    ConnectionStringsInstaller.RegisterConnectionString(configFileName, connStr.Key, connStr.Value);
                }
            }
        }
        protected virtual string CreateDatabaseObjectsSql
        {
            get
            {
                return "";
            }
        }

        protected virtual KeyValuePair<string,string>[] GetLookupData()
        {
            return new KeyValuePair<string, string>[0];
        }

        private void InsertLookups(string connectionString)
        {
            foreach (var lookupData in GetLookupData())
            {
                string tableName = lookupData.Key;
                string csv = lookupData.Value;

                string[] lines = csv.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string[] columnNames = lines[0].Split(';');
                columnNames = columnNames.Select(c => string.Format("[{0}]", c)).ToArray();

                string sql = "";
                for (int i = 1; i < lines.Length; i++)
                {
                    sql += string.Format("INSERT INTO [{0}] (",tableName);
                    sql += string.Join(",", columnNames);
                    sql +=") VALUES (";

                    string[] values = lines[i].Split(';');
                    values = values.Select(v => string.Format("'{0}'", v)).ToArray();
                    sql += string.Join(",", values);
                    sql += ")"+Environment.NewLine;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        int result=command.ExecuteNonQuery();
                        object o = "";
                    }
                }
            }
        }

        public bool CreateDatabase(DatabaseSetupInfo setupInfo)
        {
            string adminConnectionString = setupInfo.CreateConnectionString(true, false);
            ServerConnection dbServerConnection = new ServerConnection(new SqlConnection(adminConnectionString));
            Server dbServer = new Server(dbServerConnection);

            if (!dbServer.Databases.Contains(setupInfo.DatabaseName))
            {
                Database db = new Database(dbServer, setupInfo.DatabaseName);
                db.Create();
                string sql = CreateDatabaseObjectsSql;
                db.ExecuteNonQuery(sql);
                InsertLookups(setupInfo.CreateConnectionString(true,true));
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Uninstall

        private void DeleteDatabase(DatabaseSetupInfo setupInfo, bool askUser)
        {
            Server dbServer = new Server(new ServerConnection(new SqlConnection(setupInfo.CreateConnectionString(true, false))));

            if (!string.IsNullOrEmpty(setupInfo.DatabaseName) && dbServer.Databases.Contains(setupInfo.DatabaseName))
            {
                DropDatabaseForm dropDatabaseForm = new DropDatabaseForm()
                {
                    SetupInfo = setupInfo
                };

                if (!askUser || BaseForm.ShowAsDialog(dropDatabaseForm, this.InstallerWindowWrapper()) == DialogResult.Yes)
                {
                    dbServer = new Server(new ServerConnection(new SqlConnection(setupInfo.CreateConnectionString(true, false))));
                    dbServer.KillAllProcesses(setupInfo.DatabaseName);
                    dbServer.KillDatabase(setupInfo.DatabaseName);
                }
            }
        }

        #endregion

        #region ICprInstaller Members


        public void GetUnInstallInfoFromUser(IDictionary stateSaver)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
