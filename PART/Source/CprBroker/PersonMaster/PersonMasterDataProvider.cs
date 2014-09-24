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
            if (string.IsNullOrEmpty(this.EndPointConfigurationName))
            {
                EndpointAddress endPointAddress;
                if (string.IsNullOrEmpty(SpnName))
                {
                    endPointAddress = new EndpointAddress(new Uri(Address));
                }
                else
                {
                    var identity = new SpnEndpointIdentity(SpnName);
                    endPointAddress = new EndpointAddress(new Uri(Address), identity);
                }

                WSHttpBinding binding;
                if (endPointAddress.Uri.Scheme == "https")
                {
                    binding = new WSHttpBinding(SecurityMode.Transport);
                    binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                }
                else
                {
                    binding = new WSHttpBinding();
                }

                return new BasicOpClient(binding, endPointAddress);
            }
            else
            {
                return new BasicOpClient(EndPointConfigurationName);
            }
        }
        #endregion
        #region IPartPersonMappingDataProvider Members

        public Guid? GetPersonUuid(string cprNumber)
        {
            BasicOpClient client = CreateClient();
            string aux = null;
            var ret = client.GetObjectIDFromCpr(Context, cprNumber, ref aux);
            return ret;
        }

        public Guid?[] GetPersonUuidArray(string[] cprNumberArray)
        {
            BasicOpClient client = CreateClient();
            string aux = null;
            var ret = client.GetObjectIDsFromCprArray(Context, cprNumberArray, ref aux);
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
            catch (Exception ex)
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
                    new DataProviderConfigPropertyInfo(){Name="Address", Type=DataProviderConfigPropertyInfoTypes.String, Required=false, Confidential=false},
                    new DataProviderConfigPropertyInfo(){Name="Context", Type=DataProviderConfigPropertyInfoTypes.String, Required=false, Confidential=false},
                    new DataProviderConfigPropertyInfo(){Name="Spn name", Type=DataProviderConfigPropertyInfoTypes.String, Required=false, Confidential=false},
                    new DataProviderConfigPropertyInfo(){Name="End point configuration name", Type = DataProviderConfigPropertyInfoTypes.String, Required=false, Confidential=false}
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

        public string EndPointConfigurationName
        {
            get
            {
                return ConfigurationProperties["End point configuration name"];
            }
        }

        #endregion



    }
}
