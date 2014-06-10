using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;

namespace CprBroker.DBR
{
    public class DiversionListener
    {
        public int Port { get; set; }
        private System.Net.Sockets.Socket _serverSocket;
        private int _maxQueueLength = 10;
        private int _bufferSize = 1024;
        private List<xConnection> _sockets = new List<xConnection>();

        public virtual void Process222(IAsyncResult result)
        {
            using (var client = (result.AsyncState as System.Net.Sockets.TcpListener).AcceptTcpClient())
            {
                using (var stream = client.GetStream())
                {
                    string input = null, output = null;
                    using (var rd = new System.IO.StreamReader(stream))
                    {
                        input = rd.ReadToEnd();
                    }
                    output = input;

                    using (var w = new System.IO.StreamWriter(stream))
                    {
                        w.Write(input);
                    }
                }
            }
        }

        public class xConnection //: xBase
        {
            public byte[] buffer;
            public System.Net.Sockets.Socket socket;
        }

        public bool Start()
        {
            System.Net.IPHostEntry localhost = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            System.Net.IPEndPoint serverEndPoint;
            serverEndPoint = new System.Net.IPEndPoint(localhost.AddressList[0], Port);

            _serverSocket = new System.Net.Sockets.Socket(serverEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _serverSocket.Bind(serverEndPoint);

            //warning, only call this once, this is a bug in .net 2.0 that breaks if 
            // you're running multiple asynch accepts, this bug may be fixed, but
            // it was a major pain in the ass previously, so make sure there is only one
            //BeginAccept running
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), _serverSocket);
            return true;
        }

        private void AcceptCallback(IAsyncResult result)
        {
            xConnection conn = new xConnection();
            try
            {
                //Finish accepting the connection
                System.Net.Sockets.Socket s = (System.Net.Sockets.Socket)result.AsyncState;
                conn = new xConnection();
                conn.socket = s.EndAccept(result);
                conn.buffer = new byte[_bufferSize];
                lock (_sockets)
                {
                    _sockets.Add(conn);
                }
                //Queue recieving of data from the connection
                conn.socket.BeginReceive(conn.buffer, 0, conn.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), conn);
                //Queue the accept of the next incomming connection
                _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), _serverSocket);
            }
            catch (SocketException e)
            {
                if (conn.socket != null)
                {
                    conn.socket.Close();
                    lock (_sockets)
                    {
                        _sockets.Remove(conn);
                    }
                }
                //Queue the next accept, think this should be here, stop attacks based on killing the waiting listeners
                _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), _serverSocket);
            }
            catch (Exception e)
            {
                if (conn.socket != null)
                {
                    conn.socket.Close();
                    lock (_sockets)
                    {
                        _sockets.Remove(conn);
                    }
                }
                //Queue the next accept, think this should be here, stop attacks based on killing the waiting listeners
                _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), _serverSocket);
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            //get our connection from the callback
            xConnection conn = (xConnection)result.AsyncState;
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
                    //Queue the next receive
                    conn.socket.BeginReceive(conn.buffer, 0, conn.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), conn);
                }
                else
                {
                    //Callback run but no data, close the connection
                    //supposadly means a disconnect
                    //and we still have to close the socket, even though we throw the event later
                    conn.socket.Close();
                    lock (_sockets)
                    {
                        _sockets.Remove(conn);
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
                    lock (_sockets)
                    {
                        _sockets.Remove(conn);
                    }
                }
            }
        }

        protected virtual byte[] ProcessMessage(byte[] message)
        {
            return new byte[] { };
        }

    }
}
