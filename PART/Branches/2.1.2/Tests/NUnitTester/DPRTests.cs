using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.DPR;

namespace CprBroker.NUnitTester
{
    [TestFixture]
    public class DPRTests
    {
        [Test]
        [Combinatorial]
        public void TestIsDiversionAlive(
            [Values("localhost")] string address,
            [Values("213542", "7862378", "512367","80", null)] string port,
            [Values("True", "False", null)] string disableDiversion
            )
        {
            DprDatabaseDataProvider dataProvider = new DprDatabaseDataProvider();
            dataProvider.ConfigurationProperties = new Dictionary<string, string>();
            foreach (var configKey in dataProvider.ConfigurationKeys)
            {
                dataProvider.ConfigurationProperties[configKey.Name] = null;
            }
            dataProvider.ConfigurationProperties["Address"] = address;
            dataProvider.ConfigurationProperties["Port"] = port;
            dataProvider.ConfigurationProperties["Disable Diversion"] = disableDiversion;

            var isAlive = dataProvider.IsDiversionAlive();
            if(bool.TrueString.Equals(disableDiversion, StringComparison.InvariantCultureIgnoreCase))
            {
                Assert.True(isAlive);
            }
            else if (port == "80")
            {
                Assert.True(isAlive);
            }
            else
            {
                Assert.False(isAlive);
            }
        }

        [Test]
        public void TestEnsurePersonDataExists()
        { }


    }
}
