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
    public abstract class ComparisonTestBase
    {
        public static string CprBrokerConnectionString = "data source=tcp:ltkcprtest\\sqlexpress; database=cprbroker;  integrated security=sspi";
        public static string RealDprDatabaseConnectionString = "data source=tcp:ltkcprtest\\sqlexpress; database=dbr_source; integrated security=sspi";
        public static string FakeDprDatabaseConnectionString = "data source=tcp:ltkcprtest\\sqlexpress; database=dbr_target; integrated security=sspi";

        static ComparisonTestBase()
        {
            CprBroker.Tests.PartInterface.Utilities.UpdateConnectionString(CprBrokerConnectionString);
            if (CprBroker.Tests.PartInterface.Utilities.IsConsole)
            {
                // TODO: Uncomment for nunit-console.exe

                //Console.Write("Real DPR connection string :");
                //RealDprDatabaseConnectionString = Console.ReadLine();

                //Console.Write("DBR (Generated) DPR connection string :");
                //FakeDprDatabaseConnectionString = Console.ReadLine();
            }
        }
    }

    [Category("Comnparison")]
    public abstract class ComparisonTest<TObject, TDataContext> : ComparisonTestBase
        where TDataContext : System.Data.Linq.DataContext
    {

        public virtual string[] ExcludedProperties
        {
            get
            {
                return new string[] { };
            }
        }

        public abstract string[] LoadKeys();
        public abstract IQueryable<TObject> Get(TDataContext dataContext, string key);

        public PropertyInfo[] GetProperties()
        {
            var t = typeof(TObject);
            return t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttributes(typeof(System.Data.Linq.Mapping.ColumnAttribute), true).FirstOrDefault() != null)
                .Where(p => !this.ExcludedProperties.Contains(p.Name))
                .OrderBy(p => p.Name)
                .ToArray();
        }

        public string[] GetPkColumnNames()
        {
            var t = typeof(TObject);
            return t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new { Prop = p, Attr = p.GetCustomAttributes(typeof(System.Data.Linq.Mapping.ColumnAttribute), true).FirstOrDefault() as System.Data.Linq.Mapping.ColumnAttribute })
                .Where(p => p.Attr != null && p.Attr.IsPrimaryKey)
                .Select(p => string.IsNullOrEmpty(p.Attr.Name) ? p.Prop.Name : p.Attr.Name)
                .ToArray();
        }

        public void CompareProperty(TObject realObject, TObject fakeObject, PropertyInfo prop)
        {
            var r = prop.GetValue(realObject, null);
            var f = prop.GetValue(fakeObject, null);
            Assert.AreEqual(r, f, "{0}.{1}: Expected <{2}> but was<{3}>", prop.DeclaringType.Name, prop.Name, r, f);
        }

        public abstract TDataContext CreateDataContext(string connectionString);

        [Test]
        [TestCaseSource("LoadKeys")]
        public void T1_CompareCount(string pnr)
        {
            using (var realDprDataContext = CreateDataContext(RealDprDatabaseConnectionString))
            {
                using (var fakeDprDataContext = CreateDataContext(FakeDprDatabaseConnectionString))
                {
                    CompareCount(pnr, realDprDataContext, fakeDprDataContext);
                }
            }
        }

        public void CompareCount(string pnr, TDataContext realDprDataContext, TDataContext fakeDprDataContext)
        {
            var realObjects = Get(realDprDataContext, pnr).ToArray();
            var fakeObjects = Get(fakeDprDataContext, pnr).ToArray();
            Assert.GreaterOrEqual(fakeObjects.Length, realObjects.Length);
        }

        public virtual void ConvertObject(string key)
        { }

        [Test]
        public void T2_CompareContents(
            [ValueSource("GetProperties")]PropertyInfo property,
            [ValueSource("LoadKeys")]string key)
        {
            ConvertObject(key);
            using (var realDprDataContext = CreateDataContext(RealDprDatabaseConnectionString))
            {
                using (var fakeDprDataContext = CreateDataContext(FakeDprDatabaseConnectionString))
                {
                    CompareContents(property, key, realDprDataContext, fakeDprDataContext);
                }
            }
        }

        public void CompareContents(PropertyInfo property, string key, TDataContext realDprDataContext, TDataContext fakeDprDataContext)
        {
            var realObjects = Get(realDprDataContext, key).ToArray();
            var fakeObjects = Get(fakeDprDataContext, key).ToArray();
            for (int i = 0; i < realObjects.Length; i++)
            {
                var r = realObjects[i];
                var f = fakeObjects[i];
                CompareProperty(r, f, property);
            }
        }
    }
}