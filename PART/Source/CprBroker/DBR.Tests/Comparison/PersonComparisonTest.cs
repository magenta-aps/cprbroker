using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;
using System.Reflection;
using CprBroker.DBR;
using CprBroker.Utilities;

namespace CprBroker.Tests.DBR.Comparison.Person
{
    public abstract class PersonComparisonTest<TObject> : ComparisonTest<TObject, DPRDataContext>
        where TObject : new()
    {
        int timesRun = 0;
        int randomTestNumber;
        public override string[] LoadKeys()
        {
            if (KeysHolder._Keys == null)
            {
                using (var dataContext = new ExtractDataContext(CprBrokerConnectionString))
                {
                    if (timesRun < 1)
                    {
                        Random random = new Random();
                        randomTestNumber = random.Next(85416); // We have 85426 records in the Extract table.
                        timesRun++;
                    }
                    else
                        timesRun = 0;
                    Console.WriteLine("NUMBER: " + randomTestNumber);
                    KeysHolder._Keys = dataContext.ExecuteQuery<string>("select * FROM DbrPerson ORDER BY PNR").Skip(5678).Take(10).ToArray();
                    //return dataContext.ExtractItems.Select(ei => ei.PNR).Distinct().ToArray();
                }
            }
            return KeysHolder._Keys;
        }

        public override void ConvertObject(string pnr)
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
            var tableName = Utilities.DataLinq.GetTableName<TObject>();
            var propNames = string.Join(", ", GetPkColumnNames());
            Console.WriteLine(propNames);
            return dataContext.Fill<TObject>(string.Format("select * from " + tableName + " WHERE PNR={0} ORDER BY " + propNames, key)).AsQueryable();
        }

        public override DPRDataContext CreateDataContext(string connectionString)
        {
            return new DPRDataContext(connectionString);
        }

    }
}
