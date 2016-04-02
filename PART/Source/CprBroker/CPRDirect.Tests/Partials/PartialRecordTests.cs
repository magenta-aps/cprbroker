using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas.Part;
using CprBroker.Engine.Part;
using CprBroker.Utilities;

namespace CprBroker.Tests.CPRDirect.Partials
{
    [TestFixture]
    class PartialRecordTests
    {
        UuidCache UuidCache = new UuidCache()
        {
            GetUuidMethod = (pnr) => Guid.NewGuid(),
            GetUuidArrayMethod = (pnrs) => pnrs.Select(pnr => Guid.NewGuid() as Guid?).ToArray(),
        };

        Guid CprToUuid(string pnr)
        {
            return UuidCache.GetUuid(pnr);
        }

        private static void ClearOptionalRecords(IndividualResponseType pers)
        {
            //Todo: Beemen should review these.
            //Do the data correspond to the record numbers?
            //I'm mostly in doubt regarding the ones marked with '?'.
            pers.CurrentDepartureData = null;  //005 ?
            pers.CurrentDisappearanceInformation = null;  //007
            pers.BirthRegistrationInformation = null;  //009
            pers.CurrentCitizenship = null;  //010
            pers.ChurchInformation = null;  //011
            pers.CurrentCivilStatus = null;  //012
            pers.CurrentSeparation = null;  //013
            pers.MunicipalConditions = null;  //018
            pers.Notes = null;  //019
            pers.ElectionInformation = null;  //020
            pers.RelocationOrder = null;  //021

            pers.HistoricalPNR.Clear();  //022 ?
            pers.HistoricalAddress.Clear();  //023
            pers.HistoricalDeparture.Clear();  //024 ?
            pers.HistoricalDisappearance.Clear();  //025
            pers.HistoricalName.Clear();  //026
            pers.HistoricalCitizenship.Clear();  //027
            pers.HistoricalChurchInformation.Clear();  //028
            pers.HistoricalCivilStatus.Clear();  //029
            pers.HistoricalSeparation.Clear();  //030

            pers.Events.Clear();  //099 ?
            pers.ErrorRecord = null;  //910 ?
            pers.SubscriptionDeletionReceipt.Clear();  //997 ?
        }

        readonly string[] PersonsWithoutNames = new string[] { "0101005038", "3006980014" };
        
        [Test]
        public void ToRegistreringType1_Reduced_HasNameAddressPnr(
            [ValueSource(typeof(Utilities), "PNRs")]string pnr)
        {
            var all = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
            var pers = all.Where(p => p.PersonInformation.PNR == pnr).First();

            ClearOptionalRecords(pers);

            var registration = pers.ToRegistreringType1(CprToUuid);

            // Make the newest objects come at index 0
            registration.OrderByStartDate(false);

            var borgerType = (CprBorgerType)registration.AttributListe.RegisterOplysning[0].Item;

            if (borgerType.PersonNummerGyldighedStatusIndikator) // The CPR number is still active
            {
                Assert.IsNotNull(borgerType.PersonCivilRegistrationIdentifier);
                StringAssert.AreEqualIgnoringCase(pnr, borgerType.PersonCivilRegistrationIdentifier);

                if (pers.PersonInformation.Status != 70 && pers.PersonInformation.Status != 80 && pers.PersonInformation.Status != 90) // Not disappeared, emigrated or dead
                    Assert.IsNotNull(borgerType.FolkeregisterAdresse);
            }
            if (!PersonsWithoutNames.Contains(pnr)) // two persons with no current name
                Assert.False(registration.AttributListe.Egenskab[0].NavnStruktur.PersonNameStructure.IsEmpty, "Name not found");
            Assert.IsNotNull(registration.TilstandListe.LivStatus, "Life status not found");
        }

        [Test]
        public void ToRegistreringType1_Reduced_CompareWithFull(
            [ValueSource(typeof(Utilities), "PNRs")]string pnr)
        {
            var all = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
            var pers = all.Where(p => p.PersonInformation.PNR == pnr).First();
            var registrationBefore = pers.ToRegistreringType1(CprToUuid);
            registrationBefore.OrderByStartDate(false);
            var borgerTypeBefore = (CprBorgerType)registrationBefore.AttributListe.RegisterOplysning[0].Item;

            ClearOptionalRecords(pers);

            var registration = pers.ToRegistreringType1(CprToUuid);

            // Make the newest objects come at index 0
            registration.OrderByStartDate(false);

            var borgerType = (CprBorgerType)registration.AttributListe.RegisterOplysning[0].Item;

            Assert.AreNotSame(borgerType, borgerTypeBefore);
            Assert.AreNotSame(registration, registrationBefore);
            if (pers.PersonInformation.Status == 1 || pers.PersonInformation.Status == 3)
            {
                StringAssert.AreEqualIgnoringCase(
                    Strings.SerializeObject(borgerTypeBefore.FolkeregisterAdresse.Item),
                    Strings.SerializeObject(borgerType.FolkeregisterAdresse.Item)
                );
                StringAssert.AreEqualIgnoringCase(
                    Strings.SerializeObject(borgerTypeBefore.PersonCivilRegistrationIdentifier),
                    Strings.SerializeObject(borgerType.PersonCivilRegistrationIdentifier)
                );
                StringAssert.AreEqualIgnoringCase(
                    Strings.SerializeObject(registrationBefore.AttributListe.Egenskab[0].NavnStruktur),
                    Strings.SerializeObject(registration.AttributListe.Egenskab[0].NavnStruktur)
                );
            }

            StringAssert.AreEqualIgnoringCase(
                    Strings.SerializeObject(registrationBefore.TilstandListe.LivStatus),
                    Strings.SerializeObject(registration.TilstandListe.LivStatus)
                );
        }

