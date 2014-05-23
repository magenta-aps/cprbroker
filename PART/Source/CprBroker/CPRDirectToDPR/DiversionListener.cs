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
        
        public virtual void Process(IAsyncResult result)
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
    }
}
