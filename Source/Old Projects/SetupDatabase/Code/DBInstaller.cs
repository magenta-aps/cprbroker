using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.IO;
using System.Xml;
using CPRBroker.DAL;
using CPRBroker.Engine.Util;

namespace CPRBroker.SetupDatabase
{
    /// <summary>
    /// Installs the system's database
    /// </summary>
    [RunInstaller(true)]
    public partial class DBInstaller : Installer
    {
        private SetupInfo SetupInfo = new SetupInfo();

        public DBInstaller()
        {
            InitializeComponent();
        }

        #region Overrides

        public override void Install(IDictionary stateSaver)
        {
            try
            {
                base.Install(stateSaver);
                CreateDatabaseResult createDatabaseResult;
                do
                {
                    GetConnectionStringFromUser();
                    createDatabaseResult = CreateDatabase();
                } while (createDatabaseResult == CreateDatabaseResult.ExistsAndSelectAnother);
                if (createDatabaseResult == CreateDatabaseResult.Success)
                {
                    CreateDatabaseUser();
                    InsertLookups();
                }
                this.SetConnectionStringInConfigFile(this.GetWebConfigFilePath(), SetupInfo.CreateConnectionString(false, true));
            }
            catch (InstallException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new InstallException(Messages.AnErrorHasOccuredAndInstallationWillBeCancelled, ex);
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            try
            {
                base.Uninstall(savedState);
                string connectionString = this.GetConnectionStringFromWebConfig();
                SetupInfo = SetupInfo.FromConnectionString(connectionString);
                DeleteDatabase();
            }
            catch (InstallException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new InstallException(Messages.AnErrorHasOccuredAndInstallationWillBeCancelled, ex);
            }
        }

        #endregion

        #region Install

        private void GetConnectionStringFromUser()
        {
            DatabaseForm frm = new DatabaseForm();
            frm.SetupInfo = SetupInfo;
            BaseForm.ShowAsDialog(frm, this.InstallerWindowWrapper());
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
                DialogResult useExisting = MessageBox.Show(this.InstallerWindowWrapper(), Messages.DatabaseAlreadyExixts, "", MessageBoxButtons.YesNo);
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
                return CreateDatabaseResult.Success;
            }
        }

        /// <summary>
        /// Creates a new user in the database if needed
        /// </summary>
        private void CreateDatabaseUser()
        {
            if (SetupInfo.EffectiveApplicationAuthenticationInfo.IntegratedSecurity)
            {
                MessageBox.Show(this.InstallerWindowWrapper(), Messages.WindowsAuthenticationContactAdmin);
            }
            else
            {
                string adminConnectionString = SetupInfo.CreateConnectionString(true, false);
                ServerConnection dbServerConnection = new ServerConnection(new SqlConnection(adminConnectionString));
                Server dbServer = new Server(dbServerConnection);
                Database db = dbServer.Databases[SetupInfo.DatabaseName];
                string userName = SetupInfo.EffectiveApplicationAuthenticationInfo.UserName;

                User[] usersArray = new User[db.Users.Count];
                db.Users.CopyTo(usersArray, 0);
                var existingUser = (
                    from User user in usersArray
                    where user.Name.ToLower() == userName.ToLower() || user.Login.ToLower() == userName.ToLower()
                    select user
                    ).FirstOrDefault();

                if (existingUser == null)
                {
                    if (!dbServer.Logins.Contains(SetupInfo.EffectiveApplicationAuthenticationInfo.UserName))
                    {
                        Login newLogin = new Login(dbServer, SetupInfo.EffectiveApplicationAuthenticationInfo.UserName);
                        newLogin.PasswordPolicyEnforced = false;
                        newLogin.LoginType = LoginType.SqlLogin;
                        newLogin.Create(SetupInfo.EffectiveApplicationAuthenticationInfo.Password);
                    }
                    User newUser = new User(db, SetupInfo.EffectiveApplicationAuthenticationInfo.UserName);
                    newUser.Login = newUser.Name;
                    newUser.Create();
                    newUser.AddToRole("db_owner");
                }
            }
        }

        public class ColumnType
        {
            public string Name;
            public Type Type;
            public int? Index = null;

            public static readonly Dictionary<Type, string> TypeMap = new Dictionary<Type, string>();

            static ColumnType()
            {
                TypeMap[typeof(String)] = "Text Width 4000";
            }
        }

