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
                var lines = Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                var extract = new Extract(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE, Constants.DataObjectMap);
                Assert.AreEqual(lines.Length - 2, extract.ExtractItems.Count);
            }

            [Test]
            public void Constructor_Parse_CorrectStart()
            {
                var lines = Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                var extract = new Extract(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE, Constants.DataObjectMap);
                Assert.AreEqual(lines[0], extract.StartRecord);
            }

            [Test]
            public void Constructor_Parse_CorrectEnd()
            {
                var lines = Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                var extract = new Extract(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE, Constants.DataObjectMap);
                Assert.AreEqual(lines.Last(), extract.EndRecord);
            }

            [Test]
            public void Constructor_Parse_CorrectReconstruction()
            {
                var lines = Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                var extract = new Extract(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE, Constants.DataObjectMap);
                var result = extract.ExtractItems.Select(i => i.Contents).ToList();
                result.Insert(0, extract.StartRecord);
                result.Add(extract.EndRecord);
                for (int i = 0; i < lines.Length; i++)
                {
                    Assert.AreEqual(lines[i], result[i]);
                }
            }
        }
    }
}
