using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;

namespace CPRDirectToDPR
{
    public static class CprConverterExtensions
    {
        public static PersonTotal ToPersonTotal(this IndividualResponseType resp)
        {
            throw new NotImplementedException();
        }

        public static Child ToDpr(this ChildType child)
        {
            throw new NotImplementedException();
        }

        public static PersonName ToDpr(this CurrentNameInformationType currentName)
        {
            throw new NotImplementedException();
        }

        public static PersonName ToDpr(this HistoricalNameType historicalName)
        {
            throw new NotImplementedException();
        }

        public static CivilStatus ToDpr(this CurrentCivilStatusType currentCivilStatus)
        {
            throw new NotImplementedException();
        }

        public static CivilStatus ToDpr(this HistoricalCivilStatusType historicalCivilStatus)
        {
            throw new NotImplementedException();
        }

        public static Separation ToDpr(this CurrentSeparationType currentSeparation)
        {
            throw new NotImplementedException();
        }

        public static Separation ToDpr(this HistoricalSeparationType historicalSeparation)
        {
            throw new NotImplementedException();
        }

        public static Nationality ToDpr(this CurrentCitizenshipType currentCitizenship)
        {
            throw new NotImplementedException();
        }

        public static Nationality ToDpr(this HistoricalCitizenshipType historicalCitizenship)
        {
            throw new NotImplementedException();
        }

        public static Departure ToDpr(this CurrentDepartureDataType currentDeparture)
        {
            throw new NotImplementedException();
        }

        public static Departure ToDpr(this HistoricalDepartureType historicalDeparture)
        {
            throw new NotImplementedException();
        }

        public static ContactAddress ToDpr(this ContactAddressType contactAddress)
        {
            throw new NotImplementedException();
        }

        public static PersonAddress ToDpr(this CurrentAddressWrapper currentAddress)
        {
            throw new NotImplementedException();
        }

        public static PersonAddress ToDpr(this HistoricalAddressType historicalAddress)
        {
            throw new NotImplementedException();
        }




    }
}
