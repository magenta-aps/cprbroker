using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Data.Applications
{
    partial class OperationType
    {
        public enum Types
        {
            Generic = 1,
            Read = 2,
            Search = 3,
            GetUuid = 4,
            ReadPeriod = 5,
            PutSubscription = 6,
            Subscribe = 8,
            Unsubscribe = 9,
            ListSubscriptions = 10

        }
    }
}
