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
        public void ToUkendtBorgerType_Null_ThrowsException()
        {            
            Citizen.ToUkendtBorgerType(null);
        }

        [Test]
        [ExpectedException]
        public void ToUkendtBorgerType_InvalidPNR_ThrowsException(
            [Values(0,43,-99, 23.4)] decimal cprNumber)
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
