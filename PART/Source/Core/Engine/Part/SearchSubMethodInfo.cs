using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine.Part
{
    /// <summary>
    /// Sub method for Search
    /// </summary>
    public class SearchSubMethodInfo : SubMethodInfo<IPartSearchDataProvider, Guid[]>
    {
        SoegInputType1 searchCriteria;


        private SearchSubMethodInfo()
        { }

        public SearchSubMethodInfo(SoegInputType1 input)
        {
            searchCriteria = input;

            LocalDataProviderOption = LocalDataProviderUsageOption.UseFirst;
            FailIfNoDataProvider = true;
            FailOnDefaultOutput = true;
        }

        public override Guid[] RunMainMethod(IPartSearchDataProvider prov)
        {
            return prov.Search(searchCriteria);
        }
    }
}
