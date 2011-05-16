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

                string adminConnectionStringWithDb = setupInfo.CreateConnectionString(true, true);
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
                InsertLookups(adminConnectionStringWithDb, lookupDataArray);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a new user in the database if needed
        /// </summary>
        private static void CreateDatabaseUser(DatabaseSetupInfo setupInfo)
        {
            if (!setupInfo.ApplicationAuthenticationSameAsAdmin)
            {
                string userName;
                Func<SqlConnection, SqlCommand> createLoginMethod;

                if (setupInfo.EffectiveApplicationAuthenticationInfo.IntegratedSecurity)
                {
                    userName = @"NT AUTHORITY\NETWORK SERVICE";
                    createLoginMethod = (connection) => new SqlCommand(string.Format("CREATE LOGIN [{0}] FROM WINDOWS", userName), connection);// login.Create();
                }
                else
                {
                    userName = setupInfo.EffectiveApplicationAuthenticationInfo.UserName;
                    createLoginMethod = (connection) =>
                    {
                        var ret = new SqlCommand(string.Format("CREATE LOGIN [{0}] WITH PASSWORD = @Password", userName), connection);
                        ret.Parameters.Add("@Password", System.Data.SqlDbType.VarChar).Value = setupInfo.EffectiveApplicationAuthenticationInfo.Password;
                        return ret;
                    };
                }

                using (SqlConnection adminConnection = new SqlConnection(setupInfo.CreateConnectionString(true, false)))
                {
                    adminConnection.Open();
                    using (SqlConnection adminConnectionWithDb = new SqlConnection(setupInfo.CreateConnectionString(true, true)))
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

                                SqlCommand addRoleCommand = new SqlCommand("sp_addrolemember", adminConnectionWithDb);
                                addRoleCommand.CommandType = System.Data.CommandType.StoredProcedure;
                                addRoleCommand.Parameters.Add("@rolename", System.Data.SqlDbType.VarChar).Value = "db_owner";
                                addRoleCommand.Parameters.Add("@membername", System.Data.SqlDbType.VarChar).Value = userName;
                                addRoleCommand.ExecuteNonQuery();
                            }
                        }
                    }
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
