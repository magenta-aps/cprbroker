using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using CPRBroker.Schemas.Part;
using System.Web.Services.Protocols;

namespace CPRService.Services
{
    /// <summary>
    /// Summary description for Part
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/", Name = ServiceNames.Part.Service, Description = ServiceDescription.Part.Service)]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class Part : System.Web.Services.WebService
    {
        public EffectDateHeader effectDateHeader;
        public RegistrationDateHeader registrationDateHeader;

        private const string EffectDateHeaderName = "effectDateHeader";
        private const string RegistrationDateHeaderName = "registrationDateHeader";

        [WebMethod(MessageName = ServiceNames.Part.Methods.Read, Description = ServiceDescription.Part.Methods.Read)]
        [SoapHeader(EffectDateHeaderName)]
        public PersonRegistration Read(Guid personUUID)
        {
            return null;
        }

        [WebMethod(MessageName = ServiceNames.Part.Methods.List, Description = ServiceDescription.Part.Methods.List)]
        [SoapHeader(RegistrationDateHeaderName)]
        [SoapHeader(EffectDateHeaderName)]
        public PersonRegistration[] List(Guid[] personUUIDs)
        {
            return null;
        }

        [WebMethod(MessageName = ServiceNames.Part.Methods.Search, Description = ServiceDescription.Part.Methods.Search)]
        [SoapHeader(RegistrationDateHeaderName)]
        [SoapHeader(EffectDateHeaderName)]
        public Guid[] Search(PersonSearchCriteria searchCriteria)
        {
            return null;
        }
    }

}
