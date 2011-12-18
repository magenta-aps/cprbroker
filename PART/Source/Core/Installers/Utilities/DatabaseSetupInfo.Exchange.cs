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
using CprBroker.Utilities;

namespace CprBroker.Installers
{
    /// <summary>
    /// Contains the database information that are gathered from the user and used throughout the application
    /// </summary>
    public partial class DatabaseSetupInfo
    {
        public string CreateConnectionStringToMasterDatabase(bool isAdmin)
        {
            string ret = CreateConnectionString(isAdmin, true);
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ret);
            builder.InitialCatalog = "master";
            return builder.ToString();
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

        public static DatabaseSetupInfo FromSession(Session session)
        {
            DatabaseSetupInfo ret = new DatabaseSetupInfo();

            ret.ServerName = session.GetPropertyValue("DB_SERVERNAME");
            ret.DatabaseName = session.GetPropertyValue("DB_DATABASENAME");
            ret.UseExistingDatabase = bool.Parse(session.GetPropertyValue("DB_USEEXISTINGDATABASE"));

            ret.EncryptionKey = session.GetPropertyValue("DB_ENCRYPTIONKEY");
            ret.EncryptionKeyEnabled = bool.Parse(session.GetPropertyValue("DB_ENCRYPTIONKEYENABLED"));

            ret.Domain = session.GetPropertyValue("DB_DOMAIN");
            ret.DomainEnabled = bool.Parse(session.GetPropertyValue("DB_DOMAINENABLED"));

            ret.AdminAuthenticationInfo = new DatabaseSetupInfo.AuthenticationInfo();
            ret.AdminAuthenticationInfo.IntegratedSecurity = session.GetPropertyValue("DB_ADMININTEGRATEDSECURITY") == "True";
            if (!ret.AdminAuthenticationInfo.IntegratedSecurity)
            {
                ret.AdminAuthenticationInfo.UserName = session.GetPropertyValue("DB_ADMINUSERNAME");
                ret.AdminAuthenticationInfo.Password = session.GetPropertyValue("DB_ADMINPASSWORD");
            }

            ret.ApplicationIntegratedSecurityAllowed = bool.Parse(session.GetPropertyValue("DB_APPINTEGRATEDSECURITYALLOWED"));

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

            session.SetPropertyValue("DB_ENCRYPTIONKEY", this.EncryptionKey);
            session.SetPropertyValue("DB_ENCRYPTIONKEYENABLED", this.EncryptionKeyEnabled.ToString());

            session.SetPropertyValue("DB_DOMAIN", this.Domain);
            session.SetPropertyValue("DB_DOMAINENABLED", this.DomainEnabled.ToString());

            if (this.AdminAuthenticationInfo != null)
            {
                session.SetPropertyValue("DB_ADMININTEGRATEDSECURITY", this.AdminAuthenticationInfo.IntegratedSecurity.ToString());
                session.SetPropertyValue("DB_ADMINUSERNAME", this.AdminAuthenticationInfo.UserName);
                session.SetPropertyValue("DB_ADMINPASSWORD", this.AdminAuthenticationInfo.Password);
            }

            session.SetPropertyValue("DB_APPINTEGRATEDSECURITYALLOWED", this.ApplicationIntegratedSecurityAllowed.ToString());
            session.SetPropertyValue("DB_APPSAMEASADMIN", this.ApplicationAuthenticationSameAsAdmin ? "True" : "");

            if (this.ApplicationAuthenticationInfo != null)
            {
                session.SetPropertyValue("DB_APPINTEGRATEDSECURITY", this.ApplicationAuthenticationInfo.IntegratedSecurity.ToString());
                session.SetPropertyValue("DB_APPUSERNAME", this.ApplicationAuthenticationInfo.UserName);
                session.SetPropertyValue("DB_APPPASSWORD", this.ApplicationAuthenticationInfo.Password);
            }
        }

