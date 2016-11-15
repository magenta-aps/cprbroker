using CprBroker.Data.Queues;
using CprBroker.Engine.Queues;
using CprBroker.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.PartInterface.Tracking
{
    public class CleanupQueueItem : IQueueItem
    {
        public Guid PersonUuid { get; set; }
        public string PNR { get; set; }

        public DbQueueItem Impl { get; set; }

        public void DeserializeFromKey(string key)
        {
            var arr = key.Split('|');
            PersonUuid = new Guid(arr[0]);
            PNR = arr[1];
        }

        public string SerializeToKey()
        {
            return string.Format("{0}|{1}",
                PersonUuid,
                string.Format("{0}", PNR).PadLeft(10, '0')
                );
        }

        public PersonIdentifier ToPersonIdentifier()
        {
            return new PersonIdentifier()
            {
                UUID = PersonUuid,
                CprNumber = PNR,
            };
        }
    }
}
