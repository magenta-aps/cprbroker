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
}
