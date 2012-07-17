using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.CPRDirect
{
    namespace HistoricalCivilStatusTests
    {
        [TestFixture]
        public class LoadAll
        {
            [Test]
            public void LoadAll____()
            {
                var result = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                var groupings = result
                    .Where(p=>p.PersonInformation.PNR == "0708614319")
                    .GroupBy(res => res.HistoricalCivilStatus.Count)
                    .Select(g =>
                        new
                        {
                            Count = g.Key,
                            Persons = g
                            .Select(p => new
                            {
                                PNR = p.PersonInformation.PNR,
                                Current = p.CurrentCivilStatus,
                                History = p.HistoricalCivilStatus,
                                Spouses = p.ToSpouses(cpr => Guid.NewGuid()),
                                Partners = p.ToRegisteredPartners(cpr => Guid.NewGuid())
                            })
                            .ToArray()
                        })
                    .ToArray();
                
                object o = "";
            }
        }
    }
}
