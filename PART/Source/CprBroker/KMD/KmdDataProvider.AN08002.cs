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
using CprBroker.Schemas;
using CprBroker.Providers.KMD.WS_AN08002;

namespace CprBroker.Providers.KMD
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
            WS_AN08002.WS_AN08002 service = new CprBroker.Providers.KMD.WS_AN08002.WS_AN08002();
            SetServiceUrl(service, ServiceTypes.AN08002);
            service.userinfoValue = new userinfo();

            service.userinfoValue.userid = UserName;
            service.userinfoValue.password = Password;

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

        }
    }


}
