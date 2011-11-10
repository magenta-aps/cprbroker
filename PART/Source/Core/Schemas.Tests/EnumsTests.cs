using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Util;
using NUnit.Framework;

namespace CprBroker.Tests.Schemas
{
    [TestFixture]
    class EnumsTests
    {
        #region

        [Test]
        public void IsValidCivilRegistrationStatus_Valid_ReturnsTrue(
            [Values(1, 3, 5, 7, 20, 30, 50, 60, 70, 80, 90)]decimal status)
        {
            var result = Enums.IsValidCivilRegistrationStatus(status);
            Assert.True(result);
        }

        [Test]
        public void IsValidCivilRegistrationStatus_Invalid_ReturnsFalse(
            [Values(0, 15, -2, 17, 99, 103, 2007, -43)]decimal status)
        {
            var result = Enums.IsValidCivilRegistrationStatus(status);
            Assert.False(result);
        }

        #endregion

        #region IsActiveCivilRegistrationStatus

        [Test]
        public void IsActiveCivilRegistrationStatus_Inactive_ReturnsFalse(
            [Values(30, 50, 60)]decimal status)
        {
            var result = Enums.IsActiveCivilRegistrationStatus(status);
            Assert.False(result);
        }

        [Test]
        public void IsActiveCivilRegistrationStatus_Active_ReturnsTrue(
            [Values(1, 3, 5, 7, 20, 70, 80, 90)]decimal status)
        {
            var result = Enums.IsActiveCivilRegistrationStatus(status);
            Assert.True(result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void IsActiveCivilRegistrationStatus_InvalidNumber_ThrowsException(
            [Values(0, 15, -2, 17, 99, 103, 2007, -43)]decimal status)
        {
            var result = Enums.IsActiveCivilRegistrationStatus(status);
            Assert.True(result);
        }

        #endregion
    }
}
