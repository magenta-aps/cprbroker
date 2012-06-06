using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CPRDirect
{
    public enum SubscriptionType
    {
        LeaveAsIs = 0,
        PutSubscription = 1,
        DeleteSubscription = 3,
        LogonTransaction = 9
    }

    public enum DataType
    {
        NoData = 0,
        DefinedByTask = 6
    }
}
