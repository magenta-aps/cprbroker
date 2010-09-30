using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Engine;
using System.Net.Sockets;

namespace CPRBroker.Providers.DPR
{
    /// <summary>
    /// Base class for all DPR data providers
    /// </summary>
    public abstract class BaseProvider : IExternalDataProvider, IDataProvider
    {
        /// <summary>
        /// Map for error codes that are returned from DPR. Each provider fills its on list
        /// </summary>
        protected static Dictionary<string, string> ErrorCodes = new Dictionary<string, string>();

        #region IExternalDataProvider Members

        public CPRBroker.DAL.DataProvider DatabaseObject { get; set; }

        #endregion

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

        /// <summary>
        /// Used in development environment
        /// </summary>
        [Obsolete]
        protected abstract string TestResponse { get; }

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

            TcpClient client = new TcpClient(DatabaseObject.Address, DatabaseObject.Port.Value);
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
