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
    }
}
