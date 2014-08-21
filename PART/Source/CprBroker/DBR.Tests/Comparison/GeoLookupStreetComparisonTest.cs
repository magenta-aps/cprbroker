using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NUnit.Framework;

namespace CprBroker.Tests.DBR.Comparison
{
    public abstract class GeoLookupStreetComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.Streets.Select(ei => ei.VEJKOD.ToString()).Distinct().ToArray();
            }
        }

        public override IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, string key)
        {
            return Get(dataContext, decimal.Parse(key));
        }

        public abstract IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, decimal pnr);

        public void CompareObject(T fakeObject, T realObject)
        {
            var t = typeof(T);
            var props = t.GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(System.Data.Linq.Mapping.ColumnAttribute), true).FirstOrDefault() != null)
                .ToArray();
            foreach (var prop in props)
            {
                CompareProperty(fakeObject, realObject, prop);
            }
        }

        public void CompareProperty(T fakeObject, T realObject, PropertyInfo prop)
        {
            var f = prop.GetValue(fakeObject, null);
            var r = prop.GetValue(realObject, null);
            Assert.AreEqual(r, f, "{0}.{1}: Expected <{2} but was<{3}>", prop.DeclaringType.Name, prop.Name, r, f);
        }
    }
}
