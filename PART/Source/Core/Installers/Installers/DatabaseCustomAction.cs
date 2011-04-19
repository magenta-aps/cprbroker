using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Data.SqlClient;

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
