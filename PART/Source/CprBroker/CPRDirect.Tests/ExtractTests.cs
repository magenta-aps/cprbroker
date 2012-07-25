using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace ExtractTests
    {
        [TestFixture]
        public class Constructor
        {
            [Test]
            public void Constructor_Parse_CorrectCount()
            {
                var lines = LineWrapper.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);

                var extract = new Extract(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE, Constants.DataObjectMap);
                Assert.AreEqual(lines.Length - 2, extract.ExtractItems.Count);
            }

            [Test]
            public void Constructor_Parse_CorrectStart()
            {
                var lines = LineWrapper.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);

                var extract = new Extract(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE, Constants.DataObjectMap);
                Assert.AreEqual(lines.First().Contents, extract.StartRecord);
            }

            [Test]
            public void Constructor_Parse_CorrectEnd()
            {
                var lines = LineWrapper.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);

                var extract = new Extract(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE, Constants.DataObjectMap);
                Assert.AreEqual(lines.Last().Contents, extract.EndRecord);
            }

            [Test]
            public void Constructor_Parse_CorrectReconstruction()
            {
                var lines = LineWrapper.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);

                var extract = new Extract(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE, Constants.DataObjectMap);
                var result = extract.ExtractItems.Select(i => i.Contents).ToList();
                result.Insert(0, extract.StartRecord);
                result.Add(extract.EndRecord);
                for (int i = 0; i < lines.Length; i++)
                {
                    Assert.AreEqual(lines[i].Contents, result[i]);
                }
            }
        }

        [TestFixture]
        public class GetPerson
        {
            [Test]
            public void GetPerson_PersonExists_Correct(
                [Range(1, 1098, 20)]int lineNumber)
            {
                var lines = LineWrapper.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                var extract = new Extract(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE, Constants.DataObjectMap);

                var pnr = lines[2].PNR;
                var person = Extract.GetPerson(pnr, extract.ExtractItems.AsQueryable(), Constants.DataObjectMap);
                Assert.NotNull(person);
                Assert.AreEqual(pnr, person.PersonInformation.PNR);
            }

            [Test]
            public void GetPerson_PersonNotExists_Null(
                [ValueSource(typeof(Utilities), "RandomCprNumberStrings5")]string cprNumber)
            {
                var lines = LineWrapper.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                var extract = new Extract(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE, Constants.DataObjectMap);

                var person = Extract.GetPerson(cprNumber, extract.ExtractItems.AsQueryable(), Constants.DataObjectMap);
                Assert.Null(person);
            }
        }
    }
}
