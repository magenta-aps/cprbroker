using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Engine;
using CPRBroker.Providers.KMD.WS_AN08100;
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
        private WS_AN08100.EnglishAN08100Response CallAN08100(DateTime birthDate, Schemas.Part.Enums.Gender? gender)
        {
            // TODO: use correct service objects
            //var service = new CPRBroker.Providers.KMD.WS_AN08100.WS_AN08100();
            //SetServiceUrl(service, ServiceTypes.AN08100);

            //service.userinfoValue = new userinfo()
            //{
            //    userid = DatabaseObject.UserName,
            //    password = DatabaseObject.Password
            //};

            SearchData searchData = new SearchData()
            {
                DayFrom = birthDate.ToString("DD"),
                MonthFrom = birthDate.ToString("MM"),
                YearFrom = birthDate.ToString("YY"),
                CenturyFrom = birthDate.ToString("YYYY").Substring(0, 2),

                DayTo = birthDate.ToString("DD"),
                MonthTo = birthDate.ToString("MM"),
                YearTo = birthDate.ToString("YY"),
                CenturyTo = birthDate.ToString("YYYY").Substring(0, 2),

                Gender = FromPartGender(gender).ToString()
            };


            AN08100 param = new AN08100()
            {
                InputRecord = searchData.InnerResponse
            };

            // TODO: use correct response object
            var resp = new WS_AN08100.AN08100Response();
            ValidateReturnCode(resp.OutputRecord.RETURKODE.ToString(), resp.OutputRecord.RETURTEKST);

            return new EnglishAN08100Response(resp);
        }
    }
    namespace WS_AN08100
    {
        public class SearchData
        {
            public SOEGEDATA InnerResponse = new SOEGEDATA();

            public string DayFrom
            {
                get
                {
                    return this.InnerResponse.DAGFRA;
                }
                set
                {
                    this.InnerResponse.DAGFRA = value;
                }
            }

            public string DayTo
            {
                get
                {
                    return this.InnerResponse.DAGTIL;
                }
                set
                {
                    this.InnerResponse.DAGTIL = value;
                }
            }

            public string MonthFrom
            {
                get
                {
                    return this.InnerResponse.MAANEDFRA;
                }
                set
                {
                    this.InnerResponse.MAANEDFRA = value;
                }
            }

            public string MonthTo
            {
                get
                {
                    return this.InnerResponse.MAANEDTIL;
                }
                set
                {
                    this.InnerResponse.MAANEDTIL = value;
                }
            }

            public string YearFrom
            {
                get
                {
                    return this.InnerResponse.AARFRA;
                }
                set
                {
                    this.InnerResponse.AARFRA = value;
                }
            }

            public string YearTo
            {
                get
                {
                    return this.InnerResponse.AARTIL;
                }
                set
                {
                    this.InnerResponse.AARTIL = value;
                }
            }

            public string CenturyFrom
            {
                get
                {
                    return this.InnerResponse.AARHUNDFRA;
                }
                set
                {
                    this.InnerResponse.AARHUNDFRA = value;
                }
            }

            public string CenturyTo
            {
                get
                {
                    return this.InnerResponse.AARHUNDTIL;
                }
                set
                {
                    this.InnerResponse.AARHUNDTIL = value;
                }
            }

            public string STATUS
            {
                get
                {
                    return this.InnerResponse.STATUS;
                }
                set
                {
                    this.InnerResponse.STATUS = value;
                }
            }

            public string Gender
            {
                get
                {
                    return this.InnerResponse.KOEN;
                }
                set
                {
                    this.InnerResponse.KOEN = value;
                }
            }
        }

        public class EnglishAN08100Response
        {
            public EnglishAN08100Response(AN08100Response innerResponse)
            {
                OutputRecord = innerResponse.OutputRecord;
                OutputArrayRecord = Array.ConvertAll<SVARPERSONER, ReplyPeople>(innerResponse.OutputArrayRecord, (p) => new ReplyPeople(p));
            }

            public ReplyPeople[] OutputArrayRecord { get; private set; }
            public SVAR OutputRecord { get; private set; }
        }
        public class ReplyPeople
        {
            public SVARPERSONER InnerObject;

            public ReplyPeople(SVARPERSONER innerObject)
            {
                InnerObject = innerObject;
            }

            public string PNR
            {
                get
                {
                    return this.InnerObject.PERSONNUMMER;
                }
            }

            public string AddressingName
            {
                get
                {
                    return this.InnerObject.ADRESSERINGSNAVN;
                }
            }

            public string Address
            {
                get
                {
                    return this.InnerObject.ADRESSE;
                }
            }

            public string LastPersonIncidentCode
            {
                get
                {
                    return this.InnerObject.BEMAERKNING;
                }
            }
        }
    }
}
