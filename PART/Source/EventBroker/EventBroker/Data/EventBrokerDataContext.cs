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
using System.Data.SqlClient;
using CprBroker.Data;
using CprBroker.EventBroker.Properties;
using System.Linq;


namespace CprBroker.EventBroker.Data
{
    /// <summary>
    /// Represents the data context for events
    /// </summary>
    partial class EventBrokerDataContext : IDataContextCreationInfo
    {
        public EventBrokerDataContext() : this(CprBroker.Utilities.Config.ConfigManager.Current.Settings.EventBrokerConnectionString)
        {

        }

        public string[] DDL_Tables
        {
            get
            {
                return new string[] {
                    // Copied from CPR Broker
                    Resources.PersonBirthdate_Sql,
                    Resources.DataChangeEvent_Sql, 

                    // Subscriptions
                    Resources.SubscriptionType_Sql,
                    Resources.Subscription_Sql,
                    Resources.SubscriptionPerson_Sql,
                    Resources.DataSubscription_Sql,
                    Resources.BirthdateSubscription_Sql,
                    Resources.SubscriptionCriteriaMatch_Sql,
                    
                    // Channels
                    Resources.ChannelType_Sql,
                    Resources.Channel_Sql,
                    
                    // Notifications
                    Resources.EventNotification_Sql,
                    Resources.BirthdateEventNotification_Sql,
                };
            }
        }

        public string[] DDL_Logic
        {
            get
            {
                return new string[] {
                    // SP's and functions
                    Resources.EnqueueBirthdateEventNotifications_Sql,
                    Resources.EnqueueDataChangeEventNotifications_Sql,
                    Resources.IsBirthdateEvent_Sql,
                    Resources.UpdatePersonLists_Sql,
                };

            }
        }

        public string[] DDL
        {
            get
            {
                return DDL_Tables
                    .Concat(DDL_Logic)
                    .ToArray();
            }
        }

        public KeyValuePair<string, string>[] Lookups
        {
            get
            {
                return new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>(typeof(ChannelType).Name, Resources.ChannelType_Csv),
                    new KeyValuePair<string, string>(typeof(SubscriptionType).Name, Resources.SubscriptionType_Csv),
                };
            }
        }

        public Action<SqlConnection>[] CustomInitializers
        {
            get
            {
                return new Action<SqlConnection>[] { };
            }
        }

    }
}
