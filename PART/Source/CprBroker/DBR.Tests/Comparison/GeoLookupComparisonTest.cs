using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NUnit.Framework;

namespace CprBroker.Tests.DBR.Comparison
{
    public abstract class GeoLookupComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public sealed override IQueryable<T> Get(Providers.DPR.LookupDataContext dataContext, string key)
        {
            return Get(dataContext, decimal.Parse(key));
        }

        public abstract IQueryable<T> Get(Providers.DPR.LookupDataContext dataContext, decimal key);

        public override Providers.DPR.LookupDataContext CreateDataContext(string connectionString)
        {
            return new Providers.DPR.LookupDataContext(connectionString);
        }
    }
}
