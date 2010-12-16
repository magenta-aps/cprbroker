using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using CPRBroker.Engine;
using CPRBroker.Schemas;
using CPRBroker.Schemas.Part;

namespace CPRService.Services
{
    /// <summary>
    /// Summary description for Part
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/", Name = CPRBroker.Schemas.Part.ServiceNames.Part.Service, Description = CPRBroker.Schemas.Part.ServiceDescription.Part.Service)]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class Part : System.Web.Services.WebService
    {
        public ApplicationHeader applicationHeader;
        private const string ApplicationHeaderName = "applicationHeader";

        public QualityHeader qualityHeader = new QualityHeader();
        private const string QualityHeaderName = "qualityHeader";

        public EffectDateHeader effectDateHeader;
        private const string EffectDateHeaderName = "effectDateHeader";

        public RegistrationDateHeader registrationDateHeader;
        private const string RegistrationDateHeaderName = "registrationDateHeader";


        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(QualityHeaderName)]
        [SoapHeader(EffectDateHeaderName)]
        [SoapHeader(RegistrationDateHeaderName)]
        [WebMethod(MessageName = CPRBroker.Schemas.Part.ServiceNames.Part.Methods.Read, Description = CPRBroker.Schemas.Part.ServiceDescription.Part.Methods.Read)]
        public PersonRegistration Read(Guid personUUID)
        {
            return Manager.Part.Read(applicationHeader.UserToken, applicationHeader.ApplicationToken, personUUID, effectDateHeader.EffectDate, out qualityHeader.QualityLevel);
        }

        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(QualityHeaderName)]
        [SoapHeader(EffectDateHeaderName)]
        [SoapHeader(RegistrationDateHeaderName)]
        [WebMethod(MessageName = CPRBroker.Schemas.Part.ServiceNames.Part.Methods.List, Description = CPRBroker.Schemas.Part.ServiceDescription.Part.Methods.List)]
        public PersonRegistration[] List(Guid[] personUUIDs)
        {
            return null;
        }

        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(QualityHeaderName)]
        [SoapHeader(EffectDateHeaderName)]
        [SoapHeader(RegistrationDateHeaderName)]
        [WebMethod(MessageName = CPRBroker.Schemas.Part.ServiceNames.Part.Methods.Search, Description = CPRBroker.Schemas.Part.ServiceDescription.Part.Methods.Search)]
        public Guid[] Search(PersonSearchCriteria searchCriteria)
        {
            return Manager.Part.Search(applicationHeader.UserToken, applicationHeader.ApplicationToken, searchCriteria, effectDateHeader.EffectDate, out qualityHeader.QualityLevel);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CPRBroker.Schemas.Part.ServiceNames.Part.Methods.GetPersonUuid, Description = CPRBroker.Schemas.Part.ServiceDescription.Part.Methods.GetPersonUuid)]
        public Guid GetPersonUuid(string cprNumber)
        {
            return Manager.Part.GetPersonUuid(applicationHeader.UserToken, applicationHeader.ApplicationToken, cprNumber);
        }
    }

}
