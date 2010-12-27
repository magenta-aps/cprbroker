using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Web.Services
{
    /// <summary>
    /// Summary description for Part
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/", Name = CprBroker.Schemas.Part.ServiceNames.Part.Service, Description = CprBroker.Schemas.Part.ServiceDescription.Part.Service)]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class Part : System.Web.Services.WebService
    {
        public ApplicationHeader applicationHeader;
        private const string ApplicationHeaderName = "applicationHeader";

        public QualityHeader qualityHeader = new QualityHeader();
        private const string QualityHeaderName = "qualityHeader";

        // TODO: Remove unnecessary effect date header
        public EffectDateHeader effectDateHeader;
        private const string EffectDateHeaderName = "effectDateHeader";

        public RegistrationDateHeader registrationDateHeader;
        private const string RegistrationDateHeaderName = "registrationDateHeader";


        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(QualityHeaderName, Direction = SoapHeaderDirection.Out)]
        [SoapHeader(EffectDateHeaderName)]
        [SoapHeader(RegistrationDateHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Part.Methods.Read, Description = CprBroker.Schemas.Part.ServiceDescription.Part.Methods.Read)]
        public PersonRegistration Read(Guid personUUID)
        {
            return Manager.Part.Read(applicationHeader.UserToken, applicationHeader.ApplicationToken, personUUID, effectDateHeader.EffectDate, out qualityHeader.QualityLevel);
        }

        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(QualityHeaderName, Direction = SoapHeaderDirection.Out)]
        [SoapHeader(EffectDateHeaderName)]
        [SoapHeader(RegistrationDateHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Part.Methods.RefreshRead, Description = CprBroker.Schemas.Part.ServiceDescription.Part.Methods.RefreshRead)]
        public PersonRegistration RefreshRead(Guid personUUID)
        {
            return Manager.Part.RefreshRead(applicationHeader.UserToken, applicationHeader.ApplicationToken, personUUID, effectDateHeader.EffectDate, out qualityHeader.QualityLevel);
        }

        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(QualityHeaderName, Direction = SoapHeaderDirection.Out)]
        [SoapHeader(EffectDateHeaderName)]
        [SoapHeader(RegistrationDateHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Part.Methods.List, Description = CprBroker.Schemas.Part.ServiceDescription.Part.Methods.List)]
        public PersonRegistration[] List(Guid[] personUUIDs)
        {
            return null;
        }

        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(QualityHeaderName, Direction = SoapHeaderDirection.Out)]
        [SoapHeader(EffectDateHeaderName)]
        [SoapHeader(RegistrationDateHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Part.Methods.Search, Description = CprBroker.Schemas.Part.ServiceDescription.Part.Methods.Search)]
        public Guid[] Search(PersonSearchCriteria searchCriteria)
        {
            return Manager.Part.Search(applicationHeader.UserToken, applicationHeader.ApplicationToken, searchCriteria, effectDateHeader.EffectDate, out qualityHeader.QualityLevel);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Part.Methods.GetPersonUuid, Description = CprBroker.Schemas.Part.ServiceDescription.Part.Methods.GetPersonUuid)]
        public Guid GetPersonUuid(string cprNumber)
        {
            return Manager.Part.GetPersonUuid(applicationHeader.UserToken, applicationHeader.ApplicationToken, cprNumber);
        }
    }

}
