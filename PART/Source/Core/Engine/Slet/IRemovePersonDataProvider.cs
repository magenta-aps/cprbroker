using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Engine;

namespace CprBroker.Providers.Slet
{
    public interface IRemovePersonDataProvider : IDataProvider
    {
        bool RemovePerson(PersonIdentifier personIdentifier);
    }
}