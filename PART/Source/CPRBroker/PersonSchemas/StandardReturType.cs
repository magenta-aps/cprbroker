using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public partial class StandardReturType
    {
        public static StandardReturType Create(string code, string text)
        {
            return new StandardReturType()
            {
                StatuskodeKode = code,
                FejlbeskedTekst = text
            };
        }
    }
}