        [Test]
        public void ToFiltreretOejebliksbilledeType_Reduced_HasNameAddressPnr(
            [ValueSource(typeof(Utilities), "PNRs")]string pnr)
        {
            var historyResponse = new IndividualHistoryResponseType(
                new Schemas.PersonIdentifier() { CprNumber = pnr, UUID = Guid.NewGuid() },
                (new IndividualResponseType[] { CprBroker.Tests.CPRDirect.Persons.Person.GetPerson(pnr) }).AsQueryable());
            var pers = historyResponse.IndividualResponseObjects.First();

            ClearOptionalRecords(pers);

            //What about 999?
            var registration = historyResponse.ToFiltreretOejebliksbilledeType(CprToUuid);

            // Make the newest objects come at index 0
            //registration.OrderByStartDate(false);

            var borgerType = (CprBorgerType)registration.AttributListe.RegisterOplysning.Last().Item;

            if (borgerType.PersonNummerGyldighedStatusIndikator) // The CPR number is still active
            {
                Assert.IsNotNull(borgerType.PersonCivilRegistrationIdentifier);
                StringAssert.AreEqualIgnoringCase(pnr, borgerType.PersonCivilRegistrationIdentifier);

                if (pers.PersonInformation.Status != 70 && pers.PersonInformation.Status != 80 && pers.PersonInformation.Status != 90) // Not disappeared, emigrated or dead
                    Assert.IsNotNull(borgerType.FolkeregisterAdresse);
            }
            if (!PersonsWithoutNames.Contains(pnr))
                Assert.False(registration.AttributListe.Egenskab[0].NavnStruktur.PersonNameStructure.IsEmpty);
        }
        
        [Test]
        public void ToFiltreretOejebliksbilledeType_Reduced_CompareBeforeAndAfterClear(
            [ValueSource(typeof(Utilities), "PNRs")]string pnr)
        {
            var historyResponse = new IndividualHistoryResponseType(
                new Schemas.PersonIdentifier() { CprNumber = pnr, UUID = Guid.NewGuid() },
                (new IndividualResponseType[] { CprBroker.Tests.CPRDirect.Persons.Person.GetPerson(pnr) }).AsQueryable());
            var historyResponseBefore = new IndividualHistoryResponseType(
                new Schemas.PersonIdentifier() { CprNumber = pnr, UUID = Guid.NewGuid() },
                (new IndividualResponseType[] { CprBroker.Tests.CPRDirect.Persons.Person.GetPerson(pnr) }).AsQueryable());
            var pers = historyResponse.IndividualResponseObjects.First();

            ClearOptionalRecords(pers);

            //What about 999?
            var registration = historyResponse.ToFiltreretOejebliksbilledeType(CprToUuid);
            var registrationBefore = historyResponseBefore.ToFiltreretOejebliksbilledeType(CprToUuid);
            // Make the newest objects come at index 0
            //registration.OrderByStartDate(false);

            var borgerType = (CprBorgerType)registration.AttributListe.RegisterOplysning.Last().Item;
            var borgerTypeBefore = (CprBorgerType)registrationBefore.AttributListe.RegisterOplysning.Last().Item;

            Assert.AreNotSame(borgerType, borgerTypeBefore);
            Assert.AreNotSame(registration, registrationBefore);
            if (pers.PersonInformation.Status <= 7)
            {
                Console.WriteLine("{0} {1}", pnr, pers.PersonInformation.Status);
                StringAssert.AreEqualIgnoringCase(
                    Strings.SerializeObject(borgerTypeBefore.FolkeregisterAdresse.Item),
                    Strings.SerializeObject(borgerType.FolkeregisterAdresse.Item)
                );
                StringAssert.AreEqualIgnoringCase(
                    Strings.SerializeObject(borgerTypeBefore.PersonCivilRegistrationIdentifier),
                    Strings.SerializeObject(borgerType.PersonCivilRegistrationIdentifier)
                );
                StringAssert.AreEqualIgnoringCase(
                    Strings.SerializeObject(registrationBefore.AttributListe.Egenskab.Last().NavnStruktur),
                    Strings.SerializeObject(registration.AttributListe.Egenskab.Last().NavnStruktur)
                );
            }
        }
    }
}
