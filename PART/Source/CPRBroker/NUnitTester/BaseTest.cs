using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NUnitTester
{
    public abstract class BaseTest
    {
        [TestFixtureSetUp]
        public void Initialize()
        {
            TestRunner.Initialize();
            TestData.Initialize();
        }
    }
}
