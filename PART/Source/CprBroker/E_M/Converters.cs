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
        public static bool IsValidCprNumber(decimal cprNumber)
        {
            decimal cprNumber2 = Math.Abs(Math.Floor(cprNumber));
            if (cprNumber2.Equals(cprNumber))
            {
                string s = DecimalToString(cprNumber);
                if (s.Length == 9 || s.Length == 10)
                {
                    return true;
                }
            }
            return false;
        }

        public static string ToCprNumber(decimal cprNumber)
        {
            //TODO: Test cpr number conversion
            if (IsValidCprNumber(cprNumber))
            {
                string ret = DecimalToString(cprNumber);
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

        public static CivilStatusKodeType ToCivilStatusKodeType(char status)
        {
            var codes = new CivilStatusCodes();
            if (codes.ContainsKey(status))
            {
                return codes.Map(status);
            }
            else
            {
                throw new ArgumentException(string.Format("Invalid input <{0}>", status), "status");
            }
        }

        public static LivStatusKodeType ToLivStatusKodeType(short value, bool birthdateHasValue)
        {
            decimal decimalStatus = (decimal)value;
            //TODO: Validate this call
            return Schemas.Util.Enums.ToLifeStatus(decimalStatus, birthdateHasValue);
        }

        public static DateTime? ToDateTime(DateTime value, char uncertainty)
        {
            if (uncertainty == ' ')
            {
                return value;
            }
            return null;
        }

        public static string ShortToString(short val)
        {
            //TODO: Revise this short to string conversion
            return val.ToString("F0");
        }

        public static string DecimalToString(decimal val)
        {
            return val.ToString("F0");
        }

        public static PersonGenderCodeType ToPersonGenderCodeType(char value)
        {
            //TODO: check the gender conversion char codes in database            
            switch (value.ToString().ToUpper()[0])
            {
                case 'M':
                    return PersonGenderCodeType.male;
                case 'K':
                    return PersonGenderCodeType.female;
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