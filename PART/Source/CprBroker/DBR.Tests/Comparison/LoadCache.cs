using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace CprBroker.Tests.DBR.Comparison
{
    class LoadCache
    {
        Dictionary<string, object> Cache = new Dictionary<string, object>();

        public T GetOrDefault<T>(string key)
        {
            if (Cache.ContainsKey(key))
                return (T)Cache[key];
            else
                return default(T);
        }

        public T GetOrLoad<T>(string key, Func<T> loader)
        {
            if (Cache.ContainsKey(key))
                return (T)Cache[key];
            else
            {
                var ret = loader();
                Cache[key] = ret;
                return ret;
            }
        }

        public void Reset(string key)
        {
            if (Cache.ContainsKey(key))
                Cache.Remove(key);
        }

        public bool GetBoolean(string key)
        {
            return GetOrDefault<bool>(key);
        }
    }

    public class DatabaseLoadCache
    {
        public static readonly DatabaseLoadCache Root = new DatabaseLoadCache();

        LoadCache Base = new LoadCache();

        public IQueryable<T> GetOrLoad<T>(string connectionString, string key, Func<IQueryable<T>> loader)
        {
            var dic = Base.GetOrLoad<Dictionary<string, object>>(connectionString, () => new Dictionary<string, object>());

            if (dic.ContainsKey(key))
                return (IQueryable<T>)dic[key];
            else
            {
                var ret = loader();
                dic[key] = ret;
                return ret;
            }
        }

        public IQueryable<TObject> GetOrLoad<TContext, TObject>(TContext dataContext, string key, Func<TContext, IQueryable<TObject>> loader)
            where TContext : DataContext
        {
            return GetOrLoad<TObject>(
                dataContext.Connection.ConnectionString,
                key,
                () => loader(dataContext)
                );
        }

        public void Reset(DataContext dataContext)
        {
            Base.Reset(dataContext.Connection.ConnectionString);
        }
    }

}
