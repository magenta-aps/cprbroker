using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CprBroker.Engine.Queues;
using CprBroker.Data.Queues;
using CprBroker.Schemas;
using CprBroker.Engine;
using CprBroker.Providers.Local.Search;
using CprBroker.Engine.Local;
using System.Threading;

namespace CprBroker.PartInterface.Tracking
{
    public class CleanupQueue : Engine.Queues.Queue<CleanupQueueItem>
    {
        public override CleanupQueueItem[] Process(CleanupQueueItem[] items)
        {
            var prov = new TrackingDataProvider();
            var uuids = items.Select(i => i.PersonUuid).ToArray();

            var brokerContext = BrokerContext.Current;

            var tasks = items
                .Select(
                (queueItem) => Task.Factory.StartNew(new Func<CleanupQueueItem>(() => ProcessItemWithMutex(brokerContext, prov, queueItem)))
                );

            return Task.WhenAll(tasks)
                .Result
                .Where(r => r != null)
                .ToArray();
        }

        public virtual CleanupQueueItem ProcessItemWithMutex(BrokerContext brokerContext, TrackingDataProvider prov, CleanupQueueItem queueItem)
        {
            BrokerContext.Current = brokerContext;
            Mutex personMutex = null;

            try
            {
                // Establish a person based critical section
                personMutex = new Mutex(false, queueItem.PersonUuid.ToString().ToUpper());
                personMutex.WaitOne();

                // Now the person is locked, all possible usage has been recorded                
                var fromDate = DateTime.Now - CleanupDetectionEnqueuer.MaxInactivePeriod;
                var dbrFromDate = fromDate + CleanupDetectionEnqueuer.DprEmulationRemovalAllowance;

                return ProcessItem(brokerContext, prov, queueItem, fromDate, dbrFromDate);
            }
            catch (Exception ex)
            {
                Admin.LogException(ex);
                return null;
            }
            finally
            {
                // Release the lock
                if (personMutex != null)
                    personMutex.ReleaseMutex();
            }
        }

        public virtual CleanupQueueItem ProcessItem(BrokerContext brokerContext, TrackingDataProvider prov, CleanupQueueItem queueItem, DateTime fromDate, DateTime dbrFromDate)
        {
            BrokerContext.Current = brokerContext;
            var personIdentifier = queueItem.ToPersonIdentifier();

            // First, make and log the decisions
            var removePerson = false;
            var removeDprEmulation = false;

            var personHasSubscribers = prov.PersonHasSubscribers(personIdentifier.UUID.Value);
            var personHasUsage = prov.PersonHasUsage(personIdentifier.UUID.Value, fromDate, null);

            if (personHasSubscribers == false && personHasUsage == false)
            {
                Func<string, int?> codeConverter = (string s) =>
                {
                    int retVal;
                    return int.TryParse(s, out retVal) ? retVal : (int?)null;
                };

                var municipalityCode = PersonSearchCache.GetValue<int?>(personIdentifier.UUID.Value, psc => codeConverter(psc.MunicipalityCode));
                var excludedMunicipalities = CleanupDetectionEnqueuer.ExcludedMunicipalityCodes
                    .Select(mc => codeConverter(mc))
                    .Where(mc => mc.HasValue && mc.Value > 0);

                Admin.LogFormattedSuccess(
                    "<{0}>: Checking excluded municipalities: person <{1}>, municipality <{2}>, excluded municipalities <{3}>",
                    this.GetType().Name,
                    personIdentifier.UUID,
                    municipalityCode,
                    string.Join(",", excludedMunicipalities)
                    );

                if (municipalityCode.HasValue && excludedMunicipalities.Contains(municipalityCode))
                {
                    // Do not remove
                    Admin.LogFormattedSuccess(
                        "Person <{0}> excluded from cleanup due to excluded municipality of residence",
                        personIdentifier.UUID);
                    return queueItem;
                }
                else
                {
                    removePerson = true;
                }
            }
            else if (personHasSubscribers == false && prov.PersonHasUsage(personIdentifier.UUID.Value, dbrFromDate, null) == false)
            {
                removeDprEmulation = true;
            }
            else
            {
                // Person should not be removed - considered a success
                return queueItem;
            }

            // Action time
            // Remove the person if needed
            if (removePerson)
            {
                Admin.LogFormattedSuccess("<{0}>:Removing unused person <{1}>", this.GetType().Name, personIdentifier.UUID);
                var task = prov.RemovePersonAsync(personIdentifier);
                task.Wait();
                var personRemoved = task.Result;
                if (personRemoved)
                    return queueItem;
                else
                    return null;
            }
            else if (removeDprEmulation)
            {
                // Only remove from DPR emulation
                Admin.LogFormattedSuccess("<{0}>:Removing semi-unused person <{1}> from DPR emulation", this.GetType().Name, personIdentifier.UUID);
                var task = prov.DeletePersonFromAllDBR(brokerContext, personIdentifier);
                task.Wait();
                var dbrRemoved = task.Result;
                if (dbrRemoved)
                    return queueItem;
                else
                    return null;
            }
            else
            {
                // Removal not needed - success
                return queueItem;
            }
        }
    }
}
