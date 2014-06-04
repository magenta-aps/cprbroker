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

                protected override byte[] ProcessMessage(byte[] message)
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
                        var stream = client.GetStream() as NetworkStream;
                        stream.Write(inBytes, 0, inBytes.Length);

                        var outBytes = new Byte[2880 + 28];

                        int bytes = stream.Read(outBytes, 0, 30);
                        var ret = enc.GetString(outBytes, 0, bytes);
                        return ret;
                    }

                }
            }

            [Test]
            public void Run_OneConnection_OK()
            {
                var server = new TcpServerStub() { Port = new Random().Next(1000, 10000) };
                server.Start();
                var client = new Client(server);
                var msg = "123";
                var ret = client.GetResponse(msg);
                Assert.AreEqual(msg, ret);
            }

            [Test]
            public void Run_ManyConnections_OK([Range(10, 1000, 50)] int count)
            {
                var server = new TcpServerStub() { Port = 555 };
                server.Start();
                
                long passed = 0;
                var threads = new List<Thread>();
                for (int i = 0; i < count; i++)
                {
                    var th = new Thread(new ThreadStart(() => {
                        var client = new Client(server);
                        var msg = "123";
                        var ret = client.GetResponse(msg);
                        Interlocked.Increment(ref passed);
                    }));
                    threads.Add(th);
                }
                threads.ForEach(th => th.Start());
                while (Interlocked.Read(ref passed) < count)
                {
                    Thread.Sleep(100);
                }
                server.Stop();
            }
        }
    }
}
