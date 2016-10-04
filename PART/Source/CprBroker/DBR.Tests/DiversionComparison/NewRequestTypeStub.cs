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

        public override void UpdateDprDatabase(string dprConnectionString, IndividualResponseType response = null)
        {
            // Do nothing
        }
    }
}
