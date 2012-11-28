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
using System.Diagnostics;
using CprBroker.Schemas;
using CprBroker.Engine;
using CprBroker.Data;
using CprBroker.Utilities;

namespace CprBroker.Engine.Local
{
    public partial class Admin
    {
        /// <summary>
        /// Writes a success message (Information) to the log
        /// </summary>
        /// <param name="text"></param>
        public static void LogSuccess(string text)
        {
            AddNewLog(TraceEventType.Information, null, text, null, null);
        }

        public static void LogFormattedSuccess(string text, params object[] args)
        {
            AddNewLog(TraceEventType.Information, null, string.Format(text, args), null, null);
        }

        public static void LogError(string text)
        {
            AddNewLog(TraceEventType.Error, null, text, null, null);
        }

        public static void LogFormattedError(string format, params object[] args)
        {
            AddNewLog(TraceEventType.Error, null, string.Format(format, args), null, null);
        }

        /// <summary>
        /// Writes an exception to the log
        /// </summary>
        /// <param name="ex"></param>
        public static void LogException(Exception ex)
        {
            // TODO: Use BrokerContex.WebMethodMessageName
            AddNewLog(TraceEventType.Error, ex.Source, ex.ToString(), null, null);
        }

        public static void LogCriticalException(Exception ex)
        {
            // TODO: Use BrokerContex.WebMethodMessageName
            AddNewLog(TraceEventType.Critical, ex.Source, ex.ToString(), null, null);
        }

        public static void LogException(Exception ex, string moreText)
        {
            // TODO: Use BrokerContex.WebMethodMessageName
            AddNewLog(TraceEventType.Error, ex.Source, string.Format("{0}{1}{2}", moreText, Environment.NewLine, ex.ToString()), null, null);
        }

        public static void LogException(Exception ex, Exception[] moreExceptions)
        {
            // TODO: Use BrokerContex.WebMethodMessageName
            string[] msgs = Array.ConvertAll<Exception, string>(moreExceptions, e => e.ToString());
            AddNewLog(TraceEventType.Error, ex.Source, string.Format("{0}{1}{2}", ex.ToString(), Environment.NewLine, string.Join(Environment.NewLine, msgs)), null, null);
        }

        /// <summary>
        /// Writes a notification success to the log
        /// </summary>
        /// <param name="applicationToken"></param>
        /// <param name="subscriptionId"></param>
        /// <param name="notificationUrl"></param>
        public static void LogNotificationSuccess(string applicationToken, Guid subscriptionId, string notificationUrl)
        {
            LogSuccess(string.Format("{0}. Application={1}; Subscription={2} sent to {3}", TextMessages.NotificationSentSuccessfully, applicationToken, subscriptionId, notificationUrl));
        }
        /// <summary>
        /// Writes a notification exception to the log
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="applicationToken"></param>
        /// <param name="subscriptionId"></param>
        /// <param name="notificationUrl"></param>
        public static void LogNotificationException(Exception ex, string applicationToken, Guid subscriptionId, string notificationUrl)
        {
            LogException(ex, string.Format("{0}. Application={1}; Subscription={2} sent to {3}", TextMessages.NotificationSentSuccessfully, applicationToken, subscriptionId, notificationUrl));
        }

        /// <summary>
        /// Writes a new entry to the system log
        /// Other methods are used as some sort of overload fro different logging usages
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="methodName"></param>
        /// <param name="text"></param>
        /// <param name="objectType"></param>
        /// <param name="xml"></param>
        public static void AddNewLog(TraceEventType logType, string methodName, string text, string objectType, string xml)
        {
            Microsoft.Practices.EnterpriseLibrary.Logging.LogEntry ent = new Microsoft.Practices.EnterpriseLibrary.Logging.LogEntry();
            ent.ActivityId = BrokerContext.Current.ActivityId;
            ent.Categories.Add(Constants.Logging.Category);
            ent.Message = text;
            ent.RelatedActivityId = null;
            ent.Severity = logType;

            // Unused properties
            //ent.EventId = ;
            //ent.ManagedThreadName = ;
            //ent.Priority = ;
            //ent.Title = ;

            // TODO: Extended properties cause a problem

            Dictionary<string, object> extendedProperties = ent.ExtendedProperties as Dictionary<string, object>;
            extendedProperties[Constants.Logging.ApplicationId] = BrokerContext.Current.ApplicationId;
            extendedProperties[Constants.Logging.ApplicationName] = BrokerContext.Current.ApplicationName;
            extendedProperties[Constants.Logging.ApplicationToken] = BrokerContext.Current.ApplicationToken;
            extendedProperties[Constants.Logging.UserToken] = BrokerContext.Current.UserToken;
            extendedProperties[Constants.Logging.UserName] = BrokerContext.Current.UserName;
            extendedProperties[Constants.Logging.MethodName] = methodName;
            extendedProperties[Constants.Logging.DataObjectType] = objectType;
            extendedProperties[Constants.Logging.DataObjectXml] = xml;

            var keys = extendedProperties.Keys.ToArray();
            foreach (var key in keys)
            {
                if (extendedProperties[key] == null)
                {
                    extendedProperties[key] = "";
                }
            }

            Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(ent);
        }

    }
}
