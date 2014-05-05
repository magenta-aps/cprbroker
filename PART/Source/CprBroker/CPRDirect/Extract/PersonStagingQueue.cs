using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Data.Queues;

namespace CprBroker.Providers.CPRDirect
{
    public class PersonStagingQueue : Data.Queues.Queue<CprDirectExtractQueueItem>
    {
        public static readonly Guid FixedQueueId = new Guid("{8682D62B-4A9B-4362-9EAD-3852D35008DA}");

        public PersonStagingQueue()
            : base(FixedQueueId)
        {

        }

        public override IEnumerable<CprDirectExtractQueueItem> Process(IEnumerable<CprDirectExtractQueueItem> items)
        {
            var dbr = new DbrQueue();
            var cpr = new PersonExtractionQueue();

            dbr.Enqueue(items);
            cpr.Enqueue(items);
            return items;
        }
    }    
}
