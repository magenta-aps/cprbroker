using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.NUnitTester
{
    public class PartTestData
    {
        public static Guid[] PersonUUIDs = new Guid[] { Guid.NewGuid() };
        public const string PersonUUIDsFieldName = "PersonUUIDs";

        public static Guid[][] PersonUUIDsArray = new Guid[][] { PersonUUIDs, null };
        public const string PersonUUIDsArrayFieldName = "PersonUUIDsArray";
    }
}
