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
using CprBroker.Providers.KMD.WS_AN08010;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.KMD
{
    public partial class KmdDataProvider
    {
        /// <summary>
        /// Calls the AN08010 web service
        /// </summary>
        /// <param name="cprNumber"></param>
        /// <returns></returns>
        private EnglishAN08010Response CallAN08010(string cprNumber)
        {
            WS_AN08010.WS_AN08010 service = new CprBroker.Providers.KMD.WS_AN08010.WS_AN08010();
            SetServiceUrl(service, ServiceTypes.AN08010);
            service.userinfoValue = new userinfo()
            {
                userid = UserName,
                password = Password
            };
            AN08010 param = new AN08010()
            {
                InputRecord = cprNumber
            };
            AN08010Response response = service.SubmitAN08010(param);
            ValidateReturnCode(response.OutputRecord.returkode, response.OutputRecord.returtekst);
            return new EnglishAN08010Response(response);
        }
    }

    namespace WS_AN08010
    {
        /// <summary>
        /// Contains the relation codes as returned fromDate web services
        /// </summary>
        public class RelationTypes
        {
            public const string Spouse = "Æ";
            public const string Partner = "P";
            public const string Baby = "B";
            public const string ChildOver18 = "O";
            public const string Parents = "F";
        }

        public class EnglishAN08010Response
        {
            public ReplyPerson[] OutputArrayRecord { get; private set; }
            public SVAR OutputRecord { get; private set; }

            public EnglishAN08010Response(AN08010Response innerResponse)
            {
                OutputRecord = innerResponse.OutputRecord;
                if (innerResponse.OutputArrayRecord != null)
                {
                    OutputArrayRecord = Array.ConvertAll<SVARPERSONER, ReplyPerson>(innerResponse.OutputArrayRecord, (p) => new ReplyPerson(p));
                }
            }

            public PersonFlerRelationType[] Filter(string typeFilter, Func<string, Guid> cpr2uuidFunc)
            {
                return OutputArrayRecord
                    .Where(
                        per => per.Type == typeFilter && !per.IsUnknown
                    )
                    .Select(per => new PersonFlerRelationType()
                        {
                            CommentText = null,
                            // TODO: Ensure PNR format is correct
                            ReferenceID = UnikIdType.Create(cpr2uuidFunc(per.PNR)),
                            Virkning = VirkningType.Create(null, null)
                        }
                    )
                    .ToArray();
            }
        }

        public class ReplyPerson
        {
            private SVARPERSONER InnerObject;

            public ReplyPerson(SVARPERSONER innerObject)
            {
                InnerObject = innerObject;
            }

            #region Extra properties

            public bool IsUnknown
            {
                get
                {
                    string personNumber = this.PNR.Replace("-", "");
                    return string.IsNullOrEmpty(personNumber);
                }
            }
            #endregion

            #region Wrapper properties
            public string Type
            {
                get
                {
                    return InnerObject.type_;
                }
            }

            public string PNR
            {
                get
                {
                    return InnerObject.personnummer;
                }
            }

            public string MaritalStatus
            {
                get
                {
                    return InnerObject.civilstand;
                }
            }

            public string Origin
            {
                get
                {
                    return InnerObject.oprindelse;
                }
            }

            public string Name
            {
                get
                {
                    return InnerObject.navn;
                }
            }

            public string Remark
            {
                get
                {
                    return InnerObject.bemaerkning;
                }
            }

            public string Status
            {
                get
                {
                    return InnerObject.status;
                }
            }
            #endregion
        }
    }
}
