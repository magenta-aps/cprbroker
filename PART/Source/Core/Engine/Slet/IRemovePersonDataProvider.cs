using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Engine;

namespace CprBroker.Slet
{
    public interface IRemovePersonDataProvider : IDataProvider
    {
        bool RemovePerson(PersonIdentifier personIdentifier);
    }
}