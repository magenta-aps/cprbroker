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
    public class StamPlusResponse : BaseResponse
    {
        public StamPlusResponse(string xml)
            : base(xml)
        {
        }

        public AttributListeType ToAttributListeType()
        {
            var searchPerson = new SearchPerson(this._Rows.First());
            return searchPerson.ToRegistreringType1().AttributListe;
        }
        
    }
}
