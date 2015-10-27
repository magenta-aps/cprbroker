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
        public class FamilyPlusResponseTestBase : BaseResponseTests
        {
            public FamilyPlusResponse GetResponse(string pnr)
            {
                var txt = GetResponse(pnr, "Familie+");
                var w = new FamilyPlusResponse(txt);
                return w;
            }
        }

        public class FamilyTestBase : FamilyPlusResponseTestBase
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
                return GetResponse(pnr).ToRelationListeType(GetUuid);
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

        [TestFixture]
        public class ToCivilStatusTests : FamilyPlusResponseTestBase
        {
            [Test]
            [TestCase(CivilStatusKodeType.Enke, Ignore = true)]
            [TestCase(CivilStatusKodeType.Gift)]
            [TestCase(CivilStatusKodeType.Laengstlevende, Ignore = true)]
            [TestCase(CivilStatusKodeType.OphaevetPartnerskab, Ignore = true)]
            [TestCase(CivilStatusKodeType.RegistreretPartner, Ignore = true)]
            [TestCase(CivilStatusKodeType.Separeret, Ignore = true)]
            [TestCase(CivilStatusKodeType.Skilt, Ignore = true)]
            [TestCase(CivilStatusKodeType.Ugift)]
            public void ToCivilStatus_AtLeastOnePerStatus(CivilStatusKodeType status)
            {
                var count = PNRs.Select(p => GetResponse(p).ToCivilStatusType().CivilStatusKode)
                    .Where(c => c == status)
                    .Count();
                Assert.Greater(count, 0);
            }
        }

        [TestFixture]
        public class ToRelationListeTypeTests : FamilyPlusResponseTestBase
        {
            public RelationListeType CallMethod(string pnr)
            {
                var cache = UuidCacheFactory.Create();

                var familyPlus = new FamilyPlusResponse(GetResponse(pnr, ServiceInfo.FamilyPlus_Local.Name));

                return familyPlus.ToRelationListeType(cpr => cache.GetUuid(cpr));
            }

            [Test]
            public void ToRegistreringType1_SomeHaveFather()
            {
                var ret = PNRs
                    .Select(pnr => CallMethod(pnr))
                    .Where(reg => reg.Fader != null
                        && reg.Fader.FirstOrDefault() != null
                        && !string.IsNullOrEmpty(reg.Fader.First().ReferenceID.Item));

                Assert.NotNull(ret.FirstOrDefault());
            }

            [Test]
            public void ToRegistreringType1_SomeHaveMother()
            {
                var ret = PNRs
                    .Select(pnr => CallMethod(pnr))
                    .Where(reg => reg.Moder != null
                        && reg.Moder.FirstOrDefault() != null
                        && !string.IsNullOrEmpty(reg.Moder.First().ReferenceID.Item));

                Assert.NotNull(ret.FirstOrDefault());
            }

            [Test]
            public void ToRegistreringType1_SomeHaveChildren()
            {
                var ret = PNRs
                    .Select(pnr => CallMethod(pnr))
                    .Where(reg => reg.Boern != null
                        && reg.Boern.FirstOrDefault() != null
                        && !string.IsNullOrEmpty(reg.Boern.First().ReferenceID.Item));

                Assert.NotNull(ret.FirstOrDefault());
            }

            [Test]
            [Ignore("Disempowerment not available in free services")]
            public void ToRegistreringType1_SomeHaveGuardian()
            {
                var ret = PNRs
                    .Select(pnr => CallMethod(pnr))
                    .Where(reg => reg.RetligHandleevneVaergemaalsindehaver != null
                        && reg.RetligHandleevneVaergemaalsindehaver.FirstOrDefault() != null
                        && !string.IsNullOrEmpty(reg.RetligHandleevneVaergemaalsindehaver.First().ReferenceID.Item));

                Assert.NotNull(ret.FirstOrDefault());
            }

            [Test]
            public void ToRegistreringType1_SomeHaveSpouse()
            {
                var ret = PNRs
                    .Select(pnr => CallMethod(pnr))
                    .Where(reg => reg.Aegtefaelle != null
                        && reg.Aegtefaelle.FirstOrDefault() != null
                        && !string.IsNullOrEmpty(reg.Aegtefaelle.First().ReferenceID.Item));

                Assert.NotNull(ret.FirstOrDefault());
            }

            [Test]
            public void ToRegistreringType1_SomeHaveParentalAuthority()
            {
                var ret = PNRs
                    .Select(pnr => CallMethod(pnr))
                    .Where(reg => reg.Foraeldremyndighedsindehaver != null
                        && reg.Foraeldremyndighedsindehaver.FirstOrDefault() != null
                        && !string.IsNullOrEmpty(reg.Foraeldremyndighedsindehaver.First().ReferenceID.Item));

                Assert.NotNull(ret.FirstOrDefault());
            }

            string[] GetParentUuids(RelationListeType reg)
            {
                var parentUuids = new List<string>();
                if (reg.Fader.FirstOrDefault() != null)
                    parentUuids.Add(reg.Fader.First().ReferenceID.Item);
                if (reg.Moder.FirstOrDefault() != null)
                    parentUuids.Add(reg.Moder.First().ReferenceID.Item);
                return parentUuids.ToArray();
            }

            [Test]
            public void ToRegistreringType1_SomeHaveParentalAuthorityFromParents()
            {
                var ret = PNRs
                    .Select(pnr => new { PNR = pnr, Rel = CallMethod(pnr) })
                    .Where(
                        reg => reg.Rel.Foraeldremyndighedsindehaver != null
                        && reg.Rel.Foraeldremyndighedsindehaver.FirstOrDefault() != null
                        && reg.Rel.Foraeldremyndighedsindehaver.Where(
                            p =>
                                GetParentUuids(reg.Rel).Contains(p.ReferenceID.Item)
                          ).Count() > 0
                        );
                Console.WriteLine("Found: " + ret.Count());
                foreach (var reg in ret)
                    Console.WriteLine(reg.PNR);
                Assert.NotNull(ret.FirstOrDefault());
            }

            [Test]
            public void ToRegistreringType1_SomeHaveParentalAuthorityFromNonParents()
            {
                var ret = PNRs
                    .Select(pnr => new { PNR = pnr, Rel = CallMethod(pnr) })
                    .Where(
                        reg => reg.Rel.Foraeldremyndighedsindehaver != null
                        && reg.Rel.Foraeldremyndighedsindehaver.FirstOrDefault() != null
                        && reg.Rel.Foraeldremyndighedsindehaver.Where(
                            p =>
                                !GetParentUuids(reg.Rel).Contains(p.ReferenceID.Item)
                          ).Count() > 0
                        );
                Console.WriteLine("Found: " + ret.Count());
                foreach (var reg in ret)
                    Console.WriteLine(reg.PNR);
                Assert.NotNull(ret.FirstOrDefault());
            }

        }
        
    }
}
