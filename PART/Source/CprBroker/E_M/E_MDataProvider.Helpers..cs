using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace CprBroker.Providers.E_M
{
    public partial class E_MDataProvider
    {
        private string ConnectionString
        {
            get
            {
                string other = string.Format("{0}", ConfigurationProperties["Other Connection String"]);
                string dataSource = string.Format("{0}", ConfigurationProperties["Data Source"]);
                string initialCatalog = string.Format("{0}", ConfigurationProperties["Initial Catalog"]);
                string userId = string.Format("{0}", ConfigurationProperties["User ID"]);
                string password = string.Format("{0}", ConfigurationProperties["Password"]);
                string integratedSecurity = string.Format("{0}", ConfigurationProperties["Integrated Security"]);

                System.Data.SqlClient.SqlConnectionStringBuilder connectionBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(other);
                if (!string.IsNullOrEmpty(dataSource))
                {
                    connectionBuilder.DataSource = dataSource;
                }
                if (!string.IsNullOrEmpty(initialCatalog))
                {
                    connectionBuilder.InitialCatalog = initialCatalog;
                }
                if (!string.IsNullOrEmpty(userId))
                {
                    connectionBuilder.UserID = userId;
                }
                if (!string.IsNullOrEmpty(password))
                {
                    connectionBuilder.Password = password;
                }
                if (!string.IsNullOrEmpty(integratedSecurity))
                {
                    connectionBuilder.IntegratedSecurity = integratedSecurity.ToUpper() == "SSPI" || bool.Parse(integratedSecurity);
                }

                return connectionBuilder.ToString();
            }
        }
    }
}
