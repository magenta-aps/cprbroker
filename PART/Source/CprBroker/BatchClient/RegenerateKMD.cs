using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Utilities.ConsoleApps;
using CprBroker.Data.Part;
using CprBroker.Utilities;
using CprBroker.Providers.KMD;

namespace BatchClient
{
    public class RegenerateKMD : ConsoleEnvironment
    {
        public override string[] LoadCprNumbers()
        {
            var actorId = CprBroker.Providers.KMD.Constants.Actor.Item;
            using (var dataContext = new PartDataContext(this.BrokerConnectionString))
            {
                return dataContext.PersonRegistrations
                    .Where(pr => pr.SourceObjects != null && pr.ActorRef.Value == actorId)
                    .OrderBy(pr => pr.PersonRegistrationId)
                    .Select(pr => pr.PersonRegistrationId.ToString())
                    .ToArray();
            }
        }

        public override void ProcessPerson(string personRegId)
        {
            var personRegistrationId = new Guid(personRegId);
            using (var dataContext = new PartDataContext(this.BrokerConnectionString))
            {
                var dbReg = dataContext.PersonRegistrations.Where(pr => pr.PersonRegistrationId == personRegistrationId).First();
                var kmdResponse = Strings.Deserialize<KmdResponse>(dbReg.SourceObjects.ToString());

                Func<string, Guid> cpr2uuidFunc = (string pnr) =>
                    {
                        pnr = pnr.PadLeft(10, ' ');
                        return dataContext.PersonMappings.Where(pm => pm.CprNumber == pnr).Select(pm => pm.UUID).First();
                    };

                var oioReg = kmdResponse.ToRegistreringType1(cpr2uuidFunc);
                dbReg.SetContents(oioReg);
            }
        }
    }
}
