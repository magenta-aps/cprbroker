using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace CPRBroker.SetupDatabase
{
    /// <summary>
    /// Contains the database information that are gathered from the user and used throughout the application
    /// </summary>
    public class SetupInfo
    {
        /// <summary>
        /// Contains database login information
        /// </summary>
        public class AuthenticationInfo
        {
            public bool IntegratedSecurity = true;
            public string UserName = "";
            public string Password = "";
        }

        public string ServerName = "";
        public string DatabaseName = "";
        public bool ApplicationAuthenticationSameAsAdmin = true;

        public readonly AuthenticationInfo AdminAuthenticationInfo = new AuthenticationInfo();
        public readonly AuthenticationInfo ApplicationAuthenticationInfo = new AuthenticationInfo();


        public SetupInfo()
        {
            ApplicationAuthenticationInfo.IntegratedSecurity = false;
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
        /// <returns>The new SetupInfo object</returns>
        public static SetupInfo FromConnectionString(string connectionString)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            SetupInfo ret = new SetupInfo();
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
            if (ApplicationAuthenticationSameAsAdmin)
            {
                if (DatabaseExists())
                {
                    return true;
                }
                else if (AdminConnectionHasAdminRights())
                {
                    return true;
                }
                else
                {
                    // Insufficient rights
                    message = Messages.AdminConnectionHasInsufficientRights;
                    return false;
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
                    // ensure that either app login not exist or has correct info
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
            return true;
        }
    }

}

