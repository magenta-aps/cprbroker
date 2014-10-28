using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CprServices
{
    public class SearchRequest
    {
        public SearchRequest()
        {

        }

        public SearchRequest(SoegAttributListeType attributes)
        {
            // TODO: Fill criteria here
        }

        public List<KeyValuePair<string, string>> CriteriaFields = new List<KeyValuePair<string, string>>();
    }
}
