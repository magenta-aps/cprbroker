using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Util;

namespace CprBroker.Schemas.Part
{
    /// <summary>
    /// Contains common functions for civil status
    /// </summary>
    public class CivilStatusWrapper
    {
        private ICivilStatus _CivilStatus;

        public CivilStatusWrapper(ICivilStatus civil)
        {
            _CivilStatus = civil;
        }

        public CivilStatusType ToCivilStatusType(ISeparation currentSeparation)
        {
            if (currentSeparation != null)
            {
                return currentSeparation.ToCivilStatusType();
            }
            else
            {
                return new CivilStatusType()
                {
                    CivilStatusKode = new CivilStatusLookupMap().Map(this._CivilStatus.CivilStatus),
                    TilstandVirkning = TilstandVirkningType.Create(this._CivilStatus.ToCivilStatusDate()),
                };
            }
        }

        public static PersonRelationType[] ToSpouses(ICivilStatus current, List<ICivilStatus> history, Func<string, Guid> cpr2uuidFunc)
        {
            return ToSpouses(current, history, 'G', new char[] { 'F', 'E' }, 'D', false, cpr2uuidFunc);
        }

        public static PersonRelationType[] ToRegisteredPartners(ICivilStatus current, List<ICivilStatus> history, Func<string, Guid> cpr2uuidFunc)
        {
            return ToSpouses(current, history, 'P', new char[] { 'O', 'L' }, 'D', true, cpr2uuidFunc);
        }

        public static PersonRelationType[] ToSpouses(ICivilStatus current, List<ICivilStatus> history, char marriedStatus, char[] terminatedStates, char deadStatus, bool sameGenderForDead, Func<string, Guid> cpr2uuidFunc)
        {
            var all = new List<ICivilStatus>();

            // Add current status
            all.Add(current);

            // Add historical states
            history = history == null ? new List<ICivilStatus>() : history;
            all.AddRange(
                history
                .Where(h => h.IsValid())
                .Select(h => h as ICivilStatus));

            all = all.OrderBy(civil => civil.CivilStatusStartDate).ToList();

            // Convert to PART format
            var ret = all
                .Select(
                civil =>
                {
                    var civilWrapper = new CivilStatusWrapper(civil);

                    // TODO: Make sure that it is correct to return null if SpousePNR is empty
                    if (civilWrapper.ToSpousePnr() != null)
                    {
                        if (civilWrapper._CivilStatus.CivilStatus == marriedStatus)
                        {
                            return PersonRelationType.Create(cpr2uuidFunc(civilWrapper._CivilStatus.SpousePNR), civilWrapper.ToCivilStatusDate(), null);
                        }
                        else if (terminatedStates.Contains(civilWrapper._CivilStatus.CivilStatus))
                        {
                            return PersonRelationType.Create(cpr2uuidFunc(civilWrapper._CivilStatus.SpousePNR), null, civilWrapper.ToCivilStatusDate());
                        }
                        else if (civilWrapper._CivilStatus.CivilStatus == deadStatus)
                        {
                            // TODO: Will there be a spouse PNR in this case?
                            if (
                                (Enums.PersonNumberToGender(civilWrapper._CivilStatus.PNR) == Enums.PersonNumberToGender(civilWrapper.ToSpousePnr()))
                                ==
                                sameGenderForDead
                                )
                            {
                                return PersonRelationType.Create(cpr2uuidFunc(civilWrapper._CivilStatus.SpousePNR), null, civilWrapper.ToCivilStatusDate());
                            }
                        }
                    }
                    return null;
                }
            )
            .Where(r => r != null)
            .ToArray();

            return ret;
        }

        public string ToSpousePnr()
        {
            return _CivilStatus.ToSpousePnr();
        }

        public DateTime? ToCivilStatusDate()
        {
            return _CivilStatus.ToCivilStatusDate();
        }
    }
}
