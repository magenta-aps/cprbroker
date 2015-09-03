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
    [Category("PersonComparison")]
    public abstract class PersonComparisonTest<TObject> : ComparisonTest<TObject, DPRDataContext>
        where TObject : new()
    {
        public override string[] LoadKeys()
        {
            if (KeysHolder._Keys == null)
            {
                if (CprBroker.Tests.PartInterface.Utilities.IsConsole)
                {
                    Console.WriteLine("Loading PNRS");
                    using (var dataContext = new DPRDataContext(Properties.Settings.Default.RealDprConnectionString))
                    {
                        KeysHolder._Keys = dataContext
                            .PersonTotals
                            .Select(p => p.PNR)
                            .ToArray()
                            .Select(p => p.ToPnrDecimalString())
                            .ToArray();
                        Console.WriteLine("Found PNRs <{0}>", KeysHolder._Keys.Count());
                    }
                }
                else
                {
                    Random r = new Random();
                    using (var w = new System.IO.StreamWriter(string.Format("c:\\logs\\Compare-{0}-{1}-{2}.log", GetType().Name, DateTime.Now.ToString("yyyyMMdd HHmmss"), r.Next())))
                    {
                        w.AutoFlush = true;
                        w.WriteLine("Loading from <{0}>", Properties.Settings.Default.ImitatedDprConnectionString);
                        try
                        {
                            using (var dataContext = new System.Data.Linq.DataContext(Properties.Settings.Default.ImitatedDprConnectionString))
                            {
                                dataContext.Log = w;
                                KeysHolder._Keys = dataContext.ExecuteQuery<decimal>("select PNR FROM DTTOTAL ORDER BY PNR")
                                    .Skip(Properties.Settings.Default.PersonComparisonSampleSkip)
                                    .Take(Properties.Settings.Default.PersonComparisonSampleSize)
                                    .ToArray()
                                    .Select(pnr => pnr.ToPnrDecimalString())
                                    .ToArray();
                                w.WriteLine("Loaded <{0}> PNRs", KeysHolder._Keys.Length);
                            }
                        }
                        catch (Exception ex)
                        {
                            w.WriteLine(ex.ToString());
                            throw;
                        }
                    }
                }
            }
            return KeysHolder._Keys;
        }

        public override void ConvertObject(string pnr)
        {
            using (var fakeDprDataContext = new DPRDataContext(Properties.Settings.Default.ImitatedDprConnectionString))
            {
                CprConverter.DeletePersonRecords(pnr, fakeDprDataContext);
                fakeDprDataContext.SubmitChanges();
                var person = ExtractManager.GetPerson(pnr);
                CprConverter.AppendPerson(person, fakeDprDataContext);
                fakeDprDataContext.SubmitChanges();
            }
            KeysHolder._ConvertedPersons[pnr] = true;
        }

        public override IQueryable<TObject> Get(DPRDataContext dataContext, string key)
        {
            var tableName = Utilities.DataLinq.GetTableName<TObject>();
            var propNames = string.Join(", ", GetPkColumnNames());
            return dataContext.Fill<TObject>(string.Format("select * from " + tableName + " WHERE PNR={0} ORDER BY " + propNames, key)).AsQueryable();
        }

        public override DPRDataContext CreateDataContext(string connectionString)
        {
            return new DPRDataContext(connectionString);
        }

    }
}
