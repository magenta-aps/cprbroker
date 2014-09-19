using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;
using CprBroker.Engine.Part;
using CprBroker.Utilities.ConsoleApps;
using CprBroker.Providers.CPRDirect;
using CprBroker.Data.Part;

namespace BatchClient
{
    class GenerateCprDirectFromExtracts : ConsoleEnvironment
    {
        public override string[] LoadCprNumbers()
        {
            Utilities.UpdateConnectionString(BrokerConnectionString);
            BrokerContext.Initialize(this.ApplicationToken, UserToken);
            Console.WriteLine("AppToken <{0}>, User <{1}>", ApplicationToken, UserToken);


            using (var dataContext = new ExtractDataContext(BrokerConnectionString))
            {
                return dataContext.ExtractItems.Select(ei => ei.PNR).Distinct().ToArray();
            }
        }


        public override void ProcessPerson(string pnr)
        {
            BrokerContext.Initialize(this.ApplicationToken, UserToken);
            using (var partDataContext = new PartDataContext(BrokerConnectionString))
            {
                using (var extractDataContext = new ExtractDataContext())
                {
                    var p = ExtractManager.GetPerson(pnr);
                    UuidCache cache = new UuidCache();
                    cache.FillCache(p.RelatedPnrs);

                    var reg = p.ToRegistreringType1(cache.GetUuid);
                    var pId = new CprBroker.Schemas.PersonIdentifier() { CprNumber = pnr, UUID = cache.GetUuid(pnr) };
                    CprBroker.Engine.Local.UpdateDatabase.UpdatePersonRegistration(pId, reg);
                }
            }
        }
    }
}
