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
            /// <summary>
            /// Converts the address to an OIO address by using Schemas.Util.Address class
            /// </summary>
            /// <param name="personCivilRegistrationStatusCodeType"></param>
            /// <returns></returns>
            public object ToAddressComplete(PersonCivilRegistrationStatusCodeType personCivilRegistrationStatusCodeType)
            {
                Schemas.Util.Address address = new CPRBroker.Schemas.Util.Address();
                var a = this.outputRecordField;
                address[CPRBroker.Schemas.Util.AddressField.Building] = outputRecordField.EBYG;
                address[CPRBroker.Schemas.Util.AddressField.CareOfName] = a.ACONVN;
                //address[CPRBroker.Schemas.Util.AddressField.CountryCode] = "";
                //address[CPRBroker.Schemas.Util.AddressField.CountryName] = "";
                //address[CPRBroker.Schemas.Util.AddressField.DistrictSubDivisionIdentifier] = "";
                address[CPRBroker.Schemas.Util.AddressField.Door] = a.ASIDOER;
                address[CPRBroker.Schemas.Util.AddressField.Floor] = a.AETAGE;
                address[CPRBroker.Schemas.Util.AddressField.HouseNumber] = a.AHUSNR;
                //address[CPRBroker.Schemas.Util.AddressField.Line1] = "";
                //address[CPRBroker.Schemas.Util.AddressField.Line2] = "";
                //address[CPRBroker.Schemas.Util.AddressField.Line3] = "";
                //address[CPRBroker.Schemas.Util.AddressField.Line4] = "";
                //address[CPRBroker.Schemas.Util.AddressField.Line5] = "";
                //address[CPRBroker.Schemas.Util.AddressField.Line6] = "";
                //address[CPRBroker.Schemas.Util.AddressField.LocationDescription] = "";
                //address[CPRBroker.Schemas.Util.AddressField.MailDeliverSubLocationIdentifier] = "";
                address[CPRBroker.Schemas.Util.AddressField.MunicipalityCode] = a.EKOM;
                //address[CPRBroker.Schemas.Util.AddressField.MunicipalityName] = "";
                address[CPRBroker.Schemas.Util.AddressField.PostBox] = "";
                address[CPRBroker.Schemas.Util.AddressField.PostCode] = a.CSTIL;
                //address[CPRBroker.Schemas.Util.AddressField.PostDistrict] = "";
                address[CPRBroker.Schemas.Util.AddressField.StreetCode] = a.CVEJ;
                //address[CPRBroker.Schemas.Util.AddressField.StreetName] = "";
                //address[CPRBroker.Schemas.Util.AddressField.StreetNameForAddressing] = "";

                return address.ToOioAddress(personCivilRegistrationStatusCodeType);
            }

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
    }
}
