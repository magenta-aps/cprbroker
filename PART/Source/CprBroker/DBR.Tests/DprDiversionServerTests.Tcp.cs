using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Net;
using System.Net.Sockets;
using CprBroker.DBR;

namespace CprBroker.Tests.DBR
{
    [TestFixture]
    public class DprDiversionServerTcpTests : DbrTestBase
    {
        
        [Test]
        [TestCaseSource(typeof(ClassicRequestTypeTests.ClassicRequestTypeTestsBase), nameof(ClassicRequestTypeTests.ClassicRequestTypeTestsBase.PNRs))]
        public void CallDiversion_NormalPerson_TcpOK(string pnr)
        {
            var queue = this.AddDbrQueue(addPort: true);
            
            using (var server = queue.CreateListener())
            {
                server.Start();

                var data = Encoding.ASCII.GetBytes("01" + pnr);
                int retCount;
                using (TcpClient client = new TcpClient(server.Address, server.Port))
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        stream.Write(data, 0, data.Length);
                        stream.ReadTimeout = 200000;
                        data = new Byte[3500];
                        retCount = stream.Read(data, 0, data.Length);
                    }
                    var response = System.Text.Encoding.ASCII.GetString(data, 0, retCount);
                    //Assert.AreEqual("00", response.Substring(2, 2));
                }

            }
        }
    }
}
