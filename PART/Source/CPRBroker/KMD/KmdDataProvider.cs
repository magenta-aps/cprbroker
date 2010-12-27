using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;
using CprBroker.Providers.KMD.WS_AN08002;
using CprBroker.Schemas;
using CprBroker.DAL;

namespace CprBroker.Providers.KMD
{
    public partial class KmdDataProvider : IDataProvider, IExternalDataProvider, IPersonNameAndAddressDataProvider, IPersonChildrenDataProvider, IPersonRelationsDataProvider, IPersonBasicDataProvider, IPersonFullDataProvider
    {
        public enum ServiceTypes
        {
            AN08002,
            AN08010,
            AS78206,
            AS78207,
            AN08300,
            AN08100,
        }
        #region PrivateMethods
        /// <summary>
        /// Sets the KMD web service url to the correct value based on the database object and the service name
        /// </summary>
        /// <param name="service">Web service proxy object</param>
        /// <param name="serviceType">Type of service</param>
        private void SetServiceUrl(System.Web.Services.Protocols.SoapHttpClientProtocol service, ServiceTypes serviceType)
        {
            string query = string.Format("?zservice={0}", serviceType);
            string url = DatabaseObject.Address.Split(new char[] { '?' }, StringSplitOptions.RemoveEmptyEntries)[0];
            url += query;
            service.Url = url;
        }

        /// <summary>
        /// Filters the list of a person's relations based on the requested relation type
        /// </summary>
        /// <typeparam name="TRelation">Type of OIO relation object</typeparam>
        /// <param name="persons">Related persons returned from KMD service</param>
        /// <param name="typeFilters">The type codes that will be used to filter the relations</param>
        /// <returns>Filtered list of OIO relations</returns>
        private TRelation[] GetPersonRelations<TRelation>(WS_AN08010.ReplyPerson[] persons, params string[] typeFilters) where TRelation : BaseRelationshipType, new()
        {
            if (persons == null)
            {
                persons = new WS_AN08010.ReplyPerson[0];
            }

            var filteredPersons = from person in persons
                                  where Array.IndexOf<string>(typeFilters, person.Type) != -1
                                  && person.IsUnknown == false
                                  select person;
            List<TRelation> ret = new List<TRelation>();
            foreach (var person in filteredPersons)
            {
                SimpleCPRPersonType oioPerson = person.ToSimpleCprPerson();
                if (oioPerson.PersonNameStructure.IsEmpty)
                {
                    var an08002 = CallAN08002(oioPerson.PersonCivilRegistrationIdentifier);
                    oioPerson = an08002.ToSimpleCprPerson();
                }
                ret.Add(new TRelation() { SimpleCPRPerson = oioPerson });
            }

            return ret.ToArray();
        }

        /// <summary>
        /// Searches for the return code in a list of error codes, throws an Exception if a match is found
        /// </summary>
        /// <param name="returnCode">Code returned from web service</param>
        /// <param name="returnText">Text returned from the web service, used as the Exception's message if thrown</param>
        private void ValidateReturnCode(string returnCode, string returnText)
        {
            string[] errorCodes = new string[] 
            {
                //"00",//	Everything ok
                "07",//	Person is unknown in the municipality
                "08",//	Person is unknown in the region
                "10",//	The person is inactive -- moved from region
                //"15",//	Person number is invalid - former double issue
                //"16",//	Person number is invalid - the person is nynummereret
                //"17",//	The person is inactive - disappeared
                //"18",//	The person is inactive - emigrate
                //"19",//	The person is inactive - dead
                "22",//	The person is unknown in CPR
                "50",//	Bind error - contact DBA
                "51",//	Bind error - contact DBA
                "52",//	Bind error - contact DBA
                "53",//	Problems with connection to CPR - try again later
                "54",//	There are currently unable to carry on CPR
                "55",//	There is no such CICS through to DC. '
                "70",//municipal code / personal identification number is not numeric
                "78",// Error in personal / replacement personal
            };
            if (Array.IndexOf<string>(errorCodes, returnCode) != -1)
            {
                throw new Exception(returnText);
            }
        }
        #endregion

        #region Conversion methods

