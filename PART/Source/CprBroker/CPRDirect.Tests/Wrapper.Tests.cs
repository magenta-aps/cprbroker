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
    }
}
