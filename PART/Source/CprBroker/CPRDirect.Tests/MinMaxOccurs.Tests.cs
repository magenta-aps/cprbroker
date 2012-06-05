using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    [TestFixture]
    class MinMaxOccursTests
    {
        [Test]
        [ExpectedException]
        public void Validate_LessThanMin_Exception(
            [Random(0, 100, 5)]int count)
        {
            new MinMaxOccurs(count + 1, count + 10).ValidateCount(count);
        }

        [Test]
        [ExpectedException]
        public void Validate_GreaterThanMax_Exception(
            [Random(0, 100, 5)]int count)
        {
            new MinMaxOccurs(count - 10, count - 1).ValidateCount(count);
        }

        [Test]
        public void Validate_Exact_OK(
            [Random(0, 100, 5)]int count)
        {
            new MinMaxOccurs(count, count).ValidateCount(count);
        }
    }
}
