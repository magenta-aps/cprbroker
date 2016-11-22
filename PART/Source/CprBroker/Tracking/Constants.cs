using CprBroker.Data.Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.PartInterface.Tracking
{
    public static class Constants
    {
        public static readonly OperationType.Types[] PersonUsageOperations = new OperationType.Types[]
        {
            OperationType.Types.Read,
            OperationType.Types.ReadPeriod,
            OperationType.Types.DprDiversion
        };
    }
}
