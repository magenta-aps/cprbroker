using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    public class Program
    {
        public static void Main()
        {
            //BulkImport();
            //GetPersons();
            CprBroker.Engine.BrokerContext.Initialize("fd56ff6b-35bc-4b67-8ae4-bdc4485dc429", "");
            ExtractManager.ImportDataProviderFolders();
        }

        public static void BulkImport()
        {
            int batchCount = 100;
            var personRepeates = 2;

            var data = Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE;

            var lines = new List<LineWrapper>(LineWrapper.ParseBatch(data));
            var start = lines.First();
            var end = lines.Last();

            lines.RemoveAt(0);
            lines.RemoveAt(lines.Count - 1);

            var groupsByPerson = lines.GroupBy(l => l.PNR).ToArray();

            for (int iBatch = 0; iBatch < batchCount; iBatch++)
            {
                Console.WriteLine(string.Format("Creating batch <{0}> of <{1}>", iBatch + 1, batchCount));

                StringBuilder fullData = new StringBuilder(data.Length * personRepeates);
                fullData.AppendLine(start.Contents);


                for (int iPerson = 0; iPerson < groupsByPerson.Count(); iPerson++)
                {
                    var personLines = groupsByPerson[iPerson];
                    Console.WriteLine(string.Format("Inserting person<{0}> of <{1}>", iPerson + 1, groupsByPerson.Count()));

                    var personData = new List<LineWrapper>(personLines.ToArray());
                    var pnr = personData[0].PNR;
                    for (int iRepeat = 0; iRepeat < personRepeates; iRepeat++)
                    {
                        Console.WriteLine(string.Format("Batch <{0}> of <{1}> ; person<{2}> of <{3}> repeat <{4}> of <{5}>", iBatch + 1, batchCount, iPerson + 1, groupsByPerson.Count(), iRepeat + 1, personRepeates));
                        var newPnr = Utilities.RandomCprNumberString();
                        var personText = string.Join(Environment.NewLine, personData.Select(l => l.Contents).ToArray());
                        personText = personText.Replace(pnr, newPnr);
                        fullData.AppendLine(personText);
                    }
                }
                fullData.AppendLine(end.Contents);

                ExtractManager.ImportText(fullData.ToString());
            }
        }


        public static void GetPersons()
        {
            //var result = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
            var count = 1000;
            var pnrs = new List<string>();
            using (ExtractDataContext dataContext = new ExtractDataContext())
            {
                pnrs = dataContext.ExtractItems.Select(item => item.CprNumber).Distinct().ToList();
            }
            DateTime start = DateTime.Now;
            for (int i = 0; i < count; i++)
            {
                var index = Utilities.Random.Next(0, pnrs.Count);
                Console.WriteLine(string.Format("Loading <{0}> of <{1}> batches", i + 1, count));
                string pnr = pnrs[index];
                var db = ExtractManager.GetPerson(pnr);
                //var oio = db.ToRegistreringType1(cpr => Guid.NewGuid(), DateTime.Today);
            }
            Console.WriteLine(DateTime.Now - start);
            Console.ReadLine();
        }
    }
}
