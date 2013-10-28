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
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Utilities;
using System.IO;
using System.Net;
using CprBroker.Engine.Local;

namespace CprBroker.Providers.CPRDirect
{
    public partial class CPRDirectExtractDataProvider : IPartReadDataProvider, IExternalDataProvider, IPartPeriodDataProvider
    {
        #region IPartReadDataProvider members
        public RegistreringType1 Read(CprBroker.Schemas.PersonIdentifier uuid, LaesInputType input, Func<string, Guid> cpr2uuidFunc, out QualityLevel? ql)
        {
            ql = QualityLevel.Cpr;
            IndividualResponseType response = null;

            response = ExtractManager.GetPerson(uuid.CprNumber);

            if (response != null)
            {
                return response.ToRegistreringType1(cpr2uuidFunc);
            }
            return null;
        }
        #endregion

        #region IPartPeriodDataProvider members
        public FiltreretOejebliksbilledeType ReadPeriod(DateTime fromDate, DateTime toDate, PersonIdentifier pId, Func<string, Guid> cpr2UuidFunc)
        {
            using (var dataContext = new ExtractDataContext())
            {
                var loadOptions = new System.Data.Linq.DataLoadOptions();
                loadOptions.LoadWith<ExtractItem>(ei => ei.Extract);
                dataContext.LoadOptions = loadOptions;

                var individualResponses = Extract.GetPersonFromAllExtracts(pId.CprNumber, dataContext.ExtractItems, Constants.DataObjectMap);
                var fullResp = new IndividualHistoryResponseType(pId, individualResponses);

                var pnrs = fullResp.AllRelationPNRs;
                var uuids = CprBroker.Data.Part.PersonMapping.AssignGuids(pnrs);
                Func<string, Guid> uuidGetter = (pnr) =>
                {
                    var uuid = uuids[Array.IndexOf<string>(pnrs, pnr)];
                    return uuid.HasValue ? uuid.Value : cpr2UuidFunc(pnr);
                };

                return fullResp
                    .ToFiltreretOejebliksbilledeType(uuidGetter)
                    .Filter(VirkningType.Create(fromDate, toDate));

            }
        }
        #endregion

        #region IDataProvider members
        public bool IsAlive()
        {
            try
            {
                if (Directory.Exists(this.ExtractsFolder))
                {
                    // Try to create and move file
                    var filePath = CprBroker.Utilities.Strings.NewUniquePath(this.ExtractsFolder, "txt");
                    File.WriteAllText(filePath, "ABC");
                    string ss = File.ReadAllText(filePath);
                    var tmpFile = ExtractManager.MoveToProcessed(this.ExtractsFolder, filePath);
                    File.Delete(tmpFile);

                    if (this.HasFtpSource)
                    {
                        var files = ListFtpContents();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Admin.LogException(ex);
                while (ex.InnerException != null)
                {
                    Admin.LogException(ex.InnerException);
                    ex = ex.InnerException;
                }
            }
            return false;
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
                    new DataProviderConfigPropertyInfo(){ Name=Constants.PropertyNames.ExtractsFolder, Type= DataProviderConfigPropertyInfoTypes.String, Required=true, Confidential=false},
                    new DataProviderConfigPropertyInfo(){ Name=Constants.PropertyNames.HasFtpSource, Type= DataProviderConfigPropertyInfoTypes.Boolean, Required=true, Confidential=false},
                    new DataProviderConfigPropertyInfo(){ Name=Constants.PropertyNames.FtpAddress, Type= DataProviderConfigPropertyInfoTypes.String, Required=false, Confidential=false},
                    new DataProviderConfigPropertyInfo(){ Name=Constants.PropertyNames.FtpPort, Type= DataProviderConfigPropertyInfoTypes.Integer, Required=false, Confidential=false},
                    new DataProviderConfigPropertyInfo(){ Name=Constants.PropertyNames.FtpUser, Type= DataProviderConfigPropertyInfoTypes.String, Required=false, Confidential=false},
                    new DataProviderConfigPropertyInfo(){ Name=Constants.PropertyNames.FtpPassword, Type= DataProviderConfigPropertyInfoTypes.String, Required=false, Confidential=true}
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
        public string ExtractsFolder
        {
            get { return ConfigurationProperties[Constants.PropertyNames.ExtractsFolder]; }
            set { ConfigurationProperties[Constants.PropertyNames.ExtractsFolder] = value; }
        }

        public bool HasFtpSource
        {
            get { return bool.Parse(ConfigurationProperties[Constants.PropertyNames.HasFtpSource]); }
            set { ConfigurationProperties[Constants.PropertyNames.HasFtpSource] = value.ToString(); }
        }

        public string FtpAddress
        {
            get { return ConfigurationProperties[Constants.PropertyNames.FtpAddress]; }
            set { ConfigurationProperties[Constants.PropertyNames.FtpAddress] = value.ToString(); }
        }

        public int? FtpPort
        {
            get
            {
                int ret;
                if (int.TryParse(ConfigurationProperties[Constants.PropertyNames.FtpPort], out ret))
                    return ret;
                return null;
            }
            set { ConfigurationProperties[Constants.PropertyNames.FtpPort] = string.Format("{0}", value); }
        }

        public string FtpUser
        {
            get { return ConfigurationProperties[Constants.PropertyNames.FtpUser]; }
            set { ConfigurationProperties[Constants.PropertyNames.FtpUser] = value.ToString(); }
        }

        public string FtpPassword
        {
            get { return ConfigurationProperties[Constants.PropertyNames.FtpPassword]; }
            set { ConfigurationProperties[Constants.PropertyNames.FtpPassword] = value.ToString(); }
        }

        #endregion


    }
}
