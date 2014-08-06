using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using CprBroker.Engine;

namespace CprBroker.DBR
{
    public class DprDiversionServer : TcpServer
    {
        public DbrQueue DbrQueue;

        protected override byte[] ProcessMessage(byte[] message)
        {
            var req = DiversionRequest.Parse(message);
            if (req != null)
            {
                var ret = req.Process(this.DbrQueue.ConnectionString);
                return ret.ToBytes();
            }
            else
            {
                // Invalid request.
                // TODO: Handle invalid request
                return new byte[0];
            }
        }

        public class EqualityComparer : IEqualityComparer<DprDiversionServer>
        {
            public bool Equals(DprDiversionServer x, DprDiversionServer y)
            {
                //Check whether the compared objects reference the same data.
                if (Object.ReferenceEquals(x, y)) return true;

                //Check whether any of the compared objects is null.
                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;

                //Check whether the products' properties are equal.
                return x.DbrQueue.QueueId == y.DbrQueue.QueueId
                    && x.Port == y.Port;
            }

            public int GetHashCode(DprDiversionServer x)
            {
                //Check whether the object is null
                if (Object.ReferenceEquals(x, null))
                    return 0;

                return x.DbrQueue.QueueId.GetHashCode();
            }
        }
    }

    
}
