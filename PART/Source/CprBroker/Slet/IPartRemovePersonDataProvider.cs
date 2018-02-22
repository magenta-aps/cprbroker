using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CprBroker.Engine;
using CprBroker.PartInterface.Tracking;
using CprBroker.Schemas;

namespace CprBroker.Slet
{
    public interface IPartRemovePersonDataProvider : IDataProvider
    {
        bool RemovePerson(PersonIdentifier personIdentifier);
    }
}
