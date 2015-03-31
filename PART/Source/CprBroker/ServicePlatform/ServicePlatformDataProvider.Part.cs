using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.PartInterface;
using CprBroker.Schemas.Part;
using CprBroker.Providers.CprServices;
using CprBroker.Engine.Local;
using CprBroker.Engine.Part;
using CprBroker.Engine;

namespace CprBroker.Providers.ServicePlatform
{
    public partial class ServicePlatformDataProvider : IPartSearchListDataProvider, IPutSubscriptionDataProvider, IPartReadDataProvider
    {
        public LaesResultatType[] SearchList(SoegInputType1 searchCriteria)
        {
            var request = new SearchRequest(searchCriteria.SoegObjekt.SoegAttributListe);
            var searchMethod = new SearchMethod(CprServices.Properties.Resources.ADRSOG1);
            var plan = new SearchPlan(request, searchMethod);

            List<SearchPerson> ret = null;

            if (plan.IsSatisfactory)
            {
                bool searchOk = true;
                var call = plan.PlannedCalls.First();
                var xml = call.ToRequestXml(CprServices.Properties.Resources.SearchTemplate);
                var xmlOut = "";

                var kvit = CallGctpService(ServiceInfo.ADRSOG1, xml, out xmlOut);
                if (kvit.OK)
                {
                    ret = call.ParseResponse(xmlOut, true);
                }
                else
                {
                    searchOk = false;
                    string callInput = string.Join(",", call.InputFields.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)).ToArray());
                    Admin.LogFormattedError("GCTP <{0}> Failed with <{1}><{2}>. Input <{3}>", call.Name, kvit.ReturnCode, kvit.ReturnText, callInput);
                }

                if (searchOk)
                {
                    // TODO: Can this break the result? is UUID assignment necessary?
                    var cache = new UuidCache();
                    var pnrs = ret.Select(p => p.PNR).ToArray();
                    cache.FillCache(pnrs);

                    return ret.Select(p => p.ToLaesResultatType(cache.GetUuid)).ToArray();
                }
                else
                {
                    // TODO: What to do if search fails??
                }
            }
            else
            {
                string searchFields = string.Join(",", request.CriteriaFields.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)).ToArray());
                Admin.LogFormattedError("Insufficient GCTP search criteria <{0}>", searchFields);
            }
            return null;
        }

        public bool PutSubscription(Schemas.PersonIdentifier personIdentifier)
        {
            var service = CreateService<CprSubscriptionService.CprSubscriptionWebServicePortType, CprSubscriptionService.CprSubscriptionWebServicePortTypeClient>(ServiceInfo.CPRSubscription);
            using (var callContext = this.BeginCall("", ""))
            {
                var request = new CprSubscriptionService.AddPNRSubscriptionType()
                {
                    InvocationContext = GetInvocationContext<CprSubscriptionService.InvocationContextType>(ServiceInfo.CPRSubscription.UUID),
                    PNR = personIdentifier.CprNumber
                };
                var ret = service.AddPNRSubscription(request);
                object o = ret;
                return true;
            }
        }

        public RegistreringType1 Read(Schemas.PersonIdentifier uuid, LaesInputType input, Func<string, Guid> cpr2uuidFunc, out Schemas.QualityLevel? ql)
        {
            var request = new SearchRequest(uuid.CprNumber);

            var allInfos = new ServiceInfo[] { ServiceInfo.StamPlus_Local, ServiceInfo.NAVNE3_Local, ServiceInfo.FamilyPlus_Local };

            foreach (var m in allInfos)
            {
                var searchMethod = m.ToSearchMethod();
                var plan = new SearchPlan(request, true, searchMethod);
                var gctpMessage = plan.PlannedCalls.First().ToRequestXml(CprServices.Properties.Resources.SearchTemplate);

                string retXml;
                var kvit = CallGctpService(m, gctpMessage, out retXml);
                if (kvit.OK)
                {
                    // OK
                }
                else
                {
                    object o = "";
                }
            }



            ql = Schemas.QualityLevel.DataProvider;
            return null;
        }
    }
}
