using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class CurrentCivilStatusType
    {
        public CivilStatusType ToCivilStatusType(CurrentSeparationType currentSeparation)
        {
            if (currentSeparation != null)
            {
                return currentSeparation.ToCivilStatusType();
            }
            else
            {
                return new CivilStatusType()
                {
                    CivilStatusKode = new CivilStatusLookupMap().Map(this.CivilStatus),
                    TilstandVirkning = TilstandVirkningType.Create(this.ToCivilStatusDate()),
                };
            }
        }

        public PersonRelationType[] ToSpouses(Func<string, Guid> cpr2uuidFunc)
        {
            return this.ToSpouses('G', new char[] { 'F', 'E' }, 'D', false, cpr2uuidFunc);
        }

        public PersonRelationType[] ToRegisteredPartners(Func<string, Guid> cpr2uuidFunc)
        {
            return this.ToSpouses('P', new char[] { 'O', 'L' }, 'D', true, cpr2uuidFunc);
        }

        public PersonRelationType[] ToSpouses(char marriedStatus, char[] terminatedStates, char deadStatus, bool sameGenderForDead, Func<string, Guid> cpr2uuidFunc)
        {
            // TODO: Make sure that it is correct to return an empty array if SpousePNR is empty
            if (this.ToSpousePnr() != null)
            {
                if (this.CivilStatus == marriedStatus)
                {
                    return PersonRelationType.CreateList(cpr2uuidFunc(this.SpousePNR), this.ToCivilStatusDate(), null);
                }
                else if (terminatedStates.Contains(this.CivilStatus))
                {
                    return PersonRelationType.CreateList(cpr2uuidFunc(this.SpousePNR), null, this.ToCivilStatusDate());
                }
                else if (this.CivilStatus == 'D')
                {
                    // TODO: Will there be a spouse PNR in this case?
                    if (
                        (Utilities.Strings.PersonNumberToGender(this.PNR) == Utilities.Strings.PersonNumberToGender(this.ToSpousePnr()))
                        ==
                        sameGenderForDead
                        )
                    {
                        return PersonRelationType.CreateList(cpr2uuidFunc(this.SpousePNR), null, this.ToCivilStatusDate());
                    }
                }
            }
            return new PersonRelationType[0];
        }

        public string ToSpousePnr()
        {
            return Converters.ToPnrStringOrNull(this.SpousePNR);
        }

        public DateTime? ToCivilStatusDate()
        {
            return Converters.ToDateTime(this.CivilStatusStartDate, this.CivilStatusStartDateUncertainty);
        }
    }
}
