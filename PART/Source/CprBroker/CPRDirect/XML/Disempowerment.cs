using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class DisempowermentType
    {
        public PersonFlerRelationType ToPersonFlerRelationType(Func<string, Guid> cpr2uuidFunc)
        {
            // TODO: Is it correct to use RelationPNRStartDate as start date? Shall we use DisempowermentStartDate/DisempowermentStartDateUncertainty instead?
            // TODO: Is it correct to use DisempowermentEndDate as end date?
            return PersonFlerRelationType.Create(
                cpr2uuidFunc(this.ToRelationPNR()),
                this.RelationPNRStartDate,
                this.DisempowermentEndDate
                );
        }

        public static PersonFlerRelationType[] ToPersonRelationType(DisempowermentType disempowerment, Func<string, Guid> cpr2uuidFunc)
        {
            return new DisempowermentType[] { disempowerment }
                .Where(d => d != null)
                .Where(d => !string.IsNullOrEmpty(d.ToRelationPNR()))
                .Select(d => d.ToPersonFlerRelationType(cpr2uuidFunc))
                .ToArray();
        }

        public string ToRelationPNR()
        {
            return Converters.ToPnrStringOrNull(this.RelationPNR);
        }
    }
}
