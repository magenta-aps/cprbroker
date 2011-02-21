using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace CprBroker.NUnitTester
{
    public abstract class BaseTest
    {
        [TestFixtureSetUp]
        public void Initialize()
        {
            TestRunner.Initialize();
            TestData.Initialize();
        }

        public void Validate(string code, string text)
        {
            Assert.IsNotNullOrEmpty(code, "Status Code");
            Assert.AreEqual(code, "200", "Status Code");
        }

        public void ValidateInvalid(string code, string text)
        {
            Assert.IsNotNullOrEmpty(code, "Status Code");
            Assert.AreNotEqual(code, "200", "Status Code");
        }

    }
}
