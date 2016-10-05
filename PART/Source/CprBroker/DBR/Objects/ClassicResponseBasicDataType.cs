using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.DBR
{
    public partial class ClassicResponseBasicDataType
    {
        public ClassicResponseBasicDataType()
        { }

        public ClassicResponseBasicDataType(IndividualResponseType resp)
        {
            this.PNR = resp.PersonInformation.PNR;
            this.LastName = resp.CurrentNameInformation.LastName;
            this.FirstAndMiddleNames = resp.CurrentNameInformation.FirstName_s;
            this.CareOfName = resp.ClearWrittenAddress.CareOfName;
            this.StreetName = resp.ClearWrittenAddress.StreetAddressingName;
            this.HouseNumber = resp.ClearWrittenAddress.HouseNumber;
            this.Floor = resp.ClearWrittenAddress.Floor;
            this.Door = resp.ClearWrittenAddress.Door;
            this.BNR = resp.ClearWrittenAddress.BuildingNumber;
            this.PostCode = resp.ClearWrittenAddress.PostCode;
            this.PostDistrict = resp.ClearWrittenAddress.PostDistrictText;
            this.AddressProtectionDate = ProtectionType.FindProtection(resp.Protection, DateTime.Now, ProtectionType.ProtectionCategoryCodes.NameAndAddress)?.StartDate;
            this.Status = resp.PersonInformation.Status;
        }
    }
}
