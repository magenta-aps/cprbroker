using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.DBR
{
    public class CprConverter
    {

        public static void DeletePersonRecords(string cprNumber, DPRDataContext dataContext)
        {
            decimal pnr = decimal.Parse(cprNumber);

            dataContext.PersonTotals.DeleteAllOnSubmit(dataContext.PersonTotals.Where(t => t.PNR == pnr));
            dataContext.Persons.DeleteAllOnSubmit(dataContext.Persons.Where(t => t.PNR == pnr));
            dataContext.Childs.DeleteAllOnSubmit(dataContext.Childs.Where(t => t.ParentPNR == pnr));
            dataContext.PersonNames.DeleteAllOnSubmit(dataContext.PersonNames.Where(t => t.PNR == pnr));
            dataContext.CivilStatus.DeleteAllOnSubmit(dataContext.CivilStatus.Where(t => t.PNR == pnr));
            dataContext.Separations.DeleteAllOnSubmit(dataContext.Separations.Where(t => t.PNR == pnr));
            dataContext.Nationalities.DeleteAllOnSubmit(dataContext.Nationalities.Where(t => t.PNR == pnr));
            dataContext.Departures.DeleteAllOnSubmit(dataContext.Departures.Where(t => t.PNR == pnr));
            dataContext.ContactAddresses.DeleteAllOnSubmit(dataContext.ContactAddresses.Where(t => t.PNR == pnr));
            dataContext.PersonAddresses.DeleteAllOnSubmit(dataContext.PersonAddresses.Where(t => t.PNR == pnr));
            dataContext.Protections.DeleteAllOnSubmit(dataContext.Protections.Where(t => t.PNR == pnr));
            dataContext.Disappearances.DeleteAllOnSubmit(dataContext.Disappearances.Where(t => t.PNR == pnr));
            dataContext.Events.DeleteAllOnSubmit(dataContext.Events.Where(t => t.PNR == pnr));
            dataContext.Notes.DeleteAllOnSubmit(dataContext.Notes.Where(t => t.PNR == pnr));
            dataContext.MunicipalConditions.DeleteAllOnSubmit(dataContext.MunicipalConditions.Where(t => t.PNR == pnr));
            dataContext.ParentalAuthorities.DeleteAllOnSubmit(dataContext.ParentalAuthorities.Where(t => t.ChildPNR == pnr));
            dataContext.GuardianAndParentalAuthorityRelations.DeleteAllOnSubmit(dataContext.GuardianAndParentalAuthorityRelations.Where(t => t.PNR == pnr));
            dataContext.GuardianAddresses.DeleteAllOnSubmit(dataContext.GuardianAddresses.Where(t => t.PNR == pnr));
        }

        public static void AppendPerson(IndividualResponseType person, DPRDataContext dataContext)
        {
            dataContext.PersonTotals.InsertOnSubmit(person.ToPersonTotal());

            dataContext.Persons.InsertOnSubmit(person.ToPerson());

            dataContext.Childs.InsertAllOnSubmit(person.Child.Select(c => c.ToDpr()));

            dataContext.PersonNames.InsertOnSubmit(person.CurrentNameInformation.ToDpr());
            dataContext.PersonNames.InsertAllOnSubmit(person.HistoricalName.Select(n => n.ToDpr()));

            dataContext.CivilStatus.InsertOnSubmit(person.CurrentCivilStatus.ToDpr());
            dataContext.CivilStatus.InsertAllOnSubmit(person.HistoricalCivilStatus.Select(c => c.ToDpr()));

            dataContext.Separations.InsertOnSubmit(person.CurrentSeparation.ToDpr());
            dataContext.Separations.InsertAllOnSubmit(person.HistoricalSeparation.Select(c => c.ToDpr()));

            dataContext.Nationalities.InsertOnSubmit(person.CurrentCitizenship.ToDpr());
            dataContext.Nationalities.InsertAllOnSubmit(person.HistoricalCitizenship.Select(c => c.ToDpr()));

            dataContext.Departures.InsertOnSubmit(person.CurrentDepartureData.ToDpr());
            dataContext.Departures.InsertAllOnSubmit(person.HistoricalDeparture.Select(c => c.ToDpr()));

            dataContext.ContactAddresses.InsertOnSubmit(person.ContactAddress.ToDpr());

            var currenrAddress = person.GetFolkeregisterAdresseSource(false) as CurrentAddressWrapper;
            if (currenrAddress != null)
                dataContext.PersonAddresses.InsertOnSubmit(currenrAddress.ToDpr());
            dataContext.PersonAddresses.InsertAllOnSubmit(person.HistoricalAddress.Select(c => c.ToDpr()));

            dataContext.Protections.InsertAllOnSubmit(person.Protection.Select(p => p.ToDpr()));

            if (person.CurrentDisappearanceInformation != null)
                dataContext.Disappearances.InsertOnSubmit(person.CurrentDisappearanceInformation.ToDpr());
            dataContext.Disappearances.InsertAllOnSubmit(person.HistoricalDisappearance.Select(d => d.ToDpr()));

            dataContext.Events.InsertAllOnSubmit(person.Events.Select(ev => ev.ToDpr()));

            dataContext.Notes.InsertAllOnSubmit(person.Notes.Select(n => n.ToDpr()));

            dataContext.MunicipalConditions.InsertAllOnSubmit(person.MunicipalConditions.Select(c => c.ToDpr()));

            dataContext.ParentalAuthorities.InsertAllOnSubmit(person.ParentalAuthority.Select(p => p.ToDpr()));

            if (person.Disempowerment != null)
            {
                // TODO: Shall we also create records from ParentalAuthorityType??            
                dataContext.GuardianAndParentalAuthorityRelations.InsertOnSubmit(person.Disempowerment.ToDpr());
                // TODO: Shall we also create records from ParentalAuthorityType??            
                dataContext.GuardianAddresses.InsertOnSubmit(person.Disempowerment.ToDprAddress());
            }
        }
    }
}
