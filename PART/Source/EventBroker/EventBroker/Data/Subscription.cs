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
using CprBroker.Schemas.Part;
using CprBroker.Utilities;
using CprBroker.Data.Part;

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
        public CprBroker.Schemas.SubscriptionType ToOioSubscription(string appToken)
        {
            CprBroker.Schemas.SubscriptionType ret = null;
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
            ret.ForAllPersons = this.IsForAllPersons;

            Channel channel = this.Channels.Single();
            switch ((ChannelType.ChannelTypes)channel.ChannelTypeId)
            {
                case ChannelType.ChannelTypes.WebService:
                    CprBroker.Schemas.WebServiceChannelType webServiceChannel = new WebServiceChannelType();
                    webServiceChannel.WebServiceUrl = channel.Url;
                    ret.NotificationChannel = webServiceChannel;
                    break;
                case ChannelType.ChannelTypes.FileShare:
                    CprBroker.Schemas.FileShareChannelType fileShareChannel = new FileShareChannelType();
                    fileShareChannel.Path = channel.Url;
                    ret.NotificationChannel = fileShareChannel;
                    break;
            }

            ret.PersonUuids.AddRange(
                from subPerson in this.SubscriptionPersons
                select subPerson.PersonUuid.Value.ToString()
                );
            return ret;
        }

        public void MatchDataChangeEvents(DataChangeEvent[] dataChangeEvents)
        {
            var personRegistrationIds = dataChangeEvents.Select(dce => dce.PersonRegistrationId).ToArray();

            var xml = this.Criteria.ToString();
            var soegObject = Strings.Deserialize<SoegObjektType>(xml);
            using (var partDataContext = new PartDataContext())
            {
                // Add new persons                    
                var matchingPersons = CprBroker.Data.Part.PersonRegistrationKey.GetByCriteria(partDataContext, soegObject, personRegistrationIds).ToArray();
                var temp = matchingPersons.Select(prk => new TempSubscriptionPerson()
                {
                    TempSubscriptionPersonId = Guid.NewGuid(),
                    SubscriptionId = this.SubscriptionId,
                    DataChangeEventId = dataChangeEvents.Where(dce => dce.PersonRegistrationId == prk.PersonRegistrationId).Select(dce => dce.DataChangeEventId).First(),
                });
                this.TempSubscriptionPersons.AddRange(temp);
                /*
                var ppp = from dce in dataChangeEvents
                          join sp0 in this.SubscriptionPersons.Where(sp0 => sp0.Removed == null).ToArray() on dce.PersonUuid equals sp0.PersonUuid into subscriptionPersons0
                          join m0 in matchingPersons on dce.PersonRegistrationId equals m0.PersonRegistrationId into matchingPersons0
                          from sp in subscriptionPersons0.DefaultIfEmpty()
                          from m in matchingPersons0.DefaultIfEmpty()

                          select new
                          {
                              DataChangeEvent = dce,
                              InCriteria = m != null,
                              SubscriptionPerson = sp,
                          };

                // Insert new persons matching the criteria
                var newPersons = ppp.Where(p => p.InCriteria && p.SubscriptionPerson == null).Select(p => new Data.SubscriptionPerson()
                {
                    Created = DateTime.Now,
                    PersonUuid = p.DataChangeEvent.PersonUuid,
                    Removed = null,
                    SubscriptionId = this.SubscriptionId,
                    SubscriptionPersonId = Guid.NewGuid()
                });
                this.SubscriptionPersons.AddRange(newPersons);

                // Delete persons moving out of criteria, and explicitly enque their last notification
                var movingOut = ppp.Where(p => !p.InCriteria && p.SubscriptionPerson != null).Select(p => p.SubscriptionPerson);
                foreach (var sp in movingOut)
                {
                    sp.Removed = DateTime.Now;
                    this.EventNotifications.Add(new Data.EventNotification()
                    {
                        CreatedDate = DateTime.Now,
                        EventNotificationId = Guid.NewGuid(),
                        IsLastNotification = true,
                        NotificationDate = null,
                        PersonUuid = sp.PersonUuid.Value,
                        Succeeded = null,
                    });
                }
                */
            }
        }
    }
}
