using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;
using CprBroker.DBR;
using System.Reflection;
using CprBroker.Utilities;
using CprBroker.PartInterface;
namespace CprBroker.Tests.DBR.Comparison
{
    public abstract class ComparisonTest<TObject, TDataContext>
    {
        public string CprBrokerConnectionString = "data source=localhost\\sqlexpress;initial catalog=part;integrated security=sspi";
        public string RealDprDatabaseConnectionString = "";
        public string FakeDprDatabaseConnectionString = "";

        public abstract string[] LoadKeys();
        public abstract IQueryable<TObject> Get(TDataContext dataContext, string key);

        public void CompareObject(TObject fakeObject, TObject realObject)
        {
            var t = typeof(TObject);
            var props = t.GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(System.Data.Linq.Mapping.ColumnAttribute), true).FirstOrDefault() != null)
                .ToArray();
            foreach (var prop in props)
            {
                CompareProperty(fakeObject, realObject, prop);
            }
        }

        public void CompareProperty(TObject fakeObject, TObject realObject, PropertyInfo prop)
        {
            var f = prop.GetValue(fakeObject, null);
            var r = prop.GetValue(realObject, null);
            Assert.AreEqual(r, f, "{0}.{1}: Expected <{2} but was<{3}>", prop.DeclaringType.Name, prop.Name, r, f);
        }
    }

}
