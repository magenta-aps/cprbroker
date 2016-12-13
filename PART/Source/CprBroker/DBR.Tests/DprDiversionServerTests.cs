
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.DBR;
using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;
using CprBroker.Engine;
using CprBroker.Tests.PartInterface;

namespace CprBroker.Tests.DBR
{
    namespace DprDiversionServerTests
    {
        [TestFixture]
        public class ProcessMessage: TestBase
        {
            class DprDiversionServerStub : DprDiversionServer
            {
                public override string ConnectionString
                {
                    get
                    {
                        return Properties.Settings.Default.ImitatedDprConnectionString;
                    }
                }
            }

            [SetUp]
            public void InitBrokerContext()
            {
                BrokerContext.Current = null;
                BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
            }

            [Test]
            public void ProcessMessage_Empty_99xxxx()
            {
                var server = new DprDiversionServerStub();
                var msg = new byte[0];
                var ret = server.ProcessMessage(msg);
                var retString = CprBroker.Providers.DPR.Constants.DiversionEncoding.GetString(ret);
                Assert.AreEqual("99", retString.Substring(0, 2));
            }

            [Test]
            public void ProcessMessage_Invalid_Empty()
            {
                var server = new DprDiversionServerStub();
                var msg = new byte[6823];
                var ret = server.ProcessMessage(msg);
                Assert.IsEmpty(ret);
            }

            [Test]
            [TestCaseSource(typeof(ClassicRequestTypeTests.ClassicRequestTypeTestsBase), "PNRs")]
            public void ProcessMessage_Valid_NotEmpty(string pnr)
            {
                var server = new DprDiversionServerStub();
                var msg = Encoding.ASCII.GetBytes("11" + pnr);
                var ret = server.ProcessMessage(msg);
                Assert.IsNotEmpty(ret);
            }

            [Test]
            [TestCaseSource(typeof(ClassicRequestTypeTests.ClassicRequestTypeTestsBase), "PNRs")]
            public void ProcessMessage_Valid_OK(string pnr)
            {
                var server = new DprDiversionServerStub();
                var msg = Encoding.ASCII.GetBytes("11" + pnr);
                var ret = server.ProcessMessage(msg);
                var strRet = Encoding.ASCII.GetString(ret);
                Assert.AreEqual("00", strRet.Substring(2, 2));
            }
        }
    }
}
