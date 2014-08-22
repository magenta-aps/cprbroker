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
    public abstract class PersonComparisonTest<TObject> : ComparisonTest<TObject, DPRDataContext>
    {

        public override string[] LoadKeys()
        {
            using (var dataContext = new ExtractDataContext(CprBrokerConnectionString))
            {
                return dataContext.ExtractItems.Select(ei => ei.PNR).Distinct().ToArray();
            }
        }

        public override IQueryable<TObject> Get(DPRDataContext dataContext, string key)
        {
            return Get(dataContext, decimal.Parse(key));
        }

        public abstract IQueryable<TObject> Get(DPRDataContext dataContext, decimal pnr);

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

    }
}
