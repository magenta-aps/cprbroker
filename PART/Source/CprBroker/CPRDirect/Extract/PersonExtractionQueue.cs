using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Data.Queues;
using CprBroker.Engine.Local;

namespace CprBroker.Providers.CPRDirect
{
    public class PersonExtractionQueue : Data.Queues.Queue<CprDirectExtractQueueItem>
    {
        public static readonly Guid FixedQueueId = new Guid("{474A4EDE-A749-4CE1-9E06-227873FF9D50}");

        public PersonExtractionQueue()
            : base(FixedQueueId)
        {

        }

        public override CprDirectExtractQueueItem[] Process(CprDirectExtractQueueItem[] items)
        {
            var succeeded = new List<CprDirectExtractQueueItem>();
            var cache = new CprBroker.Engine.Part.UuidCache();
            var pnrs = items.Select(i => i.PNR).ToArray();
            using (var dataContext = new ExtractDataContext())
            {
                var allPersons = items.Select(queueItem =>
                    new
                    {
                        QueueItem = queueItem,
                        Extract = dataContext.Extracts.Where(ex => ex.ExtractId == queueItem.ExtractId).Single(),
                        ExtractItems = dataContext.ExtractItems.Where(ei => ei.ExtractId == queueItem.ExtractId && ei.PNR == queueItem.PNR)
                    })
                    .ToArray();

                // Cahce UUIDS
                var allPnrs = items.Select(qi => qi.PNR).ToList();
                allPnrs.AddRange(allPersons.SelectMany(qi => qi.ExtractItems.Select(ei => ei.RelationPNR)));
                allPnrs.AddRange(allPersons.SelectMany(qi => qi.ExtractItems.Select(ei => ei.RelationPNR2)));
                allPnrs = allPnrs
                    .Distinct()
                    .Select(pnr => CprBroker.Providers.CPRDirect.Converters.ToPnrStringOrNull(pnr))
                    .Where(pnr => !string.IsNullOrEmpty(pnr))
                    .ToList();
                cache.FillCache(allPnrs.ToArray());

                foreach (var person in allPersons)
                {
                    try
                    {
                        var uuid = cache.GetUuid(person.QueueItem.PNR);
                        var response = Extract.ToIndividualResponseType(person.Extract, person.ExtractItems.AsQueryable(), Constants.DataObjectMap);
                        var oioPerson = response.ToRegistreringType1(cache.GetUuid);
                        var personIdentifier = new Schemas.PersonIdentifier() { CprNumber = person.QueueItem.PNR, UUID = uuid };
                        UpdateDatabase.UpdatePersonRegistration(personIdentifier, oioPerson);

                        succeeded.Add(person.QueueItem);
                    }
                    catch (Exception ex)
                    {
                        Admin.LogException(ex);
                    }
                }
            }
            return succeeded.ToArray();
        }
    }
}
