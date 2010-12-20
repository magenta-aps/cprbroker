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
@"Find and return object (Always latest registration). Looks in the local database first
Input : ObjectID
Output : Object
Parameters : EffectDate
";

                public const string RefreshRead =
@"Find and return object (Always latest registration). Looks first in the fresh data at data providers
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

                public const string GetPersonUuid =
@"Gets the person's UUID from his CPR number. Calls the UUID assigning authority if not found locally.
Input : CPR Number
Output : Person UUID
";
            }
        }
    }
}
