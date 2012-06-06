using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Utilities;
using System.Net.Sockets;

namespace CprBroker.Providers.CPRDirect
{
    public partial class CPRDirectDataProvider
    {
        public IndividualResponseType GetResponse(IndividualRequestType request)
        {
            string response;
            string error;
            if (Send(request.Contents, out response, out error))
            {
                return new IndividualResponseType() { Contents = response };
            }
            else
            {
                throw new Exception(error);
            }
        }

        protected bool Send(string message, out string response, out string error)
        {
            error = null;
            NetworkStream stream = null;

            TcpClient client = new TcpClient(Address, Port);
            Byte[] data = Constants.DefaultEncoding.GetBytes(message);

            stream = client.GetStream();
            stream.Write(data, 0, data.Length);

            data = new Byte[3500];

            int bytes = stream.Read(data, 0, data.Length);
            response = Constants.DefaultEncoding.GetString(data, 0, bytes);


            string errorCode = response.Substring(2, 2);
            if (Constants.ErrorCodes.ContainsKey(errorCode))
            {
                error = Constants.ErrorCodes[errorCode];
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
