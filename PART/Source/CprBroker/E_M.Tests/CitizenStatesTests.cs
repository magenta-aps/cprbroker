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
        public CitizenStatesTests()
        {
            InitializeTestCitizens();
        }

        [SetUp]
        public void InitializeTestCitizens()
        {
            TestCitizen = new Citizen() { CitizenStatusCode = 1, CitizenStatusTimestamp = DateTime.Today, CitizenStatusTimestampUncertainty = ' ', MaritalStatus = 'U', MaritalStatusTimestamp = DateTime.Today, MaritalStatusTimestampUncertainty = ' ', Birthdate = DateTime.Today, BirthdateUncertainty = ' ' };
            TestCitizens = new Citizen[] { TestCitizen };
        }

        private Citizen TestCitizen = null;
        private Citizen[] TestCitizens = null;

        #region ToCivilStatusType

        [Test]
        [TestCaseSource("TestCitizens")]
        public void ToCivilStatusType_Valid_NotNull(Citizen citizen)
        {
            var result = citizen.ToCivilStatusType();
            Assert.IsNotNull(result);
        }

        [Test]
        [TestCaseSource("TestCitizens")]
        public void ToCivilStatusType_Valid_CorrectCode(Citizen citizen)
        {
            var result = citizen.ToCivilStatusType();
            Assert.AreEqual(Converters.ToCivilStatusKodeType(citizen.MaritalStatus), result.CivilStatusKode);
        }

        [Test]
        [TestCaseSource("TestCitizens")]
        public void ToCivilStatusType_Valid_TilstandVirkningNotNull(Citizen citizen)
        {
            var result = citizen.ToCivilStatusType();
            Assert.NotNull(result.TilstandVirkning);
        }
        #endregion

        #region ToLivStatusType

        [Test]
        [TestCaseSource("TestCitizens")]
        public void ToLivStatusType_Valid_NotNull(Citizen citizen)
        {
            var result = citizen.ToLivStatusType();
            Assert.IsNotNull(result);
        }

        [Test]
        [TestCaseSource("TestCitizens")]
        public void ToLivStatusType_Valid_CorrectCode(Citizen citizen)
        {
            var result = citizen.ToLivStatusType();
            Assert.AreEqual(Schemas.Util.Enums.ToLifeStatus((decimal)citizen.CitizenStatusCode, citizen.Birthdate), result.LivStatusKode);
        }

        [Test]
        [TestCaseSource("TestCitizens")]
        public void ToLivStatusType_Valid_TilstandVirkningNotNull(Citizen citizen)
        {
            var result = citizen.ToLivStatusType();
            Assert.NotNull(result.TilstandVirkning);
        }
        #endregion

    }
}
