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
        {
            this.PNR = personInfo.PersonTotal.PNR.ToPnrDecimalString();
            this.LastName = personInfo.PersonName.LastName;
            this.FirstAndMiddleNames = personInfo.PersonName.FirstName;
            this.CareOfName = personInfo.Address?.CareOfName;
            this.StreetName = personInfo.Address?.StreetAddressingName;
            this.HouseNumber = personInfo.Address?.HouseNumber;
            this.Floor = personInfo.Address?.Floor;
            this.Door = personInfo.Address?.DoorNumber;
            this.BNR = personInfo.Address?.GreenlandConstructionNumber;

            if ((personInfo.Address?.PostCode).HasValue)
            {
                this.PostCode = personInfo.Address.PostCode;
                this.PostDistrict = Providers.DPR.PostDistrict.GetPostText(
                    dprConnectionString,
                    personInfo.Address.MunicipalityCode,
                    personInfo.Address.StreetCode,
                    personInfo.Address.HouseNumber);
            }            
            this.AddressProtectionDate = ProtectionType.FindProtection(resp.Protection, DateTime.Now, ProtectionType.ProtectionCategoryCodes.NameAndAddress)?.StartDate;
            this.Status = personInfo.PersonTotal.Status;
        }
    }
}
