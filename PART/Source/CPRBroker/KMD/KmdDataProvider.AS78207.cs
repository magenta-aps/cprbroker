using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;
using CprBroker.Providers.KMD.WS_AS78207;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.KMD
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
            WS_AS78207.WS_AS78207 service = new CprBroker.Providers.KMD.WS_AS78207.WS_AS78207();
            SetServiceUrl(service, ServiceTypes.AS78207);
            service.userinfoValue = new userinfo()
            {
                userid = UserName,
                password = Password
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
                    PersonNameStructure = new CprBroker.Schemas.PersonNameStructureType(this.OutputRecord.AFORNVN, this.OutputRecord.AEFTER)
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
                Schemas.Util.Address address = new CprBroker.Schemas.Util.Address();
                var a = this.InnerResponse.OutputRecord;
                address[CprBroker.Schemas.Util.AddressField.Building] = this.BuildingNumber;
                address[CprBroker.Schemas.Util.AddressField.CareOfName] = this.CareOfName;
                //address[CprBroker.Schemas.Util.AddressField.CountryCode] = "";
                //address[CprBroker.Schemas.Util.AddressField.CountryName] = "";
                //address[CprBroker.Schemas.Util.AddressField.DistrictSubDivisionIdentifier] = "";
                address[CprBroker.Schemas.Util.AddressField.Door] = this.DoorNumber;
                address[CprBroker.Schemas.Util.AddressField.Floor] = this.Floor;
                address[CprBroker.Schemas.Util.AddressField.HouseNumber] = this.HouseNumber;
                //TODO: Add house letter to Address fields

                //address[CprBroker.Schemas.Util.AddressField.Line1] = "";
                //address[CprBroker.Schemas.Util.AddressField.Line2] = "";
                //address[CprBroker.Schemas.Util.AddressField.Line3] = "";
                //address[CprBroker.Schemas.Util.AddressField.Line4] = "";
                //address[CprBroker.Schemas.Util.AddressField.Line5] = "";
                //address[CprBroker.Schemas.Util.AddressField.Line6] = "";
                //address[CprBroker.Schemas.Util.AddressField.LocationDescription] = "";
                //address[CprBroker.Schemas.Util.AddressField.MailDeliverSubLocationIdentifier] = "";
                address[CprBroker.Schemas.Util.AddressField.MunicipalityCode] = this.MunicipalityCode;
                //address[CprBroker.Schemas.Util.AddressField.MunicipalityName] = "";
                address[CprBroker.Schemas.Util.AddressField.PostBox] = "";
                address[CprBroker.Schemas.Util.AddressField.PostCode] = this.PostCode;
                //address[CprBroker.Schemas.Util.AddressField.PostDistrict] = "";
                address[CprBroker.Schemas.Util.AddressField.StreetCode] = this.StreetCode;
                //address[CprBroker.Schemas.Util.AddressField.StreetName] = "";
                //address[CprBroker.Schemas.Util.AddressField.StreetNameForAddressing] = "";

                // TODO: validate this Civil Registration Status Code
                var personCivilRegistrationStatusCodeType = Schemas.Util.Enums.ToCivilRegistrationStatus(Utilities.GetCivilRegistrationStatus(this.StatusKmd, this.StatusCpr));
                return address.ToPartAddress(personCivilRegistrationStatusCodeType);
            }

            public AttributListeType ToAttributListeType()
            {
                return new AttributListeType()
                    {
                        Egenskaber = new EgenskaberType[]
                        {
                            ToEgenskaberType()
                        },
                        RegisterOplysninger = new RegisterOplysningerType[]
                        {
                            ToRegisterOplysningerType()
                        },

                        // Health information not implemented
                        SundhedsOplysninger = null,

                        // No extensions at the moment
                        LokalUdvidelse = null
                    };
            }
            public EgenskaberType ToEgenskaberType()
            {
                var ret = new EgenskaberType()
                {
                    PersonBirthDateStructure = new CprBroker.Schemas.Part.PersonBirthDateStructureType()
                    {
                        //TODO: Handle null birthdate
                        BirthDate = Utilities.ToDateTime(BirthDate).Value,
                        BirthDateUncertaintyIndicator = false
                    },
                    AndreAdresser = null,
                    fodselsregistreringmyndighed = null,
                    foedested = null,
                    Kontaktkanal = null,
                    NaermestePaaroerende = null,
                    //TODO: Change this
                    PersonGenderCode = Utilities.ToPartGender(this.PNR),
                    PersonNameStructure = new CprBroker.Schemas.Part.PersonNameStructureType(FirstName, LastName),
                    Virkning = VirkningType.Create(
                        Utilities.GetMaxDate(BirthDate, AbroadDate, NameDate),
                        null)
                };
                return ret;
            }

            public RegisterOplysningerType ToRegisterOplysningerType()
            {
                var ret = new RegisterOplysningerType()
                {
                    //TODO: Fill with CPR, Foreign or Unknown
                    Item = null,
                    Virkning = VirkningType.Create(null, null)
                };
                if (string.Equals(NationalityCode, Constants.DanishNationalityCode))
                {
                    ret.Item = new CprBorgerType()
                    {
                        // No address note
                        AdresseNote = null,
                        // Church membership
                        // TODO : Where to fill fromDate?
                        FolkekirkeMedlemsskab = false,
                        //TODO: Fill address when class is ready
                        FolkeregisterAdresse = null,
                        // TODO: What is this (same name as above)
                        FolkeRegisterAdresse = "",
                        // Research protection
                        // TODO: Check if this is correct
                        ForskerBeskyttelseIndikator = false,
                        // TODO: Ensure that PNR has no dashes
                        PersonCivilRegistrationIdentifier = PNR,
                        // TODO: Check if this is correct
                        PersonInformationProtectionIndicator = false,
                        PersonNationalityCode = DAL.Country.GetCountryAlpha2CodeByKmdCode(NationalityCode),
                        //PNR validity status,
                        // TODO: Make sure that true is the cirrect value
                        PersonNummerGyldighedStatus = true,
                        // TODO: Check if this is correct
                        TelefonNummerBeskyttelseIndikator = false
                    };
                    ret.Virkning.FraTidspunkt = TidspunktType.Create(Utilities.GetMaxDate(AddressDate, RelocationDate, ImmigrationDate));
                }
                else if (!string.IsNullOrEmpty(NationalityCode))
                {
                    // TODO: Validate all data in this structure
                    ret.Item = new UdenlandskBorgerType()
                    {
                        // Birth country.Not in KMD
                        FoedselsLand = null,
                        // TODO: What is that?
                        PersonID = "",
                        // Languages. Not implemented here
                        Sprog = new string[] { },
                        // Citizenships
                        Statsborgerskaber = new string[] { DAL.Country.GetCountryAlpha2CodeByKmdCode(NationalityCode) },
                        PersonCivilRegistrationReplacementIdentifier = PNR,
                    };
                    ret.Virkning.FraTidspunkt = TidspunktType.Create(Utilities.GetMaxDate(ImmigrationDate, AbroadDate));
                }
                else
                {
                    // TODO: Validate all data in this structure
                    ret.Item = new UkendtBorgerType()
                    {
                        PersonCivilRegistrationReplacementIdentifier = PNR,
                    };
                    ret.Virkning.FraTidspunkt = TidspunktType.Create(Utilities.GetMaxDate(AbroadDate, AddressDate, DisempowermentDate, ImmigrationDate, PaternityDate, RelocationDate, StatusDate));
                }
                return ret;
            }

            public TilstandListeType ToTilstandListeType()
            {
                return new TilstandListeType()
                {
                    //TODO: Fill with orgfaelles:Gyldighed as soon as knowing what that is???
                    //Gyldighed = null,

                    CivilStatus = new CivilStatusType[]
                    {
                        new CivilStatusType()
                       {
                           Status = Utilities.ToPartMaritalStatus(MaritallStatusCode[0]),
                           TilstandVirkning = TilstandVirkningType.Create(Utilities.ToDateTime(MaritalStatusDate)),
                       }
                    },
                    LivStatus = new LivStatusType[]
                    {
                        new LivStatusType
                        {
                            //TODO: Status date may not be the correct field (for example, the status may have changed fromDate 01 to  07 at the date, but the life status is still alive)
                            Status  = Schemas.Util.Enums.ToLifeStatus(Utilities.GetCivilRegistrationStatus(StatusKmd, StatusCpr), Utilities.ToDateTime(BirthDate)),
                            TilstandVirkning = TilstandVirkningType.Create(Utilities.ToDateTime(StatusDate)),                            
                        }
                    },
                    // No extensions now
                    LokalUdvidelse = null
                };
            }

            public DateTime? GetRegistrationDate()
            {
                return Utilities.GetMaxDate(
                    // TODO : GetPropertyValuesOfType the other possible registration dates
                    //AbroadDate,
                    AddressDate,
                    DisempowermentDate,
                    ImmigrationDate,
                    MaritalStatusDate,
                    //NameDate,
                    PaternityDate,
                    RelocationDate,
                    StatusDate
                );
            }

            #endregion
        }
    }
}
