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
            if (this.CountryCode > 0 && !Constants.UnknownCountryCodes.Contains(this.CountryCode))
            {
                return CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, this.CountryCode.ToString());
            }
            else
            {
                throw new ArgumentException(string.Format("Invalid country code <{0}>", this.CountryCode));
            }
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
                return CprBroker.Utilities.Strings.PersonNumberToDate(Converters.ToCprNumber(this.PNR)).Value;
            }
        }

        public virtual string ToBirthRegistrationAuthority()
        {
            return Authority.ToAuthorityName(this.BirthRegistrationAuthority);
        }

        public virtual bool ToChurchMembershipIndicator()
        {
            // TODO: What do the other values mean?
            // F U A M S 
            return this.ChurchMarker == 'F';
        }

        public static DateTime? ToDirectoryProtectionStartDate(Citizen citizen)
        {
            return Converters.ToDateTime(citizen.DirectoryProtectionDate, citizen.DirectoryProtectionDateUncertainty);
        }

        public static DateTime? ToDirectoryProtectionEndDate(Citizen citizen)
        {
            return Converters.ToDateTime(citizen.DirectoryProtectionEndDate, citizen.DirectoryProtectionEndDateUncertainty);
        }

        public virtual bool ToDirectoryProtectionIndicator(DateTime effectDate)
        {
            return Utilities.Dates.DateRangeIncludes(ToDirectoryProtectionStartDate(this), ToDirectoryProtectionEndDate(this), effectDate, false);
        }

        public static DateTime? ToAddressProtectionStartDate(Citizen citizen)
        {
            return Converters.ToDateTime(citizen.AddressProtectionDate, citizen.AddressProtectionDateUncertainty);
        }

        public static DateTime? ToAddressProtectionEndDate(Citizen citizen)
        {
            return Converters.ToDateTime(citizen.AddressProtectionEndDate, citizen.AddressProtectionEndDateUncertainty);
        }

        public virtual bool ToAddressProtectionIndicator(DateTime effectDate)
        {
            return Utilities.Dates.DateRangeIncludes(ToAddressProtectionStartDate(this), ToAddressProtectionEndDate(this), effectDate, false);
        }

        public virtual bool ToCivilRegistrationValidityStatusIndicator()
        {
            return Schemas.Util.Enums.IsActiveCivilRegistrationStatus(CitizenStatusCode);
        }
    }
}
