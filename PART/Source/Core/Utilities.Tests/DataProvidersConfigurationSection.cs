using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Utilities.Config;

namespace CprBroker.Tests.Utilities
{
    namespace DataProvidersConfigurationSectionTests
    {
        [TestFixture]
        public class GetEnumerator
        {
            [Test]
            public void GetEnumerator_New_Zero()
            {
                var section = new DataProvidersConfigurationSection();
                Assert.AreEqual(0, section.Count());
            }

            [Test]
            public void GetEnumerator_OneThenOne_CountUpdated()
            {
                var section = new DataProvidersConfigurationSection();
                section.KnownTypes.Add(new TypeElement() { TypeName = "JJJJ" });
                Assert.AreEqual(1, section.Count());
                section.KnownTypes.Add(new TypeElement() { TypeName = "SSSSS" });
                Assert.AreEqual(2, section.Count());
            }
        }
    }
}
