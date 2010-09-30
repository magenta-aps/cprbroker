using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using System;
using CPRBroker.Schemas;

namespace CPRBroker.Providers.DPR
{
    public partial class PersonTotal
    {
        public MaritalStatusCodeType MaritalStatusCodeType
        {
            get
            {
                return Schemas.Util.Enums.GetMaritalStatus(MaritalStatus.Value);
            }
        }

        internal object ToOioAddress(PersonCivilRegistrationStatusCodeType civilStatus, Street street,ContactAddress contactAddress)
        {
            Schemas.Util.Address address = new CPRBroker.Schemas.Util.Address();
            if (MunicipalityCode != 0)
            {
                address[CPRBroker.Schemas.Util.AddressField.MunicipalityCode] = MunicipalityCode.ToString();
            }
            address[CPRBroker.Schemas.Util.AddressField.MunicipalityName] = MunicipalityName;
            if (StreetCode != 0)
            {
                address[CPRBroker.Schemas.Util.AddressField.StreetCode] = StreetCode.ToString();
            }

            if(street !=null)
            {
                address[CPRBroker.Schemas.Util.AddressField.StreetName] = street.StreetAddressingName;
            }
            address[CPRBroker.Schemas.Util.AddressField.HouseNumber] = HouseNumber;
            address[CPRBroker.Schemas.Util.AddressField.Floor] = Floor;
            address[CPRBroker.Schemas.Util.AddressField.Door] = Door;
            
            if (PostCode != 0)
            {
                address[CPRBroker.Schemas.Util.AddressField.PostCode] = PostCode.ToString();
            }

            if (contactAddress != null)
            {
                address[CPRBroker.Schemas.Util.AddressField.Line1] = contactAddress.Line1;
                address[CPRBroker.Schemas.Util.AddressField.Line2] = contactAddress.Line2;
                address[CPRBroker.Schemas.Util.AddressField.Line3] = contactAddress.Line3;
                address[CPRBroker.Schemas.Util.AddressField.Line4] = contactAddress.Line4;
                address[CPRBroker.Schemas.Util.AddressField.Line5] = contactAddress.Line5;
            }
            address[CPRBroker.Schemas.Util.AddressField.PostDistrictName] = PostDistrictName;

            return address.ToOioAddress(civilStatus);
        }
    }
}