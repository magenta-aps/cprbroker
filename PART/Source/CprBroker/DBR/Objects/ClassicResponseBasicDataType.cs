using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CprBroker.Providers.CPRDirect;
using CprBroker.Utilities;

namespace CprBroker.DBR
{
    public partial class ClassicResponseBasicDataType
    {
        public ClassicResponseBasicDataType()
        { }

        public ClassicResponseBasicDataType(NewResponseBasicDataType resp)
        {
            Contents = new string(resp.Contents.Take(this.Length).ToArray()).PadRight(this.Length);

            this.AddressProtectionDate = resp.AddressProtectionDate.HasValue ? resp.AddressProtectionDate.Value.ToString("ddMMyyyy") : "00000000";
            this.Status = resp.Status;
        }
    }
}
