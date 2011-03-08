using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine.Ping
{
    public class PingManager
    {
        public static BasicOutputType<bool> Ping(string userToken, string appToken)
        {
            var facade = new PingFacadeMethod(appToken, userToken);
            return Manager.GetMethodOutput<bool>(facade);
        }
    }
}

