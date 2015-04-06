using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CprServices.Responses
{
    public class SearchResponse : BaseResponse<SearchPerson>
    {
        public SearchResponse(string xml)
            : base(xml, "//c:Rolle[@r='HovedRolle']/c:Table/c:Row[not(@u)]", (elm, nsMgr) => new SearchPerson(elm, nsMgr))
        { }
    }
}
