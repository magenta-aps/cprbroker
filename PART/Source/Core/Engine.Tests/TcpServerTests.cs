using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Engine;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace CprBroker.Tests.Engine
{
    namespace TcpServerTests
    {
        [TestFixture]
        public class Running
        {
            public class TcpServerStub : TcpServer
            {
                public int Calls = 0;

                public override byte[] ProcessMessage(byte[] message)
                {
                    System.Threading.Interlocked.Increment(ref Calls);
                    return _ProcessMessage(message);
                }

                Func<byte[], byte[]> _ProcessMessage = (msg) => msg;
            }

            public class Client
            {
                private TcpServer _Server;
                public Client(TcpServer server)
                {
                    _Server = server;
                }
                public string GetResponse(string msg)
                {
                    var enc = System.Text.Encoding.GetEncoding(1252);
                    byte[] inBytes = enc.GetBytes(msg);
                    using (var client = new TcpClient("Beemen-PC", _Server.Port))
                    {
                        using (var stream = client.GetStream() as NetworkStream)
                        {
                            stream.Write(inBytes, 0, inBytes.Length);

                            var outBytes = new Byte[2880 + 28];

                            int bytes = stream.Read(outBytes, 0, outBytes.Length);
                            var ret = enc.GetString(outBytes, 0, bytes);
                            return ret;
                        }
                    }

                }
            }

            int NewPort()
            {
                return new Random().Next(1000, 10000);
            }

            [Test]
            public void Run_OneConnection_OK()
            {
                using (var server = new TcpServerStub() { Port = NewPort() })
                {
                    server.Start();
                    var client = new Client(server);
                    var msg = "123";
                    var ret = client.GetResponse(msg);
                    Assert.AreEqual(msg, ret);
                }
            }

            public void Run_ManyConnections_OK(int count)
            {
                BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
                using (var server = new TcpServerStub() { Port = NewPort() })
                {
                    server.Start();

                    long passed = 0;
                    var threads = new List<Thread>();
                    for (int i = 0; i < count; i++)
                    {
                        var th = new Thread(new ParameterizedThreadStart((p) =>
                        {
                            var client = new Client(server);
                            var msg = Guid.NewGuid().ToString();
                            var ret = client.GetResponse(msg);
                            Interlocked.Increment(ref passed);
                            Assert.AreEqual(msg, ret);
                        }));
                        threads.Add(th);
                    }
                    threads.ForEach(th => th.Start(threads.IndexOf(th)));
                    while (Interlocked.Read(ref passed) < count)
                    {
                        Thread.Sleep(100);
                    }
                    threads.ForEach(th => th.Abort());
                }
            }

            [Test]
            public void Run_ManyConnections_P1_OK([Range(1, 9, 1)] int count)
            {
                Run_ManyConnections_OK(count);
            }

            [Test]
            public void Run_ManyConnections_P2_OK([Range(10, 90, 10)] int count)
            {
                Run_ManyConnections_OK(count);
            }

            [Test]
            public void Run_ManyConnections_P3_OK([Range(100, 900, 100)] int count)
            {
                Run_ManyConnections_OK(count);
            }

            [Ignore]
            [Test]
            public void Run_ManyConnections_P4_OK([Range(1000, 9000, 1000)] int count)
            {
                Run_ManyConnections_OK(count);
            }

            [Ignore]
            [Test]
            public void Run_ManyConnections_P5_OK([Range(10000, 90000, 10000)] int count)
            {
                Run_ManyConnections_OK(count);
            }
        }
    }
}
