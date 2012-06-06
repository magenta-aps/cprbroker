using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Utilities;

namespace CprBroker.Providers.CPRDirect
{
    public partial class CPRDirectDataProvider
    {
        public void Send(bool subscriptionType,int dataType,string pnr)
        {
            IndividualRequestType request = new IndividualRequestType();
            
        }
    }
}
