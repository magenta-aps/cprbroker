using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Utilities;

namespace CprBroker.Providers.CPRDirect
{
    public partial class CPRDirectDataProvider : IPartReadDataProvider, IExternalDataProvider
    {
        #region IPartReadDataProvider members
        public RegistreringType1 Read(CprBroker.Schemas.PersonIdentifier uuid, LaesInputType input, Func<string, Guid> cpr2uuidFunc, out QualityLevel? ql)
        {
            IndividualResponseType response = null;

            response = ExtractManager.GetPerson(uuid.CprNumber);

            if (response == null)
            {
                IndividualRequestType request = new IndividualRequestType(PutSubscription, decimal.Parse(uuid.CprNumber));
                response = this.GetResponse(request);
            }

            ql = QualityLevel.Cpr;
            DateTime effectDate = DateTime.Today;
            return response.ToRegistreringType1(cpr2uuidFunc, effectDate);
        }
        #endregion

        #region IDataProvider members
        public bool IsAlive()
        {
            System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
            try
            {
                client.Connect(this.Address, this.Port);
                client.GetStream().Close();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                client.Close();
            }
        }

        public Version Version
        {
            get { return new Version(Utilities.Constants.Versioning.Major, Utilities.Constants.Versioning.Minor); }
        }
        #endregion


        #region IExternalDataProvider Members
        public DataProviderConfigPropertyInfo[] ConfigurationKeys
        {
            get
            {
                return new DataProviderConfigPropertyInfo[] { 
                    new DataProviderConfigPropertyInfo(){ Name="Address", Type= DataProviderConfigPropertyInfoTypes.String, Required=true, Confidential=false},
                    new DataProviderConfigPropertyInfo(){ Name="Port", Type= DataProviderConfigPropertyInfoTypes.Integer, Required=true, Confidential=false},
                    new DataProviderConfigPropertyInfo(){ Name="Put subscription", Type= DataProviderConfigPropertyInfoTypes.Boolean, Required=true, Confidential=false},
                    new DataProviderConfigPropertyInfo(){ Name="Always return CprBorgerType", Type= DataProviderConfigPropertyInfoTypes.Boolean, Required=true, Confidential=false},
                };
            }
        }

        public Dictionary<string, string> ConfigurationProperties
        {
            get;
            set;
        }
        #endregion

        #region Specific members
        public string Address
        {
            get { return ConfigurationProperties["Address"]; }
        }

        public int Port
        {
            get { return Convert.ToInt32(ConfigurationProperties["Port"]); }
        }

        public bool PutSubscription
        {
            get
            { return Convert.ToBoolean(ConfigurationProperties["Put subscription"]); }
        }

        public bool AlwaysReturnCprBorgerType
        {
            get
            { return Convert.ToBoolean(ConfigurationProperties["Always return CprBorgerType"]); }
        }
        #endregion
    }
}
