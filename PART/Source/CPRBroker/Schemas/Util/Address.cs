using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Util
{
    /// <summary>
    /// All possible fields of an adderss
    /// </summary>
    public enum AddressField
    {
        CareOfName,
        StreetName,
        StreetNameForAddressing,
        StreetCode,
        HouseNumber,
        Floor,
        Door,
        Building,
        PostCode,
        PostDistrictName,
        PostBox,
        MunicipalityCode,
        MunicipalityName,
        Alpha2CountryCode,
        EnglishCountryName,
        DanishCountryName,
        LocationDescription,
        MailDeliverSubLocationIdentifier,
        DistrictSubDivisionIdentifier,
        //PostBoxIdentifier,
        AddresseeName,
        /// <summary>
        /// C/O name, Care-of name, i.e. "living at", typically person, family, college, nursing home or the like.
        /// </summary>
        Line1,
        /// <summary>
        /// Locality (building name): Equivalent to the element MailDeliverySublocationIdentifier, i.e. name of a farm, estate, building or dwelling, which is used as an additional postal address identifier.
        /// </summary>
        Line2,
        /// <summary>
        /// Standard street address. In this schema equivalent to an aggregation of the elements StreetNameForAddressingName, StreetBuildingIdenfier, FloorIdentifier and SuiteIdentifier, i.e. a line of text which contains street name, house number including letter, floor and door if any.
        /// </summary>
        Line3,
        /// <summary>
        /// Name of city (place name). In this schema equivalent to DistrictSubdivisionIdentifier, i.e. the city name that is specified as a part of the official address spcification for a certain street or specific parts of a street.
        /// </summary>
        Line4,
        /// <summary>
        ///  Postal code and postal district. Line of text which contains Postal code and postal district.
        /// </summary>
        Line5,
        /// <summary>
        /// Free line of text. CPR uses the line for customer service with a key constant.
        /// </summary>
        Line6
    }

    /// <summary>
    /// Represents an address as a collection of name/value
    /// </summary>
    public class Address : Dictionary<AddressField, string>
    {
        /// <summary>
        /// Represents an address field that is composed of one or more fields
        /// </summary>
        public class AddressComposition
        {
            public AddressField Parent;
            public AddressField[] Children;
        }

        static Address()
        {
            InitializeCompositions();
        }

        /// <summary>
        /// Initializes the list of all address compositions
        /// </summary>
        static void InitializeCompositions()
        {
            AddressCompositions.Add(
                new AddressComposition()
                {
                    Parent = AddressField.Line1,
                    Children = new AddressField[] { AddressField.CareOfName }
                });

            AddressCompositions.Add(
                new AddressComposition()
                {
                    Parent = AddressField.Line2,
                    Children = new AddressField[] { AddressField.Building }
                });

            AddressCompositions.Add(
                new AddressComposition()
                {
                    Parent = AddressField.Line3,
                    Children = new AddressField[] { AddressField.StreetName, AddressField.HouseNumber, AddressField.Floor, AddressField.Door }
                });

            // TODO: It is actually city name rather than municipality name
            AddressCompositions.Add(
                new AddressComposition()
                {
                    Parent = AddressField.Line4,
                    Children = new AddressField[] { AddressField.MunicipalityName }
                });

            AddressCompositions.Add(
                new AddressComposition()
                {
                    Parent = AddressField.Line5,
                    Children = new AddressField[] { AddressField.PostCode, AddressField.PostDistrictName }
                });
        }

        public Address()
        {
        }

        public static readonly PersonCivilRegistrationStatusCodeType[] DenmarkAddressStates = new PersonCivilRegistrationStatusCodeType[] { PersonCivilRegistrationStatusCodeType.Item01, PersonCivilRegistrationStatusCodeType.Item03 };
        public static readonly PersonCivilRegistrationStatusCodeType[] GreenlandAddressStates = new PersonCivilRegistrationStatusCodeType[] { PersonCivilRegistrationStatusCodeType.Item05, PersonCivilRegistrationStatusCodeType.Item07 };
        public static readonly char FieldSeparator = ' ';
        private static readonly List<AddressComposition> AddressCompositions = new List<AddressComposition>();


        public Nullable<DateTime> ProtectionDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Either a DanishAddressStructure or a ForeignAddressStructure</returns>
        public DanishAddressStructureType ToOioAddress(PersonCivilRegistrationStatusCodeType civilRegistrationStatus)
        {
            DanishAddressStructureType returnAddress;
            /* First level differentiation
             * Danish vs foreign address
             */
            //if (DenmarkAddressStates.Contains(civilRegistrationStatus) || GreenlandAddressStates.Contains(civilRegistrationStatus))
            if (true)// Assume danish address
            {
                DanishAddressStructureType danishAddress = new DanishAddressStructureType();
                //danishAddress.AddressStatusCode = AddressStatusCodeType.Item1;
                danishAddress.CareOfName = GetFieldValue(AddressField.CareOfName);
                danishAddress.CompletePostalLabel = new CompletePostalLabelType();
                danishAddress.CompletePostalLabel.AddresseeName = GetFieldValue(AddressField.AddresseeName);
                danishAddress.CompletePostalLabel.PostalAddressFirstLineText = GetFieldValue(AddressField.Line1);
                danishAddress.CompletePostalLabel.PostalAddressSecondLineText = GetFieldValue(AddressField.Line2);
                danishAddress.CompletePostalLabel.PostalAddressThirdLineText = GetFieldValue(AddressField.Line3);
                danishAddress.CompletePostalLabel.PostalAddressFourthLineText = GetFieldValue(AddressField.Line4);
                danishAddress.CompletePostalLabel.PostalAddressFifthLineText = GetFieldValue(AddressField.Line5);
                danishAddress.CompletePostalLabel.PostalAddressSixthLineText = GetFieldValue(AddressField.Line6);
              
                // Item
                var countNonEmpty = (from field in this
                                     where field.Key != AddressField.CareOfName && !string.IsNullOrEmpty(field.Value)
                                     select field).Count();

                if (IsAddressComplete()) // either complete or greenland complete                
                {
                    if (DenmarkAddressStates.Contains(civilRegistrationStatus))
                    {
                        AddressCompleteType addressComplete = new AddressCompleteType();
                        addressComplete.AddressAccess = new AddressAccessType();
                        addressComplete.AddressAccess.MunicipalityCode = GetFieldValue(AddressField.MunicipalityCode);
                        addressComplete.AddressAccess.StreetBuildingIdentifier = GetFieldValue(AddressField.Building);
                        addressComplete.AddressAccess.StreetCode = GetFieldValue(AddressField.StreetCode);

                        addressComplete.AddressPostal = new AddressPostalType();
                        addressComplete.AddressPostal.CountryIdentificationCode = new CountryIdentificationCodeType();
                        //addressComplete.AddressPostal.CountryIdentificationCode.Value = "";

                        addressComplete.AddressPostal.DistrictName = GetFieldValue(AddressField.PostDistrictName);
                        //addressComplete.AddressPostal.DistrictSubdivisionIdentifier = "";
                        addressComplete.AddressPostal.FloorIdentifier = GetFieldValue(AddressField.Floor);
                        //addressComplete.AddressPostal.MailDeliverySublocationIdentifier = "";
                        addressComplete.AddressPostal.PostCodeIdentifier = GetFieldValue(AddressField.PostCode);
                        //addressComplete.AddressPostal.PostOfficeBoxIdentifier = "";
                        addressComplete.AddressPostal.StreetBuildingIdentifier = GetFieldValue(AddressField.Building);
                        addressComplete.AddressPostal.StreetName = GetFieldValue(AddressField.StreetName);
                        //addressComplete.AddressPostal.StreetNameForAddressingName = "";
                        addressComplete.AddressPostal.SuiteIdentifier = GetFieldValue(AddressField.Door);

                        danishAddress.Item = addressComplete;
                    }
                    else// Greenland
                    {
                        AddressCompleteGreenlandType addresssCompleteGreenland = new AddressCompleteGreenlandType();
                        addresssCompleteGreenland.CountryIdentificationCode = new CountryIdentificationCodeType();
                        //addresssCompleteGreenland.CountryIdentificationCode.Value = "";
                        addresssCompleteGreenland.DistrictName = GetFieldValue(AddressField.PostDistrictName);
                        //addresssCompleteGreenland.DistrictSubdivisionIdentifier = "";
                        addresssCompleteGreenland.FloorIdentifier = GetFieldValue(AddressField.Floor);
                        addresssCompleteGreenland.GreenlandBuildingIdentifier = GetFieldValue(AddressField.Building);
                        //addresssCompleteGreenland.MailDeliverySublocationIdentifier = "";
                        //addresssCompleteGreenland.MunicipalityCode = "";
                        addresssCompleteGreenland.PostCodeIdentifier = GetFieldValue(AddressField.PostCode);
                        addresssCompleteGreenland.StreetBuildingIdentifier = GetFieldValue(AddressField.Building);
                        //addresssCompleteGreenland.StreetCode = "";
                        addresssCompleteGreenland.StreetName = GetFieldValue(AddressField.StreetName);
                        //addresssCompleteGreenland.StreetNameForAddressingName = "";
                        addresssCompleteGreenland.SuiteIdentifier = GetFieldValue(AddressField.Door);

                        danishAddress.Item = addresssCompleteGreenland;
                    }
                }
                else // Address not complete
                {
                    AddressNotCompleteType addressNotComplete = new AddressNotCompleteType();
                    //addressNotComplete.CountryIdentificationCode = "";
                    addressNotComplete.DistrictName = GetFieldValue(AddressField.PostDistrictName);
                    //addressNotComplete.DistrictSubdivisionIdentifier = "";
                    addressNotComplete.FloorIdentifier = GetFieldValue(AddressField.Floor);
                    addressNotComplete.GreenlandBuildingIdentifier = GetFieldValue(AddressField.Building);
                    //addressNotComplete.MailDeliverySublocationIdentifier = "";
                    //addressNotComplete.MunicipalityCode = "";
                    addressNotComplete.PostCodeIdentifier = GetFieldValue(AddressField.PostCode);
                    addressNotComplete.StreetBuildingIdentifier = GetFieldValue(AddressField.Building);
                    //addressNotComplete.StreetCode = "";
                    addressNotComplete.StreetName = GetFieldValue(AddressField.StreetName);
                    //addressNotComplete.StreetNameForAddressingName = "";
                    addressNotComplete.SuiteIdentifier = GetFieldValue(AddressField.Door);

                    danishAddress.Item = addressNotComplete;
                }

                //danishAddress.MunicipalityName = "";
                returnAddress = danishAddress;
            }
            /*else
            {
                ForeignAddressStructureType foreignAddress = new ForeignAddressStructureType();
                foreignAddress.CountryIdentificationCode = new CountryIdentificationCodeType();
                //foreignAddress.CountryIdentificationCode.Value = "";
                //foreignAddress.LocationDescriptionText = "";
                foreignAddress.PostalAddressFirstLineText = GetFieldValue(AddressField.Line1);
                foreignAddress.PostalAddressSecondLineText = GetFieldValue(AddressField.Line2);
                foreignAddress.PostalAddressThirdLineText = GetFieldValue(AddressField.Line3);
                foreignAddress.PostalAddressFourthLineText = GetFieldValue(AddressField.Line4);
                foreignAddress.PostalAddressFifthLineText = GetFieldValue(AddressField.Line5);


                returnAddress = foreignAddress;
            }*/


            return returnAddress;
        }

        public Part.Address ToPartAddress(PersonCivilRegistrationStatusCodeType civilRegistrationStatus)
        {            
            var addr = ToOioAddress(civilRegistrationStatus);
            if (addr.Item is AddressCompleteType)
            {
                return new Part.AddressDenmark()
                {
                    AddressComplete = addr.Item as Part.AddressCompleteType,
                    AddressPoint = null,
                    AddressUnknown = false,
                    Note = "",
                    SpecialStreetCode = ""
                };
            }
            else if (addr.Item is AddressCompleteGreenlandType)
            {
                return new Part.AddressGreenland()
                {
                    AddressUnknown = false,
                    GreenlandicAddress = addr.Item as Part.AddressCompleteGreenlandType,
                    Note = "",
                    SpecialStreetCode = ""
                };
            }
            else if (addr.Item is AddressNotCompleteType)
            {
                if (DenmarkAddressStates.Contains(civilRegistrationStatus))
                {
                    return new Part.AddressDenmark()
                    {
                        AddressUnknown = true,
                        SpecialStreetCode = "",
                        AddressComplete = null,
                        AddressPoint = null,
                        Note = ""
                    };
                }
                else
                {
                    return new Part.AddressGreenland()
                    {
                        AddressUnknown = true,
                        GreenlandicAddress = null,
                        Note = "",
                        SpecialStreetCode = ""
                    };
                }
            }
            return null;
        }

        /// <summary>
        /// Creates an Address fromDate an OIO address
        /// </summary>
        /// <param name="oioAddress"></param>
        /// <returns></returns>
        public static Address FromOioAddress(object oioAddress)
        {
            Address ret = new Address();

            #region Setting Address Information
            if (oioAddress is DanishAddressStructureType)
            {
                DanishAddressStructureType oioDanishAddress = (DanishAddressStructureType)oioAddress;

                #region Update current address
                //personAddress.AddressStatusId = Enums.GetAddressStatusId(oioDanishAddress.AddressStatusCode);
                //personAddress.AddressTypeId = Enums.GetAddressIdentifierId(oioPerson.AddressIdentifierCode);

                ret[AddressField.MunicipalityName] = oioDanishAddress.MunicipalityName;
                ret[AddressField.CareOfName] = oioDanishAddress.CareOfName;

                // Postal label
                ret[AddressField.AddresseeName] = oioDanishAddress.CompletePostalLabel.AddresseeName;
                ret[AddressField.Line1] = oioDanishAddress.CompletePostalLabel.PostalAddressFirstLineText;
                ret[AddressField.Line2] = oioDanishAddress.CompletePostalLabel.PostalAddressSecondLineText;
                ret[AddressField.Line3] = oioDanishAddress.CompletePostalLabel.PostalAddressThirdLineText;
                ret[AddressField.Line4] = oioDanishAddress.CompletePostalLabel.PostalAddressFourthLineText;
                ret[AddressField.Line5] = oioDanishAddress.CompletePostalLabel.PostalAddressFifthLineText;
                ret[AddressField.Line6] = oioDanishAddress.CompletePostalLabel.PostalAddressSixthLineText;

                if (oioDanishAddress.Item is AddressCompleteType)
                {
                    #region AddressCompleteType
                    AddressCompleteType completeType = (AddressCompleteType)oioDanishAddress.Item;

                    if (completeType.AddressAccess != null)
                    {
                        ret[AddressField.MunicipalityCode] = completeType.AddressAccess.MunicipalityCode;
                        ret[AddressField.StreetCode] = completeType.AddressAccess.StreetCode;
                        ret[AddressField.Building] = completeType.AddressAccess.StreetBuildingIdentifier;
                    }
                    if (completeType.AddressPostal != null)
                    {
                        if (completeType.AddressPostal.CountryIdentificationCode != null)
                        {
                            ret[AddressField.Alpha2CountryCode] = completeType.AddressPostal.CountryIdentificationCode.Value;
                        }
                        ret[AddressField.StreetName] = completeType.AddressPostal.StreetName;
                        ret[AddressField.StreetNameForAddressing] = completeType.AddressPostal.StreetNameForAddressingName;
                        ret[AddressField.Door] = completeType.AddressPostal.SuiteIdentifier;
                        ret[AddressField.Floor] = completeType.AddressPostal.FloorIdentifier;
                        ret[AddressField.MailDeliverSubLocationIdentifier] = completeType.AddressPostal.MailDeliverySublocationIdentifier;
                        ret[AddressField.PostCode] = completeType.AddressPostal.PostCodeIdentifier;
                        ret[AddressField.DistrictSubDivisionIdentifier] = completeType.AddressPostal.DistrictSubdivisionIdentifier;
                        ret[AddressField.PostBox] = completeType.AddressPostal.PostOfficeBoxIdentifier;
                        ret[AddressField.PostDistrictName] = completeType.AddressPostal.DistrictName;
                    }
                    #endregion
                }
                else if (oioDanishAddress.Item is AddressCompleteGreenlandType)
                {
                    #region AddressCompleteGreenlandType
                    AddressCompleteGreenlandType completeGreenLandType = (AddressCompleteGreenlandType)oioDanishAddress.Item;
                    ret[AddressField.MunicipalityCode] = completeGreenLandType.MunicipalityCode;
                    ret[AddressField.Building] = completeGreenLandType.StreetBuildingIdentifier;
                    ret[AddressField.StreetCode] = completeGreenLandType.StreetCode;
                    if (completeGreenLandType.CountryIdentificationCode != null)
                    {
                        ret[AddressField.Alpha2CountryCode] = completeGreenLandType.CountryIdentificationCode.Value;
                    }
                    ret[AddressField.StreetName] = completeGreenLandType.StreetName;
                    ret[AddressField.StreetNameForAddressing] = completeGreenLandType.StreetNameForAddressingName;
                    ret[AddressField.Door] = completeGreenLandType.SuiteIdentifier;
                    ret[AddressField.Floor] = completeGreenLandType.FloorIdentifier;
                    ret[AddressField.MailDeliverSubLocationIdentifier] = completeGreenLandType.MailDeliverySublocationIdentifier;
                    ret[AddressField.DistrictSubDivisionIdentifier] = completeGreenLandType.DistrictSubdivisionIdentifier;
                    ret[AddressField.PostCode] = completeGreenLandType.PostCodeIdentifier;
                    ret[AddressField.PostDistrictName] = completeGreenLandType.DistrictName;
                    #endregion
                }
                else if (oioDanishAddress.Item is AddressNotCompleteType)
                {
                    #region AddressNotCompleteType
                    AddressNotCompleteType notCompleteType = (AddressNotCompleteType)oioDanishAddress.Item;
                    ret[AddressField.MunicipalityCode] = notCompleteType.MunicipalityCode;
                    ret[AddressField.Building] = notCompleteType.StreetBuildingIdentifier;
                    ret[AddressField.StreetCode] = notCompleteType.StreetCode;
                    if (notCompleteType.CountryIdentificationCode != null)
                    {
                        ret[AddressField.Alpha2CountryCode] = notCompleteType.CountryIdentificationCode.Value;
                    }
                    ret[AddressField.StreetName] = notCompleteType.StreetName;
                    ret[AddressField.StreetNameForAddressing] = notCompleteType.StreetNameForAddressingName;
                    ret[AddressField.Door] = notCompleteType.SuiteIdentifier;
                    ret[AddressField.Floor] = notCompleteType.FloorIdentifier;
                    ret[AddressField.MailDeliverSubLocationIdentifier] = notCompleteType.MailDeliverySublocationIdentifier;
                    ret[AddressField.DistrictSubDivisionIdentifier] = notCompleteType.DistrictSubdivisionIdentifier;
                    ret[AddressField.PostCode] = notCompleteType.PostCodeIdentifier;
                    ret[AddressField.PostDistrictName] = notCompleteType.DistrictName;
                    #endregion
                }
                #endregion
            }
            else if (oioAddress is ForeignAddressStructureType)
            {
                ForeignAddressStructureType oioForeignAddress = (ForeignAddressStructureType)oioAddress;
                if (oioForeignAddress.CountryIdentificationCode != null)
                {
                    ret[AddressField.Alpha2CountryCode] = oioForeignAddress.CountryIdentificationCode.Value;
                }
                ret[AddressField.LocationDescription] = oioForeignAddress.LocationDescriptionText;
                ret[AddressField.Line1] = oioForeignAddress.PostalAddressFirstLineText;
                ret[AddressField.Line2] = oioForeignAddress.PostalAddressSecondLineText;
                ret[AddressField.Line3] = oioForeignAddress.PostalAddressThirdLineText;
                ret[AddressField.Line4] = oioForeignAddress.PostalAddressFourthLineText;
                ret[AddressField.Line5] = oioForeignAddress.PostalAddressFifthLineText;
            }

            #endregion

            return ret;
        }

        /// <summary>
        /// Converts an empty string to null, to ensure no empty strings in a returned address
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string NormalizeString(string s)
        {
            s = string.Format("{0}", s).Trim();
            string testSring = s.Replace("0", "");
            if (string.IsNullOrEmpty(testSring))
            {
                return null;
            }
            else
            {
                return s;
            }
        }

        /// <summary>
        /// Gets the value of a specific address field
        /// </summary>
        /// <param name="addressField">Field to look for</param>
        /// <returns></returns>
        public string GetFieldValue(AddressField addressField)
        {
            return GetFieldValue(addressField, true);
        }

        /// <summary>
        /// Gets the value of a specific address field
        /// </summary>
        /// <param name="addressField">Field to look for</param>
        /// <param name="calculate">Wheteher to look for the field as a composition of other fields</param>
        /// <returns></returns>
        public string GetFieldValue(AddressField addressField, bool calculate)
        {
            if (this.ContainsKey(addressField))
            {
                return NormalizeString(this[addressField]);
            }
            else if (calculate)
            {
                var composite = (from ac in AddressCompositions
                                 where ac.Parent == addressField
                                 select ac).SingleOrDefault();

                // If it is a composite, get it fromDate the children
                if (composite != null)
                {
                    var childValues = from childField in composite.Children
                                      where this.ContainsKey(childField)
                                      select this[childField];
                    if (childValues.Count() > 0)
                    {
                        return NormalizeString(string.Join(FieldSeparator.ToString(), childValues.ToArray()));
                    }
                }
                /*else
                {
                 * // Cancelled to avoid potential errors
                    // Try to see if it can be obtained fromDate the parent
                    var parent = (fromDate ac in AddressCompositions
                                  where this.ContainsKey(ac.Parent)
                                  && this[ac.Parent] != null
                                  && ac.Children.Contains(addressField)
                                  && this[ac.Parent].Split(FieldSeparator).Length == ac.Children.Length
                                  select ac
                                  ).SingleOrDefault();
                    if (parent != null)
                    {
                        string[] parentComponents = this[parent.Parent].Split(FieldSeparator);
                        return NormalizeString(parentComponents[Array.IndexOf<AddressField>(parent.Children, addressField)]);
                    }
                }*/
            }

            return null;

        }

        private bool IsAddressComplete()
        {
            Func<AddressField, bool> valueExists = (field) => !string.IsNullOrEmpty(GetFieldValue(field, true));
            return
                (valueExists(AddressField.MunicipalityCode) || valueExists(AddressField.MunicipalityName))
                && (valueExists(AddressField.StreetCode) || valueExists(AddressField.StreetName) || valueExists(AddressField.StreetNameForAddressing))
                && (valueExists(AddressField.HouseNumber) || valueExists(AddressField.Building))
                && valueExists(AddressField.MailDeliverSubLocationIdentifier)
                && valueExists(AddressField.Floor)
                && valueExists(AddressField.Door)
                && valueExists(AddressField.DistrictSubDivisionIdentifier)
                && valueExists(AddressField.PostCode)
                && valueExists(AddressField.PostDistrictName)
                && (valueExists(AddressField.Alpha2CountryCode) || valueExists(AddressField.EnglishCountryName) || valueExists(AddressField.DanishCountryName))
                ;
        }
    }
}
