using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Engine;

namespace CprBroker.EventBroker
{
    /// <summary>
    /// Contains methods related to subscriptions
    /// </summary>
    public interface ISubscriptionManager : IDataProvider
    {
        /// <summary>
        /// Subscribes to a data change
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <param name="notificationChannel">Channel through with the client would like to be notified through</param>
        /// <param name="PersonCivilRegistrationIdentifiers">CPR Numbers for people to be watched</param>
        /// <returns></returns>
        ChangeSubscriptionType Subscribe(string userToken, string appToken, ChannelBaseType notificationChannel, Guid[] PersonCivilRegistrationIdentifiers);

        /// <summary>
        /// Removes a data change subscription
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <param name="subscriptionId">Id of the subscription to remove</param>
        /// <returns></returns>
        bool Unsubscribe(string userToken, string appToken, Guid subscriptionId);

        /// <summary>
        /// Subscribes to a birthdate event
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <param name="notificationChannel">Channel through with the client would like to be notified through</param>
        /// <param name="years">Age at which the client should be notified</param>
        /// <param name="priorDays">Number of days proior to birthdate</param>
        /// <param name="PersonCivilRegistrationIdentifiers">CPR Numbers for people to be watched</param>
        /// <returns></returns>
        BirthdateSubscriptionType SubscribeOnBirthdate(string userToken, string appToken, ChannelBaseType notificationChannel, Nullable<int> years, int priorDays, Guid[] PersonCivilRegistrationIdentifiers);

        /// <summary>
        /// Removes a birthdate subscription
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <param name="subscriptionId">Id of the subscription to remove</param>
        /// <returns></returns>
        bool RemoveBirthDateSubscription(string userToken, string appToken, Guid subscriptionId);

        /// <summary>
        /// Gets the list of active subscriptions
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <returns></returns>
        CprBroker.Schemas.SubscriptionType[] GetActiveSubscriptionsList(string userToken, string appToken);

        /// <summary>
        /// Gets the latest notification that has been sent for the subscription that matches the given <paramref name="subscriptionId"/>
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <param name="subscriptionId">Subscription ID</param>
        /// <returns></returns>
        BaseNotificationType GetLatestNotification(string userToken, string appToken, Guid subscriptionId);
    }
}
