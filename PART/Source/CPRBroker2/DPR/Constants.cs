using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.DPR
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

        public static UnikIdType Actor
        {
            get
            {
                // TODO: Fill this object
                return UnikIdType.Create(new Guid("{4A953CF9-B4C1-4ce9-BF09-2BF655DC61C7}"));
            }
        }


        public const string CprNationalityKmdCode = "5001";
        public const string StatelessKmdCode = "5103";
        public const string DenmarkKmdCode = "5100";

        //TODO: Add comment text
        public const string CommentText = "";

    }
}
