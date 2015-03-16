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
using CprBroker.Engine.Local;
using CprBroker.Providers.CprServices;

namespace CprBroker.Providers.ServicePlatform
{
    public partial class ServicePlatformDataProvider : IExternalDataProvider, IPerCallDataProvider
    {
        #region IPerCallDataProvider members
        public string[] OperationKeys
        {
            get
            {
                return new string[]
                {
                    Constants.OperationKeys.ADRSOG1
                };
            }
        }

        public bool IsAlive()
        {
            string retXml;
            var call = new SearchMethodCall() { Name = Constants.OperationKeys.ADRSOG1 };
            call.InputFields.Add(new KeyValuePair<string, string>("VEJK", "9999"));
            call.InputFields.Add(new KeyValuePair<string, string>("KOMK", "9999"));
            var xml = call.ToRequestXml(CprServices.Properties.Resources.SearchTemplate);

            try
            {
                var kvit = this.CallService(Constants.ServiceUuid.ADRSOG1, xml, out retXml);
                if (!kvit.OK)
                {
                    string callInput = string.Join(",", call.InputFields.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)).ToArray());
                    Admin.LogFormattedError("GCTP <{0}> Failed with <{1}><{2}>. Input <{3}>", call.Name, kvit.ReturnCode, kvit.ReturnText, callInput);
                }
                return kvit.OK;
            }
            catch (Exception ex)
            {
                Admin.LogFormattedError("<{0}>\r\n<{1}>", xml, ex.ToString());
                return false;
            }
        }

        public Version Version
        {
            get { return new Version(CprBroker.Utilities.Constants.Versioning.Major, CprBroker.Utilities.Constants.Versioning.Minor); }
        }

        public DataProviderConfigPropertyInfo[] ConfigurationKeys
        {
            // TODO: Create new DataProviderConfigPropertyInfoTypes.UUID with necessary GUI optimizations
            get
            {
                return new DataProviderConfigPropertyInfo[]{
                    new DataProviderConfigPropertyInfo(){Name=Constants.ConfigProperties.Url, Type = DataProviderConfigPropertyInfoTypes.String, Confidential = false, Required=true},
                    new DataProviderConfigPropertyInfo(){Name=Constants.ConfigProperties.ServiceAgreementUuid, Type = DataProviderConfigPropertyInfoTypes.String, Confidential = true, Required=true},
                    new DataProviderConfigPropertyInfo(){Name=Constants.ConfigProperties.UserSystemUUID, Type = DataProviderConfigPropertyInfoTypes.String, Confidential = true, Required=true},
                    new DataProviderConfigPropertyInfo(){Name=Constants.ConfigProperties.UserUUID, Type = DataProviderConfigPropertyInfoTypes.String, Confidential = true, Required=true},
                    new DataProviderConfigPropertyInfo(){Name=Constants.ConfigProperties.CertificateSerialNumber, Type = DataProviderConfigPropertyInfoTypes.String, Confidential = true, Required=true},
                };
            }
        }

        public Dictionary<string, string> ConfigurationProperties { get; set; }
        #endregion

        #region Config properties
        public string Url
        {
            get { return this.ConfigurationProperties[Constants.ConfigProperties.Url]; }
            set { this.ConfigurationProperties[Constants.ConfigProperties.Url] = value; }
        }
        public string ServiceAgreementUuid
        {
            get { return this.ConfigurationProperties[Constants.ConfigProperties.ServiceAgreementUuid]; }
            set { this.ConfigurationProperties[Constants.ConfigProperties.ServiceAgreementUuid] = value; }
        }
        public string UserSystemUUID
        {
            get { return this.ConfigurationProperties[Constants.ConfigProperties.UserSystemUUID]; }
            set { this.ConfigurationProperties[Constants.ConfigProperties.UserSystemUUID] = value; }
        }
        public string UserUUID
        {
            get { return this.ConfigurationProperties[Constants.ConfigProperties.UserUUID]; }
            set { this.ConfigurationProperties[Constants.ConfigProperties.UserUUID] = value; }
        }

        public string CertificateSerialNumber
        {
            get { return this.ConfigurationProperties[Constants.ConfigProperties.CertificateSerialNumber]; }
            set { this.ConfigurationProperties[Constants.ConfigProperties.CertificateSerialNumber] = value; }
        }
        #endregion
    }
}
