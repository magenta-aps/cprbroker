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

namespace CprBroker.Providers.CPRDirect
{
    public partial class CPRDirectClientDataProvider : IPartReadDataProvider, IPutSubscriptionDataProvider, IExternalDataProvider, IPerCallDataProvider
    {
        #region IPartReadDataProvider members
        public RegistreringType1 Read(CprBroker.Schemas.PersonIdentifier uuid, LaesInputType input, Func<string, Guid> cpr2uuidFunc, out QualityLevel? ql)
        {
            IndividualRequestType request = new IndividualRequestType(true, DataType.DefinedByTask, decimal.Parse(uuid.CprNumber));
            IndividualResponseType response = this.GetResponse(request);

            ql = QualityLevel.Cpr;
            return response.ToRegistreringType1(cpr2uuidFunc);
        }

        public bool PutSubscription(PersonIdentifier personIdentifier)
        {
            IndividualRequestType request = new IndividualRequestType(true, DataType.NoData, decimal.Parse(personIdentifier.CprNumber));
            IndividualResponseType response = this.GetResponse(request);
            return true;
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
                    new DataProviderConfigPropertyInfo(){ Name=Constants.PropertyNames.Address, Type= DataProviderConfigPropertyInfoTypes.String, Required=true, Confidential=false},
                    new DataProviderConfigPropertyInfo(){ Name=Constants.PropertyNames.Port, Type= DataProviderConfigPropertyInfoTypes.Integer, Required=true, Confidential=false}
                };
            }
        }

        public Dictionary<string, string> OperationProperties { get; set; }

        public DataProviderConfigPropertyInfo[] OperationKeys
        {
            get
            {
                return new DataProviderConfigPropertyInfo[] { 
                    new DataProviderConfigPropertyInfo(){Type = DataProviderConfigPropertyInfoTypes.Decimal, Name="Online Cost", Required=true, Confidential=false},
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
            get { return ConfigurationProperties[Constants.PropertyNames.Address]; }
        }

        public int Port
        {
            get { return Convert.ToInt32(ConfigurationProperties[Constants.PropertyNames.Port]); }
        }

        #endregion


    }
}
