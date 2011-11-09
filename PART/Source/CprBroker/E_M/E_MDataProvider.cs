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
                    return Citizen.ToRegistreringType1(dbCitizen, effectDate, cpr2uuidFunc);
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
