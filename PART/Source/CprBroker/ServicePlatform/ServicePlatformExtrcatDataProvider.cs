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
    public partial class ServicePlatformExtractDataProvider : IExternalDataProvider, CprBroker.PartInterface.IExtractDataProvider, IPartPeriodDataProvider, IPartReadDataProvider, ILocalProxyDataProvider
    {
        #region IDataProvider members

        public bool IsAlive()
        {
            bool sftpOk = false;
            try
            {
                if (HasFtpSource)
                {
                    // TODO: Should we add more tests for SFTP connection here?

                    // This will throw an exception if it fails
                    ListFtpContents();
                    sftpOk = true;
                }
                else
                {
                    sftpOk = true;
                }
                return sftpOk;

            }
            catch (Exception ex)
            {
                CprBroker.Engine.Local.Admin.LogException(ex);
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
                    new DataProviderConfigPropertyInfo(){Name=Constants.ConfigProperties.ExtractsFolder, Type = DataProviderConfigPropertyInfoTypes.String, Confidential = false, Required=true},
                    new DataProviderConfigPropertyInfo(){Name=Constants.ConfigProperties.LocalProxyUsage, Type = DataProviderConfigPropertyInfoTypes.Enumeration, Confidential = false, Required=true, EnumType = typeof(LocalProxyUsageOptions)},

                    new DataProviderConfigPropertyInfo(){Name=Constants.ConfigProperties.HasSftpSource , Type = DataProviderConfigPropertyInfoTypes.Boolean, Confidential = false, Required=true},

                    new DataProviderConfigPropertyInfo(){Name=Constants.ConfigProperties.SftpAddress , Type = DataProviderConfigPropertyInfoTypes.String, Confidential = false, Required=true},
                    new DataProviderConfigPropertyInfo(){Name=Constants.ConfigProperties.SftpPort , Type = DataProviderConfigPropertyInfoTypes.Integer, Confidential = false, Required=true},
                    new DataProviderConfigPropertyInfo(){Name=Constants.ConfigProperties.SftpUser , Type = DataProviderConfigPropertyInfoTypes.String, Confidential = false, Required=true},
                    new DataProviderConfigPropertyInfo(){Name=Constants.ConfigProperties.SftpSshPrivateKeyPath , Type = DataProviderConfigPropertyInfoTypes.String, Confidential = false, Required=true},
                    new DataProviderConfigPropertyInfo(){Name=Constants.ConfigProperties.SftpSshPrivateKeyPassword , Type = DataProviderConfigPropertyInfoTypes.String, Confidential = true, Required=false},
                    new DataProviderConfigPropertyInfo(){Name=Constants.ConfigProperties.SftpSshHostKeyFingerprint , Type = DataProviderConfigPropertyInfoTypes.String, Confidential = false, Required=true},
                    //new DataProviderConfigPropertyInfo(){Name=Constants.ConfigProperties.SftpRegexFilter , Type = DataProviderConfigPropertyInfoTypes.String, Confidential = false, Required=true},
                    //new DataProviderConfigPropertyInfo(){Name=Constants.ConfigProperties.SftpProcessFilesFromSenderName , Type = DataProviderConfigPropertyInfoTypes.String, Confidential = false, Required=true},
                    new DataProviderConfigPropertyInfo(){Name=Constants.ConfigProperties.SftpRemotePath , Type = DataProviderConfigPropertyInfoTypes.String, Confidential = false, Required=false},
                };
            }
        }

        public Dictionary<string, string> ConfigurationProperties { get; set; }

        public LocalProxyUsageOptions LocalProxyUsage
        {
            get
            {
                return DataProviderConfigPropertyInfo.GetEnum<LocalProxyUsageOptions>(ConfigurationProperties, Constants.ConfigProperties.LocalProxyUsage);
            }
            set
            {
                this.ConfigurationProperties[Constants.ConfigProperties.LocalProxyUsage] = value.ToString();
            }
        }
        #endregion

        #region Config properties
        public bool HasFtpSource
        {
            get { return Convert.ToBoolean(this.ConfigurationProperties[Constants.ConfigProperties.HasSftpSource]); }
            set { this.ConfigurationProperties[Constants.ConfigProperties.HasSftpSource] = value.ToString(); }
        }

        public string FtpAddress
        {
            get { return this.ConfigurationProperties[Constants.ConfigProperties.SftpAddress]; }
            set { this.ConfigurationProperties[Constants.ConfigProperties.SftpAddress] = value; }
        }

        public string SftpUser
        {
            get { return this.ConfigurationProperties[Constants.ConfigProperties.SftpUser]; }
            set { this.ConfigurationProperties[Constants.ConfigProperties.SftpUser] = value; }
        }

        public int SftpPort
        {
            get { return DataProviderConfigPropertyInfo.GetInteger(this.ConfigurationProperties, Constants.ConfigProperties.SftpPort); }
            set { this.ConfigurationProperties[Constants.ConfigProperties.SftpPort] = value.ToString(); }
        }

        /// <summary>
        /// Path to SSH private key file (PuTTY format)
        /// </summary>
        public string SftpSshPrivateKeyPath
        {
            get { return this.ConfigurationProperties[Constants.ConfigProperties.SftpSshPrivateKeyPath]; }
            set { this.ConfigurationProperties[Constants.ConfigProperties.SftpSshPrivateKeyPath] = value; }
        }

        public string SftpSshPrivateKeyPassword
        {
            get { return this.ConfigurationProperties[Constants.ConfigProperties.SftpSshPrivateKeyPassword]; }
            set { this.ConfigurationProperties[Constants.ConfigProperties.SftpSshPrivateKeyPassword] = value; }
        }

        public string SftpSshHostKeyFingerprint
        {
            get { return this.ConfigurationProperties[Constants.ConfigProperties.SftpSshHostKeyFingerprint]; }
            set { this.ConfigurationProperties[Constants.ConfigProperties.SftpSshHostKeyFingerprint] = value; }
        }

        /*
        public string SftpRegexFilter
        {
            get { return this.ConfigurationProperties[Constants.ConfigProperties.SftpRegexFilter]; }
            set { this.ConfigurationProperties[Constants.ConfigProperties.SftpRegexFilter] = value; }
        }
        */
        /*
        public string SftpProcessFilesFromSenderName
        {
            get { return this.ConfigurationProperties[Constants.ConfigProperties.SftpProcessFilesFromSenderName]; }
            set { this.ConfigurationProperties[Constants.ConfigProperties.SftpProcessFilesFromSenderName] = value; }
        }*/

        public string ExtractsFolder
        {
            get { return this.ConfigurationProperties[Constants.ConfigProperties.ExtractsFolder]; }
            set { this.ConfigurationProperties[Constants.ConfigProperties.ExtractsFolder] = value; }
        }

        public string SftpRemotePath
        {
            get { return this.ConfigurationProperties[Constants.ConfigProperties.SftpRemotePath]; }
            set { this.ConfigurationProperties[Constants.ConfigProperties.SftpRemotePath] = value; }
        }

        #endregion

        public Schemas.Part.FiltreretOejebliksbilledeType ReadPeriod(DateTime fromDate, DateTime toDate, Schemas.PersonIdentifier pId, Func<string, Guid> cpr2uuidFunc)
        {
            // The same format as CPR Direct extracts, reuse the functionality
            var prov = new CPRDirect.CPRDirectExtractDataProvider();
            return prov.ReadPeriod(fromDate, toDate, pId, cpr2uuidFunc);
        }

        public Schemas.Part.RegistreringType1 Read(Schemas.PersonIdentifier uuid, Schemas.Part.LaesInputType input, Func<string, Guid> cpr2uuidFunc, out Schemas.QualityLevel? ql)
        {
            // The same format as CPR Direct extracts, reuse the functionality
            var prov = new CPRDirect.CPRDirectExtractDataProvider();
            return prov.Read(uuid, input, cpr2uuidFunc, out ql);
        }


    }
}
