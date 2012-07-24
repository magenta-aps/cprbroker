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
            // TODO: Is this the correct registration date?
            return TidspunktType.Create(this.ProductionDate);
        }
    }
}
