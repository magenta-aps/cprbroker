using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;
using CprBroker.Utilities;

namespace CprBroker.Providers.PersonMaster
{
    public class PersonMasterDataProvider : IPartPersonMappingDataProvider, IExternalDataProvider
    {
        #region IPartPersonMappingDataProvider Members

        public Guid? GetPersonUuid(string cprNumber)
        {
            PersonMasterService.BasicOpClient service = new CprBroker.Providers.PersonMaster.PersonMasterService.BasicOpClient();
            string aux = null;
            var ret = service.GetObjectIDFromCpr(Context, cprNumber, ref aux);
            return ret;
        }

        #endregion

        #region IDataProvider Members

        public bool IsAlive()
        {
            try
            {
                PersonMasterService.BasicOpClient service = new CprBroker.Providers.PersonMaster.PersonMasterService.BasicOpClient();
                string aux = null;
                var res = service.Probe(Context, ref aux);
                return true; ;
            }
            catch
            {
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
                    new DataProviderConfigPropertyInfo(){Name="Context",Required=true,Confidential=false}
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

        #endregion
    }
}
