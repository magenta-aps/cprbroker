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

namespace CprBroker.SetupDatabase
{
    /// <summary>
    /// Installs the system's database
    /// </summary>
    [RunInstaller(true)]
    public partial class DBInstaller : Installer
    {

        private SetupInfo SetupInfo = new SetupInfo() { DatabaseName = "CprBroker" };

        public DBInstaller()
        {
            InitializeComponent();
        }

        SavedStateWrapper SavedStateWrapper = null;

        #region Overrides

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
                CreateDatabaseResult createDatabaseResult;
                // Get database parameters and create database if needed                
                SavedStateWrapper = new SavedStateWrapper(stateSaver);
                do
                {
                    GetConnectionStringFromUser();
                    createDatabaseResult = CreateDatabase();
                } while (createDatabaseResult == CreateDatabaseResult.ExistsAndSelectAnother);

                CreateDatabaseUser();
                if (createDatabaseResult == CreateDatabaseResult.Success)
                {
                    SavedStateWrapper.DatabaseCreated = true;
                    LookupInsertionParameters[] insertParameters = LookupInsertionParameters.InitializeInsertionParameters(this);
                    LookupInsertionParameters.InsertLookups(insertParameters, SetupInfo.CreateConnectionString(true, true));
                }
                this.SetConnectionStringInConfigFile(this.GetWebConfigFilePath(), SetupInfo.CreateConnectionString(false, true));
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
                SavedStateWrapper = new SavedStateWrapper(savedState);
                SetupInfo = SetupInfo.FromConnectionString(SavedStateWrapper.AdminConnectionString);
                DeleteDatabase(false);
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
                SavedStateWrapper = new SavedStateWrapper(savedState);
                SetupInfo = SetupInfo.FromConnectionString(SavedStateWrapper.AdminConnectionString);
                DeleteDatabase(true);
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

        private void GetConnectionStringFromUser()
        {
            DatabaseForm frm = new DatabaseForm();
            frm.SetupInfo = SetupInfo;
            BaseForm.ShowAsDialog(frm, this.InstallerWindowWrapper());
            SavedStateWrapper.AdminConnectionString = SetupInfo.CreateConnectionString(true, true);
        }

        enum CreateDatabaseResult
        {
            Success,
            ExistsAndUse,
            ExistsAndSelectAnother
        }

        private CreateDatabaseResult CreateDatabase()
        {
            string adminConnectionString = SetupInfo.CreateConnectionString(true, false);
            ServerConnection dbServerConnection = new ServerConnection(new SqlConnection(adminConnectionString));
            Server dbServer = new Server(dbServerConnection);
            if (dbServer.Databases.Contains(SetupInfo.DatabaseName))
            {
                DialogResult useExisting = MessageBox.Show(this.InstallerWindowWrapper(), Messages.DatabaseAlreadyExists, "", MessageBoxButtons.YesNo);
                if (useExisting == DialogResult.Yes)
                {
                    return CreateDatabaseResult.ExistsAndUse;
                }
                else //if (useExisting == DialogResult.No)
                {
                    return CreateDatabaseResult.ExistsAndSelectAnother;
                }
            }
            else
            {
                Database db = new Database(dbServer, SetupInfo.DatabaseName);
                db.Create();
                string sql = Properties.Resources.CreateDatabaseObjects;
                db.ExecuteNonQuery(sql);
                SavedStateWrapper.DatabaseCreated = true;
                return CreateDatabaseResult.Success;
            }
        }

        /// <summary>
        /// Creates a new user in the database if needed
        /// </summary>
        private void CreateDatabaseUser()
        {
            string adminConnectionString = SetupInfo.CreateConnectionString(true, false);
            ServerConnection dbServerConnection = new ServerConnection(new SqlConnection(adminConnectionString));
            Server dbServer = new Server(dbServerConnection);
            Database db = dbServer.Databases[SetupInfo.DatabaseName];

            string userName;
            LoginType loginType;
            Action<Login> createLoginMethod;

            if (SetupInfo.EffectiveApplicationAuthenticationInfo.IntegratedSecurity)
            {
                userName = @"NT AUTHORITY\NETWORK SERVICE";
                loginType = LoginType.WindowsUser;
                createLoginMethod = (login) => login.Create();
            }
            else
            {
                userName = SetupInfo.EffectiveApplicationAuthenticationInfo.UserName;
                loginType = LoginType.SqlLogin;
                createLoginMethod = (login) => login.Create(SetupInfo.EffectiveApplicationAuthenticationInfo.Password);
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
            if (SetupInfo.EffectiveApplicationAuthenticationInfo.IntegratedSecurity)
            {
                MessageBox.Show(this.InstallerWindowWrapper(), Messages.WindowsAuthenticationContactAdmin);
            }
        }

        #endregion

        #region Uninstall

        private void DeleteDatabase(bool askUser)
        {
            if (SavedStateWrapper.DatabaseCreated)
            {
                Server dbServer = new Server(new ServerConnection(new SqlConnection(SetupInfo.CreateConnectionString(true, false))));

                if (!string.IsNullOrEmpty(SetupInfo.DatabaseName) && dbServer.Databases.Contains(SetupInfo.DatabaseName))
                {
                    DropDatabaseForm dropDatabaseForm = new DropDatabaseForm()
                    {
                        SetupInfo = SetupInfo
                    };

                    if (!askUser || BaseForm.ShowAsDialog(dropDatabaseForm, this.InstallerWindowWrapper()) == DialogResult.Yes)
                    {
                        dbServer = new Server(new ServerConnection(new SqlConnection(SetupInfo.CreateConnectionString(true, false))));
                        dbServer.KillAllProcesses(SetupInfo.DatabaseName);
                        dbServer.KillDatabase(SetupInfo.DatabaseName);
                    }
                }
            }
        }

        #endregion
    }
}
