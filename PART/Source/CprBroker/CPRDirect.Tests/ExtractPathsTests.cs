using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;
using System.IO;

namespace CprBroker.Tests.CPRDirect
{
    [TestFixture]
    public class ExtractPathsTests
    {
        [Test]
        public void UniqueFileName()
        {
            var fileName = CprBroker.Utilities.Strings.NewRandomString(10) + ".txt";

            var ret = ExtractPaths.UniqueFileName("..\\", fileName, false);

            var expected = new FileInfo("..\\" + fileName).FullName;
            Assert.AreEqual(expected, ret);
        }
    }
}