        private PersonCivilRegistrationStatusStructureType ToCivilRegistrationStatusStructureType(string kmdStatus, string cprStatus, string dStatus)
        {
            int iKmd = int.Parse(kmdStatus);
            int iCpr = int.Parse(cprStatus);
            PersonCivilRegistrationStatusStructureType ret = null;
            PersonCivilRegistrationStatusCodeType? code = null;
            if (iKmd == 1)// >10
            {
                int status = iCpr * 10;
                code = Schemas.Util.Enums.ToCivilRegistrationStatus(status);
            }
            else
            {
                // TODO: differentiate betwee 01, 03, 05 & 07 because cprStatus is always 0 here
                code = PersonCivilRegistrationStatusCodeType.Item01;
            }
            if (code.HasValue)
            {
                ret = new PersonCivilRegistrationStatusStructureType()
                {
                    PersonCivilRegistrationStatusCode = code.Value,
                    PersonCivilRegistrationStatusStartDate = ToDateTime(dStatus).Value
                };
            }
            return ret;
        }

        private DateTime? ToDateTime(string str)
        {
            DateTime ret;
            if (DateTime.TryParseExact(str, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out ret))
            {
                return ret;
            }
            return null;
        }

        private PersonGenderCodeType ToGenderCode(string cprNumber)
        {
            int cprNum = int.Parse(cprNumber[cprNumber.Length - 1].ToString());
            if (cprNum % 2 == 0)
            {
                return PersonGenderCodeType.female;
            }
            else
            {
                return PersonGenderCodeType.male;
            }
        }

