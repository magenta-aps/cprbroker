using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Schemas.Util
{
    public static class Misc
    {
        public static string FirstNonEmptyString(ref int index, params string[] args)
        {
            for (int i = index; i < args.Length; i++)
            {
                if (!string.IsNullOrEmpty(args[i]))
                {
                    index = i;
                    return args[i];
                }
            }
            return "";
        }
    }
}
