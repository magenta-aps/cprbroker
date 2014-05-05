using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Data.Queues;

namespace CprBroker.Providers.CPRDirect
{
    public class PersonExtractionQueue : Data.Queues.Queue<CprDirectExtractQueueItem>
    {
        public static readonly Guid FixedQueueId = new Guid("{474A4EDE-A749-4CE1-9E06-227873FF9D50}");

        public PersonExtractionQueue()
            : base(FixedQueueId)
        {

        }

        public override IEnumerable<CprDirectExtractQueueItem> Process(IEnumerable<CprDirectExtractQueueItem> items)
        {
            throw new NotImplementedException();
        }
    }
}
