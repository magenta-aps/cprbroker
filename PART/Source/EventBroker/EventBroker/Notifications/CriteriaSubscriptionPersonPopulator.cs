using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine.Local;
using CprBroker.Engine.Tasks;
using CprBroker.Utilities.Config;

namespace CprBroker.EventBroker.Notifications
{
    /// <summary>
    /// Fills criteria subscriptions with the initial list of persons
    /// </summary>
    public class CriteriaSubscriptionPersonPopulator : PeriodicTaskExecuter
    {
        protected override void PerformTimerAction()
        {
            Admin.LogFormattedSuccess("DataChangeEventEnqueuer.UpdateSubscriptionCriteriaLists() started, batch size <{0}>", this.BatchSize);

            using (var eventDataContext = new Data.EventBrokerDataContext())
            {
                var subscriptions = Data.Subscription.GetNonReadySubscriptions(eventDataContext);
                Admin.LogFormattedSuccess("Found <{0}> pending criteria subscriptions", subscriptions.Length);
                foreach (var sub in subscriptions)
                {
                    while (sub.LastCheckedUUID.HasValue)
                    {
                        var added = sub.AddMatchingSubscriptionPersons(eventDataContext, this.BatchSize);
                        eventDataContext.SubmitChanges();
                        Admin.LogFormattedSuccess("Added <{0}> persons to subscription <{1}>, next start UUID <{2}>", added, sub.SubscriptionId, sub.LastCheckedUUID);
                    }
                }
            }
            Admin.LogFormattedSuccess("DataChangeEventEnqueuer.UpdateSubscriptionCriteriaLists() finished");

        }

    }
}
