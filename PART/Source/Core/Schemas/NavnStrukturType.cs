using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public partial class NavnStrukturType
    {
        public static NavnStrukturType Create(params string[] names)
        {
            // TODO: Fill the rest of this object
            return new NavnStrukturType() 
            {
                KaldenavnTekst = null,
                NoteTekst=null,
                PersonNameForAddressingName=null,
                PersonNameStructure = new PersonNameStructureType(names),
            };
        }
    }
}
