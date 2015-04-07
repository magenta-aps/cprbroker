using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CprServices.Responses
{
    public class ErrorResponse : GenericResponse
    {
        public ErrorResponse(string xml)
            : base(xml)
        { }

        public string[] ToErrorLines()
        {
            return this.RowItems.Select(ri => ri["Kvittxt"]).ToArray();
        }
    }
}
