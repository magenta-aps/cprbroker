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
using System.Text.RegularExpressions;

namespace CprBroker.Tests.DBR.Comparison
{
    public abstract class ComparisonTestBase
    {
        static ComparisonTestBase()
        {
            CprBroker.Tests.PartInterface.Utilities.UpdateConnectionString(Properties.Settings.Default.CprBrokerConnectionString);
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

        public string[] CommonExcludedProperties
        {
            get
            {
                return new string[] { "CprUpdateDate" };
            }
        }

        public abstract string[] LoadKeys();
        public abstract IQueryable<TObject> Get(TDataContext dataContext, string key);

        public PropertyInfo[] GetProperties()
        {
            var t = typeof(TObject);
            return t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttributes(typeof(System.Data.Linq.Mapping.ColumnAttribute), true).FirstOrDefault() != null)
                .Where(p => !this.ExcludedProperties.Contains(p.Name) && !this.CommonExcludedProperties.Contains(p.Name))
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

            var stringTypes = new Type[]{
                typeof(string), typeof(char), typeof(char?)
            };
            if (stringTypes.Contains(prop.PropertyType))
            {
                r = string.Format("{0}", r).Trim();
                f = string.Format("{0}", f).Trim();
            }

            var decimalTypes = new Type[] { typeof(decimal), typeof(decimal?) };
            if (decimalTypes.Contains(prop.PropertyType))
            {
                var sR = String.Format("{0}", r);
                var sF = String.Format("{0}", f);
                var pat = @"\A\d{12}\Z";

                if (Regex.Match(sR, pat).Success && Regex.Match(sF, pat).Success) // yyyyMMddHH99 // 196804011399
                {
                    r = decimal.Parse(sR.Substring(0, 10) + "00");
                    f = decimal.Parse(sF.Substring(0, 10) + "00");
                }
            }

            Assert.AreEqual(r, f, "{0}.{1}: Expected <{2}> but was<{3}>", prop.DeclaringType.Name, prop.Name, r, f);
        }

        public abstract TDataContext CreateDataContext(string connectionString);

        [Test]
        [TestCaseSource("LoadKeys")]
        public void T0_Convert(string key)
        {
            ConvertObject(key);
        }

        [Test]
        [TestCaseSource("LoadKeys")]
        public void T1_CompareCount(string pnr)
        {
            using (var realDprDataContext = CreateDataContext(Properties.Settings.Default.RealDprConnectionString))
            {
                using (var fakeDprDataContext = CreateDataContext(Properties.Settings.Default.ImitatedDprConnectionString))
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
            using (var realDprDataContext = CreateDataContext(Properties.Settings.Default.RealDprConnectionString))
            {
                using (var fakeDprDataContext = CreateDataContext(Properties.Settings.Default.ImitatedDprConnectionString))
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