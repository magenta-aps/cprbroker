using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.DPR;
using CprBroker.Utilities;
using CprBroker.Utilities.ConsoleApps;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.DPR
{
    public class PersonAddressStub : PersonAddress
    { }

    public class PersonTotalStub : PersonTotal
    {
        public PersonTotalStub()
        {
            Status = 1;
        }
    }
}
