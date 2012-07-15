using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class StartRecordType
    {
        public TidspunktType ToTidspunktType()
        {
            return TidspunktType.Create(this.ProductionDate);
        }
    }
}
