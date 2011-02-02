using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using System;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Schemas.Part.Enums;

namespace CprBroker.Providers.DPR
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

        private Schemas.Util.Address ToInternalAddress(PersonCivilRegistrationStatusCodeType civilStatus, Street street, ContactAddress contactAddress)
        {
            Schemas.Util.Address address = new CprBroker.Schemas.Util.Address();
            if (MunicipalityCode != 0)
            {
                address[CprBroker.Schemas.Util.AddressField.MunicipalityCode] = MunicipalityCode.ToString();
            }
            address[CprBroker.Schemas.Util.AddressField.MunicipalityName] = MunicipalityName;
            if (StreetCode != 0)
            {
                address[CprBroker.Schemas.Util.AddressField.StreetCode] = StreetCode.ToString();
            }

            if (street != null)
            {
                address[CprBroker.Schemas.Util.AddressField.StreetName] = street.StreetAddressingName;
            }
            address[CprBroker.Schemas.Util.AddressField.HouseNumber] = HouseNumber;
            address[CprBroker.Schemas.Util.AddressField.Floor] = Floor;
            address[CprBroker.Schemas.Util.AddressField.Door] = Door;

            if (PostCode != 0)
            {
                address[CprBroker.Schemas.Util.AddressField.PostCode] = PostCode.ToString();
            }

            if (contactAddress != null)
            {
                address[CprBroker.Schemas.Util.AddressField.Line1] = contactAddress.Line1;
                address[CprBroker.Schemas.Util.AddressField.Line2] = contactAddress.Line2;
                address[CprBroker.Schemas.Util.AddressField.Line3] = contactAddress.Line3;
                address[CprBroker.Schemas.Util.AddressField.Line4] = contactAddress.Line4;
                address[CprBroker.Schemas.Util.AddressField.Line5] = contactAddress.Line5;
            }
            address[CprBroker.Schemas.Util.AddressField.PostDistrictName] = PostDistrictName;
            return address;
        }

        internal object ToOioAddress(PersonCivilRegistrationStatusCodeType civilStatus, Street street, ContactAddress contactAddress)
        {
            return this.ToInternalAddress(civilStatus, street, contactAddress).ToOioAddress(civilStatus);
        }

        internal Schemas.Part.Address ToPartAddress(PersonCivilRegistrationStatusCodeType civilStatus, Street street, ContactAddress contactAddress)
        {
            return this.ToInternalAddress(civilStatus, street, contactAddress).ToPartAddress(civilStatus);
        }

        public CivilStatusKodeType PartCivilStatus
        {
            get
            {
                if (this.MaritalStatus.HasValue)
                {
                    switch (this.MaritalStatus)
                    {
                        case Constants.MaritalStatus.Unmarried:
                            return CivilStatusKodeType.Ugift;
                        case Constants.MaritalStatus.Married:
                            return CivilStatusKodeType.Gift;
                        case Constants.MaritalStatus.Divorced:
                            return CivilStatusKodeType.Skilt;
                        case Constants.MaritalStatus.Widow:
                            return CivilStatusKodeType.Enke;
                        case Constants.MaritalStatus.RegisteredPartnership:
                            return CivilStatusKodeType.RegistreretPartner;
                        case Constants.MaritalStatus.AbolitionOfRegisteredPartnership:
                            return CivilStatusKodeType.OphaevetPartnerskab;
                        case Constants.MaritalStatus.LongestLivingPartner:
                            return CivilStatusKodeType.Laengstlevende;
                        // TODO : GetPropertyValuesOfType fromDate latest marital status before this record
                        case Constants.MaritalStatus.Deceased:
                            return CivilStatusKodeType.Ugift;
                        // TODO: When to use CivilStatusKode.Separeret?
                    }
                }
                throw new NotSupportedException("Unknown marital status");
            }
        }

        public LivStatusKodeType PartLifeStatus
        {
            get
            {
                return Schemas.Util.Enums.ToLifeStatus(this.Status, Utilities.DateFromDecimal(DateOfBirth));
            }
        }

        //TODO: Add logic to get value fromDate DTBOERN if not found in DTTOTAL
        public static decimal? GetParent(char? parentMarker, string parentPnrOrBirthdate)
        {
            if (parentMarker.HasValue && parentMarker.Value == '*')
            {
                string parentPnr = parentPnrOrBirthdate.Trim().Replace("-", "");
                decimal ret;
                if (parentPnr.Length == 10 && decimal.TryParse(parentPnr, out ret) && ret > 0)
                {
                    return ret;
                }
            }
            return null;
        }



    }
}