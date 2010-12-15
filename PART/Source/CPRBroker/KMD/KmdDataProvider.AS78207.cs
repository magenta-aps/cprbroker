using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Engine;
using CPRBroker.Providers.KMD.WS_AS78207;
using CPRBroker.Schemas;

namespace CPRBroker.Providers.KMD
{
    public partial class KmdDataProvider
    {
        /// <summary>
        /// Calls the AS78207 web service
        /// </summary>
        /// <param name="cprNumber"></param>
        /// <returns></returns>
        private AS78207Response CallAS78207(string cprNumber)
        {
            WS_AS78207.WS_AS78207 service = new CPRBroker.Providers.KMD.WS_AS78207.WS_AS78207();
            SetServiceUrl(service, ServiceTypes.AS78207);
            service.userinfoValue = new userinfo()
            {
                userid = DatabaseObject.UserName,
                password = DatabaseObject.Password
            };
            AS78207 param = new AS78207()
            {
                InputRecord = new PARM()
                {
                    CBESTIL = "0",
                    COMRAADE = "C",  // Municipal: K   Regional:  R   National: C
                    CREDIG = "",
                    CSTATUS = "1",
                    EKOM = "000",
                    EPNR = cprNumber
                }
            };
            var resp = service.SubmitAS78207(param);
            ValidateReturnCode(resp.OutputRecord.RETURKODE, resp.OutputRecord.RETURTEXT);
            return resp;
        }
    }
    namespace WS_AS78207
    {
        public partial class AS78207Response
        {


            public SimpleCPRPersonType ToSimpleCprPerson()
            {
                SimpleCPRPersonType ret = new SimpleCPRPersonType()
                {
                    PersonCivilRegistrationIdentifier = this.OutputRecord.EPNR,
                    PersonNameStructure = new PersonNameStructureType(this.OutputRecord.AFORNVN, this.OutputRecord.AEFTER)
                };

                return ret;
            }
        }

        public class EnglishAS78207Response
        {

            private AS78207Response InnerResponse;

            public EnglishAS78207Response(AS78207Response innerResponse)
            {
                InnerResponse = innerResponse;
            }

            #region Properties
            public string ReturnCode
            {
                get
                {
                    return this.InnerResponse.OutputRecord.RETURKODE;
                }
            }

            public string ReturnText
            {
                get
                {
                    return this.InnerResponse.OutputRecord.RETURTEXT;
                }
            }

            public string MunicipalityCode
            {
                get
                {
                    return this.InnerResponse.OutputRecord.EKOM;
                }
            }

            public string EINRKMD
            {
                get
                {
                    return this.InnerResponse.OutputRecord.EINRKMD;
                }
            }

            public string PNR
            {
                get
                {
                    return this.InnerResponse.OutputRecord.EPNR;
                }
            }

            public string BirthDate
            {
                get
                {
                    return this.InnerResponse.OutputRecord.DFOEDS;
                }
            }

            public string StatusKmd
            {
                get
                {
                    return this.InnerResponse.OutputRecord.CSTATUK;
                }
            }

            public string StatusCpr
            {
                get
                {
                    return this.InnerResponse.OutputRecord.CSTATUC;
                }
            }

            public string StatusDate
            {
                get
                {
                    return this.InnerResponse.OutputRecord.DSTATUS;
                }
            }

            public string PersonalMarker
            {
                get
                {
                    return this.InnerResponse.OutputRecord.CPNRMRK;
                }
            }

            public string TransCode
            {
                get
                {
                    return this.InnerResponse.OutputRecord.CTRANS;
                }
            }

            public string RegistrationAuthorityCode
            {
                get
                {
                    return this.InnerResponse.OutputRecord.EFOEDREG;
                }
            }

            public string RegistrationAuthorityName
            {
                get
                {
                    return this.InnerResponse.OutputRecord.AFOEDREG;
                }
            }

            public string Title
            {
                get
                {
                    return this.InnerResponse.OutputRecord.ASTIL;
                }
            }

            public string Title34
            {
                get
                {
                    return this.InnerResponse.OutputRecord.ASTIL34;
                }
            }

            public string PostCode
            {
                get
                {
                    return this.InnerResponse.OutputRecord.CSTIL;
                }
            }

            public string AddressingName
            {
                get
                {
                    return this.InnerResponse.OutputRecord.ANAVN;
                }
            }

            public string AddressingName34
            {
                get
                {
                    return this.InnerResponse.OutputRecord.ANAVN34;
                }
            }

            public string NameMarker
            {
                get
                {
                    return this.InnerResponse.OutputRecord.CNVMRK;
                }
            }

