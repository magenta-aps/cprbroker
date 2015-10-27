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

            var ret = ToRegistreringType1(responses[0], responses[1], uuidCache.GetUuid);

            return ret;
        }

        public RegistreringType1 ToRegistreringType1(string stamPlusXml, string familyPlusXml, Func<string, Guid> uuidFunc)
        {
            var familyPlus_Relations = new FamilyPlusResponse(familyPlusXml);
            var familyPlus_Person = new FamilyPlusFirstRowResponse(familyPlusXml);

            // Merge with Stam+, to get the fields not in Family+
            familyPlus_Person.MainItem.Merge(new StamPlusResponse(stamPlusXml).RowItems.First());

            // Initial filling
            var ret = new RegistreringType1()
            {
                AttributListe = familyPlus_Person.MainItem.ToAttributListeType(),
                RelationListe = familyPlus_Relations.ToRelationListeType(uuidFunc),
                TilstandListe = new TilstandListeType()
                {
                    CivilStatus = familyPlus_Relations.ToCivilStatusType(),
                    LivStatus = familyPlus_Person.MainItem.ToLivStatusType()
                },
                Tidspunkt = TidspunktType.Create(DateTime.Now),
                LivscyklusKode = LivscyklusKodeType.Rettet,
                AktoerRef = UnikIdType.Create(Constants.ActorId),
                CommentText = null,
                SourceObjectsXml = Utilities.Strings.SerializeObject(new string[] { stamPlusXml, familyPlusXml })
            };
            
            // Result should not be stored locally
            ret.IsUpdatableLocally = false;

            return ret;
        }
    }
}
