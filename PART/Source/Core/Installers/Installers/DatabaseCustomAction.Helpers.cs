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

        private static bool CreateDatabase(DatabaseSetupInfo setupInfo, string createDatabaseObjectsSql, KeyValuePair<string, string>[] lookupDataArray)
        {
            if (!setupInfo.DatabaseExists())
            {
                using (SqlConnection adminConnection = new SqlConnection(setupInfo.CreateConnectionString(true, false)))
                {
                    adminConnection.Open();
                    using (SqlCommand createCommand = new SqlCommand(string.Format("CREATE DATABASE [{0}]", setupInfo.DatabaseName), adminConnection))
                    {
                        createCommand.ExecuteNonQuery();
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void ExecuteDDL(string createDatabaseObjectsSql, DatabaseSetupInfo databaseSetupInfo)
        {
            string adminConnectionStringWithDb = databaseSetupInfo.CreateConnectionString(true, true);
            using (SqlConnection adminConnectionWDb = new SqlConnection(adminConnectionStringWithDb))
            {
                adminConnectionWDb.Open();
                using (SqlCommand createObjectsCommand = new SqlCommand("", adminConnectionWDb))
                {
                    createDatabaseObjectsSql += "\nGO";   // make sure last batch is executed.
                    string sqlBatch = "";

                    foreach (string line in createDatabaseObjectsSql.Split(new string[2] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (line.ToUpperInvariant().Trim() == "GO")
                        {
                            if (!string.IsNullOrEmpty(sqlBatch))
                            {
                                createObjectsCommand.CommandText = sqlBatch;
                                createObjectsCommand.ExecuteNonQuery();
                            }
                            sqlBatch = string.Empty;
                        }
                        else
                        {
                            sqlBatch += line + "\n";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new user in the database if needed
        /// </summary>
        public static void CreateDatabaseUser(DatabaseSetupInfo databaseSetupInfo, string[] neededTables)
        {
            if (!databaseSetupInfo.ApplicationAuthenticationSameAsAdmin)
            {
                string userName;
                Func<SqlConnection, SqlCommand> createLoginMethod;

                if (databaseSetupInfo.EffectiveApplicationAuthenticationInfo.IntegratedSecurity)
                {
                    userName = @"NT AUTHORITY\NETWORK SERVICE";
                    createLoginMethod = (connection) => new SqlCommand(string.Format("CREATE LOGIN [{0}] FROM WINDOWS", userName), connection);
                }
                else
                {
                    userName = databaseSetupInfo.EffectiveApplicationAuthenticationInfo.UserName;
                    createLoginMethod = (connection) => new SqlCommand(
                        string.Format(
                        "CREATE LOGIN [{0}] WITH PASSWORD='{1}', DEFAULT_DATABASE=[{2}], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF",
                        userName,
                        databaseSetupInfo.EffectiveApplicationAuthenticationInfo.Password,
                        databaseSetupInfo.DatabaseName
                        ),
                        connection
                    );
                }

                using (SqlConnection adminConnection = new SqlConnection(databaseSetupInfo.CreateConnectionString(true, false)))
                {
                    adminConnection.Open();
                    using (SqlConnection adminConnectionWithDb = new SqlConnection(databaseSetupInfo.CreateConnectionString(true, true)))
                    {
                        adminConnectionWithDb.Open();
                        SqlCommand selectUserCommand = new SqlCommand("SELECT COUNT(*) FROM sys.database_principals AS db INNER JOIN sys.server_principals AS S ON db.sid = s.sid WHERE db.name=@UserName OR S.name=@UserName", adminConnectionWithDb);
                        selectUserCommand.Parameters.Add("@UserName", System.Data.SqlDbType.VarChar).Value = userName;
                        bool userExists = (int)selectUserCommand.ExecuteScalar() > 0;
                        if (!userExists)
                        {
                            using (SqlCommand loginExistsCommand = new SqlCommand("SELECT COUNT(*) FROM sys.server_principals WHERE NAME=@UserName", adminConnection))
                            {
                                loginExistsCommand.Parameters.Add("@UserName", System.Data.SqlDbType.VarChar).Value = userName;
                                var loginExists = (int)loginExistsCommand.ExecuteScalar() > 0;
                                if (!loginExists)
                                {
                                    SqlCommand createLoginCommand = createLoginMethod(adminConnection);
                                    createLoginCommand.ExecuteNonQuery();
                                }
                                SqlCommand createUserCommand = new SqlCommand(string.Format("CREATE USER [{0}] FOR LOGIN  [{0}]", userName), adminConnectionWithDb);
                                createUserCommand.ExecuteNonQuery();
                            }
                        }
                        using (SqlCommand permissionsCommand = new SqlCommand("", adminConnectionWithDb))
                        {
                            if (neededTables == null || neededTables.Length == 0)
                            {
                                if (!databaseSetupInfo.IsDatabaseRoleMember("db_owner", userName, adminConnectionWithDb))
                                {
                                    permissionsCommand.CommandText = "sp_addrolemember";
                                    permissionsCommand.CommandType = System.Data.CommandType.StoredProcedure;
                                    permissionsCommand.Parameters.Add("@rolename", System.Data.SqlDbType.VarChar).Value = "db_owner";
                                    permissionsCommand.Parameters.Add("@membername", System.Data.SqlDbType.VarChar).Value = userName;
                                    permissionsCommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                permissionsCommand.CommandType = System.Data.CommandType.Text;
                                foreach (string tableName in neededTables)
                                {
                                    permissionsCommand.CommandText = string.Format("GRANT DELETE,INSERT,REFERENCES,SELECT,UPDATE,VIEW DEFINITION ON [dbo].[{0}] TO [{1}]", tableName, userName);
                                    permissionsCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void InsertLookups(KeyValuePair<string, string>[] lookupDataArray, DatabaseSetupInfo databaseSetupInfo)
        {
            foreach (var lookupData in lookupDataArray)
            {
                using (SqlConnection conn = new SqlConnection(databaseSetupInfo.CreateConnectionString(true, true)))
                {
                    conn.Open();
                    InsertLookup(lookupData.Key, lookupData.Value, conn);
                }
            }
        }

        public static void InsertLookup(string tableName, string csv, SqlConnection conn)
        {
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

            using (SqlCommand command = new SqlCommand(sql, conn))
            {
                int result = command.ExecuteNonQuery();
                object o = "";
            }
        }

        private static void RunCustomMethod(Action<SqlConnection> method, DatabaseSetupInfo databaseSetupInfo)
        {
            if (method != null)
            {
                using (SqlConnection conn = new SqlConnection(databaseSetupInfo.CreateConnectionString(true, true)))
                {
                    conn.Open();
                    method(conn);
                }
            }
        }

        private static void DropDatabase(DatabaseSetupInfo setupInfo)
        {
            if (!string.IsNullOrEmpty(setupInfo.DatabaseName) && setupInfo.DatabaseExists())
            {
                using (SqlConnection adminConnection = new SqlConnection(setupInfo.CreateConnectionString(true, false)))
                {
                    adminConnection.Open();
                    using (SqlCommand selectCommand = new SqlCommand("SELECT spid from sys.sysprocesses WHERE dbid in (SELECT database_id FROM sys.databases WHERE name=@Name)", adminConnection))
                    {
                        selectCommand.Parameters.Add("@Name", System.Data.SqlDbType.VarChar).Value = setupInfo.DatabaseName;
                        DataTable processIdsTable = new DataTable();
                        SqlDataAdapter processIdsAdapter = new SqlDataAdapter(selectCommand);
                        processIdsAdapter.Fill(processIdsTable);

                        using (SqlCommand killCommand = new SqlCommand("", adminConnection))
                        {
                            foreach (DataRow dRow in processIdsTable.Rows)
                            {
                                killCommand.CommandText += string.Format("KILL {0};\r\n", dRow[0]);
                            }
                            killCommand.CommandText += string.Format("DROP DATABASE [{0}]", setupInfo.DatabaseName);
                            killCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public static void DropDatabaseUser(DatabaseSetupInfo setupInfo)
        {
            Action<SqlConnection> dropLoginMethod = (connection) =>
            {
                using (SqlCommand dropLoginCommand = new SqlCommand("sp_droplogin", connection))
                {
                    dropLoginCommand.CommandType = CommandType.StoredProcedure;
                    dropLoginCommand.Parameters.Add("@loginame", SqlDbType.VarChar).Value = setupInfo.EffectiveApplicationAuthenticationInfo.UserName;
                    dropLoginCommand.ExecuteNonQuery();
                }
            };

            using (SqlConnection adminConnection = new SqlConnection(setupInfo.CreateConnectionString(true, false)))
            {
                adminConnection.Open();
                if (!setupInfo.ApplicationAuthenticationSameAsAdmin && !setupInfo.ApplicationAuthenticationInfo.IntegratedSecurity)
                {
                    if (!setupInfo.IsServerRoleMember("sysadmin", setupInfo.ApplicationAuthenticationInfo.UserName, adminConnection))
                    {
                        using (SqlCommand selectUsersCommand = new SqlCommand("exec sp_MSloginmappings @User", adminConnection))
                        {
                            selectUsersCommand.Parameters.Add("@User", SqlDbType.VarChar).Value = setupInfo.ApplicationAuthenticationInfo.UserName;
                            selectUsersCommand.CommandTimeout *= 4;
                            SqlDataAdapter adpt = new SqlDataAdapter(selectUsersCommand);
                            DataTable usersTable = new DataTable();
                            adpt.Fill(usersTable);
                            if (usersTable.Rows.Count == 0)
                            {
                                dropLoginMethod(adminConnection);
                            }
                            else if (usersTable.Rows.Count == 1 && usersTable.Rows[0]["DBName"].ToString().Equals(setupInfo.DatabaseName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                using (var adminConnectionWithDB = new SqlConnection(setupInfo.CreateConnectionString(true, true)))
                                {
                                    adminConnectionWithDB.Open();
                                    if (!setupInfo.IsDatabaseRoleMember("db_owner", setupInfo.ApplicationAuthenticationInfo.UserName, adminConnectionWithDB))
                                    {
                                        using (var dropUserCommand = new SqlCommand(string.Format("DROP USER [{0}]", setupInfo.ApplicationAuthenticationInfo.UserName), adminConnectionWithDB))
                                        {
                                            dropUserCommand.ExecuteNonQuery();
                                        }
                                        dropLoginMethod(adminConnection);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
