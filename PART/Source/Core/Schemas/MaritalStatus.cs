using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
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
