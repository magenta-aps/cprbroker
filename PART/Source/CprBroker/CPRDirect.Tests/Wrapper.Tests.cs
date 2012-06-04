using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.CPRDirect;
using NUnit.Framework;

namespace CprBroker.Tests.CPRDirect
{
    [TestFixture]
    public class WrapperTests
    {
        class WrapperStub : Wrapper
        {
            public int _Length;
            public override int Length
            {
                get { return _Length; }
            }
        }

        #region Contents

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Contents_InvalidLength_ThrowsException(
            [Random(0, 10000, 5)]int len,
            [Values(1, 30, -12)]int diff)
        {
            var w = new WrapperStub() { _Length = len };
            w.Contents = new string('s', len + diff);
        }

        [Test]
        public void Contents_InvalidLength_ThrowsException(
            [Random(0, 10000, 5)]int len)
        {
            var w = new WrapperStub() { _Length = len };
            var val = new string('s', len);
            w.Contents = val;
            Assert.AreEqual(val, w.Contents);
        }

        #endregion

        #region Parse

        class WrapperStub1 : Wrapper
        {
            public override int Length
            {
                get { return 6; }
            }
        }
        class WrapperStub2 : Wrapper
        {
            public override int Length
            {
                get { return 6; }
            }
        }
        class ParentWrapperStub : Wrapper
        {
            public override int Length
            {
                get { return 12; }
            }
            public WrapperStub1 WrapperStub1 = null;
            public List<WrapperStub2> WrapperStub2 = new List<WrapperStub2>();
        }

        [Test]
        public void Parse_Correct_CorrectObjects()
        {
            string data = "001AAA002BBB";
            var map = new Dictionary<string, Type>();
            map["001"] = typeof(WrapperStub1);
            map["002"] = typeof(WrapperStub2);
            var par = new ParentWrapperStub();
            System.Diagnostics.Debugger.Launch();
            par.Fill(data, map);
            Assert.NotNull(par.WrapperStub1);
            Assert.AreEqual(1, par.WrapperStub2.Count);
            Assert.NotNull(par.WrapperStub2[0]);
        }

        #endregion
    }
}
