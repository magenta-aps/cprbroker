using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Util;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
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
                    CivilStatusKode = new CivilStatusLookupMap().Map(this._CivilStatus.CivilStatus),
                    TilstandVirkning = TilstandVirkningType.Create(this.ToCivilStatusDate()),
                };
            }
        }

        public static PersonRelationType[] ToSpouses(ICivilStatus current, List<HistoricalCivilStatusType> history, Func<string, Guid> cpr2uuidFunc)
        {
            return ToSpouses(current, history, 'G', new char[] { 'F', 'E' }, 'D', false, cpr2uuidFunc);
        }

        public static PersonRelationType[] ToRegisteredPartners(ICivilStatus current, List<HistoricalCivilStatusType> history, Func<string, Guid> cpr2uuidFunc)
        {
            return ToSpouses(current, history, 'P', new char[] { 'O', 'L' }, 'D', true, cpr2uuidFunc);
        }

        public static PersonRelationType[] ToSpouses(ICivilStatus current, List<HistoricalCivilStatusType> history, char marriedStatus, char[] terminatedStates, char deadStatus, bool sameGenderForDead, Func<string, Guid> cpr2uuidFunc)
        {
            var all = new List<ICivilStatus>();

            // Add current status
            all.Add(current);

            // Add historical states
            history = history == null ? new List<HistoricalCivilStatusType>() : history;
            all.AddRange(
                history
                .Where(h => Converters.IsValidCorrectionMarker(h.CorrectionMarker))
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
            return Converters.ToPnrStringOrNull(this._CivilStatus.SpousePNR);
        }

        public DateTime? ToCivilStatusDate()
        {
            return Converters.ToDateTime(this._CivilStatus.CivilStatusStartDate, this._CivilStatus.CivilStatusStartDateUncertainty);
        }

    }
}
