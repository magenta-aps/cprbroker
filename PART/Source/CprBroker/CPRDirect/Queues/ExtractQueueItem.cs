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
            if (dataContext.LoadOptions == null)
            {
                var loadOptions = new System.Data.Linq.DataLoadOptions();
                loadOptions.LoadWith<ExtractItem>(ei => ei.Extract);
                dataContext.LoadOptions = loadOptions;
            }

            var pnrs = items.Select(i => i.PNR).Distinct().ToArray();            
            var dbExtractItems = dataContext.ExtractItems.Where(ei => pnrs.Contains(ei.PNR)).ToArray();

            foreach (var item in items)
            {
                item.ExtractItems = dbExtractItems.Where(ei => ei.ExtractId == item.ExtractId && ei.PNR == item.PNR).ToArray().AsQueryable();
                item.Extract = item.ExtractItems.First().Extract;
            }
        }
    }
}
