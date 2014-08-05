using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas.Part;

namespace CprBroker.DBR.Extensions
{
    public static partial class CprConverterExtensions
    {
        public static Nationality ToDpr(this CurrentCitizenshipType currentCitizenship)
        {
            Nationality n = new Nationality();
            n.PNR = Decimal.Parse(currentCitizenship.PNR);
            n.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(currentCitizenship.Registration.RegistrationDate, 12);
            n.CountryCode = currentCitizenship.CountryCode;

            if (currentCitizenship.CitizenshipStartDate.HasValue)
                n.NationalityStartDate = CprBroker.Utilities.Dates.DateToDecimal(currentCitizenship.CitizenshipStartDate.Value, 12);

            n.NationalityEndDate = null; // This is the current nationality
            n.CorrectionMarker = null; //This is the current status
            return n;
        }

        public static Nationality ToDpr(this HistoricalCitizenshipType historicalCitizenship)
        {
            Nationality n = new Nationality();
            n.PNR = Decimal.Parse(historicalCitizenship.PNR);
            n.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCitizenship.Registration.RegistrationDate, 12);
            n.CountryCode = historicalCitizenship.CountryCode;

            if (historicalCitizenship.CitizenshipStartDate.HasValue)
                n.NationalityStartDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCitizenship.CitizenshipStartDate.Value, 12);

            if (historicalCitizenship.CitizenshipEndDate.HasValue)
                n.NationalityEndDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCitizenship.CitizenshipEndDate.Value, 12);

            n.CorrectionMarker = historicalCitizenship.CorrectionMarker;
            return n;
        }

    }
}