            public string NameDate
            {
                get
                {
                    return this.InnerResponse.OutputRecord.DNAVN;
                }
            }

            public string NameAuthorityCode
            {
                get
                {
                    return this.InnerResponse.OutputRecord.ENVNMYN;
                }
            }

            public string NationalityCode
            {
                get
                {
                    return this.InnerResponse.OutputRecord.ESTAT;
                }
            }

            public string AddressDate
            {
                get
                {
                    return this.InnerResponse.OutputRecord.DADR;
                }
            }

            public string AddressProtection
            {
                get
                {
                    return this.InnerResponse.OutputRecord.CADRBSK;
                }
            }

            public string StreetCode
            {
                get
                {
                    return this.InnerResponse.OutputRecord.CVEJ;
                }
            }

            public string HouseNumber
            {
                get
                {
                    return this.InnerResponse.OutputRecord.AHUSNR;
                }
            }

            public string HouseLetter
            {
                get
                {
                    return this.InnerResponse.OutputRecord.ABOGSTV;
                }
            }

            public string Floor
            {
                get
                {
                    return this.InnerResponse.OutputRecord.AETAGE;
                }
            }

            public string DoorNumber
            {
                get
                {
                    return this.InnerResponse.OutputRecord.ASIDOER;
                }
            }

            public string BuildingNumber
            {
                get
                {
                    return this.InnerResponse.OutputRecord.EBYG;
                }
            }

            public string RelocationDate
            {
                get
                {
                    return this.InnerResponse.OutputRecord.DTILFL;
                }
            }

            public string EmigrationAddressType
            {
                get
                {
                    return this.InnerResponse.OutputRecord.CFRAADR;
                }
            }

            public string EmigrationPlace
            {
                get
                {
                    return this.InnerResponse.OutputRecord.EFRASTD;
                }
            }

            public string EmigrationAddress
            {
                get
                {
                    return this.InnerResponse.OutputRecord.AFRAADR;
                }
            }

            public string ChurchRelationship
            {
                get
                {
                    return this.InnerResponse.OutputRecord.CKIRKE;
                }
            }

            public string DisempowermentCode
            {
                get
                {
                    return this.InnerResponse.OutputRecord.CUMYND;
                }
            }

            public string MunicipalRelationship1
            {
                get
                {
                    return this.InnerResponse.OutputRecord.CKOMF1;
                }
            }

            public string MunicipalRelationship2
            {
                get
                {
                    return this.InnerResponse.OutputRecord.CKOMF2;
                }
            }

            public string MaritallStatusCode
            {
                get
                {
                    return this.InnerResponse.OutputRecord.CCIVS;
                }
            }

            public string MaritalStatusDate
            {
                get
                {
                    return this.InnerResponse.OutputRecord.DCIVS;
                }
            }

            public string MaritalStatusAuthorityCode
            {
                get
                {
                    return this.InnerResponse.OutputRecord.ECIVMYN;
                }
            }

            public string MaritalStatusAuthorityName
            {
                get
                {
                    return this.InnerResponse.OutputRecord.ACIVMYN;
                }
            }

            public string SpousePNR
            {
                get
                {
                    return this.InnerResponse.OutputRecord.EPNRAEGT;
                }
            }

            public string MotherPNR
            {
                get
                {
                    return this.InnerResponse.OutputRecord.EPNRMOR;
                }
            }

            public string MotherVerificationCode
            {
                get
                {
                    return this.InnerResponse.OutputRecord.CVERMOR;
                }
            }

            public string FatherPNR
            {
                get
                {
                    return this.InnerResponse.OutputRecord.EPNRFAR;
                }
            }

            public string FatherVerificationCode
            {
                get
                {
                    return this.InnerResponse.OutputRecord.CVERFAR;
                }
            }

            public string NumberOfChildren
            {
                get
                {
                    return this.InnerResponse.OutputRecord.FBOERN;
                }
            }

            public string[] ChildrenPNRs
            {
                get
                {
                    return this.InnerResponse.OutputRecord.EPNRBRN;
                }
            }

            public string[] ChildrenVerificationCodes
            {
                get
                {
                    return this.InnerResponse.OutputRecord.CVERBRN;
                }
            }

            public string LastName
            {
                get
                {
                    return this.InnerResponse.OutputRecord.AEFTER;
                }
            }

            public string FirstName
            {
                get
                {
                    return this.InnerResponse.OutputRecord.AFORNVN;
                }
            }

            public string CareOfName
            {
                get
                {
                    return this.InnerResponse.OutputRecord.ACONVN;
                }
            }

