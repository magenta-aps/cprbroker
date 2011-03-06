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
    [WebService(Namespace = CprBroker.Schemas.Part.ServiceNames.Namespace, Name = CprBroker.Schemas.Part.ServiceNames.Part.Service, Description = CprBroker.Schemas.ServiceDescription.Part.Service)]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class Part : System.Web.Services.WebService
    {
        public ApplicationHeader applicationHeader;
        private const string ApplicationHeaderName = "applicationHeader";

        public QualityHeader qualityHeader = new QualityHeader();
        private const string QualityHeaderName = "qualityHeader";

        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(QualityHeaderName, Direction = SoapHeaderDirection.Out)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Part.Methods.Read, Description = CprBroker.Schemas.ServiceDescription.Part.Methods.Read)]
        public LaesOutputType Read(LaesInputType input)
        {
            return Manager.Part.Read(applicationHeader.UserToken, applicationHeader.ApplicationToken, input, out qualityHeader.QualityLevel);
        }

        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(QualityHeaderName, Direction = SoapHeaderDirection.Out)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Part.Methods.RefreshRead, Description = CprBroker.Schemas.ServiceDescription.Part.Methods.RefreshRead)]
        public LaesOutputType RefreshRead(LaesInputType input)
        {
            return Manager.Part.RefreshRead(applicationHeader.UserToken, applicationHeader.ApplicationToken, input, out qualityHeader.QualityLevel);
        }

        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(QualityHeaderName, Direction = SoapHeaderDirection.Out)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Part.Methods.List, Description = CprBroker.Schemas.ServiceDescription.Part.Methods.List)]
        public ListOutputType1 List(ListInputType input)
        {
            return Manager.Part.List(applicationHeader.UserToken, applicationHeader.ApplicationToken, input, out qualityHeader.QualityLevel);
        }

        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(QualityHeaderName, Direction = SoapHeaderDirection.Out)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Part.Methods.Search, Description = CprBroker.Schemas.ServiceDescription.Part.Methods.Search)]
        public SoegOutputType Search(SoegInputType1 searchCriteria)
        {
            return Manager.Part.Search(applicationHeader.UserToken, applicationHeader.ApplicationToken, searchCriteria, out qualityHeader.QualityLevel);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Part.Methods.GetUuid, Description = CprBroker.Schemas.ServiceDescription.Part.Methods.GetUuid)]
        public GetUuidOutputType GetUuid(string cprNumber)
        {
            return Manager.Part.GetUuid(applicationHeader.UserToken, applicationHeader.ApplicationToken, cprNumber);
        }
    }

}
