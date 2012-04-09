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

        public static PersonRelationType[] ToToPersonRelationTypeArray(ICollection<CivilStatus> dbCivilStates, Func<decimal, Guid> cpr2uuidFunc, params char[] maritalStates)
        {
            return dbCivilStates
                .Where(civ =>
                    maritalStates.Contains(civ.MaritalStatus.Value, new CaseInvariantCharComparer())
                    && civ.SpousePNR.HasValue
                    && civ.SpousePNR.Value > 0)
                .OrderBy(civ => civ.MaritalStatusDate)
                .Select(civ => civ.ToPersonRelationType(cpr2uuidFunc))
                .ToArray();
        }

        class CaseInvariantCharComparer : IEqualityComparer<char>
        {
            public bool Equals(char x, char y)
            {
                return x.ToString().ToLower().Equals(y.ToString().ToLower());
            }

            public int GetHashCode(char obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
