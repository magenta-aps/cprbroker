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
using CprBroker.Engine;
using System.Net.Sockets;
using CprBroker.Utilities;

namespace CprBroker.Providers.DPR
{
    /// <summary>
    /// Base class for all DPR data providers
    /// </summary>
    public abstract class BaseProvider : IExternalDataProvider, IDataProvider, IPerCallDataProvider
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
                    new DataProviderConfigPropertyInfo(){Type = DataProviderConfigPropertyInfoTypes.String, Name="Data Source", Required=true, Confidential=false},
                    new DataProviderConfigPropertyInfo(){Type = DataProviderConfigPropertyInfoTypes.String, Name="Initial Catalog", Required=false, Confidential=false},
                    new DataProviderConfigPropertyInfo(){Type = DataProviderConfigPropertyInfoTypes.String, Name="User ID", Required=false, Confidential=false},
                    new DataProviderConfigPropertyInfo(){Type = DataProviderConfigPropertyInfoTypes.String, Name="Password", Required=false, Confidential=true},
                    new DataProviderConfigPropertyInfo(){Type = DataProviderConfigPropertyInfoTypes.String, Name="Integrated Security", Required=false, Confidential=false},
                    new DataProviderConfigPropertyInfo(){Type = DataProviderConfigPropertyInfoTypes.String, Name="Other Connection String", Required=false, Confidential=false},
                    
                    new DataProviderConfigPropertyInfo(){Type = DataProviderConfigPropertyInfoTypes.Boolean, Name="Disable Diversion", Required=false,Confidential=false},
                    new DataProviderConfigPropertyInfo(){Type = DataProviderConfigPropertyInfoTypes.String, Name="Address", Required=false, Confidential=false},
                    new DataProviderConfigPropertyInfo(){Type = DataProviderConfigPropertyInfoTypes.Integer, Name="Port", Required=false, Confidential=false},                    
                    new DataProviderConfigPropertyInfo(){Type = DataProviderConfigPropertyInfoTypes.Integer, Name="TCP Read Timeout (ms)" , Required=true, Confidential=false},
                    
                };
            }
        }

        public string[] OperationKeys
        {
            get
            {
                // TODO: rename the operation key to something like "Diversion". "Cost" is a bit ambigious.
                return new string[] {
                    Constants.DiversionOperationName
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

        public int TcpReadTimeout
        {
            get
            {
                return Convert.ToInt32(ConfigurationProperties["TCP Read Timeout (ms)"]);
            }
        }

        public bool DisableDiversion
        {
            get
            {
                return Convert.ToBoolean(ConfigurationProperties["Disable Diversion"]);
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

        protected string Send(string message, string cprNumber)
        {
            string response;
            string error;
            if (Send(message, cprNumber, out response, out error))
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
        protected bool Send(string message, string cprNumber, out string response, out string error)
        {
            int bytes = 0;
            error = null;

            try
            {
                using (TcpClient client = new TcpClient(Address, Port))
                {
                    Byte[] data = System.Text.Encoding.UTF7.GetBytes(message);

                    using (NetworkStream stream = client.GetStream())
                    {
                        stream.Write(data, 0, data.Length);
                        stream.ReadTimeout = this.TcpReadTimeout;
                        data = new Byte[3500];
                        bytes = stream.Read(data, 0, data.Length);
                    }
                    response = System.Text.Encoding.UTF7.GetString(data, 0, bytes);
                }

                string errorCode = response.Substring(2, 2);
                if (ErrorCodes.ContainsKey(errorCode))
                {
                    // We log the call and set the success parameter to false
                    this.LogAction(Constants.DiversionOperationName, cprNumber, false);
                    error = ErrorCodes[errorCode];
                    return false;
                }
                else
                {
                    // We log the call and set the success parameter to true
                    this.LogAction(Constants.DiversionOperationName, cprNumber, true);
                    return true;
                }
            }
            catch (Exception e)
            {
                // We log the call and set the success parameter to false
                // TODO: Shall we just rely on the exception logging?
                this.LogAction(Constants.DiversionOperationName, cprNumber, false);
                response = null;
                error = "Exception: " + e.Message;
                return false;
            }

        }

        #endregion

    }
}
