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
            public void SearchList_DataFound()
            {
                var prov = ServicePlatformDataProviderFactory.Create();
                var input = SearchCriteriaFactory.Create();
                var cache = UuidCacheFactory.Create();
                var ret = prov.SearchList(input, cache);
                Assert.Greater(ret.Length, 0);
            }

            [Test]
            public void SearchList_WrongName_NoDataFound()
            {
                var prov = ServicePlatformDataProviderFactory.Create();
                var input = SearchCriteriaFactory.Create();
                input.SoegObjekt.SoegAttributListe.SoegEgenskab[0].NavnStruktur.PersonNameStructure.PersonGivenName = Guid.NewGuid().ToString();
                var cache = UuidCacheFactory.Create();
                var ret = prov.SearchList(input, cache);
                Assert.AreEqual(0, ret.Length);
            }

            [Test]
            public void SearchList_LifeStatus()
            {
                var prov = ServicePlatformDataProviderFactory.Create();
                var input = SearchCriteriaFactory.Create();
                var cache = UuidCacheFactory.Create();
                var ret = prov.SearchList(input, cache).First();
                var oo = ret.Item as CprBroker.Schemas.Part.FiltreretOejebliksbilledeType;
                Assert.NotNull(oo.TilstandListe);
                Assert.NotNull(oo.TilstandListe.LivStatus.TilstandVirkning.FraTidspunkt.ToDateTime());
            }
        }
    }
}
