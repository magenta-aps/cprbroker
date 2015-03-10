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
using CprBroker.PartInterface;
using CprBroker.Schemas.Part;
using CprBroker.Engine;

namespace CprBroker.Providers.ServicePlatform
{
    public class ServicePlatformDataProvider : CprBroker.Engine.IPartSearchListDataProvider, IExternalDataProvider, IPerCallDataProvider
    {
        public LaesResultatType[] SearchList(SoegInputType1 searchCriteria)
        {
            throw new NotImplementedException();
        }

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
            throw new NotImplementedException();
        }

        public Version Version
        {
            get { return new Version(CprBroker.Utilities.Constants.Versioning.Major, CprBroker.Utilities.Constants.Versioning.Minor); }
        }

        public DataProviderConfigPropertyInfo[] ConfigurationKeys
        {
            // TODO: Create new DataProviderConfigPropertyInfoTypes.UUID with necessary GUI optimizations
            // TODO: Shall the UUID properties be confidential?
            get
            {
                return new DataProviderConfigPropertyInfo[]{
                    new DataProviderConfigPropertyInfo(){Name=Constants.ConfigProperties.Url, Type = DataProviderConfigPropertyInfoTypes.String, Confidential = false, Required=true},
                    new DataProviderConfigPropertyInfo(){Name=Constants.ConfigProperties.ServiceAgreementUuid, Type = DataProviderConfigPropertyInfoTypes.String, Confidential = false, Required=true},
                    new DataProviderConfigPropertyInfo(){Name=Constants.ConfigProperties.UserSystemUUID, Type = DataProviderConfigPropertyInfoTypes.String, Confidential = false, Required=true},
                    new DataProviderConfigPropertyInfo(){Name=Constants.ConfigProperties.UserUUID, Type = DataProviderConfigPropertyInfoTypes.String, Confidential = false, Required=true},
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
        #endregion

        #region Technical
        public string CallService(string serviceUuid, string gctpMessage)
        {
            var service = new CprService.CprService(){ Url = this.Url};
            var context = new CprService.InvocationContextType()
            {
                ServiceAgreementUUID = this.ServiceAgreementUuid,
                ServiceUUID = serviceUuid,
                UserSystemUUID = this.UserSystemUUID,
                UserUUID = this.UserUUID,
                OnBehalfOfUser = null,
                CallersServiceCallIdentifier = null,
                AccountingInfo = null
            };
            return service.forwardToCPRService(context, gctpMessage);
        }
        #endregion
    }
}
