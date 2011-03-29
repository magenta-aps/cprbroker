using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;
using System.Net.Sockets;
using CprBroker.Utilities;

namespace CprBroker.Providers.DPR
{
    /// <summary>
    /// Base class for all DPR data providers
    /// </summary>
    public abstract class BaseProvider : IExternalDataProvider, IDataProvider
    {
        /// <summary>
        /// Map for error codes that are returned fromDate DPR. Each provider fills its own list
        /// </summary>
        protected static Dictionary<string, string> ErrorCodes = new Dictionary<string, string>();

        #region IExternalDataProvider Members

        public Dictionary<string, string> ConfigurationProperties { get; set; }

        public DataProviderConfigPropertyInfo[] ConfigurationKeys
        {
            get
            {
                return new DataProviderConfigPropertyInfo[] { 
                    new DataProviderConfigPropertyInfo(){Name="Address", Required=true, Confidential=false},
                    new DataProviderConfigPropertyInfo(){Name="Port", Required=true, Confidential=false},                    
                    new DataProviderConfigPropertyInfo(){Name="Keep Subscription" , Required=true, Confidential=false},
                    new DataProviderConfigPropertyInfo(){Name="Data Source", Required=true, Confidential=false},
                    new DataProviderConfigPropertyInfo(){Name="Initial Catalog", Required=false, Confidential=false},
                    new DataProviderConfigPropertyInfo(){Name="User ID", Required=false, Confidential=false},
                    new DataProviderConfigPropertyInfo(){Name="Password", Required=false, Confidential=true},
                    new DataProviderConfigPropertyInfo(){Name="Integrated Security", Required=false, Confidential=false},
                    new DataProviderConfigPropertyInfo(){Name="Other Connection String", Required=false, Confidential=false},
                };
            }
        }

        #endregion

        protected string Address
        {
            get
            {
                return ConfigurationProperties["Address"];
            }
        }
        protected int Port
        {
            get
            {
                return int.Parse(ConfigurationProperties["Port"]);
            }
        }

        protected string ConnectionString
        {
            get
            {
                string other = string.Format("{0}", ConfigurationProperties["Other Connection String"]);
                string dataSource = string.Format("{0}", ConfigurationProperties["Data Source"]);
                string initialCatalog = string.Format("{0}", ConfigurationProperties["Initial Catalog"]);
                string userId = string.Format("{0}", ConfigurationProperties["User ID"]);
                string password = string.Format("{0}", ConfigurationProperties["Password"]);
                string integratedSecurity = string.Format("{0}", ConfigurationProperties["Integrated Security"]);

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
        }

        public bool KeepSubscription
        {
            get
            {
                return Convert.ToBoolean(ConfigurationProperties["Keep Subscription"]);
            }
        }

        #region IDataProvider Members

        public virtual bool IsAlive()
        {
            return true;
        }

        public Version Version
        {
            get
            {
                return new Version(CprBroker.Utilities.Constants.Versioning.Major, CprBroker.Utilities.Constants.Versioning.Minor);
            }
        }

        #endregion

        #region Protected Members

        protected string Send(string message)
        {
            string response;
            string error;
            if (Send(message, out response, out error))
            {
                return response;
            }
            else
            {
                throw new Exception(error);
            }
        }

        /// <summary>
        /// Sends a message through TCP to the server
        /// </summary>
        /// <param name="message">Message to be sent</param>
        /// <param name="response">Response to message</param>
        /// <param name="error">Error text (if any)</param>
        /// <returns>True if no error, false otherwise</returns>
        protected bool Send(string message, out string response, out string error)
        {
            error = null;
            NetworkStream stream = null;

            TcpClient client = new TcpClient(Address, Port);
            Byte[] data = System.Text.Encoding.UTF7.GetBytes(message);

            stream = client.GetStream();
            stream.Write(data, 0, data.Length);

            data = new Byte[3500];

            int bytes = stream.Read(data, 0, data.Length);
            response = System.Text.Encoding.UTF7.GetString(data, 0, bytes);


            string errorCode = response.Substring(2, 2);
            if (ErrorCodes.ContainsKey(errorCode))
            {
                error = ErrorCodes[errorCode];
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

    }
}
