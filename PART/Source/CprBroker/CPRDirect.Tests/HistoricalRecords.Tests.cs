using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    [TestFixture]
    public class HistoricalRecordsTests
    {
        [Test]
        public void SSS()
        {
            var persons = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
            var ordered = persons.OrderByDescending(p =>
                {
                    var ret = 0;
                    ret += p.HistoricalCivilStatus.Count;
                    ret += p.HistoricalName != null ? 1 : 0;
                    return ret;
                });
            var allLines = LineWrapper.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE)
                .Where(l => l.IntCode > 0 && l.IntCode < 99)
                .GroupBy(l => l.PNR)
                .Select(p=> new { Lines = p.OrderBy(l=>l.IntCode).Select(l=>l.Contents).ToArray(), History = p.ToArray().Where(l=>l.IntCode >= 22 && l.IntCode<=30).Count()})                
                .OrderByDescending(p => p.History)                
                .ToArray();

            System.Diagnostics.Debugger.Launch();
            var mostHistory = allLines.First();
            var o = "";
        }
    }
}
