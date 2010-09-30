using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Engine;
using CPRBroker.Schemas;
using CPRBroker.DAL;
namespace CPRBroker.Providers.DPR
{
    public class DPRServerDataProvider : IDataProvider, IPersonBasicDataProvider, IPersonFullDataProvider, IPersonNameAndAddressDataProvider, IPersonRelationsDataProvider, IExternalDataProvider
    {
        #region IDataProvider Members

        bool IDataProvider.IsAlive()
        {
            return true;
        }

        Version IDataProvider.Version
        {
            get { return new Version(Versioning.Major, Versioning.Minor); }
        }

        public DataProvider DatabaseObject { get; set; }

        #endregion

        #region IPersonDataProvider Members
        public PersonBasicStructureType GetCitizenBasic(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel)
        {
            // Here I'll assume the following
            // customer = appToken
            // userId = userToken
            qualityLevel = null;

            #region Login and get data
            string returnedData = string.Empty;
            string token = DPRManager.Login(appToken, userToken);
            if (!string.IsNullOrEmpty(token))
            {
                string errorCode = ((DPRManager.Enums.EnumInfoAttribute)typeof(DPRManager.Enums.ErrorCode).GetField(DPRManager.Enums.ErrorCode.NoError.ToString()).GetCustomAttributes(true)[0]).EnumDesc;
                string message = DPRManager.GenerateRequest(DPRManager.Variables.TransactionCode.ToString(),
                    DPRManager.Variables.Comma.ToString(), appToken,
                    ((int)DPRManager.Enums.SubscriptionType.UpdatedAutomaticallyFromCPR).ToString(),
                    ((int)DPRManager.Enums.ReturnedDataType.ExtendedData).ToString(),
                    token, userToken, errorCode, cprNumber);
                string data = DPRManager.SendRequestAndGetResponse(DPRManager.Variables.Server, DPRManager.Variables.Port, message);
                returnedData = data.Substring(28);
            }
            if (string.IsNullOrEmpty(returnedData))
            {
                return null;
            }
            #endregion

            PersonBasicStructureType oioPerson = new PersonBasicStructureType();

            #region Assumption
            /*
              - AddressIdentifierCode  == AddressIdentifierCodeType.Item1
              - AddressStatusCode == AddressStatusCodeType.Item1
              - SuiteIdentifier == string.Empty
              - MailDeliverySublocationIdentifier == string.Empty
              - PostalAddressFirstLine  == Contact address
              - StreetNameForAddressingName == StreetName == Road address name
              - PostOfficeBoxIdentifier == civil status - office code
              - StreetCode == Road code
              - CountryIdentificationCode == Contact address municipality code
              - DistrictSubdivisionIdentifier == Post district
              - FloorIdentifier == Floor
              - PostCodeIdentifier == Postal code
              
             */
            #endregion

            #region Missing Fields
            /*
             Fields that are not exist
             - DeathDate
             - DeathDateUncertaintyIndicator
             - InformationProtectionIndicator
             - BirthDateUncertaintyIndicator
             - StatusStartDate
             - LocationDescription
             - PostalAddressSixthLine
             */
            #endregion

            #region Setting Basic Information
            oioPerson.PersonDeathDateStructure = null;
            //oioPerson.PersonDeathDateStructure.PersonDeathDateUncertaintyIndicator = false;
            oioPerson.PersonInformationProtectionIndicator = false;
            oioPerson.RegularCPRPerson.SimpleCPRPerson.PersonCivilRegistrationIdentifier = returnedData.Substring(1, 11);
            oioPerson.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure.PersonGivenName = returnedData.Substring(90, 50);
            oioPerson.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure.PersonMiddleName = returnedData.Substring(90, 50);
            oioPerson.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure.PersonSurnameName = returnedData.Substring(140, 40);

            oioPerson.RegularCPRPerson.PersonBirthDateStructure.BirthDate = Convert.ToDateTime(returnedData.Substring(337, 9));
            oioPerson.RegularCPRPerson.PersonBirthDateStructure.BirthDateUncertaintyIndicator = false;
            oioPerson.RegularCPRPerson.PersonCivilRegistrationStatusStructure.PersonCivilRegistrationStatusCode = (PersonCivilRegistrationStatusCodeType)Enum.Parse(typeof(PersonCivilRegistrationStatusCodeType), returnedData.Substring(30, 3));
            oioPerson.RegularCPRPerson.PersonCivilRegistrationStatusStructure = null;
            oioPerson.RegularCPRPerson.PersonGenderCode = (PersonGenderCodeType)Enum.Parse(typeof(PersonGenderCodeType), returnedData.Substring(346, 1));
            oioPerson.RegularCPRPerson.PersonInformationProtectionIndicator = false;
            oioPerson.RegularCPRPerson.PersonNameForAddressingName = returnedData.Substring(270, 34);
            oioPerson.MaritalStatusCode = (MaritalStatusCodeType)Enum.Parse(typeof(MaritalStatusCodeType), returnedData.Substring(1010, 1));
            oioPerson.PersonNationalityCode = returnedData.Substring(891, 20);

            #endregion

            #region Setting Address Information
            DanishAddressStructureType oioDanishAddress = new DanishAddressStructureType();
            ForeignAddressStructureType oioForeignAddress = new ForeignAddressStructureType();
            if (!string.IsNullOrEmpty(returnedData.Substring(1161, 20)))
            {
                #region Danish Address
                oioPerson.AddressIdentifierCode = AddressIdentifierCodeType.Item1;
                oioDanishAddress.CareOfName = returnedData.Substring(270, 34);
                oioDanishAddress.AddressStatusCode = AddressStatusCodeType.Item1;
                oioDanishAddress.MunicipalityName = returnedData.Substring(1161, 20);
                if (!string.IsNullOrEmpty(returnedData.Substring(25, 5))) // check post office box identifier
                {
                    #region AddressCompleteType
                    AddressCompleteType completeType = new AddressCompleteType();
                    completeType.AddressAccess.MunicipalityCode = returnedData.Substring(979, 5);
                    completeType.AddressAccess.StreetBuildingIdentifier = returnedData.Substring(1152, 4);
                    completeType.AddressAccess.StreetCode = returnedData.Substring(1137, 5);
                    completeType.AddressPostal.CountryIdentificationCode.Value = returnedData.Substring(1132, 5);
                    completeType.AddressPostal.StreetName = returnedData.Substring(1181, 20);
                    completeType.AddressPostal.StreetNameForAddressingName = returnedData.Substring(1181, 20);
                    completeType.AddressPostal.SuiteIdentifier = string.Empty;
                    completeType.AddressPostal.DistrictSubdivisionIdentifier = returnedData.Substring(828, 20);
                    completeType.AddressPostal.FloorIdentifier = returnedData.Substring(1146, 2);
                    completeType.AddressPostal.MailDeliverySublocationIdentifier = string.Empty;
                    completeType.AddressPostal.PostCodeIdentifier = returnedData.Substring(1156, 5);
                    completeType.AddressPostal.PostOfficeBoxIdentifier = returnedData.Substring(25, 5);
                    completeType.AddressPostal.StreetBuildingIdentifier = returnedData.Substring(1152, 4);
                    oioDanishAddress.Item = completeType;
                    #endregion
                }
                else
                {
                    // Here we should find a way to specify which of AddressCompleteGreenLang and AddressNotComplete will be used
                    #region AddressCompleteGreenlandType
                    AddressCompleteGreenlandType completeGreenLandType = new AddressCompleteGreenlandType();
                    completeGreenLandType.MunicipalityCode = returnedData.Substring(979, 5);
                    completeGreenLandType.StreetBuildingIdentifier = returnedData.Substring(1152, 4);
                    completeGreenLandType.StreetCode = returnedData.Substring(1137, 5);
                    completeGreenLandType.CountryIdentificationCode.Value = returnedData.Substring(1132, 5);
                    completeGreenLandType.StreetName = returnedData.Substring(1181, 20);
                    completeGreenLandType.StreetNameForAddressingName = returnedData.Substring(1181, 20);
                    completeGreenLandType.SuiteIdentifier = string.Empty;
                    completeGreenLandType.DistrictSubdivisionIdentifier = returnedData.Substring(828, 20);
                    completeGreenLandType.FloorIdentifier = returnedData.Substring(1146, 2);
                    completeGreenLandType.MailDeliverySublocationIdentifier = string.Empty;
                    completeGreenLandType.PostCodeIdentifier = returnedData.Substring(1156, 5);
                    // is there a need to put a greenland building identifier aside from the street building identifier in the AddressTable
                    completeGreenLandType.GreenlandBuildingIdentifier = returnedData.Substring(1152, 4);
                    oioDanishAddress.Item = completeGreenLandType;
                    #endregion

                    #region AddressNotCompleteType
                    //AddressNotCompleteType notCompleteType = new AddressNotCompleteType();
                    //notCompleteType.MunicipalityCode = returnedData.Substring(979, 5);
                    //notCompleteType.StreetBuildingIdentifier = returnedData.Substring(1152, 4);
                    //notCompleteType.StreetCode = returnedData.Substring(1137, 5);
                    //notCompleteType.CountryIdentificationCode.Value = returnedData.Substring(1132, 5);
                    //notCompleteType.StreetName = returnedData.Substring(1181, 20);
                    //notCompleteType.StreetNameForAddressingName = returnedData.Substring(1181, 20);
                    //notCompleteType.SuiteIdentifier = string.Empty;
                    //notCompleteType.DistrictSubdivisionIdentifier = returnedData.Substring(828, 20);
                    //notCompleteType.FloorIdentifier = returnedData.Substring(1146, 2);
                    //notCompleteType.MailDeliverySublocationIdentifier = string.Empty;
                    //notCompleteType.PostCodeIdentifier = returnedData.Substring(1156, 5);
                    //// is there a need to put a greenland building identifier aside from the street building identifier in the AddressTable
                    //notCompleteType.GreenlandBuildingIdentifier = returnedData.Substring(1152, 4);
                    //oioDanishAddress.Item = notCompleteType;
                    #endregion
                }



                oioDanishAddress.CompletePostalLabel.AddresseeName = returnedData.Substring(270, 34);
                oioDanishAddress.CompletePostalLabel.PostalAddressFirstLineText = returnedData.Substring(1828, 34);
                oioDanishAddress.CompletePostalLabel.PostalAddressSecondLineText = returnedData.Substring(1862, 34);
                oioDanishAddress.CompletePostalLabel.PostalAddressThirdLineText = returnedData.Substring(1896, 34);
                oioDanishAddress.CompletePostalLabel.PostalAddressFourthLineText = returnedData.Substring(1930, 34);
                oioDanishAddress.CompletePostalLabel.PostalAddressFifthLineText = returnedData.Substring(1964, 34);
                oioDanishAddress.CompletePostalLabel.PostalAddressSixthLineText = string.Empty;
                #endregion

                oioPerson.Item = oioDanishAddress;
            }
            else
            {
                #region Foreign Address
                oioForeignAddress.CountryIdentificationCode.Value = returnedData.Substring(1132, 5);
                oioForeignAddress.LocationDescriptionText = string.Empty;
                oioForeignAddress.PostalAddressFirstLineText = returnedData.Substring(1627, 34);
                oioForeignAddress.PostalAddressSecondLineText = returnedData.Substring(1661, 34);
                oioForeignAddress.PostalAddressThirdLineText = returnedData.Substring(1695, 34);
                oioForeignAddress.PostalAddressFourthLineText = returnedData.Substring(1729, 34);
                oioForeignAddress.PostalAddressFifthLineText = returnedData.Substring(1763, 34);
                #endregion

                oioPerson.Item = oioForeignAddress;
            }
            #endregion

            qualityLevel = QualityLevel.DataProvider;
            return oioPerson;
        }

