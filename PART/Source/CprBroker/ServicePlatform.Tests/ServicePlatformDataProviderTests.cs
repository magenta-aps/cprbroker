using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.ServicePlatform;
using CprBroker.Providers.CprServices;
using CprBroker.Providers.ServicePlatform.Responses;
using CprBroker.Providers.CprServices.Responses;
using NUnit.Framework;

namespace CprBroker.Tests.ServicePlatform
{
    namespace ServicePlatformDataProviderTests
    {
        [TestFixture]
        public class SearchList
        {
            [SetUp]
            public void InitContext()
            {
                CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "NUnit");
            }


            [Test]
            public void SearchList_NotNull()
            {
                var prov = ServicePlatformDataProviderFactory.Create();
                var input = SearchCriteriaFactory.Create();
                var cache = UuidCacheFactory.Create();

                var ret = prov.SearchList(input, cache);
                Assert.NotNull(ret);
            }

            [Test]
            public void SearchList_DataFOund()
            {
                var prov = ServicePlatformDataProviderFactory.Create();
                var input = SearchCriteriaFactory.Create();
                var cache = UuidCacheFactory.Create();

                var ret = prov.SearchList(input, cache);
                Assert.Greater(ret.Length, 0);
            }
        }

        [TestFixture]
        public class PutSubscription : BaseResponseTests
        {
            [SetUp]
            public void InitContext()
            {
                CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "NUnit");
            }

            [Test]
            [TestCaseSource("PNRs")]
            public void PutSubscription_OK(string pnr)
            {
                System.Diagnostics.Debugger.Launch();
                var prov = ServicePlatformDataProviderFactory.Create();
                var ret = prov.PutSubscription(new Schemas.PersonIdentifier() { CprNumber = pnr, UUID = Guid.NewGuid() });
                Assert.True(ret);
            }
        }
    }
}
