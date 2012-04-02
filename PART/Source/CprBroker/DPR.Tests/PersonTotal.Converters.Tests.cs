using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.DPR;
using CprBroker.Utilities;
using CprBroker.Utilities.ConsoleApps;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.DPR.PersonTotalTests
{
    [TestFixture]
    public class Converters : BaseTests
    {
        [Test]
        public void ToChurchMembershipIndicator_F_ReturnsTrue(
            [Values('F', 'f')]char? churchStatus)
        {
            var personTotal = new PersonTotalStub() { ChristianMark = churchStatus };
            var result = personTotal.ToChurchMembershipIndicator();
            Assert.True(result);
        }

        [Test]
        public void ToChurchMembershipIndicator_NonF_ReturnsFalse(
            [Values('A', 'a', 'U', 'u', 'M', 'm', 'S', 's')]char? churchStatus)
        {
            var personTotal = new PersonTotalStub() { ChristianMark = churchStatus };
            var result = personTotal.ToChurchMembershipIndicator();
            Assert.False(result);
        }

        [Test]
        public void ToChurchMembershipIndicator_OtherValues_ReturnsFalse(
            [Values(null, 'w', '2', 'w', 'p')]char? churchStatus)
        {
            var personTotal = new PersonTotalStub() { ChristianMark = churchStatus };
            var result = personTotal.ToChurchMembershipIndicator();
            Assert.False(result);
        }



    }
}
