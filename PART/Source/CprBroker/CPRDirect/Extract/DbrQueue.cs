using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Data;
using CprBroker.Data.Queues;

namespace CprBroker.Providers.CPRDirect
{
    public class DbrQueue : Data.Queues.Queue<CprDirectExtractQueueItem>
    {
        public static readonly Guid FixedQueueId = new Guid("{E7A5C2DA-5179-4A7D-A12F-CA430F3AF19F}");

        public DbrQueue()
            : base(FixedQueueId)
        { }

        public override CprDirectExtractQueueItem[] Process(CprDirectExtractQueueItem[] items)
        {
            throw new NotImplementedException();
        }
    }
}
