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
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.E_M
{
    public partial class Citizen
    {
        public virtual CountryIdentificationCodeType ToCountryIdentificationCodeType()
        {
            return CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, this.CountryCode.ToString());
            // TODO: What do we do if Constants.UnknownCountryCodes contains CountryCode?
        }

        public virtual DateTime ToBirthdate()
        {
            var birthdate = Converters.ToDateTime(this.Birthdate, this.BirthdateUncertainty);
            if (birthdate.HasValue)
            {
                return birthdate.Value;
            }
            else
            {
                return PartInterface.Strings.PersonNumberToDate(Converters.ToCprNumber(this.PNR)).Value;
            }
        }

        public virtual string ToBirthRegistrationAuthority()
        {
            return Authority.ToAuthorityName(this.BirthRegistrationAuthority);
        }

        public virtual bool ToChurchMembershipIndicator()
        {
            return Converters.ToChurchMembershipIndicator(this.ChurchMarker);
        }

        public DateTime? ToDirectoryProtectionStartDate()
        {
            return Converters.ToDateTime(this.DirectoryProtectionDate, this.DirectoryProtectionDateUncertainty);
        }

        public DateTime? ToDirectoryProtectionEndDate()
        {
            return Converters.ToDateTime(this.DirectoryProtectionEndDate, this.DirectoryProtectionEndDateUncertainty);
        }

        public virtual bool ToDirectoryProtectionIndicator(DateTime effectDate)
        {
            return Utilities.Dates.DateRangeIncludes(this.ToDirectoryProtectionStartDate(), this.ToDirectoryProtectionEndDate(), effectDate, false);
        }

        public DateTime? ToAddressProtectionStartDate()
        {
            return Converters.ToDateTime(this.AddressProtectionDate, this.AddressProtectionDateUncertainty);
        }

        public DateTime? ToAddressProtectionEndDate()
        {
            return Converters.ToDateTime(this.AddressProtectionEndDate, this.AddressProtectionEndDateUncertainty);
        }

        public virtual bool ToAddressProtectionIndicator(DateTime effectDate)
        {
            return Utilities.Dates.DateRangeIncludes(this.ToAddressProtectionStartDate(), this.ToAddressProtectionEndDate(), effectDate, false);
        }

        public virtual bool ToCivilRegistrationValidityStatusIndicator()
        {
            return Schemas.Util.Enums.IsActiveCivilRegistrationStatus(CitizenStatusCode);
        }

        public DateTime? ToMaritalStatusDate()
        {
            return Converters.ToDateTime(this.MaritalStatusTimestamp, this.MaritalStatusTimestampUncertainty);
        }

        public DateTime? ToMaritalStatusTerminationDate()
        {
            // Termintion date is always null
            return null;
        }

        public string ToSpousePNR()
        {
            return Converters.ToCprNumber(this.SpousePNR);
        }
    }
}
