using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CprBroker.PartInterface.Tracking;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Engine;
using CprBroker.Engine.Local;

namespace CprBroker.Slet
{
    class RemovePersonManager : RequestProcessor
    {
        public BasicOutputType<bool> RemovePerson(string userToken, string appToken, Guid uuid)
        {
            var facadeMethod = new RemovePersonFacadeMethodInfo(userToken, appToken, uuid);
            return GetMethodOutput<bool>(facadeMethod);
        }
    }
}
