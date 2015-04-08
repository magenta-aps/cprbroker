using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
namespace CprBroker.Tests.ServicePlatform
{
    public class Program
    {
        public static void Main()
        {
            CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
            
            var prov = ServicePlatformDataProviderFactory.Create();

            var inp = SearchCriteriaFactory.Create();

            var ret = prov.SearchList(inp);
            //var ret2 = prov.PutSubscription(new PersonIdentifier() { CprNumber = "0123456789" });
            CprBroker.Schemas.QualityLevel? ql;
            foreach(var pnr in CprBroker.Tests.CPRDirect.Utilities.PNRs)
            {
                var ret3 = prov.Read(new PersonIdentifier() { CprNumber = pnr, UUID = Guid.NewGuid() }, null, p => Guid.NewGuid(), out ql);
            }
        }
    }
}
