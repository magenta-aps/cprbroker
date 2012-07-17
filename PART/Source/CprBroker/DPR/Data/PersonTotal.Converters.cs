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

using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using System;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.DPR
{
    /// <summary>
    /// Represents the DTTOTAL table
    /// </summary>
    public partial class PersonTotal
    {
        public CivilStatusKodeType ToCivilStatusCodeType(Separation latestSeparation)
        {
            if (this.MaritalStatus.HasValue)
            {
                switch (this.MaritalStatus.ToString().ToUpper()[0])
                {
                    case Schemas.Part.MaritalStatus.Unmarried:
                        return CivilStatusKodeType.Ugift;
                    case Schemas.Part.MaritalStatus.Married:
                        if (latestSeparation != null && !latestSeparation.EndDate.HasValue)
                        {
                            return CivilStatusKodeType.Separeret;
                        }
                        else
                        {
                            return CivilStatusKodeType.Gift;
                        }
                    case Schemas.Part.MaritalStatus.Divorced:
                        return CivilStatusKodeType.Skilt;
                    case Schemas.Part.MaritalStatus.Widow:
                        return CivilStatusKodeType.Enke;
                    case Schemas.Part.MaritalStatus.RegisteredPartnership:
                        if (latestSeparation != null && !latestSeparation.EndDate.HasValue)
                        {
                            return CivilStatusKodeType.Separeret;
                        }
                        else
                        {
                            return CivilStatusKodeType.RegistreretPartner;
                        }
                    case Schemas.Part.MaritalStatus.AbolitionOfRegisteredPartnership:
                        return CivilStatusKodeType.OphaevetPartnerskab;
                    case Schemas.Part.MaritalStatus.LongestLivingPartner:
                        return CivilStatusKodeType.Laengstlevende;
                    // TODO : Get value from latest marital status before this record
                    case Schemas.Part.MaritalStatus.Deceased:
                        return CivilStatusKodeType.Ugift;
                }
            }
            throw new NotSupportedException("Unknown marital status");
        }

        public LivStatusKodeType ToLivStatusKodeType()
        {
            return Schemas.Util.Enums.ToLifeStatus(this.Status, Utilities.DateFromDecimal(DateOfBirth));
        }

        public bool ToDirectoryProtectionIndicator()
        {
            return DirectoryProtectionMarker == '1';
        }

        public bool ToCivilRegistrationValidityStatusIndicator()
        {
            return Schemas.Util.Enums.IsActiveCivilRegistrationStatus(this.Status);
        }

        public bool ToAddressProtectionIndicator()
        {
            return AddressProtectionMarker == '1';
        }

        public bool ToChurchMembershipIndicator()
        {
            // F U A M S 
            return ChristianMark.ToString().ToUpper() == "F";
        }


    }
}