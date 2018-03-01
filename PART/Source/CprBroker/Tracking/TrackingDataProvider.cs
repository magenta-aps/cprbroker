using CprBroker.Data.Applications;
using CprBroker.Data.Part;
using CprBroker.DBR;
using CprBroker.Engine;
using CprBroker.Engine.Local;
using CprBroker.EventBroker.Data;
using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;
using CprBroker.Providers.Local.Search;
using CprBroker.Schemas;
using CprBroker.Utilities.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.PartInterface.Tracking
{
    public partial class TrackingDataProvider : ITrackingDataProvider
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

        public PersonTrack[] GetPersonUsage(Guid[] personUuids, DateTime? fromDate, DateTime? toDate)
        {
            return Operation
                .Get(
                    personUuids.Select(id => id.ToString()).ToArray(),
                    Constants.PersonUsageOperations,
                    fromDate,
                    toDate)
                .Select(kvp => kvp.Item1.ToPersonTrack(kvp.Item2))
                .ToArray();
        }

        public virtual bool PersonHasUsage(Guid personUuid, DateTime? fromDate, DateTime? toDate)
        {
            return Operation.HasUsage(
                personUuid.ToString(),
                Constants.PersonUsageOperations,
                fromDate,
                toDate
                );
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

        public virtual bool PersonHasSubscribers(Guid personUuid)
        {
            return Subscription.HasSubscriptions(personUuid);
        }

        public PersonTrack[] GetPersonUsageAndSubscribers(Guid[] personUuids, DateTime? fromDate, DateTime? toDate)
        {
            return GetPersonUsage(personUuids, fromDate, toDate)
                .Zip(
                    GetSubscribers(personUuids),
                    (track, subscriptions) =>
                    {
                        track.Subscribers = subscriptions.Subscribers;
                        return track;
                    })
                .ToArray();
        }

        public PersonIdentifier[] EnumeratePersons(int startIndex = 0, int maxCount = 200)
        {
            using (var partContext = new PartDataContext())
            {
                return partContext.PersonRegistrations
                    .Join(partContext.PersonMappings,
                        pr => pr.UUID, pm => pm.UUID, (pr, pm) => new PersonIdentifier() { UUID = pr.UUID, CprNumber = pm.CprNumber })
                    .OrderBy(pr => pr.UUID)
                    .Distinct()
                    .Skip(startIndex)
                    .Take(maxCount)
                    .ToArray();
            }
        }

        public PersonIdentifier[] EnumeratePersons(Guid? startUuidAfter = null, int maxCount = 200)
        {
            if (startUuidAfter.HasValue == false)
                startUuidAfter = Guid.Empty;

            using (var partContext = new PartDataContext())
            {
                return partContext.PersonRegistrations
                    .Join(partContext.PersonMappings,
                        pr => pr.UUID, pm => pm.UUID, (pr, pm) => new PersonIdentifier() { UUID = pr.UUID, CprNumber = pm.CprNumber })
                        .Where(pr => pr.UUID.Value.CompareTo(startUuidAfter) > 0)
                    .OrderBy(pr => pr.UUID)
                    .Distinct()
                    .Take(maxCount)
                    .ToArray();
            }
        }

        public virtual bool PersonLivesInExcludedMunicipality(PersonIdentifier personIdentifier, int[] excludedMunicipalities)
        {
            var municipalityCode = PersonSearchCache.GetValue<int?>(
                personIdentifier.UUID.Value,
                psc =>
                {
                    int retVal;
                    return int.TryParse(string.Format("{0}", psc.MunicipalityCode), out retVal) ? retVal : (int?)null;
                });

            Admin.LogFormattedSuccess(
                    "<{0}>: Checking excluded municipalities: person <{1}>, municipality <{2}>, excluded municipalities <{3}>",
                    this.GetType().Name,
                    personIdentifier.UUID,
                    municipalityCode,
                    string.Join(",", excludedMunicipalities)
                    );

            if (municipalityCode.HasValue && municipalityCode.Value > 0 && excludedMunicipalities.Contains(municipalityCode.Value))
            {
                // Do not remove
                Admin.LogFormattedSuccess(
                    "Person <{0}> excluded from cleanup due to excluded municipality of residence",
                    personIdentifier.UUID);
                return true;
            }
            else
            {
                return false;
            }
        }

        public PersonRemovalDecision GetRemovalDecision(PersonIdentifier personIdentifier, DateTime fromDate, DateTime dbrFromDate, int[] excludedMunicipalityCodes)
        {
            if (PersonLivesInExcludedMunicipality(personIdentifier, excludedMunicipalityCodes))
            {
                return PersonRemovalDecision.DoNotRemoveDueToExclusion;
            }

            var personHasSubscribers = this.PersonHasSubscribers(personIdentifier.UUID.Value);
            var personHasUsage = this.PersonHasUsage(personIdentifier.UUID.Value, fromDate, null);

            if (personHasSubscribers == false)
            {
                if (personHasUsage == false)
                {
                    return PersonRemovalDecision.RemoveCompletely;
                }
                else if (this.PersonHasUsage(personIdentifier.UUID.Value, dbrFromDate, null) == false)
                {
                    return PersonRemovalDecision.RemoveFromDprEmulation;
                }
            }
            // Person should not be removed - considered a success
            return PersonRemovalDecision.DoNotRemoveDueToUsage;
        }
    }
}
