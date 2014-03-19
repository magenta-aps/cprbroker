using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CprBroker.Utilities.ConsoleApps;

namespace BatchClient
{
    class PopulateSearchCache : ConsoleEnvironment
    {
        public override string[] LoadCprNumbers()
        {
            using (var dataContext = new CprBroker.Data.Part.PartDataContext(BrokerConnectionString))
            {
                return dataContext.PersonRegistrations
                    .Select(pr => pr.UUID.ToString()).Distinct().OrderBy(pr => pr).ToArray();
            }
        }

        public override void ProcessPerson(string uuid)
        {
            using (var dataContext = new CprBroker.Data.Part.PartDataContext(BrokerConnectionString))
            {
                var reg = dataContext.PersonRegistrations.Where(pr => pr.UUID == new Guid(uuid))
                    .OrderByDescending(pr => pr.RegistrationDate)
                    .ThenByDescending(pr => pr.BrokerUpdateDate)
                    .First();
                dataContext.ExecuteCommand("UPDATE PersonRegistration SET Contents = Contents WHERE UUID={0} AND PersonRegistrationId = {1}", uuid, reg.PersonRegistrationId);
            }
        }
    }
}
