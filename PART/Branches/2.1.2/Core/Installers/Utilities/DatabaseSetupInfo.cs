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
using System.Data.SqlClient;
using Microsoft.Deployment.WindowsInstaller;
using System.Text.RegularExpressions;

namespace CprBroker.Installers
{
    /// <summary>
    /// Contains the database information that are gathered from the user and used throughout the application
    /// </summary>
    [Serializable]
    public partial class DatabaseSetupInfo : BaseSetupInfo
    {
        /// <summary>
        /// Contains database login information
        /// </summary>
        [Serializable]
        public class AuthenticationInfo
        {
            public bool IntegratedSecurity = true;
            public string UserName = "";
            public string Password = "";

            public string EffectiveUserName
            {
                get { return IntegratedSecurity ? @"NT AUTHORITY\NETWORK SERVICE" : UserName; }
            }
        }

        public string FeatureName = "";
        public string ServerName = "";
        public string DatabaseName = "";
        public bool UseExistingDatabase = false;
        public bool ApplicationAuthenticationSameAsAdmin = true;
        public bool ApplicationIntegratedSecurityAllowed = true;

        public AuthenticationInfo AdminAuthenticationInfo { get; set; }
        public AuthenticationInfo ApplicationAuthenticationInfo { get; set; }

        [Obsolete]
        public bool DatabaseCreated;

        public bool EncryptionKeyEnabled = false;
        public string EncryptionKey;

        public bool DomainEnabled = false;
        public string Domain;


        public DatabaseSetupInfo()
        {
            AdminAuthenticationInfo = new AuthenticationInfo();
            ApplicationAuthenticationInfo = new AuthenticationInfo()
            {
                IntegratedSecurity = false
            };
        }

        /// <summary>
        /// Returns the application authentication info if different from admin info, otherwise returns the admin info
        /// </summary>
        public AuthenticationInfo EffectiveApplicationAuthenticationInfo
        {
            get
            {
                if (ApplicationAuthenticationSameAsAdmin)
                {
                    return AdminAuthenticationInfo;
                }
                else
                {
                    return ApplicationAuthenticationInfo;
                }

            }
        }

        /// <summary>
        /// Tries to open a connection with the specified connection string
        /// </summary>
        /// <param name="connectionString">Connection string to use</param>
        /// <returns>True if a connection could be opened, false otherwise</returns>
        private bool TryOpenConnection(string connectionString)
        {
            bool ret = false;
            var connectionBuilder = new SqlConnectionStringBuilder(connectionString) { ConnectTimeout = 5 };
            var conn = new SqlConnection(connectionBuilder.ConnectionString);
            try
            {
                conn.Open();
                ret = true;
            }
            catch
            {

            }
            finally
            {
                conn.Close();
            }
            return ret;
        }

        /// <summary>
        /// Checks whether the connection specified as admin connection has SysAdmin & DbCreator rights
        /// </summary>
        /// <returns></returns>
        internal bool AdminConnectionHasAdminRights()
        {
            bool ret = false;
            try
            {
                string adminConnectionString = CreateConnectionString(true, false);
                using (SqlConnection adminConnection = new SqlConnection(adminConnectionString))
                {
                    adminConnection.Open();
                    return IsServerRoleMember("sysadmin", null, adminConnection);
                }
            }
            catch (Exception)
            {

            }
            return ret;
        }

        internal bool AdminConnectionHasDboRights()
        {
            bool ret = false;
            try
            {
                string adminConnectionString = CreateConnectionString(true, false);
                using (SqlConnection adminConnection = new SqlConnection(adminConnectionString))
                {
                    adminConnection.Open();
                    return IsDatabaseRoleMember("db_owner", AdminAuthenticationInfo.EffectiveUserName, adminConnection);
                }
            }
            catch (Exception)
            {
                
            }
            return ret;
        }

        internal bool IsServerRoleMember(string role, string user, SqlConnection connection)
        {
            string sql = string.Format("SELECT ISNULL(IS_SRVROLEMEMBER ('{0}' ", role);
            if (!string.IsNullOrEmpty(user))
            {
                sql += string.Format(", '{0}'", user);
            }
            sql += "),0)";

            bool connectionOpen = connection.State == System.Data.ConnectionState.Open;
            int count;
            using (SqlCommand selectCommand = new SqlCommand(sql, connection))
            {
                if (!connectionOpen)
                    connection.Open();
                count = (int)selectCommand.ExecuteScalar();
                if (!connectionOpen)
                    connection.Close();
            }
            return count > 0;
        }

