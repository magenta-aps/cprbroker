using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class ParentalAuthorityType
    {
        public PersonRelationType ToPersonRelationType(Func<string, Guid> cpr2uuidFunc)
        {
            // TODO: Is RelationPNRStartDate the correct start date? shall we use CustodyStartDate/CustodyStartDateUncertainty instead?
            // TODO: Is CustodyEndDate the correct end date?
            return PersonRelationType.Create(
                cpr2uuidFunc(ToCustodyOwnerPnr()),
                this.RelationPNRStartDate,
                this.CustodyEndDate);
        }

        public static PersonRelationType[] ToPersonRelationType(IList<ParentalAuthorityType> parents, Func<string, Guid> cpr2uuidFunc)
        {
            return parents
                .Where(p => !string.IsNullOrEmpty(p.ToCustodyOwnerPnr()))
                .Select(p => p.ToPersonRelationType(cpr2uuidFunc))
                .ToArray();
        }

        public string ToCustodyOwnerPnr()
        {
            return Converters.ToPnrStringOrNull(this.RelationPNR);
        }

    }
}
