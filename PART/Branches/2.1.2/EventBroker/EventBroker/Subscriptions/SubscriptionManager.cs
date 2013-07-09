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
using CprBroker.Engine;

namespace CprBroker.EventBroker.Subscriptions
{
    /// <summary>
    /// This part contains methods related to admin interface
    /// All methods here simply delegate the code to Manager.CallMethod&lt;&gt;()
    /// </summary>
    public class SubscriptionManager
    {
        public static BasicOutputType<TItem> GetMethodOutput<TItem>(GenericFacadeMethodInfo<TItem> facade)
        {
            return CprBroker.Engine.Manager.GetMethodOutput<TItem>(facade);
        }

        #region Subscription
        public static BasicOutputType<ChangeSubscriptionType> Subscribe(string userToken, string appToken, ChannelBaseType notificationChannel, Guid[] personUuids)
        {
            SubscribeFacadeMethod facade = new SubscribeFacadeMethod(notificationChannel, personUuids, appToken, userToken);
            return GetMethodOutput<ChangeSubscriptionType>(facade);

        }

        public static BasicOutputType<bool> Unsubscribe(string userToken, string appToken, Guid subscriptionId)
        {
            UnsubscribeFacadeMethod facade = new UnsubscribeFacadeMethod(subscriptionId, CprBroker.EventBroker.Data.SubscriptionType.SubscriptionTypes.DataChange, appToken, userToken);
            return GetMethodOutput<bool>(facade);
        }

        public static BasicOutputType<BirthdateSubscriptionType> SubscribeOnBirthdate(string userToken, string appToken, ChannelBaseType notificationChannel, Nullable<int> years, int priorDays, Guid[] PersonCivilRegistrationIdentifiers)
        {
            SubscribeOnBirthdateFacadeMethod facade = new SubscribeOnBirthdateFacadeMethod(notificationChannel, years, priorDays, PersonCivilRegistrationIdentifiers, appToken, userToken);
            return GetMethodOutput<BirthdateSubscriptionType>(facade);
        }

        public static BasicOutputType<bool> RemoveBirthDateSubscription(string userToken, string appToken, Guid subscriptionId)
        {
            UnsubscribeFacadeMethod facade = new UnsubscribeFacadeMethod(subscriptionId, CprBroker.EventBroker.Data.SubscriptionType.SubscriptionTypes.Birthdate, appToken, userToken);
            return GetMethodOutput<bool>(facade);
        }

        public static BasicOutputType<SubscriptionType[]> GetActiveSubscriptionsList(string userToken, string appToken)
        {
            GetActiveSubscriptionsListFacadeMethod facade = new GetActiveSubscriptionsListFacadeMethod(appToken, userToken);
            return GetMethodOutput<SubscriptionType[]>(facade);
        }

        #endregion

    }
}

