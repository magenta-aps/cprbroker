using CprBroker.Data.Applications;
using CprBroker.Engine;
using CprBroker.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.PartInterface.Tracking
{
    public class TrackingRequestProcessor : RequestProcessor
    {
        public PersonTrack GetHistory(Guid[] personUuids, DateTime? fromDate, DateTime? toDate)
        {
            return null;
        }


    }

    public interface ITrackingDataProvider : IDataProvider
    {
        PersonTrack[] GetTrack(Guid[] personUuids, DateTime? fromDate, DateTime? toDate);
    }
    public class TRackingDataProvider : ITrackingDataProvider
    {
        #region IDataProvider members
        public Version Version
        {
            get
            {
                return new Version(CprBroker.Utilities.Constants.Versioning.Major, CprBroker.Utilities.Constants.Versioning.Major);
            }
        }
        #endregion

        public PersonTrack[] GetTrack(Guid[] personUuids, DateTime? fromDate, DateTime? toDate)
        {
            return Operation
                .Get(
                    personUuids.Select(id => id.ToString()).ToArray(),
                    new OperationType.Types[] { OperationType.Types.Read, OperationType.Types.ReadPeriod },
                    fromDate,
                    toDate)
                .Select(kvp => kvp.Key.ToPersonTrack(kvp.Value))
                .ToArray();
        }

        public bool IsAlive()
        {
            throw new NotImplementedException();
        }
    }

    public static class TrackingExtensions
    {
        public static PersonTrack ToPersonTrack(this string uuid, Operation[] operations)
        {
            var ret = new PersonTrack()
            {
                UUID = new Guid(uuid),
                ReadOperations = operations.Select(op => op.ToReadInstance()).ToArray(),
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

    public class PersonTrack
    {
        public Guid UUID { get; set; }
        public ApplicationId[] Subscribers { get; set; }
        public ReadInstance[] ReadOperations { get; set; }
        public DateTime? LastRead { get; set; }
    }

    public class ReadInstance
    {
        public Guid ApplicationId { get; set; }
        public DateTime ReadTime { get; set; }
    }
}
