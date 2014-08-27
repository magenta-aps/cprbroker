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
        where TDataContext : System.Data.Linq.DataContext
    {
        public static string CprBrokerConnectionString = "data source=localhost\\sqlexpress; database=cprbroker;  integrated security=sspi";
        public static string RealDprDatabaseConnectionString = "data source=localhost\\sqlexpress; database=dbr_source; integrated security=sspi";
        public static string FakeDprDatabaseConnectionString = "data source=localhost\\sqlexpress; database=dbr_target; integrated security=sspi";

        public virtual string[] ExcludedProperties { get; }

        static ComparisonTest()
        {
            BatchClient.Utilities.UpdateConnectionString(CprBrokerConnectionString);
        }

        public abstract string[] LoadKeys();
        public abstract IQueryable<TObject> Get(TDataContext dataContext, string key);

        public PropertyInfo[] GetProperties()
        {
            var t = typeof(TObject);
            return t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttributes(typeof(System.Data.Linq.Mapping.ColumnAttribute), true).FirstOrDefault() != null)
                .Where(p=> !this.ExcludedProperties.Contains(p.Name))
                .ToArray();
        }

        public void CompareProperty(TObject fakeObject, TObject realObject, PropertyInfo prop)
        {
            var f = prop.GetValue(fakeObject, null);
            var r = prop.GetValue(realObject, null);
            Assert.AreEqual(r, f, "{0}.{1}: Expected <{2} but was<{3}>", prop.DeclaringType.Name, prop.Name, r, f);
        }

        public abstract TDataContext CreateDataContext(string connectionString);

        [Test]
        [TestCaseSource("LoadKeys")]
        public void T1_CompareCount(string pnr)
        {
            using (var fakeDprDataContext = CreateDataContext(FakeDprDatabaseConnectionString))
            {
                var fakeObjects = Get(fakeDprDataContext, pnr).ToArray();
                using (var realDprDataContext = CreateDataContext(RealDprDatabaseConnectionString))
                {
                    var realObjects = Get(realDprDataContext, pnr).ToArray();
                    Assert.AreEqual(realObjects.Length, fakeObjects.Length);
                }
            }
        }

        [Test]
        public void T2_CompareContents(
            [ValueSource("GetProperties")]PropertyInfo property,
            [ValueSource("LoadKeys")]string pnr)
        {
            using (var fakeDprDataContext = CreateDataContext(FakeDprDatabaseConnectionString))
            {
                var fakeObjects = Get(fakeDprDataContext, pnr).ToArray();
                using (var realDprDataContext = CreateDataContext(RealDprDatabaseConnectionString))
                {
                    var realObjects = Get(realDprDataContext, pnr).ToArray();
                    for (int i = 0; i < Math.Min(realObjects.Length, fakeObjects.Length); i++)
                    {
                        var r = realObjects[i];
                        var f = fakeObjects[i];
                        CompareProperty(f, r, property);
                    }
                }
            }
        }
    }
}