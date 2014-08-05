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
        public static Separation ToDpr(this CurrentSeparationType currentSeparation)
        {
            Separation s = new Separation();
            s.PNR = Decimal.Parse(currentSeparation.PNR);
            s.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(currentSeparation.Registration.RegistrationDate, 12);
            s.SeparationReferalTimestamp = currentSeparation.ReferenceToAnyMaritalStatus.Value.ToString();
            s.CorrectionMarker = null; //This is the current status
            s.StartAuthorityCode = 0; //TODO: Can be fetched in CPR Services, mynkod_start
            s.StartDate = currentSeparation.SeparationStartDate.Value;
            s.StartDateMarker = currentSeparation.SeparationStartDateUncertainty;
            s.EndAuthorityCode = null; //TODO: Can be fetched in CPR Services, mynkod_slut
            s.EndDate = null; //This is the current separation
            s.EndDateMarker = null; //This is the current separation            
            return s;
        }

        public static Separation ToDpr(this HistoricalSeparationType historicalSeparation)
        {
            Separation s = new Separation();
            s.PNR = Decimal.Parse(historicalSeparation.PNR);
            s.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(historicalSeparation.Registration.RegistrationDate, 12);
            s.SeparationReferalTimestamp = historicalSeparation.ReferenceToAnyMaritalStatus.Value.ToString();
            s.CorrectionMarker = historicalSeparation.CorrectionMarker;
            s.StartAuthorityCode = 0; //TODO: Can be fetched in CPR Services, mynkod_start
            s.StartDate = historicalSeparation.SeparationStartDate.Value;
            s.StartDateMarker = historicalSeparation.SeparationStartDateUncertainty;
            s.EndAuthorityCode = 0; //TODO: Can be fetched in CPR Services, mynkod_slut
            s.EndDate = historicalSeparation.SeparationEndDate.Value;
            s.EndDateMarker = historicalSeparation.SeparationEndDateUncertainty;
            return s;
        }

    }
}
