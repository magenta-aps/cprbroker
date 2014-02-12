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
    }
}
