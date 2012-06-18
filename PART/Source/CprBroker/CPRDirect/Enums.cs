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
        /// <summary>
        /// No data available, ie. Response individual is composed solely of the 28 characters in the header, see Annex 1 . Used if necessary as proof of subscription sentence
        /// </summary>
        NoData = 0,
        
        /// <summary>
        /// Personal information according to the task parameters (see Annex 3)
        /// </summary>
        DefinedByTask = 6
    }
}
