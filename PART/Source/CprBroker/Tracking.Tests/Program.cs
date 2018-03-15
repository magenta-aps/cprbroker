using CprBroker.Data.Applications;
using CprBroker.Engine;
using CprBroker.PartInterface.Tracking;
using CprBroker.Schemas;
using CprBroker.Tests.PartInterface;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CprBroker.Slet;

namespace CprBroker.Tests.Tracking
{

    public class Program
    {
        public static void Main(string[] args)
        {
            switch (args.First().ToLower())
            {
                case "enumerate":
                    Enumerate(args.Skip(1).ToArray());
                    break;

                case "enumeratebyuuid":
                    EnumerateByUuid(args.Skip(1).ToArray());
                    break;

                case "decisions":
                    Decisions(args.Skip(1).ToArray());
                    break;
                default:
                    Console.WriteLine("Unknown option :)");
                    break;
            }
        }

        public static void Enumerate(string[] args)
        {
            var startIndex = 0;
            if (args.Length > 0)
                startIndex = int.Parse(args[0]);
            var BatchSize = 1000;
            if (args.Length > 1)
                BatchSize = int.Parse(args[1]);

            var foundUuids = new PersonIdentifier[0];
            var maximumUsageDate = DateTime.Now;
            var minimumUsageDate = maximumUsageDate - SettingsUtilities.MaxInactivePeriod;


            do
            {
                var prov = new TrackingDataProvider();
                Console.WriteLine("{0} : start index <{1}>, batch size <{2}>", DateTime.Now, startIndex, BatchSize);
                foundUuids = prov.EnumeratePersons(startIndex, BatchSize);

                var queueItems = foundUuids
                    .Select(uuid => new CleanupQueueItem() { removePersonItem = new Slet.RemovePersonItem(uuid.UUID.Value, uuid.CprNumber) })
                    .ToArray();
                Console.WriteLine("{0} : Found <{1}> persons", DateTime.Now, queueItems.Count());
                Console.WriteLine("First <{0}>, last <{1}>", foundUuids.First().CprNumber, foundUuids.Last().CprNumber);

                startIndex += BatchSize;

            } while (foundUuids.Length == BatchSize);
        }

        public static void EnumerateByUuid(string[] args)
        {
            var BatchSize = 1000;
            if (args.Length > 0)
                BatchSize = int.Parse(args[0]);

            var foundUuids = new PersonIdentifier[0];
            var maximumUsageDate = DateTime.Now;
            var minimumUsageDate = maximumUsageDate - SettingsUtilities.MaxInactivePeriod;
            Guid? startUuid = null;

            do
            {
                var prov = new TrackingDataProvider();
                Console.WriteLine("{0} : start uuid <{1}>, batch size <{2}>", DateTime.Now, startUuid, BatchSize);
                foundUuids = prov.EnumeratePersons(startUuid, BatchSize);

                var queueItems = foundUuids
                    .Select(uuid => new CleanupQueueItem() { removePersonItem = new RemovePersonItem(uuid.UUID.Value, uuid.CprNumber)})
                    .ToArray();
                Console.WriteLine("{0} : Found <{1}> persons", DateTime.Now, queueItems.Count());
                Console.WriteLine("First <{0}>, last <{1}>", foundUuids.First().CprNumber, foundUuids.Last().CprNumber);

                startUuid = foundUuids.LastOrDefault()?.UUID;

            } while (foundUuids.Length == BatchSize);
        }

        public static void Decisions(string[] args)
        {
            var startIndex = 0;
            if (args.Length > 0)
                startIndex = int.Parse(args[0]);
            var BatchSize = 1000;
            if (args.Length > 1)
                BatchSize = int.Parse(args[1]);

            Console.WriteLine("{0} : start index <{1}>, batch size <{2}>", DateTime.Now, startIndex, BatchSize);

            var prov = new TrackingDataProvider();
            var foundUuids = prov.EnumeratePersons(startIndex, BatchSize);
            var queueItems = foundUuids
                    .Select(uuid => new CleanupQueueItem() { removePersonItem = new RemovePersonItem(uuid.UUID.Value, uuid.CprNumber )})
                    .ToArray();
            Console.WriteLine("{0} : Found <{1}> persons", DateTime.Now, queueItems.Count());

            var queue = new CleanupQueueStub_Decisions();
            var ret = queue.Process(queueItems);
            Console.WriteLine("{0} : Decisions found for <{1}> persons", DateTime.Now, ret.Count());
            Console.WriteLine("ProcessItems(): called <{0}>, finished <{1}>", queue.ProcessItemCalls, queue.ProcessItemCalls2);

            foreach (var kvp in queue.RemovalDecisions)
                Console.WriteLine("{0} : {1}", kvp.Key, kvp.Value);
        }

        class CleanupQueueStub_Decisions : CleanupQueue
        {
            public Dictionary<PersonRemovalDecision, int> RemovalDecisions = new Dictionary<PersonRemovalDecision, int>();
            public int ProcessItemCalls = 0;
            public int ProcessItemCalls2 = 0;

            public CleanupQueueStub_Decisions()
            {
                foreach (PersonRemovalDecision rd in Enum.GetValues(typeof(PersonRemovalDecision)))
                    RemovalDecisions[rd] = 0;
            }

            public override CleanupQueueItem ProcessItemWithMutex(BrokerContext brokerContext, TrackingDataProvider prov, CleanupQueueItem queueItem)
            {
                BrokerContext.Current = brokerContext;
                Mutex personMutex = null;

                try
                {
                    // Establish a person based critical section
                    personMutex = new Mutex(false, CprBroker.Utilities.Strings.GuidToString(queueItem.removePersonItem.PersonUuid));
                    personMutex.WaitOne();

                    // Now the person is locked, all possible usage has been recorded                
                    var fromDate = DateTime.Now - SettingsUtilities.MaxInactivePeriod;
                    var dbrFromDate = fromDate + SettingsUtilities.DprEmulationRemovalAllowance;
                    var excludedMunicipalityCodes = SettingsUtilities.ExcludedMunicipalityCodes;

                    return ProcessItem(brokerContext, prov, queueItem, fromDate, dbrFromDate, excludedMunicipalityCodes);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return null;
                }
                finally
                {
                    // Release the lock
                    if (personMutex != null)
                        personMutex.ReleaseMutex();
                }
            }

            public override CleanupQueueItem ProcessItem(BrokerContext brokerContext, TrackingDataProvider prov, CleanupQueueItem queueItem, DateTime fromDate, DateTime dbrFromDate, int[] excludedMunicipalityCodes)
            {
                try
                {
                    Interlocked.Increment(ref ProcessItemCalls);
                    var decision = prov.GetRemovalDecision(queueItem.removePersonItem.ToPersonIdentifier(), fromDate, dbrFromDate, excludedMunicipalityCodes);
                    Interlocked.Increment(ref ProcessItemCalls2);
                    lock (String.Intern(decision.ToString()))
                    {
                        RemovalDecisions[decision]++;
                    }
                    return queueItem;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
            }
        }
    }
}
