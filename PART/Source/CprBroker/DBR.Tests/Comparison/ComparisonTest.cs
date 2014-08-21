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
    }

    public abstract class PersonComparisonTest<T> : ComparisonTest<T, DPRDataContext>
    {

        public override string[] LoadKeys()
        {
            using (var dataContext = new ExtractDataContext(CprBrokerConnectionString))
            {
                return dataContext.ExtractItems.Select(ei => ei.PNR).Distinct().ToArray();
            }
        }

        public override IQueryable<T> Get(DPRDataContext dataContext, string key)
        {
            return Get(dataContext, decimal.Parse(key));
        }

        public abstract IQueryable<T> Get(DPRDataContext dataContext, decimal pnr);

        [Test]
        [TestCaseSource("LoadPnrs")]
        public void Compare(string pnr)
        {
            var person = ExtractManager.GetPerson(pnr);
            var pnrDec = decimal.Parse(pnr);
            using (var fakeDprDataContext = new DPRDataContext(RealDprDatabaseConnectionString))
            {
                CprConverter.AppendPerson(person, fakeDprDataContext);

                var fakeObjects = Get(fakeDprDataContext, pnr).ToArray();
                using (var realDprDataContext = new DPRDataContext(FakeDprDatabaseConnectionString))
                {
                    var realObjects = Get(realDprDataContext, pnr).ToArray();

                    Assert.AreEqual(realObjects.Length, fakeObjects.Length);
                }
            }
        }

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

    public abstract class GeoLookupCityComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.Cities.Select(ei => ei.BYNVN).Distinct().ToArray();
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

    public abstract class GeoLookupAreaRestorationDistrictComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.AreaRestorationDistricts.Select(ei => ei.BYFORNYKOD).Distinct().ToArray();
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

    public abstract class GeoLookupPostDistrictComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.PostDistricts.Select(ei => ei.POSTNR.ToString()).Distinct().ToArray();
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

    public abstract class GeoLookupDiverseDistrictComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.DiverseDistricts.Select(ei => ei.DIVDISTKOD).Distinct().ToArray();
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

    public abstract class GeoLookupEvacuationDistrictComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.EvacuationDistricts.Select(ei => ei.EVAKUERKOD.ToString()).Distinct().ToArray();
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

    public abstract class GeoLookupChurchDistrictComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.ChurchDistricts.Select(ei => ei.KIRKEKOD.ToString()).Distinct().ToArray();
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

    public abstract class GeoLookupSchoolDistrictComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.SchoolDistricts.Select(ei => ei.SKOLEKOD.ToString()).Distinct().ToArray();
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

    public abstract class GeoLookupPopulationDistrictComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.PopulationDistricts.Select(ei => ei.BEFOLKKOD).Distinct().ToArray();
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

    public abstract class GeoLookupSocialDistrictComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.SocialDistricts.Select(ei => ei.SOCIALKOD.ToString()).Distinct().ToArray();
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

    public abstract class GeoLookupChurchAdministrationDistrictComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.ChurchAdministrationDistricts.Select(ei => ei.MYNKOD.ToString()).Distinct().ToArray();
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

    public abstract class GeoLookupElectionDistrictComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.ElectionDistricts.Select(ei => ei.VALGKOD.ToString()).Distinct().ToArray();
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

    public abstract class GeoLookupHeatingDistrictComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.HeatingDistricts.Select(ei => ei.VARMEKOD.ToString()).Distinct().ToArray();
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
