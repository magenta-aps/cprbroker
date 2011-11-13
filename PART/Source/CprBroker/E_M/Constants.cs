using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.E_M
{
    public class Constants
    {
        public static readonly short DenmarkCountryCode = 5100;

        public static readonly short NullCountryCode = 0;
        public static readonly short UnknownCountryCode = 5999;
        public static readonly short StatelessCountryCode = 5103;
        public static readonly short AbroadCountryCode = 5102;
        public static readonly short ReservedCountryCode = 5001;

        public static readonly short[] UnknownCountryCodes = new short[] 
        {
            NullCountryCode,
            UnknownCountryCode,
            StatelessCountryCode,
            AbroadCountryCode,
            ReservedCountryCode
        };
    }
}
