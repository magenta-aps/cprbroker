using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CPRDirect
{
    public partial class IndividualRequestType
    {
        public IndividualRequestType(bool putSubscription, decimal pnr)
        {
            this.SubscriptionType = (decimal)((putSubscription) ? CPRDirect.SubscriptionType.PutSubscription : CPRDirect.SubscriptionType.LeaveAsIs);
            this.DataType = (decimal)CPRDirect.DataType.DefinedByTask;
            this.PNR = pnr;
        }
    }
}
