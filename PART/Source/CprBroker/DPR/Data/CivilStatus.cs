using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.DPR
{
    public partial class CivilStatus
    {
        public PersonRelationType ToPersonRelationType(Func<decimal, Guid> cpr2uuidFunc)
        {
            return new PersonRelationType()
            {
                ReferenceID = UnikIdType.Create(cpr2uuidFunc(SpousePNR.Value)),
                CommentText = "",
                Virkning = VirkningType.Create(Utilities.DateFromDecimal(MaritalStatusDate), Utilities.DateFromDecimal((MaritalEndDate)))
            };
        }

        public static PersonRelationType[] ToToPersonRelationTypeArray(ICollection<CivilStatus> dbCivilStates, decimal? effectTimeDecimal, Func<decimal, Guid> cpr2uuidFunc, params char[] maritalStates)
        {
            return dbCivilStates
                .Where(civ =>
                    maritalStates.Contains(civ.MaritalStatus.Value)
                    && civ.MaritalStatusDate <= effectTimeDecimal.Value
                    && civ.SpousePNR.HasValue
                    && civ.SpousePNR.Value > 0)
                .OrderBy(civ => civ.MaritalStatusDate)
                .Select(civ => civ.ToPersonRelationType(cpr2uuidFunc))
                .ToArray();
        }
    }
}
