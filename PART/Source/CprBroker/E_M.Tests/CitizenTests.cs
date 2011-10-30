using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.E_M;

namespace CprBroker.Tests.E_M
{
    [TestFixture]
    public class CitizenTests
    {
        public decimal[] RandomCprNumbers = Utilities.RandomCprNumbers(5);

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToUdenlandskBorgerType_Null_ThrowsException()
        {
            Citizen.ToUdenlandskBorgerType(null);
        }

        [Test]
        [ExpectedException()]
        public void ToUdenlandskBorgerType_Empty_ThrowsException()
        {
            Citizen.ToUdenlandskBorgerType(new Citizen());
        }

        [Test]
        [Combinatorial]
        public void ToUdenlandskBorgerType_Valid_CorrectCountryCode(
            [ValueSource("RandomCprNumbers")] decimal cprNumber,
            [Values(1500, 762, 876)] short countryCode)
        {
            var result = Citizen.ToUdenlandskBorgerType(new Citizen() { PNR = cprNumber, CountryCode = countryCode });
            Assert.AreEqual(countryCode.ToString(), result.PersonNationalityCode[0].Value);
        }

        [Test]
        [Combinatorial]
        public void ToUdenlandskBorgerType_Valid_CorrectPNR(
            [ValueSource("RandomCprNumbers")] decimal cprNumber,
            [Values(1500, 762, 876)] short countryCode)
        {
            var result = Citizen.ToUdenlandskBorgerType(new Citizen() { PNR = cprNumber, CountryCode = countryCode });
            Assert.AreEqual(Converters.ToCprNumber(cprNumber), result.PersonCivilRegistrationReplacementIdentifier);
        }

        [Test]
        [Combinatorial]
        public void ToUdenlandskBorgerType_Valid_NullFoedselslandKode(
            [ValueSource("RandomCprNumbers")] decimal cprNumber,
            [Values(1500, 762, 876)] short countryCode)
        {
            var result = Citizen.ToUdenlandskBorgerType(new Citizen() { PNR = cprNumber, CountryCode = countryCode });
            Assert.Null(result.FoedselslandKode);
        }

        [Test]
        [Combinatorial]
        public void ToUdenlandskBorgerType_Valid_NullPersonIdentifikator(
            [ValueSource("RandomCprNumbers")] decimal cprNumber,
            [Values(1500, 762, 876)] short countryCode)
        {
            var result = Citizen.ToUdenlandskBorgerType(new Citizen() { PNR = cprNumber, CountryCode = countryCode });
            Assert.IsNullOrEmpty(result.PersonIdentifikator);
        }

        [Test]
        [Combinatorial]
        public void ToUdenlandskBorgerType_Valid_NullSprogCode(
            [ValueSource("RandomCprNumbers")] decimal cprNumber,
            [Values(1500, 762, 876)] short countryCode)
        {
            var result = Citizen.ToUdenlandskBorgerType(new Citizen() { PNR = cprNumber, CountryCode = countryCode });
            Assert.Null(result.SprogKode);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToUkendtBorgerType_Null_ThrowsException()
        {
            Citizen.ToUkendtBorgerType(null);
        }

        [Test]
        [ExpectedException]
        public void ToUkendtBorgerType_InvalidPNR_ThrowsException(
            [Values(0, 43, -99, 23.4)] decimal cprNumber)
        {
            Citizen.ToUkendtBorgerType(new Citizen() { PNR = cprNumber });
        }

        [Test]
        public void ToUkendtBorgerType_Valid_CorrectPNR(
            [ValueSource("RandomCprNumbers")] decimal cprNumber)
        {
            var result = Citizen.ToUkendtBorgerType(new Citizen() { PNR = cprNumber });
            Assert.AreEqual(Converters.ToCprNumber(cprNumber), result.PersonCivilRegistrationReplacementIdentifier);
        }
    }
}
