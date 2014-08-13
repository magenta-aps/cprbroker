using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;
using CprBroker.DBR;
using System.Reflection;

namespace CprBroker.Tests.DBR.Comparison
{
    public abstract class ComparisonTest<T>
    {
        public string CprBrokerConnectionString = "data source=localhost\\sqlexpress;initial catalog=part;integrated security=sspi";
        public string RealDprDatabaseConnectionString = "";
        public string FakeDprDatabaseConnectionString = "";

        public abstract IQueryable<T> Get(DPRDataContext dataContext, decimal pnr);

        public string[] LoadPnrs()
        {
            using (var dataContext = new ExtractDataContext(CprBrokerConnectionString))
            {
                return dataContext.ExtractItems.Select(ei => ei.PNR).Distinct().ToArray();
            }
        }

        [Test]
        [TestCaseSource("LoadPnrs")]
        public void Compare(string pnr)
        {
            var person = ExtractManager.GetPerson(pnr);
            var pnrDec = decimal.Parse(pnr);
            using (var fakeDprDataContext = new DPRDataContext(RealDprDatabaseConnectionString))
            {
                CprConverter.AppendPerson(person, fakeDprDataContext);

                var fakeObjects = Get(fakeDprDataContext, pnrDec).ToArray();
                using (var realDprDataContext = new DPRDataContext(FakeDprDatabaseConnectionString))
                {
                    var realObjects = Get(realDprDataContext, pnrDec).ToArray();

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
