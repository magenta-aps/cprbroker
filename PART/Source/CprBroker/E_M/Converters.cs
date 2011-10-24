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
        public static string ToCprNumber(decimal? cprNumber)
        {
            //TODO: Test cpr number conversion
            if (cprNumber.HasValue)
            {
                string ret = cprNumber.Value.ToString("F0");
                ret = new string('0', 10 - ret.Length) + ret;
                return ret;
            }
            else
            {
                return null;
            }
        }

        public static CivilStatusKodeType ToCivilStatusKodeType(char? status)
        {
            if (status.HasValue)
            {
                return new CivilStatusCodes().Map(status.Value);
            }
            else
            {
                throw new Exception(string.Format("Invalid input <{0}>"));
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
            //TODO: Fill date time conversion
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