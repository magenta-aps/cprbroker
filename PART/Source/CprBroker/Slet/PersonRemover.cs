using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CprBroker.PartInterface.Tracking;
using CprBroker.Schemas;
using CprBroker.Engine;
using CprBroker.Engine.Local;

namespace CprBroker.Slet
{
    class PersonRemover
    {
        public CleanupQueueItem RemovePerson(BrokerContext brokerContext, CleanupQueueItem queueItem, DateTime fromDate, DateTime dbrFromDate, int[] excludedMunicipalityCodes)
        {
            RemovePerson(brokerContext, queueItem.removePersonItem, fromDate, dbrFromDate, excludedMunicipalityCodes);
            return queueItem;
        }

        public RemovePersonItem RemovePerson(BrokerContext brokerContext, RemovePersonItem removePersonItem, DateTime fromDate, DateTime dbrFromDate, int[] excludedMunicipalityCodes)
        {
            TrackingDataProvider prov = new TrackingDataProvider();
            BrokerContext.Current = brokerContext;
            var personIdentifier = removePersonItem.ToPersonIdentifier();

            // First, make and log the decisions
            var decision = prov.GetRemovalDecision(personIdentifier, fromDate, dbrFromDate, excludedMunicipalityCodes);
            removePersonItem.removalDecision = decision;

            // Action time
            // Remove the person if needed
            switch (decision)
            {
                case PersonRemovalDecision.RemoveCompletely:
                    Admin.LogFormattedSuccess("<{0}>:Removing unused person <{1}>", this.GetType().Name, personIdentifier.UUID);
                    var task1 = prov.RemovePersonAsync(personIdentifier);
                    task1.Wait();
                    var personRemoved = task1.Result;
                    if (personRemoved)
                        return removePersonItem;
                    else
                        return null;

                case PersonRemovalDecision.RemoveFromDprEmulation: // Only remove from DPR emulation                    
                    Admin.LogFormattedSuccess("<{0}>:Removing semi-unused person <{1}> from DPR emulation", this.GetType().Name, personIdentifier.UUID);
                    var task2 = prov.DeletePersonFromAllDBR(brokerContext, personIdentifier);
                    task2.Wait();
                    var dbrRemoved = task2.Result;
                    if (dbrRemoved)
                        return removePersonItem;
                    else
                        return null;

                case PersonRemovalDecision.DoNotRemoveDueToExclusion:
                    return removePersonItem;

                case PersonRemovalDecision.DoNotRemoveDueToUsage:
                    return removePersonItem;

                default:
                    throw new Exception(string.Format("Unknown value <{0}> for type <{1}>", decision, typeof(PersonRemovalDecision).Name));
            }
        }
    }
}
