using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public partial class TidspunktType
    {
        public static TidspunktType Create()
        {
            return new TidspunktType()
            {
                //TODO : Xml element called either Tidsstempel:datetime or GraenseIndikator:bool
                Item = null
            };
        }
    }
}
