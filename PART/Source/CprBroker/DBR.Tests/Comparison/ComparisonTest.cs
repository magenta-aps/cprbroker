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

        public virtual bool IgnoreCount
        {
            get { return false; }
        }

        public abstract string[] LoadKeys();
        public abstract IQueryable<TObject> Get(TDataContext dataContext, string key);

        public PropertyInfo[] GetProperties()
        {
            var t = typeof(TObject);
            return DataLinq.GetColumnProperties(t)
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
                .Select(p => DataLinq.GetColumnName(p.Prop))
                .ToArray();
        }

        public virtual string[] GetOrderByColumnNames()
        {
            return GetPkColumnNames();
        }

        public string GetCorrectionMarkerColumnName()
        {
            Func<PropertyInfo, System.Data.Linq.Mapping.ColumnAttribute, string> columnNameGetter = (p, a) =>
            {
                if (string.IsNullOrEmpty(a.Name))
                    return p.Name;
                else
                    return a.Name;
            };

            return GetProperties()
                .Select(p => new { Prop = p, Attr = p.GetCustomAttributes(typeof(System.Data.Linq.Mapping.ColumnAttribute), true).FirstOrDefault() as System.Data.Linq.Mapping.ColumnAttribute })
                .Where(p => p.Prop.Name.ToLower().Equals("correctionmarker") || columnNameGetter(p.Prop, p.Attr).ToLower().Equals("annkor"))
                .Select(p => p.Attr.Name)
                .FirstOrDefault();
        }

        public void CompareProperty(TObject realObject, TObject fakeObject, PropertyInfo prop)
        {
            var r = prop.GetValue(realObject, null);
            var f = prop.GetValue(fakeObject, null);

            var stringTypes = new Type[]{
                typeof(string), typeof(char), typeof(char?)
            };
            Func<object, string> stringNormalizer = (o) =>
                string.Format("{0}", o).Trim().ToLower()
                .Replace(" ", "").Replace("-", "").Replace(",", "")
                .Replace("københavns", "københavn")
                .Replace("ä", "æ");

            if (stringTypes.Contains(prop.PropertyType))
            {
                r = stringNormalizer(r);
                f = stringNormalizer(f);
            }

            var decimalTypes = new Type[] { typeof(decimal), typeof(decimal?) };
            if (decimalTypes.Contains(prop.PropertyType))
            {
                Func<object, string> decimalStringNormalizer = (o) =>
                    {

                        var s = String.Format("{0}", o);
                        if (s.Equals("0"))
                            s = "";
                        return s;
                    };

                var sR = decimalStringNormalizer(r);
                var sF = decimalStringNormalizer(f);

                var pat = @"\A\d{12}\Z";

                if (Regex.Match(sR, pat).Success && Regex.Match(sF, pat).Success) // yyyyMMddHH99 // 196804011399
                {
                    r = decimal.Parse(sR.Substring(0, 8) + "0000");
                    f = decimal.Parse(sF.Substring(0, 8) + "0000");
                }
                else
                {
                    r = sR;
                    f = sF;
                }
            }
            Assert.AreEqual(r, f, "{0}({1}).{2}({3}): Expected <{4}> but was<{5}>",
                prop.DeclaringType.Name, DataLinq.GetTableName(prop.DeclaringType),
                prop.Name, DataLinq.GetColumnName(prop),
                r, f);
        }

        public abstract TDataContext CreateDataContext(string connectionString);


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
            if (!IgnoreCount)
            {
                var realObjects = Get(realDprDataContext, pnr).ToArray();
                var fakeObjects = Get(fakeDprDataContext, pnr).ToArray();
                Assert.GreaterOrEqual(fakeObjects.Length, realObjects.Length);
            }
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

            var numRows = 0;
            if (IgnoreCount)
                numRows = Math.Min(realObjects.Length, fakeObjects.Length);
            else
                numRows = realObjects.Length;

            for (int i = 0; i < numRows; i++)
            {
                var r = realObjects[i];
                var f = fakeObjects[i];
                CompareProperty(r, f, property);
            }
        }
    }
}