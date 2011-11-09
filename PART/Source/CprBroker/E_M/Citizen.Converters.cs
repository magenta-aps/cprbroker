using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.E_M
{
    public partial class Citizen
    {
        public static CountryIdentificationCodeType ToCountryIdentificationCodeType(Citizen citizen)
        {
            if (citizen != null)
            {
                if (citizen.CountryCode > 0 && !Constants.UnknownCountryCodes.Contains(citizen.CountryCode))
                {
                    return CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, citizen.CountryCode.ToString());
                }
                else
                {
                    throw new ArgumentException(string.Format("Invalid country code <{0}>", citizen.CountryCode));
                }
            }
            else
            {
                throw new ArgumentNullException("citizen");
            }
        }

        public static DateTime ToBirthdate(Citizen citizen)
        {
            if (citizen != null)
            {
                var birthdate = Converters.ToDateTime(citizen.Birthdate, citizen.BirthdateUncertainty);
                if (birthdate.HasValue)
                {
                    return birthdate.Value;
                }
                else
                {
                    return CprBroker.Utilities.Strings.PersonNumberToDate(Converters.ToCprNumber(citizen.PNR)).Value;
                }
            }
            else
            {
                throw new ArgumentNullException("citizen");
            }
        }

        public static bool ToChurchMembershipIndicator(Citizen citizen)
        {
            if (citizen != null)
            {
                // TODO: What do the other values mean?
                // F U A M S 
                return citizen.ChurchMarker == 'F';
            }
            else
            {
                throw new ArgumentNullException("citizen");
            }
        }

        public static DateTime? ToDirectoryProtectionStartDate(Citizen citizen)
        {
            return Converters.ToDateTime(citizen.DirectoryProtectionDate, citizen.DirectoryProtectionDateUncertainty);
        }

        public static DateTime? ToDirectoryProtectionEndDate(Citizen citizen)
        {
            return Converters.ToDateTime(citizen.DirectoryProtectionEndDate, citizen.DirectoryProtectionEndDateUncertainty);
        }

        public static bool ToDirectoryProtectionIndicator(Citizen citizen, DateTime effectDate)
        {
            return Utilities.Dates.DateRangeIncludes(ToDirectoryProtectionStartDate(citizen), ToDirectoryProtectionEndDate(citizen), effectDate, false);
        }

        public static DateTime? ToAddressProtectionStartDate(Citizen citizen)
        {
            return Converters.ToDateTime(citizen.AddressProtectionDate, citizen.AddressProtectionDateUncertainty);
        }

        public static DateTime? ToAddressProtectionEndDate(Citizen citizen)
        {
            return Converters.ToDateTime(citizen.AddressProtectionEndDate, citizen.AddressProtectionEndDateUncertainty);
        }

        public static bool ToAddressProtectionIndicator(Citizen citizen, DateTime effectDate)
        {
            return Utilities.Dates.DateRangeIncludes(ToAddressProtectionStartDate(citizen), ToAddressProtectionEndDate(citizen), effectDate, false);
        }
    }
}
