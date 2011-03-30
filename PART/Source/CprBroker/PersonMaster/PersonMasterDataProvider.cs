using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;
using CprBroker.Utilities;
using CprBroker.Providers.PersonMaster.PersonMasterService;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.IdentityModel;

namespace CprBroker.Providers.PersonMaster
{
    /// <summary>
    /// Gets person UUIDs from GK web services
    /// </summary>
    public class PersonMasterDataProvider : IPartPersonMappingDataProvider, IExternalDataProvider
    {
        #region Utility Methods
        private BasicOpClient CreateClient()
        {
            WSHttpBinding binding = new WSHttpBinding();

            var identity = new SpnEndpointIdentity(SpnName);
            EndpointAddress endPointAddress = new EndpointAddress(new Uri(Address + "/PersonMasterService12"), identity);
            BasicOpClient client=new BasicOpClient(binding, endPointAddress);
            return client;

        }
        #endregion
        #region IPartPersonMappingDataProvider Members

        public Guid? GetPersonUuid(string cprNumber)
        {
            return Guid.NewGuid();
            BasicOpClient client = CreateClient();
            string aux = null;
            var ret = client.GetObjectIDFromCpr(Context, cprNumber, ref aux);
            return ret;
        }

        #endregion

        #region IDataProvider Members

        public bool IsAlive()
        {
            try
            {
                BasicOpClient client = CreateClient();
                string aux = null;
                var res = client.Probe(Context, ref aux);
                return true; ;
            }
            catch(Exception ex)
            {
                Engine.Local.Admin.LogException(ex);
                return false;
            }
        }

        public Version Version
        {
            get { return new Version(Constants.Versioning.Major, Constants.Versioning.Minor); }
        }

        #endregion

        #region IExternalDataProvider Members

        public Dictionary<string, string> ConfigurationProperties { get; set; }

        public DataProviderConfigPropertyInfo[] ConfigurationKeys
        {
            get
            {
                return new DataProviderConfigPropertyInfo[] 
                {
                    new DataProviderConfigPropertyInfo(){Name="Address",Required=true,Confidential=false},
                    new DataProviderConfigPropertyInfo(){Name="Context",Required=true,Confidential=false},
                    new DataProviderConfigPropertyInfo(){Name="Spn name",Required=true,Confidential=false}
                };
            }
        }

        #endregion

        #region Configuration properties

        public string Address
        {
            get
            {
                return ConfigurationProperties["Address"];
            }
        }

        public string Context
        {
            get
            {
                return ConfigurationProperties["Context"];
            }
        }

        public string SpnName
        {
            get
            {
                return ConfigurationProperties["Spn name"];
            }
        }

        #endregion
    }
}
