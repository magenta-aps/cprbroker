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
using System.Data.SqlClient;

namespace CprBroker.Providers.E_M
{
    public partial class E_MDataProvider : IExternalDataProvider, IPartReadDataProvider
    {
        public static readonly Guid ActorId = new Guid("{F2B564A4-CB97-4984-990C-39A5F010BDE3}");
        #region IPartReadDataProvider Members

        public RegistreringType1 Read(PersonIdentifier uuid, LaesInputType input, Func<string, Guid> cpr2uuidFunc, out CprBroker.Schemas.QualityLevel? ql)
        {
            DateTime effectDate = DateTime.Now;
            ql = QualityLevel.DataProvider;

            using (var dataContext = new E_MDataContext(ConnectionString))
            {
                var dbCitizen = dataContext.Citizens
                    .Where(cit => cit.PNR == decimal.Parse(uuid.CprNumber))
                     .FirstOrDefault();
                if (dbCitizen != null)
                {
                    return dbCitizen.ToRegistreringType1(effectDate, cpr2uuidFunc);
                }
            }

            return null;
        }

        #endregion

        #region IDataProvider Members

        public bool IsAlive()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public Version Version
        {
            get
            {
                return new Version(CprBroker.Utilities.Constants.Versioning.Major, CprBroker.Utilities.Constants.Versioning.Minor);
            }
        }

        #endregion

        #region IExternalDataProvider Members

        public DataProviderConfigPropertyInfo[] ConfigurationKeys
        {
            get
            {
                return new DataProviderConfigPropertyInfo[] {                     
                    new DataProviderConfigPropertyInfo(){Name="Data Source", Required=true, Confidential=false},
                    new DataProviderConfigPropertyInfo(){Name="Initial Catalog", Required=false, Confidential=false},
                    new DataProviderConfigPropertyInfo(){Name="User ID", Required=false, Confidential=false},
                    new DataProviderConfigPropertyInfo(){Name="Password", Required=false, Confidential=true},
                    new DataProviderConfigPropertyInfo(){Name="Integrated Security", Required=false, Confidential=false},
                    new DataProviderConfigPropertyInfo(){Name="Other Connection String", Required=false, Confidential=false},
                };
            }
        }

        public Dictionary<string, string> ConfigurationProperties { get; set; }

        #endregion

    }
}
