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
        private AN08010Response CallAN08010(string cprNumber)
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
            return response;
        }
    }
    namespace WS_AN08010
    {
        /// <summary>
        /// Extends the class by adding ToSimpleCprPerson() method
        /// </summary>
        public partial class SVARPERSONER
        {
            public SimpleCPRPersonType ToSimpleCprPerson()
            {
                return new SimpleCPRPersonType()
                {
                    PersonCivilRegistrationIdentifier = this.personnummer.Replace("-", ""),
                    PersonNameStructure = new PersonNameStructureType(this.navn)
                };
            }
            public bool IsUnknown
            {
                get 
                {
                    string personNumber = this.personnummer.Replace("-", "");
                    return string.IsNullOrEmpty(personNumber);
                }
            }
        }
    }
}
