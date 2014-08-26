using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using CprBroker.Utilities.Config;
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
        public static string CprBrokerConnectionString       = "data source=localhost\\sqlexpress; database=cprbroker;  integrated security=sspi";
        public static string RealDprDatabaseConnectionString = "data source=localhost\\sqlexpress; database=dbr_source; integrated security=sspi";
        public static string FakeDprDatabaseConnectionString = "data source=localhost\\sqlexpress; database=dbr_target; integrated security=sspi";
        
        static ComparisonTest()
        {
            BatchClient.Utilities.UpdateConnectionString(CprBrokerConnectionString);
        }
        
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