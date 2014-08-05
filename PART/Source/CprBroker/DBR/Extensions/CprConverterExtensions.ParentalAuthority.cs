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
        public static ParentalAuthority ToDpr(this ParentalAuthorityType auth)
        {
            ParentalAuthority p = new ParentalAuthority();
            p.ChildPNR = decimal.Parse(auth.PNR);
            p.RelationType = auth.RelationshipType;
            p.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(auth.Registration.RegistrationDate, 12);
            p.ParentalAuthorityCode = 0; //TODO: Can be fetched in CPR Services, mynkod_start

            if (auth.CustodyStartDate.HasValue)
                p.StartDate = auth.CustodyStartDate.Value;

            p.StartDateUncertainty = auth.CustodyStartDateUncertainty;

            if (auth.CustodyEndDate.HasValue)
                p.EndDate = auth.CustodyEndDate.Value;

            return p;
        }
    }
}
