using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.Schemas
{
    namespace PersonNameStructureTypeTests
    {
        [TestFixture]
        public class Constructor
        {
            [Test]
            public void Constructor_NoComma_OK()
            {
                var names = new string[] { "John", "Smith" };
                var ret = new PersonNameStructureType(names);
                Assert.AreEqual("John", ret.PersonGivenName);
                Assert.AreEqual("Smith", ret.PersonSurnameName);
            }

            [Test]
            public void Constructor_Comma_OK()
            {
                var names = new string[] { "Smith, John" };
                var ret = new PersonNameStructureType(names);
                Assert.AreEqual("John", ret.PersonGivenName);
                Assert.AreEqual("Smith", ret.PersonSurnameName);
            }
        }
    }
}
