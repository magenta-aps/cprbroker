using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace IndividualResponse
    {
        [TestFixture]
        public class ParseBatch
        {

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
