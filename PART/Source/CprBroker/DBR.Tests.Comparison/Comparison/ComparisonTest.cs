﻿using System;
using System.Linq;
using NUnit.Framework;
using System.Reflection;
using CprBroker.Utilities;
using System.Text.RegularExpressions;
using CprBroker.Tests.DBR.ComparisonResults;
using CprBroker.Providers.DPR;

namespace CprBroker.Tests.DBR.Comparison
{
    public abstract class ComparisonTestBase
    {
        static ComparisonTestBase()
        {
            CprBroker.Tests.PartInterface.Utilities.UpdateConnectionString(Properties.Settings.Default.CprBrokerConnectionString);
        }
    }

    [Category("Comnparison")]
    public abstract class ComparisonTest<TObject, TDataContext> : ComparisonTestBase, IComparisonType
        where TDataContext : System.Data.Linq.DataContext
    {

        public virtual PropertyComparisonResult[] ExcludedPropertiesInformation
        {
            get
            {
                return new PropertyComparisonResult[] { };
            }
        }

        public Type TargetType
        {
            get
            {
                return typeof(TObject);
            }
        }

        public PropertyInfo[] DataProperties()
        {
            return DataLinq.GetColumnProperties(typeof(TObject));
        }

        public string SourceName
        {
            get
            {
                return Utilities.DataLinq.GetTableName(typeof(TObject));
            }
        }

        public string[] ExcludedPropertyNames
        {
            get
            {
                return PropertyComparisonResult
                    .ExcludedAlways(ExcludedPropertiesInformation)
                    .Select(p => p.PropertyName).ToArray();
            }
        }

        public string[] ExcludedPropertyNamesIfInactive
        {
            get
            {
                return PropertyComparisonResult
                    .OfStatus(ExcludedPropertiesInformation, ExclusionStatus.Dead)
                    .Select(p => p.PropertyName).ToArray();
            }
        }

        public string[] CommonExcludedProperties
        {
            get
            {
                return new string[] {
                    "CprUpdateDate",
                    "PNR"
                };
            }
        }

        public virtual bool IgnoreCount
        {
            get { return false; }
        }

        public virtual void NormalizeObject(TObject realObject, string realConnectionString,
            TObject imitatedObject, string imitatedConnectionString)
        {

        }

        public abstract string[] LoadKeys();
        public abstract IQueryable<TObject> Get(TDataContext dataContext, string key);

        public PropertyInfo[] GetProperties()
        {
            var t = typeof(TObject);
            return DataLinq.GetColumnProperties(t)
                .Where(p => !this.ExcludedPropertyNames.Contains(p.Name) && !this.CommonExcludedProperties.Contains(p.Name))
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
                .Replace(" ", "")
                .Replace("-", "")
                .Replace(",", "")
                .Replace("københavns", "københavn")
                .Replace("adresseikkekomplet", "")
                .Replace("ü", "y")
                .Replace("ä", "æ")
                .Replace("ö", "ø")
                .Replace("é", "e")
                .Replace("á", "a");


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

                if (Regex.Match(sR, pat).Success || Regex.Match(sF, pat).Success) // yyyyMMddHH99 // 196804011399
                {
                    r = decimal.Parse(sR.Substring(0, Math.Min(8, sR.Length)) + "0000");
                    f = decimal.Parse(sF.Substring(0, Math.Min(8, sF.Length)) + "0000");
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
        [TestCaseSource(nameof(LoadKeys))]
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

        //public virtual void ConvertObject(string key)
        //{ }
        public virtual bool IsActiveRecord(string key, TDataContext fakeDprDataContext)
        {
            return true;
        }

        [Test]
        public void T2_CompareContents(
            [ValueSource(nameof(GetProperties))]PropertyInfo property,
            [ValueSource(nameof(LoadKeys))]string key)
        {
            using (var realDprDataContext = CreateDataContext(Properties.Settings.Default.RealDprConnectionString))
            {
                using (var fakeDprDataContext = CreateDataContext(Properties.Settings.Default.ImitatedDprConnectionString))
                {
                    if (ExcludedPropertyNamesIfInactive.Contains(property.Name))
                    {
                        if (IsActiveRecord(key, fakeDprDataContext))
                            CompareContents(property, key, realDprDataContext, fakeDprDataContext);
                        else
                            Console.WriteLine("Inactive object - Comparison skipped");
                    }
                    else
                    {
                        CompareContents(property, key, realDprDataContext, fakeDprDataContext);
                    }   
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
                this.NormalizeObject(
                    r, Properties.Settings.Default.RealDprConnectionString,
                    f, Properties.Settings.Default.ImitatedDprConnectionString);
                CompareProperty(r, f, property);
            }
        }
    }
}