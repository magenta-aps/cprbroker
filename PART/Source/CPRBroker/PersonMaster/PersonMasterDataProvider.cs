using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;

namespace CprBroker.Providers.PersonMaster
{
    public class PersonMasterDataProvider : IPartPersonMappingDataProvider, IExternalDataProvider
    {
        #region IPartPersonMappingDataProvider Members

        public Guid GetPersonUuid(string cprNumber)
        {
            PersonMasterService.BasicOpClient service = new CprBroker.Providers.PersonMaster.PersonMasterService.BasicOpClient("wsBasicHttp");
            string aux = null;
            var ret = service.GetObjectIDFromCpr(Context, cprNumber, ref aux);
            return ret;
        }

        #endregion

        #region IDataProvider Members

        public bool IsAlive()
        {
            System.Net.WebClient client = new System.Net.WebClient();
            try
            {
                System.Uri uri = new Uri(Address);
                uri = new Uri(uri, "?wsdl");
                client.DownloadData(uri);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                client.Dispose();
            }
        }

        public Version Version
        {
            get { return new Version(CprBroker.Engine.Versioning.Major, CprBroker.Engine.Versioning.Minor); }
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
                    new DataProviderConfigPropertyInfo(){Name="Address",IsConfidential=false},
                    new DataProviderConfigPropertyInfo(){Name="Context",IsConfidential=false}
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
