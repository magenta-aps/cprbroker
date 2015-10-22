using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.ServicePlatform;
using CprBroker.Providers.CprServices;
using CprBroker.Providers.ServicePlatform.Responses;
using CprBroker.Providers.CprServices.Responses;
using CprBroker.Schemas.Part;
using NUnit.Framework;

namespace CprBroker.Tests.ServicePlatform
{
    namespace ServicePlatformDataProviderTests
    {

        [TestFixture]
        public class PutSubscription : BaseResponseTests
        {
            [SetUp]
            public void InitContext()
            {
                CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "NUnit");
            }

            [Test]
            [TestCaseSource("PNRs")]
            public void PutSubscription_OK(string pnr)
            {
                var prov = ServicePlatformDataProviderFactory.Create();
                var ret = prov.PutSubscription(new Schemas.PersonIdentifier() { CprNumber = pnr, UUID = Guid.NewGuid() });
                Assert.True(ret);
            }
        }

        [TestFixture]
        public class ToRegistreringType1 : BaseResponseTests
        {
            public RegistreringType1 CallMethod(string pnr)
            {
                var cache = UuidCacheFactory.Create();

                var stamPlus = new StamPlusResponse(GetResponse(pnr, ServiceInfo.StamPlus_Local.Name));
                var familyPlus = new FamilyPlusResponse(GetResponse(pnr, ServiceInfo.FamilyPlus_Local.Name));

                var prov = new ServicePlatformDataProvider();
                return prov.ToRegistreringType1(stamPlus, familyPlus, null, cpr => cache.GetUuid(cpr));
            }

            [TestCaseSource("PNRs")]
            public void ToRegistreringType1_NotNull(string pnr)
            {
                Assert.NotNull(CallMethod(pnr));
            }

            [TestCaseSource("PNRs")]
            public void ToRegistreringType1_GenderOk(string pnr)
            {
                var ret = CallMethod(pnr);

                PersonGenderCodeType gender = (long.Parse(pnr) % 2) == 1 ? PersonGenderCodeType.male : PersonGenderCodeType.female;
                Assert.AreEqual(gender, ret.AttributListe.Egenskab.First().PersonGenderCode);
            }

            [Test]
            public void ToRegistreringType1_SomeHaveFather()
            {
                var ret = PNRs
                    .Select(pnr => CallMethod(pnr))
                    .Where(reg => reg.RelationListe.Fader != null
                        && reg.RelationListe.Fader.FirstOrDefault() != null
                        && !string.IsNullOrEmpty(reg.RelationListe.Fader.First().ReferenceID.Item));

                Assert.NotNull(ret.FirstOrDefault());
            }

            [Test]
            public void ToRegistreringType1_SomeHaveMother()
            {
                var ret = PNRs
                    .Select(pnr => CallMethod(pnr))
                    .Where(reg => reg.RelationListe.Moder != null
                        && reg.RelationListe.Moder.FirstOrDefault() != null
                        && !string.IsNullOrEmpty(reg.RelationListe.Moder.First().ReferenceID.Item));

                Assert.NotNull(ret.FirstOrDefault());
            }

            [Test]
            public void ToRegistreringType1_SomeHaveChildren()
            {
                var ret = PNRs
                    .Select(pnr => CallMethod(pnr))
                    .Where(reg => reg.RelationListe.Boern != null
                        && reg.RelationListe.Boern.FirstOrDefault() != null
                        && !string.IsNullOrEmpty(reg.RelationListe.Boern.First().ReferenceID.Item));

                Assert.NotNull(ret.FirstOrDefault());
            }

            [Test]
            public void ToRegistreringType1_SomeHaveGuardian()
            {
                var ret = PNRs
                    .Select(pnr => CallMethod(pnr))
                    .Where(reg => reg.RelationListe.RetligHandleevneVaergemaalsindehaver != null
                        && reg.RelationListe.RetligHandleevneVaergemaalsindehaver.FirstOrDefault() != null
                        && !string.IsNullOrEmpty(reg.RelationListe.RetligHandleevneVaergemaalsindehaver.First().ReferenceID.Item));

                Assert.NotNull(ret.FirstOrDefault());
            }

            [Test]
            public void ToRegistreringType1_SomeHaveSpouse()
            {
                var ret = PNRs
                    .Select(pnr => CallMethod(pnr))
                    .Where(reg => reg.RelationListe.Aegtefaelle != null
                        && reg.RelationListe.Aegtefaelle.FirstOrDefault() != null
                        && !string.IsNullOrEmpty(reg.RelationListe.Aegtefaelle.First().ReferenceID.Item));

                Assert.NotNull(ret.FirstOrDefault());
            }

            [Test]
            public void ToRegistreringType1_SomeHaveParentalAuthority()
            {
                var ret = PNRs
                    .Select(pnr => CallMethod(pnr))
                    .Where(reg => reg.RelationListe.Foraeldremyndighedsindehaver != null
                        && reg.RelationListe.Foraeldremyndighedsindehaver.FirstOrDefault() != null
                        && !string.IsNullOrEmpty(reg.RelationListe.Foraeldremyndighedsindehaver.First().ReferenceID.Item));

                Assert.NotNull(ret.FirstOrDefault());
            }

            [Test]
            public void ToRegistreringType1_SomeHaveParentalAuthorityFromParents()
            {
                var ret = PNRs
                    .Select(pnr => CallMethod(pnr))
                    .Where(
                        reg => reg.RelationListe.Foraeldremyndighedsindehaver != null
                        && reg.RelationListe.Foraeldremyndighedsindehaver.FirstOrDefault() != null
                        && reg.RelationListe.Foraeldremyndighedsindehaver.Where(
                            p=>                            
                                (reg.RelationListe.Fader.FirstOrDefault()!=null &&  p.ReferenceID.Item.Equals(reg.RelationListe.Fader.First().ReferenceID.Item))
                                || (reg.RelationListe.Moder.FirstOrDefault()!=null &&  p.ReferenceID.Item.Equals(reg.RelationListe.Moder.First().ReferenceID.Item))
                          ).Count()>0
                        );

                Assert.NotNull(ret.FirstOrDefault());
            }

            [Test]
            public void ToRegistreringType1_SomeHaveParentalAuthorityFromNonParents()
            {
                var ret = PNRs
                    .Select(pnr => CallMethod(pnr))
                    .Where(
                        reg => reg.RelationListe.Foraeldremyndighedsindehaver != null
                        && reg.RelationListe.Foraeldremyndighedsindehaver.FirstOrDefault() != null
                        && reg.RelationListe.Foraeldremyndighedsindehaver.Where(
                            p =>
                                (reg.RelationListe.Fader.FirstOrDefault() != null && !p.ReferenceID.Item.Equals(reg.RelationListe.Fader.First().ReferenceID.Item))
                                || (reg.RelationListe.Moder.FirstOrDefault() != null && !p.ReferenceID.Item.Equals(reg.RelationListe.Moder.First().ReferenceID.Item))
                          ).Count() > 0
                        );

                Assert.NotNull(ret.FirstOrDefault());
            }

            
        }
    }
}
