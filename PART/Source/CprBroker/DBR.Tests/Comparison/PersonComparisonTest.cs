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
        private static string[] _Keys = null;
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

        public override IQueryable<TObject> Get(DPRDataContext dataContext, string key)
        {
            ConvertPerson(key);
            var tableName = Utilities.DataLinq.GetTableName<TObject>();
            return dataContext.ExecuteQuery<TObject>("select * from " + tableName + " WHERE PNR={0}", key).AsQueryable();
        }

        public override DPRDataContext CreateDataContext(string connectionString)
        {
            return new DPRDataContext(connectionString);
        }

    }
}
