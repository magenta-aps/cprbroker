using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{


    public enum HttpErrorCode
    {
        OK = 200,
        UNSPECIFIED = 500, /* Server */
        DATASOURCE_UNAVAILABLE = 503, /* Server unavailable*/
        BAD_CLIENT_REQUEST = 400, /* Client */
        NOT_FOUND = 404, /*  */
        NOT_IMPLEMENTED = 501
    }
    
    public partial class StandardReturType
    {
        public static StandardReturType Create(HttpErrorCode code)
        {
            return Create(code, code.ToString());

        }
        public static StandardReturType Create(HttpErrorCode code, string text)
        {
            return new StandardReturType()
            {
                StatuskodeKode = code.ToString(),
                FejlbeskedTekst = text
            };
        }
    }
}
