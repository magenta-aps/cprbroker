using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Providers.CprServices;
using CprBroker.Providers.CprServices.Responses;

namespace CprBroker.Providers.ServicePlatform.Responses
{
    public class StamPlusResponse : BaseResponse<SearchPerson>
    {
        public StamPlusResponse(string xml)
            : base(xml, (e, nsMgr) => new SearchPerson(e, nsMgr))
        {
        }

        public AttributListeType ToAttributListeType()
        {
            var searchPerson = this._RowItems.First();
            return searchPerson.ToRegistreringType1().AttributListe;
        }
    }
}
