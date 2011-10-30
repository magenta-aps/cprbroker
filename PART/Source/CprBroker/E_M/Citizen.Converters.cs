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
    }
}
