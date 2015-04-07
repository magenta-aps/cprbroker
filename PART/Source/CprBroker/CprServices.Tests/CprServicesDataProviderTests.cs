using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CprServices;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.CprServices
{
    namespace CprServicesDataProviderTests
    {
        [TestFixture]
        public class SearchList
        {
            [SetUp]
            public void InitContext()
            {
                CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
            }

            [Test]
            public void Search_OK()
            {
                var prov = CprServicesDataProviderFactory.Create();
                var inp = SearchCriteriaFactory.Create();
                var cache = UuidCacheFactory.Create();
                var ret = prov.SearchList(inp, cache);
                Console.WriteLine(ret.Length);                
                Assert.GreaterOrEqual(ret.Length, 0);
            }
        }
    }
}
