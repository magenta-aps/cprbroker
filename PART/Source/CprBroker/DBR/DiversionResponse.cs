using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;
using CprBroker.Schemas.Wrappers;

namespace CprBroker.DBR
{
    public abstract class DiversionResponse : Wrapper
    {
        public byte[] ToBytes()
        {
            return Constants.DiversionEncoding.GetBytes(this.Contents);
        }
    }
}
