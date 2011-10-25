using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.E_M
{
    public class Converters
    {
        public static bool IsValidCprNumber(decimal? cprNumber)
        {
            if (cprNumber.HasValue)
            {
                cprNumber = Math.Floor(cprNumber.Value);
                string s = cprNumber.Value.ToString("F0");
                if (s.Length == 9 || s.Length == 10)
                {
                    return true;
                }
            }
            return false;
        }

        public static string ToCprNumber(decimal? cprNumber)
        {
            //TODO: Test cpr number conversion
            if (cprNumber.HasValue)
            {
                if (IsValidCprNumber(cprNumber))
                {
                    string ret = cprNumber.Value.ToString("F0");
                    if (ret.Length == 9)
                    {
                        ret = new string('0', 10 - ret.Length) + ret;
                    }
                    return ret;
                }
                else
                {
                    throw new ArgumentException("Invalid CPR number", string.Format("{0}", cprNumber));
                }
            }
            else
            {
                throw new ArgumentNullException("cprNumber");
            }
        }

        public static CivilStatusKodeType ToCivilStatusKodeType(char? status)
        {
            var codes = new CivilStatusCodes();
            if (status.HasValue && codes.ContainsKey(status.Value))
            {
                return codes.Map(status.Value);
            }
            else
            {
                throw new ArgumentException(string.Format("Invalid input <{0}>", status), "status");
            }
        }

        public static LivStatusKodeType ToLivStatusKodeType(short? value, DateTime? birthDate)
        {
            decimal decimalStatus = 0;
            if (value.HasValue)
            {
                decimalStatus = (decimal)value.Value;
            }
            //TODO: Validate this call
            return Schemas.Util.Enums.ToLifeStatus(decimalStatus, birthDate);
        }

        public static DateTime? ToDateTime(DateTime? value, char? uncertainty)
        {
            //TODO: make sure that 'T' means ''certain'
            if (value.HasValue && uncertainty.HasValue && uncertainty.Value == 'T')
            {
                return value.Value;
            }
            return null;
        }

        public static string ShortToString(short? val)
        {
            //TODO: Revise this short to string conversion
            if (val.HasValue)
            {
                return val.Value.ToString("F0");
            }
            return null;
        }

        public static PersonGenderCodeType ToPersonGenderCodeType(char? value)
        {
            //TODO: check the gender conversion char codes in database
            if (value.HasValue)
            {
                switch (value.ToString().ToUpper()[0])
                {
                    case 'M':
                        return PersonGenderCodeType.male;
                    case 'K':
                        return PersonGenderCodeType.female;
                }
            }
            return PersonGenderCodeType.unspecified;
        }

        public static DateTime? GetMaxDate(params DateTime?[] dates)
        {
            var datesWithValues = dates.Where(d => d.HasValue).Select(d => d.Value);
            if (datesWithValues.Count() > 0)
            {
                return datesWithValues.Max();
            }
            else
            {
                return null;
            }
        }

    }
}