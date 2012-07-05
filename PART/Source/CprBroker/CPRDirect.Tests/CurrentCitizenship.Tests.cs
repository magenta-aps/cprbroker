using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace CurrentCitizenship
    {
        [TestFixture]
        public class StringContryCode
        {
            [Test]
            [Sequential]
            public void StringContryCode_Code_Expected(
                [Values(1,5100,22.0)]decimal code,
                [Values("1","5100","22")]string expected)
            {
                var cit = new CurrentCitizenshipType() { CountryCode = code };
                var ret = cit.StringCountryCode;
                Assert.AreEqual(expected, ret);
            }
        }
    }
}
