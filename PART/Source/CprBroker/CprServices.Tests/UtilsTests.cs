using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using NUnit.Framework;
using CprBroker.Providers.CprServices;


namespace CprBroker.Tests.CprServices
{
    namespace UtilsTests
    {
        [TestFixture]
        public class GetToken
        {
            [Test]
            [ExpectedException]
            public void GetToken_Null_Exception()
            {
                var s = Utils.GetToken(null);
            }

            [Test]
            public void GetToken_Emty_Null()
            {
                var s = Utils.GetToken(new WebHeaderCollection());
                Assert.Null(s);
            }

            [Test]
            public void GetToken_Value_Correct()
            {
                var headers = new WebHeaderCollection();
                headers["Set-Cookie"] = "Token=8PX0MBQ7; Expires=Tue, 21-Oct-2014 13:39:41 GMT";
                var s = Utils.GetToken(headers);
                Assert.AreEqual("8PX0MBQ7", s);
            }
        }
    }
}
