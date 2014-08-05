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
        public static Protection ToDpr(this ProtectionType protection)
        {
            Protection p = new Protection();
            p.PNR = Decimal.Parse(protection.PNR);
            p.ProtectionType = protection.ProtectionType_;
            p.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(protection.Registration.RegistrationDate, 12);

            if (protection.StartDate.HasValue)
                p.StartDate = protection.StartDate.Value;

            p.EndDate = protection.EndDate;
            p.ReportingMarker = null; //TODO: Can be fetched in CPR Services, indrap
            return p;
        }
    }
}
