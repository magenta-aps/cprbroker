using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Data.Queues;

namespace CprBroker.Providers.CPRDirect
{
    public class CprDirectExtractQueueItem : QueueItemBase
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

        public static void LoadExtractAndItems(this CprDirectExtractQueueItem[] items, ExtractDataContext dataContext)
        {
            foreach (var item in items)
            {
                item.Extract = dataContext.Extracts.Where(ex => ex.ExtractId == item.ExtractId).Single();
                item.ExtractItems = dataContext.ExtractItems.Where(ei => ei.ExtractId == item.ExtractId && ei.PNR == item.PNR);
            }
        }
    }
}
