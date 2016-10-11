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

        public ClassicResponseBasicDataType(IndividualResponseType resp, PersonInfoExtended personInfo, string dprConnectionString)
            :base(resp,personInfo,dprConnectionString)
        {
            var addressProtectionDate = ProtectionType.FindProtection(resp.Protection, DateTime.Now, ProtectionType.ProtectionCategoryCodes.NameAndAddress)?.StartDate;
            this.AddressProtectionDate = addressProtectionDate.HasValue ? addressProtectionDate.Value.ToString("ddMMyyyy") : "00000000";

            this.Status = personInfo.PersonTotal.Status;
        }
    }
}