        public PersonNameAndAddressStructureType GetCitizenNameAndAddress(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel)
        {
            qualityLevel = null;
            #region Login and get data
            string returnedData = string.Empty;
            string token = DPRManager.Login(appToken, userToken);
            if (!string.IsNullOrEmpty(token))
            {
                string errorCode = ((DPRManager.Enums.EnumInfoAttribute)typeof(DPRManager.Enums.ErrorCode).GetField(DPRManager.Enums.ErrorCode.NoError.ToString()).GetCustomAttributes(true)[0]).EnumDesc;
                string message = DPRManager.GenerateRequest(DPRManager.Variables.TransactionCode.ToString(),
                    DPRManager.Variables.Comma.ToString(), appToken,
                    ((int)DPRManager.Enums.SubscriptionType.UpdatedAutomaticallyFromCPR).ToString(),
                    ((int)DPRManager.Enums.ReturnedDataType.ExtendedData).ToString(),
                    token, userToken, errorCode, cprNumber);
                string data = DPRManager.SendRequestAndGetResponse(DPRManager.Variables.Server, DPRManager.Variables.Port, message);
                returnedData = data.Substring(28);
            }
            if (string.IsNullOrEmpty(returnedData))
            {
                return null;
            }
            #endregion

            PersonNameAndAddressStructureType oioPerson = new PersonNameAndAddressStructureType();

            #region Setting Basic Information
            oioPerson.PersonInformationProtectionIndicator = false;
            oioPerson.SimpleCPRPerson.PersonCivilRegistrationIdentifier = returnedData.Substring(1, 11);
            oioPerson.SimpleCPRPerson.PersonNameStructure.PersonGivenName = returnedData.Substring(90, 50);
            oioPerson.SimpleCPRPerson.PersonNameStructure.PersonMiddleName = returnedData.Substring(90, 50);
            oioPerson.SimpleCPRPerson.PersonNameStructure.PersonSurnameName = returnedData.Substring(140, 40);
            #endregion

            #region Setting Address Information
            DanishAddressStructureType oioDanishAddress = new DanishAddressStructureType();
            ForeignAddressStructureType oioForeignAddress = new ForeignAddressStructureType();
            if (!string.IsNullOrEmpty(returnedData.Substring(1161, 20)))
            {
                #region Danish Address
                oioPerson.AddressIdentifierCode = AddressIdentifierCodeType.Item1;
                oioDanishAddress.CareOfName = returnedData.Substring(270, 34);
                oioDanishAddress.AddressStatusCode = AddressStatusCodeType.Item1;
                oioDanishAddress.MunicipalityName = returnedData.Substring(1161, 20);
                if (!string.IsNullOrEmpty(returnedData.Substring(25, 5))) // check post office box identifier
                {
                    #region AddressCompleteType
                    AddressCompleteType completeType = new AddressCompleteType();
                    completeType.AddressAccess.MunicipalityCode = returnedData.Substring(979, 5);
                    completeType.AddressAccess.StreetBuildingIdentifier = returnedData.Substring(1152, 4);
                    completeType.AddressAccess.StreetCode = returnedData.Substring(1137, 5);
                    completeType.AddressPostal.CountryIdentificationCode.Value = returnedData.Substring(1132, 5);
                    completeType.AddressPostal.StreetName = returnedData.Substring(1181, 20);
                    completeType.AddressPostal.StreetNameForAddressingName = returnedData.Substring(1181, 20);
                    completeType.AddressPostal.SuiteIdentifier = string.Empty;
                    completeType.AddressPostal.DistrictSubdivisionIdentifier = returnedData.Substring(828, 20);
                    completeType.AddressPostal.FloorIdentifier = returnedData.Substring(1146, 2);
                    completeType.AddressPostal.MailDeliverySublocationIdentifier = string.Empty;
                    completeType.AddressPostal.PostCodeIdentifier = returnedData.Substring(1156, 5);
                    completeType.AddressPostal.PostOfficeBoxIdentifier = returnedData.Substring(25, 5);
                    completeType.AddressPostal.StreetBuildingIdentifier = returnedData.Substring(1152, 4);
                    oioDanishAddress.Item = completeType;
                    #endregion
                }
                else
                {
                    // Here we should find a way to specify which of AddressCompleteGreenLang and AddressNotComplete will be used
                    #region AddressCompleteGreenlandType
                    AddressCompleteGreenlandType completeGreenLandType = new AddressCompleteGreenlandType();
                    completeGreenLandType.MunicipalityCode = returnedData.Substring(979, 5);
                    completeGreenLandType.StreetBuildingIdentifier = returnedData.Substring(1152, 4);
                    completeGreenLandType.StreetCode = returnedData.Substring(1137, 5);
                    completeGreenLandType.CountryIdentificationCode.Value = returnedData.Substring(1132, 5);
                    completeGreenLandType.StreetName = returnedData.Substring(1181, 20);
                    completeGreenLandType.StreetNameForAddressingName = returnedData.Substring(1181, 20);
                    completeGreenLandType.SuiteIdentifier = string.Empty;
                    completeGreenLandType.DistrictSubdivisionIdentifier = returnedData.Substring(828, 20);
                    completeGreenLandType.FloorIdentifier = returnedData.Substring(1146, 2);
                    completeGreenLandType.MailDeliverySublocationIdentifier = string.Empty;
                    completeGreenLandType.PostCodeIdentifier = returnedData.Substring(1156, 5);
                    // is there a need to put a greenland building identifier aside from the street building identifier in the AddressTable
                    completeGreenLandType.GreenlandBuildingIdentifier = returnedData.Substring(1152, 4);
                    oioDanishAddress.Item = completeGreenLandType;
                    #endregion

                    #region AddressNotCompleteType
                    //AddressNotCompleteType notCompleteType = new AddressNotCompleteType();
                    //notCompleteType.MunicipalityCode = returnedData.Substring(979, 5);
                    //notCompleteType.StreetBuildingIdentifier = returnedData.Substring(1152, 4);
                    //notCompleteType.StreetCode = returnedData.Substring(1137, 5);
                    //notCompleteType.CountryIdentificationCode.Value = returnedData.Substring(1132, 5);
                    //notCompleteType.StreetName = returnedData.Substring(1181, 20);
                    //notCompleteType.StreetNameForAddressingName = returnedData.Substring(1181, 20);
                    //notCompleteType.SuiteIdentifier = string.Empty;
                    //notCompleteType.DistrictSubdivisionIdentifier = returnedData.Substring(828, 20);
                    //notCompleteType.FloorIdentifier = returnedData.Substring(1146, 2);
                    //notCompleteType.MailDeliverySublocationIdentifier = string.Empty;
                    //notCompleteType.PostCodeIdentifier = returnedData.Substring(1156, 5);
                    //// is there a need to put a greenland building identifier aside from the street building identifier in the AddressTable
                    //notCompleteType.GreenlandBuildingIdentifier = returnedData.Substring(1152, 4);
                    //oioDanishAddress.Item = notCompleteType;
                    #endregion
                }



                oioDanishAddress.CompletePostalLabel.AddresseeName = returnedData.Substring(270, 34);
                oioDanishAddress.CompletePostalLabel.PostalAddressFirstLineText = returnedData.Substring(1828, 34);
                oioDanishAddress.CompletePostalLabel.PostalAddressSecondLineText = returnedData.Substring(1862, 34);
                oioDanishAddress.CompletePostalLabel.PostalAddressThirdLineText = returnedData.Substring(1896, 34);
                oioDanishAddress.CompletePostalLabel.PostalAddressFourthLineText = returnedData.Substring(1930, 34);
                oioDanishAddress.CompletePostalLabel.PostalAddressFifthLineText = returnedData.Substring(1964, 34);
                oioDanishAddress.CompletePostalLabel.PostalAddressSixthLineText = string.Empty;
                #endregion

                oioPerson.Item = oioDanishAddress;
            }
            else
            {
                #region Foreign Address
                oioForeignAddress.CountryIdentificationCode.Value = returnedData.Substring(1132, 5);
                oioForeignAddress.LocationDescriptionText = string.Empty;
                oioForeignAddress.PostalAddressFirstLineText = returnedData.Substring(1627, 34);
                oioForeignAddress.PostalAddressSecondLineText = returnedData.Substring(1661, 34);
                oioForeignAddress.PostalAddressThirdLineText = returnedData.Substring(1695, 34);
                oioForeignAddress.PostalAddressFourthLineText = returnedData.Substring(1729, 34);
                oioForeignAddress.PostalAddressFifthLineText = returnedData.Substring(1763, 34);
                #endregion

                oioPerson.Item = oioForeignAddress;
            }
            #endregion

            qualityLevel = QualityLevel.DataProvider;
            return oioPerson;
        }

