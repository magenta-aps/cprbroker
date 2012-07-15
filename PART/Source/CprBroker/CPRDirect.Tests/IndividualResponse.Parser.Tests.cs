using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace IndividualResponseTests
    {
        [TestFixture]
        public class ParseBatch
        {
            [Test]
            public void ParseAll___()
            {

                var rd = new System.IO.StringReader(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);

                var dataLines = rd.ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Where(l => l.Length >= 3);
                var allLineWrappers = dataLines.Select(line => new LineWrapper(line));

                var start = allLineWrappers.Where(w => w.IntCode == 0).First();
                var end = allLineWrappers.Where(w => w.IntCode == 999).First();

                allLineWrappers = allLineWrappers.Where(w => w.IntCode != 0 && w.IntCode != 999).ToArray();

                var groupedWrapers = allLineWrappers
                    .GroupBy(w => w.PNR);

                foreach (var group in groupedWrapers)
                {
                    var list = new List<LineWrapper>(group.ToList());
                    list.Insert(0, start);
                    list.Add(end);

                    var pnrLines = list.Select(w => w.Contents).ToArray();
                    var data = string.Join("\r\n", pnrLines.ToArray());
                }
            }

            [Test]
            public void ParseBatch_ChangeExtract_80Persons()
            {
                var result = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                Assert.AreEqual(80, result.Count);
            }

            [Test]
            public void ParseBatch_ChangeExtract_AllHasStartRecord(
                [Range(0, 79)]int index)
            {
                var result = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                Assert.NotNull(result[index].StartRecord);
            }

            [Test]
            public void ParseBatch_ChangeExtract_AllShareStartRecord()
            {
                var result = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                Assert.AreEqual(1, result.Select(ind => ind.StartRecord).Distinct().Count());
            }

            [Test]
            public void ParseBatch_ChangeExtract_AllHasEndRecord(
                [Range(0, 79)]int index)
            {
                var result = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                Assert.NotNull(result[index].EndRecord);
            }

            [Test]
            public void ParseBatch_ChangeExtract_AllShareEndRecord()
            {
                var result = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                Assert.AreEqual(1, result.Select(ind => ind.EndRecord).Distinct().Count());
            }
        }
    }
}