        internal bool IsDatabaseRoleMember(string role, string user, SqlConnection connection)
        {
            string sql = ""
                + " select count(*) from"
                + " sys.database_role_members as drm "
                + " inner join "
                + " ("
                + " select principal_id from sys.database_principals where type='R' and name=@Role"
                + " ) as databaseRoles "
                + " on drm.role_principal_id = databaseRoles.principal_id"
                + " inner join"
                + " ("
                + "  select dp0.principal_id from sys.database_principals  dp0 inner join sys.server_principals sp0 on dp0.sid = sp0.sid"
                + "  where dp0.type='S' and sp0.name = @Login"
                + " ) as databaseUsers "
                + " on drm.member_principal_id = databaseUsers.principal_id";

            bool connectionOpen = connection.State == System.Data.ConnectionState.Open;
            int count;
            using (SqlCommand selectCommand = new SqlCommand(sql, connection))
            {
                if (!connectionOpen)
                    connection.Open();
                selectCommand.Parameters.Add("@Role", System.Data.SqlDbType.VarChar).Value = role;
                selectCommand.Parameters.Add("@Login", System.Data.SqlDbType.VarChar).Value = user;
                count = (int)selectCommand.ExecuteScalar();
                if (!connectionOpen)
                    connection.Close();
            }
            return count > 0;
        }

