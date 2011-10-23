using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Schemas;

namespace CprBroker.Providers.E_M
{
    [TestFixture]
    public class UnitTests
    {
        public void PickRandomRecord(string connectionString)
        {

        }

        public void Read(string cprNumber, string connectionString)
        {
            // Create data provider
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
            E_MDataProvider emProvider = new E_MDataProvider();
            emProvider.ConfigurationProperties = new Dictionary<string, string>();
            foreach (var configurationKey in emProvider.ConfigurationKeys)
            {
                if (builder.ContainsKey(configurationKey.Name))
                    emProvider.ConfigurationProperties[configurationKey.Name] = builder[configurationKey.Name].ToString();
            }
            QualityLevel? ql;
            var registration = emProvider.Read(new CprBroker.Schemas.PersonIdentifier() { CprNumber = cprNumber }, null, (cpr) => Guid.NewGuid(), out ql);
            Assert.NotNull(registration);
        }

        public static void ValidateNulls<TSource, TResult>(TSource address, TResult result)
            where TSource : class
            where TResult : class
        {
            if (address == null)
            {
                Assert.IsNull(result);
            }
            else
            {
                Assert.IsNotNull(result);
            }
        }
    }
}
