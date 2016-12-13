/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Utilities;
using CprBroker.Data.Part;
using System.Data.Linq;

namespace CprBroker.EventBroker.Data
{
    /// <summary>
    /// Represents the Subscription table
    /// </summary>
    public partial class Subscription
    {
        /// <summary>
        /// Sets the load options to load the child entities with a root Subscription object
        /// </summary>
        /// <param name="loadOptions"></param>
        public static void SetLoadOptionsForChildren(System.Data.Linq.DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<Subscription>((Subscription sub) => sub.DataSubscription);
            loadOptions.LoadWith<Subscription>((Subscription sub) => sub.BirthdateSubscription);
            loadOptions.LoadWith<Subscription>((Subscription sub) => sub.Channels);
            loadOptions.LoadWith<Subscription>((Subscription sub) => sub.SubscriptionPersons);
        }

        public CprBroker.Schemas.Part.SubscriptionType ToOioSubscription(string appToken)
        {
            CprBroker.Schemas.Part.SubscriptionType ret = null;
            if (this.DataSubscription != null)
            {
                ChangeSubscriptionType dataSubscription = new ChangeSubscriptionType();
                ret = dataSubscription;
            }
            else // birthdate subscription
            {
                BirthdateSubscriptionType birthdateSubscription = new BirthdateSubscriptionType();
                birthdateSubscription.AgeYears = this.BirthdateSubscription.AgeYears;
                birthdateSubscription.PriorDays = this.BirthdateSubscription.PriorDays;
                ret = birthdateSubscription;
            }
            ret.SubscriptionId = this.SubscriptionId.ToString();
            ret.ApplicationToken = appToken;
            ret.Persons = new SubscriptionPersonsType();
            if (this.IsForAllPersons)
            {
                ret.Persons.Item = true;
            }
            else if (this.Criteria == null)
            {
                ret.Persons.Item = new PersonUuidsType()
                {
                    UUID = this.SubscriptionPersons
                        .Select(sp => sp.PersonUuid.Value.ToString())
                        .ToArray()
                };
            }
            else
            {
                ret.Persons.Item = Utilities.Strings.Deserialize<SoegObjektType>(this.Criteria.ToString());
            }

            Channel channel = this.Channels.Single();
            switch ((ChannelType.ChannelTypes)channel.ChannelTypeId)
            {
                case ChannelType.ChannelTypes.WebService:
                    CprBroker.Schemas.Part.WebServiceChannelType webServiceChannel = new WebServiceChannelType();
                    webServiceChannel.WebServiceUrl = channel.Url;
                    ret.NotificationChannel = webServiceChannel;
                    break;
                case ChannelType.ChannelTypes.FileShare:
                    CprBroker.Schemas.Part.FileShareChannelType fileShareChannel = new FileShareChannelType();
                    fileShareChannel.Path = channel.Url;
                    ret.NotificationChannel = fileShareChannel;
                    break;
            }
            return ret;
        }

        public static IQueryable<Subscription> ActiveSubscriptions(EventBrokerDataContext dataContext)
        {
            return dataContext.Subscriptions
                .Where(s => s.Deactivated == null)
                // TODO: Detect Deactivated applications here
                ;
        }

        public int AddMatchingSubscriptionPersons(EventBrokerDataContext eventDataContext, int batchSize)
        {
            using (var partDataContext = new PartDataContext())
            {
                // Read the next (n) persons with the latest registration of each
                var persons = partDataContext.Persons
                    .Where(p => p.UUID.CompareTo(this.LastCheckedUUID.Value) > 0)
                    .OrderBy(p => p.UUID)
                    .Select(p => new
                    {
                        UUID = p.UUID,
                        PersonRegistrationId = p.PersonRegistrations
                            .Where(pr => pr.BrokerUpdateDate <= this.Created)
                            .OrderByDescending(pr => pr.RegistrationDate)
                            .ThenByDescending(pr => pr.BrokerUpdateDate)
                            .Select(pr => pr.PersonRegistrationId as Guid?)
                            .FirstOrDefault()
                    })
                    .Take(batchSize)
                    .ToArray();

                // Search for matching criteria
                var personRegistrationIds = persons
                    .Where(p => p.PersonRegistrationId.HasValue)
                    .Select(p => p.PersonRegistrationId.Value)
                    .ToArray();
                var soegObject = Strings.Deserialize<SoegObjektType>(this.Criteria.ToString());
                var matchingPersons = CprBroker.Data.Part.PersonRegistrationKey
                    .GetByCriteria(partDataContext, soegObject, personRegistrationIds)
                    .ToArray();

                if (persons.Count() == batchSize)
                {
                    this.LastCheckedUUID = persons.Last().UUID;
                }
                else
                {
                    this.LastCheckedUUID = null;
                    this.Ready = true;
                }

                var subscriptionPersons = matchingPersons.Select(mp => new SubscriptionPerson()
                {
                    Created = DateTime.Now,
                    PersonUuid = mp.UUID,
                    Removed = null,
                    SubscriptionPersonId = Guid.NewGuid()
                });
                this.SubscriptionPersons.AddRange(subscriptionPersons);
                return subscriptionPersons.Count();
            }
        }

