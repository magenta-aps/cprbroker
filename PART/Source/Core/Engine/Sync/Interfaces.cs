using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Engine.Sync
{
    public interface IDataStore<TKey>
    {

    }

    public interface ISourceDataStore<TKey, TObject> : IDataStore<TKey>
    {
        TKey[] GetChangeKeys(int maxCount = 1);
        TObject[] GetObjects(TKey[] keys);
        void DeleteChanges(TKey[] changes);
    }

    public interface ITargetDataStore<TKey, TObject> : IDataStore<TKey>
    {
        void PushChanges(TKey[] changes, TObject[] objects);
    }

}
