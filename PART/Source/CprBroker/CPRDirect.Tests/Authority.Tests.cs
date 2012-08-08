using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace AuthorityTests
    {
        [TestFixture]
        public class ImportText
        {
            [Test]
            public void ImportAll()
            {
                Authority.ImportText(Properties.Resources._4357);
            }
        }

        [TestFixture]
        public class GetNameByCode
        {
            [Test]
            public void GetNameByCode_Denmark_Denmark()
            {
                var name = Authority.GetNameByCode("5100");
                Assert.AreEqual("Danmark", name);
            }

            [Test]
            public void GetNameByCode_Invalid_Null(
                [Values("jhjk", null, "-111")]string code)
            {
                var name = Authority.GetNameByCode(code);
                Assert.IsNullOrEmpty(name);
            }
        }
    }
}
