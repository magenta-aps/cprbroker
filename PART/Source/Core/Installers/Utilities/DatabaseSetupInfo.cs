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
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Deployment.WindowsInstaller;

namespace CprBroker.Installers
{
    /// <summary>
    /// Contains the database information that are gathered from the user and used throughout the application
    /// </summary>
    [Serializable]
    public class DatabaseSetupInfo
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
        }

        public string ServerName = "";
        public string DatabaseName = "";
        public bool ApplicationAuthenticationSameAsAdmin = true;

        public AuthenticationInfo AdminAuthenticationInfo { get; set; }
        public AuthenticationInfo ApplicationAuthenticationInfo { get; set; }

        public bool DatabaseCreated;

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
        /// Creates a connection string from local members
        /// </summary>
        /// <param name="isAdmin">Whether to create an admin connection string or otherwise an application connection string</param>
        /// <param name="includeDatabase">Whether include database name as InitialCatalog</param>
        /// <returns></returns>
        public string CreateConnectionString(bool isAdmin, bool includeDatabase)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = ServerName;
            builder.InitialCatalog = includeDatabase ? DatabaseName : "";

            AuthenticationInfo authenticationInfo = isAdmin ? AdminAuthenticationInfo : EffectiveApplicationAuthenticationInfo;

            builder.IntegratedSecurity = authenticationInfo.IntegratedSecurity;
            if (!authenticationInfo.IntegratedSecurity)
            {
                builder.UserID = authenticationInfo.UserName;
                builder.Password = authenticationInfo.Password;
            }
            return builder.ToString();
        }

        /// <summary>
        /// Creates a new object from a connection string, copying the info to AdminAuthenticationInfo
        /// </summary>
        /// <param name="connectionString">The connection string to use</param>
        /// <returns>The new DatabaseSetupInfo object</returns>
        public static DatabaseSetupInfo FromConnectionString(string connectionString)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            DatabaseSetupInfo ret = new DatabaseSetupInfo();
            ret.ServerName = builder.DataSource;
            ret.DatabaseName = builder.InitialCatalog;
            ret.AdminAuthenticationInfo.IntegratedSecurity = builder.IntegratedSecurity;
            if (!ret.AdminAuthenticationInfo.IntegratedSecurity)
            {
                ret.AdminAuthenticationInfo.UserName = builder.UserID;
                ret.AdminAuthenticationInfo.Password = builder.Password;
            }
            return ret;
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
        private bool AdminConnectionHasAdminRights()
        {
            bool ret = false;
            try
            {
                string adminConnectionString = CreateConnectionString(true, false);
                ServerConnection dbServerConnection = new ServerConnection(new SqlConnection(adminConnectionString));

                if ((((int)dbServerConnection.FixedServerRoles & (int)FixedServerRoles.DBCreator) > 0)
                    || (((int)dbServerConnection.FixedServerRoles & (int)FixedServerRoles.SysAdmin) > 0))
                {
                    ret = true;
                }
            }
            catch (Exception)
            {

            }
            return ret;
        }

        /// <summary>
        /// Checks whether the database specified already exists
        /// </summary>
        /// <returns>True is database exists, false otherwise</returns>
        private bool DatabaseExists()
        {
            string adminConnectionString = CreateConnectionString(true, false);
            ServerConnection dbServerConnection = new ServerConnection(new SqlConnection(adminConnectionString));
            Server dbServer = new Server(dbServerConnection);
            return dbServer.Databases.Contains(this.DatabaseName);
        }

        /// <summary>
        /// Checks wheteher the login specified as application login exists
        /// </summary>
        /// <returns>True if login exists or windows authentication, false otherwise</returns>
        private bool AppLoginExists()
        {
            string adminConnectionString = CreateConnectionString(true, false);
            ServerConnection dbServerConnection = new ServerConnection(new SqlConnection(adminConnectionString));
            Server dbServer = new Server(dbServerConnection);
            if (!ApplicationAuthenticationInfo.IntegratedSecurity)
            {
                return dbServer.Logins.Contains(ApplicationAuthenticationInfo.UserName);
            }
            else
            {
                return true;
            }
        }

        public bool Validate(ref string message)
        {
            if (!TryOpenConnection(CreateConnectionString(true, false)))
            {
                // Admin connection failed
                message = Messages.AdminConnectionFailed;
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
                        // ensure that either app login either does not exist or has correct info
                        if (AppLoginExists())
                        {
                            if (!TryOpenConnection(CreateConnectionString(false, false)))
                            {
                                message = Messages.ApplicationConnectionFailed;
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

        public void ClearSensitiveDate()
        {
            ApplicationAuthenticationInfo = new AuthenticationInfo();
        }

        public static DatabaseSetupInfo FromSession(Session session)
        {
            DatabaseSetupInfo ret = new DatabaseSetupInfo();
            Func<string, string> propGetter = null;

            if (session.GetMode(InstallRunMode.Scheduled) || session.GetMode(InstallRunMode.Rollback))
            {
                 propGetter= (key) => session.CustomActionData[key];
            }
            else
            {
                propGetter= (key) => session[key];
            }

            ret.ServerName = propGetter("DB_SERVERNAME");
            ret.DatabaseName = propGetter("DB_DATABASENAME");

            ret.AdminAuthenticationInfo = new DatabaseSetupInfo.AuthenticationInfo();
            ret.AdminAuthenticationInfo.IntegratedSecurity = propGetter("DB_ADMININTEGRATEDSECURITY") == "SSPI";
            if (!ret.AdminAuthenticationInfo.IntegratedSecurity)
            {
                ret.AdminAuthenticationInfo.UserName = propGetter("DB_ADMINUSERNAME");
                ret.AdminAuthenticationInfo.Password = propGetter("DB_ADMINPASSWORD");
            }

            ret.ApplicationAuthenticationSameAsAdmin = !string.IsNullOrEmpty(propGetter("DB_APPSAMEASADMIN"));
            if (!ret.ApplicationAuthenticationSameAsAdmin)
            {
                ret.ApplicationAuthenticationInfo = new DatabaseSetupInfo.AuthenticationInfo();
                ret.ApplicationAuthenticationInfo.IntegratedSecurity = propGetter("DB_APPINTEGRATEDSECURITY") == "SSPI";
                if (!ret.ApplicationAuthenticationInfo.IntegratedSecurity)
                {
                    ret.ApplicationAuthenticationInfo.UserName = propGetter("DB_APPUSERNAME");
                    ret.ApplicationAuthenticationInfo.Password = propGetter("DB_APPPASSWORD");
                }
            }
            return ret;
        }

    }

}