        private void InsertLookups()
        {
            string connectionString = SetupInfo.CreateConnectionString(false, true);
            Insert<AddressStatus>(Properties.Resources.AddressStatus, connectionString);
            Insert<CPRBroker.DAL.Application>(Properties.Resources.Application, connectionString,
                new ColumnType() { Name = "ApplicationId", Type = typeof(Guid) }
                );
            Insert<ChannelType>(Properties.Resources.ChannelType, connectionString);
            Insert<Country>(Properties.Resources.Country, connectionString);
            Insert<DataProviderType>(Properties.Resources.DataProviderType, connectionString);
            Insert<DataProvider>(Properties.Resources.DataProvider, connectionString);
            Insert<DetailLevel>(Properties.Resources.DetailLevel, connectionString);
            Insert<Gender>(Properties.Resources.Gender, connectionString);
            Insert<LogType>(Properties.Resources.LogType, connectionString);
            Insert<MaritalStatusType>(Properties.Resources.MaritalStatusType, connectionString);
            Insert<Municipality>(Properties.Resources.Municipality, connectionString,
                new ColumnType() { Name = "MunicipalityCode", Type = typeof(string) }
                );
            Insert<OperationType>(Properties.Resources.OperationType, connectionString);
            Insert<PersonStatusType>(Properties.Resources.PersonStatusType, connectionString,
                new ColumnType() { Name = "PersonStatusTypeCode", Type = typeof(string), Index = 1 },
                new ColumnType() { Name = "PersonStatusName", Type = typeof(string), Index = 2 }
                );
            Insert<RelationshipType>(Properties.Resources.RelationshipType, connectionString);
            Insert<SubscriptionType>(Properties.Resources.SubscriptionType, connectionString);
        }

        public void Insert<TTable>(string contents, string connectionString, params ColumnType[] columnTypes)
        {
            string tableName = typeof(TTable).Name;
            string fileName = string.Format("{0}\\{1}.csv", this.GetAssemblyFolderPath(), Guid.NewGuid().ToString());
            FileInfo targetFileInfo = new FileInfo(fileName);

            // Write contents to physical file
            File.WriteAllText(targetFileInfo.FullName, contents, System.Text.Encoding.Unicode);

            // Write Schema.ini file
            string schemaIniContents = string.Format("[{0}]\r\nFormat={1}\r\nColNameHeader={2}\r\nCharacterSet={3}",
                    targetFileInfo.Name,
                    "Delimited(;)",
                    true,
                    "Unicode"
                    );
            foreach (ColumnType colType in columnTypes)
            {
                if (colType.Index.HasValue && ColumnType.TypeMap.ContainsKey(colType.Type))
                {
                    schemaIniContents += string.Format("\r\nCol{0}={1} {2}",
                        colType.Index.Value,
                        colType.Name,
                        ColumnType.TypeMap[colType.Type]
                        );
                }
            }

            string schemaFileName = targetFileInfo.Directory.FullName + "\\Schema.ini";
            File.WriteAllText(schemaFileName, schemaIniContents);

            // Read data table
            OleDbConnectionStringBuilder lookupCsvConnectionStringBuilder = new OleDbConnectionStringBuilder();
            lookupCsvConnectionStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
            lookupCsvConnectionStringBuilder.DataSource = targetFileInfo.Directory.FullName;
            lookupCsvConnectionStringBuilder.Add("Extended Properties", "text;");

            string readLookupDataSql = string.Format("select * from [{0}]", targetFileInfo.Name);
            OleDbDataAdapter oleAdpt = new OleDbDataAdapter(readLookupDataSql, lookupCsvConnectionStringBuilder.ConnectionString);

            DataTable lookupDataTable = new DataTable(tableName);
            foreach (ColumnType colType in columnTypes)
            {
                lookupDataTable.Columns.Add(colType.Name, colType.Type);
            }
            oleAdpt.Fill(lookupDataTable);

            // Save to database
            SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString);
            bulkCopy.DestinationTableName = tableName;

            bulkCopy.WriteToServer(lookupDataTable);

            // Delete temp files
            File.Delete(targetFileInfo.FullName);
            File.Delete(schemaFileName);
        }

        #endregion

        #region Uninstall

        private void DeleteDatabase()
        {
            Server dbServer = new Server(new ServerConnection(new SqlConnection(SetupInfo.CreateConnectionString(true, false))));

            if (!string.IsNullOrEmpty(SetupInfo.DatabaseName) && dbServer.Databases.Contains(SetupInfo.DatabaseName))
            {
                DropDatabaseForm dropDatabaseForm = new DropDatabaseForm()
                {
                    SetupInfo = SetupInfo
                };
                DialogResult res = BaseForm.ShowAsDialog(dropDatabaseForm, this.InstallerWindowWrapper());
                if (res == DialogResult.Yes)
                {
                    dbServer = new Server(new ServerConnection(new SqlConnection(SetupInfo.CreateConnectionString(true, false))));
                    dbServer.KillAllProcesses(SetupInfo.DatabaseName);
                    dbServer.KillDatabase(SetupInfo.DatabaseName);
                }
            }
        }

        #endregion
    }
}
