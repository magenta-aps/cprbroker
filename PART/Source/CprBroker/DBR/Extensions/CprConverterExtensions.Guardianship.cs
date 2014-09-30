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
        public static GuardianAndParentalAuthorityRelation ToDpr(this DisempowermentType disempowerment)
        {
            if (disempowerment.GuardianshipType == DisempowermentType.GuardianshipTypes.ParentOrGuardianPnrFound)
            {
                GuardianAndParentalAuthorityRelation gapa = new GuardianAndParentalAuthorityRelation();
                gapa.PNR = decimal.Parse(disempowerment.PNR);

                if (!string.IsNullOrEmpty(disempowerment.RelationPNR))
                    gapa.RelationPnr = decimal.Parse(disempowerment.RelationPNR);

                gapa.RelationType = disempowerment.GuardianRelationType;
                gapa.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(disempowerment.Registration.RegistrationDate, 12);

                if (disempowerment.DisempowermentStartDate.HasValue)
                    gapa.StartDate = disempowerment.DisempowermentStartDate.Value;

                if (disempowerment.DisempowermentEndDate.HasValue)
                    gapa.EndDate = disempowerment.DisempowermentEndDate.Value;

                gapa.AuthorityCode = 0; //TODO: Can be fetched in CPR Services, mynkod
                return gapa;
            }
            return null;
        }

        public static GuardianAddress ToDprAddress(this DisempowermentType disempowerment)
        {
            if (disempowerment.GuardianshipType == DisempowermentType.GuardianshipTypes.ParentOrGuardianAddressExists)
            {
                GuardianAddress ga = new GuardianAddress();
                ga.PNR = decimal.Parse(disempowerment.PNR);
                ga.Address = disempowerment.GuardianName;
                ga.RelationType = disempowerment.GuardianRelationType;
                ga.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(disempowerment.Registration.RegistrationDate, 12);
                ga.AddressLine1 = disempowerment.RelationText1;
                ga.AddressLine2 = disempowerment.RelationText2;
                ga.AddressLine3 = disempowerment.RelationText3;
                ga.AddressLine4 = disempowerment.RelationText4;
                ga.AddressLine5 = disempowerment.RelationText5;

                // TODO: Sample PNR 709614126 has start date equal to 1/1/1 !!!
                if (disempowerment.GuardianAddressStartDate.HasValue)
                    ga.StartDate = disempowerment.GuardianAddressStartDate.Value;

                ga.EndDate = disempowerment.DisempowermentEndDate;
                ga.AuthorityCode = 0; //TODO: Can be fetched in CPR Services, mynkod
                return ga;
            }
            return null;
        }
    }
}
