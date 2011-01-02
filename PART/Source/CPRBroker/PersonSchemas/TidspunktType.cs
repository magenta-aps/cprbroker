using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public partial class TidspunktType
    {
        public static TidspunktType Create(DateTime? value)
        {
            var ret = new TidspunktType();
            
            //TODO : Xml element called either Tidsstempel:datetime or GraenseIndikator:bool
            if (value.HasValue)
            {
                ret.Item = value.Value;
            }
            else
            {
                //TODO: is the value of true relevant here? cou
                ret.Item = false;
            }
            return ret;
        }

        public DateTime? ToDateTime()
        {
            if (Item is DateTime)
            {
                return (DateTime)Item;
            }
            return null;
        }

    }
}
