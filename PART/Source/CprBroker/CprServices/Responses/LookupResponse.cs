using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CprServices.Responses
{
    public class LookupResponse : BaseResponse<SearchPerson>
    {
        public LookupResponse(string xml)
            : base(xml, "//c:Rolle[c:Field][@r='HovedRolle']", (elm, nsMgr) => new SearchPerson(elm, nsMgr))
        { }
    }
}
