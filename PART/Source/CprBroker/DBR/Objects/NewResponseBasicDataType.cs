using CprBroker.Providers.CPRDirect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.DBR
{
    public partial class NewResponseBasicDataType
    {
        public NewResponseBasicDataType()
        { }

        public NewResponseBasicDataType(IndividualResponseType resp)
            : base(resp)
        { }
    }
}
