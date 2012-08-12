using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.CPRDirect;
using NUnit.Framework;

namespace CprBroker.Tests.CPRDirect.Persons
{
    [TestFixture]
    public class Person
    {
        public string GetPNR()
        {
            return this.GetType().Name.Substring(2, 10);
        }

        public string GetData()
        {
            var t = typeof(Properties.Resources);
            var ret = t.InvokeMember(
                string.Format("PNR_3112970079",GetPNR()),
                 System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                 null,
                 null,
                 new object[0]
                 );
            return ret.ToString();
        }

        public IndividualResponseType GetPerson()
        {
            var all = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
            return all.Where(p => p.PersonInformation.PNR == GetPNR()).First();
        }

        [Test]
        public static void Split()
        {
            var enc = Encoding.GetEncoding(1252);

            var txt = "";
            txt = Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE;
            //txt = System.IO.File.ReadAllText(@"C:\Magenta Workspace\PART\Doc\Data Providers\CPR Direct\Test data\U12170-P opgavenr 110901 ADRNVN FE", enc);
            var dataLines = LineWrapper.ParseBatch(txt);
            var allWrappers = dataLines.ToList();

            var startRecord = allWrappers.Where(lw => lw.Code == "000").First();
            var endRecord = allWrappers.Where(lw => lw.Code == "999").First();

            allWrappers.Remove(startRecord);
            allWrappers.Remove(endRecord);

            var groupedWrapers = allWrappers
                .Where(w => w != null)
                .GroupBy(w => w.PNR)
                .ToList();

            foreach (var individualWrappersGrouping in groupedWrapers)
            {
                var individualLines = individualWrappersGrouping.ToList();
                var pnr = individualWrappersGrouping.First().PNR;
                var myLines = new List<LineWrapper>(individualWrappersGrouping);
                myLines.Insert(0, startRecord);
                myLines.Add(endRecord);
                var txtLines = myLines.Select(lw => lw.Contents);
                System.IO.File.WriteAllLines(
                    string.Format(@"..\..\Resources\PNR_{0}.txt", pnr),
                    txtLines.ToArray(),
                    enc
                    );
            }
        }
    }
}
