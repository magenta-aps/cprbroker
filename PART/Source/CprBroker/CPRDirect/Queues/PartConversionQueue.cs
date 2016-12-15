using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Data.Queues;
using CprBroker.Engine.Local;
using System.Threading;

namespace CprBroker.Providers.CPRDirect
{
    public class PartConversionQueue : Engine.Queues.Queue<ExtractQueueItem>
    {
        public const int QueueTypeId = 101;

        public override ExtractQueueItem[] Process(ExtractQueueItem[] items)
        {
            var succeeded = new List<ExtractQueueItem>();
            var cache = new CprBroker.Engine.Part.UuidCache();
            var pnrs = items.Select(i => i.PNR).ToArray();
            using (var dataContext = new ExtractDataContext())
            {
                items.LoadExtractAndItems(dataContext);

                // Cahce UUIDS
                var allPnrs = items.Select(qi => qi.PNR).ToList();
                allPnrs.AddRange(items.SelectMany(qi => qi.ExtractItems.Select(ei => ei.RelationPNR)));
                allPnrs.AddRange(items.SelectMany(qi => qi.ExtractItems.Select(ei => ei.RelationPNR2)));
                allPnrs = allPnrs
                    .Distinct()
                    .Select(pnr => CprBroker.Providers.CPRDirect.Converters.ToPnrStringOrNull(pnr))
                    .Where(pnr => !string.IsNullOrEmpty(pnr))
                    .ToList();
                cache.FillCache(allPnrs.ToArray());

                foreach (var person in items)
                {
                    Mutex personMutex = null;
                    try
                    {
                        var uuid = cache.GetUuid(person.PNR);
                        
                        // Establish a person based critical section
                        personMutex = new Mutex(false, CprBroker.Utilities.Strings.GuidToString(uuid));
                        personMutex.WaitOne();

                        var response = Extract.ToIndividualResponseType(person.Extract, person.ExtractItems.AsQueryable(), Constants.DataObjectMap);
                        var oioPerson = response.ToRegistreringType1(cache.GetUuid);
                        var personIdentifier = new Schemas.PersonIdentifier() { CprNumber = person.PNR, UUID = uuid };
                        UpdateDatabase.UpdatePersonRegistration(personIdentifier, oioPerson);

                        succeeded.Add(person);
                    }
                    catch (Exception ex)
                    {
                        Admin.LogException(ex);
                    }
                    finally
                    {
                        // Release the lock
                        if (personMutex != null)
                            personMutex.ReleaseMutex();
                    }
                }
            }
            return succeeded.ToArray();
        }
    }
}
