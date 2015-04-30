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
using CprBroker.Providers.CprServices.Responses;
using CprBroker.Providers.ServicePlatform.Responses;

namespace CprBroker.Providers.ServicePlatform
{
    public partial class ServicePlatformDataProvider : IPutSubscriptionDataProvider, IPartReadDataProvider
    {
        public bool PutSubscription(Schemas.PersonIdentifier personIdentifier)
        {
            var service = CreateService<CprSubscriptionService.CprSubscriptionWebServicePortType, CprSubscriptionService.CprSubscriptionWebServicePortTypeClient>(ServiceInfo.CPRSubscription);
            using (var callContext = this.BeginCall("AddPNRSubscription", personIdentifier.CprNumber))
            {
                try
                {
                    var request = new CprSubscriptionService.AddPNRSubscriptionType()
                    {
                        InvocationContext = GetInvocationContext<CprSubscriptionService.InvocationContextType>(ServiceInfo.CPRSubscription.UUID),
                        PNR = personIdentifier.CprNumber
                    };
                    var ret = service.AddPNRSubscription(request);
                    callContext.Succeed();
                    return true;
                }
                catch(Exception ex)
                {
                    Admin.LogException(ex);
                    callContext.Fail();
                    return false;
                }
                //object o = ret.Result;
                return true;
            }
        }

        public RegistreringType1 Read(Schemas.PersonIdentifier uuid, LaesInputType input, Func<string, Guid> cpr2uuidFunc, out Schemas.QualityLevel? ql)
        {
            ql = Schemas.QualityLevel.DataProvider;

            var request = new SearchRequest(uuid.CprNumber);

            var allInfos = new ServiceInfo[] { ServiceInfo.StamPlus_Local, ServiceInfo.NAVNE3_Local, ServiceInfo.FamilyPlus_Local };
            var responses = new List<string>();
            foreach (var m in allInfos)
            {
                var searchMethod = m.ToSearchMethod();
                var plan = new SearchPlan(request, true, searchMethod);
                var gctpMessage = plan.PlannedCalls.First().ToRequestXml(CprServices.Properties.Resources.SearchTemplate);

                string retXml;
                var kvit = CallGctpService(m, gctpMessage, out retXml);
                if (kvit.OK)
                {
                    responses.Add(retXml);
                }
                else
                {
                    //Error
                    Admin.LogFormattedError("GCTP <{0}> Failed with <{1}><{2}>. Input <{3}>", searchMethod.Name, kvit.ReturnCode, kvit.ReturnText, uuid.CprNumber);
                    return null;
                }
            }

            // Now we are sure that all calls have succeeded
            var stamPlus = new StamPlusResponse(responses[0]);
            //var navne3 = new Navne3Response(responses[1]);
            var familyPlus = new FamilyPlusResponse(responses[2]);


            // Initial filling
            var ret = stamPlus.RowItems.First().ToRegistreringType1();
            
            // Result should not be stored locally
            ret.IsUpdatableLocally = false;

            // UUID mappings
            var uuidCache = new UuidCache();
            var pnrs = familyPlus.ToRelationPNRs().ToList();
            pnrs.Add(uuid.CprNumber);
            uuidCache.FillCache(pnrs.ToArray());

            // Overwritten properties
            ret.RelationListe = familyPlus.ToRelationListeType(uuidCache.GetUuid);
            ret.TilstandListe = new TilstandListeType()
            {
                CivilStatus = familyPlus.ToCivilStatusType(),
                LivStatus = stamPlus.RowItems.First().ToLivStatusType()
            };
            ret.AktoerRef = UnikIdType.Create(Constants.ActorId);
            ret.SourceObjectsXml = Utilities.Strings.SerializeObject(responses.ToArray());

            return ret;
        }
    }
}
