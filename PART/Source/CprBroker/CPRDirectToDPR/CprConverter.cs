using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;

namespace CPRDirectToDPR
{
    public class CprConverter
    {
        public void AppendPerson(IndividualResponseType person, DPRDataContext dataContext)
        {
            dataContext.PersonTotals.InsertOnSubmit(person.ToPersonTotal());
            
            dataContext.Childs.InsertAllOnSubmit(person.Child.Select(c=>c.ToDpr()));
            
            dataContext.PersonNames.InsertOnSubmit(person.CurrentNameInformation.ToDpr());
            dataContext.PersonNames.InsertAllOnSubmit(person.HistoricalName.Select(n=>n.ToDpr()));
            
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
            if(currenrAddress != null)
                dataContext.PersonAddresses.InsertOnSubmit(currenrAddress.ToDpr());
            dataContext.CivilStatus.InsertAllOnSubmit(person.HistoricalCivilStatus.Select(c => c.ToDpr()));
        }
    }
}
