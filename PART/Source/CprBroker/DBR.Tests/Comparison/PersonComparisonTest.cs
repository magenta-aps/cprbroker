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
        private string[] _Keys = null;
        public override string[] LoadKeys()
        {
            if (_Keys == null)
            {
                using (var dataContext = new ExtractDataContext(CprBrokerConnectionString))
                {
                    _Keys = dataContext.ExecuteQuery<string>("select * FROM DbrPerson ORDER BY PNR").Skip(10).Take(10).ToArray();
                    //return dataContext.ExtractItems.Select(ei => ei.PNR).Distinct().ToArray();
                }
            }
            return _Keys;
        }

        Dictionary<string, bool> _ConvertedPersons = new Dictionary<string, bool>();
        public void ConvertPerson(string pnr)
        {
            if (!_ConvertedPersons.ContainsKey(pnr))
            {
                using (var fakeDprDataContext = new DPRDataContext(FakeDprDatabaseConnectionString))
                {
                    CprConverter.DeletePersonRecords(pnr, fakeDprDataContext);
                    fakeDprDataContext.SubmitChanges();
                    var person = ExtractManager.GetPerson(pnr);
                    CprConverter.AppendPerson(person, fakeDprDataContext);
                    fakeDprDataContext.SubmitChanges();
                }
                _ConvertedPersons[pnr] = true;
            }
        }

        public override sealed IQueryable<TObject> Get(DPRDataContext dataContext, string key)
        {
            ConvertPerson(key);
            var tableName = Utilities.DataLinq.GetTableName<TObject>();
            return dataContext.ExecuteQuery<TObject>("select * from " + tableName + " WHERE PNR={0}", key).AsQueryable();
        }

        public abstract IQueryable<TObject> Get(DPRDataContext dataContext, decimal pnr);


        [Test]
        [TestCaseSource("LoadKeys")]
        public void T1_CompareCount(string pnr)
        {
            using (var fakeDprDataContext = new DPRDataContext(FakeDprDatabaseConnectionString))
            {
                var fakeObjects = Get(fakeDprDataContext, pnr).ToArray();
                using (var realDprDataContext = new DPRDataContext(RealDprDatabaseConnectionString))
                {
                    var realObjects = Get(realDprDataContext, pnr).ToArray();
                    Assert.AreEqual(realObjects.Length, fakeObjects.Length);
                }
            }
        }

        public PropertyInfo[] GetProperties()
        {
            var t = typeof(TObject);
            return t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttributes(typeof(System.Data.Linq.Mapping.ColumnAttribute), true).FirstOrDefault() != null)
                .ToArray();
        }

        [Test]
        public void T2_CompareContents(
            [ValueSource("GetProperties")]PropertyInfo property,
            [ValueSource("LoadKeys")]string pnr)
        {
            using (var fakeDprDataContext = new DPRDataContext(FakeDprDatabaseConnectionString))
            {
                var fakeObjects = Get(fakeDprDataContext, pnr).ToArray();
                using (var realDprDataContext = new DPRDataContext(RealDprDatabaseConnectionString))
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
