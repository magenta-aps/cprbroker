using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.ServicePlatform;
using CprBroker.Providers.ServicePlatform.Responses;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.ServicePlatform
{
    namespace StamPlusResponseTests
    {
        public class StamPlusTests : BaseResponseTests
        {
            public StamPlusResponse GetResponse(string pnr)
            {
                var txt = GetResponse(pnr, "Stam+");
                var w = new StamPlusResponse(txt);
                return w;
            }
        }

        public class StamPlusTests_Concrete : StamPlusTests
        {
            [Test]
            [TestCaseSource("PNRs")]
            public void Response_OneRow(string pnr)
            {
                var resp = GetResponse(pnr);
                Assert.AreEqual(1, resp.RowItems.Count());
            }
        }

        [TestFixture]
        public class ToAttributListTests : StamPlusTests
        {
            public AttributListeType GetAttributes(string pnr)
            {
                return GetResponse(pnr).ToAttributListeType();
            }

            public CprBorgerType GetCprBorger(string pnr)
            {
                return GetAttributes(pnr).RegisterOplysning.First().Item as CprBorgerType;
            }
            public AdresseType GetAddress(string pnr)
            {
                return GetCprBorger(pnr).FolkeregisterAdresse;
            }
            public NavnStrukturType GetName(string pnr)
            {
                return GetAttributes(pnr).Egenskab.First().NavnStruktur;
            }

            [Test]
            [TestCase(typeof(DanskAdresseType))]
            //[TestCase(typeof(GroenlandAdresseType))]
            [TestCase(typeof(VerdenAdresseType))]
            public void ToAttributList_HasOneAddressOfType(Type t)
            {
                var matchTypes = this.PNRs
                    .Select(p => GetAddress(p))
                    .Where(a => a != null && t.IsInstanceOfType(a.Item));
                Assert.Greater(matchTypes.Count(), 0);
            }

            bool HasNoName(string pnr)
            {
                return new string[] { "0101005038", "3006980014" }.Contains(pnr);
            }

            [Test]
            [TestCaseSource("PNRs")]
            public void ToAttributList_AddressingNameNotEmpty(string pnr)
            {
                if (HasNoName(pnr))
                    return;
                var name = GetName(pnr);
                Assert.IsNotEmpty(name.PersonNameForAddressingName);
            }

            [Test]
            [TestCaseSource("PNRs")]
            public void ToAttributList_NameNotEmpty(string pnr)
            {
                if (HasNoName(pnr))
                    return;
                var name = GetName(pnr);
                Assert.False(name.PersonNameStructure.IsEmpty);
            }
        }

        [TestFixture]
        public class ToLivStatusTypeTests : StamPlusTests
        {
            [Test]
            [TestCaseSource("PNRs")]
            public void ToLivStatusType_NotNull(string pnr)
            {
                var liv = GetResponse(pnr).RowItems.First().ToLivStatusType();
                Assert.NotNull(liv);
            }

            [Test]
            public void ToLivStatusType_AtLeastOneHasStartDate()
            {
                var liv = this.PNRs
                    .Select(pnr => GetResponse(pnr).RowItems.First().ToLivStatusType().TilstandVirkning.ToVirkningType().FraTidspunkt.ToDateTime())
                    .Where(d => d.HasValue)
                    .Count();

                Assert.Greater(liv, 0);
            }

            [Test]
            [TestCase(LivStatusKodeType.Doed)]
            [TestCase(LivStatusKodeType.Foedt)]
            [TestCase(LivStatusKodeType.Forsvundet)]
            [TestCase(LivStatusKodeType.Prenatal, Ignore = true)]
            public void ToLivStatusType_AtLeastOneOfType(LivStatusKodeType statusCode)
            {
                var liv = this.PNRs
                    .Select(pnr => GetResponse(pnr).RowItems.First().ToLivStatusType().LivStatusKode)
                    .Where(code => code == statusCode)
                    .Count();

                Console.WriteLine("{0} {1}", statusCode, liv);
                Assert.Greater(liv, 0);
            }
        }

    }
}
