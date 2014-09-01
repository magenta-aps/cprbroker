using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;
using System.Reflection;
using CprBroker.DBR;

namespace CprBroker.Tests.DBR.Comparison.Person
{
    public abstract class PersonComparisonTest<TObject> : ComparisonTest<TObject, DPRDataContext>
    {

        public override string[] LoadKeys()
        {
            if (KeysHolder._Keys == null)
            {
                using (var dataContext = new ExtractDataContext(CprBrokerConnectionString))
                {
                    KeysHolder._Keys = dataContext.ExecuteQuery<string>("select * FROM DbrPerson ORDER BY PNR").Skip(10).Take(10).ToArray();
                    //return dataContext.ExtractItems.Select(ei => ei.PNR).Distinct().ToArray();
                }
            }
            return KeysHolder._Keys;
        }

        public void ConvertPerson(string pnr)
        {
            if (!KeysHolder._ConvertedPersons.ContainsKey(pnr))
            {
                using (var fakeDprDataContext = new DPRDataContext(FakeDprDatabaseConnectionString))
                {
                    CprConverter.DeletePersonRecords(pnr, fakeDprDataContext);
                    fakeDprDataContext.SubmitChanges();
                    var person = ExtractManager.GetPerson(pnr);
                    CprConverter.AppendPerson(person, fakeDprDataContext);
                    fakeDprDataContext.SubmitChanges();
                }
                KeysHolder._ConvertedPersons[pnr] = true;
            }
        }

        public override IQueryable<TObject> Get(DPRDataContext dataContext, string key)
        {
            ConvertPerson(key);
            var tableName = Utilities.DataLinq.GetTableName<TObject>();
            var propNames = string.Join(", ", GetPkColumnNames());
            Console.WriteLine(propNames);
            return dataContext.ExecuteQuery<TObject>("select * from " + tableName + " WHERE PNR={0} ORDER BY " + propNames, key).AsQueryable();
        }

        public override DPRDataContext CreateDataContext(string connectionString)
        {
            return new DPRDataContext(connectionString);
        }

    }
}
