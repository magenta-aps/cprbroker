using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CPRDirect
{
    public static class Converters
    {
        public static string DecimalToString(decimal value, int length)
        {
            var ret = value.ToString(new string('0', length));
            if (ret.Length > length)
            {
                throw new ArgumentOutOfRangeException("value", string.Format("Value <{0}> cannot be fit in <{1}> characters", value, length));
            }
            return ret;
        }

        public static DateTime? ToDateTime(DateTime value, char uncertainty)
        {
            if (uncertainty == ' ')
                return value;
            else
                return null;
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

    }
}
