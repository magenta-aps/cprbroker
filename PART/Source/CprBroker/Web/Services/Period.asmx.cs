using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Engine;

namespace CprBroker.Web.Services
{
    /// <summary>
    /// Summary description for Period
    /// </summary>
    [WebService(Namespace = CprBroker.Schemas.Part.ServiceNames.Namespace, Name = CprBroker.Schemas.Part.ServiceNames.Period.Service
        //, Description = CprBroker.Schemas.ServiceDescription.Part.Service
        )]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class Period : PartService
    {
        [WebMethod]
        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(SourceUsageOrderHeaderName, Direction = SoapHeaderDirection.In)]
        public LaesOutputType ReadSnapshot(string UUID, DateTime virkningDato)
        {
            var sourceUsageOrder = SourceUsageOrderHeader.GetLocalDataProviderUsageOption(this.sourceUsageOrderHeader);
            return Manager.Period.ReadAtTime(UUID, virkningDato, applicationHeader.ApplicationToken, applicationHeader.UserToken, sourceUsageOrder);
        }

        [WebMethod]
        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(SourceUsageOrderHeaderName, Direction = SoapHeaderDirection.In)]
        public LaesOutputType ReadPeriod(string UUID, DateTime fraVirkningDato, DateTime tilVirkningDato)
        {
            var sourceUsageOrder = SourceUsageOrderHeader.GetLocalDataProviderUsageOption(this.sourceUsageOrderHeader);
            return Manager.Period.ReadPeriod(UUID, fraVirkningDato, tilVirkningDato, applicationHeader.ApplicationToken, applicationHeader.UserToken, sourceUsageOrder);
        }

        [WebMethod]
        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(SourceUsageOrderHeaderName, Direction = SoapHeaderDirection.In)]
        public ListOutputType1 ListSnapshot(string[] UUIDs, DateTime virkningDato)
        {
            var sourceUsageOrder = SourceUsageOrderHeader.GetLocalDataProviderUsageOption(this.sourceUsageOrderHeader);
            return Manager.Period.ListAtTime(UUIDs, virkningDato, applicationHeader.ApplicationToken, applicationHeader.UserToken, sourceUsageOrder);
        }

        [WebMethod]
        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(SourceUsageOrderHeaderName, Direction = SoapHeaderDirection.In)]
        public ListOutputType1 ListPeriod(string[] UUIDs, DateTime fraVirkningDato, DateTime tilVirkningDato)
        {
            var sourceUsageOrder = SourceUsageOrderHeader.GetLocalDataProviderUsageOption(this.sourceUsageOrderHeader);
            return Manager.Period.ListPeriod(UUIDs, fraVirkningDato, tilVirkningDato, applicationHeader.ApplicationToken, applicationHeader.UserToken, sourceUsageOrder);
        }

    }
}
