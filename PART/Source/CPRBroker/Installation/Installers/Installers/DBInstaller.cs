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

        protected virtual string SuggestedDatabaseName
        {
            get
            {
                return "";
            }
        }

        public DBInstaller()
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
                Messages.ShowException(ex);
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            System.Diagnostics.Debugger.Break();
            try
            {
                base.Uninstall(savedState);
                var savedStateWrapper = new SavedStateWrapper(savedState);
                var setupInfo = savedStateWrapper.GetDatabaseSetupInfo();
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
            System.Diagnostics.Debugger.Break();
            foreach (var configFile in this.GetConnectionStringsToConfigure(stateSaver))
            {
                var configFileName = configFile.Key;
                foreach (var connStr in configFile.Value)
                {
                    CprBroker.Engine.Util.Installation.SetConnectionStringInConfigFile(configFileName, connStr.Key, connStr.Value);
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
                string sql = CreateDatabaseObjectsSql;
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
