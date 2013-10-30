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
using CprBroker.Providers.KMD.WS_AS78205;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.KMD
{
    public partial class KmdDataProvider
    {
        /// <summary>
        /// Calls the AS78205 web service
        /// </summary>
        /// <param name="cprNumber"></param>
        /// <returns></returns>
        public EnglishAS78205Response CallAS78205(string cprNumber)
        {
            if (modulus11OK(cprNumber))
            {
                WS_AS78205.WS_AS78205 service = new CprBroker.Providers.KMD.WS_AS78205.WS_AS78205();

                SetServiceUrl(service, ServiceTypes.AS78205);
                service.userinfoValue = new userinfo();

                service.userinfoValue.userid = UserName;
                service.userinfoValue.password = Password;

                var input = new AS78205()
                {
                    InputRecord = new PARM()
                    {
                        CBESTIL = "0",
                        COMRAADE = "C",  // Municipal: K   Regional:  R   National: C
                        CREDIG = "O",  // Address line format: Fixed line position: F Organized (move empty lines to the end): O
                        CSTATUS = "1",
                        EKOM = "000",
                        EPNR = cprNumber,
                    }
                };
                var response = service.SubmitAS78205(input);
                var englishResponse = new EnglishAS78205Response(response);
                ValidateReturnCode(englishResponse.ReturnCode, englishResponse.ReturnText);

                return englishResponse;
            }
            else
            {
                return null;
            }
        }

    }

    namespace WS_AS78205
    {
        public class EnglishAS78205Response
        {
            public AS78205Response InnerResponse;

            public EnglishAS78205Response(AS78205Response innerResponse)
            {
                InnerResponse = innerResponse;
            }

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

            public string JobDescription
            {
                get
                {
                    return InnerResponse.OutputRecord.ASTIL_A;
                }
            }

            public string JobDescription34
            {
                get
                {
                    return InnerResponse.OutputRecord.ASTIL34_A;
                }
            }

            public string[] AddressLines
            {
                get
                {
                    return InnerResponse.OutputRecord.ALIN;
                }
            }

            public int NumberOfAddressLines
            {
                get
                {
                    return int.Parse(InnerResponse.OutputRecord.FANT_LIN);
                }
            }

            public string PNR
            {
                get
                {
                    return InnerResponse.OutputRecord.EPNR_A;
                }
            }

            public string FFF
            {
                get
                {
                    return InnerResponse.OutputRecord.DFOEDS_A;
                }
            }

            public string Statuskmd
            {
                get
                {
                    return InnerResponse.OutputRecord.CSTATUK_A;
                }
            }

            public string AddressingName
            {
                get
                {
                    return InnerResponse.OutputRecord.ANAVN_A;
                }
            }

            public string AddressingName34
            {
                get
                {
                    return InnerResponse.OutputRecord.ANAVN34_A;
                }
            }

            public string MunicipalityCode
            {
                get
                {
                    return InnerResponse.OutputRecord.EKOM_A;
                }
            }

            public string StreetCode
            {
                get
                {
                    return InnerResponse.OutputRecord.CVEJ_A;
                }
            }

            public string HouseNumber
            {
                get
                {
                    return InnerResponse.OutputRecord.AHUSNR_A;
                }
            }

            public string HouseLetter
            {
                get
                {
                    return InnerResponse.OutputRecord.ABOGSTV_A;
                }
            }

            public string Floor
            {
                get
                {
                    return InnerResponse.OutputRecord.AETAGE_A;
                }
            }

            public string Door
            {
                get
                {
                    return InnerResponse.OutputRecord.ASIDOER_A;
                }
            }

            public string BuildingCode
            {
                get
                {
                    return InnerResponse.OutputRecord.EBYG_A;
                }
            }

            public string StreetName
            {
                get
                {
                    return InnerResponse.OutputRecord.AVEJ;
                }
            }

            public string CityName
            {
                get
                {
                    return InnerResponse.OutputRecord.ABYNA;
                }
            }

            public string CityName34
            {
                get
                {
                    return InnerResponse.OutputRecord.ABYNA34_A;
                }
            }

            public string PostCode
            {
                get
                {
                    return InnerResponse.OutputRecord.EPOSTNR_A;
                }
            }

            public string PostDistrict
            {
                get
                {
                    return InnerResponse.OutputRecord.APOST_A;
                }
            }

            public string PostDistrict20
            {
                get
                {
                    return InnerResponse.OutputRecord.APOST20_A;
                }
            }

            public string Protection
            {
                get
                {
                    return InnerResponse.OutputRecord.CADRBSK_A;
                }
            }

            public string CareOfName
            {
                get
                {
                    return InnerResponse.OutputRecord.ACONAVN_A;
                }
            }

            public string CareOfName34
            {
                get
                {
                    return InnerResponse.OutputRecord.ACONAVN34_A;
                }
            }

            public string SpousePnr
            {
                get
                {
                    return InnerResponse.OutputRecord.EPNRAEGT_A;
                }
            }

            public string CivilStatusCode
            {
                get
                {
                    return InnerResponse.OutputRecord.CCIVS_A;
                }
            }

            public string MotherPnr
            {
                get
                {
                    return InnerResponse.OutputRecord.EPNRMOR_A;
                }
            }

            public string FatherPnr
            {
                get
                {
                    return InnerResponse.OutputRecord.EPNRFAR_A;
                }
            }

            public bool ToSpecielVejkodeIndikator()
            {
                int code;
                if (int.TryParse(this.StreetCode, out code))
                {
                    return code > 9900;
                }
                return false;
            }

            public bool ToSpecielVejkodeIndikatorSpecified()
            {
                int code;
                return int.TryParse(this.StreetCode, out code);
            }

            public AdresseType ToAdresseType()
            {
                return new AdresseType()
                {
                    Item = new DanskAdresseType()
                    {
                        AddressComplete = new CprBroker.Schemas.Part.AddressCompleteType()
                        {
                            AddressAccess = new CprBroker.Schemas.Part.AddressAccessType()
                            {
                                MunicipalityCode = MunicipalityCode,
                                StreetBuildingIdentifier = string.Format("{0}{1}", HouseNumber, HouseLetter),
                                StreetCode = StreetCode,
                            },
                            AddressPostal = new CprBroker.Schemas.Part.AddressPostalType()
                            {
                                CountryIdentificationCode = null,
                                DistrictName = PostDistrict20,
                                DistrictSubdivisionIdentifier = CityName34,
                                FloorIdentifier = Floor,
                                MailDeliverySublocationIdentifier = null,
                                PostCodeIdentifier = PostCode,
                                PostOfficeBoxIdentifier = null,
                                StreetBuildingIdentifier = string.Format("{0}{1}", HouseNumber, HouseLetter),
                                StreetName = StreetName,
                                StreetNameForAddressingName = StreetName,
                                SuiteIdentifier = Door
                            },
                        },
                        AddressPoint = null,
                        NoteTekst = null,
                        PolitiDistriktTekst = null,
                        PostDistriktTekst = PostDistrict,
                        SkoleDistriktTekst = null,
                        SocialDistriktTekst = null,
                        SogneDistriktTekst = null,
                        SpecielVejkodeIndikator = ToSpecielVejkodeIndikator(),
                        SpecielVejkodeIndikatorSpecified = ToSpecielVejkodeIndikatorSpecified(),
                        UkendtAdresseIndikator = false,
                        ValgkredsDistriktTekst = null
                    },
                };

            }

        }
    }
}
