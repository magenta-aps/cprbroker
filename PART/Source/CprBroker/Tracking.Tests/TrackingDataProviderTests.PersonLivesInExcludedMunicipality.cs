using CprBroker.Data.Part;
using CprBroker.PartInterface.Tracking;
using CprBroker.Providers.Local.Search;
using CprBroker.Schemas;
using CprBroker.Tests.PartInterface;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Tests.Tracking
{
    namespace TrackingDataProviderTests
    {
        [TestFixture]
        public class PersonLivesInExcludedMunicipality : TestBase
        {
            [SetUp]
            public void InitBrokerContext()
            {
                Utilities.InitBrokerContext();
            }

            void AddPersonSearchCache(PersonIdentifier pId, string municipalityCode)
            {
                using (var dataContext = new PartSearchDataContext())
                {
                    var psc = new PersonSearchCache()
                    {
                        UUID = pId.UUID.Value,
                        PersonCivilRegistrationIdentifier = pId.CprNumber,
                        MunicipalityCode = municipalityCode
                    };
                    dataContext.PersonSearchCaches.InsertOnSubmit(psc);
                    dataContext.SubmitChanges();
                }
            }

            [Test]
            public void PersonLivesInExcludedMunicipality_NonExistingPerson_False(
                [Values(true, false)]bool excludedMunicipalitiesExist)
            {
                var pId = Utilities.NewPersonIdentifier();
                var prov = new TrackingDataProvider();
                var ret = prov.PersonLivesInExcludedMunicipality(pId, new int[] { });
                Assert.False(ret);
            }

            [Test]
            public void PersonLivesInExcludedMunicipality_NoneExcluded_False(
                [Values(null, "", "0", "22", "378", "kJu")]string currentMunicipalityCode)
            {
                var pId = Utilities.NewPersonIdentifier();
                AddPersonSearchCache(pId, currentMunicipalityCode);
                var prov = new TrackingDataProvider();
                var ret = prov.PersonLivesInExcludedMunicipality(pId, new int[] { });
                Assert.False(ret);
            }

            [Test]
            public void PersonLivesInExcludedMunicipality_ZeroCodeExcluded_False(
                [Values(0, 20)]int excludedCode)
            {
                var pId = Utilities.NewPersonIdentifier();
                AddPersonSearchCache(pId, "0");
                var prov = new TrackingDataProvider();
                var ret = prov.PersonLivesInExcludedMunicipality(pId, new int[] { excludedCode });
                Assert.False(ret);
            }

            [Test]
            public void PersonLivesInExcludedMunicipality_Excluded_True(
                [Values(1, 22)]int currentMunicipalityCode)
            {
                var pId = Utilities.NewPersonIdentifier();
                AddPersonSearchCache(pId, currentMunicipalityCode.ToString());
                var prov = new TrackingDataProvider();
                var ret = prov.PersonLivesInExcludedMunicipality(pId, new int[] { 1, 22 });
                Assert.True(ret);
            }
        }
    }
}
