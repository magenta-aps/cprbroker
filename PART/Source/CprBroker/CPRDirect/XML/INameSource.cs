using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public interface INameSource
    {
        string FirstName_s { get; }
        string MiddleName { get; }
        string LastName { get; }
        DateTime? NameStartDate { get; }
        char NameStartDateUncertainty { get; }

        NavnStrukturType ToNavnStrukturType();
    }
}
