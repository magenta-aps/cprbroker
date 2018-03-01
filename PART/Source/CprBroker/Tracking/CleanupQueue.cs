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
using CprBroker.Slet;
using System.Threading;

namespace CprBroker.PartInterface.Tracking
{
    public class CleanupQueue : Engine.Queues.Queue<CleanupQueueItem>
    {
        public const int QueueTypeId = 400;

        public override CleanupQueueItem[] Process(CleanupQueueItem[] items)
        {
            var uuids = items.Select(i => i.removePersonItem.PersonUuid).ToArray();

            var brokerContext = BrokerContext.Current;

            var tasks = items
                .Select(
                (queueItem) => Task.Factory.StartNew(new Func<CleanupQueueItem>(() => ProcessItemWithMutex(brokerContext, queueItem)))
                );

            return Task.WhenAll(tasks)
                .Result
                .Where(r => r != null)
                .ToArray();
        }

        public virtual CleanupQueueItem ProcessItemWithMutex(BrokerContext brokerContext, CleanupQueueItem queueItem)
        {
            BrokerContext.Current = brokerContext;
            PersonRemover personRemover = new PersonRemover();
            Mutex personMutex = null;

            try
            {
                // Establish a person based critical section
                personMutex = new Mutex(false, Utilities.Strings.GuidToString(queueItem.removePersonItem.PersonUuid));
                personMutex.WaitOne();

                // Now the person is locked, all possible usage has been recorded                
                var fromDate = DateTime.Now - SettingsUtilities.MaxInactivePeriod;
                var dbrFromDate = fromDate + SettingsUtilities.DprEmulationRemovalAllowance;
                var excludedMunicipalityCodes = SettingsUtilities.ExcludedMunicipalityCodes;

                return personRemover.RemovePerson(brokerContext, queueItem, fromDate, dbrFromDate, excludedMunicipalityCodes);
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
    }
}
