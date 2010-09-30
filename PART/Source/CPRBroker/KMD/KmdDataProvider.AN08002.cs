using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas;
using CPRBroker.Providers.KMD.WS_AN08002;

namespace CPRBroker.Providers.KMD
{
    public partial class KmdDataProvider
    {
        /// <summary>
        /// Calls the AN08002 web service
        /// </summary>
        /// <param name="cprNumber"></param>
        /// <returns></returns>
        private EnglishAN08002Response CallAN08002(string cprNumber)
        {
            WS_AN08002.WS_AN08002 service = new CPRBroker.Providers.KMD.WS_AN08002.WS_AN08002();
            SetServiceUrl(service, ServiceTypes.AN08002);
            service.userinfoValue = new userinfo();

            service.userinfoValue.userid = DatabaseObject.UserName;
            service.userinfoValue.password = DatabaseObject.Password;

            AN08002 input = new AN08002()
            {
                InputRecord = new PARM()
                {
                    personnummer = cprNumber,
                    omraade = "C"  // Municipal: K   Regional:  R   National: C
                }
            };
            AN08002Response response = service.SubmitAN08002(input);
            EnglishAN08002Response englishResponse = new EnglishAN08002Response(response);
            ValidateReturnCode(englishResponse.ReturnCode, englishResponse.ReturnText);
            return englishResponse;
        }

        private PersonNameAndAddressStructureType ToPersonNameAndAddress(EnglishAN08002Response englishResponse)
        {
            PersonNameAndAddressStructureType personNameAndAddress = new PersonNameAndAddressStructureType();
            personNameAndAddress.AddressIdentifierCode = AddressIdentifierCodeType.Item1;
            //personNameAndAddress.AddressIdentifierCodeSpecified = true;

            Schemas.Util.Address address = new CPRBroker.Schemas.Util.Address();
            //address[CPRBroker.Schemas.Util.AddressField.Building] = 
            address[CPRBroker.Schemas.Util.AddressField.Door] = englishResponse.AddressDoor;
            address[CPRBroker.Schemas.Util.AddressField.Floor] = englishResponse.AddressFloor;
            address[CPRBroker.Schemas.Util.AddressField.HouseNumber] = string.Format("{0}{1}", englishResponse.AddressHouseNumber, englishResponse.AddressHouseNumberChar);
            address[CPRBroker.Schemas.Util.AddressField.PostCode] = englishResponse.EPostNumber;
            address[CPRBroker.Schemas.Util.AddressField.PostDistrictName] = englishResponse.AddressPost_20;
            address[CPRBroker.Schemas.Util.AddressField.StreetName] = englishResponse.Route;
            address[CPRBroker.Schemas.Util.AddressField.StreetCode] = englishResponse.RouteCode;
            address[CPRBroker.Schemas.Util.AddressField.MunicipalityCode] = englishResponse.MunicipalityCode;
            address[CPRBroker.Schemas.Util.AddressField.MunicipalityName] = englishResponse.MunicipalityName;

            address[CPRBroker.Schemas.Util.AddressField.Line1] = englishResponse.AddressLine_1;
            address[CPRBroker.Schemas.Util.AddressField.Line2] = englishResponse.AddressLine_2;
            address[CPRBroker.Schemas.Util.AddressField.Line3] = englishResponse.AddressLine_3;
            address[CPRBroker.Schemas.Util.AddressField.Line4] = englishResponse.AddressLine_4;
            address[CPRBroker.Schemas.Util.AddressField.Line5] = englishResponse.AddressLine_5;

            personNameAndAddress.Item = address.ToOioAddress(PersonCivilRegistrationStatusCodeType.Item01);

            //personNameAndAddress.PersonInformationProtectionIndicator = true;

            personNameAndAddress.SimpleCPRPerson = englishResponse.ToSimpleCprPerson();

            return personNameAndAddress;
        }
    }

    
    namespace WS_AN08002
    {
        /// <summary>
        /// Implements the Adapter design pattern for the AN08002 class, by translationg all fields to English
        /// </summary>
        public class EnglishAN08002Response
        {

