using CprBroker.Data.Applications;
using CprBroker.EventBroker.Data;
using CprBroker.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.PartInterface.Tracking
{
    public static class TrackingExtensions
    {
        public static PersonTrack ToPersonTrack(this string uuid, Operation[] operations)
        {
            var ret = new PersonTrack()
            {
                UUID = new Guid(uuid),
                ReadOperations = operations.Select(op => op.ToReadInstance()).ToArray(),
                Subscribers = null,
                LastRead = null
            };
            ret.LastRead = ret.ReadOperations.FirstOrDefault()?.ReadTime;
            return ret;
        }

        public static ReadInstance ToReadInstance(this Operation op)
        {
            return new ReadInstance()
            {
                ApplicationId = op.Activity?.ApplicationId,
                ReadTime = op.Activity.StartTS
            };
        }

        public static PersonTrack ToPersonSubscribers(this Guid uuid, SubscriptionPerson[] subscriptionPersons, Func<Guid, ApplicationType> converter)
        {
            var ret = new PersonTrack()
            {
                UUID = uuid,
                Subscribers = subscriptionPersons
                    .Select(sp => sp.Subscription.ApplicationId)
                    .Distinct()
                    .Select(applicationId => converter(applicationId))
                    .ToArray(),
                ReadOperations = null,
                LastRead = null
            };
            ret.LastRead = null;
            return ret;
        }
    }
}
