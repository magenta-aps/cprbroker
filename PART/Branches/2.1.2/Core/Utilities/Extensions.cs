using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Utilities
{
    public static class Extensions
    {
        public static string ToDecimalString(this decimal val)
        {
            return val.ToString("F0");
        }

        public static string ToPnrDecimalString(this decimal val)
        {
            var ret = ToDecimalString(val);
            int cprNumberLength = 10;
            ret = new string('0', cprNumberLength - ret.Length) + ret;
            return ret;
        }
    }
}
