using CprBroker.DBR;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Tests.DBR
{
    namespace DiversionResquestTests
    {
        [TestFixture]
        public class Parse
        {
            [Test]
            public void Parse_InvalidInput_ErrorResponse(
                [Values(
                null,
                "",
                "1253",
                "wqd",
                "qwertyuiopop",
                "123456789012",
                "111234567890",
                "101234567890"
                )]
                string input)
            {
                var ret = DiversionRequest.Parse(input);
                Assert.AreEqual(nameof(ErrorRequestType), ret.GetType().Name);
            }

            [Test]
            public void Parse_RandomPnr_Classic(
                [Values('3', '1', '0')]char type,
                [Values('1', '0')]char largeData,
                [ValueSource(typeof(CPRDirect.Utilities), nameof(CPRDirect.Utilities.RandomCprNumberStrings5))] string pnr)
            {
                string input = "" + type + largeData + pnr;
                var ret = DiversionRequest.Parse(input);
                Assert.AreEqual(nameof(ClassicRequestType), ret.GetType().Name);
            }

            [Test]
            public void Parse_RandomPnr_New(
                [Values('3', '1', '0')]char type,
                [Values('1', '0')]char largeData,
                [ValueSource(typeof(CPRDirect.Utilities), nameof(CPRDirect.Utilities.RandomCprNumberStrings5))] string pnr,
                [Values('1', '0')]char force,
                [Values('I', 'S', 'U')] char responseData)
            {
                string input = "" + type + largeData + pnr + "MMXIII" + force + responseData;
                var ret = DiversionRequest.Parse(input);
                Assert.AreEqual(nameof(NewRequestType), ret.GetType().Name);
            }
        }
    }
}
