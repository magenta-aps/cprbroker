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
    }
}
