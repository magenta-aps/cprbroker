using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace LineWrapperTests
    {
        [TestFixture]
        public class ToWrapper
        {
            [Test]
            public void ToWrapper_EmptyMap_Null()
            {
                var ret = new LineWrapper("2221234567890").ToWrapper(new Dictionary<string, Type>());
                Assert.Null(ret);
            }

            [Test]
            public void ToWrapper_KeyOnly_Null()
            {
                var ret = new LineWrapper("222").ToWrapper(new Dictionary<string, Type>());
                Assert.Null(ret);
            }

            class MyLengthWrapper : Wrapper
            {
                public static int Len = -1;
                public override int Length
                {
                    get { return Len; }
                }


                public static Dictionary<string, Type> GetMyLengthWrapperMap(string key)
                {
                    var ret = new Dictionary<string, Type>();
                    ret[key] = typeof(MyLengthWrapper);
                    return ret;
                }
            }

            [Test]
            public void ToWrapper_ShortString_PaddedCorrectly(
                [Values(10, 20, 87)]int length)
            {
                var line = new LineWrapper("222dok;o");
                MyLengthWrapper.Len = length;
                var ret = line.ToWrapper(MyLengthWrapper.GetMyLengthWrapperMap("222"));
                Assert.AreEqual(length, ret.Contents.Length);
            }
        }
    }
}
