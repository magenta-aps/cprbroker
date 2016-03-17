using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect.Partials
{
    [TestFixture]
    class PartialRecordTests
    {
        Guid CprToUuid(string pnr)
        {
            return Guid.NewGuid();
        }

        [Test]
        public void ToRegistreringType1(
            [ValueSource(typeof(Utilities), "PNRs")]string pnr)
        {
            System.Diagnostics.Debugger.Launch();
            var all = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
            var pers = all.Where(p => p.PersonInformation.PNR == pnr).First();

            //Todo: This was the original
            //pers.HistoricalCivilStatus.Clear();
            //pers.CurrentCivilStatus = null;

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

            pers.Events = null;  //099 ?
            pers.ErrorRecord = null;  //910 ?
            pers.SubscriptionDeletionReceipt = null;  //997 ?

            //What about 999?

            var registration = pers.ToRegistreringType1(CprToUuid);
        }

        [Test]
        public void ToFiltreretOejebliksbilledeType(
            [ValueSource(typeof(Utilities), "PNRs")]string pnr)
        {
            System.Diagnostics.Debugger.Launch();
            var historyResponse = new IndividualHistoryResponseType(
                new Schemas.PersonIdentifier() { CprNumber = pnr, UUID = Guid.NewGuid() },
                (new IndividualResponseType[] { CprBroker.Tests.CPRDirect.Persons.Person.GetPerson(pnr) }).AsQueryable());
            var pers = historyResponse.IndividualResponseObjects.First();
            //historyResponse.IndividualResponseObjects.ToList().RemoveAt(0);

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

            pers.Events = null;  //099 ?
            pers.ErrorRecord = null;  //910 ?
            pers.SubscriptionDeletionReceipt = null;  //997 ?

            //What about 999?
            var registration = historyResponse.ToFiltreretOejebliksbilledeType(CprToUuid);

        }
    }
}
