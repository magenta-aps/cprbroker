using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Engine
{
    public enum DataProviderConfigPropertyInfoTypes
    {
        String,
        Integer,
        Boolean,
        Decimal
    }

    public class DataProviderConfigPropertyInfo
    {
        public string Name { get; set; }
        public bool Confidential { get; set; }
        public bool Required { get; set; }
        public DataProviderConfigPropertyInfoTypes Type { get; set; }

        public DataProviderConfigPropertyInfo()
        {
            Type = DataProviderConfigPropertyInfoTypes.String;
        }

        public static string GetValue(Dictionary<string, string> configuration, string key)
        {
            return GetValue(configuration, key, null);
        }

        public static string GetValue(Dictionary<string, string> configuration, string key, string defaultValue)
        {
            if (configuration.ContainsKey(key))
                return configuration[key];
            else
                return defaultValue;
        }

        public static T GetValue<T>(Dictionary<string, string> configuration, string key, T defaultValue, Func<string, T> converter)
        {
            var val = GetValue(configuration, key, null);
            if (string.IsNullOrEmpty(val))
                return defaultValue;
            else
                return converter(val);
        }

        public static bool GetBoolean(Dictionary<string, string> configuration, string key)
        {
            return GetValue<bool>(configuration, key, false, (s) => Boolean.Parse(s));
        }

        public static class Templates
        {
            public static DataProviderConfigPropertyInfo[] ConnectionStringKeys
            {
                get
                {
                    return new DataProviderConfigPropertyInfo[] {                     
                        new DataProviderConfigPropertyInfo(){Name="Data Source", Required=true, Confidential=false},
                        new DataProviderConfigPropertyInfo(){Name="Initial Catalog", Required=false, Confidential=false},
                        new DataProviderConfigPropertyInfo(){Name="User ID", Required=false, Confidential=false},
                        new DataProviderConfigPropertyInfo(){Name="Password", Required=false, Confidential=true},
                        new DataProviderConfigPropertyInfo(){Name="Integrated Security", Required=false, Confidential=false},
                        new DataProviderConfigPropertyInfo(){Name="Other Connection String", Required=false, Confidential=false},
                    };
                }
            }

            public static string GetConnectionString(Dictionary<string, string> configurationProperties)
            {
                string other = string.Format("{0}", configurationProperties["Other Connection String"]);
                string dataSource = string.Format("{0}", configurationProperties["Data Source"]);
                string initialCatalog = string.Format("{0}", configurationProperties["Initial Catalog"]);
                string userId = string.Format("{0}", configurationProperties["User ID"]);
                string password = string.Format("{0}", configurationProperties["Password"]);
                string integratedSecurity = string.Format("{0}", configurationProperties["Integrated Security"]);

                System.Data.SqlClient.SqlConnectionStringBuilder connectionBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(other);
                if (!string.IsNullOrEmpty(dataSource))
                {
                    connectionBuilder.DataSource = dataSource;
                }
                if (!string.IsNullOrEmpty(initialCatalog))
                {
                    connectionBuilder.InitialCatalog = initialCatalog;
                }
                if (!string.IsNullOrEmpty(userId))
                {
                    connectionBuilder.UserID = userId;
                }
                if (!string.IsNullOrEmpty(password))
                {
                    connectionBuilder.Password = password;
                }
                if (!string.IsNullOrEmpty(integratedSecurity))
                {
                    connectionBuilder.IntegratedSecurity = integratedSecurity.ToUpper() == "SSPI" || bool.Parse(integratedSecurity);
                }

                return connectionBuilder.ToString();
            }

            public static void SetConnectionString(string connectionString, Dictionary<string, string> configurationProperties)
            {
                System.Data.SqlClient.SqlConnectionStringBuilder connectionBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
                configurationProperties["Data Source"] = connectionBuilder.DataSource;
                configurationProperties["Initial Catalog"] = connectionBuilder.InitialCatalog;
                configurationProperties["User ID"] = connectionBuilder.UserID;
                configurationProperties["Password"] = connectionBuilder.Password;
                configurationProperties["Integrated Security"] = connectionBuilder.IntegratedSecurity.ToString();
                // TODO: Fill otherConnectionString
                configurationProperties["Other Connection String"] = "";
            }
        }
    }

    public class DataProviderConfigProperty : DataProviderConfigPropertyInfo
    {
        public string Value { get; set; }
    }
}
