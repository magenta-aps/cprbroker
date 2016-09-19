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

        public bool IsAlive()
        {
            return true;
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
                .Select(kvp => kvp.Item1.ToPersonTrack(kvp.Item2))
                .ToArray();
        }

        public PersonTrack[] GetSubscribers(Guid[] personUuids)
        {
            // Cache for application table
            var applications = new Dictionary<Guid, Application>();
            using (var dataContext = new ApplicationDataContext())
            {
                applications = dataContext.Applications.ToDictionary(a => a.ApplicationId);
                foreach (var app in applications)
                    app.Value.Token = null;
            }
            Func<Guid, ApplicationType> converter = id =>
            {
                if (applications.ContainsKey(id))
                    return applications[id].ToXmlType();
                else
                    return null as ApplicationType;
            };

            var subscriptions = Subscription.GetSubscriptions(personUuids);
            return subscriptions.Select(s => s.Item1.ToPersonSubscribers(s.Item2, converter))
                .ToArray();
        }
    }
}
