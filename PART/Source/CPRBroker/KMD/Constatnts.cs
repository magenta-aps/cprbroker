using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.KMD
{
    public static class Constants
    {
        public static UnikIdType Actor 
        {
            get
            {
                // TODO: Fill this object
                return UnikIdType.Create(new Guid("{A4E9B3E0-275F-4b76-AADB-4398AA56B871}"));
            }
        }

        // TODO: Validate that this code is correct
        public const string DanishNationalityCode = "5100";

        //TODO: Add comment text
        public const string CommentText = "";
    }
}
