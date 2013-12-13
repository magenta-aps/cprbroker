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
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
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
using CprBroker.EventBroker.Data;
using CprBroker.Utilities;
using CprBroker.Schemas.Part;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CprBroker.EventBroker.Subscriptions
{
    /// <summary>
    /// Data provider that implements subscription methods
    /// </summary>
    public partial class SubscriptionDataProvider : ISubscriptionDataProvider
    {
        /// <summary>
        /// Adds a new Subscription object to the <paramref name="dataContext"/>
        /// This method inserts rows in Subscription, Channel (and children) and  SubscriptionPerson
        /// A later call to SubmitChanges() is required to save to the database
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="subscriptionType"></param>
        /// <param name="applicationId"></param>
        /// <param name="PersonCivilRegistrationIdentifiers"></param>
        /// <param name="notificationChannel"></param>
        /// <returns></returns>
        private Subscription AddSubscription(EventBrokerDataContext dataContext, CprBroker.EventBroker.Data.SubscriptionType.SubscriptionTypes subscriptionType, Guid applicationId, ChannelBaseType notificationChannel, Guid[] personUuids, SoegObjektType soegObject)
        {
            List<Guid> listOfPersonsIDs = new List<Guid>();

            Subscription subscription = new Subscription();
            subscription.SubscriptionId = Guid.NewGuid();
            subscription.SubscriptionTypeId = (int)subscriptionType;
            subscription.ApplicationId = applicationId;
            subscription.Created = DateTime.Now;
            if (soegObject != null)
            {
                var xml = Strings.SerializeObject(soegObject);
                subscription.Criteria = System.Xml.Linq.XElement.Load(new StringReader(xml));
                subscription.LastCheckedUUID = Guid.Empty;
            }


            #region Set IsForAllPersons
            if (personUuids != null && personUuids.Length > 0)
            {
                subscription.IsForAllPersons = false;
            }
            else
            {
                subscription.IsForAllPersons = true;
            }
            #endregion

            #region Set the channel
            var dbChannel = Channel.FromXmlType(notificationChannel);
            subscription.Channels.Add(dbChannel);
            #endregion

            // Mark the new objects to be inserted later
            dataContext.Subscriptions.InsertOnSubmit(subscription);

            if (!subscription.IsForAllPersons && personUuids != null)
            {
                dataContext.SubscriptionPersons.InsertAllOnSubmit(
                    from PersonUuid in personUuids
                    select new SubscriptionPerson() { SubscriptionPersonId = Guid.NewGuid(), SubscriptionId = subscription.SubscriptionId, PersonUuid = PersonUuid }
                    );
            }
            dataContext.Channels.InsertOnSubmit(dbChannel);

            return subscription;
        }

        /// <summary>
        /// Deletes the given subscription from the database
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="subscriptionType"></param>
        /// <returns></returns>
        private bool DeleteSubscription(Guid subscriptionId, CprBroker.EventBroker.Data.SubscriptionType.SubscriptionTypes subscriptionType)
        {
            // Find the Subscription object and delete it and its children
            using (EventBrokerDataContext dataContext = new EventBrokerDataContext())
            {
                System.Data.Linq.DataLoadOptions loadOptions = new System.Data.Linq.DataLoadOptions();
                Subscription.SetLoadOptionsForChildren(loadOptions);
                dataContext.LoadOptions = loadOptions;

                var subscription = (from sub in dataContext.Subscriptions
                                    where sub.SubscriptionId == subscriptionId && sub.SubscriptionTypeId == (int)subscriptionType
                                    select sub
                                    ).SingleOrDefault();

                if (subscription != null)
                {
                    subscription.Deactivated = DateTime.Now;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Interface implementation
        /// </summary>
        public ChangeSubscriptionType Subscribe(ChannelBaseType notificationChannel, Guid[] personUuids)
        {
            using (EventBrokerDataContext dataContext = new EventBrokerDataContext())
            {
                Data.Subscription subscription = AddSubscription(dataContext, Data.SubscriptionType.SubscriptionTypes.DataChange, CprBroker.Engine.BrokerContext.Current.ApplicationId.Value, notificationChannel, personUuids, null);
                if (subscription != null)
                {
                    subscription.DataSubscription = new DataSubscription();
                }
                dataContext.SubmitChanges();
                if (subscription != null)
                {
                    // Now get the subscription from the database to make sure everything is OK
                    return GetActiveSubscriptionsList(subscription.SubscriptionId).SingleOrDefault() as ChangeSubscriptionType;
                }
            }
            return null;
        }

        /// <summary>
        /// Interface implementation
        /// </summary>
        public bool Unsubscribe(Guid subscriptionId)
        {
            return DeleteSubscription(subscriptionId, Data.SubscriptionType.SubscriptionTypes.DataChange);
        }

        /// <summary>
        /// Interface implementation
        /// </summary>
        public BirthdateSubscriptionType SubscribeOnBirthdate(ChannelBaseType notificationChannel, Nullable<int> years, int priorDays, Guid[] personUuids)
        {
            using (EventBrokerDataContext dataContext = new EventBrokerDataContext())
            {
                Data.Subscription subscription = AddSubscription(dataContext, Data.SubscriptionType.SubscriptionTypes.Birthdate, CprBroker.Engine.BrokerContext.Current.ApplicationId.Value, notificationChannel, personUuids, null);
                if (subscription != null)
                {
                    subscription.BirthdateSubscription = new BirthdateSubscription();
                    subscription.BirthdateSubscription.AgeYears = years;
                    subscription.BirthdateSubscription.PriorDays = priorDays;
                }

                dataContext.SubmitChanges();
                if (subscription != null)
                {
                    // Now get the subscription from the database to make sure everything is OK
                    return GetActiveSubscriptionsList(subscription.SubscriptionId).SingleOrDefault() as BirthdateSubscriptionType;
                }
            }
            return null;
        }

        /// <summary>
        /// Interface implementation
        /// </summary>
        public bool RemoveBirthDateSubscription(Guid subscriptionId)
        {
            return DeleteSubscription(subscriptionId, Data.SubscriptionType.SubscriptionTypes.Birthdate);
        }

        /// <summary>
        /// Interface implementation
        /// </summary>
        public ChangeSubscriptionType SubscribeOnCriteria(ChannelBaseType notificationChannel, SoegObjektType criteria)
        {
            using (EventBrokerDataContext dataContext = new EventBrokerDataContext())
            {
                /*
                 * For now we assign this data type as a data change type.
                 */
                Data.Subscription subscription = AddSubscription(dataContext, Data.SubscriptionType.SubscriptionTypes.DataChange /* May be changed later */, CprBroker.Engine.BrokerContext.Current.ApplicationId.Value, notificationChannel, new Guid[0], criteria);
                if (subscription != null)
                {
                    subscription.DataSubscription = new DataSubscription();
                }
                dataContext.SubmitChanges();
                if (subscription != null)
                {
                    // Now get the subscription from the database to make sure everything is OK
                    return GetActiveSubscriptionsList(subscription.SubscriptionId).SingleOrDefault() as ChangeSubscriptionType;
                }
            }
            return null;
        }

        /// <summary>
        /// Interface implementation
        /// </summary>
        public CprBroker.Schemas.Part.SubscriptionType[] GetActiveSubscriptionsList()
        {
            return GetActiveSubscriptionsList(null);
        }

        /// <summary>
        /// Retrieves the subscriptions that match the supplied criteria
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        private CprBroker.Schemas.Part.SubscriptionType[] GetActiveSubscriptionsList(Nullable<Guid> subscriptionId)
        {
            List<CprBroker.Schemas.Part.SubscriptionType> listType = new List<CprBroker.Schemas.Part.SubscriptionType>();
            using (EventBrokerDataContext context = new EventBrokerDataContext())
            {
                System.Data.Linq.DataLoadOptions loadOptions = new System.Data.Linq.DataLoadOptions();
                Subscription.SetLoadOptionsForChildren(loadOptions);
                context.LoadOptions = loadOptions;

                // Create basic LINQ expression
                System.Linq.Expressions.Expression<Func<IQueryable<Subscription>>> exp =
                    () =>
                    from sub in context.Subscriptions
                    where sub.ApplicationId == CprBroker.Engine.BrokerContext.Current.ApplicationId
                    select sub;

                // Add filter for subscription id (if required)
                IQueryable<Subscription> subscriptions;
                if (subscriptionId.HasValue)
                {
                    subscriptions = exp.Compile()().Where((Subscription sub) => sub.SubscriptionId == subscriptionId);
                }
                else
                {
                    subscriptions = exp.Compile()();
                }

                // Now create list of OIO subscriptions
                foreach (var sub in subscriptions)
                {
                    CprBroker.Schemas.Part.SubscriptionType subscriptionType = sub.ToOioSubscription(CprBroker.Engine.BrokerContext.Current.ApplicationToken);
                    listType.Add(subscriptionType);
                }
                return listType.ToArray();
            }
        }

        #region IDataProvider Members

        public bool IsAlive()
        {
            return true;
        }

        public Version Version
        {
            get { return new Version(Constants.Versioning.Major, Constants.Versioning.Minor); }
        }

        #endregion
    }
}
