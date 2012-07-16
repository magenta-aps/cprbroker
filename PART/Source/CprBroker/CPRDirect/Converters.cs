using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public static class Converters
    {
        public static string DecimalToString(decimal value)
        {
            return value.ToString("G");
        }
        public static string DecimalToString(decimal value, int length)
        {
            var ret = value.ToString(new string('0', length));
            if (ret.Length > length)
            {
                throw new ArgumentOutOfRangeException("value", string.Format("Value <{0}> cannot be fit in <{1}> characters", value, length));
            }
            return ret;
        }

        public static DateTime? ToDateTime(DateTime? value, char uncertainty)
        {
            if (uncertainty == ' ')
                return value;
            else
                return null;
        }

        public static String ToString(string value, char uncertainty)
        {
            if (uncertainty == ' ')
                return value;
            else
                return string.Empty;
        }

        public static string ToPnrStringOrNull(string pnr)
        {
            decimal decimalPnr;
            if (decimal.TryParse(pnr, out decimalPnr))
            {
                if (decimalPnr.ToString().Length == 9 || decimalPnr.ToString().Length == 10)
                {
                    return DecimalToString(decimalPnr, 10);
                }
            }
            return null;
        }

        public static PersonGenderCodeType ToPersonGenderCodeType(char gender)
        {
            switch (gender.ToString().ToUpper()[0])
            {
                case 'M':
                    return PersonGenderCodeType.male;
                    break;
                case 'K':
                    return PersonGenderCodeType.female;
                    break;
                default:
                    throw new ArgumentException(
                        string.Format("Invalied value <{0}>, must be either 'M' or 'K'", gender),
                        "gender");
            }
        }

        public static bool ToFolkekirkeMedlemIndikator(char churchRelation)
        {
            switch (churchRelation.ToString().ToUpper()[0])
            {
                case 'F':
                    return true;
                    break;
                case 'M':
                    return true;
                    break;
                case 'S':
                    return false;
                    break;
                case 'A':
                    return false;
                    break;
                case 'U':
                    return false;
                    break;
                default:
                    throw new ArgumentException(
                        string.Format("Invalied value <{0}>, must be 'A', 'M', 'F', 'S' or 'D'", churchRelation),
                        "churchRelation");
            }
        }

        public static bool IsValidCorrectionMarker(char correctionMarker)
        {
            return correctionMarker == Constants.CorrectionMarker.OK;
        }

    }
}
