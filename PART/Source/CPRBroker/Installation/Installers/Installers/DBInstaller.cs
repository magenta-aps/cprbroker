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
using CprBroker.DAL;
using CprBroker.Engine.Util;
using System.Reflection;

namespace CprBroker.Installers
{
    /// <summary>
    /// Installs the system's database
    /// </summary>
    public partial class DBInstaller : Installer, ICprInstaller
    {

        private string SuggestedDatabaseName;

        public DBInstaller(string databaseName)
        {
            InitializeComponent();
            SuggestedDatabaseName = databaseName;
        }

        #region Overrides

        public void GetInstallInfoFromUser(IDictionary stateSaver)
        {
            DatabaseForm frm = new DatabaseForm();
            var setupInfo = new DatabaseSetupInfo() { DatabaseName = SuggestedDatabaseName };
            var savedStateWrapper = new SavedStateWrapper(stateSaver);
            frm.SetupInfo = setupInfo;

            do
            {
                BaseForm.ShowAsDialog(frm, this.InstallerWindowWrapper());
                savedStateWrapper.AdminConnectionString = setupInfo.CreateConnectionString(true, true);

                string adminConnectionString = setupInfo.CreateConnectionString(true, false);
                ServerConnection dbServerConnection = new ServerConnection(new SqlConnection(adminConnectionString));
                Server dbServer = new Server(dbServerConnection);

                if (dbServer.Databases.Contains(setupInfo.DatabaseName))
                {
                    DialogResult useExisting = MessageBox.Show(this.InstallerWindowWrapper(), Messages.DatabaseAlreadyExists, "", MessageBoxButtons.YesNo);
                    if (useExisting == DialogResult.Yes)
                    {
                        goto WindowsAuth;
                    }
                }

            WindowsAuth:
                if (setupInfo.EffectiveApplicationAuthenticationInfo.IntegratedSecurity)
                {
                    DialogResult useWindowsAuth = MessageBox.Show(this.InstallerWindowWrapper(), Messages.WindowsAuthenticationContactAdmin, "", MessageBoxButtons.YesNo);
                    if (useWindowsAuth == DialogResult.Yes)
                    {
                        break;
                    }
                }
            } while (true);
            savedStateWrapper.DatabaseSetupInfo = setupInfo;
        }

        protected virtual string[] ConfigFileNames
        {
            get
            {
                return new string[0];
            }
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

                DatabaseSetupInfo setupInfo = new SavedStateWrapper(stateSaver).DatabaseSetupInfo;
                new SavedStateWrapper(stateSaver).DatabaseCreated = CreateDatabase(setupInfo);
                CreateDatabaseUser(setupInfo);

                foreach (string configFileName in this.ConfigFileNames)
                {
                    this.SetConnectionStringInConfigFile(configFileName, setupInfo.CreateConnectionString(false, true));
                }
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
                var w = new SavedStateWrapper(savedState);
                if (w.DatabaseCreated)
                {
                    DeleteDatabase(w.DatabaseSetupInfo, false);
                }
            }
            catch (Exception ex)
            {
                Messages.ShowException(ex);
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            try
            {
                base.Uninstall(savedState);
                var setupInfo = DatabaseSetupInfo.FromConnectionString(new SavedStateWrapper(savedState).AdminConnectionString);
                DeleteDatabase(setupInfo, true);
            }
            catch (Exception ex)
            {
                Messages.ShowException(ex);
            }
        }

        #endregion

        #region Install

        /// <summary>
        /// Loads all Dlls while still in LocalSystem to avoid security exceptions on Vista
        /// </summary>
        private void LoadAllAssemlies()
        {
            string[] fileNames = Directory.GetFiles(this.GetAssemblyFolderPath(), "*.dll");
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

        protected virtual string CreateDatabaseObjectsSql
        {
            get
            {
                return "";
            }
        }

        protected virtual LookupInsertionParameters[] GetLookupInsertionParameters()
        {
            return new LookupInsertionParameters[0];
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
                string sql = Properties.Resources.CreateDatabaseObjects;
                db.ExecuteNonQuery(sql);

                LookupInsertionParameters.InsertLookups(GetLookupInsertionParameters(), setupInfo.CreateConnectionString(true, true));
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
