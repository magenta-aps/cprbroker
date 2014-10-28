using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CprServices
{
    /// <summary>
    /// Provides a one constructor time calculation of the search plan
    /// </summary>
    public class SearchPlan
    {
        public bool IsSatisfactory { get; private set; }

        public List<SearchMethodCall> PlannedCalls = new List<SearchMethodCall>();

        public SearchPlan(SearchRequest request, params SearchMethod[] availableMethods)
        {
            // TODO: Fill IsSatisfactory
            // TODO: Fill planned calls here
        }


    }
}
