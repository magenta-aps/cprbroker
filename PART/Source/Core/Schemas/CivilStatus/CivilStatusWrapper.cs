using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Util;
using CprBroker.Utilities;

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
            var statusCode = new CivilStatusLookupMap().Map(this._CivilStatus.CivilStatusCode);

            // TODO: Copy from DPR
            if (currentSeparation != null && (statusCode == CivilStatusKodeType.Gift || statusCode == CivilStatusKodeType.RegistreretPartner))
            {
                return currentSeparation.ToCivilStatusType();
            }
            else
            {
                return new CivilStatusType()
                {
                    CivilStatusKode = statusCode,
                    TilstandVirkning = TilstandVirkningType.Create(this._CivilStatus.ToCivilStatusStartDate()),
                };
            }
        }

        public PersonRelationType ToPersonRelationType(Func<string, Guid> cpr2uuidFunc, bool forPreviousInterval = false)
        {
            return new PersonRelationType()
            {
                ReferenceID = UnikIdType.Create(cpr2uuidFunc(ToSpousePnr())),
                CommentText = "",
                Virkning = VirkningType.Create(
                    forPreviousInterval ? null : _CivilStatus.ToCivilStatusStartDate(),
                    forPreviousInterval ? _CivilStatus.ToCivilStatusStartDate() : _CivilStatus.ToCivilStatusEndDate()
                )
            };
        }

        public static PersonRelationType[] ToSpouses(ICivilStatus current, List<ICivilStatus> history, Func<string, Guid> cpr2uuidFunc)
        {
            return ToPersonRelationTypeArray(current, history, cpr2uuidFunc, MaritalStatus.Married, MaritalStatus.Divorced, MaritalStatus.Widow);
        }

        public static PersonRelationType[] ToRegisteredPartners(ICivilStatus current, List<ICivilStatus> history, Func<string, Guid> cpr2uuidFunc)
        {
            return ToPersonRelationTypeArray(current, history, cpr2uuidFunc, MaritalStatus.RegisteredPartnership, MaritalStatus.AbolitionOfRegisteredPartnership, MaritalStatus.LongestLivingPartner);
        }

        public static PersonRelationType[] ToPersonRelationTypeArray(ICivilStatus currentStatus, IList<ICivilStatus> historyCivilStates, Func<string, Guid> cpr2uuidFunc, char marriedStatus, char divorcedStatus, char widowStatus)
        {
            char[] maritalStates = new char[] { marriedStatus, divorcedStatus, widowStatus };

            var allCivilStates = new List<ICivilStatus>();

            // Add current status
            allCivilStates.Add(currentStatus);

            // Add historical states
            historyCivilStates = historyCivilStates == null ? new List<ICivilStatus>() : historyCivilStates;
            allCivilStates.AddRange(historyCivilStates);

            // Filter the correct records
            allCivilStates = allCivilStates
                .Where(h =>
                    h != null
                    && maritalStates.Contains(h.CivilStatusCode, new CaseInvariantCharComparer())
                    && h.IsValid()
                )
                .OrderBy(civ => civ.ToCivilStatusStartDate())
                .ToList();

            var ret = new List<PersonRelationType>();
            for (int i = 0; i < allCivilStates.Count; i++)
            {
                var dbCivilStatus = allCivilStates[i];
                var dbCivilStatusWrapper = new CivilStatusWrapper(dbCivilStatus);

                var previousDbCivilStatus =
                    (i > 0) && (allCivilStates[i - 1].ToCivilStatusEndDate() == dbCivilStatus.ToCivilStatusStartDate()) ?
                    allCivilStates[i - 1] : null;

                if (dbCivilStatus.CivilStatusCode == marriedStatus)
                {
                    ret.Add(dbCivilStatusWrapper.ToPersonRelationType(cpr2uuidFunc));
                }
                else if (
                    dbCivilStatus.CivilStatusCode == divorcedStatus || dbCivilStatus.CivilStatusCode == widowStatus)
                {
                    // Statistics show that if previous row exists, it will be always 'married'
                    if (previousDbCivilStatus == null)
                    {
                        // Only add a relation if the previous row (married) does not exist
                        // Reverse times because we need the 'marriage' interval, not the 'divorce/widow'
                        ret.Add(dbCivilStatusWrapper.ToPersonRelationType(cpr2uuidFunc, true));
                    }
                }
            }
            return ret.ToArray();
        }

        public string ToSpousePnr()
        {
            return _CivilStatus.ToSpousePnr();
        }

        public DateTime? ToCivilStatusDate()
        {
            return _CivilStatus.ToCivilStatusStartDate();
        }
    }
}
