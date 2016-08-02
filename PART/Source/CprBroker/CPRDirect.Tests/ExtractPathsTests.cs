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
    namespace ExtractPathsTests
    {
        [TestFixture]
        public class UniqueFileName
        {
            [Test]
            public void UniqueFileName_Normal()
            {
                var fileName = CprBroker.Utilities.Strings.NewRandomString(10) + ".txt";

                var ret = ExtractPaths.UniqueFileName("..\\", fileName, null, false);

                var expected = new FileInfo("..\\" + fileName).FullName;
                Assert.AreEqual(expected, ret);
            }

            [Test]
            public void UniqueFileName_Exists_NewPath()
            {
                var fileName = CprBroker.Utilities.Strings.NewRandomString(10) + ".txt";

                var expected = new FileInfo("..\\" + fileName).FullName;
                File.WriteAllText(expected, "ABC");

                var ret = ExtractPaths.UniqueFileName("..\\", fileName, null, false);
                Assert.AreNotEqual(expected, ret);
            }

            [Test]
            public void UniqueFileName_CompanionExists_NewPath()
            {
                var fileName = CprBroker.Utilities.Strings.NewRandomString(10) + ".txt";
                var companion = "."+ CprBroker.Utilities.Strings.NewRandomString(10);
                var expected = new FileInfo("..\\" + fileName).FullName;
                File.WriteAllText(ExtractPaths.CompanionFilePath(expected, companion), "ABC");

                Assert.False(File.Exists(expected));
                var ret = ExtractPaths.UniqueFileName("..\\", fileName, companion, false);
                Assert.AreNotEqual(expected, ret);
            }
        }

        [TestFixture]
        public class CompanionFilePath
        {
            [Test]
            public void CompanionFilePath_Null_SameAsInput([Values(null, "")] string postfix)
            {
                var path = CprBroker.Utilities.Strings.NewRandomString(10) + ".txt";
                var ret = ExtractPaths.CompanionFilePath(path, postfix);
                Assert.AreEqual(path, ret);
            }

            [Test]
            public void CompanionFilePath_Value_Appended()
            {
                var path = CprBroker.Utilities.Strings.NewRandomString(10) + ".txt";
                var postfix = CprBroker.Utilities.Strings.NewRandomString(7);
                var expected = path + postfix;
                var ret = ExtractPaths.CompanionFilePath(path, postfix);
                Assert.AreEqual(expected, ret);
            }
        }        
    }
}
