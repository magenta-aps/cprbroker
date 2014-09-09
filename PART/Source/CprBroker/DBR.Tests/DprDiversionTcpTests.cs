using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Net;
using System.Net.Sockets;

namespace CprBroker.Tests.DBR
{
    [TestFixture]
    public class DprDiversionTcpTests
    {
        public string Address = "localhost";
        public int Port = 999;

        [Test]
        [TestCaseSource(typeof(ClassicRequestTypeTests.ClassicRequestTypeTestsBase), "PNRs")]
        public void CallDiversion_NormalPerson_00(string pnr)
        {
            var data = Encoding.ASCII.GetBytes("11" + pnr);
            int retCount;
            using (TcpClient client = new TcpClient(Address, Port))
            {
                using (NetworkStream stream = client.GetStream())
                {
                    stream.Write(data, 0, data.Length);
                    stream.ReadTimeout = 2000;
                    data = new Byte[3500];
                    retCount = stream.Read(data, 0, data.Length);
                }
                var response = System.Text.Encoding.UTF7.GetString(data, 0, retCount);
                Assert.AreEqual("00", response.Substring(2, 2));
            }
        }
    }
}
