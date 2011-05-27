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
using CprBroker.Engine;

namespace CprBroker.EventBroker.Subscriptions
{
    /// <summary>
    /// Contains methods related to subscriptions
    /// </summary>
    public interface ISubscriptionDataProvider : IDataProvider
    {
        /// <summary>
        /// Subscribes to a data change
        /// </summary>
        /// <param name="notificationChannel">Channel through with the client would like to be notified through</param>
        /// <param name="PersonCivilRegistrationIdentifiers">CPR Numbers for people to be watched</param>
        /// <returns></returns>
        ChangeSubscriptionType Subscribe(ChannelBaseType notificationChannel, Guid[] personUuids);

        /// <summary>
        /// Removes a data change subscription
        /// </summary>
        /// <param name="subscriptionId">Id of the subscription to remove</param>
        /// <returns></returns>
        bool Unsubscribe(Guid subscriptionId);

        /// <summary>
        /// Subscribes to a birthdate event
        /// </summary>
        /// <param name="notificationChannel">Channel through with the client would like to be notified through</param>
        /// <param name="years">Age at which the client should be notified</param>
        /// <param name="priorDays">Number of days proior to birthdate</param>
        /// <param name="PersonCivilRegistrationIdentifiers">CPR Numbers for people to be watched</param>
        /// <returns></returns>
        BirthdateSubscriptionType SubscribeOnBirthdate(ChannelBaseType notificationChannel, Nullable<int> years, int priorDays, Guid[] PersonCivilRegistrationIdentifiers);

        /// <summary>
        /// Removes a birthdate subscription
        /// </summary>
        /// <param name="subscriptionId">Id of the subscription to remove</param>
        /// <returns></returns>
        bool RemoveBirthDateSubscription(Guid subscriptionId);

        /// <summary>
        /// Gets the list of active subscriptions
        /// </summary>
        /// <returns></returns>
        CprBroker.Schemas.SubscriptionType[] GetActiveSubscriptionsList();

        /*
        /// <summary>
        /// Gets the latest notification that has been sent for the subscription that matches the given <paramref name="subscriptionId"/>
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <param name="subscriptionId">Subscription ID</param>
        /// <returns></returns>
        BaseNotificationType GetLatestNotification(string userToken, string appToken, Guid subscriptionId);
        */
    }
}
