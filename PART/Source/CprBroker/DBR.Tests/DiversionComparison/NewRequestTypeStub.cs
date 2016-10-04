using CprBroker.DBR;
using CprBroker.Providers.CPRDirect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Tests.DBR.DiversionComparison
{
    public class NewRequestTypeStub : NewRequestType
    {
        public NewRequestTypeStub(string contents) : base(contents)
        {
        }

        public override IndividualResponseType GetPerson(IEnumerable<ICprDirectPersonDataProvider> dataProviders, out ICprDirectPersonDataProvider usedProvider)
        {
            usedProvider = new CPRDirectExtractDataProvider();
            return ExtractManager.GetPerson(this.PNR);
        }

        public override bool PutSubscription()
        {
            return true;
        }

        public override IList<object> GetDatabaseInserts(string dprConnectionString, IndividualResponseType response)
        {
            return base.GetDatabaseInserts(dprConnectionString, response);
        }

        public override void UpdateDprDatabase(string dprConnectionString, IList<object> objectsToInsert)
        {
            // Do nothing
        }
    }
}