        private PersonDeathDateStructureType ToDeathDate(PersonCivilRegistrationStatusStructureType status)
        {
            if (status != null
                && status.PersonCivilRegistrationStatusCode == PersonCivilRegistrationStatusCodeType.Item90 // dead
                )
            {
                return new PersonDeathDateStructureType()
                {
                    PersonDeathDate = status.PersonCivilRegistrationStatusStartDate,
                    PersonDeathDateUncertaintyIndicator = false,
                };
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region IDataProvider Members

        public bool IsAlive()
        {
            System.Net.WebClient client = new System.Net.WebClient();
            try
            {
                System.Uri uri = new Uri(DatabaseObject.Address);
                uri = new Uri(uri, "wsdl/AN08002.wsdl");
                client.DownloadData(uri);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                client.Dispose();
            }
        }

        public Version Version
        {
            get
            {
                return new Version(Versioning.Major, Versioning.Minor);
            }
        }

        #endregion

        #region IExternalDataProvider Members

        public CprBroker.DAL.DataProvider DatabaseObject { get; set; }

        #endregion

        #region IPersonNameAndAddressDataProvider Members

        public PersonNameAndAddressStructureType GetCitizenNameAndAddress(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel)
        {
            EnglishAN08002Response englishResponse = CallAN08002(cprNumber);
            PersonNameAndAddressStructureType personNameAndAddress = ToPersonNameAndAddress(englishResponse);
            qualityLevel = QualityLevel.Cpr;
            return personNameAndAddress;
        }

        #endregion

        #region IPersonChildrenDataProvider Members

        public SimpleCPRPersonType[] GetCitizenChildren(string userToken, string appToken, string cprNumber, bool includeCustodies, out QualityLevel? qualityLevel)
        {
            var resp = CallAN08010(cprNumber);
            var children = GetPersonRelations<ChildRelationshipType>(resp.OutputArrayRecord, RelationTypes.Baby, RelationTypes.ChildOver18);
            var persons = from child in children
                          select child.SimpleCPRPerson;

            qualityLevel = QualityLevel.Cpr;
            return persons.OrderBy((p) => DAL.Person.PersonNumberToDate(p.PersonCivilRegistrationIdentifier)).ToArray();
        }

        #endregion

        #region IPersonRelationsDataProvider Members

        public PersonRelationsType GetCitizenRelations(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel)
        {
            PersonRelationsType ret = null;
            var personResponse = CallAN08002(cprNumber);
            var relationsResponse = CallAN08010(cprNumber);


            ret = new PersonRelationsType();
            ret.SimpleCPRPerson = personResponse.ToSimpleCprPerson();
            ret.Parents.AddRange(GetPersonRelations<ParentRelationshipType>(relationsResponse.OutputArrayRecord, RelationTypes.Parents).OrderBy((p) => DAL.Person.PersonNumberToDate(p.SimpleCPRPerson.PersonCivilRegistrationIdentifier)));
            ret.Children.AddRange(GetPersonRelations<ChildRelationshipType>(relationsResponse.OutputArrayRecord, RelationTypes.Baby, RelationTypes.ChildOver18).OrderBy((p) => DAL.Person.PersonNumberToDate(p.SimpleCPRPerson.PersonCivilRegistrationIdentifier)));
            ret.Spouses.AddRange(GetPersonRelations<MaritalRelationshipType>(relationsResponse.OutputArrayRecord, RelationTypes.Spouse, RelationTypes.Partner));

            qualityLevel = QualityLevel.Cpr;
            return ret;
        }

        #endregion

        #region IPersonBasicDataProvider Members

        public PersonBasicStructureType GetCitizenBasic(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel)
        {
            // Call web service and fill return object
            var resp = CallAS78207(cprNumber);
            var addressResp = CallAN08002(cprNumber);
            bool protectAddress = resp.OutputRecord.CADRBSK.Equals("B") || resp.OutputRecord.CADRBSK.Equals("L");
            PersonBasicStructureType ret = new PersonBasicStructureType()
            {
                //AddressIdentifierCode = "",
                //AddressIdentifierCodeSpecified = "",
                MaritalStatusCode = Schemas.Util.Enums.GetMaritalStatus(resp.OutputRecord.CCIVS[0]),
                PersonNationalityCode = Country.GetCountryAlpha2CodeByKmdCode(resp.OutputRecord.ESTAT),
                RegularCPRPerson = new RegularCPRPersonType()
                {
                    PersonBirthDateStructure = new PersonBirthDateStructureType()
                    {
                        BirthDate = ToDateTime(resp.OutputRecord.DFOEDS).Value,
                        BirthDateUncertaintyIndicator = false
                    },
                    PersonCivilRegistrationStatusStructure = ToCivilRegistrationStatusStructureType(resp.OutputRecord.CSTATUK, resp.OutputRecord.CSTATUC, resp.OutputRecord.DSTATUS),
                    PersonInformationProtectionIndicator = protectAddress,
                    PersonNameForAddressingName = resp.OutputRecord.ANAVN,
                    SimpleCPRPerson = resp.ToSimpleCprPerson(),
                }
            };
            ret.Item = ToPersonNameAndAddress(addressResp).Item;
            ret.PersonDeathDateStructure = ToDeathDate(ret.RegularCPRPerson.PersonCivilRegistrationStatusStructure);
            ret.RegularCPRPerson.PersonGenderCode = ToGenderCode(ret.RegularCPRPerson.SimpleCPRPerson.PersonCivilRegistrationIdentifier);
            qualityLevel = QualityLevel.Cpr;
            return ret;

        }

        #endregion

        #region IPersonFullDataProvider Members

        public PersonFullStructureType GetCitizenFull(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel)
        {
            // Call web service and fill return object
            var resp = CallAS78207(cprNumber);
            var addressResp = CallAN08002(cprNumber);
            bool protectAddress = resp.OutputRecord.CADRBSK.Equals("B") || resp.OutputRecord.CADRBSK.Equals("L");
            PersonFullStructureType ret = new PersonFullStructureType()
            {
                //AddressIdentifierCode = "",
                //AddressIdentifierCodeSpecified = "",
                MaritalStatusCode = Schemas.Util.Enums.GetMaritalStatus(resp.OutputRecord.CCIVS[0]),
                NumberOfChildren = int.Parse(resp.OutputRecord.FBOERN),
                PersonNationalityCode = Country.GetCountryAlpha2CodeByKmdCode(resp.OutputRecord.ESTAT),
                RegularCPRPerson = new RegularCPRPersonType()
                {
                    PersonBirthDateStructure = new PersonBirthDateStructureType()
                    {
                        BirthDate = ToDateTime(resp.OutputRecord.DFOEDS).Value,
                        BirthDateUncertaintyIndicator = false
                    },
                    PersonCivilRegistrationStatusStructure = ToCivilRegistrationStatusStructureType(resp.OutputRecord.CSTATUK, resp.OutputRecord.CSTATUC, resp.OutputRecord.DSTATUS),
                    PersonInformationProtectionIndicator = protectAddress,
                    PersonNameForAddressingName = resp.OutputRecord.ANAVN,
                    SimpleCPRPerson = resp.ToSimpleCprPerson(),
                },
            };
            ret.Item = ToPersonNameAndAddress(addressResp).Item;
            ret.PersonDeathDateStructure = ToDeathDate(ret.RegularCPRPerson.PersonCivilRegistrationStatusStructure);
            ret.RegularCPRPerson.PersonGenderCode = ToGenderCode(ret.RegularCPRPerson.SimpleCPRPerson.PersonCivilRegistrationIdentifier);

            // Spouse name
            string spousePnrString = resp.OutputRecord.EPNRAEGT;
            int spousePnr;
            if (int.TryParse(spousePnrString, out spousePnr) && spousePnr > 0)
            {
                var nameAndAddressResp = CallAN08002(spousePnrString);
                ret.SpouseName = nameAndAddressResp.ToSimpleCprPerson().PersonNameStructure.ToString();
            }
            qualityLevel = QualityLevel.Cpr;
            return ret;
        }

        #endregion
    }
}
