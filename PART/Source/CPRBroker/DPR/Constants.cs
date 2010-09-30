using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Providers.DPR
{
    /// <summary>
    /// Contains constants that are used in DPR
    /// </summary>
    public class Constants
    {
        public class MaritalStatus
        {
            public const char Unmarried = 'U';
            public const char Married = 'G';
            public const char Divorced = 'F';
            public const char Deceased = 'D';
            public const char Widow = 'E';
            public const char RegisteredPartnership = 'P';
            public const char AbolitionOfRegisteredPartnership = 'O';
            public const char LongestLivingPartner = 'L';
        }
    }
}
