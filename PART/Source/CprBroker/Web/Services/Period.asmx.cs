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
    /// Allows callers to explicitly request a filtered snapshot(s) of CPR data
    /// </summary>
    [WebService(
        Namespace = CprBroker.Schemas.Part.ServiceNames.Namespace,
        Name = "Period",
        Description = "Allows callers to explicitly request a filtered snapshot(s) of CPR data"
        )]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class Period : PartService
    {
        [WebMethod(Description = "Gets a single snapshot of one person's CPR data")]
        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(SourceUsageOrderHeaderName, Direction = SoapHeaderDirection.In)]
        public LaesOutputType ReadSnapshot(string UUID, DateTime virkningDato)
        {
            var sourceUsageOrder = SourceUsageOrderHeader.GetLocalDataProviderUsageOption(this.sourceUsageOrderHeader);
            return Manager.Period.ReadAtTime(UUID, virkningDato, applicationHeader.ApplicationToken, applicationHeader.UserToken, sourceUsageOrder);
        }

        [WebMethod(Description = "Gets all snapshots of one person's CPR data that fall between the input effect dates")]
        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(SourceUsageOrderHeaderName, Direction = SoapHeaderDirection.In)]
        public LaesOutputType ReadPeriod(string UUID, DateTime fraVirkningDato, DateTime tilVirkningDato)
        {
            var sourceUsageOrder = SourceUsageOrderHeader.GetLocalDataProviderUsageOption(this.sourceUsageOrderHeader);
            return Manager.Period.ReadPeriod(UUID, fraVirkningDato, tilVirkningDato, applicationHeader.ApplicationToken, applicationHeader.UserToken, sourceUsageOrder);
        }

        [WebMethod(Description = "Gets a single snapshot of the given persons' CPR data")]
        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(SourceUsageOrderHeaderName, Direction = SoapHeaderDirection.In)]
        public ListOutputType1 ListSnapshot(string[] UUIDs, DateTime virkningDato)
        {
            var sourceUsageOrder = SourceUsageOrderHeader.GetLocalDataProviderUsageOption(this.sourceUsageOrderHeader);
            return Manager.Period.ListAtTime(UUIDs, virkningDato, applicationHeader.ApplicationToken, applicationHeader.UserToken, sourceUsageOrder);
        }

        [WebMethod(Description = "Gets all snapshots of the given persons' CPR data that fall between the input effect dates")]
        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(SourceUsageOrderHeaderName, Direction = SoapHeaderDirection.In)]
        public ListOutputType1 ListPeriod(string[] UUIDs, DateTime fraVirkningDato, DateTime tilVirkningDato)
        {
            var sourceUsageOrder = SourceUsageOrderHeader.GetLocalDataProviderUsageOption(this.sourceUsageOrderHeader);
            return Manager.Period.ListPeriod(UUIDs, fraVirkningDato, tilVirkningDato, applicationHeader.ApplicationToken, applicationHeader.UserToken, sourceUsageOrder);
        }

    }
}
