using CprBroker.Data.Applications;
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
                ApplicationId = op.Activity.ApplicationId,
                ReadTime = op.Activity.StartTS
            };
        }
    }
}