        public IEnumerable<SubscriptionCriteriaMatch> Matches(DataChangeEvent[] dataChangeEvents)
        {
            var personRegistrationIds = dataChangeEvents
                .Select(dce => dce.PersonRegistrationId)
                .ToArray();

            var soegObject = Strings.Deserialize<SoegObjektType>(this.Criteria.ToString());
            using (var partDataContext = new PartDataContext())
            {
                // Add new persons                    
                var matchingPersons = CprBroker.Data.Part.PersonRegistrationKey
                    .GetByCriteria(partDataContext, soegObject, personRegistrationIds)
                    .ToArray();

                var temp = matchingPersons.Select(prk => new SubscriptionCriteriaMatch()
                {
                    SubscriptionCriteriaMatchId = Guid.NewGuid(),
                    DataChangeEventId = dataChangeEvents
                        .Where(dce => dce.PersonRegistrationId == prk.PersonRegistrationId)
                        .Select(dce => dce.DataChangeEventId)
                        .First(),
                });
                return temp;
            }
        }

        public static Subscription[] NonReadySubscriptions(EventBrokerDataContext dataContext)
        {
            return ActiveSubscriptions(dataContext)
                .Where(s => s.Ready == false)
                .OrderBy(s => s.Created)
                .ToArray();
        }

        public void UpdateCriteriaMatches(DataChangeEvent[] dataChangeEvents)
        {
            var matches = Matches(dataChangeEvents);
            this.SubscriptionCriteriaMatches.AddRange(matches);
        }

        public static IQueryable<SubscriptionPerson> GetSubscriptionPersonsAsQueryable(EventBrokerDataContext dataContext, Guid[] personUuids)
        {
            if (dataContext.LoadOptions == null)
            {
                var options = new DataLoadOptions();
                options.LoadWith((SubscriptionPerson sp) => sp.Subscription);
                dataContext.LoadOptions = options;
            }

            return dataContext.SubscriptionPersons.Where(
                   sp => personUuids.Contains(sp.PersonUuid.Value)
                && sp.Removed == null // Only persons that are still within the subscription
                && sp.Subscription.IsForAllPersons == false // Only subscriptions that are not for all persons
                && sp.Subscription.Criteria == null // Only non criteria subscriptions
                && sp.Subscription.Deactivated == null // Only active subscriptions
                );
        }

        public static Tuple<Guid, SubscriptionPerson[]>[] GetSubscriptions(Guid[] personUuids)
        {
            using (var dataContext = new EventBrokerDataContext())
            {
                var all = GetSubscriptionPersonsAsQueryable(dataContext, personUuids)
                    .GroupBy(o => o.PersonUuid)
                    .ToDictionary(o => o.Key.Value, o => o.ToArray());

                return personUuids.Select(uuid =>
                {
                    var subs = new SubscriptionPerson[0];
                    if (all.ContainsKey(uuid))
                    {
                        subs = all[uuid];
                    }
                    return new Tuple<Guid, SubscriptionPerson[]>(uuid, subs);
                })
                .ToArray();
            }
        }

        public static bool HasSubscriptions(Guid personUuid)
        {
            using (var dataContext = new EventBrokerDataContext())
            {
                return GetSubscriptionPersonsAsQueryable(dataContext, new Guid[] { personUuid })
                    .FirstOrDefault() != null;
            }
        }

        public static void RemovePersonFromAllSubscriptions(Guid personUuid)
        {
            using (var dataContext = new EventBrokerDataContext())
            {
                // Subscription persons
                var subscriptionPersonsToDelete = dataContext.SubscriptionPersons
                    .Where(sp => sp.PersonUuid == personUuid);
                dataContext.SubscriptionPersons.DeleteAllOnSubmit(subscriptionPersonsToDelete);

                // Person birthdates
                var personBirthdatesToRemove = dataContext.PersonBirthdates
                    .Where(pbd => pbd.PersonUuid == personUuid);
                dataContext.PersonBirthdates.DeleteAllOnSubmit(personBirthdatesToRemove);

                /* 
                    Remove events that have not been sent yet
                    There is a small probability of missing some rows here, if the system 
                    is currently processing changes or events for this particular person
                */
                // DataChangeEvent
                var dataChangeEventsToDelete = dataContext.DataChangeEvents
                    .Where(dce => dce.PersonUuid == personUuid);
                dataContext.DataChangeEvents.DeleteAllOnSubmit(dataChangeEventsToDelete);

                // EventNotifications that have not been sent yet
                var eventNotificationsToDelete = dataContext.EventNotifications
                    .Where(ev =>
                        ev.PersonUuid == personUuid
                        && ev.Succeeded == null // only those that were not attempted yet
                    );
                dataContext.EventNotifications.DeleteAllOnSubmit(eventNotificationsToDelete);

                var eventNotificationIds = eventNotificationsToDelete.Select(en => en.EventNotificationId);
                var birthdateEventNotificationsToDelete = dataContext.BirthdateEventNotifications
                    .Where(ben => eventNotificationIds.Contains(ben.EventNotificationId));
                dataContext.BirthdateEventNotifications.DeleteAllOnSubmit(birthdateEventNotificationsToDelete);

                dataContext.SubmitChanges();
            }
        }
    }
}