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
        public static CivilStatus ToDpr(this CurrentCivilStatusType currentCivilStatus)
        {
            CivilStatus cs = new CivilStatus();
            cs.PNR = Decimal.Parse(currentCivilStatus.PNR);
            cs.UpdateDateOfCpr = CprBroker.Utilities.Dates.DateToDecimal(currentCivilStatus.Registration.RegistrationDate, 12);
            cs.MaritalStatus = currentCivilStatus.CivilStatusCode;
            cs.MaritalStatusAuthorityCode = null; //TODO: Can be fetched in CPR Services, mynkod

            if (!string.IsNullOrEmpty(currentCivilStatus.SpousePNR))
                cs.SpousePNR = Decimal.Parse(currentCivilStatus.SpousePNR);

            if (currentCivilStatus.SpouseBirthDate.HasValue)
                cs.SpouseBirthdate = CprBroker.Utilities.Dates.DateToDecimal(currentCivilStatus.SpouseBirthDate.Value, 8);

            cs.SpouseDocumentation = null; //TODO: Can be fetched in CPR Services, aegtedok

            if (currentCivilStatus.CivilStatusStartDate.HasValue)
                cs.MaritalStatusDate = CprBroker.Utilities.Dates.DateToDecimal(currentCivilStatus.CivilStatusStartDate.Value, 12);

            cs.MaritalEndDate = null; //This is the current status
            cs.CorrectionMarker = null; //This is the current status
            cs.AuthorityTextUpdateDate = null; //TODO: Can be fetched in CPR Services,  myntxttimestamp
            cs.MaritalStatusAuthorityText = null; //TODO: Can be fetched in CPR Services,  myntxt
            if (!string.IsNullOrEmpty(currentCivilStatus.SpouseName))
                cs.SpouseName = currentCivilStatus.SpouseName;
            else
                cs.SpouseName = null;

            if (currentCivilStatus.ReferenceToAnySeparation.HasValue)
            {
                cs.SeparationReferralTimestamp = currentCivilStatus.ReferenceToAnySeparation.Value.ToString();
            }
            else
            {
                cs.SeparationReferralTimestamp = null;
            }
            return cs;
        }

        public static CivilStatus ToDpr(this HistoricalCivilStatusType historicalCivilStatus)
        {
            CivilStatus cs = new CivilStatus();
            cs.PNR = Decimal.Parse(historicalCivilStatus.PNR);
            cs.UpdateDateOfCpr = CprBroker.Utilities.Dates.DateToDecimal(historicalCivilStatus.Registration.RegistrationDate, 12);
            cs.MaritalStatus = historicalCivilStatus.CivilStatusCode;
            cs.MaritalStatusAuthorityCode = null; //TODO: Can be fetched in CPR Services, mynkod

            if (!string.IsNullOrEmpty(historicalCivilStatus.SpousePNR))
                cs.SpousePNR = Decimal.Parse(historicalCivilStatus.SpousePNR);

            if (historicalCivilStatus.SpouseBirthdate.HasValue)
                cs.SpouseBirthdate = CprBroker.Utilities.Dates.DateToDecimal(historicalCivilStatus.SpouseBirthdate.Value, 8);

            cs.SpouseDocumentation = null; //This is the current status

            if (historicalCivilStatus.CivilStatusStartDate.HasValue)
                cs.MaritalStatusDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCivilStatus.CivilStatusStartDate.Value, 12);

            if (historicalCivilStatus.CivilStatusEndDate.HasValue)
                cs.MaritalEndDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCivilStatus.CivilStatusEndDate.Value, 12);

            cs.CorrectionMarker = historicalCivilStatus.CorrectionMarker;
            cs.AuthorityTextUpdateDate = null; //TODO: Can be fetched in CPR Services, myntxttimestamp
            cs.MaritalStatusAuthorityText = null; //TODO: Can be fetched in CPR Services, myntxt
            cs.SpouseName = historicalCivilStatus.SpouseName;

            if (historicalCivilStatus.ReferenceToAnySeparation.HasValue)
                cs.SeparationReferralTimestamp = historicalCivilStatus.ReferenceToAnySeparation.Value.ToString();

            return cs;
        }

    }
}
