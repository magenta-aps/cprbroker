/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

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
                    TilstandVirkning = TilstandVirkningType.Create(this._CivilStatus.ToStartTS()),
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
                    forPreviousInterval ? null : _CivilStatus.ToStartTS(),
                    forPreviousInterval ? _CivilStatus.ToStartTS() : _CivilStatus.ToEndTS()
                )
            };
        }

        public static PersonRelationType[] ToSpouses(ICivilStatus current, List<ICivilStatus> history, Func<string, Guid> cpr2uuidFunc)
        {
            return ToPersonRelationTypeArray(current, history, cpr2uuidFunc, MaritalStatus.Married, MaritalStatus.Divorced, MaritalStatus.Widow, MaritalStatus.Deceased, false);
        }

        public static PersonRelationType[] ToRegisteredPartners(ICivilStatus current, List<ICivilStatus> history, Func<string, Guid> cpr2uuidFunc)
        {
            return ToPersonRelationTypeArray(current, history, cpr2uuidFunc, MaritalStatus.RegisteredPartnership, MaritalStatus.AbolitionOfRegisteredPartnership, MaritalStatus.LongestLivingPartner, MaritalStatus.Deceased, true);
        }

        
        public static PersonRelationType[] ToPersonRelationTypeArray(ICivilStatus currentStatus, IList<ICivilStatus> historyCivilStates, Func<string, Guid> cpr2uuidFunc, char marriedStatus, char divorcedStatus, char widowStatus, char deadStatus, bool deadAsSameSex)
        {
            // TODO: Shall we take separation into account?
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
                    && (maritalStates.Contains(h.CivilStatusCode, new CaseInvariantCharComparer()) || h.CivilStatusCode == deadStatus)
                    && h.IsValid()
                )
                .OrderBy(civ => civ.ToStartTS())
                .ToList();

            var ret = new List<PersonRelationType>();
            for (int i = 0; i < allCivilStates.Count; i++)
            {
                var dbCivilStatus = allCivilStates[i];
                var dbCivilStatusWrapper = new CivilStatusWrapper(dbCivilStatus);

                var previousDbCivilStatus =
                    (i > 0) && (allCivilStates[i - 1].ToEndTS() == dbCivilStatus.ToStartTS()) ?
                    allCivilStates[i - 1] : null;

                if (dbCivilStatus.CivilStatusCode == marriedStatus)
                {
                    ret.Add(dbCivilStatusWrapper.ToPersonRelationType(cpr2uuidFunc));
                }
                else if (previousDbCivilStatus == null) // Statistics show that if previous row exists, it will be always 'married'                    
                {
                    if (dbCivilStatus.CivilStatusCode == divorcedStatus || dbCivilStatus.CivilStatusCode == widowStatus)
                    {
                        // Only add a relation if the previous row (married) does not exist
                        // Reverse times because we need the 'marriage' interval, not the 'divorce/widow'
                        ret.Add(dbCivilStatusWrapper.ToPersonRelationType(cpr2uuidFunc, true));
                    }
                    else if (dbCivilStatus.CivilStatusCode == deadStatus)
                    {
                        // TODO: Rely on person data to get gender instead of CPR number
                        bool isSameSex = Util.Enums.PersonNumberToGender(dbCivilStatusWrapper._CivilStatus.PNR) == Util.Enums.PersonNumberToGender(dbCivilStatusWrapper.ToSpousePnr());
                        if (isSameSex == deadAsSameSex)
                        {
                            ret.Add(dbCivilStatusWrapper.ToPersonRelationType(cpr2uuidFunc, true));
                        }
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
            return _CivilStatus.ToStartTS();
        }

        /// <summary>
        /// Main purpose of this method is r
        /// - Remove ivalid civil states 
        /// - Add a starting 'unmarried' interval if needed (and possible)
        /// </summary>
        /// <param name="currentStatus"></param>
        /// <param name="historyCivilStates"></param>
        /// <param name="singleStatus"></param>
        /// <returns></returns>
        public static List<ICivilStatus> PopulateIntervals(ICivilStatus currentStatus, List<ICivilStatus> historyCivilStates, char singleStatus)
        {
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
                    && h.IsValid()
                )
                .OrderBy(civ => civ.ToStartTS())
                .ToList();

            // Add a dummy 'Single' interval
            if (allCivilStates.Count > 1)
            {
                var firstStatus = allCivilStates.First();
                if (firstStatus.ToStartTS().HasValue && firstStatus.CivilStatusCode != singleStatus)
                {
                    var dummy = new DummyCivilStatus()
                    {
                        PNR = firstStatus.PNR,
                        SpousePnr = null,
                        CivilStatusCode = singleStatus,
                        StartTS = null,
                        EndTS = firstStatus.ToStartTS()
                    };
                    allCivilStates.Add(dummy);
                }
            }
            return allCivilStates;
        }
    }
}