        /// <summary>
        /// Checks whether the database specified already exists
        /// </summary>
        /// <returns>True is database exists, false otherwise</returns>
        public bool DatabaseExists()
        {
            string adminConnectionString = CreateConnectionString(true, false);
            using (SqlConnection adminConnection = new SqlConnection(adminConnectionString))
            {
                adminConnection.Open();
                using (SqlCommand selectCommand = new SqlCommand("SELECT COUNT (*) FROM sys.databases WHERE name=@Name", adminConnection))
                {
                    selectCommand.Parameters.Add("@Name", System.Data.SqlDbType.VarChar).Value = this.DatabaseName; ;
                    int count = (int)selectCommand.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Checks wheteher the login specified as application login exists
        /// </summary>
        /// <returns>True if login exists or windows authentication, false otherwise</returns>
        private bool AppLoginExists()
        {
            if (!ApplicationAuthenticationInfo.IntegratedSecurity)
            {
                string adminConnectionString = CreateConnectionString(true, false);
                using (SqlConnection adminConnection = new SqlConnection(adminConnectionString))
                {
                    adminConnection.Open();
                    using (SqlCommand selectCommand = new SqlCommand("SELECT COUNT (*) FROM sys.sql_logins WHERE name=@Name", adminConnection))
                    {
                        selectCommand.Parameters.Add("@Name", System.Data.SqlDbType.VarChar).Value = ApplicationAuthenticationInfo.UserName;
                        int count = (int)selectCommand.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            else
            {
                return true;
            }
        }

        private bool IsIntegratedSecurityOnly()
        {
            string adminConnectionString = CreateConnectionString(true, false);
            using (SqlConnection adminConnection = new SqlConnection(adminConnectionString))
            {
                adminConnection.Open();
                using (SqlCommand selectCommand = new SqlCommand("SELECT SERVERPROPERTY('IsIntegratedSecurityOnly')", adminConnection))
                {
                    int isIntegratedSecurityOnly = (int)selectCommand.ExecuteScalar();
                    return isIntegratedSecurityOnly > 0;
                }
            }
        }

        protected virtual bool ValidateEncryptionKey(ref string message)
        {
            if (!EncryptionKeyEnabled)
            {
                return true;
            }
            if (this.EncryptionKey.Length < 6)
            {
                message = "Encryption key is too short";
                return false;
            }
            if (!Regex.Match(EncryptionKey, @"\d+").Success)
            {
                message = "Encryption key should contain at least one digit";
                return false;
            }
            if (!Regex.Match(EncryptionKey, @"[a-z]+").Success)
            {
                message = "Encryption key should contain at least one lowercase character";
                return false;
            }
            if (!Regex.Match(EncryptionKey, @"[A-Z]+").Success)
            {
                message = "Encryption key should contain at least one uppercase character";
                return false;
            }
            if (!Regex.Match(EncryptionKey, @"\W+").Success)
            {
                message = "Encryption key should contain at least one non alphanumeric character";
                return false;
            }
            return true;
        }
        public bool Validate(ref string message)
        {
            if (!ValidateEncryptionKey(ref message))
            {
                return false;
            }
            if (!TryOpenConnection(CreateConnectionString(true, false)))
            {
                // Admin connection failed
                message = Messages.AdminConnectionFailed;
                return false;
            }
            if (!ApplicationIntegratedSecurityAllowed && EffectiveApplicationAuthenticationInfo.IntegratedSecurity)
            {
                message = Messages.WindowsAuthenticationNotAllowed;
                return false;
            }
            // From now on, we have a valid connection string (without DB)
            if (ApplicationAuthenticationSameAsAdmin)
            {
                if (DatabaseExists())
                {
                    if (TryOpenConnection(CreateConnectionString(true, true)))//TODO: modify this condition to look for db_owner role too
                    {
                        // There is nothing more to do because there is no need to create a database or a user
                        return true;
                    }
                    else
                    {
                        // This means that Admin connection could not be opened with this particular database
                        message = Messages.AdminConnectionHasInsufficientRights;
                        return false;
                    }
                }
                else
                {
                    if (AdminConnectionHasAdminRights())
                    {
                        // Admin connection works and can create a new database
                        return true;
                    }
                    else
                    {
                        // Insufficient rights
                        message = Messages.AdminConnectionHasInsufficientRights;
                        return false;
                    }
                }
            }
            else
            {
                if (!AdminConnectionHasAdminRights())
                {
                    var appConnectionWithDB = CreateConnectionString(false, true);
                    if (TryOpenConnection(appConnectionWithDB))
                    {
                        if (!ApplicationAuthenticationInfo.IntegratedSecurity)
                        {
                            var connection = new SqlConnection(appConnectionWithDB);
                            if (IsDatabaseRoleMember("db_owner", ApplicationAuthenticationInfo.EffectiveUserName, connection))
                            {
                                // A special case when the databse exists with a db_owner user and a sysadmin login cannot be used (but not needed)
                                return true;
                            }
                        }
                    }

                    // Insufficient rights
                    message = Messages.AdminConnectionHasInsufficientRights;
                    return false;
                }
                else
                {
                    if (ApplicationAuthenticationInfo.IntegratedSecurity)
                    {
                        // always will be able to create a login and user for Network Service account
                        return true;
                    }
                    else
                    {
                        if (IsIntegratedSecurityOnly())
                        {
                            message = Messages.SqlAuthenticationNotAllowedOnServer;
                            return false;
                        }
                        // ensure that either app login either does not exist or has correct info
                        else if (AppLoginExists())
                        {
                            if (
                                !TryOpenConnection(CreateConnectionString(false, false))
                                && !TryOpenConnection(CreateConnectionStringToMasterDatabase(false))
                                )
                            {
                                message = Messages.ApplicationConnectionFailedCheckUserNameAndPassword;
                                return false; // Incorrect password
                            }
                        }
                        else // need to create new login
                        {
                            return true;// can always create a login
                        }
                    }

                }
            }
            return true;
        }

        public bool ValidateDatabaseExistence(bool databaseShouldBeNew, Func<bool> askOverwrite, ref string message)
        {
            if (databaseShouldBeNew)
            {
                if (!this.DatabaseExists()) // Normal case
                {
                    return true;
                }
                else if (askOverwrite != null && askOverwrite()) // Database exists and won't be created
                {
                    this.UseExistingDatabase = true;
                    return true;
                }
                else  // Database exists and user should change its name
                {
                    message = Messages.DatabaseAlreadyExists;
                    return false;
                }
            }
            else
            {
                if (this.DatabaseExists())
                {
                    return true;
                }
                else
                {
                    message = Messages.DatabaseDoesNotExist;
                    return false;
                }
            }
        }

        public void ClearSensitiveDate()
        {
            ApplicationAuthenticationInfo = new AuthenticationInfo();
        }

    }

}

