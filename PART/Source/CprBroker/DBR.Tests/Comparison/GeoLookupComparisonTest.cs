using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NUnit.Framework;
using CprBroker.Providers.DPR;
using System.Data.Linq;

namespace CprBroker.Tests.DBR.Comparison.Geo
{
    public abstract class GeoLookupComparisonTest<T> : ComparisonTest<T, LookupDataContext>
        where T : class
    {
        public sealed override IQueryable<T> Get(Providers.DPR.LookupDataContext dataContext, string key)
        {
            return Get(dataContext, decimal.Parse(key));
        }

        public abstract IQueryable<T> Get(Providers.DPR.LookupDataContext dataContext, decimal key);

        public override sealed Providers.DPR.LookupDataContext CreateDataContext(string connectionString)
        {
            return new Providers.DPR.LookupDataContext(connectionString);
        }

        public override sealed string[] LoadKeys()
        {
            using (var dataContext = new LookupDataContext(RealDprDatabaseConnectionString))
            {
                var propType = typeof(Table<>).MakeGenericType(typeof(T));
                var prop = dataContext.GetType().GetProperties().Where(p => p.PropertyType == propType).Single();
                var objects = prop.GetValue(dataContext, null) as Table<T>;
                var arr = objects.Skip(10).Take(10).ToArray();
                return arr.Select(o => GetKey(o)).ToArray();
            }
        }

        public abstract string GetKey(T obj);

    }
}
