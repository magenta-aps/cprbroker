using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CprServices
{
    public class SearchMethod
    {
        public SearchMethod()
        {

        }

        public SearchMethod(string xmlTemplate)
        {

        }

        public bool CanSatisfy(SearchRequest request)
        {
            // TODO: Fill here
            throw new System.NotImplementedException();
        }

        public void Call(SearchRequest request)
        {
            // TODO: Fill here
            throw new System.NotImplementedException();
        }

        public System.Collections.Generic.List<InputField> InputFields = new List<InputField>();
        public string Name;
    }
}
