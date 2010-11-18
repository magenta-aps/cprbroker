using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Engine;
using CPRBroker.Providers.KMD.WS_AN08010;
using CPRBroker.Schemas;

namespace CPRBroker.Providers.KMD
{
    public partial class KmdDataProvider
    {
        /// <summary>
        /// Contains the relation codes as returned from web services
        /// </summary>
        private class RelationTypes
        {
            public const string Spouse = "Æ";
            public const string Partner = "P";
            public const string Baby = "B";
            public const string ChildOver18 = "O";
            public const string Parents = "F";
        }

        /// <summary>
        /// Calls the AN08010 web service
        /// </summary>
        /// <param name="cprNumber"></param>
        /// <returns></returns>
        private EnglishAN08010Response CallAN08010(string cprNumber)
        {
            WS_AN08010.WS_AN08010 service = new CPRBroker.Providers.KMD.WS_AN08010.WS_AN08010();
            SetServiceUrl(service, ServiceTypes.AN08010);
            service.userinfoValue = new userinfo()
            {
                userid = DatabaseObject.UserName,
                password = DatabaseObject.Password
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
        public class EnglishAN08010Response
        {
            public ReplyPerson[] OutputArrayRecord { get; private set; }
            public SVAR OutputRecord { get; private set; }

            public EnglishAN08010Response(AN08010Response innerResponse)
            {
                OutputRecord = innerResponse.OutputRecord;
                OutputArrayRecord = Array.ConvertAll<SVARPERSONER, ReplyPerson>(innerResponse.OutputArrayRecord, (p) => new ReplyPerson(p));
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
            public SimpleCPRPersonType ToSimpleCprPerson()
            {
                return new SimpleCPRPersonType()
                {
                    PersonCivilRegistrationIdentifier = this.PNR.Replace("-", ""),
                    PersonNameStructure = new PersonNameStructureType(this.Name)
                };
            }

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
