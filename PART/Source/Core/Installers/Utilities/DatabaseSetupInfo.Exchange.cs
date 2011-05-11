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
using CprBroker.Utilities;

namespace CprBroker.Installers
{
    /// <summary>
    /// Contains the database information that are gathered from the user and used throughout the application
    /// </summary>
    public partial class DatabaseSetupInfo
    {

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

        public static DatabaseSetupInfo FromSession(Session session)
        {
            DatabaseSetupInfo ret = new DatabaseSetupInfo();

            ret.ServerName = session.GetPropertyValue("DB_SERVERNAME");
            ret.DatabaseName = session.GetPropertyValue("DB_DATABASENAME");
            ret.UseExistingDatabase = bool.Parse(session.GetPropertyValue("DB_USEEXISTINGDATABASE"));

            ret.AdminAuthenticationInfo = new DatabaseSetupInfo.AuthenticationInfo();
            ret.AdminAuthenticationInfo.IntegratedSecurity = session.GetPropertyValue("DB_ADMININTEGRATEDSECURITY") == "True";
            if (!ret.AdminAuthenticationInfo.IntegratedSecurity)
            {
                ret.AdminAuthenticationInfo.UserName = session.GetPropertyValue("DB_ADMINUSERNAME");
                ret.AdminAuthenticationInfo.Password = session.GetPropertyValue("DB_ADMINPASSWORD");
            }

            ret.ApplicationAuthenticationSameAsAdmin = !string.IsNullOrEmpty(session.GetPropertyValue("DB_APPSAMEASADMIN"));
            if (!ret.ApplicationAuthenticationSameAsAdmin)
            {
                ret.ApplicationAuthenticationInfo = new DatabaseSetupInfo.AuthenticationInfo();
                ret.ApplicationAuthenticationInfo.IntegratedSecurity = session.GetPropertyValue("DB_APPINTEGRATEDSECURITY") == "True";
                if (!ret.ApplicationAuthenticationInfo.IntegratedSecurity)
                {
                    ret.ApplicationAuthenticationInfo.UserName = session.GetPropertyValue("DB_APPUSERNAME");
                    ret.ApplicationAuthenticationInfo.Password = session.GetPropertyValue("DB_APPPASSWORD");
                }
            }
            return ret;
        }

        public void CopyToSession(Session session)
        {
            session.SetPropertyValue("DB_SERVERNAME", this.ServerName);
            session.SetPropertyValue("DB_DATABASENAME", this.DatabaseName);
            session.SetPropertyValue("DB_USEEXISTINGDATABASE", this.UseExistingDatabase.ToString());

            if (this.AdminAuthenticationInfo != null)
            {
                session.SetPropertyValue("DB_ADMININTEGRATEDSECURITY", this.AdminAuthenticationInfo.IntegratedSecurity.ToString());
                session.SetPropertyValue("DB_ADMINUSERNAME", this.AdminAuthenticationInfo.UserName);
                session.SetPropertyValue("DB_ADMINPASSWORD", this.AdminAuthenticationInfo.Password);
            }

            session.SetPropertyValue("DB_APPSAMEASADMIN", this.ApplicationAuthenticationSameAsAdmin ? "True" : "");

            if (this.ApplicationAuthenticationInfo != null)
            {
                session.SetPropertyValue("DB_APPINTEGRATEDSECURITY", this.ApplicationAuthenticationInfo.IntegratedSecurity.ToString());
                session.SetPropertyValue("DB_APPUSERNAME", this.ApplicationAuthenticationInfo.UserName);
                session.SetPropertyValue("DB_APPPASSWORD", this.ApplicationAuthenticationInfo.Password);
            }
        }

    }

}

