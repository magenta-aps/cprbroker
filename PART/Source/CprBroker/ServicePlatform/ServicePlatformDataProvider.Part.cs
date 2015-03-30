using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.PartInterface;
using CprBroker.Schemas.Part;
using CprBroker.Providers.CprServices;
using CprBroker.Engine.Local;
using CprBroker.Engine.Part;

namespace CprBroker.Providers.ServicePlatform
{
    public partial class ServicePlatformDataProvider : CprBroker.Engine.IPartSearchListDataProvider
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

                var kvit = CallGctpService(Constants.ServiceUuid.ADRSOG1, xml, out xmlOut);
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
    }
}
