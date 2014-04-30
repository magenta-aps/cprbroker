using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Engine.Sync
{
    public abstract class SyncManager<TKey, TSourceObject, TTargetObject, TSourceStore, TTargetStore>
        where TSourceObject : ISourceDataStore<TKey, TSourceObject>
        where TTargetObject : ITargetDataStore<TKey, TTargetObject>
    {
        public int ConvertBatch(TSourceObject source, TTargetObject target, Converter<TSourceObject, TTargetObject> converter, int batchSize)
        {
            TKey[] keys = source.GetChangeKeys(batchSize);
            Local.Admin.LogFormattedSuccess("<{0}> changes found at store <{1}>, first = <{2}>", keys.Length, source, keys.FirstOrDefault());
            TSourceObject[] sourceObjects = source.GetObjects(keys);
            TTargetObject[] targetObjects = Array.ConvertAll<TSourceObject, TTargetObject>(sourceObjects, converter);
            target.PushChanges(keys, targetObjects);
            source.DeleteChanges(keys);
            Local.Admin.LogFormattedSuccess("Done pushing <{0}> changes to store <{1}>", keys.Length, target);

            return keys.Length;
        }

        public int ConvertAll(TSourceObject source, TTargetObject target, Converter<TSourceObject, TTargetObject> converter, int batchSize, int maxItems)
        {
            int convertedSoFar = 0;
            bool run = true;

            Local.Admin.LogFormattedSuccess("Requesting store <{0}> for <{1}> changes", source, batchSize);
            
            while (run && convertedSoFar < maxItems)
            {
                int myBatchSize = Math.Min(batchSize, maxItems-convertedSoFar);
                convertedSoFar += ConvertBatch(source, target, converter, myBatchSize);
            }
            return convertedSoFar;
        }
    }
}
