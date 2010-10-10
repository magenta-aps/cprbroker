using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Schemas.Part
{
    public class EffectDateHeader : System.Web.Services.Protocols.SoapHeader
    {
        public DateTime? EffectDate { get; set; }
    }
}
