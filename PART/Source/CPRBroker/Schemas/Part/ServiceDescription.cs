using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Schemas.Part
{
    public static class ServiceDescription
    {
        public static class Part
        {
            public const string Service = "Allows accesss to CPR data through a standard PART interface";
            public static class Methods
            {
                

                public const string Read =
@"Find and return object (Always latest registration)
Input : ObjectID
Output : Object
Parameters : EffectDate
";
                
                
                public const string List =
@"Find and return several objects that match the ID List supplied
Input: ID List
Output : Object List
Parameters : RegistrationDate, EffectDate
";
                
                
                public const string Search =
@"Find and return several objects that match the search criteria
Input : Search Criteria
Output : ID List
Parameters : RegistrationDate, EffectDate
";

            }
        }
    }
}
