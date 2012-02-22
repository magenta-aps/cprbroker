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

        public void CopyToCurrentDetails(Session session)
        {
            session.SetPropertyValue(FeaturePropertyName, this.FeatureName);

            session.SetPropertyValue(Constants.ServerName, this.ServerName);
            session.SetPropertyValue(Constants.DatabaseName, this.DatabaseName);
            session.SetPropertyValue(Constants.UseExistingDatabase, this.UseExistingDatabase.ToString());

            session.SetPropertyValue(Constants.EncryptionKey, this.EncryptionKey);
            session.SetPropertyValue(Constants.EncryptionKeyEnabled, this.EncryptionKeyEnabled.ToString());

            session.SetPropertyValue(Constants.Domain, this.Domain);
            session.SetPropertyValue(Constants.DomainEnabled, this.DomainEnabled.ToString());

            if (this.AdminAuthenticationInfo != null)
            {
                session.SetPropertyValue(Constants.AdminIntegratedSecurity, this.AdminAuthenticationInfo.IntegratedSecurity.ToString());
                session.SetPropertyValue(Constants.AdminUsername, this.AdminAuthenticationInfo.UserName);
                session.SetPropertyValue(Constants.AdminPassword, this.AdminAuthenticationInfo.Password);
            }

            session.SetPropertyValue(Constants.AppIntegratedSecurityAllowed, this.ApplicationIntegratedSecurityAllowed.ToString());
            session.SetPropertyValue(Constants.AppSameAsAdmin, this.ApplicationAuthenticationSameAsAdmin ? "True" : "");

            if (this.ApplicationAuthenticationInfo != null)
            {
                session.SetPropertyValue(Constants.AppIntegratedSecurity, this.ApplicationAuthenticationInfo.IntegratedSecurity.ToString());
                session.SetPropertyValue(Constants.AppUsername, this.ApplicationAuthenticationInfo.UserName);
                session.SetPropertyValue(Constants.AppPassword, this.ApplicationAuthenticationInfo.Password);
            }
        }

        public static string[] GetDatabaseFeatureNames(Session session)
        {
            return session.GetPropertyValue(AllFeaturesPropertyName).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] GetSuggestedDatabaseNames(Session session)
        {
            return session.GetPropertyValue(SuggestedDatabaseNamesPropertyName).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static readonly string FeaturePropertyName = "DB_Feature";
        public static readonly string AllFeaturesPropertyName = "DB_FeatureNames";
        public static readonly string AllInfoPropertyName = "DB_ALL";
        public static readonly string SuggestedDatabaseNamesPropertyName = "DB_SuggestedDatabaseNames";

        private static DatabaseSetupInfo[] DeserializeAllFeatures(Session session)
        {
            var allPropVal = session.GetPropertyValue(AllInfoPropertyName);
            if (allPropVal == "-")
                allPropVal = "";
            var allInfo = CprBroker.Utilities.Strings.Deserialize<DatabaseSetupInfo[]>(allPropVal);
            if (allInfo == null)
                allInfo = new DatabaseSetupInfo[0];
            return allInfo;
        }

        private static void SerializeAllFeatures(Session session, DatabaseSetupInfo[] allInfo)
        {
            var allPropVal = CprBroker.Utilities.Strings.SerializeObject(allInfo);
            session.SetPropertyValue(AllInfoPropertyName, allPropVal);
        }

        public static void AddFeatureDetails(Session session, DatabaseSetupInfo databaseSetupInfo)
        {
            var allInfo = DeserializeAllFeatures(session);
            var index = Array.FindIndex<DatabaseSetupInfo>(allInfo, inf => inf.FeatureName == databaseSetupInfo.FeatureName);
            if (index != -1)
            {
                allInfo[index] = databaseSetupInfo;
            }
            else
            {
                var list = new List<DatabaseSetupInfo>(allInfo);
                list.Add(databaseSetupInfo);
                allInfo = list.ToArray();
            }
            SerializeAllFeatures(session, allInfo);
        }

        public static DatabaseSetupInfo CreateFromCurrentDetails(Session session)
        {
            return CreateFromCurrentDetails(session, "");
        }

        public static DatabaseSetupInfo CreateFromCurrentDetails(Session session, string featureName)
        {
            return CreateFromCurrentDetails(session, featureName, false);
        }

        public static DatabaseSetupInfo CreateFromCurrentDetails(Session session, string featureName, bool tryWithoutFeature)
        {
            DatabaseSetupInfo ret = new DatabaseSetupInfo();

            if (string.IsNullOrEmpty(featureName))
            {
                ret.FeatureName = session.GetPropertyValue(FeaturePropertyName);
            }
            else
            {
                ret.FeatureName = featureName;
                session.SetPropertyValue(FeaturePropertyName, ret.FeatureName);
            }

            ret.ServerName = session.GetPropertyValue(Constants.ServerName, featureName, tryWithoutFeature);
            ret.DatabaseName = session.GetPropertyValue(Constants.DatabaseName, featureName, tryWithoutFeature);
            ret.UseExistingDatabase = session.GetBooleanPropertyValue(Constants.UseExistingDatabase, featureName, tryWithoutFeature);

            ret.EncryptionKey = session.GetPropertyValue(Constants.EncryptionKey, featureName, tryWithoutFeature);
            ret.EncryptionKeyEnabled = session.GetBooleanPropertyValue(Constants.EncryptionKeyEnabled, featureName, tryWithoutFeature);

            ret.Domain = session.GetPropertyValue(Constants.Domain, featureName, tryWithoutFeature);
            ret.DomainEnabled = session.GetBooleanPropertyValue(Constants.DomainEnabled, featureName, tryWithoutFeature);

            ret.AdminAuthenticationInfo = new DatabaseSetupInfo.AuthenticationInfo();
            ret.AdminAuthenticationInfo.IntegratedSecurity = session.GetBooleanPropertyValue(Constants.AdminIntegratedSecurity, featureName, tryWithoutFeature);
            if (!ret.AdminAuthenticationInfo.IntegratedSecurity)
            {
                ret.AdminAuthenticationInfo.UserName = session.GetPropertyValue(Constants.AdminUsername, featureName, tryWithoutFeature);
                ret.AdminAuthenticationInfo.Password = session.GetPropertyValue(Constants.AdminPassword, featureName, tryWithoutFeature);
            }

            ret.ApplicationIntegratedSecurityAllowed = session.GetBooleanPropertyValue(Constants.AppIntegratedSecurityAllowed, featureName, tryWithoutFeature);

            ret.ApplicationAuthenticationSameAsAdmin = session.GetBooleanPropertyValue(Constants.AppSameAsAdmin, featureName, tryWithoutFeature);
            if (!ret.ApplicationAuthenticationSameAsAdmin)
            {
                ret.ApplicationAuthenticationInfo = new DatabaseSetupInfo.AuthenticationInfo();
                ret.ApplicationAuthenticationInfo.IntegratedSecurity = session.GetBooleanPropertyValue(Constants.AppIntegratedSecurity, featureName, tryWithoutFeature);
                if (!ret.ApplicationAuthenticationInfo.IntegratedSecurity)
                {
                    ret.ApplicationAuthenticationInfo.UserName = session.GetPropertyValue(Constants.AppUsername, featureName, tryWithoutFeature);
                    ret.ApplicationAuthenticationInfo.Password = session.GetPropertyValue(Constants.AppPassword, featureName, tryWithoutFeature);
                }
            }
            return ret;
        }

        public static DatabaseSetupInfo CreateFromFeature(Session session, string featureName)
        {
            var allPropValue = session.GetPropertyValue(AllInfoPropertyName);
            var allInfo = DeserializeAllFeatures(session);
            return allInfo.Where(inf => inf.FeatureName == featureName).FirstOrDefault();
        }

        public static CustomActionData GetCustomActionData(Session session)
        {
            var commponProps = BaseSetupInfo.GetCustomActionData(session);

            var dbProps = new CustomActionData();
            dbProps[DatabaseSetupInfo.AllInfoPropertyName] = session.GetPropertyValue(DatabaseSetupInfo.AllInfoPropertyName);
            dbProps[DatabaseSetupInfo.FeaturePropertyName] = session.GetPropertyValue(DatabaseSetupInfo.FeaturePropertyName);
            dbProps[DatabaseSetupInfo.AllFeaturesPropertyName] = session.GetPropertyValue(DatabaseSetupInfo.AllFeaturesPropertyName);
            commponProps.Merge(dbProps);

            return commponProps;
        }

        public static void SetSuggestedPropertyValues(Session session, string featureName)
        {
            BaseSetupInfo.SetSuggestedPropertyValues(
                session,
                featureName,
                DatabaseSetupInfo.GetDatabaseFeatureNames(session),
                DatabaseSetupInfo.GetSuggestedDatabaseNames(session),
                new string[] { DatabaseSetupInfo.Constants.DatabaseName, DatabaseSetupInfo.Constants.AppUsername }
            );
        }

    }

}

