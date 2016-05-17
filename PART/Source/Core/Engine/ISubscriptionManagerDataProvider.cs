using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Engine
{
    public interface ISubscriptionManagerDataProvider : IExternalDataProvider
    {
        // PNR, KomKode 
        string[] SubscriptionFields { get; }

        // PNR1, PNR2, etc - or 101, 503, etc
        string[] GetSubscriptions(string field);

        bool PutSubscription(string field, string value);
        bool RemoveSubscription(string field, string value);

        Dictionary<string, string> EnumerateField(string field);
    }

    public static class ISubscriptionManagerDataProviderExtensions
    {
        public static void MergeSubscriptions(this ISubscriptionManagerDataProvider prov, string field, string[] values, bool replace = false)
        {
            var existing = prov.GetSubscriptions(field);

            // Only add a subscription if not already there
            string[] toAdd = values.Except(existing).ToArray();

            string[] toRemove = replace ?
                existing.Except(values).ToArray() // Unsubscribe to keys that are no longer needed
                : new string[] { }; // Do not remove any subscriptions

            foreach (var value in toAdd)
            {
                prov.PutSubscription(field, value);
            }

            foreach (var value in toRemove)
            {
                prov.RemoveSubscription(field, value);
            }
        }
    }
}