            private AN08002Response Response;

            public EnglishAN08002Response(AN08002Response response)
            {
                Response = response;
            }

            public string ReturnCode
            {
                get
                {
                    return Response.OutputRecord.returkode;
                }
            }

            public string ReturnText
            {
                get
                {
                    return Response.OutputRecord.returtekst;
                }
            }

            public string Authorized
            {
                get
                {
                    return Response.OutputRecord.autoriseret;
                }
            }

            public string PersonNumber
            {
                get
                {
                    return Response.OutputRecord.personnummer;
                }
            }

            public string AddressingName_34
            {
                get
                {
                    return Response.OutputRecord.anavn_34;
                }
            }

            public string AddressLine_1
            {
                get
                {
                    return Response.OutputRecord.alin_1;
                }
            }

            public string AddressLine_2
            {
                get
                {
                    return Response.OutputRecord.alin_2;
                }
            }

            public string AddressLine_3
            {
                get
                {
                    return Response.OutputRecord.alin_3;
                }
            }

            public string AddressLine_4
            {
                get
                {
                    return Response.OutputRecord.alin_4;
                }
            }

            public string AddressLine_5
            {
                get
                {
                    return Response.OutputRecord.alin_5;
                }
            }

            public string AddressStyle_34
            {
                get
                {
                    return Response.OutputRecord.astil_34;
                }
            }

            public string Immigration
            {
                get
                {
                    return Response.OutputRecord.tilflyttet;
                }
            }

            public string MaritalStatus // Could be civil status
            {
                get
                {
                    return Response.OutputRecord.civilstand;
                }
            }

            public string MaritalStatusDate// Could be civil status date
            {
                get
                {
                    return Response.OutputRecord.civildato;
                }
            }

            public string AuthorityCode
            {
                get
                {
                    return Response.OutputRecord.myndigkode;
                }
            }

            public string AuthorityName
            {
                get
                {
                    return Response.OutputRecord.myndignavn;
                }
            }

            public string Spouse
            {
                get
                {
                    return Response.OutputRecord.aegtefaelle;
                }
            }

            public string AddressMessage
            {
                get
                {
                    return Response.OutputRecord.adressebesk;
                }
            }

            public string MunicipalityCode
            {
                get
                {
                    return Response.OutputRecord.kommunekode;
                }
            }

            public string MunicipalityName
            {
                get
                {
                    return Response.OutputRecord.kommunenavn;
                }
            }

            public string RouteCode
            {
                get
                {
                    return Response.OutputRecord.vejkode;
                }
            }

            public string Route
            {
                get
                {
                    return Response.OutputRecord.avej;
                }
            }

            public string AddressHouseNumber
            {
                get
                {
                    return Response.OutputRecord.ahusnr;
                }
            }


            public string AddressHouseNumberChar
            {
                get
                {
                    return Response.OutputRecord.abogstv;
                }
            }

            public string AddressFloor
            {
                get
                {
                    return Response.OutputRecord.aetage;
                }
            }

            public string AddressDoor
            {
                get
                {
                    return Response.OutputRecord.asidoer;
                }
            }

            public string EPostNumber
            {
                get
                {
                    return Response.OutputRecord.epostnr;
                }
            }

            public string AddressPost_20
            {
                get
                {
                    return Response.OutputRecord.apost_20;
                }
            }

            public string tct_kommune //unknown
            {
                get
                {
                    return Response.OutputRecord.tct_kommune;
                }
            }

            public SimpleCPRPersonType ToSimpleCprPerson()
            {                
                return new SimpleCPRPersonType()
                {
                    PersonCivilRegistrationIdentifier = this.PersonNumber,
                    PersonNameStructure = new PersonNameStructureType(this.AddressingName_34)
                };
            }
        }
    }


}
