using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Tests.CPRDirect;
using CprBroker.Schemas.Part;
using System.IO;
using CprBroker.Providers.ServicePlatform;
using CprBroker.Providers.ServicePlatform.Responses;
using System.Xml;

namespace CprBroker.Tests.ServicePlatform
{
    namespace FamilyPlusResponseTests
    {
        public class FamilyTestBase : BaseResponseTests
        {
            Dictionary<string, Guid> _UuidMap = new Dictionary<string, Guid>();
            public Guid GetUuid(string pnr)
            {
                if (!_UuidMap.ContainsKey(pnr))
                    _UuidMap[pnr] = Guid.NewGuid();
                return _UuidMap[pnr];
            }

            public RelationListeType GetRelations(string pnr)
            {
                var txt = GetResponse(pnr, "Familie+");
                var w = new FamilyPlusResponse(txt);
                return w.ToRelationListeType(GetUuid);
            }

            public string[] GetAllRelationTypes()
            {
                List<string> relTypes = new List<string>();
                foreach (var pnr in this.PNRs)
                {
                    //var ret3 = prov.Read(new PersonIdentifier() { CprNumber = pnr, UUID = Guid.NewGuid() }, null, p => Guid.NewGuid(), out ql);
                    var doc = new XmlDocument();
                    var path = string.Format(@"Resources\{0}.Familie+.Response.OK.xml", pnr);
                    doc.Load(path);
                    var nsMgr = new XmlNamespaceManager(doc.NameTable);
                    nsMgr.AddNamespace("s", "http://www.cpr.dk");
                    var nodes = doc.SelectNodes("//s:Field[@r='FAMMRK']/@v", nsMgr).OfType<XmlAttribute>().Select(a => a.Value).ToArray();
                    relTypes.AddRange(nodes);
                }
                relTypes = relTypes.Distinct().OrderBy(a => a).ToList();
                return relTypes.ToArray();
            }

            void MaxElements(string pnr, Func<RelationListeType, Array> func, int c)
            {
                var rel = GetRelations(pnr);
                Assert.LessOrEqual(func(rel).Length, c);
            }

            void SomeHasRelations(Func<RelationListeType, Array> func)
            {
                var rel = this.PNRs.Select(pnr => GetRelations(pnr)).ToArray().Select(r => func(r).Length).Sum();
                Assert.Greater(rel, 1);
            }

            [Test]
            [TestCaseSource("PNRs")]
            public void GetRelations_OneSpouseMax(string pnr)
            {
                MaxElements(pnr, rel => rel.Aegtefaelle, 1);
            }

            [Test]
            public void GetRelations_SomeHaveSpouses()
            {
                SomeHasRelations(rel => rel.Aegtefaelle);
            }

            [Test]
            [TestCaseSource("PNRs")]
            public void GetRelations_TenChildrenMax(string pnr)
            {
                MaxElements(pnr, rel => rel.Boern, 10);
            }

            [Test]
            public void GetRelations_SomeHaveChildren()
            {
                SomeHasRelations(rel => rel.Boern);
            }

            [Test]
            [TestCaseSource("PNRs")]
            public void GetRelations_OneFatherMax(string pnr)
            {
                MaxElements(pnr, rel => rel.Fader, 1);
            }

            [Test]
            public void GetRelations_SomeHaveFathers()
            {
                SomeHasRelations(rel => rel.Fader);
            }

            [Test]
            [TestCaseSource("PNRs")]
            public void GetRelations_OneMotherMax(string pnr)
            {
                MaxElements(pnr, rel => rel.Moder, 1);
            }

            [Test]
            public void GetRelations_SomeHaveMother()
            {
                SomeHasRelations(rel => rel.Moder);
            }
        }
    }
}
