using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;
using System.Net.Sockets;

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

        //public CprBroker.DAL.DataProviders.DataProvider DatabaseObject { get; set; }

        public Dictionary<string, string> ConfigurationProperties { get; set; }

        public DataProviderConfigPropertyInfo[] ConfigurationKeys
        {
            get
            {
                return new DataProviderConfigPropertyInfo[] { 
                    new DataProviderConfigPropertyInfo(){Name="Address",Required=true,Confidential=false},
                    new DataProviderConfigPropertyInfo(){Name="Port",Required=true,Confidential=false},
                    new DataProviderConfigPropertyInfo(){Name="ConnectionString",Required=true,Confidential=true},
                    new DataProviderConfigPropertyInfo(){Name="KeepSubscription" ,Required=true,Confidential=false}
                };
            }
        }

        #endregion

        public string Address
        {
            get
            {
                return ConfigurationProperties["Address"];
            }
        }
        public int Port
        {
            get
            {
                return int.Parse(ConfigurationProperties["Port"]);
            }
        }

        public string ConnectionString
        {
            get
            {
                return ConfigurationProperties["ConnectionString"];
            }
        }

        public bool KeepSubscription
        {
            get
            {
                return Convert.ToBoolean(ConfigurationProperties["KeepSubscription"]);
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
                return new Version(Versioning.Major, Versioning.Minor);
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