        public PersonFullStructureType GetCitizenFull(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel)
        {
            // Here I'll assume the following
            // customer = appToken
            // userId = userToken
            qualityLevel = null;

            #region Login and get data
            string returnedData = string.Empty;
            //string token = DPRManager.Login(appToken, userToken);
            //if (!string.IsNullOrEmpty(token))
            {
                string errorCode = ((DPRManager.Enums.EnumInfoAttribute)typeof(DPRManager.Enums.ErrorCode).GetField(DPRManager.Enums.ErrorCode.NoError.ToString()).GetCustomAttributes(true)[0]).EnumDesc;
                string message;
                message = DPRManager.GenerateRequest(
                    DPRManager.Variables.TransactionCode,
                    DPRManager.Variables.Comma,
                    DPRManager.Variables.Customer,
                    ((int)DPRManager.Enums.SubscriptionType.NotUpdatedAutomaticallyFromCPR).ToString(),
                    ((int)DPRManager.Enums.ReturnedDataType.ExtendedData).ToString(),
                    "00000000", // token
                    "00000000", // user id
                    errorCode,
                    cprNumber
                    );
                string data = DPRManager.SendRequestAndGetResponse(DPRManager.Variables.Server, DPRManager.Variables.Port, message);
                returnedData = data.Substring(28);
            }
            if (string.IsNullOrEmpty(returnedData))
            {
                return null;
            }
            #endregion

            PersonFullStructureType oioPerson = new PersonFullStructureType();

            #region Assumption
            /*
              - AddressIdentifierCode  == AddressIdentifierCodeType.Item1
              - AddressStatusCode == AddressStatusCodeType.Item1
              - SuiteIdentifier == string.Empty
              - MailDeliverySublocationIdentifier == string.Empty
              - PostalAddressFirstLine  == Contact address
              - StreetNameForAddressingName == StreetName == Road address name
              - PostOfficeBoxIdentifier == civil status - office code
              - StreetCode == Road code
              - CountryIdentificationCode == Contact address municipality code
              - DistrictSubdivisionIdentifier == Post district
              - FloorIdentifier == Floor
              - PostCodeIdentifier == Postal code
              
             */
            #endregion

            #region Missing Fields
            /*
             Fields that are not exist
             - DeathDate
             - DeathDateUncertaintyIndicator
             - InformationProtectionIndicator
             - BirthDateUncertaintyIndicator
             - StatusStartDate
             - LocationDescription
             - PostalAddressSixthLine
             */
            #endregion

            #region Setting Basic Information
            oioPerson.PersonDeathDateStructure = null;
            //oioPerson.PersonDeathDateStructure.PersonDeathDateUncertaintyIndicator = false;
            oioPerson.PersonInformationProtectionIndicator = false;
            oioPerson.RegularCPRPerson.SimpleCPRPerson.PersonCivilRegistrationIdentifier = returnedData.Substring(1, 11);
            oioPerson.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure.PersonGivenName = returnedData.Substring(90, 50);
            oioPerson.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure.PersonMiddleName = returnedData.Substring(90, 50);
            oioPerson.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure.PersonSurnameName = returnedData.Substring(140, 40);

            oioPerson.RegularCPRPerson.PersonBirthDateStructure.BirthDate = Convert.ToDateTime(returnedData.Substring(337, 9));
            oioPerson.RegularCPRPerson.PersonBirthDateStructure.BirthDateUncertaintyIndicator = false;
            oioPerson.RegularCPRPerson.PersonCivilRegistrationStatusStructure.PersonCivilRegistrationStatusCode = (PersonCivilRegistrationStatusCodeType)Enum.Parse(typeof(PersonCivilRegistrationStatusCodeType), returnedData.Substring(30, 3));
            oioPerson.RegularCPRPerson.PersonCivilRegistrationStatusStructure = null;
            oioPerson.RegularCPRPerson.PersonGenderCode = (PersonGenderCodeType)Enum.Parse(typeof(PersonGenderCodeType), returnedData.Substring(346, 1));
            oioPerson.RegularCPRPerson.PersonInformationProtectionIndicator = false;
            oioPerson.RegularCPRPerson.PersonNameForAddressingName = returnedData.Substring(270, 34);
            oioPerson.MaritalStatusCode = (MaritalStatusCodeType)Enum.Parse(typeof(MaritalStatusCodeType), returnedData.Substring(1010, 1));
            oioPerson.PersonNationalityCode = returnedData.Substring(891, 20);

            #endregion

            #region Setting Address Information
            DanishAddressStructureType oioDanishAddress = new DanishAddressStructureType();
            ForeignAddressStructureType oioForeignAddress = new ForeignAddressStructureType();
            if (!string.IsNullOrEmpty(returnedData.Substring(1161, 20)))
            {
                #region Danish Address
                oioPerson.AddressIdentifierCode = AddressIdentifierCodeType.Item1;
                oioDanishAddress.CareOfName = returnedData.Substring(270, 34);
                oioDanishAddress.AddressStatusCode = AddressStatusCodeType.Item1;
                oioDanishAddress.MunicipalityName = returnedData.Substring(1161, 20);
                if (!string.IsNullOrEmpty(returnedData.Substring(25, 5))) // check post office box identifier
                {
                    #region AddressCompleteType
                    AddressCompleteType completeType = new AddressCompleteType();
                    completeType.AddressAccess.MunicipalityCode = returnedData.Substring(979, 5);
                    completeType.AddressAccess.StreetBuildingIdentifier = returnedData.Substring(1152, 4);
                    completeType.AddressAccess.StreetCode = returnedData.Substring(1137, 5);
                    completeType.AddressPostal.CountryIdentificationCode.Value = returnedData.Substring(1132, 5);
                    completeType.AddressPostal.StreetName = returnedData.Substring(1181, 20);
                    completeType.AddressPostal.StreetNameForAddressingName = returnedData.Substring(1181, 20);
                    completeType.AddressPostal.SuiteIdentifier = string.Empty;
                    completeType.AddressPostal.DistrictSubdivisionIdentifier = returnedData.Substring(828, 20);
                    completeType.AddressPostal.FloorIdentifier = returnedData.Substring(1146, 2);
                    completeType.AddressPostal.MailDeliverySublocationIdentifier = string.Empty;
                    completeType.AddressPostal.PostCodeIdentifier = returnedData.Substring(1156, 5);
                    completeType.AddressPostal.PostOfficeBoxIdentifier = returnedData.Substring(25, 5);
                    completeType.AddressPostal.StreetBuildingIdentifier = returnedData.Substring(1152, 4);
                    oioDanishAddress.Item = completeType;
                    #endregion
                }
                else
                {
                    // Here we should find a way to specify which of AddressCompleteGreenLang and AddressNotComplete will be used
                    #region AddressCompleteGreenlandType
                    AddressCompleteGreenlandType completeGreenLandType = new AddressCompleteGreenlandType();
                    completeGreenLandType.MunicipalityCode = returnedData.Substring(979, 5);
                    completeGreenLandType.StreetBuildingIdentifier = returnedData.Substring(1152, 4);
                    completeGreenLandType.StreetCode = returnedData.Substring(1137, 5);
                    completeGreenLandType.CountryIdentificationCode.Value = returnedData.Substring(1132, 5);
                    completeGreenLandType.StreetName = returnedData.Substring(1181, 20);
                    completeGreenLandType.StreetNameForAddressingName = returnedData.Substring(1181, 20);
                    completeGreenLandType.SuiteIdentifier = string.Empty;
                    completeGreenLandType.DistrictSubdivisionIdentifier = returnedData.Substring(828, 20);
                    completeGreenLandType.FloorIdentifier = returnedData.Substring(1146, 2);
                    completeGreenLandType.MailDeliverySublocationIdentifier = string.Empty;
                    completeGreenLandType.PostCodeIdentifier = returnedData.Substring(1156, 5);
                    // is there a need to put a greenland building identifier aside from the street building identifier in the AddressTable
                    completeGreenLandType.GreenlandBuildingIdentifier = returnedData.Substring(1152, 4);
                    oioDanishAddress.Item = completeGreenLandType;
                    #endregion

                    #region AddressNotCompleteType
                    //AddressNotCompleteType notCompleteType = new AddressNotCompleteType();
                    //notCompleteType.MunicipalityCode = returnedData.Substring(979, 5);
                    //notCompleteType.StreetBuildingIdentifier = returnedData.Substring(1152, 4);
                    //notCompleteType.StreetCode = returnedData.Substring(1137, 5);
                    //notCompleteType.CountryIdentificationCode.Value = returnedData.Substring(1132, 5);
                    //notCompleteType.StreetName = returnedData.Substring(1181, 20);
                    //notCompleteType.StreetNameForAddressingName = returnedData.Substring(1181, 20);
                    //notCompleteType.SuiteIdentifier = string.Empty;
                    //notCompleteType.DistrictSubdivisionIdentifier = returnedData.Substring(828, 20);
                    //notCompleteType.FloorIdentifier = returnedData.Substring(1146, 2);
                    //notCompleteType.MailDeliverySublocationIdentifier = string.Empty;
                    //notCompleteType.PostCodeIdentifier = returnedData.Substring(1156, 5);
                    //// is there a need to put a greenland building identifier aside from the street building identifier in the AddressTable
                    //notCompleteType.GreenlandBuildingIdentifier = returnedData.Substring(1152, 4);
                    //oioDanishAddress.Item = notCompleteType;
                    #endregion
                }



                oioDanishAddress.CompletePostalLabel.AddresseeName = returnedData.Substring(270, 34);
                oioDanishAddress.CompletePostalLabel.PostalAddressFirstLineText = returnedData.Substring(1828, 34);
                oioDanishAddress.CompletePostalLabel.PostalAddressSecondLineText = returnedData.Substring(1862, 34);
                oioDanishAddress.CompletePostalLabel.PostalAddressThirdLineText = returnedData.Substring(1896, 34);
                oioDanishAddress.CompletePostalLabel.PostalAddressFourthLineText = returnedData.Substring(1930, 34);
                oioDanishAddress.CompletePostalLabel.PostalAddressFifthLineText = returnedData.Substring(1964, 34);
                oioDanishAddress.CompletePostalLabel.PostalAddressSixthLineText = string.Empty;
                #endregion

                oioPerson.Item = oioDanishAddress;
            }
            else
            {
                #region Foreign Address
                oioForeignAddress.CountryIdentificationCode.Value = returnedData.Substring(1132, 5);
                oioForeignAddress.LocationDescriptionText = string.Empty;
                oioForeignAddress.PostalAddressFirstLineText = returnedData.Substring(1627, 34);
                oioForeignAddress.PostalAddressSecondLineText = returnedData.Substring(1661, 34);
                oioForeignAddress.PostalAddressThirdLineText = returnedData.Substring(1695, 34);
                oioForeignAddress.PostalAddressFourthLineText = returnedData.Substring(1729, 34);
                oioForeignAddress.PostalAddressFifthLineText = returnedData.Substring(1763, 34);
                #endregion

                oioPerson.Item = oioForeignAddress;
            }
            #endregion

            #region Setting Other Information
            oioPerson.SpouseName = returnedData.Substring(1085, 34);
            oioPerson.NumberOfChildren = Convert.ToInt32(returnedData.Substring(2544, 2));
            #endregion

            qualityLevel = QualityLevel.DataProvider;
            return oioPerson;
        }

