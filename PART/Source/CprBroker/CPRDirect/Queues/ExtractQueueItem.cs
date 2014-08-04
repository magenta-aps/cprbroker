using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine.Queues;

namespace CprBroker.Providers.CPRDirect
{
    public class ExtractQueueItem : QueueItemBase
    {
        public Guid ExtractId { get; set; }
        public string PNR { get; set; }

        public Extract Extract { get; internal set; }
        public IQueryable<ExtractItem> ExtractItems { get; internal set; }


        public override string SerializeToKey()
        {
            return string.Format("{0}|{1}", ExtractId, PNR);
        }

        public override void DeserializeFromKey(string key)
        {
            var arr = key.Split('|');
            ExtractId = new Guid(arr[0]);
            PNR = arr[1];
        }
    }

    public static class CprDirectExtractQueueItemExtensions
    {

        public static void LoadExtractAndItems(this ExtractQueueItem[] items, ExtractDataContext dataContext)
        {
            foreach (var item in items)
            {
                item.Extract = dataContext.Extracts.Where(ex => ex.ExtractId == item.ExtractId).Single();
                // TODO: Change into something faster and avoid ToArray() but do not use a similar to this :
                //         item.ExtractItems = dataContext.ExtractItems.Where(ei => ei.ExtractId == item.ExtractId && ei.PNR == item.PNR)
                item.ExtractItems = dataContext.ExtractItems.Where(ei => ei.ExtractId == item.ExtractId && ei.PNR == item.PNR).ToArray().AsQueryable();
            }
        }
    }
}
