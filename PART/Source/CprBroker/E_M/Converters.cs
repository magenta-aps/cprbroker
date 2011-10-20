using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.E_M
{
    internal class Converters
    {
        internal static string ToCprNumber(decimal? cprNumber)
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

        internal static CivilStatusKodeType ToCivilStatusKodeType(char? status)
        {
            //TODO: Handle 'D' (dead) 
            //TODO: See what fits into CivilStatusKodeType.Separeret

            if (status.HasValue)
            {
                switch (status.Value)
                {
                    case 'D':
                        break;
                    case 'E':
                        return CivilStatusKodeType.Enke;
                    case 'F': // divorced
                        return CivilStatusKodeType.Skilt;
                    case 'G':
                        return CivilStatusKodeType.Gift;
                    case 'L':
                        return CivilStatusKodeType.Laengstlevende;
                    case 'O':
                        return CivilStatusKodeType.OphaevetPartnerskab;
                    case 'P':
                        return CivilStatusKodeType.RegistreretPartner;
                    case 'U':
                        return CivilStatusKodeType.Ugift;
                }
            }
            else
            {
                // TODO: Handle null input
            }
            return CivilStatusKodeType.Ugift;
        }

        internal static LivStatusKodeType ToLivStatusKodeType(short? value, DateTime? birthDate)
        {
            decimal? decimalStatus = null;
            if (value.HasValue)
            {
                decimalStatus = (decimal)value.Value;
            }
            //TODO: Validate this call
            return Schemas.Util.Enums.ToLifeStatus((decimal)value.Value, birthDate);
        }

        internal static DateTime? ToDateTime(DateTime? value, char? uncertainty)
        {
            //TODO: Fill date time conversion
            return null;
        }

        internal static string ShortToString(short? val)
        {
            //TODO: Revise this short to string conversion
            if (val.HasValue)
            {
                return val.Value.ToString("F0");
            }
            return null;
        }

        internal static PersonGenderCodeType ToPersonGenderCodeType(char? value)
        {
            //TODO: Fill gender conversion
            return PersonGenderCodeType.unspecified;
        }

        internal static DateTime? GetMaxDate(params DateTime?[] dates)
        {
            var datesWithValues = dates.Where(d => d.HasValue).Select(d=>d.Value);
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