        #endregion

        #region IPersonRelationsDataProvider Members

        public PersonRelationsType GetCitizenRelations(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel)
        {
            qualityLevel = null;
            #region Login and get data
            string returnedData = string.Empty;
            string token = DPRManager.Login(appToken, userToken);
            if (!string.IsNullOrEmpty(token))
            {
                string errorCode = ((DPRManager.Enums.EnumInfoAttribute)typeof(DPRManager.Enums.ErrorCode).GetField(DPRManager.Enums.ErrorCode.NoError.ToString()).GetCustomAttributes(true)[0]).EnumDesc;
                string message = DPRManager.GenerateRequest(DPRManager.Variables.TransactionCode.ToString(),
                    DPRManager.Variables.Comma.ToString(), appToken,
                    ((int)DPRManager.Enums.SubscriptionType.UpdatedAutomaticallyFromCPR).ToString(),
                    ((int)DPRManager.Enums.ReturnedDataType.ExtendedData).ToString(),
                    token, userToken, errorCode, cprNumber);
                string data = DPRManager.SendRequestAndGetResponse(DPRManager.Variables.Server, DPRManager.Variables.Port, message);
                returnedData = data.Substring(28);
            }
            if (string.IsNullOrEmpty(returnedData))
            {
                return null;
            }
            #endregion

            PersonRelationsType oioPersonRelationStructure = new PersonRelationsType();
            if (!string.IsNullOrEmpty(returnedData.Substring(548, 11))) // Mother pnr
            {
                #region Mother
                ParentRelationshipType oioRelation = new ParentRelationshipType();
                oioRelation.SimpleCPRPerson.PersonCivilRegistrationIdentifier = returnedData.Substring(548, 11);
                oioRelation.SimpleCPRPerson.PersonNameStructure.PersonGivenName = returnedData.Substring(612, 34);
                oioRelation.SimpleCPRPerson.PersonNameStructure.PersonMiddleName = returnedData.Substring(612, 34);
                oioRelation.SimpleCPRPerson.PersonNameStructure.PersonSurnameName = returnedData.Substring(612, 34);

                //oioRelation.RelationStartDate = null;
                //oioRelation.RelationEndDate = null;
                //oioRelation.Authority = null;
                #endregion
                oioPersonRelationStructure.Parents.Add(oioRelation);
            }
            if (!string.IsNullOrEmpty(returnedData.Substring(571, 11))) // father pnr
            {
                #region Father
                ParentRelationshipType oioRelation = new ParentRelationshipType();
                oioRelation.SimpleCPRPerson.PersonCivilRegistrationIdentifier = returnedData.Substring(571, 11);
                oioRelation.SimpleCPRPerson.PersonNameStructure.PersonGivenName = returnedData.Substring(656, 34);
                oioRelation.SimpleCPRPerson.PersonNameStructure.PersonMiddleName = returnedData.Substring(656, 34);
                oioRelation.SimpleCPRPerson.PersonNameStructure.PersonSurnameName = returnedData.Substring(646, 34);
                //oioRelation.RelationStartDate = Convert.ToDateTime(returnedData.Substring(594, 13));
                //oioRelation.RelationEndDate = null;
                //oioRelation.Authority = returnedData.Substring(607, 5);

                #endregion
                oioPersonRelationStructure.Parents.Add(oioRelation);
            }
            if (!string.IsNullOrEmpty(returnedData.Substring(1016, 11))) // spouse personal
            {
                #region Spouse
                MaritalRelationshipType oioRelation = new MaritalRelationshipType();
                oioRelation.SimpleCPRPerson.PersonCivilRegistrationIdentifier = returnedData.Substring(1016, 11);
                oioRelation.SimpleCPRPerson.PersonNameStructure.PersonGivenName = returnedData.Substring(1085, 34);
                oioRelation.SimpleCPRPerson.PersonNameStructure.PersonMiddleName = returnedData.Substring(1085, 34);
                oioRelation.SimpleCPRPerson.PersonNameStructure.PersonSurnameName = returnedData.Substring(1085, 34);
                //oioRelation.RelationStartDate = null;
                oioRelation.RelationEndDate = null;
                //oioRelation.Authority = null;

                #endregion
                oioPersonRelationStructure.Spouses.Add( oioRelation);
            }
            qualityLevel = QualityLevel.DataProvider;
            return oioPersonRelationStructure;
        }

        #endregion
    }
}
