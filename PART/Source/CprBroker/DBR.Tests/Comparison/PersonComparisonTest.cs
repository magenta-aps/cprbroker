using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;
using System.Reflection;
using CprBroker.DBR;

namespace CprBroker.Tests.DBR.Comparison
{
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
}