        public static string[] GetDatabaseFeatureNames(Session session)
        {
            return session.GetPropertyValue("DB_FeatureNames").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string GetDatabaseFeatureName(Session session)
        {
            return session.GetPropertyValue("DB_Feature");
        }

        public static void SetDatabaseFeatureName(Session session, string value)
        {
            session.SetPropertyValue("DB_Feature", value);
        }

        public static string GetDatabaseFeaturePropertyName(Session session)
        {
            return GetDatabaseFeaturePropertyName(session.GetPropertyValue("DB_Feature"));
        }

        public static string GetDatabaseFeaturePropertyName(string featureName)
        {
            return "DB_ALLPROPERTIES_" + featureName;
        }

        public static void ExtractDatabaseFeatureProperties(Session session, string featureName)
        {
            var allPropName = GetDatabaseFeaturePropertyName(featureName);
            var allPropVal = session.GetPropertyValue(allPropName);
            allPropVal = allPropVal.Replace(DBPropSeparator, ";");
            string[] propArr = allPropVal.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var prop in propArr)
            {
                var arr = prop.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                var propName = arr[0];
                var propVal = arr.Length == 1 ? "" : arr[1];
                session.SetPropertyValue(propName, propVal);
            }
        }

        public static void AggregateFeatureProperties(Session session)
        {
            var propName = GetDatabaseFeaturePropertyName(session);
            var propVal = "DB_SERVERNAME=" + session.GetPropertyValue("DB_SERVERNAME") + ";" + "DB_DATABASENAME=" + session.GetPropertyValue("DB_DATABASENAME") + ";" + "DB_USEEXISTINGDATABASE=" + session.GetPropertyValue("DB_USEEXISTINGDATABASE") + ";" + "DB_ADMININTEGRATEDSECURITY=" + session.GetPropertyValue("DB_ADMININTEGRATEDSECURITY") + ";" + "DB_ADMINUSERNAME=" + session.GetPropertyValue("DB_ADMINUSERNAME") + ";" + "DB_ADMINPASSWORD=" + session.GetPropertyValue("DB_ADMINPASSWORD") + ";" + "DB_APPSAMEASADMIN=" + session.GetPropertyValue("DB_APPSAMEASADMIN") + ";" + "DB_APPINTEGRATEDSECURITY=" + session.GetPropertyValue("DB_APPINTEGRATEDSECURITY") + ";" + "DB_APPINTEGRATEDSECURITYALLOWED=" + session.GetPropertyValue("DB_APPINTEGRATEDSECURITYALLOWED") + ";" + "DB_APPUSERNAME=" + session.GetPropertyValue("DB_APPUSERNAME") + ";" + "DB_APPPASSWORD=" + session.GetPropertyValue("DB_APPPASSWORD") + ";" + "DB_ENCRYPTIONKEY=" + session.GetPropertyValue("DB_ENCRYPTIONKEY") + ";" + "DB_ENCRYPTIONKEYENABLED=" + session.GetPropertyValue("DB_ENCRYPTIONKEYENABLED") + ";" + "DB_DOMAIN=" + session.GetPropertyValue("DB_DOMAIN") + ";" + "DB_DOMAINENABLED=" + session.GetPropertyValue("DB_DOMAINENABLED") + ";" + "ProductName=" + session.GetPropertyValue("ProductName");
            session.SetPropertyValue(propName, propVal);
        }

        public static readonly string DBPropSeparator = ".......";

        public static string AggregateAllProperties(Session session)
        {
            var allValues = DatabaseSetupInfo.GetDatabaseFeatureNames(session)
                .Select((featureName) =>
                {
                    DatabaseSetupInfo.SetDatabaseFeatureName(session, featureName);
                    string propName = DatabaseSetupInfo.GetDatabaseFeaturePropertyName(session);
                    string propVal = session.GetPropertyValue(propName).Replace(";", DBPropSeparator);
                    return string.Format("{0}={1}", propName, propVal);
                }
                ).ToArray();
            string val = string.Format("DB_FeatureNames={0};{1}", session.GetPropertyValue("DB_FeatureNames"), string.Join(";", allValues));
            return val;
        }

    }

}

