using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas.Part;

namespace CprBroker.DBR.Extensions
{
    public static partial class CprConverterExtensions
    {
        public static Child ToDpr(this ChildType child)
        {
            Child ch = new Child();
            ch.ParentPNR = Decimal.Parse(child.PNR);
            ch.ChildPNR = Decimal.Parse(child.ChildPNR);
            ch.MotherOrFatherDocumentation = null; //TODO: This is a concatenation of mother documentation and father documentation - can be retrieved from CPR Services
            return ch;
        }
    }
}