            public string CareOfName34
            {
                get
                {
                    return this.InnerResponse.OutputRecord.ACONVN34;
                }
            }

            public string PaternityDate
            {
                get
                {
                    return this.InnerResponse.OutputRecord.DFADSKB;
                }
            }

            public string PaternityAuthorityCode
            {
                get
                {
                    return this.InnerResponse.OutputRecord.EFADMYN;
                }
            }

            public string SearchName
            {
                get
                {
                    return this.InnerResponse.OutputRecord.ASOEGE;
                }
            }

            public string SearchName34
            {
                get
                {
                    return this.InnerResponse.OutputRecord.ASOEGE34;
                }
            }

            public string ImmigrationDate
            {
                get
                {
                    return this.InnerResponse.OutputRecord.DINDV;
                }
            }

            public string SupplementaryAddress
            {
                get
                {
                    return this.InnerResponse.OutputRecord.ASUPPL;
                }
            }

            public string AbroadDate
            {
                get
                {
                    return this.InnerResponse.OutputRecord.DUDLAND;
                }
            }

            public string DisempowermentDate
            {
                get
                {
                    return this.InnerResponse.OutputRecord.DUMYND;
                }
            }

            public string DisempowermentAuthority
            {
                get
                {
                    return this.InnerResponse.OutputRecord.EUMYND;
                }
            }

            public string KmdCurrentPNR
            {
                get
                {
                    return this.InnerResponse.OutputRecord.EPNRGAEL;
                }
            }
            #endregion

            #region Converters
            /// <summary>
            /// Converts the address to an OIO address by using Schemas.Util.Address class
            /// </summary>
            /// <param name="personCivilRegistrationStatusCodeType"></param>
            /// <returns></returns>            
            public Schemas.Part.Address ToPartAddress()
            {
                Schemas.Util.Address address = new CPRBroker.Schemas.Util.Address();
                var a = this.InnerResponse.OutputRecord;
                address[CPRBroker.Schemas.Util.AddressField.Building] = this.BuildingNumber;
                address[CPRBroker.Schemas.Util.AddressField.CareOfName] = this.CareOfName;
                //address[CPRBroker.Schemas.Util.AddressField.CountryCode] = "";
                //address[CPRBroker.Schemas.Util.AddressField.CountryName] = "";
                //address[CPRBroker.Schemas.Util.AddressField.DistrictSubDivisionIdentifier] = "";
                address[CPRBroker.Schemas.Util.AddressField.Door] = this.DoorNumber;
                address[CPRBroker.Schemas.Util.AddressField.Floor] = this.Floor;
                address[CPRBroker.Schemas.Util.AddressField.HouseNumber] = this.HouseNumber;
                //TODO: Add house letter to Address fields

                //address[CPRBroker.Schemas.Util.AddressField.Line1] = "";
                //address[CPRBroker.Schemas.Util.AddressField.Line2] = "";
                //address[CPRBroker.Schemas.Util.AddressField.Line3] = "";
                //address[CPRBroker.Schemas.Util.AddressField.Line4] = "";
                //address[CPRBroker.Schemas.Util.AddressField.Line5] = "";
                //address[CPRBroker.Schemas.Util.AddressField.Line6] = "";
                //address[CPRBroker.Schemas.Util.AddressField.LocationDescription] = "";
                //address[CPRBroker.Schemas.Util.AddressField.MailDeliverSubLocationIdentifier] = "";
                address[CPRBroker.Schemas.Util.AddressField.MunicipalityCode] = this.MunicipalityCode;
                //address[CPRBroker.Schemas.Util.AddressField.MunicipalityName] = "";
                address[CPRBroker.Schemas.Util.AddressField.PostBox] = "";
                address[CPRBroker.Schemas.Util.AddressField.PostCode] = this.PostCode;
                //address[CPRBroker.Schemas.Util.AddressField.PostDistrict] = "";
                address[CPRBroker.Schemas.Util.AddressField.StreetCode] = this.StreetCode;
                //address[CPRBroker.Schemas.Util.AddressField.StreetName] = "";
                //address[CPRBroker.Schemas.Util.AddressField.StreetNameForAddressing] = "";

                // TODO: validate this Civil Registration Status Code
                var personCivilRegistrationStatusCodeType = Schemas.Util.Enums.ToCivilRegistrationStatus(KmdDataProvider.GetCivilRegistrationStatus(this.StatusKmd, this.StatusCpr));
                return address.ToPartAddress(personCivilRegistrationStatusCodeType);
            }
            #endregion
        }
    }
}
