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
                catch (Exception ex)
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

            var allInfos = new ServiceInfo[] {                 
                ServiceInfo.StamPlus_Local, // Although Stam is included in Familie+, Spam+ contains street and post district names (not in Familie+)
                ServiceInfo.FamilyPlus_Local,
            };

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
            var familyPlus = new FamilyPlusResponse(responses[1]);

            // UUID mappings
            var uuidCache = new UuidCache();
            var pnrs = familyPlus.ToRelationPNRs().ToList();
            pnrs.Add(uuid.CprNumber);
            uuidCache.FillCache(pnrs.ToArray());

            var ret = ToRegistreringType1(stamPlus, familyPlus, responses.ToArray(), uuidCache.GetUuid);

            return ret;
        }

        public RegistreringType1 ToRegistreringType1(StamPlusResponse stamPlus, FamilyPlusResponse familyPlus, string[] sourceXmlStrings, Func<string, Guid> uuidFunc)
        {
            // Initial filling
            var ret = new RegistreringType1()
            {
                AttributListe = stamPlus.RowItems.First().ToAttributListeType(),
                RelationListe = familyPlus.ToRelationListeType(uuidFunc),
                TilstandListe = new TilstandListeType()
                {
                    CivilStatus = familyPlus.ToCivilStatusType(),
                    LivStatus = stamPlus.RowItems.First().ToLivStatusType()
                },
                Tidspunkt = TidspunktType.Create(DateTime.Now),
                LivscyklusKode = LivscyklusKodeType.Rettet,
                AktoerRef = UnikIdType.Create(Constants.ActorId),
                CommentText = null,
                SourceObjectsXml = sourceXmlStrings != null ? Utilities.Strings.SerializeObject(sourceXmlStrings) : null
            };


            // override name start date
            var dates = new DateTime?[]{
                familyPlus.ToNameStartDate(),
                ret.AttributListe.Egenskab.First().Virkning.FraTidspunkt.ToDateTime()
            };
            var maxDate = dates.Where(d => d.HasValue).OrderByDescending(d => d.Value).FirstOrDefault();
            ret.AttributListe.Egenskab.First().Virkning.FraTidspunkt = TidspunktType.Create(maxDate);

            // override birthdate
            ret.AttributListe.Egenskab.First().BirthDate = familyPlus.ToBirthdate().Value;

            // Result should not be stored locally
            ret.IsUpdatableLocally = false;

            return ret;
        }
    }
}
