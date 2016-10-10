using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// TODO: Change namespace to CprBroker.PartInterface
namespace CprBroker.Utilities
{
    public static class Extensions
    {
        public static string ToPnrDecimalString(this decimal val, bool dash = false)
        {
            var ret = val.ToDecimalString();
            int cprNumberLength = 10;
            ret = new string('0', cprNumberLength - ret.Length) + ret;
            if (dash)
                ret = ret.Substring(0, 6) + "-" + ret.Substring(6, 4);
            return ret;
        }
    }
}
