/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

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
        public AS78207Response CallAS78207(string cprNumber)
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
            Engine.Local.Admin.AddNewLog(System.Diagnostics.TraceEventType.Information, "CallAS78207", string.Format("Calling AS78207 with PNR {0}", cprNumber), null, null);
            var resp = service.SubmitAS78207(param);
            ValidateReturnCode(resp.OutputRecord.RETURKODE, resp.OutputRecord.RETURTEXT);
            // We log the call and set the success parameter to true
            DataProviderManager.LogAction(this, "Read", true);
            return resp;
        }
    }
    namespace WS_AS78207
    {

        public class EnglishAS78207Response
        {

            public AS78207Response InnerResponse;

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

            public string Protection
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

            public AttributListeType ToAttributListeType(WS_AS78205.EnglishAS78205Response addressResponse)
            {
                return new AttributListeType()
                    {
                        Egenskab = new EgenskabType[]
                        {
                            ToEgenskaberType()
                        },
                        RegisterOplysning = new RegisterOplysningType[]
                        {
                            ToRegisterOplysningType(addressResponse)
                        },

                        // Health information not implemented
                        SundhedOplysning = null,

                        // No extensions at the moment
                        LokalUdvidelse = null
                    };
            }
            public EgenskabType ToEgenskaberType()
            {
                var ret = new EgenskabType()
                {
                    BirthDate = Utilities.ToDateTime(BirthDate).Value,
                    AndreAdresser = null,
                    FoedselsregistreringMyndighedNavn = null,
                    FoedestedNavn = null,
                    KontaktKanal = null,
                    NaermestePaaroerende = null,
                    //TODO: Change this
                    PersonGenderCode = Utilities.ToPartGender(this.PNR),
                    NavnStruktur = ToNavnStrukturType(),
                    Virkning = VirkningType.Create(
                        Utilities.GetMaxDate(BirthDate, AbroadDate, NameDate),
                        null)
                };
                return ret;
            }

            public NavnStrukturType ToNavnStrukturType()
            {
                return NavnStrukturType.Create(new string[] { this.FirstName, this.LastName }, this.AddressingName);
            }

            public RegisterOplysningType ToRegisterOplysningType(WS_AS78205.EnglishAS78205Response addressResponse)
            {
                var ret = new RegisterOplysningType()
                {
                    Item = null,
                    Virkning = VirkningType.Create(null, null)
                };
                // TODO: Always return CprBorgerType !!!!
                if (string.Equals(NationalityCode, Constants.DanishNationalityCode))
                {
                    ret.Item = new CprBorgerType()
                    {
                        PersonCivilRegistrationIdentifier = PNR,
                        PersonNationalityCode = Schemas.Part.CountryIdentificationCodeType.Create(CprBroker.Schemas.Part._CountryIdentificationSchemeType.imk, NationalityCode),
                        FolkeregisterAdresse = addressResponse.ToAdresseType(),
                        // Research protection
                        ForskerBeskyttelseIndikator = Protection.Equals(Constants.ResearchProtection),
                        // Name and address protection
                        NavneAdresseBeskyttelseIndikator = Protection.Equals(Constants.AddressProtection),
                        // Church membership
                        // TODO: Shall this be ChurchRelationship = 'F'?
                        FolkekirkeMedlemIndikator = ChurchRelationship.Length > 0,
                        // No address note
                        AdresseNoteTekst = null,
                        //PNR validity status
                        // TODO: Shall this be set as other providers, false if status is 30,50,60 ?
                        PersonNummerGyldighedStatusIndikator = int.Parse(ReturnCode) < 10,

                        // TODO: Check if this is correct
                        TelefonNummerBeskyttelseIndikator = Protection.Equals(Constants.AddressProtection),
                    };
                    ret.Virkning.FraTidspunkt = TidspunktType.Create(Utilities.GetMaxDate(AddressDate, RelocationDate, ImmigrationDate));
                }
                else if (!string.IsNullOrEmpty(NationalityCode))
                {
                    // TODO: Validate all data in this structure
                    ret.Item = new UdenlandskBorgerType()
                    {
                        // Birth country.Not in KMD
                        FoedselslandKode = null,
                        // TODO: What is that?
                        PersonIdentifikator = "",
                        // Languages. Not implemented here
                        SprogKode = new CprBroker.Schemas.Part.CountryIdentificationCodeType[0],
                        // Citizenships
                        PersonNationalityCode = new CprBroker.Schemas.Part.CountryIdentificationCodeType[] { CprBroker.Schemas.Part.CountryIdentificationCodeType.Create(CprBroker.Schemas.Part._CountryIdentificationSchemeType.imk, NationalityCode) },
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
                    CivilStatus = new CivilStatusType()
                    {
                        CivilStatusKode = Utilities.ToPartMaritalStatus(MaritallStatusCode[0]),
                        TilstandVirkning = TilstandVirkningType.Create(Utilities.ToDateTime(MaritalStatusDate)),
                    },
                    LivStatus = new LivStatusType
                    {
                        //TODO: Status date may not be the correct field (for example, the status may have changed from 01 to  07 at the date, but the life status is still alive)
                        LivStatusKode = Schemas.Util.Enums.ToLifeStatus(Utilities.GetCivilRegistrationStatus(StatusKmd, StatusCpr), Utilities.ToDateTime(BirthDate)),
                        TilstandVirkning = TilstandVirkningType.Create(Utilities.ToDateTime(StatusDate)),
                    },
                    // No extensions now
                    LokalUdvidelse = null
                };
            }



            public DateTime? GetRegistrationDate()
            {
                return Utilities.GetMaxDate(
                    // TODO : What should be the registration date? There are all effect dates !!!
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
