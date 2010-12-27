using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Util;

namespace CprBroker.DAL.Part
{
    public partial class Address
    {
        public object ToXmlType()
        {
            /*
             * The idea here is to fill the Schemas.UtilAddress fields and then return its ToOioAddress() method
            */

            Schemas.Util.Address address = new CprBroker.Schemas.Util.Address();
            address[AddressField.Building] = this.StreetBuildingIdentifier;
            address[AddressField.CareOfName] = this.CareOfName;
            address[AddressField.Door] = this.SuiteIdentifier;
            address[AddressField.Floor] = this.FloorIdentifier;
            address[AddressField.HouseNumber] = this.StreetBuildingIdentifier;
            address[AddressField.Line1] = this.Line1;
            address[AddressField.Line2] = this.Line2;
            address[AddressField.Line3] = this.Line3;
            address[AddressField.Line4] = this.Line4;
            address[AddressField.Line5] = this.Line5;
            address[AddressField.Line6] = this.Line6;


            address[AddressField.MunicipalityCode] = this.MunicipalityCode;
            if (this.Municipality != null)
            {
                address[AddressField.MunicipalityName] = this.Municipality.MunicipalityName;
            }
            address[AddressField.StreetCode] = this.StreetCode;
            address[AddressField.StreetName] = this.StreetName;
            address[AddressField.StreetNameForAddressing] = this.StreetNameForAddressing;

            address[AddressField.PostCode] = this.PostCode;
            address[AddressField.PostDistrictName] = this.PostDistrictName;

            // TODO: get the correct address status
            return address.ToOioAddress(PersonCivilRegistrationStatusCodeType.Item01);
        }
    }
}
