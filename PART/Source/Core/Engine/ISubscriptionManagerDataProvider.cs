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

    public class SubcriptionMergeResult
    {
        public string Field { get; private set; }

        public List<string> Added = new List<string>();
        public List<string> AddFailed = new List<string>();
        public List<string> Removed = new List<string>();
        public List<string> RemoveFailed = new List<string>();
        public List<string> AlreadyExisting = new List<string>();
        public List<string> Kept = new List<string>();

        public SubcriptionMergeResult(string field)
        {
            this.Field = field;
        }

    }
    public static class ISubscriptionManagerDataProviderExtensions
    {
        public static SubcriptionMergeResult MergeSubscriptions(this ISubscriptionManagerDataProvider prov, string field, string[] values, bool replace = false)
        {
            values = values.Distinct().ToArray();

            var ret = new SubcriptionMergeResult(field);

            var existing = prov.GetSubscriptions(field);

            // Only add a subscription if not already there
            string[] toAdd = values.Except(existing).ToArray();

            string[] toRemove = replace ?
                existing.Except(values).ToArray() // Unsubscribe to keys that are no longer needed
                : new string[] { }; // Do not remove any subscriptions

            ret.AlreadyExisting = values.Except(toAdd).ToList();
            ret.Kept = existing.Except(toRemove).ToList();

            foreach (var value in toAdd)
            {
                var res = false;
                try
                {
                    res = prov.PutSubscription(field, value);
                }
                catch (Exception ex)
                {
                    CprBroker.Engine.Local.Admin.LogException(ex);
                }

                if (res)
                    ret.Added.Add(value);
                else
                    ret.AddFailed.Add(value);
            }

            foreach (var value in toRemove)
            {
                var res = false;
                try
                {
                    res = prov.RemoveSubscription(field, value);
                }
                catch (Exception ex)
                {
                    CprBroker.Engine.Local.Admin.LogException(ex);
                }

                if (res)
                    ret.Removed.Add(value);
                else
                    ret.RemoveFailed.Add(value);
            }

            return ret;
        }
    }
}
