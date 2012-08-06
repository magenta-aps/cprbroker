using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class DisempowermentType : IReversibleRelationship
    {
        public PersonFlerRelationType ToPersonFlerRelationType(Func<string, Guid> cpr2uuidFunc)
        {
            return PersonFlerRelationType.Create(
                cpr2uuidFunc(this.ToRelationPNR()),
                this.RelationPNRStartDate,
                this.DisempowermentEndDate
                );
        }

        public static PersonFlerRelationType[] ToPersonRelationType(DisempowermentType disempowerment, Func<string, Guid> cpr2uuidFunc)
        {
            // TODO: (Reverse relation) Shall we also implement unknown persons from their addresses?
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
