using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas
{
    /// <summary>
    /// Used by CPR web service clients to pass their authentication tokens
    /// </summary>
    public class ApplicationHeader : System.Web.Services.Protocols.SoapHeader
    {
        public string ApplicationToken;
        public string UserToken;
    }
}
