using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace CprBroker.Engine
{
    public class TcpServer : IDisposable
    {
        public int Port { get; set; }
        private int _maxQueueLength = 1000;
        private int _bufferSize = 1024;

        bool _Running = false;
        private System.Net.Sockets.Socket _serverSocket;
        private List<Session> _Sessions = new List<Session>();

        public class Session
        {
            public byte[] buffer;
            public System.Net.Sockets.Socket socket;
        }

        public bool Start()
        {
            CprBroker.Engine.Local.Admin.LogFormattedSuccess("Starting TCP server <{0}> ", Port);
            _Running = true;
            System.Net.IPHostEntry localhost = System.Net.Dns.GetHostEntry("localhost");
            System.Net.IPEndPoint serverEndPoint;
            serverEndPoint = new System.Net.IPEndPoint(localhost.AddressList.Last(), Port);

            _serverSocket = new System.Net.Sockets.Socket(serverEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _serverSocket.Bind(serverEndPoint);
            _serverSocket.Listen(_maxQueueLength);
            //warning, only call this once, this is a bug in .net 2.0 that breaks if 
            // you're running multiple asynch accepts, this bug may be fixed, but
            // it was a major pain in the ass previously, so make sure there is only one
            //BeginAccept running

            BeginAccept();
            CprBroker.Engine.Local.Admin.LogFormattedSuccess("TCP server started at<{0}> ", Port);
            return true;
        }

        public void Stop()
        {
            _Running = false;
            do
            {
                lock (_Sessions)
                {
                    if (_Sessions.Count == 0 && _serverSocket != null)
                    {
                        _serverSocket.Close();
                        break;
                    }
                }
                System.Threading.Thread.Sleep(100);
            } while (_Sessions.Count > 0);
        }

        private void BeginAccept()
        {
            if (_Running)
            {
                _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), _serverSocket);
            }
        }

        private void AcceptCallback(IAsyncResult result)
        {
            BrokerContext.Initialize(Utilities.Constants.EventBrokerApplicationToken.ToString(), Utilities.Constants.UserToken);

            if (_Running)
            {
                Local.Admin.LogFormattedSuccess("AcceptCallback started");

                Session session = new Session();
                try
                {
                    //Finish accepting the connection
                    var sessionSocket = result.AsyncState as Socket;
                    session = new Session();
                    session.socket = sessionSocket.EndAccept(result);
                    session.buffer = new byte[_bufferSize];
                    lock (_Sessions)
                    {
                        _Sessions.Add(session);
                    }
                    //Queue recieving of data from the connection
                    session.socket.BeginReceive(session.buffer, 0, session.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), session);

                    //Queue the accept of the next incomming connection
                    BeginAccept();
                }
                catch (SocketException e)
                {
                    Local.Admin.LogException(e);

                    if (session.socket != null)
                    {
                        session.socket.Close();
                        lock (_Sessions)
                        {
                            _Sessions.Remove(session);
                        }
                    }
                    //Queue the next accept, think this should be here, stop attacks based on killing the waiting listeners
                    BeginAccept();
                }
                catch (Exception e)
                {
                    Local.Admin.LogException(e);
                    if (session.socket != null)
                    {
                        session.socket.Close();
                        lock (_Sessions)
                        {
                            _Sessions.Remove(session);
                        }
                    }
                    //Queue the next accept, think this should be here, stop attacks based on killing the waiting listeners
                    BeginAccept();
                }
            }
            else
            {
                Local.Admin.LogFormattedSuccess("AcceptCallback() Finished - was not run");
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            BrokerContext.Initialize(Utilities.Constants.EventBrokerApplicationToken.ToString(), Utilities.Constants.UserToken);

            if (_Running)
            {
                Local.Admin.LogFormattedSuccess("ReceiveCallback started");

                //get our connection from the callback
                Session conn = (Session)result.AsyncState;
                //catch any errors, we'd better not have any
                try
                {
                    //Grab our buffer and count the number of bytes receives
                    int bytesRead = conn.socket.EndReceive(result);
                    //make sure we've read something, if we haven't it supposadly means that the client disconnected
                    if (bytesRead > 0)
                    {
                        //put whatever you want to do when you receive data here
                        var response = ProcessMessage(conn.buffer.Take(bytesRead).ToArray());
                        conn.socket.Send(response);

                        //Queue the next receive
                        conn.socket.BeginReceive(conn.buffer, 0, conn.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), conn);
                    }
                    else
                    {
                        //Callback run but no data, close the connection
                        //supposadly means a disconnect
                        //and we still have to close the socket, even though we throw the event later
                        conn.socket.Close();
                        lock (_Sessions)
                        {
                            _Sessions.Remove(conn);
                        }
                    }
                }
                catch (SocketException e)
                {
                    //Something went terribly wrong
                    //which shouldn't have happened
                    if (conn.socket != null)
                    {
                        conn.socket.Close();
                        lock (_Sessions)
                        {
                            _Sessions.Remove(conn);
                        }
                    }
                }
            }
            else
            {
                Local.Admin.LogFormattedSuccess("ReceiveCallback finished - not run");
            }
        }

        public virtual byte[] ProcessMessage(byte[] message)
        {
            return new byte[] { };
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
