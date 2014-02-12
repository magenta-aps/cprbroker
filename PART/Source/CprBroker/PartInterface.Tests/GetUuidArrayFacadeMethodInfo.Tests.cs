using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using CprBroker.Engine;
using CprBroker.Engine.Part;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.Engine
{
    namespace GetUuidArrayFacadeMethodInfoTests
    {
        [TestFixture]
        public class ValidateInput
        {
            [Test]
            public void ValidateInput_Correct_OK([Values(1, 10,1000)]int count)
            {
                var pnrs = Utilities.RandomCprNumbers(count);
                var facade = new GetUuidArrayFacadeMethodInfo(pnrs, "", "");
                var ret = facade.ValidateInput();
                Assert.AreEqual("200", ret.StatusKode);
                Assert.AreEqual("OK", ret.FejlbeskedTekst);
            }

            [Test]
            public void ValidateInput_Zero_Invalid()
            {
                var pnrs = Utilities.RandomCprNumbers(0);
                var facade = new GetUuidArrayFacadeMethodInfo(pnrs, "", "");
                var ret = facade.ValidateInput();
                Assert.AreNotEqual("200", ret.StatusKode);
                Assert.AreNotEqual("OK", ret.FejlbeskedTekst);
            }

            [Test]
            public void ValidateInput_OneNull_Invalid([Random(0,10,3)] int nullIndex)
            {
                var pnrs = Utilities.RandomCprNumbers(10);
                pnrs[nullIndex] = null;
                var facade = new GetUuidArrayFacadeMethodInfo(pnrs, "", "");
                var ret = facade.ValidateInput();
                Assert.AreNotEqual("200", ret.StatusKode);
                Assert.AreNotEqual("OK", ret.FejlbeskedTekst);
            }
        }
    }
}
