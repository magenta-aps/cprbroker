using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.E_M;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.E_M
{
    [TestFixture]
    class CitizenStatesTests
    {
            [TestFixtureSetUp]
            public void InitializeTestCitizens()
            {

            }
            private Citizen[] TestCitizens = new Citizen[]
            {
                null,
                new Citizen(){CitizenStatusCode = 1, CitizenStatusTimestamp = DateTime.Today, CitizenStatusTimestampUncertainty='T',MaritalStatus='U',MaritalStatusTimestamp=DateTime.Today, MaritalStatusTimestampUncertainty='T'}
            };

            [Test]
            [TestCaseSource("TestCitizens")]
            public void TestToCivilStatusType(Citizen citizen)
            {
                var result = Citizen.ToCivilStatusType(citizen);
                UnitTests.ValidateNulls<Citizen, CivilStatusType>(citizen, result);
            }

            [Test]
            [TestCaseSource("TestCitizens")]
            public void TestToLivStatusType(Citizen citizen)
            {
                var result = Citizen.ToLivStatusType(citizen);
                UnitTests.ValidateNulls<Citizen, LivStatusType>(citizen, result);         
        }
    }
}
