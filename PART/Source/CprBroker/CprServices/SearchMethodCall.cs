using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CprServices
{
    public class SearchMethodCall
    {
        public string Name;
        public List<KeyValuePair<string, string>> InputFields = new List<KeyValuePair<string, string>>();

        public SearchMethodCall(SearchMethod template, SearchRequest request)
        {
            // TODO: Fill fields here
        }

        public string ToRequestXml()
        {
            throw new System.NotImplementedException();
        }

        public List<SearchPerson> ParseResponse(string responseXml)
        {
            throw new System.NotImplementedException();
        }

    }
}
